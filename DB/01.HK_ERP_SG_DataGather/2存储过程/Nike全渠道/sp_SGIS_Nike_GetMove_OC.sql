IF EXISTS (SELECT *
           FROM sys.objects
           WHERE object_id = OBJECT_ID(N'[dbo].[sp_SGIS_Nike_GetMove_OC]')
                 AND type IN (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_SGIS_Nike_GetMove_OC];
GO

/* ==========================================================================================
描    述：Nike全渠道调拨单数据
创 建 人：lcj
创建日期：2023-08-24
说明：
更新说明：
-- ==========================================================================================*/
CREATE PROCEDURE [dbo].[sp_SGIS_Nike_GetMove_OC]
    @VendorID INT = -1,          -- 供应商ID
    @GatType INT = 1,            -- 采集类型，1：正常采集；0：补采
    @CompanyID VARCHAR(20) = '', -- 公司ID 
    @CardID VARCHAR(20) = '',    -- 品牌ID 
    @StartTime DATETIME,         -- 补采时开始时间
    @EndTime DATETIME,           -- 补采结束时间
    @DateNow DATETIME,           -- 当前开始采集的时间
    @IsMC INT = 0                -- 是否月结数据
AS
BEGIN

    IF OBJECT_ID('tempdB..#ShopStock') IS NOT NULL
        DROP TABLE #ShopStock;

    CREATE TABLE #ShopStock
    (
        CompanyID VARCHAR(50) NULL,
        ShopID VARCHAR(50) NULL,
        ShopCode VARCHAR(50) NULL,      -- 仓库代码
        ShopName NVARCHAR(100) NULL,
        StockID VARCHAR(50) NULL,       -- 仓库代码
        StockCode VARCHAR(50) NULL,     -- 仓库代码
        StockName NVARCHAR(100) NULL,
        StockType INT NULL,             -- 仓库类型 0仓库 1店铺
        IsShop BIT NULL                 -- 是否店铺 1店铺 0大仓
    ); -- 提报店铺

	DECLARE @RepeatStartTime DATETIME = GETDATE(); --开始同步时间
    DECLARE @Start DATETIME = GETDATE();   -- 本次捞取数据的开始时间
    DECLARE @End DATETIME = GETDATE();     -- 本次捞取数据的结束时间
    DECLARE @HisDays INT = 60;             -- 历史数据保留60天

    DECLARE @IsAuto BIT = 1;               -- 提取方案方式 1：自动 0：手动
    DECLARE @SolutionID INT = 0;           -- 提报方案ID
    DECLARE @DrewTime VARCHAR(20);         -- 提报时间点

    -- ============================================================
    -- 店仓
    -- ============================================================
    DELETE FROM #ShopStock; -- 清空上一次循环的数据（取店铺）

    INSERT INTO #ShopStock
    (
        ShopID
    )
    SELECT DISTINCT
        ShopID
    FROM HK_ERP_SG_DataGather.dbo.Gat_ShopRight
    WHERE AllowUsed = 1
          AND CompanyID = @CompanyID
          AND CardID = @CardID
          AND ISNULL(VendorID, -2) = @VendorID;

    UPDATE a
    SET a.CompanyID = @CompanyID
    FROM #ShopStock a;

    UPDATE a
    SET a.ShopCode = b.ShopCode,
        a.ShopName = b.ShopName,
        a.StockID = b.StockID,
        a.StockCode = c.StockCode,
        a.StockName = c.StockName,
        a.StockType = c.StockType,
        a.IsShop = 1
    FROM #ShopStock a
        INNER JOIN dbo.Bas_Shop b
            ON b.CompanyID = a.CompanyID
               AND b.ShopID = a.ShopID
        LEFT JOIN dbo.Bas_Stock c
            ON c.StockID = b.StockID
               AND c.CompanyID = b.CompanyID;

    -- ============================================================
    -- 大仓
    -- ============================================================
    INSERT INTO #ShopStock
    (
        CompanyID,
        ShopID,
        ShopCode,       -- 仓库代码
        ShopName,
        StockID,        -- 仓库代码
        StockCode,      -- 仓库代码
        StockName,
        StockType,      -- 仓库类型 0仓库 1店铺
        IsShop          -- 是否店铺 1店铺 0大仓
    )
    SELECT DISTINCT
        a.CompanyID,
        '' AS ShopID,
        '' AS ShopCode,
        '' AS ShopName,
        a.StockID,
        b.StockCode,
        b.StockName,
        b.StockType,
        0 AS IsShop
    FROM HK_ERP_SG_DataGather.dbo.Gat_StockRight a
        LEFT JOIN dbo.Bas_Stock b
            ON b.StockID = a.StockID
               AND b.CompanyID = a.CompanyID
    WHERE a.CompanyID = @CompanyID
          AND ISNULL(a.VendorID, -2) = @VendorID
          AND a.CardID = @CardID
          AND NOT EXISTS
    (
        SELECT TOP 1 1
        FROM #ShopStock b
        WHERE b.StockID = a.StockID
    );

    -- ============================================================
    -- ①. 先获取方案内容
    -- ============================================================
    SELECT TOP 1
        @IsAuto = ISNULL(IsAuto, 1),
		@RepeatStartTime = ISNULL(RepeatStartTime, GETDATE()),
        @SolutionID = ID
    FROM HK_ERP_SG_DataGather.dbo.Gat_Solution
    WHERE ISNULL(VendorID, -2) = @VendorID
          AND CompanyID = @CompanyID
          AND CardID = @CardID;

    -- ============================================================
    -- ②. 设置时间范围
    -- ============================================================
    IF @IsMC = 0
    BEGIN
        IF @GatType = 0 -- 补采
        BEGIN
            SET @Start = @StartTime;
            SET @End = @EndTime;
        END;
        ELSE
        BEGIN
         Set @End = @DateNow
		 If Not Exists (
				Select Top 1 1 From HK_ERP_SG_DataGather.dbo.Sys_Log  Where IsSucceed = 1 And LogType = 1 And ISNULL(VendorID,0) = @VendorID AND CompanyID = @CompanyID AND CardID = @CardID
			) --在这之前没有插入过数据
			Begin
				Select Top 1 @Start = RepeatStartTime 
				From	HK_ERP_SG_DataGather.dbo.Gat_Solution 
				Where	IsNULL(VendorID,-2) = @VendorID And CompanyID = @CompanyID And CardID = @CardID
			End
		Else
		Begin
			Select  @Start = MAX(ModifyDTM) 
			From	HK_ERP_SG_DataGather.dbo.Sys_Log 
			Where	IsSucceed = 1 And LogType = 1 AND IsNULL(VendorID,-2) = @VendorID And CompanyID = @CompanyID AND CardID = @CardID--成功提取数据的日志
		End
        END;
    END;
    ELSE
    BEGIN
        SET @Start = @StartTime;
        SET @End = @EndTime;
    END;

	IF @Start < @RepeatStartTime
    BEGIN
      SELECT @Start = @RepeatStartTime;
    END;

    -- ============================================================
    -- ③. 创建临时表 #Move
    -- ============================================================
    IF OBJECT_ID('tempdb..#Move', 'U') IS NOT NULL
        DROP TABLE #Move;

    CREATE TABLE #Move
    (
        [id] [int] IDENTITY(1, 1) NOT NULL,
        [SolutionID] [int] NULL,
        [CompanyID] [varchar](40) NULL,
        [CompanyName] [nvarchar](100) NULL,
        [CardID] [varchar](40) NULL,
        [CardName] [nvarchar](60) NULL,
        [BillNo] [varchar](40) NULL,            -- Movement_Document_Number 出入库移动凭证 出入库同一张单据
		[Sequence] [varchar](10) NOT NULL,
        [BillDate] [datetime] NULL,
        [MaterialCode] [VARCHAR](50) NULL,      -- PRODUCT_ID 货号
        [MaterialName] [NVARCHAR](200) NULL,    -- PRODUCT_NM 商品名称
        [BarCode] [VARCHAR](60) NULL,           -- UPC 条形码
        [SizeCode] [VARCHAR](40) NULL,          -- SIZE_CODE 尺码
        [SizeName] [NVARCHAR](40) NULL,
        [OutStockCode] [varchar](50) NULL,      -- Departure_SITE_CODE 发货店/仓编码
        [OutStockName] [nvarchar](100) NULL,
        [InStockCode] [varchar](50) NULL,       -- Destination_SITE_CODE 目的店/仓编码
        [InStockName] [nvarchar](100) NULL,
        [Qty] [numeric](10, 2) NULL,            -- Quantity 数量 出入库件数
        [MovementType] [varchar](10) NULL,      -- 货物移动类型
        [StockTransferStatus] [varchar](100) NULL, -- Stock_Transfer_Status 商品调拨状态
        [ModifyTime] [datetime] NULL,           -- transfer_dt 出入库日期
        [IsImport] BIT NULL DEFAULT (0),
        [BillTypeID] [VARCHAR](20) NULL,
        [BillTypeName] [NVARCHAR](40) NULL      -- 单据类型名称
    );

    -- ============================================================
    -- ④. 调拨出库单
    -- ============================================================
    BEGIN
        INSERT INTO #Move
        (
            SolutionID,
            CompanyID,
            CompanyName,
            CardID,
            CardName,
            BillNo,
			Sequence,
            BillDate,
            MaterialCode,
            MaterialName,
            BarCode,
            SizeCode,
            SizeName,
            OutStockCode,
            OutStockName,
            InStockCode,
            InStockName,
            Qty,
            MovementType,
            StockTransferStatus,
            ModifyTime,
            IsImport,
            BillTypeID,
            BillTypeName
        )
        SELECT
            @SolutionID AS SolutionID,          -- 方案ID
            a.CompanyID,                        -- 公司ID
            j.CompanyName,
            @CardID AS CardID,                  -- 品牌
            i.CardName,
            b.BillNo,                           -- 单号
			a.Sequence,
            m.OutCheckDate AS BillDate,
            c.MaterialCode,                     -- 货号
            c.MaterialShortName MaterialName,
            ms.BarCode,                         -- 条码
            f.SizeCode,                         -- 尺码
            f.SizeName,                         -- 尺码名称
            dd.StockCode AS OutStockCode,       -- 出库仓库代码
            dd.StockName AS OutStockName,       -- 出库仓库名称
            k.StockCode AS InStockCode,         -- 入库仓库代码
            k.StockName AS InStockName,         -- 入库仓库名称
            a.OutQty AS Qty,                    -- 数量
            N'1' AS MovementType,
            N'InTransit' AS StockTransferStatus,
            NULL AS ModifyTime,
            0 AS IsImport,
            'Inv_MovOut' AS BillTypeID,         -- 单据类型
            '调拨出库单' AS BillTypeName          -- 单据类型名称
        FROM dbo.SD_Inv_MoveSize a
            INNER JOIN dbo.SD_Inv_MoveDetail b
                ON b.CompanyID = a.CompanyID
                   AND b.BillNo = a.BillNo
                   AND b.Sequence = a.Sequence
            INNER JOIN dbo.SD_Inv_MoveMaster m
                ON a.CompanyID = m.CompanyID
                   AND a.BillNo = m.BillNo
            INNER JOIN dbo.SD_Mat_Material c
                ON a.MaterialID = c.MaterialID
            INNER JOIN dbo.SD_Mat_Size f
                ON a.SizeID = f.SizeID
            LEFT JOIN dbo.Bas_Company j
                ON a.CompanyID = j.CompanyID
            LEFT JOIN dbo.SD_Mat_Card i
                ON c.CardID = i.CardID
            LEFT JOIN dbo.Bas_Stock dd
                ON dd.CompanyID = m.CompanyID
                   AND dd.StockID = m.OutStockID
            LEFT JOIN dbo.Bas_Stock k
                ON k.CompanyID = m.CompanyID
                   AND k.StockID = m.InStockID
            JOIN dbo.SD_Mat_MaterialSize ms
                ON ms.SizeID = a.SizeID
                   AND ms.MaterialID = a.MaterialID
        WHERE m.CompanyID = @CompanyID
              AND c.CardID = @CardID
              AND EXISTS
        (
            SELECT TOP 1 1
            FROM #ShopStock d
            WHERE d.StockID IN (m.OutStockID, m.InStockID)
                  AND a.CompanyID = d.CompanyID
        )
              AND m.OutCheckDate > @Start
              AND m.OutCheckDate <= @End
              AND m.OutBillStatus = 4;
    END;

    -- ============================================================
    -- ⑤. 调拨入库单
    -- ============================================================
    BEGIN
        INSERT INTO #Move
        (
            SolutionID,
            CompanyID,
            CompanyName,
            CardID,
            CardName,
            BillNo,
			Sequence,
            BillDate,
            MaterialCode,
            MaterialName,
            BarCode,
            SizeCode,
            SizeName,
            OutStockCode,
            OutStockName,
            InStockCode,
            InStockName,
            Qty,
            MovementType,
            StockTransferStatus,
            ModifyTime,
            IsImport,
            BillTypeID,
            BillTypeName
        )
        SELECT
            @SolutionID AS SolutionID,          -- 方案ID
            a.CompanyID,                        -- 公司ID
            j.CompanyName,
            @CardID AS CardID,                  -- 品牌
            i.CardName,
            b.BillNo,                           -- 单号
			a.Sequence,
            m.InCheckDate AS BillDate,
            c.MaterialCode,                     -- 货号
            c.MaterialShortName MaterialName,
            ms.BarCode,                         -- 条码
            f.SizeCode,                         -- 尺码
            f.SizeName,                         -- 尺码名称
            dd.StockCode AS OutStockCode,       -- 出库仓库代码
            dd.StockName AS OutStockName,       -- 出库仓库名称
            k.StockCode AS InStockCode,         -- 入库仓库代码
            k.StockName AS InStockName,         -- 入库仓库名称
            a.InQty AS Qty,                     -- 数量
            N'2' AS MovementType,
            N'Complete' AS StockTransferStatus,
            NULL AS ModifyTime,
            0 AS IsImport,
            'Inv_MovIn' AS BillTypeID,          -- 单据类型
            '调拨入库单' AS BillTypeName          -- 单据类型名称
        FROM dbo.SD_Inv_MoveSize a
            INNER JOIN dbo.SD_Inv_MoveDetail b
                ON b.CompanyID = a.CompanyID
                   AND b.BillNo = a.BillNo
                   AND b.Sequence = a.Sequence
            INNER JOIN dbo.SD_Inv_MoveMaster m
                ON a.CompanyID = m.CompanyID
                   AND a.BillNo = m.BillNo
            INNER JOIN dbo.SD_Mat_Material c
                ON a.MaterialID = c.MaterialID
            INNER JOIN dbo.SD_Mat_Size f
                ON a.SizeID = f.SizeID
            LEFT JOIN dbo.Bas_Company j
                ON a.CompanyID = j.CompanyID
            LEFT JOIN dbo.SD_Mat_Card i
                ON c.CardID = i.CardID
            LEFT JOIN dbo.Bas_Stock dd
                ON dd.CompanyID = m.CompanyID
                   AND dd.StockID = m.OutStockID
            LEFT JOIN dbo.Bas_Stock k
                ON k.CompanyID = m.CompanyID
                   AND k.StockID = m.InStockID
            JOIN dbo.SD_Mat_MaterialSize ms
                ON ms.SizeID = a.SizeID
                   AND ms.MaterialID = a.MaterialID
        WHERE m.CompanyID = @CompanyID
              AND c.CardID = @CardID
              AND EXISTS
        (
            SELECT TOP 1 1
            FROM #ShopStock d
            WHERE d.StockID IN (m.OutStockID, m.InStockID)
                  AND a.CompanyID = d.CompanyID
        )
              AND m.InCheckDate > @Start
              AND m.InCheckDate <= @End
              AND m.InBillStatus = 4;
    END;

    -- ============================================================
    -- ⑥. 写入正式表
    -- ============================================================
    IF @IsMC = 0
    BEGIN
        -- 删除指定范围之内的数据，防止补采产生重复数据
        DELETE a
        FROM HK_ERP_SG_DataGather.dbo.Gat_Nike_Move_OC a
            INNER JOIN #Move b
                ON b.SolutionID = a.SolutionID
                   AND b.CompanyID = a.CompanyID
                   AND b.BillNo = a.BillNo
                   AND b.BillTypeID = a.BillTypeID
                   AND b.CardID = a.CardID
        WHERE a.CompanyID = @CompanyID
              AND a.CardID = @CardID;

        -- 删除历史数据
        DELETE FROM HK_ERP_SG_DataGather.dbo.Gat_Nike_Move_OC
        WHERE CompanyID = @CompanyID
              AND CardID = @CardID
              AND ModifyTime <= DATEADD(DAY, @HisDays * -1, CONVERT(VARCHAR(10), GETDATE(), 120));

        INSERT INTO HK_ERP_SG_DataGather.dbo.Gat_Nike_Move_OC
        (
            SolutionID,
            CompanyID,
            CompanyName,
            CardID,
            CardName,
            BillNo,
			Sequence,
            BillDate,
            MaterialCode,
            MaterialName,
            BarCode,
            SizeCode,
            SizeName,
            OutStockCode,
            OutStockName,
            InStockCode,
            InStockName,
            Qty,
            MovementType,
            StockTransferStatus,
            ModifyTime,
            IsImport,
            BillTypeID,
            BillTypeName
        )
        SELECT
            SolutionID,
            CompanyID,
            CompanyName,
            CardID,
            CardName,
            BillNo,
			Sequence,
            BillDate,
            MaterialCode,
            MaterialName,
            BarCode,
            SizeCode,
            SizeName,
            OutStockCode,
            OutStockName,
            InStockCode,
            InStockName,
            Qty,
            MovementType,
            StockTransferStatus,
            GETDATE() AS ModifyTime,
            0 AS IsImport,
            BillTypeID,
            BillTypeName
        FROM #Move;
    END;

END;
GO
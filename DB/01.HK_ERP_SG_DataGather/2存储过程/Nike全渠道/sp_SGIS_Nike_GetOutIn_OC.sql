IF EXISTS (SELECT *
           FROM sys.objects
           WHERE object_id = OBJECT_ID(N'[dbo].[sp_SGIS_Nike_GetOutIn_OC]')
                 AND type IN (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_SGIS_Nike_GetOutIn_OC];
GO

/* ==========================================================================================
描    述：Nike全渠道出入库单数据
创 建 人：lcj
创建日期：
说明：
更新说明：
-- ==========================================================================================*/
CREATE PROCEDURE [dbo].[sp_SGIS_Nike_GetOutIn_OC]
    @VendorID INT = -1,              -- 供应商ID
    @GatType INT = 1,                -- 采集类型，1：正常采集；0：补采
    @CompanyID VARCHAR(20) = '',     -- 公司ID 
    @CardID VARCHAR(20) = '',        -- 品牌ID 
    @StartTime DATETIME,             -- 补采时开始时间
    @EndTime DATETIME,               -- 补采结束时间
    @DateNow DATETIME,               -- 当前开始采集的时间
    @IsMC INT = 0                    -- 是否三小时数据
AS
BEGIN

    IF OBJECT_ID('tempdB..#ShopStock') IS NOT NULL
        DROP TABLE #ShopStock;

    -- 提报店铺
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
    );
	DECLARE @RepeatStartTime DATETIME = GETDATE(); --开始同步时间
    DECLARE @Start DATETIME = GETDATE();    -- 本次捞取数据的开始时间
    DECLARE @End DATETIME = GETDATE();      -- 本次捞取数据的结束时间
    DECLARE @HisDays INT = 180;             -- 历史数据保留60天
    DECLARE @IsAuto BIT = 1;                -- 提取方案方式 1：自动 0：手动
    DECLARE @SolutionID INT = 0;            -- 提报方案ID
    DECLARE @DrewTime VARCHAR(20);          -- 提报时间点

    -- ============================================================
    -- 店仓
    -- ============================================================
    DELETE FROM #ShopStock;   -- 清空上一次循环的数据（取店铺）

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
    -- ③. 创建临时表 #OutIn
    -- ============================================================
    IF OBJECT_ID('tempdb..#OutIn', 'U') IS NOT NULL
        DROP TABLE #OutIn;

    CREATE TABLE #OutIn
    (
        [id] INT IDENTITY(1, 1) NOT NULL,
        [SolutionID] INT NULL,
        [CompanyID] VARCHAR(40) NULL,
        [CompanyName] NVARCHAR(100) NULL,
        [CardID] VARCHAR(40) NULL,
        [CardName] NVARCHAR(60) NULL,
        [BillNo] VARCHAR(40) NULL,          -- Movement_Document_Number 出入库移动凭证 出入库同一张单据
		[Sequence] [varchar](10) NOT NULL,
        [BillDate] DATETIME NULL,
        --[DrewTime] VARCHAR(20) NULL,        -- transfer_dt 出入库日期
        [MaterialCode] VARCHAR(50) NULL,    -- PRODUCT_ID 货号
        [MaterialName] NVARCHAR(200) NULL,  -- PRODUCT_NM 商品名称
        [BarCode] VARCHAR(60) NULL,         -- UPC 条形码
        [SizeCode] VARCHAR(40) NULL,        -- SIZE_CODE 尺码
        [SizeName] NVARCHAR(40) NULL,
        [OutStockCode] VARCHAR(50) NULL,    -- Departure_SITE_CODE 发货店/仓编码
        [OutStockName] NVARCHAR(100) NULL,
        [InStockCode] VARCHAR(50) NULL,     -- Destination_SITE_CODE 目的店/仓编码
        [InStockName] NVARCHAR(100) NULL,
        [Qty] NUMERIC(10, 2) NULL,          -- Quantity 数量 出入库件数
        [Amount] DECIMAL(24, 6) NULL,       -- 金额
        [MovementType] VARCHAR(10) NULL,    -- 货物移动类型
        [PackingListNumber] NVARCHAR(1000) NULL, -- 装箱单号
        [ModifyTime] DATETIME NULL,
        [IsImport] BIT NULL DEFAULT (0),
        BillTypeID VARCHAR(20) NULL,
        BillTypeName NVARCHAR(40) NULL,      -- 单据类型名称
		VendCustID VARCHAR(20) NULL,
    );

    -- ============================================================
    -- ④. 插入出入库数据
    -- ============================================================

    BEGIN
        -- 采购入库单 采购退货出库单 销售出库单 销售退货入库单
        INSERT INTO #OutIn
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
            Amount,
            MovementType,
            PackingListNumber,
            ModifyTime,
            IsImport,
            BillTypeID,
            BillTypeName,
			VendCustID
        )
        SELECT
            @SolutionID AS SolutionID,          -- 方案ID
            a.CompanyID,                        -- 公司ID
            j.CompanyName AS CompanyName,
            @CardID AS CardID,                  -- 品牌
            i.CardName AS CardName,
            b.BillNo,
			a.Sequence,
            m.CheckDate AS BillDate,            -- 销售日期
            c.MaterialCode,                     -- 货号
            c.MaterialShortName MaterialName,
            ms.BarCode,                         -- 条码
            f.SizeCode,                         -- 尺码
            f.SizeName,                         -- 尺码名称
            -- 出库仓库代码
            CASE
                WHEN m.BillTypeID IN ('Inv_Ssck', 'Inv_Thck') THEN d.StockCode
                ELSE N''
            END AS OutStockCode,
            -- 出库仓库名称
            CASE
                WHEN m.BillTypeID IN ('Inv_Ssck', 'Inv_Thck') THEN d.StockName
                ELSE N''
            END AS OutStockName,
            -- 入库仓库代码
            CASE
                WHEN m.BillTypeID IN ('Inv_Thrk', 'Inv_Cgrk') THEN d.StockCode
                ELSE N''
            END AS InStockCode,
            -- 入库仓库名称
            CASE
                WHEN m.BillTypeID IN ('Inv_Thrk', 'Inv_Cgrk') THEN d.StockName
                ELSE N''
            END AS InStockName,
            a.Qty,                              -- 数量
            a.Qty*b.BalaPrice AS Amount,
            (CASE
                WHEN m.BillTypeID = 'Inv_Ssck' THEN '1'   -- 销售出库单
                WHEN m.BillTypeID = 'Inv_Thrk' THEN '2'   -- 销售退货入库单
                WHEN m.BillTypeID = 'Inv_Thck' THEN '3'   -- 采购退货出库
                WHEN m.BillTypeID = 'Inv_Cgrk' THEN '4'   -- 采购入库
                ELSE ''
            END) AS MovementType,
            (CASE
                WHEN m.BillTypeID = 'Inv_Thck' THEN m.ManualBillNo   -- 采购退货出库
                WHEN m.BillTypeID = 'Inv_Cgrk' THEN m.Remark   -- 采购入库
                ELSE ''
            END)  AS PackingListNumber,
            NULL AS ModifyTime,
            0 AS IsImport,
            m.BillTypeID,
            bt.BillTypeName,                     -- 单据类型名称
			m.VendCustID
        FROM dbo.SD_Inv_TransSize a
            INNER JOIN dbo.SD_Inv_TransDetail b
                ON b.CompanyID = a.CompanyID
                   AND b.BillNo = a.BillNo
                   AND b.Sequence = a.Sequence
            INNER JOIN dbo.SD_Inv_TransMaster m
                ON a.CompanyID = m.CompanyID
                   AND a.BillNo = m.BillNo
            INNER JOIN dbo.SD_Mat_Material c
                ON a.MaterialID = c.MaterialID
            INNER JOIN #ShopStock d
                ON d.StockID = m.StockID
                   AND a.CompanyID = d.CompanyID
            INNER JOIN dbo.SD_Mat_Size f
                ON a.SizeID = f.SizeID
            INNER JOIN dbo.SD_Mat_MaterialSize ms
                ON ms.SizeID = a.SizeID
                   AND ms.MaterialID = a.MaterialID
            LEFT JOIN dbo.Bas_Company j
                ON a.CompanyID = j.CompanyID
            LEFT JOIN dbo.SD_Mat_Card i
                ON c.CardID = i.CardID
            LEFT JOIN dbo.Sys_BillType bt
                ON bt.BillTypeID = m.BillTypeID
        WHERE m.CompanyID = @CompanyID
              AND c.CardID = @CardID
              AND m.CheckDate > @Start
              AND m.CheckDate <= @End
              AND m.BillTypeID IN ('Inv_Cgrk', 'Inv_Thck', 'Inv_Ssck', 'Inv_Thrk')
              AND m.BillStatus = 4;
    END;


	UPDATE   a SET  a.InStockCode=b.VendCustCode,a.InStockName=b.VendCustName  FROM #OutIn a
	INNER JOIN Bas_InterCompany b ON b.VendCustID=a.VendCustID
	WHERE a.BillTypeID='Inv_Ssck'

    -- ============================================================
    -- ⑤. 写入正式表
    -- ============================================================
    SELECT @DrewTime = FORMAT(GETDATE(), 'yyyyMMddHH');

    IF @IsMC = 0
    BEGIN
        -- 删除2天前的历史数据
        DELETE a
        FROM HK_ERP_SG_DataGather.dbo.Gat_Nike_OutIn_OC a
            INNER JOIN #OutIn b
                ON b.CompanyID = a.CompanyID
                   AND b.BillTypeID = a.BillTypeID
                   AND b.BillNo = a.BillNo
        WHERE a.SolutionID = @SolutionID

        INSERT INTO HK_ERP_SG_DataGather.dbo.Gat_Nike_OutIn_OC
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
            Amount,
            MovementType,
            PackingListNumber,
            ModifyTime,
            IsImport,
            BillTypeID,
            BillTypeName
        )
        SELECT DISTINCT
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
            Amount,
            MovementType,
            PackingListNumber,
            GETDATE() ModifyTime,
            IsImport,
            BillTypeID,
            BillTypeName
        FROM #OutIn
		
    END;

END;
GO
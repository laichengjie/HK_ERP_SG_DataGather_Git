IF EXISTS
(
    SELECT *
    FROM sys.objects
    WHERE object_id = OBJECT_ID(N'[dbo].[sp_SGIS_IceBreaker_GetPosSale]')
          AND type IN ( N'P', N'PC' )
)
    DROP PROCEDURE [dbo].sp_SGIS_IceBreaker_GetPosSale;
GO

/* ==========================================================================================
描    述：拓冰者 POS数据
创 建 人：
创建日期：
说明：
更新说明：
-- ==========================================================================================*/
CREATE PROCEDURE [dbo].[sp_SGIS_IceBreaker_GetPosSale]
    @VendorID INT = -1,           --供应商ID
    @GatType INT = 1,             --采集类型，1：正常采集；0：补采
    @CompanyID NVARCHAR(20) = '', --公司ID 
    @CardID NVARCHAR(20) = '',    -- 品牌ID 
    @StartTime DATETIME,          -- 补采时开始时间
    @EndTime DATETIME,            --补采结束时间
    @DateNow DATETIME             --当前开始采集的时间
AS
BEGIN

    CREATE TABLE #Shop
    (
        ShopID NVARCHAR(50)
    ); --提报店铺
    CREATE TABLE #VendShop
    (
        -- 供应商店铺
        VendShopID INT NOT NULL, --
        ShopCode VARCHAR(50) NULL,
        ShopName VARCHAR(100) NULL,
        StockCode VARCHAR(50) NULL,
        StockName VARCHAR(100) NULL,
    );
    CREATE TABLE #ShopMap
    (
        -- 店铺关系1
        VendShopID INT NOT NULL,
        ShopID VARCHAR(20) NOT NULL,
        CompanyID VARCHAR(20)
    );
    DECLARE @RepeatStartTime DATETIME = GETDATE(); --开始同步时间
    DECLARE @Start DATETIME = GETDATE(); --本次捞取数据的开始时间
    DECLARE @End DATETIME = GETDATE(); --本次捞取数据的结束时间
    DECLARE @HisDays INT = 60; -- 历史数据保留60天

    DECLARE @IsAuto BIT = 1; --提取方案方式 1：自动：0:手动
    DECLARE @SolutionID INT = 0; --提报方案ID
    DECLARE @CardName NVARCHAR(40) = N'';
    DECLARE @CompanyName NVARCHAR(40) = N'';

    -- 供应商店铺
    INSERT INTO #VendShop
    SELECT ID,
           ShopCode,
           ShopName,
           StockCode,
           StockName
    FROM HK_ERP_SG_DataGather.dbo.Bas_VendShop
    WHERE VendorID = @VendorID;

    -- 店铺映射关系
    INSERT INTO #ShopMap
    SELECT VendShopID,
           ShopID,
           CompanyID
    FROM HK_ERP_SG_DataGather.dbo.Bas_ShopMap a
        INNER JOIN HK_ERP_SG_DataGather.dbo.Bas_VendShop b
            ON a.VendShopID = b.ID
    WHERE b.VendorID = @VendorID;

    DELETE FROM #Shop; --清空上一次循环的数据（取店铺）
    INSERT INTO #Shop
    SELECT DISTINCT
           ShopID
    FROM HK_ERP_SG_DataGather.dbo.Gat_ShopRight
    WHERE AllowUsed = 1
          AND CompanyID = @CompanyID
          AND CardID = @CardID
          AND ISNULL(VendorID, -2) = @VendorID;
    --①.先获取方案内容
    SELECT TOP 1
           @IsAuto = ISNULL(IsAuto, 1),
           @RepeatStartTime = ISNULL(RepeatStartTime, GETDATE()),
           @SolutionID = ID
    FROM HK_ERP_SG_DataGather.dbo.Gat_Solution
    WHERE ISNULL(VendorID, -2) = @VendorID
          AND CompanyID = @CompanyID
          AND CardID = @CardID;

    --不同步货品范围
    IF OBJECT_ID('tempdB..#MaterialRange') IS NOT NULL
        DROP TABLE #MaterialRange;
    CREATE TABLE #MaterialRange
    (
        MaterialCode VARCHAR(60)
    );
    INSERT INTO #MaterialRange
    (
        MaterialCode
    )
    SELECT MaterialCode
    FROM HK_ERP_SG_DataGather.dbo.Gat_MaterialRange
    WHERE SolutionId = @SolutionID;

    SELECT TOP 1
           @CardName = CardName
    FROM HK_ERP_SG_DataGather.dbo.Bas_Card
    WHERE CardID = @CardID;
    SELECT TOP 1
           @CompanyName = CompanyName
    FROM HK_ERP_SG_DataGather.dbo.Bas_Company
    WHERE CompanyID = @CompanyID;

    IF @GatType = 0 --补采
    BEGIN
        SET @Start = @StartTime;
        SET @End = @EndTime;
    END;
    ELSE
    BEGIN
        SET @End = @DateNow;
        IF NOT EXISTS
        (
            SELECT TOP 1
                   1
            FROM HK_ERP_SG_DataGather.dbo.Sys_Log
            WHERE IsSucceed = 1
                  AND LogType = 1
                  AND ISNULL(VendorID, 0) = @VendorID
                  AND CompanyID = @CompanyID
                  AND CardID = @CardID
        ) --在这之前没有插入过数据
        BEGIN
            SELECT TOP 1
                   @Start = RepeatStartTime
            FROM HK_ERP_SG_DataGather.dbo.Gat_Solution
            WHERE ISNULL(VendorID, -2) = @VendorID
                  AND CompanyID = @CompanyID
                  AND CardID = @CardID;
        END;
        ELSE
        BEGIN
            SELECT @Start = MAX(ModifyDTM)
            FROM HK_ERP_SG_DataGather.dbo.Sys_Log
            WHERE IsSucceed = 1
                  AND LogType = 1
                  AND ISNULL(VendorID, -2) = @VendorID
                  AND CompanyID = @CompanyID
                  AND CardID = @CardID; --成功提取数据的日志
        END;
    END;

    IF @Start < @RepeatStartTime
    BEGIN
        SELECT @Start = @RepeatStartTime;
    END;

    IF OBJECT_ID('tempdB..#PosSale') IS NOT NULL
        DROP TABLE #PosSale;
    CREATE TABLE #PosSale
    (
        [SolutionID] [INT] NOT NULL,
        [CardID] [VARCHAR](20) NULL,
        [CardName] [VARCHAR](20) NULL,
        [CompanyID] [VARCHAR](20) NOT NULL,
        [CompanyName] [NVARCHAR](50) NULL,
        [ShopID] [NVARCHAR](50) NOT NULL,
        [ShopCode] [VARCHAR](50) NOT NULL,
        [ShopName] [NVARCHAR](100) NULL,
        [BillNo] [VARCHAR](40) NOT NULL,
        [SourceBillNo] [VARCHAR](40) NOT NULL,
        [BillDate] [DATETIME] NOT NULL,
        [BillTime] [DATETIME] NULL,
        [MaterialCode] [NVARCHAR](50) NOT NULL,
        [BarCode] [VARCHAR](100) NOT NULL,
        [Qty] [DECIMAL](24, 6) NULL,
        [RetailPrice] [DECIMAL](24, 6) NULL, -- 吊牌价 取零售价
        [PayAmount] [DECIMAL](24, 6) NULL,   --业绩金额 取 结算金额
        [IsImport] [BIT] NULL,
        [ModifyDtm] [DATETIME] NULL,
		SizeName NVARCHAR(50)  NULL
    );
    INSERT INTO #PosSale
    (
        SolutionID,
        CardID,
        CardName,
        CompanyID,
        CompanyName,
        ShopID,
        ShopCode,
        ShopName,
        BillNo,
        SourceBillNo,
        BillDate,
        BillTime,
        MaterialCode,
        BarCode,
        Qty,
        RetailPrice,
        PayAmount,
        IsImport,
        ModifyDtm,
		SizeName
    )
    SELECT @SolutionID SolutionID,
           @CardID CardID,
           @CardName CardName,
           @CompanyID CompanyID,
           @CompanyName CompanyName,
           b.ShopID,
           ISNULL(h.ShopCode, d.ShopCode),
           ISNULL(h.ShopName, d.ShopName),
           b.BillNo,
           a.SourceBillNo SourceBillNo,
           b.BillDate,
           b.BillTime,
           c.MaterialCode,
           a.BarCode,
           a.Qty,
           a.Price RetailPrice,
           a.PayAmount,
           0 IsImport,
           GETDATE() ModifyDtm,
		   f.SizeName
    FROM SD_POS_SaleDetail a
        INNER JOIN SD_POS_SaleMaster b
            ON a.CompanyID = b.CompanyID
               AND a.BillNo = b.BillNo
        INNER JOIN SD_Mat_Material c
            ON a.MaterialID = c.MaterialID
        INNER JOIN Bas_Shop d
            ON a.ShopID = d.ShopID
               AND a.CompanyID = d.CompanyID
        INNER JOIN SD_Bas_ShopCard e
            ON d.CompanyID = e.CompanyID
               AND d.ShopID = e.ShopID
               AND c.CardID = e.CardID
        INNER JOIN #Shop k
            ON a.ShopID = k.ShopID
        INNER JOIN SD_Mat_Size f
            ON a.SizeID = f.SizeID
        LEFT JOIN Bas_Company j
            ON a.CompanyID = j.CompanyID
        LEFT JOIN SD_Mat_Card i
            ON c.CardID = i.CardID
        LEFT JOIN #ShopMap g
            ON a.CompanyID = g.CompanyID
               AND a.ShopID = g.ShopID -- 关联到店铺映射
        LEFT JOIN #VendShop h
            ON g.VendShopID = h.VendShopID
    WHERE a.CompanyID = @CompanyID
          AND e.CardID = @CardID
          AND b.BillTime > @Start
          AND b.BillTime <= @End
    UNION ALL
    SELECT @SolutionID SolutionID,
           @CardID CardID,
           @CardName CardName,
           @CompanyID CompanyID,
           @CompanyName CompanyName,
           b.ShopID,
           ISNULL(h.ShopCode, d.ShopCode),
           ISNULL(h.ShopName, d.ShopName),
           b.BillNo,
           a.SourceBillNo SourceBillNo,
           b.BillDate,
           b.BillTime,
           c.MaterialCode,
           a.BarCode,
           a.Qty,
           a.Price RetailPrice,
           a.PayAmount,
           0 IsImport,
           GETDATE() ModifyDtm,
		   f.SizeName
    FROM SD_POS_SaleDetail2 a
        INNER JOIN SD_POS_SaleMaster2 b
            ON a.CompanyID = b.CompanyID
               AND a.BillNo = b.BillNo
        INNER JOIN SD_Mat_Material c
            ON a.MaterialID = c.MaterialID
        INNER JOIN Bas_Shop d
            ON a.ShopID = d.ShopID
               AND a.CompanyID = d.CompanyID
        INNER JOIN SD_Bas_ShopCard e
            ON d.CompanyID = e.CompanyID
               AND d.ShopID = e.ShopID
               AND c.CardID = e.CardID
        INNER JOIN #Shop k
            ON a.ShopID = k.ShopID
        INNER JOIN SD_Mat_Size f
            ON a.SizeID = f.SizeID
        LEFT JOIN Bas_Company j
            ON a.CompanyID = j.CompanyID
        LEFT JOIN SD_Mat_Card i
            ON c.CardID = i.CardID
        LEFT JOIN #ShopMap g
            ON a.CompanyID = g.CompanyID
               AND a.ShopID = g.ShopID -- 关联到店铺映射
        LEFT JOIN #VendShop h
            ON g.VendShopID = h.VendShopID
    WHERE a.CompanyID = @CompanyID
          AND e.CardID = @CardID
          AND b.BillTime > @Start
          AND b.BillTime <= @End;

    --删除指定范围之内的数据，防止补采产生重复数据
    DELETE a
    FROM HK_ERP_SG_DataGather.dbo.Gat_IceBreaker_PosSale a
        INNER JOIN #PosSale b
            ON b.CompanyID = a.CompanyID
               AND b.BillNo = a.BillNo
               AND b.SolutionID = a.SolutionID;


    DELETE a
    FROM HK_ERP_SG_DataGather.dbo.Gat_IceBreaker_PosSale a
    WHERE a.SolutionID = @SolutionID
          AND BillTime > @Start
          AND BillTime <= @End;
    --删除历史数据
    DELETE a
    FROM HK_ERP_SG_DataGather.dbo.Gat_IceBreaker_PosSale a
    WHERE a.SolutionID = @SolutionID
          AND BillTime <= DATEADD(DAY, @HisDays * -1, CONVERT(VARCHAR(10), GETDATE(), 120));


    --删除不同步货号
    DELETE a
    FROM HK_ERP_SG_DataGather.dbo.Gat_IceBreaker_PosSale a
        INNER JOIN #MaterialRange b
            ON b.MaterialCode = a.MaterialCode;


    IF (@IsAuto = 0) --手动数据去到中转库
    BEGIN
        -- 开始插入数据
        INSERT INTO HK_ERP_SG_DataGather.dbo.Gat_IceBreaker_PosSale_transfer
        (
            SolutionID,
            CardID,
            CardName,
            CompanyID,
            CompanyName,
            ShopID,
            ShopCode,
            ShopName,
            BillNo,
            SourceBillNo,
            BillDate,
            BillTime,
            MaterialCode,
            BarCode,
            Qty,
            RetailPrice,
            PayAmount,
            IsImport,
            ModifyDtm,
			SizeName
        )
        SELECT SolutionID,
               CardID,
               CardName,
               CompanyID,
               CompanyName,
               ShopID,
               ShopCode,
               ShopName,
               BillNo,
               SourceBillNo,
               BillDate,
               BillTime,
               MaterialCode,
               BarCode,
               Qty,
               RetailPrice,
               PayAmount,
               IsImport,
               ModifyDtm,
			   SizeName
        FROM #PosSale;
    END;
    IF (@IsAuto = 1) --自动数据去到真实库
    BEGIN
        -- 开始插入数据
        INSERT INTO HK_ERP_SG_DataGather.dbo.Gat_IceBreaker_PosSale
        (
            SolutionID,
            CardID,
            CardName,
            CompanyID,
            CompanyName,
            ShopID,
            ShopCode,
            ShopName,
            BillNo,
            SourceBillNo,
            BillDate,
            BillTime,
            MaterialCode,
            BarCode,
            Qty,
            RetailPrice,
            PayAmount,
            IsImport,
            ModifyDtm,
			SizeName
        )
        SELECT SolutionID,
               CardID,
               CardName,
               CompanyID,
               CompanyName,
               ShopID,
               ShopCode,
               ShopName,
               BillNo,
               SourceBillNo,
               BillDate,
               BillTime,
               MaterialCode,
               BarCode,
               Qty,
               RetailPrice,
               PayAmount,
               IsImport,
               ModifyDtm,
			   SizeName
        FROM #PosSale;
    END;
    TRUNCATE TABLE #Shop;
    TRUNCATE TABLE #VendShop;
    TRUNCATE TABLE #ShopMap;
    DROP TABLE #Shop;
    DROP TABLE #ShopMap;
    DROP TABLE #VendShop;


END;



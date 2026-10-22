IF EXISTS
(
    SELECT *
    FROM sys.objects
    WHERE object_id = OBJECT_ID(N'[dbo].[sp_SGIS_IceBreaker_GetMaterailINV]')
          AND type IN ( N'P', N'PC' )
)
    DROP PROCEDURE [dbo].sp_SGIS_IceBreaker_GetMaterailINV;
GO

/* ==========================================================================================
描    述：
创 建 人：
创建日期：
说明：
更新说明：
-- ==========================================================================================*/
CREATE PROCEDURE [dbo].[sp_SGIS_IceBreaker_GetMaterailINV]
    @VendorID INT = -1,           --供应商ID
    @GatType INT = 1,             --采集类型，1：正常采集；0：补采
    @CompanyID NVARCHAR(20) = '', --公司ID 
    @CardID NVARCHAR(20) = '',    -- 品牌ID 
    @StartTime DATETIME,          -- 补采时开始时间
    @EndTime DATETIME,            --补采结束时间
    @DateNow DATETIME             --当前开始采集的时间
AS
BEGIN
    DECLARE @CardName NVARCHAR(40) = N'';
    DECLARE @CompanyName NVARCHAR(40) = N'';

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

    -- 供应商店铺
    INSERT INTO #VendShop
    (
        VendShopID,
        ShopCode,
        ShopName,
        StockCode,
        StockName
    )
    SELECT ID,
           ShopCode,
           ShopName,
           StockCode,
           StockName
    FROM HK_ERP_SG_DataGather.dbo.Bas_VendShop
    WHERE VendorID = @VendorID;

    -- 店铺映射关系
    INSERT INTO #ShopMap
    (
        VendShopID,
        ShopID,
        CompanyID
    )
    SELECT VendShopID,
           ShopID,
           CompanyID
    FROM HK_ERP_SG_DataGather.dbo.Bas_ShopMap a
        INNER JOIN HK_ERP_SG_DataGather.dbo.Bas_VendShop b
            ON a.VendShopID = b.ID
    WHERE b.VendorID = @VendorID;

    --①.先获取方案内容
    DECLARE @SolutionID INT = 0; --提报方案ID
    SELECT TOP 1
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


    DELETE FROM #Shop; --清空上一次循环的数据
    INSERT INTO #Shop
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
   

    IF OBJECT_ID('tempdB..#MaterailInv') IS NOT NULL
        DROP TABLE #MaterailInv;
    CREATE TABLE #MaterailInv
    (
        [SolutionID] [INT] NOT NULL,
        [CardID] [VARCHAR](20) NULL,
        [CardName] [VARCHAR](20) NULL,
        [CompanyID] [VARCHAR](20) NOT NULL,
        [CompanyName] [NVARCHAR](50) NULL,
        [ShopID] [NVARCHAR](50) NOT NULL,
        [ShopCode] [VARCHAR](50) NOT NULL,
        [ShopName] [NVARCHAR](100) NULL,
        [MaterialCode] [NVARCHAR](60) NOT NULL,
        [Barcode] [NVARCHAR](100) NOT NULL,
        [ActualQty] [NUMERIC](24, 6) NULL, --库存数量
        [PathQty] [NUMERIC](24, 6) NULL,   --在途数量
        [IsImport] [BIT] NULL,
        [ModifyDtm] [DATETIME] NULL,
		SizeName NVARCHAR(50)  NULL
    );

    INSERT INTO #MaterailInv
    (
        SolutionID,
        CardID,
        CardName,
        CompanyID,
        CompanyName,
        ShopID,
        ShopCode,
        ShopName,
        MaterialCode,
        Barcode,
        ActualQty,
        PathQty,
        IsImport,
        ModifyDtm,
		SizeName
    )
    SELECT @SolutionID SolutionID,
           @CardID CardID,
           @CardName CardName,
           @CompanyID CompanyID,
           @CardName CompanyName,
           c.ShopID ShopID,
           ISNULL(h.ShopCode, c.ShopCode) ShopCode,
           ISNULL(h.ShopName, c.ShopName) ShopName,
           g.MaterialCode,
           d.BarCode,
           b.ActualQty,
           b.PathQty,
           0 IsImport,
           GETDATE() ModifyDtm,
		   f.SizeName
    FROM SD_Inv_MaterialMaster a
        JOIN SD_Inv_MaterialDetailSize b
            ON a.CompanyID = b.CompanyID
               AND a.StockID = b.StockID
               AND a.MaterialID = b.MaterialID
        INNER JOIN SD_Mat_Material g
            ON a.MaterialID = g.MaterialID
        JOIN Bas_Shop c
            ON a.StockID = c.StockID
               AND a.CompanyID = c.CompanyID
        JOIN SD_Mat_MaterialSize d
            ON b.SizeID = d.SizeID
               AND b.MaterialID = d.MaterialID
        JOIN SD_Bas_ShopCard e
            ON c.CompanyID = e.CompanyID
               AND c.ShopID = e.ShopID
               AND e.CardID = g.CardID
        LEFT JOIN SD_Mat_Size f
            ON d.SizeID = f.SizeID
        LEFT JOIN Bas_Company j
            ON a.CompanyID = j.CompanyID
        LEFT JOIN SD_Mat_Card k
            ON g.CardID = k.CardID
        LEFT JOIN #ShopMap i
            ON c.CompanyID = i.CompanyID
               AND c.ShopID = i.ShopID -- 关联到店铺映射
        LEFT JOIN #VendShop h
            ON i.VendShopID = h.VendShopID
    WHERE a.CompanyID = @CompanyID
          AND c.ShopID IN
              (
                  SELECT ShopID FROM #Shop
              )
          AND e.CardID = @CardID
          AND NOT EXISTS
    (
        SELECT TOP 1
               1
        FROM #MaterialRange mr
        WHERE mr.MaterialCode = g.MaterialCode
    );
    --删除不同步货号
    DELETE a
    FROM #MaterailInv a
        INNER JOIN #MaterialRange b
            ON b.MaterialCode = a.MaterialCode;


 -- 删除以前捞取的库存数据，点库存捞取
    DELETE FROM HK_ERP_SG_DataGather.dbo.Gat_IceBreaker_MaterialInv
    WHERE SolutionID=@SolutionID
          AND CompanyID = @CompanyID
          AND CardID = @CardID;

    -- 开始插入数据
    INSERT INTO HK_ERP_SG_DataGather.dbo.Gat_IceBreaker_MaterialInv
    (
        SolutionID,
        CardID,
        CardName,
        CompanyID,
        CompanyName,
        ShopID,
        ShopCode,
        ShopName,
        MaterialCode,
        Barcode,
        ActualQty,
        PathQty,
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
           MaterialCode,
           Barcode,
           ActualQty,
           PathQty,
           IsImport,
           ModifyDtm,
		   SizeName
    FROM #MaterailInv;


    TRUNCATE TABLE #Shop;
    TRUNCATE TABLE #VendShop;
    TRUNCATE TABLE #ShopMap;
    DROP TABLE #Shop;
    DROP TABLE #ShopMap;
    DROP TABLE #VendShop;


END;
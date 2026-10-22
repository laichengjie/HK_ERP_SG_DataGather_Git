IF EXISTS (SELECT *
           FROM sys.objects
           WHERE object_id = OBJECT_ID(N'[dbo].[sp_SGIS_Nike_GetMaterailINV_OC]')
                 AND type IN (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_SGIS_Nike_GetMaterailINV_OC];
GO

/* ==========================================================================================
描    述：Nike全渠道 库存数据
创 建 人：lcj
创建日期：
说明：
更新说明：
-- ==========================================================================================*/
CREATE PROCEDURE [dbo].[sp_SGIS_Nike_GetMaterailINV_OC]
    @VendorID INT = -1,               -- 供应商ID
    @GatType INT = 1,                 -- 采集类型，1：正常采集；0：补采
    @CompanyID NVARCHAR(20) = '',     -- 公司ID 
    @CardID NVARCHAR(20) = '',        -- 品牌ID 
    @StartTime DATETIME,              -- 补采时开始时间
    @EndTime DATETIME,                -- 补采结束时间
    @DateNow DATETIME,                -- 当前开始采集的时间
    @IsMC INT = 0                     -- 是否月结数据
AS
BEGIN

    DECLARE @SolutionID INT = 0;

    SELECT TOP 1 @SolutionID = a.ID
    FROM HK_ERP_SG_DataGather.dbo.Gat_Solution a
    WHERE CompanyID = @CompanyID
          AND VendorID = @VendorID
          AND CardID = @CardID;

    -- 提报店铺
    CREATE TABLE #Stock
    (
        StockID NVARCHAR(50),
        isShop BIT,
        InventorySource VARCHAR(20)
    );

    -- 供应商店铺
    CREATE TABLE #VendShop
    (
        VendShopID INT NOT NULL,
        ShopCode VARCHAR(50) NULL,
        ShopName VARCHAR(100) NULL,
        StockCode VARCHAR(50) NULL,
        StockName VARCHAR(100) NULL
    );

    -- 店铺关系
    CREATE TABLE #ShopMap
    (
        VendShopID INT NOT NULL,
        ShopID VARCHAR(50) NOT NULL,
        CompanyID VARCHAR(50)
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
    SELECT a.VendShopID,
           a.ShopID,
           a.CompanyID
    FROM HK_ERP_SG_DataGather.dbo.Bas_ShopMap a
        INNER JOIN HK_ERP_SG_DataGather.dbo.Bas_VendShop b
            ON a.VendShopID = b.ID
    WHERE b.VendorID = @VendorID;

    DELETE FROM #Stock;

    -- 店铺库存权限
    INSERT INTO #Stock
    (
        StockID,
        isShop
    )
    SELECT DISTINCT
        b.StockID,
        1
    FROM HK_ERP_SG_DataGather.dbo.Gat_ShopRight a
        INNER JOIN dbo.Bas_Shop b
            ON a.CompanyID = b.CompanyID
               AND a.ShopID = b.ShopID
    WHERE a.CompanyID = @CompanyID
          AND ISNULL(a.VendorID, -2) = @VendorID
          AND a.AllowUsed = 1
          AND a.CardID = @CardID;

    -- 大仓库存权限
    INSERT INTO #Stock
    (
        StockID,
        isShop
    )
    SELECT DISTINCT
        StockID,
        0
    FROM HK_ERP_SG_DataGather.dbo.Gat_StockRight a
    WHERE a.CompanyID = @CompanyID
          AND ISNULL(a.VendorID, -2) = @VendorID
          AND a.AllowUsed = 1
          AND a.CardID = @CardID
          AND NOT EXISTS (SELECT TOP 1 1 FROM #Stock s WHERE s.StockID = a.StockID);

    IF OBJECT_ID('tempdB..#MeterialINV') IS NOT NULL
        DROP TABLE #MeterialINV;

    CREATE TABLE #MeterialINV
    (
        [SolutionID]        INT              NULL,
        [CompanyID]         VARCHAR(40)      NULL,
        [CompanyName]       NVARCHAR(50)     NULL,
        [CardID]            VARCHAR(40)      NULL,
        [CardName]          VARCHAR(20)      NULL,
        [ShopCode]          VARCHAR(40)      NULL,       -- SITE_CODE 门店/仓库编码
        [ShopName]          NVARCHAR(60)     NULL,
        [StockCode]         VARCHAR(40)      NULL,       -- fulfill_shop_code 发货门店代码
        [StockName]         NVARCHAR(200)    NULL,
        [MaterialCode]      VARCHAR(50)      NULL,       -- PRODUCT_ID(SKU) 货号
        [MaterialName]      NVARCHAR(200)    NULL,       -- PRODUCT_NM 商品名称
        [BarCode]           VARCHAR(60)      NULL,       -- UPC 条形码
        [SizeCode]          VARCHAR(40)      NULL,       -- SIZE_CODE 尺码
        [SizeName]          NVARCHAR(40)     NULL,
        [UpdateDate]        DATETIME         NULL,       -- INVENTORY_DATE 库存日期
        [Qty]               NUMERIC(24, 6)   NULL,       -- QUANTITY 库存数量
		[PathQty]           NUMERIC(24, 6)   NULL,   --在途数量
        [InventoryTypeDesc] NVARCHAR(50)     NULL,       -- INVENTORY_TYPE_DESC 库存类型描述
        [RetailPrice]       DECIMAL(24, 6)   NULL,       -- MSRP_VALUE 建议零售价(吊牌金额)
        [ModifyTime]        DATETIME         NULL,
        [IsImport]          BIT              NULL DEFAULT (0),
        --[DrewTime]          VARCHAR(20)      NULL,       -- LAST_UPDATED 最后更新时间
        [MaterialID]        VARCHAR(50)      NOT NULL,
        [YCQty]             NUMERIC(24, 6)   NULL,
        [StockID]           VARCHAR(50)      NULL        -- 仓库代码
    );

    -- ============================================================
    -- 插入门店库存数据（isShop = 1）
    -- ============================================================
    INSERT INTO #MeterialINV
    (
        SolutionID,
        CompanyID,
        CompanyName,
        CardID,
        CardName,
        ShopCode,
        ShopName,
        StockCode,
        StockName,
        MaterialCode,
        MaterialName,
        BarCode,
        SizeCode,
        SizeName,
        UpdateDate,
        Qty,
		PathQty,
        InventoryTypeDesc,
        RetailPrice,
        ModifyTime,
        IsImport,
        MaterialID,
        YCQty,
        StockID
    )
    SELECT
        @SolutionID AS SolutionID,
        a.CompanyID,
        j.CompanyName,
        f.CardID,
        i.CardName,
        NULL AS ShopCode,
        NULL AS ShopName,
        l.StockCode,
        l.StockName,
        f.MaterialCode,
        f.MaterialShortName MaterialName,
        d.BarCode,
        n.SizeCode,
        n.SizeName,
        GETDATE() AS UpdateDate,
        b.ActualQty AS Qty,
		ISNULL(b.PathQty,0) PathQty,
        NULL AS InventoryTypeDesc,
        0 AS RetailPrice,
        GETDATE() AS ModifyTime,
        0 AS IsImport,
        a.MaterialID,
        0 AS YCQty,
        b.StockID
    FROM dbo.SD_Inv_MaterialMaster a
        JOIN dbo.SD_Mat_Material f
            ON a.MaterialID = f.MaterialID
        JOIN dbo.SD_Inv_MaterialDetailSize b
            ON a.CompanyID = b.CompanyID
               AND a.StockID = b.StockID
               AND a.MaterialID = b.MaterialID
        JOIN dbo.SD_Mat_MaterialSize d
            ON b.SizeID = d.SizeID
               AND b.MaterialID = d.MaterialID
        LEFT JOIN dbo.Bas_Company j
            ON a.CompanyID = j.CompanyID
        LEFT JOIN dbo.SD_Mat_Card i
            ON f.CardID = i.CardID
        LEFT JOIN dbo.SD_Mat_Color m
            ON f.ColorID = m.ColorID
        LEFT JOIN dbo.Bas_Stock l
            ON a.StockID = l.StockID
               AND a.CompanyID = l.CompanyID
        LEFT JOIN dbo.SD_Mat_Size n
            ON b.SizeID = n.SizeID
        INNER JOIN #Stock k
            ON a.StockID = k.StockID   -- 获得所有的仓库数据
    WHERE a.CompanyID = @CompanyID
          AND f.CardID = @CardID
          AND k.isShop = 1;

    -- ============================================================
    -- 插入大仓库存数据（isShop = 0），按物料/尺码汇总
    -- ============================================================
    INSERT INTO #MeterialINV
    (
        SolutionID,
        CompanyID,
        CompanyName,
        CardID,
        CardName,
        ShopCode,
        ShopName,
        StockCode,
        StockName,
        MaterialCode,
        MaterialName,
        BarCode,
        SizeCode,
        SizeName,
        UpdateDate,
        Qty,
		PathQty,
        InventoryTypeDesc,
        RetailPrice,
        ModifyTime,
        IsImport,
        MaterialID,
        YCQty,
        StockID
    )
    SELECT
        @SolutionID AS SolutionID,
        CompanyID,
        CompanyName,
        CardID,
        CardName,
        NULL AS ShopCode,
        NULL AS ShopName,
        StockCode,
        StockName,
        MaterialCode,
        MaterialName,
        BarCode,
        SizeCode,
        SizeName,
        null AS UpdateDate,
        SUM(Qty) AS Qty,
		sum(PathQty) PathQty,
        NULL AS InventoryTypeDesc,
        0 RetailPrice,
        null AS ModifyTime,
        0 IsImport,
        MaterialID,
        0 YCQty,
        StockID
    FROM
    (
        SELECT
            a.CompanyID,
            j.CompanyName,
            f.CardID,
            i.CardName,
            NULL AS ShopCode,
            NULL AS ShopName,
            l.StockCode,
            l.StockName,
            f.MaterialCode,
            f.MaterialShortName MaterialName,
            d.BarCode,
            n.SizeCode,
            n.SizeName,
            null AS UpdateDate,
            b.ActualQty AS Qty,
			ISNULL(b.PathQty,0) PathQty,
            NULL AS InventoryTypeDesc,
            0 AS RetailPrice,
            null AS ModifyTime,
            0 AS IsImport,
            a.MaterialID,
            0 AS YCQty,
            b.StockID
        FROM dbo.SD_Inv_MaterialMaster a
            JOIN dbo.SD_Mat_Material f
                ON a.MaterialID = f.MaterialID
            JOIN dbo.SD_Inv_MaterialDetailSize b
                ON a.CompanyID = b.CompanyID
                   AND a.StockID = b.StockID
                   AND a.MaterialID = b.MaterialID
            JOIN dbo.SD_Mat_MaterialSize d
                ON b.SizeID = d.SizeID
                   AND b.MaterialID = d.MaterialID
            LEFT JOIN dbo.Bas_Company j
                ON a.CompanyID = j.CompanyID
            LEFT JOIN dbo.SD_Mat_Card i
                ON f.CardID = i.CardID
            LEFT JOIN dbo.SD_Mat_Color m
                ON f.ColorID = m.ColorID
            LEFT JOIN dbo.Bas_Stock l
                ON a.StockID = l.StockID
                   AND a.CompanyID = l.CompanyID
            LEFT JOIN dbo.SD_Mat_Size n
                ON b.SizeID = n.SizeID
            INNER JOIN #Stock k
                ON a.StockID = k.StockID   -- 获得所有的仓库数据
        WHERE a.CompanyID = @CompanyID
              AND f.CardID = @CardID
              AND k.isShop = 0
    ) temp
    GROUP BY
        UpdateDate,
        BarCode,
        MaterialCode,
        MaterialName,
        SizeName,
        CompanyID,
        CompanyName,
        CardID,
        CardName,
        StockCode,
        StockName,
        SizeCode,
        MaterialID,
        StockID;

    -- ============================================================
    -- 云仓库存
    -- ============================================================
    IF OBJECT_ID('tempdB..#PostStockQty') IS NOT NULL
        DROP TABLE #PostStockQty;

    CREATE TABLE #PostStockQty
    (
        CompanyID VARCHAR(40),
        MaterialID VARCHAR(40),
        StockID VARCHAR(40),
        BarCode VARCHAR(40),
        YCQty NUMERIC(24, 6) NULL
    );

    INSERT INTO #PostStockQty
    (
        CompanyID,
        MaterialID,
        StockID,
        BarCode,
        YCQty
    )
    SELECT a.CompanyID,
           a.MaterialID,
           a.StockID,
           c.BarCode,
           ISNULL(a.Qty2, 0) - ISNULL(a.UsedQtyForSend, 0) AS YCQty
    FROM dbo.BC_OC_OCCloudStockInventoryDetail a
        INNER JOIN dbo.SD_Mat_MaterialSize c
            ON a.MaterialID = c.MaterialID
               AND a.SizeID = c.SizeID
        INNER JOIN #Stock k
            ON a.StockID = k.StockID
    WHERE a.CompanyID = @CompanyID
          AND a.CloudStockID = 'CS240509FAYAsys-001';

    -- 更新云仓库存数量
    UPDATE a
    SET a.YCQty = b.YCQty
    FROM #MeterialINV a
        INNER JOIN #PostStockQty b
            ON b.CompanyID = a.CompanyID
               AND b.MaterialID = a.MaterialID
               AND b.StockID = a.StockID
               AND b.BarCode = a.BarCode;

    -- ============================================================
    -- 写入正式表
    -- ============================================================
    DECLARE @DrewTime VARCHAR(20);   -- 提报时间点
    SELECT @DrewTime = FORMAT(GETDATE(), 'yyyyMMddHH');

    --更新零售价
    UPDATE a
    SET a.RetailPrice = b.RetailPrice
    FROM #MeterialINV a
        INNER JOIN SD_Mat_MaterialParam b
            ON b.CompanyID = a.CompanyID
               AND b.MaterialID = a.MaterialID

			    
    DELETE #MeterialINV WHERE ISNULL(Qty,0)<=0 

    IF @IsMC = 0
    BEGIN
        -- 删除以前捞取的库存数据，点库存捞取
        DELETE FROM HK_ERP_SG_DataGather.dbo.Gat_Nike_MeterialINV_OC
        WHERE CompanyID = @CompanyID
              AND CardID = @CardID;

        INSERT INTO HK_ERP_SG_DataGather.dbo.Gat_Nike_MeterialINV_OC
        (
            SolutionID,
            CompanyID,
            CompanyName,
            CardID,
            CardName,
            ShopCode,
            ShopName,
            StockCode,
            StockName,
            MaterialCode,
            MaterialName,
            BarCode,
            SizeCode,
            SizeName,
            UpdateDate,
            Qty,
			PathQty,
            InventoryTypeDesc,
            RetailPrice,
            ModifyTime,
            IsImport
        )
        SELECT
            @SolutionID AS SolutionID,
            CompanyID,
            CompanyName,
            CardID,
            CardName,
            ShopCode,
            ShopName,
            StockCode,
            StockName,
            MaterialCode,
            MaterialName,
            BarCode,
            SizeCode,
            SizeName,
            GETDATE() AS UpdateDate,
            isnull(Qty,0) Qty,
			isnull(PathQty,0) PathQty,
            InventoryTypeDesc,
            RetailPrice,
            GETDATE() AS ModifyTime,
            0 AS IsImport
        FROM #MeterialINV;
    END;

    -- 清理临时表
    TRUNCATE TABLE #Stock;
    TRUNCATE TABLE #VendShop;
    TRUNCATE TABLE #ShopMap;

    DROP TABLE #Stock;
    DROP TABLE #ShopMap;
    DROP TABLE #VendShop;

END;
GO
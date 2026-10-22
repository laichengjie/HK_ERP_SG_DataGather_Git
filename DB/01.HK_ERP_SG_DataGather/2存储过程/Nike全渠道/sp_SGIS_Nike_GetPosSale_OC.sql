IF EXISTS (SELECT *
           FROM sys.objects
           WHERE object_id = OBJECT_ID(N'[dbo].[sp_SGIS_Nike_GetPosSale_OC]')
                 AND type IN (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_SGIS_Nike_GetPosSale_OC];
GO

/* ==========================================================================================
描    述：Nike全渠道POS数据
创 建 人：
创建日期：
说明：
更新说明：
-- ==========================================================================================*/
CREATE PROCEDURE [dbo].[sp_SGIS_Nike_GetPosSale_OC]
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
    IF OBJECT_ID('tempdB..#Shop') IS NOT NULL
        DROP TABLE #Shop;

    CREATE TABLE #Shop
    (
        ID INT NOT NULL,
        ShopID VARCHAR(50),
        ShopCode VARCHAR(50),               -- 店铺代码
        Discount NUMERIC(14, 2) NOT NULL,   -- 折扣
        DefaultDiscount BIT,
        IsNoCalculationSale BIT,            -- 不计算电商销售
        IsECSale BIT                        -- 是否正价电商销售
    ); -- 提报店铺

    IF OBJECT_ID('tempdB..#VendShop') IS NOT NULL
        DROP TABLE #VendShop;

    CREATE TABLE #VendShop
    ( -- 供应商店铺
        VendShopID INT NOT NULL,
        ShopCode VARCHAR(50) NULL,
        ShopName VARCHAR(100) NULL,
        StockCode VARCHAR(50) NULL,
        StockName VARCHAR(100) NULL
    );

    IF OBJECT_ID('tempdB..#ShopMap') IS NOT NULL
        DROP TABLE #ShopMap;

    CREATE TABLE #ShopMap
    ( -- 店铺关系1
        VendShopID INT NOT NULL,
        ShopID VARCHAR(50) NOT NULL,
        CompanyID VARCHAR(50)
    );

    -- 不上传店铺
    IF OBJECT_ID('tempdB..#NoUploadShop') IS NOT NULL
        DROP TABLE #NoUploadShop;

    CREATE TABLE #NoUploadShop
    (
        ShopID VARCHAR(50),
        DSShopID VARCHAR(50),
        DSShopCode VARCHAR(50) -- 店铺代码
    );

    DECLARE @RepeatStartTime DATETIME = GETDATE(); -- 开始同步时间
    DECLARE @Start DATETIME = GETDATE();           -- 本次捞取数据的开始时间
    DECLARE @End DATETIME = GETDATE();             -- 本次捞取数据的结束时间
    DECLARE @HisDays INT = 180;                    -- 历史数据保留180天

    DECLARE @IsAuto BIT = 1;                       -- 提取方案方式 1：自动：0:手动
    DECLARE @SolutionID INT = 0;                   -- 提报方案ID

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

    TRUNCATE TABLE #Shop; -- 清空上一次循环的数据（取店铺）

    INSERT INTO #Shop
    (
        ID,
        ShopID,
        Discount,
        DefaultDiscount,
        IsNoCalculationSale,
        IsECSale
    )
    SELECT DISTINCT
        id,
        ShopID,
        ISNULL(Discount, 1) AS Discount,
        ISNULL(DefaultDiscount, 0) AS DefaultDiscount,
        ISNULL(IsNoCalculationSale, 0) AS IsNoCalculationSale,
        ISNULL(IsECSale, 0) AS IsECSale
    FROM HK_ERP_SG_DataGather.dbo.Gat_ShopRight
    WHERE AllowUsed = 1
          AND CompanyID = @CompanyID
          AND CardID = @CardID
          AND ISNULL(VendorID, -2) = @VendorID;

    -- 取店铺代码
    UPDATE a
    SET a.ShopCode = b.ShopCode
    FROM #Shop a
        INNER JOIN dbo.Bas_Shop b
            ON b.ShopID = a.ShopID;

    -- 获取不上传店铺
    TRUNCATE TABLE #NoUploadShop; -- 清空上一次循环的数据（取店铺）

    INSERT INTO #NoUploadShop
    (
        ShopID,
        DSShopID
    )
    SELECT DISTINCT
        a.ShopID,
        a.DSShopID
    FROM HK_ERP_SG_DataGather.dbo.Gat_ShopRightDS a
        INNER JOIN #Shop b
            ON b.ID = a.ShopRightID
               AND b.ShopID = a.ShopID;

    UPDATE a
    SET a.DSShopCode = b.ShopCode
    FROM #NoUploadShop a
        INNER JOIN dbo.Bas_Shop b
            ON b.ShopID = a.DSShopID;

    --①.先获取方案内容
    SELECT TOP 1
        @IsAuto = ISNULL(IsAuto, 1),
        @RepeatStartTime = ISNULL(RepeatStartTime, GETDATE()),
        @SolutionID = ID
    FROM HK_ERP_SG_DataGather.dbo.Gat_Solution
    WHERE ISNULL(VendorID, -2) = @VendorID
          AND CompanyID = @CompanyID
          AND CardID = @CardID;

    -- 不步货品范围
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

    -- 货品折扣
    IF OBJECT_ID('tempdB..#MaterialDiscount') IS NOT NULL
        DROP TABLE #MaterialDiscount;

    CREATE TABLE #MaterialDiscount
    (
        MaterialCode VARCHAR(60),
        Discount NUMERIC(24, 6) NULL
    );

    INSERT INTO #MaterialDiscount
    (
        MaterialCode,
        Discount
    )
    SELECT MaterialCode,
           Discount
    FROM HK_ERP_SG_DataGather.dbo.Gat_MaterialDiscount
    WHERE SolutionId = @SolutionID;

    -- ============================================================
    -- 设置时间范围
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
            SET @End = @DateNow;

            IF NOT EXISTS
            (
                SELECT TOP 1 1
                FROM HK_ERP_SG_DataGather.dbo.Sys_Log
                WHERE IsSucceed = 1
                      AND LogType = 1
                      AND ISNULL(VendorID, 0) = @VendorID
                      AND CompanyID = @CompanyID
                      AND CardID = @CardID
            ) -- 在这之前没有插入过数据
            BEGIN
                SELECT TOP 1
                    @Start = RepeatStartTime
                FROM HK_ERP_SG_DataGather.dbo.Gat_Solution
                WHERE ISNULL(VendorID, -2) = @VendorID
                      AND CompanyID = @CompanyID
                      AND CardID = @CardID;
            END
            ELSE
            BEGIN
                SELECT
                    @Start = MAX(ModifyDTM)
                FROM HK_ERP_SG_DataGather.dbo.Sys_Log
                WHERE IsSucceed = 1
                      AND LogType = 1
                      AND ISNULL(VendorID, -2) = @VendorID
                      AND CompanyID = @CompanyID
                      AND CardID = @CardID; -- 成功提取数据的日志
            END
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
    -- 创建临时表 #PosSale
    -- ============================================================
    IF OBJECT_ID('tempdb..#PosSale', 'U') IS NOT NULL
        DROP TABLE #PosSale;

    CREATE TABLE #PosSale
    (
        [id] [int] IDENTITY(1, 1) NOT NULL,
        [SolutionID] [int] NOT NULL,
        [CompanyID] [varchar](40) NULL,
        [CompanyName] [nvarchar](100) NULL,
        [CardID] [varchar](40) NULL,
        [CardName] [nvarchar](60) NULL,
        [ShopCode] [varchar](40) NOT NULL,                          -- SHOP_CODE 门店编码
        [ShopName] [nvarchar](60) NULL,
        [StockCode] [varchar](40) NULL,                             -- fulfill_shop_code 发货门店代码
        [StockName] [nvarchar](60) NULL,
        [BillNo] [varchar](40) NOT NULL,                            -- TRANSACTION_ID 交易流水号/单据号
		[Sequence] [varchar](10) NOT NULL,
        [BillDate] [datetime] NOT NULL,                             -- TRANSACTION_DT 销售日期
        [OutBillDate] [datetime] NULL,                              -- shipment_DT 发货日期
        [BillTime] [datetime] NOT NULL,
        [MaterialCode] [varchar](50) NULL,                          -- PRODUCT_ID 货号
        [MaterialName] [nvarchar](200) NULL,                        -- PRODUCT_NM 商品名称
        [BarCode] [varchar](60) NULL,                               -- UPC 条形码
        [SizeCode] [varchar](40) NULL,                              -- SIZE_CODE 尺码
        [SizeName] [nvarchar](40) NULL,
        [Qty] [numeric](10, 2) NOT NULL,                            -- QUANTITY 销售数量
        [BalaPrice] [decimal](24, 6) NULL,                          -- PRICE 实际销售价
        [SaleAmount] [decimal](24, 6) NULL,                         -- SALES_AMOUNT 实际销售金额
        [RetailPrice] [decimal](24, 6) NULL,                        -- MSRP_VALUE 建议零售价(吊牌金额)
        [RetailAmount] [decimal](24, 6) NULL,                       -- TOTAL_MSRP_VALUE 吊牌总金额
        [DiscountAmount] [decimal](24, 6) NULL,                     -- TOTAL_DISCOUNT_AMT 折扣总金额
        [TradeType] [varchar](20) NULL DEFAULT ('1'),               -- 交易类型
        [ModifyTime] [datetime] NULL,                               -- PROCESS_DT 数据处理时间戳
        [IsImport] [bit] NULL DEFAULT (0),
        IsYunShop BIT DEFAULT 0 NULL,                               -- 是否云店
        ShopType INT NULL,                                          -- 店铺类型 0 实体店 1网店
        SaleType [VARCHAR](20),                                     -- 销售类型 8 团购销售 9团购退货
        ShopID VARCHAR(50),                                         -- 销售门店
        IsNoCalculationSale BIT,                                    -- 不计算电商销售
        Price [NUMERIC](24, 6) NULL,                                -- 零售价（ERP明细）
        OMSBillNo [VARCHAR](40)                                     -- OMS单号
    );

    -- ============================================================
    -- 插入POS销售数据
    -- ============================================================
    INSERT INTO #PosSale (
        SolutionID,
        CompanyID,
        CompanyName,
        CardID,
        CardName,
        ShopCode,
        ShopName,
        StockCode,
        StockName,
        BillNo,
		Sequence,
        BillDate,
        OutBillDate,
        BillTime,
        MaterialCode,
        MaterialName,
        BarCode,
        SizeCode,
        SizeName,
        Qty,
        BalaPrice,
        SaleAmount,
        RetailPrice,
        RetailAmount,
        DiscountAmount,
        TradeType,
        ModifyTime,
        IsImport,
        IsYunShop,
        ShopType,
        SaleType,
        ShopID,
        IsNoCalculationSale,
        Price,
        OMSBillNo
    )
    SELECT
        @SolutionID AS SolutionID,          -- 方案ID
        a.CompanyID,
        j.CompanyName AS CompanyName,
        @CardID AS CardID,
        i.CardName AS CardName,
        ISNULL(h.ShopCode, d.ShopCode) AS ShopCode,    -- 店铺代码
        ISNULL(h.ShopName, d.ShopName) AS ShopName,    -- 店铺名称
        NULL AS StockCode,                              -- 仓库代码
        NULL AS StockName,                              -- 仓库名称
        b.BillNo,                                       -- 单号
		a.Sequence,
        b.BillDate,                                     -- 销售日期
        b.BillDate AS OutBillDate,
        b.BillTime,                                     -- 录入时间
        c.MaterialCode,                                 -- 货号
        c.MaterialShortName MaterialName,
        a.BarCode,                                      -- 条码
        f.SizeCode,
        f.SizeName,                                     -- 尺码名称
        a.Qty,                                          -- 数量
        a.BalaPrice,
        CASE
            WHEN k.DefaultDiscount = 1 THEN a.PayAmount
            ELSE a.Price * k.Discount * a.Qty
        END AS SaleAmount,
        a.Price AS RetailPrice,                         -- 零售价
        a.Price * a.Qty AS RetailAmount,                -- 零售金额
        0 AS DiscountAmount,
        NULL AS TradeType,
        GETDATE() AS ModifyTime,                        -- 更新时间
        0 AS IsImport,
        ISNULL(d.IsYunShop, 0) AS IsYunShop,
        ISNULL(d.ShopType, 0) AS ShopType,
        a.SaleType,
        a.ShopID,
        k.IsNoCalculationSale,
        a.Price,
        b.OMSBillNo
    FROM dbo.SD_POS_SaleDetail a
        INNER JOIN dbo.SD_POS_SaleMaster b
            ON a.CompanyID = b.CompanyID
               AND a.BillNo = b.BillNo
        INNER JOIN dbo.SD_Mat_Material c
            ON a.MaterialID = c.MaterialID
        INNER JOIN dbo.Bas_Shop d
            ON a.ShopID = d.ShopID
               AND a.CompanyID = d.CompanyID
        INNER JOIN #Shop k
            ON a.ShopID = k.ShopID
        INNER JOIN dbo.SD_Mat_Size f
            ON a.SizeID = f.SizeID
        LEFT JOIN dbo.Bas_Company j
            ON a.CompanyID = j.CompanyID
        LEFT JOIN dbo.SD_Mat_Card i
            ON c.CardID = i.CardID
        LEFT JOIN #ShopMap g
            ON a.CompanyID = g.CompanyID
               AND a.ShopID = g.ShopID
        LEFT JOIN #VendShop h
            ON g.VendShopID = h.VendShopID
    WHERE a.CompanyID = @CompanyID
          AND c.CardID = @CardID
          AND b.BillTime > @Start
          AND b.BillTime <= @End
    UNION ALL
    SELECT
        @SolutionID AS SolutionID,          -- 方案ID
        a.CompanyID,
        j.CompanyName AS CompanyName,
        @CardID AS CardID,
        i.CardName AS CardName,
        ISNULL(h.ShopCode, d.ShopCode) AS ShopCode,    -- 店铺代码
        ISNULL(h.ShopName, d.ShopName) AS ShopName,    -- 店铺名称
        NULL AS StockCode,                              -- 仓库代码
        NULL AS StockName,                              -- 仓库名称
        b.BillNo,                                       -- 单号
		a.Sequence,
        b.BillDate,                                     -- 销售日期
        b.BillDate AS OutBillDate,
        b.BillTime,                                     -- 录入时间
        c.MaterialCode,                                 -- 货号
        c.MaterialShortName MaterialName,
        a.BarCode,                                      -- 条码
        f.SizeCode,
        f.SizeName,                                     -- 尺码名称
        a.Qty,                                          -- 数量
        a.BalaPrice,
        CASE
            WHEN k.DefaultDiscount = 1 THEN a.PayAmount
            ELSE a.Price * k.Discount * a.Qty
        END AS SaleAmount,
        a.Price AS RetailPrice,                         -- 零售价
        a.Price * a.Qty AS RetailAmount,                -- 零售金额
        0 AS DiscountAmount,
        NULL AS TradeType,
        GETDATE() AS ModifyTime,                        -- 更新时间
        0 AS IsImport,
        ISNULL(d.IsYunShop, 0) AS IsYunShop,
        ISNULL(d.ShopType, 0) AS ShopType,
        a.SaleType,
        a.ShopID,
        k.IsNoCalculationSale,
        a.Price,
        b.OMSBillNo
    FROM dbo.SD_POS_SaleDetail2 a
        INNER JOIN dbo.SD_POS_SaleMaster2 b
            ON a.CompanyID = b.CompanyID
               AND a.BillNo = b.BillNo
        INNER JOIN dbo.SD_Mat_Material c
            ON a.MaterialID = c.MaterialID
        INNER JOIN dbo.Bas_Shop d
            ON a.ShopID = d.ShopID
               AND a.CompanyID = d.CompanyID
        INNER JOIN #Shop k
            ON a.ShopID = k.ShopID
        INNER JOIN dbo.SD_Mat_Size f
            ON a.SizeID = f.SizeID
        LEFT JOIN dbo.Bas_Company j
            ON a.CompanyID = j.CompanyID
        LEFT JOIN dbo.SD_Mat_Card i
            ON c.CardID = i.CardID
        LEFT JOIN #ShopMap g
            ON a.CompanyID = g.CompanyID
               AND a.ShopID = g.ShopID
        LEFT JOIN #VendShop h
            ON g.VendShopID = h.VendShopID
    WHERE a.CompanyID = @CompanyID
          AND c.CardID = @CardID
          AND b.BillTime > @Start
          AND b.BillTime <= @End

    -- ============================================================
    -- 删除历史数据
    -- ============================================================
    IF @IsMC = 0
    BEGIN
        -- 删除指定范围之内的数据，防止补采产生重复数据
        DELETE a
        FROM HK_ERP_SG_DataGather.dbo.Gat_Nike_PosSale_OC a
            INNER JOIN #PosSale b
                ON b.SolutionID = a.SolutionID
                   AND b.CompanyID = a.CompanyID
                   AND b.BillNo = a.BillNo
                   AND b.CardID = a.CardID
        WHERE a.CompanyID = @CompanyID
              AND a.CardID = @CardID;

        -- 删除历史数据
        DELETE FROM HK_ERP_SG_DataGather.dbo.Gat_Nike_PosSale_OC
        WHERE CompanyID = @CompanyID
              AND CardID = @CardID
              AND BillTime <= DATEADD(DAY, @HisDays * -1, CONVERT(VARCHAR(10), GETDATE(), 120));
    END;

    -- ============================================================
    -- 取配送店铺
    -- ============================================================
    BEGIN
        UPDATE pos
        SET pos.StockCode = B.ShopCode,
            pos.StockName = B.ShopName
        FROM #PosSale pos
            INNER JOIN dbo.BC_Sal_OrderMaster A
                ON A.OutBillNo = pos.BillNo
            INNER JOIN dbo.Bas_Shop B
                ON A.O2OShopID = B.ShopID
                   AND B.CompanyID = pos.CompanyID;

        UPDATE a
        SET a.StockCode = c.StockCode,
            a.StockName = c.StockName
        FROM #PosSale a
            INNER JOIN dbo.HK_EDI_POSSaleMaster_ERP2 b
                ON b.billno = a.OMSBillNo
            INNER JOIN dbo.Bas_Stock c
                ON c.StockCode = b.stockcode
                   AND c.CompanyID = a.CompanyID
        WHERE ISNULL(a.StockCode, '') = ''
              AND a.OMSBillNo > ''
              AND EXISTS
        (
            SELECT TOP 1 1
            FROM dbo.Bas_HKGroundSetMaster x
            WHERE x.CompanyID = a.CompanyID
                  AND x.OrganizationCode = b.billoucode
        );
    END;

    -- ============================================================
    -- 不上传店铺处理
    -- ============================================================
    BEGIN
        DELETE a
        FROM #PosSale a
            INNER JOIN #NoUploadShop b
                ON b.ShopID = a.ShopID
                   AND b.DSShopCode = a.StockCode
        WHERE ISNULL(a.StockCode, '') > '';
    END

    -- ============================================================
    -- 交易类型处理
    -- ============================================================
    BEGIN
        UPDATE a
        SET TradeType = 1
        FROM #PosSale a;

        -- POS的开单店铺，店铺资料勾选了启用云店，且发货仓代码=开单店代码 传2
        UPDATE a
        SET TradeType = 2
        FROM #PosSale a
        WHERE IsYunShop = 1
              AND ShopCode = StockCode;

        -- 的开单店铺，店铺类型="网店"且未勾选是否云店，发货仓代码=开单店代码 传3
        UPDATE a
        SET TradeType = 3
        FROM #PosSale a
        WHERE IsYunShop = 0
              AND ShopType = 1
              AND ShopCode = StockCode;

        -- 销售类型为"团购"的传 4
        UPDATE a
        SET TradeType = 4
        FROM #PosSale a
        WHERE SaleType IN ('8', '9');

        -- 如果配送店铺代码不为空，且配送店铺代码=发货店铺代码，则交易类型传"2"
        UPDATE a
        SET TradeType = 2
        FROM #PosSale a
        WHERE ISNULL(a.StockCode, '') > ''
              AND ISNULL(a.StockCode, '') = ISNULL(a.ShopCode, '');

        -- 如果配送店铺代码不为空，且配送店铺代码≠发货店铺代码，则交易类型传"3"
        UPDATE a
        SET TradeType = 3
        FROM #PosSale a
        WHERE ISNULL(a.StockCode, '') > ''
              AND ISNULL(a.StockCode, '') <> ISNULL(a.ShopCode, '');
    END;

    -- ============================================================
    -- 配送店代码不为空时，店铺代码取配送店代码
    -- ============================================================
    UPDATE a
    SET a.ShopCode = a.StockCode,
        a.ShopName = a.StockName
    FROM #PosSale a
    WHERE ISNULL(a.StockCode, '') > '';

    -- 勾选了"正价电商销售"，该店铺的交易类型无论是网店还是实体店都只传1
    UPDATE a
    SET TradeType = 1
    FROM #PosSale a
        INNER JOIN #Shop b
            ON a.ShopCode = b.ShopCode
               AND b.IsECSale = 1

    -- ============================================================
    -- 不上传数据处理
    -- ============================================================
    BEGIN
        -- 店铺性质是"网店"产生的pos单配送店铺为空也不推送
        DELETE a
        FROM #PosSale a
        WHERE a.ShopType = 1
              AND ISNULL(a.StockCode, '') = '';

        -- 店铺性质是"网店"，产生的pos单配送店铺只有在平台提取店铺范围内勾选的店铺才推POS，否则不推
        DELETE a
        FROM #PosSale a
        WHERE a.ShopType = 1
              AND ISNULL(a.StockCode, '') > ''
              AND NOT EXISTS
        (
            SELECT TOP 1 1
            FROM #Shop b
            WHERE b.ShopCode = a.StockCode
        );

        -- 删除不上传货号处理
        DELETE a
        FROM #PosSale a
            INNER JOIN #MaterialRange b
                ON b.MaterialCode = a.MaterialCode;

        -- 是否不计算电商销售，若勾选，则POS销售配送店铺是该店铺的，这笔销售不传
        DELETE a
        FROM #PosSale a
            INNER JOIN #Shop b
                ON b.ShopCode = a.StockCode
        WHERE ISNULL(a.StockCode, '') > ''
              AND b.IsNoCalculationSale = 1;
    END;

	   -- ============================================================
    -- 发货店铺为空都取销售门店
    -- ============================================================
	    UPDATE a
      SET a.StockCode = a.ShopCode,
          a.StockName = a.ShopName
      FROM #PosSale a
    WHERE ISNULL(a.StockCode, '') = '';

    -- ============================================================
    -- 货品折扣处理
    -- ============================================================
    BEGIN
        UPDATE a
        SET a.BalaPrice = a.Price * b.Discount,
            a.SaleAmount = a.Price * b.Discount * a.Qty
        FROM #PosSale a
            INNER JOIN #MaterialDiscount b
                ON b.MaterialCode = a.MaterialCode;
    END

    -- ============================================================
    -- 折扣金额计算
    -- ============================================================
    UPDATE a
    SET DiscountAmount = RetailAmount - SaleAmount
    FROM #PosSale a

    -- ============================================================
    -- 写入正式表
    -- ============================================================
    DECLARE @DrewTime VARCHAR(20); -- 提报时间点

    IF @IsMC = 0
    BEGIN
        INSERT INTO HK_ERP_SG_DataGather.dbo.Gat_Nike_PosSale_OC
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
            BillNo,
			Sequence,
            BillDate,
            OutBillDate,
            BillTime,
            MaterialCode,
            MaterialName,
            BarCode,
            SizeCode,
            SizeName,
            Qty,
            BalaPrice,
            SaleAmount,
            RetailPrice,
            RetailAmount,
            DiscountAmount,
            TradeType,
            ModifyTime,
            IsImport
        )
        SELECT
            SolutionID,
            CompanyID,
            CompanyName,
            CardID,
            CardName,
            ShopCode,
            ShopName,
            StockCode,
            StockName,
            BillNo,
			Sequence,
            BillDate,
            OutBillDate,
            BillTime,
            MaterialCode,
            MaterialName,
            BarCode,
            SizeCode,
            SizeName,
            Qty,
            BalaPrice,
            SaleAmount,
            RetailPrice,
            RetailAmount,
            DiscountAmount,
            TradeType,
            ModifyTime,
            IsImport
        FROM #PosSale;
    END;

END;
GO
IF EXISTS (SELECT *
           FROM sys.objects
           WHERE object_id=OBJECT_ID(N'[dbo].[sp_SGIS_Ad_GetPosSale_OC]')AND type IN (N'P', N'PC'))
    DROP PROCEDURE [dbo].sp_SGIS_Ad_GetPosSale_OC;
GO

/* ==========================================================================================
描    述：阿迪全渠道POS数据
创 建 人：lcj
创建日期：2023-08-24
说明：
更新说明：
-- ==========================================================================================*/
CREATE PROCEDURE [dbo].[sp_SGIS_Ad_GetPosSale_OC]
    @VendorID INT = -1,          --供应商ID
    @GatType INT = 1,            --采集类型，1：正常采集；0：补采
    @CompanyID VARCHAR(20) = '', --公司ID 
    @CardID VARCHAR(20) = '',    -- 品牌ID 
    @StartTime DATETIME,         -- 补采时开始时间
    @EndTime DATETIME,           --补采结束时间
    @DateNow DATETIME,           --当前开始采集的时间
    @IsTB INT = 0                --是否三小时数据
AS
BEGIN

    /*
select @VendorID  = 1040,          --供应商ID
    @GatType  = 1,            --采集类型，1：正常采集；0：补采
    @CompanyID  = 'HK', --公司ID 
    @CardID  = '00000000000002',    -- 品牌ID 
    @StartTime ='2024-04-11 10:00:00',         -- 补采时开始时间
    @EndTime ='2024-04-11 12:00:00',           --补采结束时间
    @DateNow ='2024-04-11 11:27:38',           --当前开始采集的时间
    @IsTB  = 1                --是否三小时数据
*/

    IF OBJECT_ID('tempdB..#Shop') IS NOT NULL
        DROP TABLE #Shop;
    CREATE TABLE #Shop
    (
	    ID int NOT NULL,
        ShopID VARCHAR(50),
        ShopCode VARCHAR(50),               --店铺代码
        Discount [NUMERIC](14, 2) NOT NULL, --折扣
        DefaultDiscount BIT,
        IsNoCalculationSale BIT,             --不计算电商销售
		IsECSale  BIT   --是否正价电商销售
    ); --提报店铺

    IF OBJECT_ID('tempdB..#VendShop') IS NOT NULL
        DROP TABLE #VendShop;
    CREATE TABLE #VendShop
    ( -- 供应商店铺
        VendShopID INT NOT NULL, --
        ShopCode VARCHAR(50) NULL,
        ShopName VARCHAR(100) NULL,
        StockCode VARCHAR(50) NULL,
        StockName VARCHAR(100) NULL,
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
        DROP TABLE #NoUploadShop
    CREATE TABLE #NoUploadShop
    (
        ShopID VARCHAR(50),
		DSShopID VARCHAR(50),
        DSShopCode VARCHAR(50),               --店铺代码
    ); 

    DECLARE @Start DATETIME = GETDATE(); --本次捞取数据的开始时间
    DECLARE @End DATETIME = GETDATE(); --本次捞取数据的结束时间
    DECLARE @HisDays INT = 60; -- 历史数据保留60天

    DECLARE @IsAuto BIT = 1; --提取方案方式 1：自动：0:手动
    DECLARE @SolutionID INT = 0; --提报方案ID
    DECLARE @DrewTime VARCHAR(20); --提报时间点

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

    TRUNCATE TABLE   #Shop; --清空上一次循环的数据（取店铺）
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
           ISNULL(Discount, 1) Discount,
           ISNULL(DefaultDiscount, 0) DefaultDiscount,
           ISNULL(IsNoCalculationSale, 0) IsNoCalculationSale,
		   ISNULL(IsECSale,0) IsECSale
    FROM HK_ERP_SG_DataGather.dbo.Gat_ShopRight
    WHERE AllowUsed = 1
          AND CompanyID = @CompanyID
          AND CardID = @CardID
          AND ISNULL(VendorID, -2) = @VendorID;
    --取店铺代码
    UPDATE a
    SET a.ShopCode = b.ShopCode
    FROM #Shop a
        INNER JOIN dbo.Bas_Shop b
            ON b.ShopID = a.ShopID;

   --获取不上传店铺
    TRUNCATE TABLE  #NoUploadShop; --清空上一次循环的数据（取店铺）
    INSERT INTO #NoUploadShop
    (
        ShopID,
		DSShopID 
    )
    SELECT DISTINCT
           a.ShopID,
		   a.DSShopID
    FROM HK_ERP_SG_DataGather.dbo.Gat_ShopRightDS  a
	inner join  #Shop b on b.ID=a.ShopRightID and b.ShopID=a.ShopID

	UPDATE a
    SET a.DSShopCode = b.ShopCode
    FROM #NoUploadShop a
        INNER JOIN dbo.Bas_Shop b
            ON b.ShopID = a.DSShopID;
 


    --①.先获取方案内容
    SELECT TOP 1
           @IsAuto = ISNULL(IsAuto, 1),
           @SolutionID = ID
    FROM HK_ERP_SG_DataGather.dbo.Gat_Solution
    WHERE ISNULL(VendorID, -2) = @VendorID
          AND CompanyID = @CompanyID
          AND CardID = @CardID;

    --不步货品范围
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


	--货品折扣
    IF OBJECT_ID('tempdB..#MaterialDiscount') IS NOT NULL
        DROP TABLE #MaterialDiscount;
    CREATE TABLE #MaterialDiscount
    (
        MaterialCode VARCHAR(60),
		Discount NUMERIC(24,6)  NULL,
    );
    INSERT INTO #MaterialDiscount
    (
        MaterialCode,
		Discount
    )
    SELECT MaterialCode,Discount
    FROM HK_ERP_SG_DataGather.dbo.Gat_MaterialDiscount
    WHERE SolutionId = @SolutionID;


    IF @IsTB = 0
    BEGIN
        IF @GatType = 0 --补采
        BEGIN
            SET @Start = @StartTime;
            SET @End = @EndTime;
        END;
        ELSE
        BEGIN
            SET @Start = @StartTime;
            SET @End = @DateNow;
        END;
    END;
    ELSE
    BEGIN
        SET @Start = @StartTime;
        SET @End = @EndTime;
    END;
    IF OBJECT_ID('tempdb..#PosSale', 'U') IS NOT NULL
        DROP TABLE #PosSale;
    CREATE TABLE #PosSale
    (
        [id] [INT] IDENTITY(1, 1) NOT NULL,  --序号
        [SolutionID] [INT] NOT NULL,         --方案ID
        [CompanyID] [VARCHAR](20) NULL,      --公司
        [CompanyName] [NVARCHAR](100) NULL,  --公司
        [BillNo] [VARCHAR](40) NOT NULL,     --单号
        [ShopCode] [VARCHAR](50) NOT NULL,   --店铺代码
        [ShopName] [NVARCHAR](100) NULL,     --店铺代码
        [StockCode] [VARCHAR](50) NULL,      --仓库代码
        [StockName] [NVARCHAR](100) NULL,
        [BillDate] [DATETIME] NOT NULL,      --销售日期
        [BillTime] [DATETIME] NOT NULL,      --制单时间
        [MaterialCode] [VARCHAR](50) NULL,   --货号
        [BarCode] [VARCHAR](128) NULL,       --条码
        [SizeName] [VARCHAR](50) NULL,       --尺码名称
        [Qty] [NUMERIC](10, 2) NOT NULL,     --数量
        [BalaPrice] [NUMERIC](24, 6) NULL,   --单价
        [SaleAmount] [DECIMAL](24, 6) NULL,  --金额
        [CardID] [VARCHAR](20) NULL,         --品牌
        [CardName] [NVARCHAR](50) NULL,      --品牌名稱
        [SmallTicketNo] [NVARCHAR](40) NULL, --小票号
        [PersonnelID] [NVARCHAR](20) NULL,   --业务员
        [PersonelName] [NVARCHAR](50) NULL,  --业务员
        [Remark] [NVARCHAR](500) NULL,       --备注
        [TradeType] [VARCHAR](20),           --交易类型
        [ModifyTime] [DATETIME] NULL,        --修改时间
        IsImport BIT
            DEFAULT 0 NULL,                  --是否導入
        IsYunShop BIT
            DEFAULT 0 NULL,                  --是否云店
        ShopType INT NULL,                   --店铺类型 0 实体店 1网店
        SaleType [VARCHAR](20),              --销售类型 8 团购销售 9团购退货
        OMSBillNo [VARCHAR](40),             --OMS单号
        IsNoCalculationSale BIT, --不计算电商销售
		Price [NUMERIC](24, 6) NULL,   --零售价（ERP明细 ）
		ShopID VARCHAR(50), --销售门店
    ); 
    INSERT INTO #PosSale
    (
        SolutionID,   --方案ID
        CompanyID,    --公司ID
        ShopCode,     --店铺代码
        ShopName,     --店铺名称
        StockCode,    --仓库代码
        StockName,    --仓库名称
        BillDate,     --销售日期
        BillTime,     --导入时间
        BillNo,       --单号
        MaterialCode, --货号
        BarCode,      --条码
        SizeName,     --尺码名称
        Qty,          --数量
        BalaPrice,    --结算价
        SaleAmount,   --金额
        ModifyTime,   --更新时间,
        IsYunShop,
        ShopType,
        SaleType,
        CardID,
        OMSBillNo,
        IsNoCalculationSale,
		Price,
		ShopID
    )
    SELECT @SolutionID SolutionID,                  --方案ID
           a.CompanyID,                             --公司ID
           ISNULL(h.ShopCode, d.ShopCode) ShopCode, --店铺代码
           ISNULL(h.ShopName, d.ShopName) ShopName, --店铺名称
           NULL StockCode,                          --仓库代码
           NULL StockName,                          --仓库名称
           b.BillDate,                              --销售日期
           b.BillTime,                              --导入时间
           b.BillNo,                                --单号
           c.MaterialCode,                          --货号
           a.BarCode,                               --条码
           f.SizeName,                              --尺码名称
           a.Qty,                                   --数量
           a.Price BalaPrice,                       --零售价
           (CASE
                WHEN k.DefaultDiscount = 1 THEN
                    a.PayAmount
                ELSE
                    a.Price * k.Discount * a.Qty
            END
           ) AS SaleAmount,                         --金额
           GETDATE() ModifyTime,                    --更新时间
           ISNULL(d.IsYunShop, 0) IsYunShop,
           ISNULL(d.ShopType, 0) ShopType,
           a.SaleType,
           @CardID CardID,
           b.OMSBillNo,
           k.IsNoCalculationSale,
		   a.Price,
		   a.ShopID
    FROM dbo.SD_POS_SaleDetail a
        INNER JOIN dbo.SD_POS_SaleMaster b
            ON a.CompanyID = b.CompanyID
               AND a.BillNo = b.BillNo
        INNER JOIN dbo.SD_Mat_Material c
            ON a.MaterialID = c.MaterialID
        INNER JOIN dbo.Bas_Shop d
            ON a.ShopID = d.ShopID
               AND a.CompanyID = d.CompanyID
        INNER JOIN dbo.SD_Bas_ShopCard e
            ON d.CompanyID = e.CompanyID
               AND d.ShopID = e.ShopID
               AND c.CardID = e.CardID
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
               AND a.ShopID = g.ShopID -- 关联到店铺映射
        LEFT JOIN #VendShop h
            ON g.VendShopID = h.VendShopID
    --LEFT JOIN dbo.Bas_Stock s
    --    ON s.CompanyID = a.CompanyID
    --       AND s.StockID = a.StockID
    WHERE a.CompanyID = @CompanyID
          AND e.CardID = @CardID
          AND b.BillTime > @Start
          AND b.BillTime <= @End
    UNION ALL
    SELECT @SolutionID SolutionID,                  --方案ID
           a.CompanyID,                             --公司ID
           ISNULL(h.ShopCode, d.ShopCode) ShopCode, --店铺代码
           ISNULL(h.ShopName, d.ShopName) ShopName, --店铺名称
           NULL StockCode,                          --s.StockCode StockCode,                   --仓库代码
           NULL StockName,                          --s.StockName StockName,                   --仓库名称
           b.BillDate,                              --销售日期
           b.BillTime,                              --导入时间
           b.BillNo,                                --单号
           c.MaterialCode,                          --货号
           a.BarCode,                               --条码
           f.SizeName,                              --尺码名称
           a.Qty,                                   --数量
           a.Price BalaPrice,                       --零售价
           (CASE
                WHEN k.DefaultDiscount = 1 THEN
                    a.PayAmount
                ELSE
                    a.Price * k.Discount * a.Qty
            END
           ) AS SaleAmount,                         --金额
           GETDATE() ModifyTime,                    --更新时间
           ISNULL(d.IsYunShop, 0) IsYunShop,
           ISNULL(d.ShopType, 0) ShopType,
           a.SaleType,
           @CardID CardID,
           b.OMSBillNo,
           k.IsNoCalculationSale,
		   a.Price,
		   a.ShopID
    FROM dbo.SD_POS_SaleDetail2 a
        INNER JOIN dbo.SD_POS_SaleMaster2 b
            ON a.CompanyID = b.CompanyID
               AND a.BillNo = b.BillNo
        INNER JOIN dbo.SD_Mat_Material c
            ON a.MaterialID = c.MaterialID
        INNER JOIN dbo.Bas_Shop d
            ON a.ShopID = d.ShopID
               AND a.CompanyID = d.CompanyID
        INNER JOIN dbo.SD_Bas_ShopCard e
            ON d.CompanyID = e.CompanyID
               AND d.ShopID = e.ShopID
               AND c.CardID = e.CardID
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
               AND a.ShopID = g.ShopID -- 关联到店铺映射
        LEFT JOIN #VendShop h
            ON g.VendShopID = h.VendShopID
    --LEFT JOIN Bas_Stock s
    --    ON s.CompanyID = a.CompanyID
    --       AND s.StockID = a.StockID
    WHERE a.CompanyID = @CompanyID
          AND e.CardID = @CardID
          AND b.BillTime > @Start
          AND b.BillTime <= @End;

    IF @IsTB = 1
    BEGIN --补偿取 上一次遗漏的单据
        --上一次提报时间点
        SELECT @DrewTime = FORMAT(MAX(ModifyTime), 'yyyyMMddHH')
        FROM HK_ERP_SG_DataGather.dbo.Gat_Ad_PosSale_OCTB;

        IF OBJECT_ID('tempdb..#PosSaleBill', 'U') IS NOT NULL
            DROP TABLE #PosSaleBill;
        CREATE TABLE #PosSaleBill
        (
            [CompanyID] [VARCHAR](20) NULL, --公司
            [BillNo] [VARCHAR](40) NOT NULL, --单号
        );
        INSERT INTO #PosSaleBill
        (
            CompanyID,
            BillNo
        )
        SELECT CompanyID,
               BillNo
        FROM HK_ERP_SG_DataGather.dbo.Gat_Ad_PosSale_OCTB a
        WHERE a.SolutionID = @SolutionID
              AND a.DrewTime = @DrewTime;

        DECLARE @LastStart DATETIME; --本次捞取数据的开始时间
        DECLARE @LastEnd DATETIME; --本次捞取数据的结束时间

        SELECT @LastStart = DATEADD(HOUR, -1, @Start);
        SELECT @LastEnd = DATEADD(HOUR, -1, @End);

        INSERT INTO #PosSale
        (
            SolutionID,   --方案ID
            CompanyID,    --公司ID
            ShopCode,     --店铺代码
            ShopName,     --店铺名称
            StockCode,    --仓库代码
            StockName,    --仓库名称
            BillDate,     --销售日期
            BillTime,     --导入时间
            BillNo,       --单号
            MaterialCode, --货号
            BarCode,      --条码
            SizeName,     --尺码名称
            Qty,          --数量
            BalaPrice,    --结算价
            SaleAmount,   --金额
            ModifyTime,   --更新时间,
            IsYunShop,
            ShopType,
            SaleType,
            CardID,
            OMSBillNo,
            IsNoCalculationSale,
			Price,
			ShopID
        )
        SELECT @SolutionID SolutionID,                  --方案ID
               a.CompanyID,                             --公司ID
               ISNULL(h.ShopCode, d.ShopCode) ShopCode, --店铺代码
               ISNULL(h.ShopName, d.ShopName) ShopName, --店铺名称
               NULL StockCode,                          --s.StockCode StockCode,                   --仓库代码
               NULL StockName,                          --s.StockName StockName,                   --仓库名称
               b.BillDate,                              --销售日期
               b.BillTime,                              --导入时间
               b.BillNo,                                --单号
               c.MaterialCode,                          --货号
               a.BarCode,                               --条码
               f.SizeName,                              --尺码名称
               a.Qty,                                   --数量
               a.Price BalaPrice,                       --零售价
               (CASE
                    WHEN k.DefaultDiscount = 1 THEN
                        a.PayAmount
                    ELSE
                        a.Price * k.Discount * a.Qty
                END
               ) AS SaleAmount,                         --金额
               GETDATE() ModifyTime,                    --更新时间
               ISNULL(d.IsYunShop, 0) IsYunShop,
               ISNULL(d.ShopType, 0) ShopType,
               a.SaleType,
               @CardID CardID,
               b.OMSBillNo,
               k.IsNoCalculationSale,
			   a.Price,
			   a.ShopID
        FROM dbo.SD_POS_SaleDetail a
            INNER JOIN dbo.SD_POS_SaleMaster b
                ON a.CompanyID = b.CompanyID
                   AND a.BillNo = b.BillNo
            INNER JOIN dbo.SD_Mat_Material c
                ON a.MaterialID = c.MaterialID
            INNER JOIN dbo.Bas_Shop d
                ON a.ShopID = d.ShopID
                   AND a.CompanyID = d.CompanyID
            INNER JOIN dbo.SD_Bas_ShopCard e
                ON d.CompanyID = e.CompanyID
                   AND d.ShopID = e.ShopID
                   AND c.CardID = e.CardID
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
                   AND a.ShopID = g.ShopID -- 关联到店铺映射
            LEFT JOIN #VendShop h
                ON g.VendShopID = h.VendShopID
        --LEFT JOIN dbo.Bas_Stock s
        --    ON s.CompanyID = a.CompanyID
        --       AND s.StockID = a.StockID
        WHERE b.CompanyID = @CompanyID
              AND e.CardID = @CardID
              AND b.BillTime > @LastStart
              AND b.BillTime <= @LastEnd
              AND NOT EXISTS
        (
            SELECT TOP 1
                   1
            FROM #PosSaleBill p
            WHERE p.CompanyID = a.CompanyID
                  AND p.BillNo = a.BillNo
        )
              AND NOT EXISTS
        (
            SELECT TOP 1
                   1
            FROM #PosSale p
            WHERE p.CompanyID = a.CompanyID
                  AND p.BillNo = a.BillNo
        )
        UNION ALL
        SELECT @SolutionID SolutionID,                  --方案ID
               a.CompanyID,                             --公司ID
               ISNULL(h.ShopCode, d.ShopCode) ShopCode, --店铺代码
               ISNULL(h.ShopName, d.ShopName) ShopName, --店铺名称
               NULL StockCode,                          --s.StockCode StockCode,                   --仓库代码
               NULL StockName,                          --s.StockName StockName,                   --仓库名称
               b.BillDate,                              --销售日期
               b.BillTime,                              --导入时间
               b.BillNo,                                --单号
               c.MaterialCode,                          --货号
               a.BarCode,                               --条码
               f.SizeName,                              --尺码名称
               a.Qty,                                   --数量
               a.Price BalaPrice,                       --零售价
               (CASE
                    WHEN k.DefaultDiscount = 1 THEN
                        a.PayAmount
                    ELSE
                        a.Price * k.Discount * a.Qty
                END
               ) AS SaleAmount,                         --金额
               GETDATE() ModifyTime,                    --更新时间
               ISNULL(d.IsYunShop, 0) IsYunShop,
               ISNULL(d.ShopType, 0) ShopType,
               a.SaleType,
               @CardID CardID,
               b.OMSBillNo,
               k.IsNoCalculationSale,
			   a.Price,
			   a.ShopID
        FROM dbo.SD_POS_SaleDetail2 a
            INNER JOIN dbo.SD_POS_SaleMaster2 b
                ON a.CompanyID = b.CompanyID
                   AND a.BillNo = b.BillNo
            INNER JOIN dbo.SD_Mat_Material c
                ON a.MaterialID = c.MaterialID
            INNER JOIN dbo.Bas_Shop d
                ON a.ShopID = d.ShopID
                   AND a.CompanyID = d.CompanyID
            INNER JOIN dbo.SD_Bas_ShopCard e
                ON d.CompanyID = e.CompanyID
                   AND d.ShopID = e.ShopID
                   AND c.CardID = e.CardID
            INNER JOIN #Shop k
                ON a.ShopID = k.ShopID
            INNER JOIN SD_Mat_Size f
                ON a.SizeID = f.SizeID
            LEFT JOIN dbo.Bas_Company j
                ON a.CompanyID = j.CompanyID
            LEFT JOIN dbo.SD_Mat_Card i
                ON c.CardID = i.CardID
            LEFT JOIN #ShopMap g
                ON a.CompanyID = g.CompanyID
                   AND a.ShopID = g.ShopID -- 关联到店铺映射
            LEFT JOIN #VendShop h
                ON g.VendShopID = h.VendShopID
            LEFT JOIN dbo.Bas_Stock s
                ON s.CompanyID = a.CompanyID
                   AND s.StockID = a.StockID
        WHERE b.CompanyID = @CompanyID
              AND e.CardID = @CardID
              AND b.BillTime > @LastStart
              AND b.BillTime <= @LastEnd
              AND NOT EXISTS
        (
            SELECT TOP 1
                   1
            FROM #PosSaleBill p
            WHERE p.CompanyID = a.CompanyID
                  AND p.BillNo = a.BillNo
        )
              AND NOT EXISTS
        (
            SELECT TOP 1
                   1
            FROM #PosSale p
            WHERE p.CompanyID = a.CompanyID
                  AND p.BillNo = a.BillNo
        );

    END;

	 --删除历史数据
    IF @IsTB = 0
    BEGIN
        --删除指定范围之内的数据，防止补采产生重复数据
        DELETE a
        FROM HK_ERP_SG_DataGather.dbo.Gat_Ad_PosSale_OC a
            INNER JOIN #PosSale b
                ON b.SolutionID = a.SolutionID
                   AND b.CompanyID = a.CompanyID
                   AND b.BillNo = a.BillNo
                   AND b.CardID = a.CardID
        WHERE a.CompanyID = @CompanyID
              AND a.CardID = @CardID;
        --AND a.BillTime > @Start
        --AND a.BillTime <= @End;

        --删除历史数据
        DELETE FROM HK_ERP_SG_DataGather.dbo.Gat_Ad_PosSale_OC
        WHERE CompanyID = @CompanyID
              AND CardID = @CardID
              AND BillTime <= DATEADD(DAY, @HisDays * -1, CONVERT(VARCHAR(10), GETDATE(), 120));
    END;
    ELSE
    BEGIN
        SELECT @DrewTime = FORMAT(GETDATE(), 'yyyyMMddHH');
        --删除指定范围之内的数据，防止补采产生重复数据
        --DELETE FROM HK_ERP_SG_DataGather.dbo.Gat_Ad_PosSale_OCTB
        --WHERE CompanyID = @CompanyID
        --      AND CardID = @CardID
        --      AND BillTime > @Start
        --      AND BillTime <= @End;
        DELETE FROM HK_ERP_SG_DataGather.dbo.Gat_Ad_PosSale_OCTB
        WHERE ModifyTime <= DATEADD(DAY, -2, CONVERT(VARCHAR(10), GETDATE(), 120));

    END;

    --取配送店铺
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
            SELECT TOP 1
                   1
            FROM dbo.Bas_HKGroundSetMaster x
            WHERE x.CompanyID = a.CompanyID
                  AND x.OrganizationCode = b.billoucode
        );
    END;

	--不上传店铺处理
	begin
	 DELETE a
        FROM #PosSale a
		inner join #NoUploadShop b on b.ShopID=a.ShopID and b.DSShopCode=a.StockCode
		where ISNULL(a.StockCode,'')>''
	end


    --交易类型
    BEGIN
        UPDATE a
        SET TradeType = 1
        FROM #PosSale a;
        --POS的开单店铺，店铺资料勾选了启用云店，且发货仓代码=开单店代码 传2
        UPDATE a
        SET TradeType = 2
        FROM #PosSale a
        WHERE IsYunShop = 1
              AND ShopCode = StockCode;
        --的开单店铺，店铺类型=“网店"且未勾选是否云店，发货仓代码=开单店代码 传3
        UPDATE a
        SET TradeType = 3
        FROM #PosSale a
        WHERE IsYunShop = 0
              AND ShopType = 1
              AND ShopCode = StockCode;
        --销售类型为“团购”的传 4
        UPDATE a
        SET TradeType = 4
        FROM #PosSale a
        WHERE SaleType IN ( '8', '9' );
        --如果配送店铺代码不为空，且配送店铺代码=发货店铺代码，则交易类型传“2”；
        UPDATE a
        SET TradeType = 2
        FROM #PosSale a
        WHERE ISNULL(a.StockCode, '') > ''
              AND ISNULL(a.StockCode, '') = ISNULL(a.ShopCode, '');
        --如果配送店铺代码不为空，且配送店铺代码≠发货店铺代码，则交易类型传“3”；
        UPDATE a
        SET TradeType = 3
        FROM #PosSale a
        WHERE ISNULL(a.StockCode, '') > ''
              AND ISNULL(a.StockCode, '') <> ISNULL(a.ShopCode, '');

    END;

    --配送店代码不为空时，店铺代码取配送店代码
    UPDATE a
    SET a.ShopCode = a.StockCode,
        a.ShopName = a.StockName
    FROM #PosSale a
    WHERE ISNULL(a.StockCode, '') > '';

	 --勾选了“ 正价电商销售”,该店铺的交易类型无论是网店还是实体店都只传1
	 UPDATE a
     SET TradeType = 1
     FROM #PosSale a 
	 INNER JOIN #Shop b ON a.ShopCode = b.ShopCode  AND b.IsECSale=1

    --不上传数据处理
    BEGIN
        --店铺性质是“网店”产生的pos单配送店铺为空也不推送
        DELETE a
        FROM #PosSale a
        WHERE a.ShopType = 1
              AND ISNULL(a.StockCode, '') = '';
        --店铺性质是“网店”，产生的pos单配送店铺只有在平台提取店铺范围内勾选的店铺才推POS，否则不推
        DELETE a
        FROM #PosSale a
        WHERE a.ShopType = 1
              AND ISNULL(a.StockCode, '') > ''
              AND NOT EXISTS
        (
            SELECT TOP 1 1 FROM #Shop b WHERE b.ShopCode = a.StockCode
        );
        --不上传货号处理
        DELETE a
        FROM #PosSale a
            INNER JOIN #MaterialRange b
                ON b.MaterialCode = a.MaterialCode;

        --是否不计算电商销售，若勾选，则POS销售配送店铺是该店铺的，这笔销售不传，包含当日、三小时数据都按此规则；
        DELETE a
        FROM #PosSale a
		inner join #Shop b on b.ShopCode=a.StockCode
        WHERE ISNULL(a.StockCode, '') > ''
              and b.IsNoCalculationSale = 1  
    END;

	--货品折扣处理
	begin
	    update a  set 
		a.BalaPrice=a.Price*b.Discount,
		a.SaleAmount = a.Price * b.Discount * a.Qty
        FROM #PosSale a
		inner join #MaterialDiscount b on b.MaterialCode=a.MaterialCode
    end


    IF @IsTB = 0
    BEGIN
        -- 开始插入数据
        IF (@IsAuto = 0) --手动数据去到中转库
        BEGIN
            INSERT INTO HK_ERP_SG_DataGather.dbo.Gat_Ad_PosSale_transfer_OC
            (
                SolutionID,   --方案ID
                CompanyID,    --公司ID
                ShopCode,     --店铺代码
                ShopName,     --店铺名称
                StockCode,    --仓库代码
                StockName,    --仓库名称
                BillDate,     --销售日期
                BillTime,     --导入时间
                BillNo,       --单号
                MaterialCode, --货号
                BarCode,      --条码
                SizeName,     --尺码名称
                Qty,          --数量
                BalaPrice,    --结算价
                SaleAmount,   --金额
                ModifyTime,   --更新时间
                TradeType,
                CardID
            )
            SELECT SolutionID,   --方案ID
                   CompanyID,    --公司ID
                   ShopCode,     --店铺代码
                   ShopName,     --店铺名称
                   StockCode,    --仓库代码
                   StockName,    --仓库名称
                   BillDate,     --销售日期
                   BillTime,     --导入时间
                   BillNo,       --单号
                   MaterialCode, --货号
                   BarCode,      --条码
                   SizeName,     --尺码名称
                   Qty,          --数量
                   BalaPrice,    --结算价
                   SaleAmount,   --金额
                   ModifyTime,   --更新时间
                   TradeType,
                   CardID
            FROM #PosSale;
        END;
        IF (@IsAuto = 1) --自动数据去到真实库
        BEGIN
            INSERT INTO HK_ERP_SG_DataGather.dbo.Gat_Ad_PosSale_OC
            (
                SolutionID,   --方案ID
                CompanyID,    --公司ID
                ShopCode,     --店铺代码
                ShopName,     --店铺名称
                StockCode,    --仓库代码
                StockName,    --仓库名称
                BillDate,     --销售日期
                BillTime,     --导入时间
                BillNo,       --单号
                MaterialCode, --货号
                BarCode,      --条码
                SizeName,     --尺码名称
                Qty,          --数量
                BalaPrice,    --结算价
                SaleAmount,   --金额
                ModifyTime,   --更新时间
                TradeType,
                CardID
            )
            SELECT SolutionID,   --方案ID
                   CompanyID,    --公司ID
                   ShopCode,     --店铺代码
                   ShopName,     --店铺名称
                   StockCode,    --仓库代码
                   StockName,    --仓库名称
                   BillDate,     --销售日期
                   BillTime,     --导入时间
                   BillNo,       --单号
                   MaterialCode, --货号
                   BarCode,      --条码
                   SizeName,     --尺码名称
                   Qty,          --数量
                   BalaPrice,    --结算价
                   SaleAmount,   --金额
                   ModifyTime,   --更新时间
                   TradeType,
                   CardID
            FROM #PosSale;
        END;
    END;
    ELSE
    BEGIN
        INSERT INTO HK_ERP_SG_DataGather.dbo.Gat_Ad_PosSale_OCTB
        (
            SolutionID,   --方案ID
            CompanyID,    --公司ID
            ShopCode,     --店铺代码
            ShopName,     --店铺名称
            StockCode,    --仓库代码
            StockName,    --仓库名称
            BillDate,     --销售日期
            BillTime,     --导入时间
            BillNo,       --单号
            MaterialCode, --货号
            BarCode,      --条码
            SizeName,     --尺码名称
            Qty,          --数量
            BalaPrice,    --结算价
            SaleAmount,   --金额
            ModifyTime,   --更新时间
            TradeType,
            DrewTime,     -- 提报时间点
            CardID
        )
        SELECT  distinct SolutionID,   --方案ID
               CompanyID,    --公司ID
               ShopCode,     --店铺代码
               ShopName,     --店铺名称
               StockCode,    --仓库代码
               StockName,    --仓库名称
               BillDate,     --销售日期
               BillTime,     --导入时间
               BillNo,       --单号
               MaterialCode, --货号
               BarCode,      --条码
               SizeName,     --尺码名称
               Qty,          --数量
               BalaPrice,    --结算价
               SaleAmount,   --金额
               ModifyTime,   --更新时间
               TradeType,
               @DrewTime DrewTime,
               CardID
        FROM #PosSale;
    END;
END;



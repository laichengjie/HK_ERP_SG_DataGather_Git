IF EXISTS (SELECT *
           FROM sys.objects
           WHERE object_id=OBJECT_ID(N'[dbo].[sp_SGIS_Ad_GetPosSale]')AND type IN (N'P', N'PC'))
    DROP PROCEDURE [dbo].sp_SGIS_Ad_GetPosSale;
GO

--阿迪品牌提报
CREATE Procedure [dbo].[sp_SGIS_Ad_GetPosSale]
	@VendorID INT = -1 ,			--供应商ID
	@GatType Int = 1,				--采集类型，1：正常采集；0：补采
	@CompanyID VARCHAR(20) = '',	--公司ID 
	@CardID VARCHAR(20) = '',		-- 品牌ID 
	@StartTime  DateTime ,			-- 补采时开始时间
	@EndTime DateTime ,				--补采结束时间
	@DateNow DateTime				--当前开始采集的时间
As
BEGIN

    CREATE TABLE #Shop
    (
	    ID int NOT NULL,
        ShopID VARCHAR(50)
    ); --提报店铺
    CREATE TABLE #VendShop
    ( -- 供应商店铺
        VendShopID INT NOT NULL, --
        ShopCode VARCHAR(50) NULL,
        ShopName VARCHAR(100) NULL,
        StockCode VARCHAR(50) NULL,
        StockName VARCHAR(100) NULL,
    );
    CREATE TABLE #ShopMap
    ( -- 店铺关系1
        VendShopID INT NOT NULL,
        ShopID VARCHAR(20) NOT NULL,
        CompanyID VARCHAR(20)
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
    INSERT INTO #Shop(ID,ShopID)
    SELECT DISTINCT
          ID,ShopID
    FROM HK_ERP_SG_DataGather.dbo.Gat_ShopRight
    WHERE AllowUsed = 1
          AND CompanyID = @CompanyID
          AND CardID = @CardID
          AND ISNULL(VendorID, -2) = @VendorID;

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

    --删除指定范围之内的数据，防止补采产生重复数据
    DELETE FROM HK_ERP_SG_DataGather.dbo.Gat_Ad_PosSale
    WHERE CompanyID = @CompanyID
          AND CardID = @CardID
          AND BillTime > @Start
          AND BillTime <= @End;
    --删除历史数据
    DELETE FROM HK_ERP_SG_DataGather.dbo.Gat_Ad_PosSale
    WHERE CompanyID = @CompanyID
          AND CardID = @CardID
          AND BillTime <= DATEADD(DAY, @HisDays * -1, CONVERT(VARCHAR(10), GETDATE(), 120));

    SELECT ISNULL(h.ShopCode, d.ShopCode) ShopCode,
           b.BillDate,
           b.BillTime,
           c.MaterialCode,
           a.Qty,
           b.BillNo,
           a.PayAmount AS SaleAmount,
           NULL PersonelName,
           a.BarCode,
           f.SizeName,
           a.CompanyID,
           j.ShortName CompanyName,
           c.CardID,
           c.SubItem CardName,
           @SolutionID SolutionID,
           a.BalaPrice,
           ISNULL(h.ShopName, d.ShopName) ShopName,
           h.StockName StockName,
		   a.Price,   --零售价（ERP明细 ）
		   a.ShopID,  --销售门店
		   CONVERT(NVARCHAR(40), '') StockCode, --发货店铺
		   b.OMSBillNo
    INTO #PosSale
    FROM SD_POS_SaleDetail a
        INNER JOIN SD_POS_SaleMaster b
            ON a.CompanyID = b.CompanyID
               AND a.BillNo = b.BillNo
        INNER JOIN SD_Mat_Material c
            ON a.MaterialID = c.MaterialID
        INNER JOIN Bas_Shop d
            ON a.ShopID = d.ShopID
               AND a.CompanyID = d.CompanyID
        --INNER JOIN SD_Bas_ShopCard e
        --    ON d.CompanyID = e.CompanyID
        --       AND d.ShopID = e.ShopID
        --       AND c.CardID = e.CardID
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
          AND c.CardID = @CardID
          AND b.BillTime > @Start
          AND b.BillTime <= @End
    UNION ALL
    SELECT ISNULL(h.ShopCode, d.ShopCode),
           b.BillDate,
           b.BillTime,
           c.MaterialCode,
           a.Qty,
           b.BillNo,
           a.PayAmount AS SaleAmount,
           NULL,
           a.BarCode,
           f.SizeName,
           a.CompanyID,
           j.ShortName,
           c.CardID,
           c.SubItem,
           @SolutionID,
           a.BalaPrice,
           ISNULL(h.ShopName, d.ShopName),
           h.StockName,
		   a.Price,   --零售价（ERP明细 ）
		   a.ShopID , --销售门店
		   CONVERT(NVARCHAR(40), '') StockCode, --发货店铺
		   b.OMSBillNo
    FROM SD_POS_SaleDetail2 a
        INNER JOIN SD_POS_SaleMaster2 b
            ON a.CompanyID = b.CompanyID
               AND a.BillNo = b.BillNo
        INNER JOIN SD_Mat_Material c
            ON a.MaterialID = c.MaterialID
        INNER JOIN Bas_Shop d
            ON a.ShopID = d.ShopID
               AND a.CompanyID = d.CompanyID
        --INNER JOIN SD_Bas_ShopCard e
        --    ON d.CompanyID = e.CompanyID
        --       AND d.ShopID = e.ShopID
        --       AND c.CardID = e.CardID
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
          AND c.CardID = @CardID
          AND b.BillTime > @Start
          AND b.BillTime <= @End;

    --取配送店铺
    BEGIN
        UPDATE pos
        SET pos.StockCode = B.ShopCode
        FROM #PosSale pos
            INNER JOIN dbo.BC_Sal_OrderMaster A
                ON A.OutBillNo = pos.BillNo
            INNER JOIN dbo.Bas_Shop B
                ON A.O2OShopID = B.ShopID
                   AND B.CompanyID = pos.CompanyID;
        UPDATE a
        SET a.StockCode = c.StockCode
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

    --不上传货号处理
    DELETE a
    FROM #PosSale a
        INNER JOIN #MaterialRange b
            ON b.MaterialCode = a.MaterialCode;


	--货品折扣处理
	begin
	    update a  set 
		a.BalaPrice=a.Price*b.Discount
        FROM #PosSale a
		inner join #MaterialDiscount b on b.MaterialCode=a.MaterialCode
    end



    IF (@IsAuto = 0) --手动数据去到中转库
    BEGIN
        INSERT INTO HK_ERP_SG_DataGather.dbo.Gat_Ad_PosSale_transfer
        (
            ShopCode,
            BillDate,
            BillTime,
            MaterialCode,
            Qty,
            BillNo,
            SalesAmount,
            PersonnelID,
            BarCode,
            SizeName,
            CompanyID,
            CompanyName,
            CardID,
            CardName,
            SolutionID,
            BalaPrice,
            ShopName,
            StockName
        )
        SELECT ShopCode,
               BillDate,
               BillTime,
               MaterialCode,
               Qty,
               BillNo,
               SaleAmount,
               PersonelName,
               BarCode,
               SizeName,
               CompanyID,
               CompanyName,
               CardID,
               CardName,
               SolutionID,
               BalaPrice,
               ShopName,
               StockName
        FROM #PosSale;
    END;
    IF (@IsAuto = 1) --自动数据去到真实库
    BEGIN
        INSERT INTO HK_ERP_SG_DataGather.dbo.Gat_Ad_PosSale
        (
            ShopCode,
            BillDate,
            BillTime,
            MaterialCode,
            Qty,
            BillNo,
            SaleAmount,
            PersonelName,
            BarCode,
            SizeName,
            CompanyID,
            CompanyName,
            CardID,
            CardName,
            SolutionID,
            BalaPrice,
            ShopName,
            StockName
        )
        SELECT ShopCode,
               BillDate,
               BillTime,
               MaterialCode,
               Qty,
               BillNo,
               SaleAmount,
               PersonelName,
               BarCode,
               SizeName,
               CompanyID,
               CompanyName,
               CardID,
               CardName,
               SolutionID,
               BalaPrice,
               ShopName,
               StockName
        FROM #PosSale;
    END;


    TRUNCATE TABLE #PosSale;
    TRUNCATE TABLE #Shop;
    TRUNCATE TABLE #VendShop;
    TRUNCATE TABLE #ShopMap;
    DROP TABLE #Shop;
    DROP TABLE #ShopMap;
    DROP TABLE #VendShop;
    DROP TABLE #PosSale;



END;



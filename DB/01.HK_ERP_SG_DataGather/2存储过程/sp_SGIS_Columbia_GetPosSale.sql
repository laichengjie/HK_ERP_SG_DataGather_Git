
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_SGIS_Columbia_GetPosSale]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].sp_SGIS_Columbia_GetPosSale
go

Create PROCEDURE [dbo].[sp_SGIS_Columbia_GetPosSale]
    @VendorID INT = -1 ,			--供应商ID
    @GatType INT = 1 ,				--采集类型，1：正常采集；0：补采
    @CompanyID NVARCHAR(20) = '' ,	--公司ID 
    @CardID NVARCHAR(20) = '' ,		-- 品牌ID 
    @StartTime DATETIME ,			-- 补采时开始时间
    @EndTime DATETIME ,				--补采结束时间
    @DateNow DATETIME				--当前开始采集的时间
AS
BEGIN

    CREATE TABLE #Shop ( ShopID NVARCHAR(50) ); --提报店铺
    CREATE TABLE #VendShop
    (
      -- 供应商店铺
      VendShopID INT NOT NULL , --
      ShopCode VARCHAR(50) NULL ,
      ShopName VARCHAR(100) NULL ,
      StockCode VARCHAR(50) NULL ,
      StockName VARCHAR(100) NULL,
	 );
    CREATE TABLE #ShopMap
    (
      -- 店铺关系1
      VendShopID INT NOT NULL ,
      ShopID VARCHAR(20) NOT NULL ,
      CompanyID VARCHAR(20)
    );
	
    DECLARE @Start DATETIME = GETDATE(); --本次捞取数据的开始时间
    DECLARE @End DATETIME = GETDATE();   --本次捞取数据的结束时间
    DECLARE @HisDays INT = 60;	-- 历史数据保留60天

    DECLARE @IsAuto BIT = 1; --提取方案方式 1：自动：0:手动
    DECLARE @SolutionID INT = 0; --提报方案ID
	
	-- 供应商店铺
    INSERT  INTO #VendShop
            SELECT  ID, ShopCode, ShopName, StockCode, StockName
            FROM    HK_ERP_SG_DataGather_JLYJ.dbo.Bas_VendShop
            WHERE   VendorID = @VendorID;
		
	-- 店铺映射关系
    INSERT  INTO #ShopMap
            SELECT  VendShopID, ShopID, CompanyID
            FROM    HK_ERP_SG_DataGather_JLYJ.dbo.Bas_ShopMap a
            INNER JOIN HK_ERP_SG_DataGather_JLYJ.dbo.Bas_VendShop b ON a.VendShopID = b.ID
            WHERE   b.VendorID = @VendorID;
	
    DELETE  FROM #Shop; --清空上一次循环的数据（取店铺）
    INSERT  INTO #Shop
            SELECT DISTINCT
                    ShopID
            FROM    HK_ERP_SG_DataGather_JLYJ.dbo.Gat_ShopRight
            WHERE   AllowUsed = 1 AND CompanyID = @CompanyID AND CardID = @CardID AND ISNULL(VendorID,
                                                              -2) = @VendorID;	
	--①.先获取方案内容
    SELECT TOP 1
            @IsAuto = ISNULL(IsAuto,1), @SolutionID = ID
    FROM    HK_ERP_SG_DataGather_JLYJ.dbo.Gat_Solution
    WHERE   ISNULL(VendorID, -2) = @VendorID AND CompanyID = @CompanyID AND CardID = @CardID;

    IF @GatType = 0 --补采
        BEGIN
            SET @Start = @StartTime;
            SET @End = @EndTime;
        END;
    ELSE
        BEGIN
            SET @End = @DateNow;
            IF NOT EXISTS ( SELECT TOP 1
                                    1
                            FROM    HK_ERP_SG_DataGather_JLYJ.dbo.Sys_Log
                            WHERE   IsSucceed = 1 AND LogType = 1 AND ISNULL(VendorID,
                                                              0) = @VendorID AND CompanyID = @CompanyID AND CardID = @CardID ) --在这之前没有插入过数据
                BEGIN
                    SELECT TOP 1
                            @Start = RepeatStartTime
                    FROM    HK_ERP_SG_DataGather_JLYJ.dbo.Gat_Solution
                    WHERE   ISNULL(VendorID, -2) = @VendorID AND CompanyID = @CompanyID AND CardID = @CardID;
                END;
            ELSE
                BEGIN
                    SELECT  @Start = MAX(ModifyDTM)
                    FROM    HK_ERP_SG_DataGather_JLYJ.dbo.Sys_Log
                    WHERE   IsSucceed = 1 AND LogType = 1 AND ISNULL(VendorID,
                                                              -2) = @VendorID AND CompanyID = @CompanyID AND CardID = @CardID;--成功提取数据的日志
                END;
        END;	
	
	--删除指定范围之内的数据，防止补采产生重复数据
    DELETE  FROM HK_ERP_SG_DataGather_JLYJ.dbo.Gat_Columbia_PosSale
    WHERE   CompanyID = @CompanyID AND CardID = @CardID AND BillTime > @Start AND BillTime <= @End;	
    --删除历史数据
    DELETE  FROM HK_ERP_SG_DataGather_JLYJ.dbo.Gat_Columbia_PosSale
    WHERE   CompanyID = @CompanyID AND CardID = @CardID AND BillTime <= DATEADD(DAY,
                                                              @HisDays * -1,
                                                              CONVERT(VARCHAR(10), GETDATE(), 120));
    IF ( @IsAuto = 0 )--手动数据去到中转库
        BEGIN
	        -- 开始插入数据
            INSERT  INTO HK_ERP_SG_DataGather_JLYJ.dbo.Gat_Columbia_PosSale_transfer ( ShopCode,ShopName,
                                                              BillDate,
                                                              BillTime,
                                                              MaterialCode,
                                                              Qty, BillNo,
                                                              SalesAmount,
															  BalaPrice,
                                                              PersonnelID,
                                                              BarCode,
                                                              SizeName,
                                                              CompanyID,
                                                              CompanyName,
                                                              CardID, CardName,
                                                              SolutionID,StockName )
                    SELECT  ISNULL(h.ShopCode, d.ShopCode),ISNULL(h.ShopName, d.ShopName), b.BillDate,
                            b.BillTime, c.MaterialCode, a.Qty, b.BillNo,
                            a.PayAmount AS SalesAmount,
							a.BalaPrice,
							 b.PersonnelID,  
                            a.BarCode, f.SizeName, a.CompanyID, j.ShortName,
                            c.CardID, i.CardName, @SolutionID,h.StockName
                    FROM    SD_POS_SaleDetail a
                    INNER JOIN SD_POS_SaleMaster b ON a.CompanyID = b.CompanyID AND a.BillNo = b.BillNo
                    INNER JOIN SD_Mat_Material c ON a.MaterialID = c.MaterialID
                    INNER JOIN Bas_Shop d ON a.ShopID = d.ShopID AND a.CompanyID = d.CompanyID
                    INNER JOIN SD_Bas_ShopCard e ON d.CompanyID = e.CompanyID AND d.ShopID = e.ShopID AND c.CardID = e.CardID
                    INNER JOIN #Shop k ON a.ShopID = k.ShopID
                    INNER JOIN SD_Mat_Size f ON a.SizeID = f.SizeID
                    LEFT JOIN Bas_Company j ON a.CompanyID = j.CompanyID
                    LEFT JOIN SD_Mat_Card i ON c.CardID = i.CardID
                    LEFT JOIN #ShopMap g ON a.CompanyID = g.CompanyID AND a.ShopID = g.ShopID -- 关联到店铺映射
                    LEFT JOIN #VendShop h ON g.VendShopID = h.VendShopID
                    WHERE   a.CompanyID = @CompanyID AND e.CardID = @CardID AND b.BillTime > @Start AND b.BillTime <= @End
                    UNION ALL
                    SELECT  ISNULL(h.ShopCode, d.ShopCode),ISNULL(h.ShopName, d.ShopName), b.BillDate,
                            b.BillTime, c.MaterialCode, a.Qty, b.BillNo,
                            a.PayAmount AS SaleAmount, 
							a.BalaPrice,
							b.PersonnelID,
                            a.BarCode, f.SizeName, a.CompanyID, j.ShortName,
                            c.CardID, i.CardName,@SolutionID, h.StockName
                    FROM    SD_POS_SaleDetail2 a
                    INNER JOIN SD_POS_SaleMaster2 b ON a.CompanyID = b.CompanyID AND a.BillNo = b.BillNo
                    INNER JOIN SD_Mat_Material c ON a.MaterialID = c.MaterialID
                    INNER JOIN Bas_Shop d ON a.ShopID = d.ShopID AND a.CompanyID = d.CompanyID
                    INNER JOIN SD_Bas_ShopCard e ON d.CompanyID = e.CompanyID AND d.ShopID = e.ShopID AND c.CardID = e.CardID
                    INNER JOIN #Shop k ON a.ShopID = k.ShopID
                    INNER JOIN SD_Mat_Size f ON a.SizeID = f.SizeID
                    LEFT JOIN Bas_Company j ON a.CompanyID = j.CompanyID
                    LEFT JOIN SD_Mat_Card i ON c.CardID = i.CardID
                    LEFT JOIN #ShopMap g ON a.CompanyID = g.CompanyID AND a.ShopID = g.ShopID -- 关联到店铺映射
                    LEFT JOIN #VendShop h ON g.VendShopID = h.VendShopID
                    WHERE   a.CompanyID = @CompanyID AND e.CardID = @CardID AND b.BillTime > @Start AND b.BillTime <= @End;
        END;
    IF ( @IsAuto = 1 )--自动数据去到真实库
        BEGIN
			-- 开始插入数据
            INSERT  INTO HK_ERP_SG_DataGather_JLYJ.dbo.Gat_Columbia_PosSale ( ShopCode,ShopName,
                                                              BillDate,
                                                              BillTime,
                                                              MaterialCode,
                                                              Qty, BillNo,
                                                              SalesAmount,
															  BalaPrice,
                                                              PersonnelID,
                                                              BarCode,
                                                              SizeName,
                                                              CompanyID,
                                                              CompanyName,
                                                              CardID, CardName,
                                                              SolutionID,StockName )
                    SELECT  ISNULL(h.ShopCode, d.ShopCode),ISNULL(h.ShopName, d.ShopName), b.BillDate,
                            b.BillTime, c.MaterialCode, a.Qty, b.BillNo,
                            a.PayAmount AS SalesAmount,
							a.BalaPrice,
							b.PersonnelID,
                            a.BarCode, f.SizeName, a.CompanyID, j.ShortName,
                            c.CardID, i.CardName, @SolutionID,h.StockName
                    FROM    SD_POS_SaleDetail a
                    INNER JOIN SD_POS_SaleMaster b ON a.CompanyID = b.CompanyID AND a.BillNo = b.BillNo
                    INNER JOIN SD_Mat_Material c ON a.MaterialID = c.MaterialID
                    INNER JOIN Bas_Shop d ON a.ShopID = d.ShopID AND a.CompanyID = d.CompanyID
                    INNER JOIN SD_Bas_ShopCard e ON d.CompanyID = e.CompanyID AND d.ShopID = e.ShopID AND c.CardID = e.CardID
                    INNER JOIN #Shop k ON a.ShopID = k.ShopID
                    INNER JOIN SD_Mat_Size f ON a.SizeID = f.SizeID
                    LEFT JOIN Bas_Company j ON a.CompanyID = j.CompanyID
                    LEFT JOIN SD_Mat_Card i ON c.CardID = i.CardID
                    LEFT JOIN #ShopMap g ON a.CompanyID = g.CompanyID AND a.ShopID = g.ShopID -- 关联到店铺映射
                    LEFT JOIN #VendShop h ON g.VendShopID = h.VendShopID
                    WHERE   a.CompanyID = @CompanyID AND e.CardID = @CardID AND b.BillTime > @Start AND b.BillTime <= @End
                    UNION ALL
                    SELECT  ISNULL(h.ShopCode, d.ShopCode),ISNULL(h.ShopName, d.ShopName), b.BillDate,
                            b.BillTime, c.MaterialCode, a.Qty, b.BillNo,
                            a.PayAmount AS SaleAmount, 
							a.BalaPrice,
							b.PersonnelID,
                            a.BarCode, f.SizeName, a.CompanyID, j.ShortName,
                            c.CardID, i.CardName,@SolutionID,h.StockName
                    FROM    SD_POS_SaleDetail2 a
                    INNER JOIN SD_POS_SaleMaster2 b ON a.CompanyID = b.CompanyID AND a.BillNo = b.BillNo
                    INNER JOIN SD_Mat_Material c ON a.MaterialID = c.MaterialID
                    INNER JOIN Bas_Shop d ON a.ShopID = d.ShopID AND a.CompanyID = d.CompanyID
                    INNER JOIN SD_Bas_ShopCard e ON d.CompanyID = e.CompanyID AND d.ShopID = e.ShopID AND c.CardID = e.CardID
                    INNER JOIN #Shop k ON a.ShopID = k.ShopID
                    INNER JOIN SD_Mat_Size f ON a.SizeID = f.SizeID
                    LEFT JOIN Bas_Company j ON a.CompanyID = j.CompanyID
                    LEFT JOIN SD_Mat_Card i ON c.CardID = i.CardID
                    LEFT JOIN #ShopMap g ON a.CompanyID = g.CompanyID AND a.ShopID = g.ShopID -- 关联到店铺映射
                    LEFT JOIN #VendShop h ON g.VendShopID = h.VendShopID
                    WHERE   a.CompanyID = @CompanyID AND e.CardID = @CardID AND b.BillTime > @Start AND b.BillTime <= @End;
			  
        END;
    TRUNCATE TABLE #Shop;
    TRUNCATE TABLE #VendShop;
    TRUNCATE TABLE #ShopMap;
    DROP TABLE #Shop;
    DROP TABLE #ShopMap;
    DROP TABLE #VendShop;
	
		 
END;


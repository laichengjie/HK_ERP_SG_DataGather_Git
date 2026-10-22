IF OBJECT_ID ('Gat_LiNing_MeterialINV', 'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[Gat_LiNing_MeterialINV](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[SolutionId] [INT] NOT NULL,
		[ShopCode] [varchar](208) NOT NULL,
		[ShopName] [varchar](208) NOT NULL,
		[UpdateDate] [datetime] NULL,
		[BarCode] [varchar](128) NOT NULL,
		[MaterialCode] [varchar](128) NOT NULL,
		[SizeName] [varchar](50) NULL,
		[Qty] [numeric](24, 6) NULL,
		[CompanyID] [varchar](20) NULL,
		[CompanyName] [varchar](20) NULL,
		[CardID] [varchar](20) NULL,
		[CardName] [varchar](20) NULL
	) ON [PRIMARY]
	END
GO

Create PROCEDURE [dbo].[sp_SGIS_LiNing_GetMaterailINV]
    @VendorID INT = -1 ,--供应商ID
    @GatType INT = 1 , --采集类型，1：正常采集；0：补采
    @CompanyID NVARCHAR(20) = '' ,--公司ID 
    @CardID NVARCHAR(20) = '' ,-- 品牌ID 
    @StartTime DATETIME , -- 补采时开始时间
    @EndTime DATETIME , --补采结束时间
    @DateNow DATETIME,   --当前开始采集的时间
	@SolutionID int
AS
BEGIN
	
    CREATE TABLE #Stock ( StockID NVARCHAR(50) ); --提报仓库
	CREATE TABLE #Shop ( ShopID NVARCHAR(50),tradeStoreCode NVARCHAR(50) ); --提报店铺
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
	 CREATE TABLE #StockMap
    (
      -- 店铺关系1
      VendStockID INT NOT NULL ,
      StockID VARCHAR(30) NOT NULL ,
      CompanyID VARCHAR(20)
    );
	-- 供应商店铺
    INSERT  INTO #VendShop ( VendShopID, ShopCode, ShopName, StockCode,
                             StockName )
            SELECT  ID, ShopCode, ShopName, StockCode, StockName
            FROM    HK_ERP_SG_DataGather_XAJX.dbo.Bas_VendShop
            WHERE   VendorID = @VendorID;
		
	-- 店铺映射关系
    INSERT  INTO #ShopMap ( VendShopID, ShopID, CompanyID )
            SELECT  VendShopID, ShopID, CompanyID
            FROM    HK_ERP_SG_DataGather_XAJX.dbo.Bas_ShopMap a
            INNER JOIN HK_ERP_SG_DataGather_XAJX.dbo.Bas_VendShop b ON a.VendShopID = b.ID
            WHERE   b.VendorID = @VendorID;
	
    DELETE  FROM #Shop; --清空上一次循环的数据
    INSERT  INTO #Shop ( ShopID ,tradeStoreCode)
            SELECT DISTINCT
                    ShopID,TradeCode
            FROM    HK_ERP_SG_DataGather_XAJX.dbo.Gat_ShopRight
            WHERE   AllowUsed = 1 AND CompanyID = @CompanyID AND CardID = @CardID AND ISNULL(VendorID,
                                                              -2) = @VendorID;

	 -- 仓库映射关系										
    INSERT  INTO #StockMap ( VendStockID, StockID, CompanyID )
            SELECT  VendStockID, StockID, CompanyID
            FROM    HK_ERP_SG_DataGather_XAJX.dbo.Bas_StockMap a
            INNER JOIN HK_ERP_SG_DataGather_XAJX.dbo.Bas_VendShop b ON a.VendStockID = b.ID
            WHERE   b.VendorID = @VendorID;
	
    DELETE  FROM #Stock; --清空上一次循环的数据
    INSERT  INTO #Stock ( StockID )
            SELECT DISTINCT
                    StockID
            FROM    HK_ERP_SG_DataGather_XAJX.dbo.Gat_StockRight
            WHERE   AllowUsed = 1 AND CompanyID = @CompanyID AND CardID = @CardID AND ISNULL(VendorID,
                                                              -2) = @VendorID;
															  
															  		
	 -- 删除以前捞取的库存数据，点库存捞取
    DELETE  FROM HK_ERP_SG_DataGather_XAJX.dbo.Gat_LiNing_MeterialINV
    WHERE   1 = 1 AND CompanyID = @CompanyID AND CardID = @CardID;
	
	-- 开始插入数据  select * from   HK_ERP_SG_DataGather_XAJX.[dbo].[Gat_Asics_MeterialINV]
    INSERT  INTO HK_ERP_SG_DataGather_XAJX.dbo.Gat_LiNing_MeterialINV ( SolutionId,ShopCode,
	                                                          ShopName,
                                                              UpdateDate,
                                                              BarCode,
                                                              MaterialCode,
                                                              SizeName, Qty,
                                                              CompanyID,
                                                              CompanyName,
                                                              CardID)
            SELECT  @SolutionID,m.tradeStoreCode,SK.StockName, GETDATE() AS UpdateDate,
                    d.BarCode, a.MaterialID, f.SizeName, b.ActualQty AS Qty,--点库存
                    j.CompanyID, j.ShortName, g.CardID
            FROM    SD_Inv_MaterialMaster a
            JOIN    SD_Inv_MaterialDetailSize b ON a.CompanyID = b.CompanyID AND a.StockID = b.StockID AND a.MaterialID = b.MaterialID
            INNER JOIN SD_Mat_Material g ON a.MaterialID = g.MaterialID
            JOIN    Bas_Shop c ON a.StockID = c.StockID AND a.CompanyID = c.CompanyID
			inner join #Shop m on c.ShopID=m.ShopID
            JOIN    SD_Mat_MaterialSize d ON b.SizeID = d.SizeID AND b.MaterialID = d.MaterialID
            JOIN    SD_Bas_ShopCard e ON c.CompanyID = e.CompanyID AND c.ShopID = e.ShopID AND e.CardID = g.CardID
            LEFT JOIN SD_Mat_Size f ON d.SizeID = f.SizeID
			LEFT JOIN Bas_Stock sk ON sk.StockID=a.StockID and sk.CompanyID=a.CompanyID
            LEFT JOIN Bas_Company j ON a.CompanyID = j.CompanyID
            LEFT JOIN SD_Mat_Card k ON g.CardID = k.CardID
            LEFT JOIN #ShopMap i ON c.CompanyID = i.CompanyID AND c.ShopID = i.ShopID -- 关联到店铺映射
            LEFT JOIN #VendShop h ON i.VendShopID = h.VendShopID
            WHERE   a.CompanyID = @CompanyID AND e.CardID = @CardID
			union  
		   SELECT  @SolutionID,ISNULL(h1.ShopCode, sk.StockCode),SK.StockName, GETDATE() AS UpdateDate,
                    d.BarCode, a.MaterialID, f.SizeName, b.ActualQty AS Qty,--点库存
                    j.CompanyID, j.ShortName, g.CardID
            FROM    SD_Inv_MaterialMaster a
            JOIN    SD_Inv_MaterialDetailSize b ON a.CompanyID = b.CompanyID AND a.StockID = b.StockID AND a.MaterialID = b.MaterialID
		
             JOIN SD_Mat_Material g ON a.MaterialID = g.MaterialID
		    JOIN Bas_Stock sk ON sk.StockID=a.StockID and sk.CompanyID=a.CompanyID
            JOIN    SD_Mat_MaterialSize d ON b.SizeID = d.SizeID AND b.MaterialID = d.MaterialID
		    JOIN    Bas_StockCard sc on sk.CompanyID = sc.CompanyID AND sk.StockID = sc.StockID and sc.CardID=g.CardID
            LEFT JOIN SD_Mat_Size f ON d.SizeID = f.SizeID
            LEFT JOIN Bas_Company j ON a.CompanyID = j.CompanyID
            LEFT JOIN SD_Mat_Card k ON g.CardID = k.CardID
			LEFT JOIN #StockMap si on sk.CompanyID=si.CompanyID  and sk.StockID=si.StockID
			LEFT JOIN #VendShop h1 ON  si.VendStockID = h1.VendShopID
			WHERE   a.CompanyID = @CompanyID   AND sk.StockID IN ( SELECT
                                                              StockID
                                                              FROM
                                                              #Stock)   AND sc.CardID = @CardID

	
 	TRUNCATE TABLE #Stock;
	TRUNCATE TABLE #StockMap;
    TRUNCATE TABLE #Shop;
    TRUNCATE TABLE #VendShop;
    TRUNCATE TABLE #ShopMap;
    DROP TABLE #Shop;
    DROP TABLE #ShopMap;
    DROP TABLE #VendShop;
	DROP TABLE #Stock;
    DROP TABLE #StockMap;
	
	
END;

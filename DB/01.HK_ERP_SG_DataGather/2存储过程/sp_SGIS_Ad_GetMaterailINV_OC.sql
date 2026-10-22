IF EXISTS (SELECT *
           FROM sys.objects
           WHERE object_id=OBJECT_ID(N'[dbo].[sp_SGIS_Ad_GetMaterailINV_OC]')AND type IN (N'P', N'PC'))
    DROP PROCEDURE [dbo].sp_SGIS_Ad_GetMaterailINV_OC;
GO

/* ==========================================================================================
描    述：阿迪全渠道 库存数据
创 建 人：lcj
创建日期：
说明：
更新说明：
-- ==========================================================================================*/
Create PROCEDURE [dbo].[sp_SGIS_Ad_GetMaterailINV_OC]
    @VendorID INT = -1,           --供应商ID
    @GatType INT = 1,             --采集类型，1：正常采集；0：补采
    @CompanyID NVARCHAR(20) = '', --公司ID 
    @CardID NVARCHAR(20) = '',    -- 品牌ID 
    @StartTime DATETIME,          -- 补采时开始时间
    @EndTime DATETIME,            --补采结束时间
    @DateNow DATETIME,            --当前开始采集的时间
    @IsTB INT = 0                 --是否三小时数据
AS
BEGIN
   

   declare @SolutionID int=0;
   SELECT top 1 @SolutionID= a.ID FROM   HK_ERP_SG_DataGather.dbo.Gat_Solution a
    WHERE CompanyID=@CompanyID 
	AND VendorID=@VendorID 
	AND CardID=@CardID

    CREATE TABLE #Stock
    (
        StockID NVARCHAR(50),
        isShop BIT,
		InventorySource VARCHAR(20)
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
    SELECT VendShopID,
           ShopID,
           CompanyID
    FROM HK_ERP_SG_DataGather.dbo.Bas_ShopMap a
        INNER JOIN HK_ERP_SG_DataGather.dbo.Bas_VendShop b
            ON a.VendShopID = b.ID
    WHERE b.VendorID = @VendorID;

    DELETE FROM #Stock;
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
          AND a.CardID = @CardID

     --大仓
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
		  and  not exists  (select top 1 1 from #Stock  s where s.StockID= a.StockID)

	Update  a  Set  a.InventorySource=  (case b.StockType when 0 then  'DC' else 'POS' end)   from #Stock a 
    inner join Bas_Stock b on b.StockID=a.StockID

    --如果店铺性质是“网店”，则对应店铺对应仓库的库存不推送
    --去掉限制 2026-04-03
    /*
    BEGIN
        --网店仓库
        CREATE TABLE #YDStock
        (
            StockID NVARCHAR(50)
        );
        INSERT INTO #YDStock
        (
            StockID
        )
        SELECT StockID
        FROM dbo.Bas_Shop a
        WHERE ISNULL(ShopType, 0) = 1
              AND ISNULL(a.StockID, '') > '';

        DELETE a
        FROM #Stock a
            INNER JOIN #YDStock b
                ON a.StockID = b.StockID;
    END;
    */

    IF OBJECT_ID('tempdB..#MeterialINV') IS NOT NULL
        DROP TABLE #MeterialINV;
    CREATE TABLE #MeterialINV
    (
        [ShopCode] [VARCHAR](20) NULL,
        [UpdateDate] [DATETIME] NULL,
        [BarCode] [VARCHAR](128) NOT NULL,
        [MaterialCode] [VARCHAR](50) NOT NULL,
        [SizeName] [VARCHAR](50) NULL,
        [Qty] [NUMERIC](24, 6) NULL,
        [YCQty] [NUMERIC](24, 6) NULL,
        [CompanyID] [VARCHAR](50) NULL,
        [CompanyName] [NVARCHAR](100) NULL,
        [CardID] [VARCHAR](20) NULL,
        [CardName] [VARCHAR](50) NULL,
        [StockID] [VARCHAR](50) NULL,   --仓库代码
        [StockCode] [VARCHAR](50) NULL, --仓库代码
        [StockName] [NVARCHAR](100) NULL,
        [SizeCode] [VARCHAR](50) NULL,
        [MaterialID] [VARCHAR](50) NOT NULL,
		[InventorySource] [VARCHAR](20) NULL,  --门店传POS 仓库传DC
		[SubModel] [nvarchar](60), --子款型
		[PCode] [VARCHAR](40) NULL,  --PCode
    );
    -- 开始插入数据
    -- 开始插入数据
    INSERT INTO #MeterialINV
    (
        ShopCode,
        UpdateDate,
        BarCode,
        MaterialCode,
        SizeName,
        Qty,
        YCQty,
        CompanyID,
        CompanyName,
        CardID,
        CardName,
        StockCode,
        StockName,
        SizeCode,
        MaterialID,
        StockID,
		InventorySource,
		SubModel
    )
    SELECT NULL ShopCode,
           GETDATE() AS UpdateDate,
           d.BarCode,
           f.MaterialCode,
           n.SizeName,
           b.ActualQty Qty,
           0 YCQty,
           a.CompanyID,
           j.CompanyName,
           f.CardID,
           i.CardName,
           l.StockCode,
           l.StockName,
           n.SizeCode,
           a.MaterialID,
           b.StockID,
		   k.InventorySource,
		   f.SubModel
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
            ON a.StockID = k.StockID --获得所有的仓库数据
    WHERE a.CompanyID = @CompanyID
          AND f.CardID = @CardID
          AND b.ActualQty <> 0
          AND k.isShop = 1;
    INSERT INTO #MeterialINV
    (
        ShopCode,
        UpdateDate,
        BarCode,
        MaterialCode,
        SizeName,
        Qty,
        YCQty,
        CompanyID,
        CompanyName,
        CardID,
        CardName,
        StockCode,
        StockName,
        SizeCode,
        MaterialID,
        StockID,
		InventorySource,
		SubModel
    )
    SELECT NULL ShopCode,
           UpdateDate,
           BarCode,
           MaterialCode,
           SizeName,
           SUM(Qty),
           0 YCQty,
           CompanyID,
           CompanyName,
           CardID,
           CardName,
           StockCode,
           StockName,
           SizeCode,
           MaterialID,
           StockID,
		   InventorySource,
		   SubModel
    FROM
    (
        SELECT NULL ShopCode,
               GETDATE() AS UpdateDate,
               d.BarCode,
               f.MaterialCode,
               n.SizeName,
               b.ActualQty Qty,
               0 YCQty,
               a.CompanyID,
               j.CompanyName,
               f.CardID,
               i.CardName,
               l.StockCode,
               l.StockName,
               n.SizeCode,
               a.MaterialID,
               b.StockID,
			   k.InventorySource,
			   f.SubModel
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
                ON a.StockID = k.StockID --获得所有的仓库数据
        WHERE a.CompanyID = @CompanyID
              AND f.CardID = @CardID
              AND b.ActualQty <> 0
              AND k.isShop = 0
    ) temp
    GROUP BY UpdateDate,
             BarCode,
             MaterialCode,
             SizeName,
             CompanyID,
             CompanyName,
             CardID,
             CardName,
             StockCode,
             StockName,
             SizeCode,
             MaterialID,
             StockID,
			 InventorySource,
			 SubModel



    -- PCode 取值
    IF OBJECT_ID('tempdB..#StockSubModelSet') IS NOT NULL
        DROP TABLE #StockSubModelSet;
    CREATE TABLE #StockSubModelSet
	(
       [StockCode] [varchar](40) ,
       [SubModel] [nvarchar](60) ,
	   [PCode] [varchar](40) ,
	)
	insert into #StockSubModelSet(StockCode,SubModel,PCode)
	select StockCode,SubModel,PCode from HK_ERP_SG_DataGather.dbo.Gat_StockSubModelSet a 
	where a.SolutionId=@SolutionID

	UPDATE a
    SET a.PCode = b.PCode
    FROM #MeterialINV a
        INNER JOIN #StockSubModelSet b
            ON b.StockCode = a.StockCode
               AND b.SubModel = a.SubModel
              


    --云仓库存
    IF OBJECT_ID('tempdB..#PostStockQty') IS NOT NULL
        DROP TABLE #PostStockQty;
    CREATE TABLE #PostStockQty
    (
        CompanyID VARCHAR(40),
        MaterialID VARCHAR(40),
        StockID VARCHAR(40),
        BarCode VARCHAR(40),
        YCQty NUMERIC(24, 6) NULL,
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
           ISNULL(a.Qty2, 0) - ISNULL(a.UsedQtyForSend, 0) YCQty
    FROM dbo.BC_OC_OCCloudStockInventoryDetail a
        INNER JOIN dbo.SD_Mat_MaterialSize c
            ON a.MaterialID = c.MaterialID
               AND a.SizeID = c.SizeID
        INNER JOIN #Stock k
            ON a.StockID = k.StockID
    WHERE a.CompanyID = @CompanyID
	and  a.CloudStockID='CS240509FAYAsys-001'


    UPDATE a
    SET a.YCQty = b.YCQty
    FROM #MeterialINV a
        INNER JOIN #PostStockQty b
            ON b.CompanyID = a.CompanyID
               AND b.MaterialID = a.MaterialID
               AND b.StockID = a.StockID
               AND b.BarCode = a.BarCode;

    DECLARE @DrewTime VARCHAR(20); --提报时间点
    SELECT @DrewTime = FORMAT(GETDATE(), 'yyyyMMddHH');
    IF @IsTB = 0
    BEGIN
        -- 删除以前捞取的库存数据，点库存捞取
        DELETE FROM HK_ERP_SG_DataGather.dbo.Gat_Ad_MeterialINV_OC
        WHERE CompanyID = @CompanyID
              AND CardID = @CardID;

        INSERT INTO HK_ERP_SG_DataGather.dbo.Gat_Ad_MeterialINV_OC
        (
            ShopCode,
            UpdateDate,
            BarCode,
            MaterialCode,
            SizeName,
            Qty,
            YCQty,
            CompanyID,
            CompanyName,
            CardID,
            CardName,
            StockCode,
            StockName,
            SizeCode,
            ModifyTime, --修改时间
            IsImport,   --是否導入
            DrewTime,    --提报时间点
			InventorySource,
			PCode
        )
        SELECT ShopCode,
               GETDATE() AS UpdateDate,
               BarCode,
               MaterialCode,
               SizeName,
               Qty,
               YCQty,
               CompanyID,
               CompanyName,
               CardID,
               CardName,
               StockCode,
               StockName,
               SizeCode,
               GETDATE() ModifyTime, --修改时间
               0 IsImport,           --是否導入
               @DrewTime DrewTime,    --提报时间点
			   InventorySource,
			   PCode
        FROM #MeterialINV;


    END;
    ELSE
    BEGIN

-- 声明批处理大小
DECLARE @BatchSize INT = 1000;
DECLARE @RowsAffected INT;
-- 初始设置 @RowsAffected 以进入循环
SET @RowsAffected = 1;
DECLARE @chishu INT = 0;
-- 开始循环
WHILE (@RowsAffected > 0)
BEGIN
    -- 批量删除
    DELETE TOP (@BatchSize)
    FROM  HK_ERP_SG_DataGather.dbo.Gat_Ad_MeterialINV_OCTB
  WHERE  ModifyTime <=  DATEADD(Hour,-72, CONVERT(VARCHAR(20), GETDATE(), 120));
    -- 检查删除的行数
    SET @RowsAffected = @@ROWCOUNT;

	set @chishu=@chishu+1
	select  @chishu ,@RowsAffected
    -- 可选：每批次删除后等待片刻，减轻服务器负载
     WAITFOR DELAY '00:00:01';  -- 等待 1 秒
END




        INSERT INTO HK_ERP_SG_DataGather.dbo.Gat_Ad_MeterialINV_OCTB
        (
            ShopCode,
            UpdateDate,
            BarCode,
            MaterialCode,
            SizeName,
            Qty,
            YCQty,
            CompanyID,
            CompanyName,
            CardID,
            CardName,
            StockCode,
            StockName,
            SizeCode,
            ModifyTime, --修改时间
            IsImport,   --是否導入
            DrewTime,    --提报时间点
			InventorySource,
			PCode
        )
        SELECT ShopCode,
               GETDATE() AS UpdateDate,
               BarCode,
               MaterialCode,
               SizeName,
               Qty,
               YCQty,
               CompanyID,
               CompanyName,
               CardID,
               CardName,
               StockCode,
               StockName,
               SizeCode,
               GETDATE() ModifyTime, --修改时间
               0 IsImport,           --是否導入
               @DrewTime DrewTime,   --提报时间点
			   InventorySource,
			   PCode
        FROM #MeterialINV;
    END;

    TRUNCATE TABLE #Stock;
    TRUNCATE TABLE #VendShop;
    TRUNCATE TABLE #ShopMap;
    DROP TABLE #Stock;
    DROP TABLE #ShopMap;
    DROP TABLE #VendShop;
END;

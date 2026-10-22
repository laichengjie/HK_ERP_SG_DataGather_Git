IF EXISTS (SELECT *
           FROM sys.objects
           WHERE object_id=OBJECT_ID(N'[dbo].[sp_SGIS_Ad_GetOutIn_OC]')AND type IN (N'P', N'PC'))
    DROP PROCEDURE [dbo].sp_SGIS_Ad_GetOutIn_OC;
GO
/* ==========================================================================================
描    述：阿迪全渠道出入库单数据
创 建 人：lcj
创建日期：2023-08-24
说明：
更新说明：
-- ==========================================================================================*/
Create PROCEDURE [dbo].[sp_SGIS_Ad_GetOutIn_OC]
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

    IF OBJECT_ID('tempdB..#ShopStock') IS NOT NULL
        DROP TABLE #ShopStock;
    CREATE TABLE #ShopStock
    (
        CompanyID VARCHAR(50) NULL,
        ShopID VARCHAR(50) NULL,
        ShopCode VARCHAR(50) NULL,  --仓库代码
        ShopName NVARCHAR(100) NULL,
        StockID VARCHAR(50) NULL,   --仓库代码
        StockCode VARCHAR(50) NULL, --仓库代码
        StockName NVARCHAR(100) NULL,
        StockType INT NULL, --仓库类型  0仓库  1店铺
		IsShop bit null --是否店铺 1店铺 0大仓
    ); --提报店铺



    DECLARE @Start DATETIME = GETDATE(); --本次捞取数据的开始时间
    DECLARE @End DATETIME = GETDATE(); --本次捞取数据的结束时间
    DECLARE @HisDays INT = 60; -- 历史数据保留60天

    DECLARE @IsAuto BIT = 1; --提取方案方式 1：自动：0:手动
    DECLARE @SolutionID INT = 0; --提报方案ID
    DECLARE @DrewTime VARCHAR(20); --提报时间点

    /*
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
        ShopID VARCHAR(20) NOT NULL,
        CompanyID VARCHAR(20)
    );

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
	*/
	--店仓
    DELETE FROM #ShopStock; --清空上一次循环的数据（取店铺）
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
		a.IsShop=1
    FROM #ShopStock a
        INNER JOIN dbo.Bas_Shop b
            ON b.CompanyID = a.CompanyID
               AND b.ShopID = a.ShopID
        LEFT JOIN dbo.Bas_Stock c
            ON c.StockID = b.StockID
               AND c.CompanyID = b.CompanyID;

    --大仓
   insert into #ShopStock( 
        CompanyID,
        ShopID ,
        ShopCode ,  --仓库代码
        ShopName ,
        StockID ,   --仓库代码
        StockCode , --仓库代码
        StockName ,
        StockType , --仓库类型  0仓库  1店铺
		IsShop  --是否店铺 1店铺 0大仓
		)
    SELECT DISTINCT
        a.CompanyID,
        ''ShopID ,
        ''ShopCode ,  --仓库代码
        '' ShopName ,
        a.StockID ,   --仓库代码
        b.StockCode,
        b.StockName,
        b.StockType,
		0 IsShop  --是否店铺 1店铺 0大仓)
    FROM HK_ERP_SG_DataGather.dbo.Gat_StockRight a
	LEFT JOIN dbo.Bas_Stock b ON b.StockID = a.StockID  AND b.CompanyID = a.CompanyID
    WHERE  a.AllowUsed = 1 AND a.CompanyID = @CompanyID
          AND ISNULL(a.VendorID, -2) = @VendorID
          AND a.CardID = @CardID
          AND NOT EXISTS
         (
          SELECT TOP 1 1 FROM #ShopStock b WHERE b.StockID = a.StockID
          );   

    --①.先获取方案内容
    SELECT TOP 1
           @IsAuto = ISNULL(IsAuto, 1),
           @SolutionID = ID
    FROM HK_ERP_SG_DataGather.dbo.Gat_Solution
    WHERE ISNULL(VendorID, -2) = @VendorID
          AND CompanyID = @CompanyID
          AND CardID = @CardID;

    IF @IsTB = 1
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
    IF OBJECT_ID('tempdb..#OutIn', 'U') IS NOT NULL
        DROP TABLE #OutIn;
    CREATE TABLE #OutIn
    (
        id INT IDENTITY(1, 1) NOT NULL,     --序号
        SolutionID INT NOT NULL,            --方案ID
        CompanyID VARCHAR(20) NULL,         --公司
        CompanyName NVARCHAR(100) NULL,     --公司
        BillNo VARCHAR(40) NOT NULL,        --单号
        ShopCode VARCHAR(50) NOT NULL,      --店铺代码
        ShopName NVARCHAR(100) NULL,        --店铺名称
        OutStockCode VARCHAR(50) NULL,      --仓库代码
        OutStockName NVARCHAR(100) NULL,
        OutStocType VARCHAR(100) NULL,      --出库仓类型/发货仓类型
        InStockCode VARCHAR(50) NULL,       --仓库代码
        InStockName NVARCHAR(100) NULL,
        InStocType VARCHAR(100) NULL,       --入库仓类型/入库仓类型
        BillDate DATETIME NOT NULL,         --单据生效日期
        BillTime DATETIME NOT NULL,         --制单时间
        MaterialCode VARCHAR(50) NULL,      --货号
        BarCode VARCHAR(128) NULL,          --条码
        SizeCode VARCHAR(50) NULL,          --尺码
        SizeName NVARCHAR(100) NULL,        --尺码名称
        Qty NUMERIC(10, 2) NOT NULL,        --数量
        ModifyTime DATETIME NULL,           --修改时间
        BillTypeCode VARCHAR(40) NOT NULL,  --单据类型代码
        BillTypeName NVARCHAR(40) NOT NULL, --单据类型名称
        DrewTime VARCHAR(20) NULL,          --提报时间点
        BillTypeID VARCHAR(20) NULL,        --提报时间点
        CardID VARCHAR(20) NULL,            --品牌
        CardName VARCHAR(20) NULL,          --品牌名稱
        Adidas VARCHAR(20) NULL, -- 门店编号 盘盈盘亏的仓库代码填写此处
    );


    IF @IsTB = 1
    BEGIN
        BEGIN --采购入库单  采购退货出库单 盘盈入库单 盘库出库单
            INSERT INTO #OutIn
            (
                SolutionID,   --方案ID
                CompanyID,    --公司
                BillNo,       --单号
                ShopCode,     --店铺代码
                ShopName,     --店铺名称
                OutStockCode, --仓库代码
                OutStockName,
                OutStocType,  --出库仓类型/发货仓类型
                InStockCode,  --仓库代码
                InStockName,
                InStocType,   --入库仓类型/入库仓类型
                BillDate,     --单据生效日期
                BillTime,     --制单时间
                MaterialCode, --货号
                BarCode,      --条码
                SizeCode,     --尺码
                SizeName,     --尺码名称
                Qty,          --数量
                ModifyTime,   --修改时间
                BillTypeCode, --单据类型代码
                BillTypeName, --单据类型名称
                BillTypeID,   --提报时间点
                CardID,       --品牌
                Adidas        -- 门店编号 盘盈盘亏的仓库代码填写此处
            )
            SELECT @SolutionID SolutionID,       --方案ID
                   a.CompanyID,                  --公司ID
                   b.BillNo,                     --单号
                   d.ShopCode ShopCode,          --店铺代码
                   d.ShopName ShopName,          --店铺名称
                   (CASE
                        WHEN m.BillTypeID = 'Inv_Thck' THEN
                            d.StockCode
                        WHEN m.BillTypeID = 'Inv_Pkck' THEN
                            d.StockCode
                        ELSE
                            'ADDC'
                    END
                   ) OutStockCode,               --仓库代码
                   (CASE
                        WHEN m.BillTypeID = 'Inv_Thck' THEN
                            d.StockName
                        WHEN m.BillTypeID = 'Inv_Pkck' THEN
                            d.StockName
                        ELSE
                            'ADDC'
                    END
                   ) OutStockName,               --仓库名称
                   NULL OutStocType,             --出库仓类型/发货仓类型
                   (CASE
                        WHEN m.BillTypeID = 'Inv_Cgrk' THEN
                            d.StockCode
                        WHEN m.BillTypeID = 'Inv_Pyrk' THEN
                            d.StockCode
                        ELSE
                            'ADDC'
                    END
                   ) InStockCode,                --仓库代码
                   (CASE
                        WHEN m.BillTypeID = 'Inv_Cgrk' THEN
                            d.StockName
                        WHEN m.BillTypeID = 'Inv_Pyrk' THEN
                            d.StockName
                        ELSE
                            'ADDC'
                    END
                   ) InStockName,                --仓库名称
                   NULL InStocType,              --入库仓类型/入库仓类型
                   m.CheckDate BillDate,         --销售日期
                   GETDATE() BillTime,           --导入时间
                   c.MaterialCode,               --货号
                   ms.BarCode,                   --条码
                   f.SizeCode,                   --尺码
                   f.SizeName,                   --尺码名称
                   a.Qty,                        --数量
                   GETDATE() ModifyTime,         --更新时间
                   (CASE
                        WHEN m.BillTypeID = 'Inv_Cgrk' THEN
                            '101'
                        WHEN m.BillTypeID = 'Inv_Thck' THEN
                            '102'
                        WHEN m.BillTypeID = 'Inv_Pyrk' THEN
                            '701'
                        WHEN m.BillTypeID = 'Inv_Pkck' THEN
                            '702'
                        ELSE
                            ''
                    END
                   ) BillTypeCode,               --单据类型代码
                   bt.BillTypeName BillTypeName, --单据类型
                   m.BillTypeID,                 --单据类型
                   @CardID CardID,               --品牌
                   (CASE
                        WHEN m.BillTypeID = 'Inv_Pyrk' THEN
                            '701'
                        WHEN m.BillTypeID = 'Inv_Pkck' THEN
                            '702'
                        ELSE
                            ''
                    END
                   ) Adidas
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
                       AND a.CompanyID = d.CompanyID --and d.IsShop=1
                --INNER JOIN dbo.SD_Bas_ShopCard e
                --    ON d.CompanyID = e.CompanyID
                --       AND d.ShopID = e.ShopID
                --       AND c.CardID = e.CardID
                INNER JOIN dbo.SD_Mat_Size f
                    ON a.SizeID = f.SizeID
                LEFT JOIN dbo.Bas_Company j
                    ON a.CompanyID = j.CompanyID
                LEFT JOIN dbo.SD_Mat_Card i
                    ON c.CardID = i.CardID
                LEFT JOIN dbo.Sys_BillType bt
                    ON bt.BillTypeID = m.BillTypeID
                JOIN dbo.SD_Mat_MaterialSize ms
                    ON ms.SizeID = a.SizeID
                       AND ms.MaterialID = a.MaterialID
            WHERE m.CompanyID = @CompanyID
                  AND c.CardID = @CardID
                  AND m.CheckDate > @Start
                  AND m.CheckDate <= @End
                  AND m.BillTypeID IN ( 'Inv_Cgrk', 'Inv_Thck', 'Inv_Pyrk', 'Inv_Pkck' )
				  AND m.BillStatus=4
				  ;
        END;
        BEGIN --调拨出库单
            INSERT INTO #OutIn
            (
                SolutionID,   --方案ID
                CompanyID,    --公司
                BillNo,       --单号
                ShopCode,     --店铺代码
                ShopName,     --店铺名称
                OutStockCode, --仓库代码
                OutStockName,
                OutStocType,  --出库仓类型/发货仓类型
                InStockCode,  --仓库代码
                InStockName,
                InStocType,   --入库仓类型/入库仓类型
                BillDate,     --单据生效日期
                BillTime,     --制单时间
                MaterialCode, --货号
                BarCode,      --条码
                SizeCode,     --尺码
                SizeName,     --尺码名称
                Qty,          --数量
                ModifyTime,   --修改时间
                BillTypeCode, --单据类型代码
                BillTypeName, --单据类型名称
                BillTypeID,   --提报时间点
                CardID,       --品牌
                Adidas        -- 门店编号 盘盈盘亏的仓库代码填写此处
            )
            SELECT @SolutionID SolutionID,   --方案ID
                   a.CompanyID,              --公司ID
                   b.BillNo,                 --单号
                    '' ShopCode,      --店铺代码
               '' ShopName,      --店铺名称
               dd.StockCode OutStockCode, --仓库代码
               dd.StockName OutStockName, --仓库名称
               dd.StockType OutStocType,  --出库仓类型/发货仓类型
               k.StockCode InStockCode,  --仓库代码
               k.StockName InStockName,  --仓库名称
               k.StockType InStocType,   --入库仓类型/入库仓类型
                   m.OutCheckDate BillDate,  --销售日期
                   GETDATE() BillTime,       --导入时间
                   c.MaterialCode,           --货号
                   ms.BarCode,               --条码
                   f.SizeCode,               --尺码
                   f.SizeName,               --尺码名称
                   a.OutQty Qty,             --数量
                   GETDATE() ModifyTime,     --更新时间
                   '301' BillTypeCode,
                   '调拨出库单' BillTypeName,     --单据类型
                   'Inv_MovOut' BillTypeID,  --单据类型
                   @CardID CardID,           --品牌
                   NULL Adidas
            FROM dbo.SD_Inv_MoveSize a
                INNER JOIN dbo.SD_Inv_MoveDetail b
                    ON b.CompanyID = a.CompanyID
                       AND b.BillNo = a.BillNo
                       AND b.Sequence = a.Sequence
                INNER JOIN dbo.SD_Inv_MoveMaster m
                    ON a.CompanyID = m.CompanyID
                       AND a.BillNo = m.BillNo
                INNER JOIN dbo.SD_Mat_Material c
                    ON a.MaterialID = c.MaterialID
                INNER JOIN dbo.SD_Mat_Size f
                    ON a.SizeID = f.SizeID
                LEFT JOIN dbo.Bas_Company j
                    ON a.CompanyID = j.CompanyID
                LEFT JOIN dbo.SD_Mat_Card i
                    ON c.CardID = i.CardID
                 LEFT JOIN dbo.Bas_Stock dd
                ON dd.CompanyID = m.CompanyID
                   AND dd.StockID = m.OutStockID
            LEFT JOIN dbo.Bas_Stock k
                ON k.CompanyID = m.CompanyID
                   AND k.StockID = m.InStockID
                JOIN dbo.SD_Mat_MaterialSize ms
                    ON ms.SizeID = a.SizeID
                       AND ms.MaterialID = a.MaterialID
            WHERE m.CompanyID = @CompanyID
                  AND c.CardID = @CardID
				   and  exists (select top 1 1 from #ShopStock d where d.StockID in( m.OutStockID,m.InStockID)AND a.CompanyID = d.CompanyID)
                  AND m.OutCheckDate > @Start
                  AND m.OutCheckDate <= @End
                  AND m.OutBillStatus = 4;

        END;
        BEGIN --调拨入库单
            INSERT INTO #OutIn
            (
                SolutionID,   --方案ID
                CompanyID,    --公司
                BillNo,       --单号
                ShopCode,     --店铺代码
                ShopName,     --店铺名称
                OutStockCode, --仓库代码
                OutStockName,
                OutStocType,  --出库仓类型/发货仓类型
                InStockCode,  --仓库代码
                InStockName,
                InStocType,   --入库仓类型/入库仓类型
                BillDate,     --单据生效日期
                BillTime,     --制单时间
                MaterialCode, --货号
                BarCode,      --条码
                SizeCode,     --尺码
                SizeName,     --尺码名称
                Qty,          --数量
                ModifyTime,   --修改时间
                BillTypeCode, --单据类型代码
                BillTypeName, --单据类型名称
                BillTypeID,   --提报时间点
                CardID,       --品牌
                Adidas        -- 门店编号 盘盈盘亏的仓库代码填写此处
            )
            SELECT @SolutionID SolutionID,   --方案ID
                   a.CompanyID,              --公司ID
                   b.BillNo,                 --单号
                    '' ShopCode,      --店铺代码
               '' ShopName,      --店铺名称
               dd.StockCode OutStockCode, --仓库代码
               dd.StockName OutStockName, --仓库名称
               dd.StockType OutStocType,  --出库仓类型/发货仓类型
               k.StockCode InStockCode,  --仓库代码
               k.StockName InStockName,  --仓库名称
               k.StockType InStocType,   --入库仓类型/入库仓类型
                   m.InCheckDate BillDate,  --销售日期
                   GETDATE() BillTime,       --导入时间
                   c.MaterialCode,           --货号
                   ms.BarCode,               --条码
                   f.SizeCode,               --尺码
                   f.SizeName,               --尺码名称
                   a.InQty Qty,              --数量
                   GETDATE() ModifyTime,     --更新时间
                   '302' BillTypeCode,
                   '调拨入库单' BillTypeName,     --单据类型
                   'Inv_MovIn' BillTypeID,   --单据类型
                   @CardID CardID,           --品牌
                   NULL Adidas
            FROM dbo.SD_Inv_MoveSize a
                INNER JOIN dbo.SD_Inv_MoveDetail b
                    ON b.CompanyID = a.CompanyID
                       AND b.BillNo = a.BillNo
                       AND b.Sequence = a.Sequence
                INNER JOIN dbo.SD_Inv_MoveMaster m
                    ON a.CompanyID = m.CompanyID
                       AND a.BillNo = m.BillNo
                INNER JOIN dbo.SD_Mat_Material c
                    ON a.MaterialID = c.MaterialID
                INNER JOIN dbo.SD_Mat_Size f
                    ON a.SizeID = f.SizeID
                LEFT JOIN dbo.Bas_Company j
                    ON a.CompanyID = j.CompanyID
                LEFT JOIN dbo.SD_Mat_Card i
                    ON c.CardID = i.CardID
                LEFT JOIN dbo.Bas_Stock dd
                ON dd.CompanyID = m.CompanyID
                   AND dd.StockID = m.OutStockID
            LEFT JOIN dbo.Bas_Stock k
                ON k.CompanyID = m.CompanyID
                   AND k.StockID = m.InStockID
                JOIN dbo.SD_Mat_MaterialSize ms
                    ON ms.SizeID = a.SizeID
                       AND ms.MaterialID = a.MaterialID
            WHERE m.CompanyID = @CompanyID
                 AND c.CardID = @CardID
				   and  exists (select top 1 1 from #ShopStock d where d.StockID in( m.OutStockID,m.InStockID)AND a.CompanyID = d.CompanyID)
                  AND m.InCheckDate > @Start
                  AND m.InCheckDate <= @End
                  AND m.InBillStatus = 4;
        END;
        BEGIN --POS单
            INSERT INTO #OutIn
            (
                SolutionID,   --方案ID
                CompanyID,    --公司
                BillNo,       --单号
                ShopCode,     --店铺代码
                ShopName,     --店铺名称
                OutStockCode, --仓库代码
                OutStockName,
                OutStocType,  --出库仓类型/发货仓类型
                InStockCode,  --仓库代码
                InStockName,
                InStocType,   --入库仓类型/入库仓类型
                BillDate,     --单据生效日期
                BillTime,     --制单时间
                MaterialCode, --货号
                BarCode,      --条码
                SizeCode,     --尺码
                SizeName,     --尺码名称
                Qty,          --数量
                ModifyTime,   --修改时间
                BillTypeCode, --单据类型代码
                BillTypeName, --单据类型名称
                BillTypeID,   --提报时间点
                CardID,       --品牌
                Adidas        -- 门店编号 盘盈盘亏的仓库代码填写此处
            )
            SELECT @SolutionID SolutionID,   --方案ID
                   a.CompanyID,              --公司ID
                   a.BillNo,                 --单号
                   d.ShopCode ShopCode,      --店铺代码
                   d.ShopName ShopName,      --店铺名称
                   d.StockCode OutStockCode, --仓库代码
                   d.StockName OutStockName, --仓库名称
                   NULL OutStocType,         --出库仓类型/发货仓类型
                   d.StockCode InStockCode,  --仓库代码
                   d.StockName InStockName,  --仓库名称
                   NULL InStocType,          --入库仓类型/入库仓类型
                   m.BillDate BillDate,      --销售日期
                   GETDATE() BillTime,       --导入时间
                   c.MaterialCode,           --货号
                   a.BarCode,                --条码
                   f.SizeCode,               --尺码
                   f.SizeName,               --尺码名称
                   a.Qty Qty,                --数量
                   GETDATE() ModifyTime,     --更新时间
                   (CASE
                        WHEN a.Qty > 0 THEN
                            '261'
                        ELSE
                            '262'
                    END
                   ) BillTypeCode,
                   'POS单' BillTypeName,      --单据类型
                   'POS' BillTypeID,         --单据类型
                   @CardID CardID,           --品牌
                   NULL Adidas
            FROM dbo.SD_POS_SaleDetail a
                INNER JOIN dbo.SD_POS_SaleMaster m
                    ON a.CompanyID = m.CompanyID
                       AND a.BillNo = m.BillNo
                INNER JOIN dbo.SD_Mat_Material c
                    ON a.MaterialID = c.MaterialID
                INNER JOIN #ShopStock d
                    ON d.StockID = m.StockID
                      AND d.CompanyID = m.CompanyID --and d.IsShop=1
                --INNER JOIN dbo.SD_Bas_ShopCard e
                --    ON d.CompanyID = e.CompanyID
                --       AND d.ShopID = e.ShopID
                --       AND c.CardID = e.CardID
                INNER JOIN dbo.SD_Mat_Size f
                    ON a.SizeID = f.SizeID
                LEFT JOIN dbo.Bas_Company j
                    ON a.CompanyID = j.CompanyID
                LEFT JOIN dbo.SD_Mat_Card i
                    ON c.CardID = i.CardID
            WHERE m.CompanyID = @CompanyID
                  AND c.CardID = @CardID
                  AND m.BillTime > @Start
                  AND m.BillTime <= @End
                  AND m.BillStatus = 4
            UNION ALL
            SELECT @SolutionID SolutionID,   --方案ID
                   a.CompanyID,              --公司ID
                   a.BillNo,                 --单号
                   d.ShopCode ShopCode,      --店铺代码
                   d.ShopName ShopName,      --店铺名称
                   d.StockCode OutStockCode, --仓库代码
                   d.StockName OutStockName, --仓库名称
                   NULL OutStocType,         --出库仓类型/发货仓类型
                   d.StockCode InStockCode,  --仓库代码
                   d.StockName InStockName,  --仓库名称
                   NULL InStocType,          --入库仓类型/入库仓类型
                   m.BillDate BillDate,      --销售日期
                   GETDATE() BillTime,       --导入时间
                   c.MaterialCode,           --货号
                   a.BarCode,                --条码
                   f.SizeCode,               --尺码
                   f.SizeName,               --尺码名称
                   a.Qty Qty,                --数量
                   GETDATE() ModifyTime,     --更新时间
                   (CASE
                        WHEN a.Qty > 0 THEN
                            '261'
                        ELSE
                            '262'
                    END
                   ) BillTypeCode,
                   'POS单' BillTypeName,      --单据类型
                   'POS' BillTypeID,         --单据类型
                   @CardID CardID,           --品牌
                   NULL Adidas
            FROM dbo.SD_POS_SaleDetail2 a
                INNER JOIN dbo.SD_POS_SaleMaster2 m
                    ON a.CompanyID = m.CompanyID
                       AND a.BillNo = m.BillNo
                INNER JOIN dbo.SD_Mat_Material c
                    ON a.MaterialID = c.MaterialID
                INNER JOIN #ShopStock d
                    ON d.StockID = m.StockID
                       AND d.CompanyID = m.CompanyID --and d.IsShop=1
                --INNER JOIN dbo.SD_Bas_ShopCard e
                --    ON d.CompanyID = e.CompanyID
                --       AND d.ShopID = e.ShopID
                --       AND c.CardID = e.CardID
                INNER JOIN dbo.SD_Mat_Size f
                    ON a.SizeID = f.SizeID
                LEFT JOIN dbo.Bas_Company j
                    ON a.CompanyID = j.CompanyID
                LEFT JOIN dbo.SD_Mat_Card i
                    ON c.CardID = i.CardID
            WHERE m.CompanyID = @CompanyID
                  AND c.CardID = @CardID
                  AND m.BillTime > @Start
                  AND m.BillTime <= @End
                  AND m.BillStatus = 4;
        END;



        BEGIN --补偿取 上一次遗漏的单据
            --上一次提报时间点
            SELECT @DrewTime = FORMAT(MAX(ModifyTime), 'yyyyMMddHH')
            FROM HK_ERP_SG_DataGather.dbo.Gat_Ad_OutIn_OCTB;

            IF OBJECT_ID('tempdb..#OutInBill', 'U') IS NOT NULL
                DROP TABLE #OutInBill;
            CREATE TABLE #OutInBill
            (
                CompanyID VARCHAR(20) NULL,  --公司
                BillNo VARCHAR(40) NOT NULL, --单号
                BillTypeID VARCHAR(40) NULL,
            );
            INSERT INTO #OutInBill
            (
                CompanyID,
                BillNo,
                BillTypeID
            )
            SELECT CompanyID,
                   BillNo,
                   BillTypeID
            FROM HK_ERP_SG_DataGather.dbo.Gat_Ad_OutIn_OCTB a
            WHERE a.SolutionID = @SolutionID
                  AND a.DrewTime = @DrewTime;

            DECLARE @LastStart DATETIME; --本次捞取数据的开始时间
            DECLARE @LastEnd DATETIME; --本次捞取数据的结束时间

            SELECT @LastStart = DATEADD(HOUR, -3, @Start);
            SELECT @LastEnd = DATEADD(HOUR, -3, @End);

            BEGIN --采购入库单
                INSERT INTO #OutIn
                (
                    SolutionID,   --方案ID
                    CompanyID,    --公司
                    BillNo,       --单号
                    ShopCode,     --店铺代码
                    ShopName,     --店铺名称
                    OutStockCode, --仓库代码
                    OutStockName,
                    OutStocType,  --出库仓类型/发货仓类型
                    InStockCode,  --仓库代码
                    InStockName,
                    InStocType,   --入库仓类型/入库仓类型
                    BillDate,     --单据生效日期
                    BillTime,     --制单时间
                    MaterialCode, --货号
                    BarCode,      --条码
                    SizeCode,     --尺码
                    SizeName,     --尺码名称
                    Qty,          --数量
                    ModifyTime,   --修改时间
                    BillTypeCode, --单据类型代码
                    BillTypeName, --单据类型名称
                    BillTypeID,   --提报时间点
                    CardID,       --品牌
                    Adidas        -- 门店编号 盘盈盘亏的仓库代码填写此处

                )
                SELECT @SolutionID SolutionID,       --方案ID
                       a.CompanyID,                  --公司ID
                       b.BillNo,                     --单号
                       d.ShopCode ShopCode,          --店铺代码
                       d.ShopName ShopName,          --店铺名称
                       'ADDC' OutStockCode,          --仓库代码
                       NULL OutStockName,            --仓库名称
                       NULL OutStocType,             --出库仓类型/发货仓类型
                       d.StockCode InStockCode,      --仓库代码
                       d.StockName InStockName,      --仓库名称
                       NULL InStocType,              --入库仓类型/入库仓类型
                       m.CheckDate BillDate,         --销售日期
                       GETDATE() BillTime,           --导入时间
                       c.MaterialCode,               --货号
                       ms.BarCode,                   --条码
                       f.SizeCode,                   --尺码
                       f.SizeName,                   --尺码名称
                       a.Qty,                        --数量
                       GETDATE() ModifyTime,         --更新时间
                       (CASE
                            WHEN m.BillTypeID = 'Inv_Cgrk' THEN
                                '101'
                            WHEN m.BillTypeID = 'Inv_Thck' THEN
                                '102'
                            WHEN m.BillTypeID = 'Inv_Pyrk' THEN
                                '701'
                            WHEN m.BillTypeID = 'Inv_Pkck' THEN
                                '702'
                            ELSE
                                ''
                        END
                       ) BillTypeCode,               --单据类型代码
                       bt.BillTypeName BillTypeName, --单据类型
                       m.BillTypeID,                 --单据类型
                       @CardID CardID,               --品牌
                       NULL Adidas
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
                          AND a.CompanyID = d.CompanyID --and d.IsShop=1
                    --INNER JOIN dbo.SD_Bas_ShopCard e
                    --    ON d.CompanyID = e.CompanyID
                    --       AND d.ShopID = e.ShopID
                    --       AND c.CardID = e.CardID
                    INNER JOIN dbo.SD_Mat_Size f
                        ON a.SizeID = f.SizeID
                    LEFT JOIN dbo.Bas_Company j
                        ON a.CompanyID = j.CompanyID
                    LEFT JOIN dbo.SD_Mat_Card i
                        ON c.CardID = i.CardID
                    LEFT JOIN dbo.Sys_BillType bt
                        ON bt.BillTypeID = m.BillTypeID
                    JOIN dbo.SD_Mat_MaterialSize ms
                        ON ms.SizeID = a.SizeID
                           AND ms.MaterialID = a.MaterialID
                WHERE m.CompanyID = @CompanyID
                      AND c.CardID = @CardID
                      AND m.CheckDate > @LastStart
                      AND m.CheckDate <= @LastEnd
                      AND m.BillTypeID IN ( 'Inv_Cgrk', 'Inv_Thck', 'Inv_Pyrk', 'Inv_Pkck' )
					  AND m.BillStatus=4
                      AND NOT EXISTS
                (
                    SELECT TOP 1
                           1
                    FROM #OutInBill p
                    WHERE p.CompanyID = a.CompanyID
                          AND p.BillNo = a.BillNo
                )
                      AND NOT EXISTS
                (
                    SELECT TOP 1
                           1
                    FROM #OutIn p
                    WHERE p.CompanyID = a.CompanyID
                          AND p.BillNo = a.BillNo
                );
            END;
            BEGIN --调拨出库单
                INSERT INTO #OutIn
                (
                    SolutionID,   --方案ID
                    CompanyID,    --公司
                    BillNo,       --单号
                    ShopCode,     --店铺代码
                    ShopName,     --店铺名称
                    OutStockCode, --仓库代码
                    OutStockName,
                    OutStocType,  --出库仓类型/发货仓类型
                    InStockCode,  --仓库代码
                    InStockName,
                    InStocType,   --入库仓类型/入库仓类型
                    BillDate,     --单据生效日期
                    BillTime,     --制单时间
                    MaterialCode, --货号
                    BarCode,      --条码
                    SizeCode,     --尺码
                    SizeName,     --尺码名称
                    Qty,          --数量
                    ModifyTime,   --修改时间
                    BillTypeCode, --单据类型代码
                    BillTypeName, --单据类型名称
                    BillTypeID,   --提报时间点
                    CardID,       --品牌
                    Adidas        -- 门店编号 盘盈盘亏的仓库代码填写此处
                )
                SELECT @SolutionID SolutionID,   --方案ID
                       a.CompanyID,              --公司ID
                       b.BillNo,                 --单号
                       '' ShopCode,      --店铺代码
               '' ShopName,      --店铺名称
               dd.StockCode OutStockCode, --仓库代码
               dd.StockName OutStockName, --仓库名称
               dd.StockType OutStocType,  --出库仓类型/发货仓类型
               k.StockCode InStockCode,  --仓库代码
               k.StockName InStockName,  --仓库名称
               k.StockType InStocType,   --入库仓类型/入库仓类型
                       m.OutCheckDate BillDate,  --销售日期
                       GETDATE() BillTime,       --导入时间
                       c.MaterialCode,           --货号
                       ms.BarCode,               --条码
                       f.SizeCode,               --尺码
                       f.SizeName,               --尺码名称
                       a.OutQty Qty,             --数量
                       GETDATE() ModifyTime,     --更新时间
                       '301' BillTypeCode,
                       '调拨出库单' BillTypeName,     --单据类型
                       'Inv_MovOut' BillTypeID,  --单据类型
                       @CardID CardID,           --品牌
                       NULL Adidas
                FROM dbo.SD_Inv_MoveSize a
                    INNER JOIN dbo.SD_Inv_MoveDetail b
                        ON b.CompanyID = a.CompanyID
                           AND b.BillNo = a.BillNo
                           AND b.Sequence = a.Sequence
                    INNER JOIN dbo.SD_Inv_MoveMaster m
                        ON a.CompanyID = m.CompanyID
                           AND a.BillNo = m.BillNo
                    INNER JOIN dbo.SD_Mat_Material c
                        ON a.MaterialID = c.MaterialID
                    INNER JOIN dbo.SD_Mat_Size f
                        ON a.SizeID = f.SizeID
                    LEFT JOIN dbo.Bas_Company j
                        ON a.CompanyID = j.CompanyID
                    LEFT JOIN dbo.SD_Mat_Card i
                        ON c.CardID = i.CardID
                    LEFT JOIN dbo.Bas_Stock dd
                ON dd.CompanyID = m.CompanyID
                   AND dd.StockID = m.OutStockID
            LEFT JOIN dbo.Bas_Stock k
                ON k.CompanyID = m.CompanyID
                   AND k.StockID = m.InStockID
                    JOIN dbo.SD_Mat_MaterialSize ms
                        ON ms.SizeID = a.SizeID
                           AND ms.MaterialID = a.MaterialID
                WHERE m.CompanyID = @CompanyID
                      AND c.CardID = @CardID
				   and  exists (select top 1 1 from #ShopStock d where d.StockID in( m.OutStockID,m.InStockID)AND a.CompanyID = d.CompanyID)
                      AND m.OutCheckDate > @LastStart
                      AND m.OutCheckDate <= @LastEnd
                      AND m.OutBillStatus = 4
                      AND NOT EXISTS
                (
                    SELECT TOP 1
                           1
                    FROM #OutInBill p
                    WHERE p.CompanyID = a.CompanyID
                          AND p.BillNo = a.BillNo
                          AND p.BillTypeID = 'Inv_MovOut'
                )
                      AND NOT EXISTS
                (
                    SELECT TOP 1
                           1
                    FROM #OutIn p
                    WHERE p.CompanyID = a.CompanyID
                          AND p.BillNo = a.BillNo
                          AND p.BillTypeID = 'Inv_MovOut'
                );
            END;
            BEGIN --调拨入库单
                INSERT INTO #OutIn
                (
                    SolutionID,   --方案ID
                    CompanyID,    --公司
                    BillNo,       --单号
                    ShopCode,     --店铺代码
                    ShopName,     --店铺名称
                    OutStockCode, --仓库代码
                    OutStockName,
                    OutStocType,  --出库仓类型/发货仓类型
                    InStockCode,  --仓库代码
                    InStockName,
                    InStocType,   --入库仓类型/入库仓类型
                    BillDate,     --单据生效日期
                    BillTime,     --制单时间
                    MaterialCode, --货号
                    BarCode,      --条码
                    SizeCode,     --尺码
                    SizeName,     --尺码名称
                    Qty,          --数量
                    ModifyTime,   --修改时间
                    BillTypeCode, --单据类型代码
                    BillTypeName, --单据类型名称
                    BillTypeID,   --提报时间点
                    CardID,       --品牌
                    Adidas        -- 门店编号 盘盈盘亏的仓库代码填写此处

                )
                SELECT @SolutionID SolutionID,   --方案ID
                       a.CompanyID,              --公司ID
                       b.BillNo,                 --单号
                        '' ShopCode,      --店铺代码
               '' ShopName,      --店铺名称
               dd.StockCode OutStockCode, --仓库代码
               dd.StockName OutStockName, --仓库名称
               dd.StockType OutStocType,  --出库仓类型/发货仓类型
               k.StockCode InStockCode,  --仓库代码
               k.StockName InStockName,  --仓库名称
               k.StockType InStocType,   --入库仓类型/入库仓类型
                       m.InCheckDate BillDate,  --销售日期
                       GETDATE() BillTime,       --导入时间
                       c.MaterialCode,           --货号
                       ms.BarCode,               --条码
                       f.SizeCode,               --尺码
                       f.SizeName,               --尺码名称
                       a.InQty Qty,              --数量
                       GETDATE() ModifyTime,
                       '301' BillTypeCode,
                       '调拨入库单' BillTypeName,     --单据类型
                       'Inv_MovIn' BillTypeID,   --单据类型
                       @CardID CardID,           --品牌
                       NULL Adidas
                FROM dbo.SD_Inv_MoveSize a
                    INNER JOIN dbo.SD_Inv_MoveDetail b
                        ON b.CompanyID = a.CompanyID
                           AND b.BillNo = a.BillNo
                           AND b.Sequence = a.Sequence
                    INNER JOIN dbo.SD_Inv_MoveMaster m
                        ON a.CompanyID = m.CompanyID
                           AND a.BillNo = m.BillNo
                    INNER JOIN dbo.SD_Mat_Material c
                        ON a.MaterialID = c.MaterialID
                    INNER JOIN dbo.SD_Mat_Size f
                        ON a.SizeID = f.SizeID
                    LEFT JOIN dbo.Bas_Company j
                        ON a.CompanyID = j.CompanyID
                    LEFT JOIN dbo.SD_Mat_Card i
                        ON c.CardID = i.CardID
                   LEFT JOIN dbo.Bas_Stock dd
                ON dd.CompanyID = m.CompanyID
                   AND dd.StockID = m.OutStockID
            LEFT JOIN dbo.Bas_Stock k
                ON k.CompanyID = m.CompanyID
                   AND k.StockID = m.InStockID
                    JOIN dbo.SD_Mat_MaterialSize ms
                        ON ms.SizeID = a.SizeID
                           AND ms.MaterialID = a.MaterialID
                WHERE m.CompanyID = @CompanyID
                     AND c.CardID = @CardID
				   and  exists (select top 1 1 from #ShopStock d where d.StockID in( m.OutStockID,m.InStockID)AND a.CompanyID = d.CompanyID)
                      AND m.InCheckDate > @LastStart
                      AND m.InCheckDate <= @LastEnd
                      AND m.InBillStatus = 4
                      AND NOT EXISTS
                (
                    SELECT TOP 1
                           1
                    FROM #OutInBill p
                    WHERE p.CompanyID = a.CompanyID
                          AND p.BillNo = a.BillNo
                          AND p.BillTypeID = 'Inv_MovIn'
                )
                      AND NOT EXISTS
                (
                    SELECT TOP 1
                           1
                    FROM #OutIn p
                    WHERE p.CompanyID = a.CompanyID
                          AND p.BillNo = a.BillNo
                          AND p.BillTypeID = 'Inv_MovIn'
                );
            END;
            BEGIN --POS单
                INSERT INTO #OutIn
                (
                    SolutionID,   --方案ID
                    CompanyID,    --公司
                    BillNo,       --单号
                    ShopCode,     --店铺代码
                    ShopName,     --店铺名称
                    OutStockCode, --仓库代码
                    OutStockName,
                    OutStocType,  --出库仓类型/发货仓类型
                    InStockCode,  --仓库代码
                    InStockName,
                    InStocType,   --入库仓类型/入库仓类型
                    BillDate,     --单据生效日期
                    BillTime,     --制单时间
                    MaterialCode, --货号
                    BarCode,      --条码
                    SizeCode,     --尺码
                    SizeName,     --尺码名称
                    Qty,          --数量
                    ModifyTime,   --修改时间
                    BillTypeCode, --单据类型代码
                    BillTypeName, --单据类型名称
                    BillTypeID,   --提报时间点
                    CardID,       --品牌
                    Adidas        -- 门店编号 盘盈盘亏的仓库代码填写此处
                )
                SELECT @SolutionID SolutionID,   --方案ID
                       a.CompanyID,              --公司ID
                       a.BillNo,                 --单号
                       d.ShopCode ShopCode,      --店铺代码
                       d.ShopName ShopName,      --店铺名称
                       d.StockCode OutStockCode, --仓库代码
                       d.StockName OutStockName, --仓库名称
                       NULL OutStocType,         --出库仓类型/发货仓类型
                       d.StockCode InStockCode,  --仓库代码
                       d.StockName InStockName,  --仓库名称
                       NULL InStocType,          --入库仓类型/入库仓类型
                       m.BillDate BillDate,      --销售日期
                       GETDATE() BillTime,       --导入时间
                       c.MaterialCode,           --货号
                       a.BarCode,                --条码
                       f.SizeCode,               --尺码
                       f.SizeName,               --尺码名称
                       a.Qty Qty,                --数量
                       GETDATE() ModifyTime,     --更新时间
                       (CASE
                            WHEN a.Qty > 0 THEN
                                '261'
                            ELSE
                                '262'
                        END
                       ) BillTypeCode,
                       'POS单' BillTypeName,      --单据类型
                       'POS' BillTypeID,         --单据类型
                       @CardID CardID,           --品牌
                       NULL Adidas
                FROM dbo.SD_POS_SaleDetail a
                    INNER JOIN dbo.SD_POS_SaleMaster m
                        ON a.CompanyID = m.CompanyID
                           AND a.BillNo = m.BillNo
                    INNER JOIN dbo.SD_Mat_Material c
                        ON a.MaterialID = c.MaterialID
                    INNER JOIN #ShopStock d
                        ON d.StockID = m.StockID
                           AND d.CompanyID = m.CompanyID --and d.IsShop=1
                    --INNER JOIN dbo.SD_Bas_ShopCard e
                    --    ON d.CompanyID = e.CompanyID
                    --       AND d.ShopID = e.ShopID
                    --       AND c.CardID = e.CardID
                    INNER JOIN dbo.SD_Mat_Size f
                        ON a.SizeID = f.SizeID
                    LEFT JOIN dbo.Bas_Company j
                        ON a.CompanyID = j.CompanyID
                    LEFT JOIN dbo.SD_Mat_Card i
                        ON c.CardID = i.CardID
                WHERE m.CompanyID = @CompanyID
                      AND c.CardID = @CardID
                      AND m.BillTime > @LastStart
                      AND m.BillTime <= @LastEnd
                      AND m.BillStatus = 4
                      AND NOT EXISTS
                (
                    SELECT TOP 1
                           1
                    FROM #OutInBill p
                    WHERE p.CompanyID = a.CompanyID
                          AND p.BillNo = a.BillNo
                          AND p.BillTypeID = 'POS'
                )
                      AND NOT EXISTS
                (
                    SELECT TOP 1
                           1
                    FROM #OutIn p
                    WHERE p.CompanyID = a.CompanyID
                          AND p.BillNo = a.BillNo
                          AND p.BillTypeID = 'POS'
                )
                UNION ALL
                SELECT @SolutionID SolutionID,   --方案ID
                       a.CompanyID,              --公司ID
                       a.BillNo,                 --单号
                       d.ShopCode ShopCode,      --店铺代码
                       d.ShopName ShopName,      --店铺名称
                       d.StockCode OutStockCode, --仓库代码
                       d.StockName OutStockName, --仓库名称
                       NULL OutStocType,         --出库仓类型/发货仓类型
                       d.StockCode InStockCode,  --仓库代码
                       d.StockName InStockName,  --仓库名称
                       NULL InStocType,          --入库仓类型/入库仓类型
                       m.BillDate BillDate,      --销售日期
                       GETDATE() BillTime,       --导入时间
                       c.MaterialCode,           --货号
                       a.BarCode,                --条码
                       f.SizeCode,               --尺码
                       f.SizeName,               --尺码名称
                       a.Qty Qty,                --数量
                       GETDATE() ModifyTime,     --更新时间
                       (CASE
                            WHEN a.Qty > 0 THEN
                                '261'
                            ELSE
                                '262'
                        END
                       ) BillTypeCode,
                       'POS单' BillTypeName,      --单据类型
                       'POS' BillTypeID,         --单据类型
                       @CardID CardID,           --品牌
                       NULL Adidas
                FROM dbo.SD_POS_SaleDetail2 a
                    INNER JOIN dbo.SD_POS_SaleMaster2 m
                        ON a.CompanyID = m.CompanyID
                           AND a.BillNo = m.BillNo
                    INNER JOIN dbo.SD_Mat_Material c
                        ON a.MaterialID = c.MaterialID
                    INNER JOIN #ShopStock d
                        ON d.StockID = m.StockID
                           AND d.CompanyID = m.CompanyID --and d.IsShop=1
                    --INNER JOIN dbo.SD_Bas_ShopCard e
                    --    ON d.CompanyID = e.CompanyID
                    --       AND d.ShopID = e.ShopID
                    --       AND c.CardID = e.CardID
                    INNER JOIN dbo.SD_Mat_Size f
                        ON a.SizeID = f.SizeID
                    LEFT JOIN dbo.Bas_Company j
                        ON a.CompanyID = j.CompanyID
                    LEFT JOIN dbo.SD_Mat_Card i
                        ON c.CardID = i.CardID
                WHERE m.CompanyID = @CompanyID
                      AND c.CardID = @CardID
                      AND m.BillTime > @LastStart
                      AND m.BillTime <= @LastEnd
                      AND m.BillStatus = 4
                      AND NOT EXISTS
                (
                    SELECT TOP 1
                           1
                    FROM #OutInBill p
                    WHERE p.CompanyID = a.CompanyID
                          AND p.BillNo = a.BillNo
                          AND p.BillTypeID = 'POS'
                )
                      AND NOT EXISTS
                (
                    SELECT TOP 1
                           1
                    FROM #OutIn p
                    WHERE p.CompanyID = a.CompanyID
                          AND p.BillNo = a.BillNo
                          AND p.BillTypeID = 'POS'
                );
            END;
        END;

        SELECT @DrewTime = FORMAT(GETDATE(), 'yyyyMMddHH');
        --删除指定范围之内的数据，防止补采产生重复数据
        --DELETE FROM HK_ERP_SG_DataGather.dbo.Gat_Ad_OutIn_OCTB
        --WHERE CompanyID = @CompanyID
        --      AND CardID = @CardID
        --      AND BillTime > @Start
        --      AND BillTime <= @End;

		 --采购入库 出库方ADDC  入库方pos
  UPDATE a
    SET a.OutStocType = 'ADDC',
        a.InStocType = 'POS'
    FROM #OutIn a
    WHERE a.BillTypeID='Inv_Cgrk'

--采购退货  出库方  pos        入库方  ADDC 
  UPDATE a
    SET a.OutStocType = 'POS',
        a.InStocType = 'ADDC'
    FROM #OutIn a
    WHERE a.BillTypeID='Inv_Thck'

	--盘盈入库单  出库方 空  入库方  POS
  UPDATE a
    SET a.OutStocType = '',
        a.InStocType = 'POS'
    FROM #OutIn a
    WHERE a.BillTypeID='Inv_Pyrk'

		--盘亏出库单 出库方POS  入库方  空
  UPDATE a
    SET a.OutStocType = 'POS',
        a.InStocType = ''
    FROM #OutIn a
    WHERE a.BillTypeID='Inv_Pkck'


    --调拨单仓库类型处理  0  仓  1 店
	begin
    --举例：仓发店：发货仓类型DC入库仓类型是POS；
    UPDATE a
    SET a.OutStocType = 'DC',
        a.InStocType = 'POS'
    FROM #OutIn a
    WHERE a.BillTypeID IN ( 'Inv_MovOut', 'Inv_MovIn' )
          AND a.OutStocType = '0'
          AND a.InStocType = '1';
    --店发仓：发货仓类型POS入库仓类型是DC；
    UPDATE a
    SET a.OutStocType = 'POS',
        a.InStocType = 'DC'
    FROM #OutIn a
    WHERE a.BillTypeID IN ( 'Inv_MovOut', 'Inv_MovIn' )
          AND a.OutStocType = '1'
          AND a.InStocType = '0';
    --店间调拨：发货放与入库方均是POS；
    UPDATE a
    SET a.OutStocType = 'POS',
        a.InStocType = 'POS'
    FROM #OutIn a
    WHERE a.BillTypeID IN ( 'Inv_MovOut', 'Inv_MovIn' )
          AND a.OutStocType = '1'
          AND a.InStocType = '1';

     --仓到仓调拨 ：发货放与入库方均是POS；
    UPDATE a
    SET a.OutStocType = 'DC',
        a.InStocType = 'DC'
    FROM #OutIn a
    WHERE a.BillTypeID IN ( 'Inv_MovOut', 'Inv_MovIn' )
          AND a.OutStocType = '0'
          AND a.InStocType = '0';

    --仓到仓调拨不提取
	/*
	DELETE a
    FROM #OutIn a
    WHERE a.BillTypeID IN ( 'Inv_MovOut', 'Inv_MovIn' )
          AND a.OutStocType = '0'
          AND a.InStocType = '0'
	end
	*/

	/*
pos销售  出库方  pos   入库方 空
pos退货  出库方  空       入库方  pos 
*/
	 --POS单仓库类型处理
	begin
	UPDATE a
    SET a.OutStocType = 'POS',
        a.InStocType = ''
    FROM #OutIn a
    WHERE a.BillTypeID='POS'

	UPDATE a
    SET a.OutStocType = '',
        a.InStocType = 'POS'
    FROM #OutIn a
    WHERE a.BillTypeID='POS'
	 and a.Qty<0 
	end
	   
	   	DELETE FROM HK_ERP_SG_DataGather.dbo.Gat_Ad_OutIn_OCTB
        WHERE  ModifyTime <= DATEADD(DAY,-2, CONVERT(VARCHAR(10), GETDATE(), 120));

        INSERT INTO HK_ERP_SG_DataGather.dbo.Gat_Ad_OutIn_OCTB
        (
            SolutionID,   --方案ID
            CompanyID,    --公司
            BillNo,       --单号
            ShopCode,     --店铺代码
            ShopName,     --店铺名称
            OutStockCode, --仓库代码
            OutStockName,
            OutStocType,  --出库仓类型/发货仓类型
            InStockCode,  --仓库代码
            InStockName,
            InStocType,   --入库仓类型/入库仓类型
            BillDate,     --单据生效日期
            BillTime,     --制单时间
            MaterialCode, --货号
            BarCode,      --条码
            SizeCode,     --尺码
            SizeName,     --尺码名称
            Qty,          --数量
            ModifyTime,   --修改时间
            BillTypeCode, --单据类型代码
            BillTypeName, --单据类型名称
            BillTypeID,   --提报时间点
            CardID,       --品牌
            Adidas,       -- 门店编号 盘盈盘亏的仓库代码填写此处
            DrewTime
        )
        SELECT distinct SolutionID,   --方案ID
               CompanyID,    --公司
               BillNo,       --单号
               ShopCode,     --店铺代码
               ShopName,     --店铺名称
               OutStockCode, --仓库代码
               OutStockName,
               OutStocType,  --出库仓类型/发货仓类型
               InStockCode,  --仓库代码
               InStockName,
               InStocType,   --入库仓类型/入库仓类型
               BillDate,     --单据生效日期
               BillTime,     --制单时间
               MaterialCode, --货号
               BarCode,      --条码
               SizeCode,     --尺码
               SizeName,     --尺码名称
               Qty,          --数量
               ModifyTime,   --修改时间
               BillTypeCode, --单据类型代码
               BillTypeName, --单据类型名称
               BillTypeID,   --提报时间点
               CardID,       --品牌
               Adidas,       -- 门店编号 盘盈盘亏的仓库代码填写此处
               @DrewTime DrewTime
        FROM #OutIn;
    END;
END;

end
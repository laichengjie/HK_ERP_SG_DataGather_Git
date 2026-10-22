IF EXISTS (SELECT *
           FROM sys.objects
           WHERE object_id=OBJECT_ID(N'[dbo].[sp_SGIS_SkechersPosData]')AND type IN (N'P', N'PC'))
    DROP PROCEDURE [dbo].sp_SGIS_SkechersPosData;
GO

--斯凯奇品牌提报
CREATE PROCEDURE sp_SGIS_SkechersPosData @SolutionId INT, --提报方案Id
    @StartTime DATETIME, --开始时间
    @EndTime DATETIME --结束时间
AS BEGIN
    DECLARE @VendorID INT, @CompanyID VARCHAR(20) ='', --公司ID 
        @CardID VARCHAR(20); -- 品牌ID 

    SELECT @VendorID=VendorID, @CompanyID=CompanyID, @CardID=CardID
    FROM HK_ERP_SG_DataGather.dbo.Gat_Solution
    WHERE ID=@SolutionId;

    --获取提报方案店铺及品牌
    CREATE TABLE #Shop (ShopID VARCHAR(50),tradeShopCode VARCHAR(50)); --提报店铺

    CREATE TABLE #VendShop ( -- 供应商店铺
    VendShopID INT NOT NULL, --
    ShopCode VARCHAR(50) NULL,
    ShopName VARCHAR(100) NULL,
    StockCode VARCHAR(50) NULL,
    StockName VARCHAR(100) NULL, );

    CREATE TABLE #ShopMap ( -- 店铺关系1
    VendShopID INT NOT NULL,
    ShopID VARCHAR(20) NOT NULL,
    CompanyID VARCHAR(20));

    -- 供应商店铺
    INSERT INTO #VendShop
    SELECT ID, ShopCode, ShopName, StockCode, StockName
    FROM HK_ERP_SG_DataGather.dbo.Bas_VendShop
    WHERE VendorID=@VendorID;

    -- 店铺映射关系
    INSERT INTO #ShopMap
    SELECT VendShopID, ShopID, CompanyID
    FROM HK_ERP_SG_DataGather.dbo.Bas_ShopMap a
         INNER JOIN HK_ERP_SG_DataGather.dbo.Bas_VendShop b ON a.VendShopID=b.ID
    WHERE b.VendorID=@VendorID;

    DELETE FROM #Shop; --清空上一次循环的数据（取店铺）

    INSERT INTO #Shop(ShopID,tradeShopCode)
    SELECT DISTINCT ShopID,TradeCode
    FROM HK_ERP_SG_DataGather.dbo.Gat_ShopRight
    WHERE AllowUsed=1 AND CompanyID=@CompanyID AND CardID=@CardID AND ISNULL(VendorID, -2)=@VendorID;

    CREATE TABLE #SkechersMasterData (refno VARCHAR(50), billDate DATETIME, storeCode VARCHAR(50),ShopId VARCHAR(50));

    --主表参数：
    --编号	字段名	说明	类型/长度	约束说明	备注
    --1	refno	POS零售单号	String/80	不可为空	
    --2	billDate	单据日期	Integer/8	不可为空	如20170401
    --3	storeCode	店仓编号	String/100	不可为空	SKX店仓编号
    --4	origPosno	原单号	string	可为空	退换货时
    --5	sumQty	总数量	Integer	不可为空	单据总数量
    --6	sumTotAmtActual	总成交金额	Float	不可为空	单据总成交金额
    --7	salesrepCode	营业员编号	String	可为空	
    --8	description	备注	String/500	可为空	
    --9	detail	明细数据	Object[]	不可为空	如: “detail”:[obj1,obj2…]
    INSERT INTO #SkechersMasterData(refno, billDate, storeCode, ShopId)
    SELECT BillNo refno, a.BillDate billDate, ISNULL(e.ShopCode, c.tradeShopCode) storeCode,c.ShopID
    FROM dbo.SD_POS_SaleMaster a
         INNER JOIN #Shop c ON a.ShopID=c.ShopID
         LEFT JOIN #ShopMap d ON a.CompanyID=d.CompanyID AND a.ShopID=d.ShopID -- 关联到店铺映射
         LEFT JOIN #VendShop e ON d.VendShopID=e.VendShopID
    WHERE a.CompanyID=@CompanyID AND a.BillDate>=@StartTime AND a.BillDate<=@EndTime;

	SELECT refno, billDate, storeCode, ShopId FROM #SkechersMasterData

    --明细表参数：

    --编号	字段名	说明	类型/长度	约束说明	备注
    --1	no	国际条码	String/200	不可为空	
    --2	qty	数量	Integer	不可为空	
    --3	price	零售价	Float	可为空	
    --4	priceActual	成交价	Float	不可为空	
    --5	totAmtActual	成交金额	Float	可为空	=qty*priceActual
    --6	discount	折扣	Float	可为空	
    --7	productaliasNo	条码	Float	可为空		
    --8	salesrepCode	营业员编号	String	可为空
    SELECT a.BillNo refno,b.BarCode AS no, a.Qty, a.NPPrice AS price, a.BalaPrice AS priceActual, a.Qty * a.BalaPrice AS totAmtActual
    FROM dbo.SD_POS_SaleDetail a
         LEFT JOIN dbo.SD_Mat_MaterialSize b ON a.MaterialID=b.MaterialID AND a.SizeID=b.SizeID
         INNER JOIN #SkechersMasterData c ON a.BillNo=c.refno
         INNER JOIN dbo.SD_Mat_Material d ON a.MaterialID=d.MaterialID
    WHERE a.CompanyID=@CompanyID and d.CardID=@CardID;
END;

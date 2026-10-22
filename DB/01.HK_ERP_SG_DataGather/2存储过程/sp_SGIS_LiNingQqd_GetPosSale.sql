

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_SGIS_LiNingQqd_GetPosSale]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].sp_SGIS_LiNingQqd_GetPosSale
go


/*----------------------------
全渠道采集POS数据推送到李宁
------------------------------*/
Create PROC [dbo].[sp_SGIS_LiNingQqd_GetPosSale]
	@StartTime  DateTime ,			-- 补采时开始时间  预留暂未使用
	@EndTime DateTime ,				--补采结束时间     预留暂未使用
	@SolutionID INT,
	@isBu INT     --是否补集
AS
BEGIN

DECLARE @VendorID INT, @CompanyID VARCHAR(20) ='', --公司ID 
        @CardID VARCHAR(20); -- 品牌ID 

    SELECT @VendorID=VendorID, @CompanyID=CompanyID, @CardID=CardID
    FROM HK_ERP_SG_XAJX.dbo.Gat_Solution
    WHERE ID=@SolutionId;
	--SELECT @VendorID
    --获取提报方案店铺及品牌
    CREATE TABLE #Shop (ShopID VARCHAR(50), tradeShopCode VARCHAR(50)); --提报店铺
     
	TRUNCATE TABLE #Shop;

    --清空上一次循环的数据（取店铺）

    --直接获取李宁会员供应商下面的所有店铺，不区分具体的提报方案
    --原因：李宁会员的数据是直接所有的一起提报的，如果区分具体品牌，明细就会分开，数据是无法重复上传的
    INSERT INTO #Shop(ShopID, tradeShopCode)
    SELECT DISTINCT ShopID, TradeCode
    FROM HK_ERP_SG_XAJX.dbo.Gat_ShopRight
    WHERE AllowUsed=1 AND CompanyID=@CompanyID AND ISNULL(VendorID, -2)=@VendorID AND TradeCode>'';

	DECLARE @ResultStartTime DATETIME=convert(varchar(10),DATEADD(dd,-1,GETDATE()),120)
	DECLARE @ResultEndTime DATETIME=DATEADD(dd,1,@ResultStartTime)

	IF(@isBu=1)
	BEGIN 
	   SELECT @ResultStartTime=convert(varchar(10),@StartTime,120);
	   SELECT @ResultEndTime=convert(varchar(10),DATEADD(dd,1,@EndTime),120)
	   IF(@ResultEndTime>=convert(varchar(10),DATEADD(dd,1,GETDATE()),120))
	   BEGIN
            SET  @ResultEndTime=convert(varchar(10),GETDATE(),120)
	   end
	end
	

    --取POS数据 先取到主表店铺范围及是金额大于等于0 销售数据
    
    SELECT  CompanyID,BillNo,PayAmount,a.ShopID,b.tradeShopCode, CONVERT(varchar(100), a.BillDate, 112) docDate  INTO #sd_pos_saledt  FROM dbo.SD_POS_SaleMaster a 
    INNER JOIN #Shop b ON b.ShopID = a.ShopID
    WHERE CompanyID=@CompanyID AND  PayAmount>=0 AND BillDate>=@ResultStartTime AND BillDate<@ResultEndTime
    
    
    SELECT  CompanyID,BillNo,PayAmount,a.ShopID,b.tradeShopCode,CONVERT(varchar(100), a.BillDate, 112) docDate INTO #sd_pos_saledt2  FROM dbo.SD_POS_SaleMaster2 a 
    INNER JOIN #Shop b ON b.ShopID = a.ShopID
    WHERE CompanyID=@CompanyID AND  PayAmount>=0 AND BillDate>=@ResultStartTime AND BillDate<@ResultEndTime
    
    --删除其他销售类型的数据
    DELETE a FROM #sd_pos_saledt a 
    INNER JOIN  dbo.SD_POS_SaleDetail b ON b.CompanyID = a.CompanyID AND b.BillNo = a.BillNo
    WHERE b.Qty<0 OR b.SaleType NOT IN ('1','2','4')
    
    DELETE a FROM #sd_pos_saledt2 a 
    INNER JOIN  dbo.SD_POS_SaleDetail2 b ON b.CompanyID = a.CompanyID AND b.BillNo = a.BillNo
    WHERE b.Qty<0 OR b.SaleType NOT IN ('1','2','4')
	      
    DELETE a FROM  dbo.LingNi_QqdData a 
	INNER JOIN #sd_pos_saledt b 
	ON  a.ShopCode=b.tradeShopCode AND a.SaleDate=b.docDate
	WHERE  a.SendStatus='0'


	SELECT  docDate,CAST(CAST( ROUND(SUM(a.PayAmount),2) AS FLOAT) AS VARCHAR(30))  gmv,a.tradeShopCode shopCode  FROM (
	SELECT 
           a.PayAmount,
           a.ShopID ,
           a.tradeShopCode,docDate FROM #sd_pos_saledt a 
	UNION ALL 
	SELECT 
           PayAmount ,
           ShopID ,
           tradeShopCode,docDate FROM #sd_pos_saledt2) a GROUP BY a.ShopID,a.tradeShopCode,docDate



	DROP TABLE #sd_pos_saledt
	DROP TABLE #sd_pos_saledt2


	DROP TABLE #Shop
END


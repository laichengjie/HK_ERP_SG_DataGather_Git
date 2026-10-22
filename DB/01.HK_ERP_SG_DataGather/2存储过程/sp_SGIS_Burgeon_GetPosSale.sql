IF Exists(select 1 from sys.procedures WHERE name='sp_SGIS_Burgeon_GetPosSale')
BEGIN 
	DROP PROCEDURE sp_SGIS_Burgeon_GetPosSale
END 
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* ==========================================================================================
描    述：Burgeon pos 数据
创 建 人：lcj
创建日期：2025-04-15
说明：
更新说明：
-- ==========================================================================================*/
Create PROCEDURE [dbo].[sp_SGIS_Burgeon_GetPosSale]
    @SolutionId INT,          --提报方案Id
    @StartTime DATETIME = '', --开始时间 补采和正常采集时必传
    @EndTime DATETIME = '',   --结束时间 补采和正常采集时必传
    @Ids VARCHAR(500) = ''    --id集合,逗号分割 按单采集时必传
AS
BEGIN
    /*
   EXEC sp_SGIS_Burgeon_GetPosSale
    @SolutionId =69,          --提报方案Id
    @StartTime  = '2025-01-01', --开始时间 补采和正常采集时必传
    @EndTime  = '2025-05-01',   --结束时间 补采和正常采集时必传
    @Ids  = ''    --id集合,逗号分割 按单采集时必传
		   */
    IF OBJECT_ID('tempdB..#Shop') IS NOT NULL
        DROP TABLE #Shop;
    CREATE TABLE #Shop
    (
        ShopID VARCHAR(50),
        ShopCode VARCHAR(50),
        StockID VARCHAR(50),
        TradeCode VARCHAR(50),  --品牌店铺代码
        TradeCode2 VARCHAR(50), --品牌店铺代码
        isShop BIT
    );
	DECLARE @RepeatStartTime DATETIME = GETDATE(); --开始同步时间
    DECLARE @Start DATETIME = GETDATE(); --本次捞取数据的开始时间
    DECLARE @End DATETIME = GETDATE(); --本次捞取数据的结束时间
    DECLARE @HisDays INT = 365; -- 历史数据保留60天
    DECLARE @IsAuto BIT = 1; --提取方案方式 1：自动：0:手动
    DECLARE @VendorID INT = 0; --供应商ID
    DECLARE @CompanyID VARCHAR(20); --公司ID
    DECLARE @CardID VARCHAR(20); --品牌ID

    --①.先获取方案内容
    SELECT TOP 1
           @IsAuto = ISNULL(IsAuto, 1),
           @CompanyID = CompanyID,
           @CardID = CardID,
		   @RepeatStartTime = ISNULL(RepeatStartTime, GETDATE()),
           @VendorID = VendorID
    FROM HK_ERP_SG_DataGather.dbo.Gat_Solution
    WHERE ID = @SolutionId;

	--不同步货品范围
    IF OBJECT_ID('tempdB..#MaterialRange') IS NOT NULL
        DROP TABLE #MaterialRange;
    CREATE TABLE #MaterialRange
    (
        MaterialCode VARCHAR(60)
    );
	INSERT INTO #MaterialRange(MaterialCode)
	SELECT MaterialCode
	FROM HK_ERP_SG_DataGather.dbo.Gat_MaterialRange
	WHERE SolutionID = @SolutionId;

    DELETE FROM #Shop; --清空上一次循环的数据（取店铺）
    INSERT INTO #Shop
    (
        ShopID,
        StockID,
        isShop,
        TradeCode,
        TradeCode2
    )
    SELECT DISTINCT
           a.ShopID,
           b.StockID,
           1 isShop,
           a.TradeCode,
           a.TradeCode2
    FROM HK_ERP_SG_DataGather.dbo.Gat_ShopRight a
        INNER JOIN dbo.Bas_Shop b
            ON a.CompanyID = b.CompanyID
               AND a.ShopID = b.ShopID
    WHERE a.AllowUsed = 1
          AND a.CompanyID = @CompanyID
          AND a.CardID = @CardID
          AND ISNULL(a.VendorID, -2) = @VendorID;

    UPDATE a
    SET a.ShopCode = b.ShopCode
    FROM #Shop a
        INNER JOIN dbo.Bas_Shop b
            ON b.ShopID = a.ShopID;

    --明细查询
    BEGIN
        IF OBJECT_ID('tempdb..#SaleDetail', 'U') IS NOT NULL
            DROP TABLE #SaleDetail;
        CREATE TABLE #SaleDetail
        (
            [ID] [int] IDENTITY(1,1) NOT NULL,
            [SolutionId] [int] NOT NULL,
            [CompanyID] [varchar](20) NOT NULL,
            [BillNo] [varchar](40) NOT NULL,        --线上订单号
            [ExternalBillNo] [varchar](40) NOT NULL,--外部单号
			--[Salesrep] VARCHAR(40)   NULL,--营业员
	  --      [Rate] VARCHAR(10)   NULL,--销售比例
	  --      [CardID] [varchar](20) NULL,--品牌ID
	  --      [ShopID] [varchar](50) NULL,
	  --      [ShopCode] [varchar](50) NULL,--店铺代码
            [MaterialID] VARCHAR(40) NULL,          --货号ID
            [MaterialCode] NVARCHAR(40) NULL,       --货号ID
            [BarCode] NVARCHAR(40) NULL,            --条码
            [Qty] [int] NOT NULL,                   --销售数量
            [BalaPrice] [numeric](24, 6) NOT NULL,  --成交单价
            [PayAmount] [numeric](24, 6) NOT NULL,  --成交金额
            [Type] [int]  NULL,                     --1销售 2退货 3赠品
            [Sequence] VARCHAR(10) NOT NULL,
            [IsImport] [bit] NULL,
			[Saletype] VARCHAR(40) NULL,            --销售类型   SH 销售 TH退货 HH换货 
			PushBillNo VARCHAR(40) NULL             --推送单号
        );

		IF OBJECT_ID('tempdb..#tempBillCodes', 'U') IS NOT NULL
            DROP TABLE #tempBillCodes;
		CREATE TABLE #tempBillCodes
        (
            id VARCHAR(20) NOT NULL,
            CompanyID VARCHAR(20) NOT NULL, --公司
            BillNo VARCHAR(40) NOT NULL,    --单号
            PushBillNo VARCHAR(40) NOT NULL --单号
        );

        IF ISNULL(@Ids, '') > ''
        BEGIN
            IF OBJECT_ID('tempdb..#tempIds', 'U') IS NOT NULL
                DROP TABLE #tempIds;
            SELECT value
            INTO #tempIds
            FROM dbo.fn_Split(@Ids, ',');

		    INSERT INTO #tempBillCodes(CompanyID, BillNo, PushBillNo, id)
            SELECT a.CompanyID,
                   a.BillNo,
                   ISNULL(a.PushBillNo, a.BillNo) PushBillNo,
                   a.id
            FROM HK_ERP_SG_DataGather.dbo.[Gat_Burgeon_PosSale] a
                INNER JOIN #tempIds b
                    ON a.ID = b.value
            WHERE a.SolutionId = @SolutionId
                  AND a.IsSuccess = 0;

            INSERT INTO #SaleDetail
            (
                SolutionId,     --提报方案
                CompanyID,      --公司
                BillNo,         --线上订单号
                ExternalBillNo, --外部单号
                MaterialID,     --货号ID
                MaterialCode,   --货号ID
                BarCode,        --条码
                Qty,            --销售数量
                BalaPrice,      --成交单价
                PayAmount,      --成交金额
                Sequence,
                IsImport        --是否導入
            )
            SELECT
                @SolutionId SolutionID,     --方案ID
                a.CompanyID,                --公司ID
                a.BillNo,                   --线上订单号
                a.BillNo ExternalBillNo,    --外部单号
                a.MaterialID,               --货号ID
                c.MaterialCode,             --货号
                a.BarCode,                  --条码
                ISNULL(a.Qty, 0) Qty,       --销售数量
                a.BalaPrice,                --成交单价
                a.PayAmount,                --成交金额
                a.Sequence,
                0 IsImport                  --是否導入
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
                INNER JOIN #tempBillCodes bill --关联单号查询
                    ON bill.CompanyID = a.CompanyID
                       AND bill.PushBillNo = a.BillNo
            WHERE a.CompanyID = @CompanyID
                  AND e.CardID = @CardID;

            INSERT INTO #SaleDetail
            (
                SolutionId,     --提报方案
                CompanyID,      --公司
                BillNo,         --线上订单号
                ExternalBillNo, --外部单号
                MaterialID,     --货号ID
                MaterialCode,   --货号ID
                BarCode,        --条码
                Qty,            --销售数量
                BalaPrice,      --成交单价
                PayAmount,      --成交金额
                Sequence,
                IsImport        --是否導入
            )
            SELECT
                @SolutionId SolutionID,     --方案ID
                a.CompanyID,                --公司ID
                a.BillNo,                   --线上订单号
                a.BillNo ExternalBillNo,    --外部单号
                a.MaterialID,               --货号ID
                c.MaterialCode,             --货号
                a.BarCode,                  --条码
                ISNULL(a.Qty, 0) Qty,       --销售数量
                a.BalaPrice,                --成交单价
                a.PayAmount,                --成交金额
                a.Sequence,
                0 IsImport                  --是否導入
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
                INNER JOIN #tempBillCodes bill --关联单号查询
                    ON bill.CompanyID = a.CompanyID
                       AND bill.PushBillNo = a.BillNo
            WHERE a.CompanyID = @CompanyID
                  AND e.CardID = @CardID;
        END;
        ELSE
        BEGIN
            SET @Start = @StartTime;
            SET @End = @EndTime;
			IF @Start < @RepeatStartTime
            BEGIN
                SELECT @Start = @RepeatStartTime;
            END;

            INSERT INTO #SaleDetail
            (
                SolutionId,     --提报方案
                CompanyID,      --公司
                BillNo,         --线上订单号
                ExternalBillNo, --外部单号
                MaterialID,     --货号ID
                MaterialCode,   --货号ID
                BarCode,        --条码
                Qty,            --销售数量
                BalaPrice,      --成交单价
                PayAmount,      --成交金额
                Sequence,
                IsImport        --是否導入
            )
            SELECT
                @SolutionId SolutionID,     --方案ID
                a.CompanyID,                --公司ID
                a.BillNo,                   --线上订单号
                a.BillNo ExternalBillNo,    --外部单号
                a.MaterialID,               --货号ID
                c.MaterialCode,             --货号
                a.BarCode,                  --条码
                ISNULL(a.Qty, 0) Qty,       --销售数量
                a.BalaPrice,                --成交单价
                a.PayAmount,                --成交金额
                a.Sequence,
                0 IsImport                  --是否導入
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
            WHERE a.CompanyID = @CompanyID
                  AND c.CardID = @CardID
                  AND b.BillTime > @Start
                  AND b.BillTime <= @End;
				 
            INSERT INTO #SaleDetail
            (
                SolutionId,     --提报方案
                CompanyID,      --公司
                BillNo,         --线上订单号
                ExternalBillNo, --外部单号
                MaterialID,     --货号ID
                MaterialCode,   --货号ID
                BarCode,        --条码
                Qty,            --销售数量
                BalaPrice,      --成交单价
                PayAmount,      --成交金额
                Sequence,
                IsImport        --是否導入
            )
            SELECT
                @SolutionId SolutionID,     --方案ID
                a.CompanyID,                --公司ID
                a.BillNo,                   --线上订单号
                a.BillNo ExternalBillNo,    --外部单号
                a.MaterialID,               --货号ID
                c.MaterialCode,             --货号
                a.BarCode,                  --条码
                ISNULL(a.Qty, 0) Qty,       --销售数量
                a.BalaPrice,                --成交单价
                a.PayAmount,                --成交金额
                a.Sequence,
                0 IsImport                  --是否導入
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
            WHERE a.CompanyID = @CompanyID
                  AND c.CardID = @CardID
                  AND b.BillTime > @Start
                  AND b.BillTime <= @End;
        END;

		-- 明细销售 1销售 2退货 3赠品
	    UPDATE #SaleDetail SET [type] = 1 WHERE ISNULL(Qty, 0) >= 0;
	    UPDATE #SaleDetail SET [type] = 2 WHERE ISNULL(Qty, 0) < 0;
    END;

	--不上传货号处理
	DELETE a
    FROM #SaleDetail a
        INNER JOIN #MaterialRange b
            ON b.MaterialCode = a.MaterialCode;

    --主表
    BEGIN
        IF OBJECT_ID('tempdb..#SaleMaster', 'U') IS NOT NULL
            DROP TABLE #SaleMaster;
        CREATE TABLE #SaleMaster
        (
            [ID] [int] IDENTITY(1,1) NOT NULL,
            [SolutionId] [int] NOT NULL,
            [CompanyID] [varchar](20) NOT NULL,
            [BillDate] [datetime] NOT NULL,          --订单时间
            [BillNo] [varchar](40) NOT NULL,         --线上订单号
            [ExternalBillNo] [varchar](40) NOT NULL, --外部单号
            [Salesrep] VARCHAR(40) NULL,             --营业员
            [Rate] VARCHAR(10) NULL,                 --销售比例
            [CardID] [varchar](20) NULL,             --品牌ID
            [ShopID] [varchar](50) NULL,
            [ShopCode] [varchar](50) NULL,           --店铺代码
            [OrigCode] [varchar](50) NULL,           --发货店仓
            [DestCode] [varchar](50) NULL,           --收货店仓
            [Qty] [int] NULL,                        --数量
            [Amount] [decimal](24, 6) NULL,          --订单总金额（POS单总结算金额）
            [IsImport] [bit] NULL,
            PushBillNo VARCHAR(40) NULL,             --推送单号
            [Saletype] VARCHAR(40) NULL,             --销售类型   SH 销售 TH退货 HH换货 
            PersonnelID VARCHAR(40) NULL             --营业员
        );

        INSERT INTO #SaleMaster
        (
            SolutionId,     --提报方案
            CompanyID,      --公司
			BillDate,
            BillNo,         --单号
			ExternalBillNo, --外部单号
	        Salesrep,       --营业员
	        Rate,           --销售比例
            CardID,         --品牌ID
            ShopID,         --店铺ID
            ShopCode,       --店铺代码
			PersonnelID
        )
        SELECT DISTINCT
		    @SolutionId SolutionId,     --提报方案
            a.CompanyID,                --公司
		    a.BillDate,
            a.BillNo,                   --单号
			a.BillNo ExternalBillNo,
	        '' Salesrep,                --营业员
	        '1' Rate,                   --销售比例
            @CardID CardID,             --品牌ID
            a.ShopID,                   --店铺ID
            k.ShopCode,
			a.PersonnelID
        FROM dbo.SD_POS_SaleMaster a
            INNER JOIN #SaleDetail b
                ON b.CompanyID = a.CompanyID
                   AND b.BillNo = a.BillNo
            INNER JOIN #Shop k
                ON a.ShopID = k.ShopID
        WHERE b.IsImport = 0;

        INSERT INTO #SaleMaster
        (
            SolutionId,     --提报方案
            CompanyID,      --公司
			BillDate,
            BillNo,         --单号
			ExternalBillNo, --外部单号
	        Salesrep,       --营业员
	        Rate,           --销售比例
            CardID,         --品牌ID
            ShopID,         --店铺ID
            ShopCode,       --店铺代码
			PersonnelID
        )
        SELECT DISTINCT
		    @SolutionId SolutionId,     --提报方案
            a.CompanyID,                --公司
		    a.BillDate,
            a.BillNo,                   --单号
			a.BillNo ExternalBillNo,
	        '' Salesrep,                --营业员
	        '1' Rate,                   --销售比例
            @CardID CardID,             --品牌ID
            a.ShopID,                   --店铺ID
            k.ShopCode,
			a.PersonnelID
        FROM dbo.SD_POS_SaleMaster2 a
            INNER JOIN #SaleDetail b
                ON b.CompanyID = a.CompanyID
                   AND b.BillNo = a.BillNo
            INNER JOIN #Shop k
                ON a.ShopID = k.ShopID
        WHERE b.IsImport = 0;

        UPDATE #SaleMaster SET PushBillNo = BillNo;

		--营业员
		UPDATE a
        SET a.Salesrep = b.PersonnelCode
        FROM #SaleMaster a
            INNER JOIN Bas_Personnel b
                ON b.PersonnelID = a.PersonnelID;

        --销售类型处理
        UPDATE a
        SET a.Saletype = 'SH'
        FROM #SaleMaster a;

        UPDATE a
        SET a.Saletype = 'TH'
        FROM #SaleMaster a
        WHERE EXISTS
        (
            SELECT TOP 1 1
            FROM #SaleDetail b
            WHERE b.CompanyID = a.CompanyID
                  AND b.BillNo = a.BillNo
                  AND Qty < 1
        );

        UPDATE a
        SET a.Saletype = 'HH'
        FROM #SaleMaster a
        WHERE EXISTS
        (
            SELECT TOP 1 1
            FROM #SaleDetail b
            WHERE b.CompanyID = a.CompanyID
                  AND b.BillNo = a.BillNo
                  AND Qty < 0
        )
        AND EXISTS
        (
            SELECT TOP 1 1
            FROM #SaleDetail b
            WHERE b.CompanyID = a.CompanyID
                  AND b.BillNo = a.BillNo
                  AND Qty > 0
        );

		--回写明细销售类型
        UPDATE a
        SET a.Saletype = b.Saletype,
		    a.PushBillNo = b.PushBillNo
        FROM #SaleDetail a
            INNER JOIN #SaleMaster b
                ON b.CompanyID = a.CompanyID
                   AND b.BillNo = a.BillNo;
    END;

	--收款
	BEGIN
        IF OBJECT_ID('tempdb..#SaleReceipt', 'U') IS NOT NULL
            DROP TABLE #SaleReceipt;
        CREATE TABLE #SaleReceipt
        (
            [ID] [int] IDENTITY(1,1) NOT NULL,
            [SolutionId] [int] NOT NULL,
            [CompanyID] [varchar](20) NOT NULL,
            [BillNo] [varchar](40) NOT NULL,              --线上订单号
            [ExternalBillNo] [varchar](40) NOT NULL,      --外部单号
            [Sequence] VARCHAR(10) NOT NULL,
            [PaymentMethod] [Nvarchar](60) NULL,          --支付方式 cash现金 coupon购物券 weixinpay微信 unipay银行卡 alipay支付宝
            [Amount] [numeric](24, 6) NULL,               --支付金额
            [BalanceNumber] [Nvarchar](60) NULL,          --券号
            SystemDefID VARCHAR(40) NULL
        );

        INSERT INTO #SaleReceipt
        (
            SolutionId,
            CompanyID,
            BillNo,         --线上订单号
            ExternalBillNo, --外部单号
            Sequence,
            PaymentMethod,  --支付方式 cash现金 coupon购物券 weixinpay微信 unipay银行卡 alipay支付宝
            Amount,         --支付金额
            BalanceNumber,  --券号
            SystemDefID
        )
        SELECT
		    @SolutionId SolutionID,
		    a.CompanyID,
		    a.BillNo,               --线上订单号
		    a.BillNo ExternalBillNo,--外部单号
		    b.Sequence,
		    '' PaymentMethod,       --支付方式 cash现金 coupon购物券 weixinpay微信 unipay银行卡 alipay支付宝
		    SUM(a.ActualReceiptAmount) Amount,     --支付金额
		    B.BalanceNumber,        --券号
            a.SystemDefID
        FROM dbo.SD_Pos_SaleReceiptDetailsub a
            INNER JOIN SD_Pos_SaleReceiptDetail b
                ON b.CompanyID = a.CompanyID
                   AND b.BillNo = a.BillNo
                   AND b.Sequence = a.ReceiptSequence
            INNER JOIN #SaleDetail c
                ON c.CompanyID = a.CompanyID
                   AND c.BillNo = a.BillNo
                   AND c.Sequence = a.DetailSequence
            INNER JOIN #SaleMaster d
                ON d.CompanyID = a.CompanyID
                   AND d.BillNo = a.BillNo
        GROUP BY a.CompanyID,
                 a.BillNo,
                 b.Sequence,
                 a.ReceiptTypeID,
				 B.BalanceNumber,        --券号
                 a.SystemDefID;

        INSERT INTO #SaleReceipt
        (
            SolutionId,
            CompanyID,
            BillNo,         --线上订单号
            ExternalBillNo, --外部单号
            Sequence,
            PaymentMethod,  --支付方式 cash现金 coupon购物券 weixinpay微信 unipay银行卡 alipay支付宝
            Amount,         --支付金额
            BalanceNumber,  --券号
            SystemDefID
        )
        SELECT
		    @SolutionId SolutionID,
		    a.CompanyID,
		    a.BillNo,               --线上订单号
		    a.BillNo ExternalBillNo,--外部单号
		    b.Sequence,
		    '' PaymentMethod,       --支付方式 cash现金 coupon购物券 weixinpay微信 unipay银行卡 alipay支付宝
		    SUM(a.ActualReceiptAmount) Amount,     --支付金额
		    B.BalanceNumber,        --券号
            a.SystemDefID
        FROM dbo.SD_Pos_SaleReceiptDetailsub2 a
            INNER JOIN SD_Pos_SaleReceiptDetail2 b
                ON b.CompanyID = a.CompanyID
                   AND b.BillNo = a.BillNo
                   AND b.Sequence = a.ReceiptSequence
            INNER JOIN #SaleDetail c
                ON c.CompanyID = a.CompanyID
                   AND c.BillNo = a.BillNo
                   AND c.Sequence = a.DetailSequence
            INNER JOIN #SaleMaster d
                ON d.CompanyID = a.CompanyID
                   AND d.BillNo = a.BillNo
        GROUP BY a.CompanyID,
                 a.BillNo,
                 b.Sequence,
                 a.ReceiptTypeID,
				 B.BalanceNumber,
                 a.SystemDefID;

		UPDATE #SaleReceipt SET PaymentMethod = N'未知';
		--Cash	 =>现金
        UPDATE #SaleReceipt SET PaymentMethod = N'现金' WHERE SystemDefID = 'Cash';
		--WXPay	 =>微信
        UPDATE #SaleReceipt SET PaymentMethod = N'微信' WHERE SystemDefID = 'WXPay';
		 --BankCard /Cheque =>银行卡银联VISA
        UPDATE #SaleReceipt SET PaymentMethod = N'银行卡银联VISA' WHERE SystemDefID IN('BankCard','Cheque') ;
		 --Alipay =>支付宝
        UPDATE #SaleReceipt SET PaymentMethod = N'支付宝' WHERE SystemDefID = 'Alipay';
		--CloudFlashPay=>云闪付
        UPDATE #SaleReceipt SET PaymentMethod = N'云闪付' WHERE SystemDefID = 'CloudFlashPay';
		--Other=>调账
        UPDATE #SaleReceipt SET PaymentMethod = N'调账' WHERE SystemDefID = 'Other';
		--THQ  VIPCoupon OtherVIPPoints =>优惠券
        UPDATE #SaleReceipt SET PaymentMethod = N'优惠券' WHERE SystemDefID IN('THQ','VIPCoupon','OtherVIPPoints') ;

		--其他收款都按购物券但是券号 传空
		--Select StateId,StateType,cnStateName as StateName From Sys_State Where StateFixFlag = 'ReceiptType'
		UPDATE #SaleReceipt SET BalanceNumber = '' WHERE SystemDefID NOT IN('THQ','VIPCoupon','OtherVIPPoints') ;


    END;

	--换货拆成两个单  销售\退货  SH 销售   TH 退货   HH 换货 
	BEGIN
        INSERT INTO #SaleMaster
        (
            SolutionId,     --提报方案
            CompanyID,      --公司
			BillDate,
            BillNo,         --单号
			ExternalBillNo, --外部单号
	        Salesrep,       --营业员
	        Rate,           --销售比例
            CardID,         --品牌ID
            ShopID,         --店铺ID
            ShopCode,       --店铺代码
			OrigCode,       --发货店仓
	        DestCode,       --收货店仓
			IsImport,
	        PushBillNo,     --推送单号
            Saletype,       --销售类型   SH 销售 TH退货 HH换货 
	        PersonnelID     --营业员
        )
        SELECT
		    SolutionId,     --提报方案
            CompanyID,      --公司
			BillDate,
            BillNo + '-1' BillNo,                   --单号
			ExternalBillNo + '-1' ExternalBillNo,   --外部单号
	        Salesrep,       --营业员
	        Rate,           --销售比例
            CardID,         --品牌ID
            ShopID,         --店铺ID
            ShopCode,       --店铺代码
			OrigCode,       --发货店仓
	        DestCode,       --收货店仓
			IsImport,
	        PushBillNo,     --推送单号
            'SH' Saletype,  --销售类型   SH 销售 TH退货 HH换货 
	        PersonnelID     --营业员
        FROM #SaleMaster
        WHERE Saletype = 'HH'
        UNION ALL
        SELECT
		    SolutionId,     --提报方案
            CompanyID,      --公司
			BillDate,
            BillNo + '-2' BillNo,                   --单号
			ExternalBillNo + '-2' ExternalBillNo,   --外部单号
	        Salesrep,       --营业员
	        Rate,           --销售比例
            CardID,         --品牌ID
            ShopID,         --店铺ID
            ShopCode,       --店铺代码
			OrigCode,       --发货店仓
	        DestCode,       --收货店仓
			IsImport,
	        PushBillNo,     --推送单号
            'TH' Saletype,  --销售类型   SH 销售 TH退货 HH换货 
	        PersonnelID     --营业员
        FROM #SaleMaster
        WHERE Saletype = 'HH';

        INSERT INTO #SaleDetail
        (
            SolutionId,     --提报方案
            CompanyID,      --公司
            BillNo,         --线上订单号
            ExternalBillNo, --外部单号
            MaterialID,     --货号ID
            MaterialCode,   --货号ID
            BarCode,        --条码
            Qty,            --销售数量
            BalaPrice,      --成交单价
            PayAmount,      --成交金额
            [Type],         --1销售 2退货 3赠品
            Sequence,
            IsImport,       --是否導入
			Saletype,
			PushBillNo 
        )
        SELECT
		    SolutionId,     --提报方案
            CompanyID,      --公司
            a.BillNo + (CASE WHEN a.Qty > 0 THEN '-1' ELSE '-2' END) BillNo,                     --单号         --线上订单号
            a.ExternalBillNo + (CASE WHEN a.Qty > 0 THEN '-1' ELSE '-2' END) ExternalBillNo,     --外部单号
            MaterialID,     --货号ID
            MaterialCode,   --货号ID
            BarCode,        --条码
            Qty,            --销售数量
            BalaPrice,      --成交单价
            PayAmount,      --成交金额
            [Type],         --1销售 2退货 3赠品
            Sequence,
            IsImport,       --是否導入
			'HH' Saletype,
			PushBillNo 
        FROM #SaleDetail a
        WHERE a.Saletype = 'HH';

		 --删除换货明细
         DELETE a FROM #SaleDetail a
		 INNER JOIN #SaleMaster b ON b.CompanyID=a.CompanyID AND b.BillNo=a.BillNo  
		 WHERE B.Saletype = 'HH';
		
		--删除换货主表
         DELETE a FROM #SaleMaster a WHERE a.Saletype = 'HH';

		INSERT INTO #SaleReceipt
        (
            SolutionId,
            CompanyID,
            BillNo,         --线上订单号
            ExternalBillNo, --外部单号
            Sequence,
            PaymentMethod,  --支付方式 cash现金 coupon购物券 weixinpay微信 unipay银行卡 alipay支付宝
            Amount,         --支付金额
            BalanceNumber,  --券号
            SystemDefID
        )
        SELECT
		    distinct
		    a.SolutionID,
            a.CompanyID,
            a.BillNo,         --线上订单号
            a.ExternalBillNo, --外部单号
            b.Sequence,
            b.PaymentMethod,  --支付方式 cash现金 coupon购物券 weixinpay微信 unipay银行卡 alipay支付宝
            b.Amount,         --支付金额
            b.BalanceNumber,  --券号
            b.SystemDefID
        FROM #SaleDetail a 
		INNER join  #SaleReceipt b ON b.CompanyID=a.CompanyID AND  b.BillNo=a.PushBillNo  AND a.Qty>0 AND b.Amount>0
		WHERE a.Saletype='HH';

		INSERT INTO #SaleReceipt
        (
            SolutionId,
            CompanyID,
            BillNo,         --线上订单号
            ExternalBillNo, --外部单号
            Sequence,
            PaymentMethod,  --支付方式 cash现金 coupon购物券 weixinpay微信 unipay银行卡 alipay支付宝
            Amount,         --支付金额
            BalanceNumber,  --券号
            SystemDefID
        )
        SELECT
		    distinct
		    a.SolutionID,
            a.CompanyID,
            a.BillNo,         --线上订单号
            a.ExternalBillNo, --外部单号
            b.Sequence,
            b.PaymentMethod,  --支付方式 cash现金 coupon购物券 weixinpay微信 unipay银行卡 alipay支付宝
            b.Amount,         --支付金额
            b.BalanceNumber,  --券号
            b.SystemDefID
        FROM #SaleDetail a 
		INNER join  #SaleReceipt b ON b.CompanyID=a.CompanyID AND  b.BillNo=a.PushBillNo  AND a.Qty<0 AND b.Amount<0
		WHERE a.Saletype='HH';

		--没有 补一条0现金收款
        INSERT INTO #SaleReceipt
        (
            SolutionId,
            CompanyID,
            BillNo,         --线上订单号
            ExternalBillNo, --外部单号
            Sequence,
            PaymentMethod,  --支付方式 cash现金 coupon购物券 weixinpay微信 unipay银行卡 alipay支付宝
            Amount,         --支付金额
            BalanceNumber,  --券号
            SystemDefID
        )
        SELECT
		    DISTINCT
		    @SolutionId SolutionID,
            CompanyID,
            BillNo,         --线上订单号
            ExternalBillNo, --外部单号
            '0001' Sequence,
            'cash' PaymentMethod,  --支付方式 cash现金 coupon购物券 weixinpay微信 unipay银行卡 alipay支付宝
            0 Amount,         --支付金额
            '' BalanceNumber,  --券号
            'Cash' SystemDefID
        FROM #SaleDetail a
		WHERE a.Saletype='HH'
		and  NOT EXISTS (SELECT TOP 1 1 FROM #SaleReceipt c WHERE c.CompanyID=a.CompanyID AND c.billno=a.billno );

		--删除原来换货单收款明细
        DELETE a
        FROM #SaleReceipt a
        WHERE NOT EXISTS
        (
            SELECT TOP 1 1
            FROM #SaleMaster b
            WHERE b.CompanyID = a.CompanyID
                  AND b.BillNo = a.BillNo
        );

		--换货明细 单价 金额 设置为0
		UPDATE  a SET  a.BalaPrice=0,a.PayAmount=0  FROM  #SaleDetail  a WHERE a.Saletype = 'HH';

		--将换货收款最总金额 写到明细其中一个PayAmount
		-- ===================== 【补：换货收款总额写入明细，反算单价保留2位小数】 =====================
        IF OBJECT_ID('tempdb..#HH_ReceiptTotal') IS NOT NULL
            DROP TABLE #HH_ReceiptTotal;
        CREATE TABLE #HH_ReceiptTotal(
            CompanyID VARCHAR(20),
            BillNo VARCHAR(40),
            TotalPay numeric(24,6)
        );
        INSERT INTO #HH_ReceiptTotal(CompanyID,BillNo,TotalPay)
        SELECT CompanyID, BillNo, SUM(Amount) AS TotalPay
        FROM #SaleReceipt
        GROUP BY CompanyID, BillNo;
			
        UPDATE sd
        SET
            PayAmount = h.TotalPay,
            BalaPrice = CASE WHEN ISNULL(sd.Qty,0) <> 0 THEN ROUND(h.TotalPay / sd.Qty,2) ELSE 0 END
        FROM #SaleDetail sd
        INNER JOIN (
            -- 子查询：每张单据只取第1条明细，作为要更新的目标行
		    --反算换货明细单价=金额、数量 保留2位小数
            SELECT 
                CompanyID, BillNo, ID
            FROM (
                SELECT 
                    CompanyID, BillNo, ID,
                    ROW_NUMBER() OVER(PARTITION BY CompanyID, BillNo ORDER BY ID) AS rn
                FROM #SaleDetail
                WHERE Saletype = 'HH'
            ) t
            WHERE rn = 1 -- 每张单据只取 rn=1 的那一条明细
        ) target ON sd.CompanyID = target.CompanyID AND sd.BillNo = target.BillNo AND sd.ID = target.ID
        INNER JOIN #HH_ReceiptTotal h ON sd.CompanyID = h.CompanyID AND sd.BillNo = h.BillNo
        WHERE sd.Saletype = 'HH';
    END;

	--删除已经同步成功的历史单据
	BEGIN
        IF OBJECT_ID('tempdb..#PosSaleBill', 'U') IS NOT NULL
            DROP TABLE #PosSaleBill;
        CREATE TABLE #PosSaleBill
        (
            CompanyID VARCHAR(20) NOT NULL, --公司
            BillNo VARCHAR(40) NOT NULL     --单号
        );

        IF ISNULL(@Ids, '') > ''
        BEGIN
            INSERT INTO #PosSaleBill(CompanyID, BillNo)
            SELECT a.CompanyID,
                   a.BillNo
            FROM HK_ERP_SG_DataGather.dbo.Gat_Burgeon_PosSale a
                INNER JOIN #tempBillCodes bill
                    ON bill.CompanyID = a.CompanyID
                       AND bill.BillNo = a.BillNo
            WHERE a.SolutionId = @SolutionId
                  AND ISNULL(a.IsSuccess, 0) = 1;

            DELETE a
            FROM #SaleMaster a
                LEFT JOIN #tempBillCodes b
                    ON b.CompanyID = a.CompanyID
                       AND b.BillNo = a.BillNo
            WHERE b.id IS NULL;
        END;
        ELSE
        BEGIN
            INSERT INTO #PosSaleBill(CompanyID, BillNo)
            SELECT a.CompanyID,
                   a.BillNo
            FROM HK_ERP_SG_DataGather.dbo.Gat_Burgeon_PosSale a
            WHERE a.SolutionId = @SolutionId
                  AND ISNULL(a.IsSuccess, 0) = 1;
        END;

        DELETE a
        FROM #SaleDetail a
            INNER JOIN #PosSaleBill b
                ON b.CompanyID = a.CompanyID
                   AND b.BillNo = a.BillNo;
        DELETE a
        FROM #SaleMaster a
            INNER JOIN #PosSaleBill b
                ON b.CompanyID = a.CompanyID
                   AND b.BillNo = a.BillNo;
        DELETE a
        FROM #SaleReceipt a
            INNER JOIN #PosSaleBill b
                ON b.CompanyID = a.CompanyID
                   AND b.BillNo = a.BillNo;
        DELETE a
        FROM #SaleReceipt a
        WHERE NOT EXISTS
        (
            SELECT TOP 1 1
            FROM #SaleMaster b
            WHERE b.CompanyID = a.CompanyID
                  AND b.BillNo = a.BillNo
        );
    END;
 
    --主表数量金额
    UPDATE a
    SET a.qty = b.qty,
        a.Amount = c.Amount
    FROM #SaleMaster a
        INNER JOIN
        (
            SELECT BillNo,
                   SUM(Qty) qty
            FROM #SaleDetail
            GROUP BY BillNo
        ) b
            ON b.BillNo = a.BillNo
        INNER JOIN
        (
            SELECT BillNo,
                   SUM(Amount) Amount
            FROM #SaleReceipt
            GROUP BY BillNo
        ) c
            ON c.BillNo = a.BillNo;

    UPDATE a
    SET a.qty = ISNULL(a.qty, 0),
        a.Amount = ISNULL(a.Amount, 0)
    FROM #SaleMaster a;

    DELETE a
    FROM HK_ERP_SG_DataGather.dbo.Gat_Burgeon_PosSale_Detail a
        INNER JOIN #SaleMaster b
            ON b.CompanyID = a.CompanyID
               AND b.BillNo = a.BillNo
    WHERE a.SolutionID = @SolutionId;

    DELETE a
    FROM HK_ERP_SG_DataGather.dbo.Gat_Burgeon_PosSale_Receipt a
        INNER JOIN #SaleMaster b
            ON b.CompanyID = a.CompanyID
               AND b.BillNo = a.BillNo
    WHERE a.SolutionID = @SolutionId;

    INSERT INTO HK_ERP_SG_DataGather.dbo.Gat_Burgeon_PosSale_Detail
    (
        SolutionId,     --提报方案
        CompanyID,      --公司
        BillNo,         --线上订单号
        ExternalBillNo, --外部单号
		BillDate,
	    Salesrep,--营业员
	    Rate,--销售比例
	    CardID,--品牌ID
	    ShopID,
	    ShopCode,--店铺代码
        MaterialID,     --货号ID
        MaterialCode,   --货号ID
        BarCode,        --条码
        Qty,            --销售数量
        BalaPrice,      --成交单价
        PayAmount,      --成交金额
        [Type],         --1销售 2退货 3赠品
        Sequence,
        IsImport,        --是否導入
		IsSuccess
    )
    SELECT a.SolutionId,      --提报方案
           a.CompanyID,       --公司
           a.BillNo,          --线上订单号
           a.ExternalBillNo,  --外部单号
		   b.BillDate,
		   b.Salesrep,--营业员
	       b.Rate,--销售比例
	       b.CardID,--品牌ID
	       b.ShopID,
	       b.ShopCode,--店铺代码
           a.MaterialID,      --货号ID
           a.MaterialCode,    --货号ID
           a.BarCode,         --条码
           a.Qty,             --销售数量
           a.BalaPrice,       --成交单价
           a.PayAmount,       --成交金额
           a.[Type],          --1销售 2退货 3赠品
           a.Sequence,
           a.IsImport,         --是否導入
		   0 IsSuccess
    FROM #SaleDetail a 
	INNER JOIN #SaleMaster b ON b.CompanyID=a.CompanyID AND b.BillNo=a.BillNo

    INSERT INTO HK_ERP_SG_DataGather.dbo.Gat_Burgeon_PosSale_Receipt
    (
        SolutionId,
        CompanyID,
        BillNo,         --线上订单号
        ExternalBillNo, --外部单号
        Sequence,
        PaymentMethod,  --支付方式 cash现金 coupon购物券 weixinpay微信 unipay银行卡 alipay支付宝
        Amount,         --支付金额
        BalanceNumber   --券号
    )
    SELECT SolutionId,
           CompanyID,
           BillNo,          --线上订单号
           ExternalBillNo,  --外部单号
           Sequence,
           PaymentMethod,   --支付方式 cash现金 coupon购物券 weixinpay微信 unipay银行卡 alipay支付宝
           Amount,          --支付金额
           BalanceNumber    --券号
    FROM #SaleReceipt;

    SELECT  SolutionId,     --提报方案
            CompanyID,      --公司
			BillDate,
            BillNo,         --单号
			ExternalBillNo, --外部单号
	        Salesrep,       --营业员
	        '1'Rate,           --销售比例
            CardID,         --品牌ID
            ShopID,         --店铺ID
            ShopCode,       --店铺代码
			OrigCode,       --发货店仓
	        DestCode,       --收货店仓
			0 IsImport,
	        PushBillNo,     --推送单号
            Saletype,       --销售类型   SH 销售 TH退货 HH换货 
	        PersonnelID,     --营业员
			Qty,
			Amount
    FROM #SaleMaster
    ORDER BY BillNo;

    SELECT SolutionId,      --提报方案
           CompanyID,       --公司
           BillNo,          --线上订单号
           ExternalBillNo,  --外部单号
           MaterialID,      --货号ID
           MaterialCode,    --货号ID
           BarCode,         --条码
           Qty,             --销售数量
           BalaPrice,       --成交单价
           PayAmount,       --成交金额
           [Type],          --1销售 2退货 3赠品
           Sequence,
           IsImport,         --是否導入
		   Saletype,
		   PushBillNo
    FROM #SaleDetail;

    SELECT SolutionId,
           CompanyID,
           BillNo,          --线上订单号
           ExternalBillNo,  --外部单号
           Sequence,
           PaymentMethod,   --支付方式 cash现金 coupon购物券 weixinpay微信 unipay银行卡 alipay支付宝
           SUM(Amount) Amount, --支付金额
           BalanceNumber    --券号
    FROM #SaleReceipt
    GROUP BY SolutionId,
             CompanyID,
             BillNo,         --线上订单号
             ExternalBillNo, --外部单号
             Sequence,
             PaymentMethod,  --支付方式 cash现金 coupon购物券 weixinpay微信 unipay银行卡 alipay支付宝
             BalanceNumber;  --券号

    IF OBJECT_ID('tempdb..#PosSaleBill', 'U') IS NOT NULL
        DROP TABLE #PosSaleBill;
    IF OBJECT_ID('tempdb..#tempIds', 'U') IS NOT NULL
        DROP TABLE #tempIds;
    IF OBJECT_ID('tempdb..#tempBillCodes', 'U') IS NOT NULL
        DROP TABLE #tempBillCodes;
END;

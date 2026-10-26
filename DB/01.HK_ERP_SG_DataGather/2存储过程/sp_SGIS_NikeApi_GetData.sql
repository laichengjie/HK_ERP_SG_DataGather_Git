IF EXISTS (SELECT 1 FROM sys.procedures WHERE name = 'sp_SGIS_NikeApi_GetData')
BEGIN
    DROP PROCEDURE sp_SGIS_NikeApi_GetData
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
CREATE PROCEDURE [dbo].[sp_SGIS_NikeApi_GetData]
    @SolutionId INT,               --提报方案Id
    @StartTime  DATETIME = '',     --开始时间 补采和正常采集时必传
    @EndTime    DATETIME = '',     --结束时间 补采和正常采集时必传
    @Ids        VARCHAR(500) = '', --id集合,逗号分割 按单采集时必传
    @Type       VARCHAR(20) = ''   --单据类型 POS销售
AS
BEGIN
    --方案加载
    BEGIN

        IF OBJECT_ID('tempdb..#Shop') IS NOT NULL
            DROP TABLE #Shop;

        CREATE TABLE #Shop
        (
            ShopID     VARCHAR(50),
            ShopCode   VARCHAR(50),
            StockID    VARCHAR(50),
            StockCode  VARCHAR(50), --仓库代码
            TradeCode  VARCHAR(50), --品牌店铺代码
            TradeCode2 VARCHAR(50), --品牌店铺代码
            isShop     BIT
        );

        IF OBJECT_ID('tempdb..#Log', 'U') IS NOT NULL
            DROP TABLE #Log;

        CREATE TABLE #Log
        (
            [ID] [int] IDENTITY(1,1) NOT NULL,
            [SolutionId] [int] NOT NULL,
            [CompanyID] [varchar](20) NOT NULL,
            [BillType] [varchar](40) NOT NULL, --单据类型
            [BillTypeID][varchar](40) NOT NULL, --单据类型
            [BillTypeName] NVARCHAR(40) NOT NULL, --单据类型名称
            [BillDate] [datetime] NOT NULL, --销售时间
            [BillNo] [varchar](40) NOT NULL, --单号
            [CardID] [varchar](20) NULL, --品牌ID
            [ShopID] [varchar](50) NULL,
            [ShopCode] [varchar](50) NULL, --店铺代码
            [StockID] VARCHAR(50) NULL,   --仓库代码
            [StockCode] VARCHAR(50) NULL, --仓库代码
            [PostJaon] NVARCHAR(MAX) NULL,
            [ReturnJson] NVARCHAR(MAX) NULL,
            [IsSuccess] [bit] NULL,
            [CreateDtm] [datetime] NULL,
            [ModifyDtm] [datetime] NULL,
            [Qty] [int] NULL, --数量
            [Amount] [decimal](24, 6) NULL, --订单总金额（POS单总结算金额）
            [IsImport] [bit] NULL,
            [PushBillNo] VARCHAR(40) NULL, --推送单号
        );

        DECLARE @RepeatStartTime DATETIME = GETDATE(); --开始同步时间
        DECLARE @Start           DATETIME = GETDATE(); --本次捞取数据的开始时间
        DECLARE @End             DATETIME = GETDATE(); --本次捞取数据的结束时间
        DECLARE @HisDays         INT = 365;            --历史数据保留60天
        DECLARE @IsAuto          BIT = 1;              --提取方案方式 1：自动：0:手动
        DECLARE @VendorID        INT = 0;              --供应商ID
        DECLARE @CompanyID       VARCHAR(20);          --公司ID
        DECLARE @CardID          VARCHAR(20);          --品牌ID
        DECLARE @Now             DATETIME = GETDATE();

        --①.先获取方案内容
        SELECT TOP 1
               @IsAuto          = ISNULL(IsAuto, 1),
               @CompanyID       = CompanyID,
               @CardID          = CardID,
               @RepeatStartTime = ISNULL(RepeatStartTime, GETDATE()),
               @VendorID        = VendorID
        FROM HK_ERP_SG_DataGather.dbo.Gat_Solution
        WHERE ID = @SolutionId;

        --不同步货品范围
        IF OBJECT_ID('tempdb..#MaterialRange') IS NOT NULL
            DROP TABLE #MaterialRange;

        CREATE TABLE #MaterialRange
        (
            MaterialCode VARCHAR(60)
        );

        INSERT INTO #MaterialRange (MaterialCode)
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
               1 AS isShop,
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

        --大仓
        INSERT INTO #Shop
        (
            ShopID,
            StockID,
            isShop,
            TradeCode,
            TradeCode2
        )
        SELECT DISTINCT
               NULL AS ShopID,
               b.StockID,
               0 AS isShop,
               a.TradeCode,
               '' AS TradeCode2
        FROM HK_ERP_SG_DataGather.dbo.Gat_StockRight a
            LEFT JOIN dbo.Bas_Stock b
                ON b.StockID = a.StockID
                   AND b.CompanyID = a.CompanyID
        WHERE a.CompanyID = @CompanyID
              AND ISNULL(a.VendorID, -2) = @VendorID
              AND a.CardID = @CardID
              AND a.AllowUsed = 1
              AND NOT EXISTS
              (
                  SELECT TOP 1 1
                  FROM #Shop b
                  WHERE b.StockID = a.StockID
              );

        UPDATE a
        SET a.StockCode = b.StockCode
        FROM #Shop a
            INNER JOIN dbo.Bas_Stock b
                ON b.StockID = a.StockID;

        IF OBJECT_ID('tempdb..#tempBillCodes', 'U') IS NOT NULL
            DROP TABLE #tempBillCodes;

        CREATE TABLE #tempBillCodes
        (
            id         VARCHAR(20) NOT NULL,
            CompanyID  VARCHAR(20) NOT NULL, --公司
            BillNo     VARCHAR(40) NOT NULL, --单号
            PushBillNo VARCHAR(40) NOT NULL  --单号
        );

        IF OBJECT_ID('tempdb..#tempIds', 'U') IS NOT NULL
            DROP TABLE #tempIds;

        --先创建表
        CREATE TABLE #tempIds
        (
            value VARCHAR(50) NOT NULL   --类型按 fn_Split 返回的 value 实际类型/长度来定
        );

        IF ISNULL(@Ids, '') > ''
        BEGIN
            --再插入数据
            INSERT INTO #tempIds (value)
            SELECT value
            FROM dbo.fn_Split(@Ids, ',');

            INSERT INTO #tempBillCodes (CompanyID, BillNo, PushBillNo, id)
            SELECT a.CompanyID,
                   a.BillNo,
                   ISNULL(a.PushBillNo, a.BillNo) AS PushBillNo,
                   a.id
            FROM HK_ERP_SG_DataGather.dbo.[Gat_NikeApi_Log] a
                INNER JOIN #tempIds b
                    ON a.ID = b.value
            WHERE a.SolutionId = @SolutionId
                  AND a.IsSuccess = 0;
        END;

    END

    --销售查询
    IF @Type = 'POS'
    BEGIN
        BEGIN
            IF OBJECT_ID('tempdb..#SaleDetail', 'U') IS NOT NULL
                DROP TABLE #SaleDetail;

            CREATE TABLE #SaleDetail
            (
                [ID]                      INT IDENTITY(1, 1) NOT NULL,
                [SolutionId]              INT NOT NULL,
                [CompanyID]               VARCHAR(20) NOT NULL,
                [BillNo]                  VARCHAR(40) NOT NULL,     --ORDER_NBR 交易/订单号 String 是 交易唯一键
                [CardID]                  VARCHAR(20) NULL,         --品牌ID
                [ShopID]                  VARCHAR(50) NULL,
                [ShopCode]                VARCHAR(50) NULL,
                [STORE_CD]                VARCHAR(50) NULL,         --STORE_CD 门店编码 String 是 订单发生地点的客户门店主数据编码
                [BusinessType]            NVARCHAR(50) NULL,        --BUSINESS_TYPE 业务类型 String 是 订单行级别业务分类
                [BillDate]                DATETIME NULL,            --TRANSACTION_DT 交易时间 Timestamp 是 线下：订单在收银台完成交易的时间戳.线上：发货时间
                [OrderType]               NVARCHAR(50) NULL,        --ORDER_TYPE 订单类型 String 选填 枚举值：销售、退货、换货
                [UPC]                     NVARCHAR(40) NULL,        --UPC 条形码 String 是 条形码，商品唯一识别码
                [MaterialID]              VARCHAR(40) NULL,         --货号ID
                [PRODUCT_CD]              NVARCHAR(40) NULL,        --PRODUCT_CD 货号 String 是 商品编码包含款号和颜色号
                [PRODUCT_NM]              NVARCHAR(60) NULL,        --PRODUCT_NM 商品名称 String 选填 商品名称
                [SIZE_CD]                 NVARCHAR(40) NULL,        --SIZE_CD 尺码 String 是 商品尺码代码
                [LINE_ITEM_NBR]           NVARCHAR(10) NULL,        --LINE_ITEM_NBR 行项目序号 String 是 商品的行项目序号
                [NET_SALES_PRC_INCL_TAX]  NUMERIC(24, 6) NULL,      --NET_SALES_PRC_INCL_TAX 实际销售单价 Decimal 是 实际销售单价
                [REBATE_AMT]              NUMERIC(24, 6) NULL,      --REBATE_AMT 商场补贴金额 Decimal 选填 商场补贴金额
                [NET_SALES_AMT_INCL_TAX]  NUMERIC(24, 6) NULL,      --NET_SALES_AMT_INCL_TAX 实际销售金额 Decimal 是 客户实收金额：由（实际销售单价含税金额 × 数量）派生得出的实收含税总金额；若为退货则为负数
                [SALES_QTY]               INT NULL,                 --SALES_QTY 销售数量 Integer 是 消费者实际购买的商品数量；若为退货则为负数
                [MSRP_LC]                 NUMERIC(24, 6) NULL,      --MSRP_LC 建议零售单价(吊牌价) Decimal 选填 建议零售单价(吊牌价)
                [TOTAL_PARTNER_MSRP_AMT]  NUMERIC(24, 6) NULL,      --TOTAL_PARTNER_MSRP_AMT 吊牌总金额 Decimal 选填 吊牌总金额
                [TOTAL_DISCOUNT_AMT]      NUMERIC(24, 6) NULL,      --TOTAL_DISCOUNT_AMT 折扣总金额 Decimal 选填 本币计的价格调整/折扣总金额
                [ORIGINAL_STORE_CD]       VARCHAR(50) NULL,         --ORIGINAL_STORE_CD 退换货原始门店编码 String 选填 退换货匹配原始门店编码
                [ORIGINAL_ORDER_NBR]      VARCHAR(40) NULL,         --ORIGINAL_ORDER_NBR 退换货原始交易/订单号 String 选填 退换货匹配原始交易流水号/单据号
                [ORIGINAL_TRANSACTION_DT] DATETIME NULL,            --ORIGINAL_TRANSACTION_DT 退换货原始交易日期 Timestamp 选填 退换货匹配原始交易日期YYYY-MM-DD HH:mm:ss
                [ORIGINAL_LINE_ITEM_NBR]  NVARCHAR(10) NULL,        --ORIGINAL_LINE_ITEM_NBR 退换货原始行项目序号 String 选填 退换货匹配原始商品行项目序号
                [LAST_UPDATED_TMST]       DATETIME2(0) NULL,        --LAST_UPDATED_TMST 最后更新时间 Timestamp 是 记录最后更新的时间戳YYYY-MM-DD HH:mm:ss
                [IsImport]                BIT NULL,
                [IsSuccess]               BIT NULL,
                [ModifyDtm]               DATETIME NULL,
                [PushBillNo]              VARCHAR(40) NULL          --推送单号
            );

            IF ISNULL(@Ids, '') > ''
            BEGIN
                INSERT INTO #SaleDetail
                (
                    SolutionId,              --方案
                    CompanyID,               --公司
                    BillNo,                  --单号
                    CardID,                  --品牌
                    ShopID,                  --店铺
                    ShopCode,                --店铺代码
                    STORE_CD,                --店铺代码
                    BusinessType,            --订单行级别业务分类 线下或者线上
                    BillDate,                --销售日期
                    OrderType,               --销售类型销售、退货、换货
                    UPC,                     --条码
                    MaterialID,              --货号ID
                    PRODUCT_CD,              --货号
                    PRODUCT_NM,              --货号名称
                    SIZE_CD,                 --尺码代码
                    LINE_ITEM_NBR,           --POS明细行号
                    NET_SALES_PRC_INCL_TAX,  --结算价
                    REBATE_AMT,              --商场补贴金额 传 null
                    NET_SALES_AMT_INCL_TAX,  --结算金额
                    SALES_QTY,               --数量
                    MSRP_LC,                 --零售价
                    TOTAL_PARTNER_MSRP_AMT,  --零售金额
                    TOTAL_DISCOUNT_AMT,      --传 null 本币计的价格调整/折扣总金额
                    ORIGINAL_STORE_CD,       --传 null 退换货匹配原始门店编码
                    ORIGINAL_ORDER_NBR,      --传 null 退换货匹配原始交易流水号/单据号
                    ORIGINAL_TRANSACTION_DT, --传 null 退换货匹配原始交易日期
                    ORIGINAL_LINE_ITEM_NBR,  --传 null 退换货匹配原始商品行项目序号
                    --LAST_UPDATED_TMST,     --记录最后更新的时间戳 YYYY-MM-DD HH:mm:ss
                    IsImport,                --是否导入
                    IsSuccess,               --是否成功
                    --ModifyDtm,             --提报时间
                    PushBillNo               --推送单号
                )
                SELECT
                    @SolutionId                       AS SolutionId,         --方案
                    a.CompanyID                       AS CompanyID,          --公司
                    a.BillNo                          AS BillNo,             --单号
                    @CardID                           AS CardID,             --品牌
                    a.ShopID                          AS ShopID,             --店铺
                    ISNULL(NULLIF(k.TradeCode, ''), k.ShopCode)   AS ShopCode,           --店铺代码
                    ISNULL(NULLIF(k.TradeCode, ''), k.ShopCode)   AS STORE_CD,           --店铺代码
                    CASE WHEN ISNULL(b.BCSourceBillNo, '') = '' AND ISNULL(b.OMSBillNo, '') = '' THEN N'线下'  ELSE N'线上'  END   AS BusinessType,       --订单行级别业务分类 线下或者线上
                    b.BillDate                        AS BillDate,           --销售日期
                    N'销售'                           AS OrderType,          --销售类型销售、退货、换货
                    a.BarCode                         AS UPC,                --条码
                    a.MaterialID                      AS MaterialID,         --货号ID
                    a.MaterialCode                    AS PRODUCT_CD,         --货号
                    a.MaterialShortName               AS PRODUCT_NM,         --货号名称
                    s.SizeCode                        AS SIZE_CD,            --尺码代码
                    a.Sequence                        AS LINE_ITEM_NBR,      --POS明细行号
                    a.BalaPrice                       AS NET_SALES_PRC_INCL_TAX, --结算价
                    NULL                              AS REBATE_AMT,         --商场补贴金额 传 null
                    a.PayAmount                       AS NET_SALES_AMT_INCL_TAX, --结算金额
                    ISNULL(a.Qty, 0)                  AS SALES_QTY,          --数量
                    a.Price                           AS MSRP_LC,            --零售价
                    a.Price * ISNULL(a.Qty, 0)        AS TOTAL_PARTNER_MSRP_AMT, --零售金额
                    NULL                              AS TOTAL_DISCOUNT_AMT, --传 null 本币计的价格调整/折扣总金额
                    NULL                              AS ORIGINAL_STORE_CD,  --传 null 退换货匹配原始门店编码
                    NULL                              AS ORIGINAL_ORDER_NBR, --传 null 退换货匹配原始交易流水号/单据号
                    NULL                              AS ORIGINAL_TRANSACTION_DT, --传 null 退换货匹配原始交易日期
                    NULL                              AS ORIGINAL_LINE_ITEM_NBR, --传 null 退换货匹配原始商品行项目序号
                    --LAST_UPDATED_TMST,            --记录最后更新的时间戳 YYYY-MM-DD HH:mm:ss
                    0                                 AS IsImport,           --是否导入
                    0                                 AS IsSuccess,          --是否成功
                    --ModifyDtm,                   --提报时间
                    a.BillNo                          AS PushBillNo          --推送单号
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
                    INNER JOIN #tempBillCodes bill --关联单号查询
                        ON bill.CompanyID = a.CompanyID
                           AND bill.PushBillNo = a.BillNo
                    LEFT JOIN SD_Mat_Size s
                        ON s.SizeID = a.SizeID
                WHERE a.CompanyID = @CompanyID
                      AND c.CardID = @CardID;

                INSERT INTO #SaleDetail
                (
                    SolutionId,              --方案
                    CompanyID,               --公司
                    BillNo,                  --单号
                    CardID,                  --品牌
                    ShopID,                  --店铺
                    ShopCode,                --店铺代码
                    STORE_CD,                --店铺代码
                    BusinessType,            --订单行级别业务分类 线下或者线上
                    BillDate,                --销售日期
                    OrderType,               --销售类型销售、退货、换货
                    UPC,                     --条码
                    MaterialID,              --货号ID
                    PRODUCT_CD,              --货号
                    PRODUCT_NM,              --货号名称
                    SIZE_CD,                 --尺码代码
                    LINE_ITEM_NBR,           --POS明细行号
                    NET_SALES_PRC_INCL_TAX,  --结算价
                    REBATE_AMT,              --商场补贴金额 传 null
                    NET_SALES_AMT_INCL_TAX,  --结算金额
                    SALES_QTY,               --数量
                    MSRP_LC,                 --零售价
                    TOTAL_PARTNER_MSRP_AMT,  --零售金额
                    TOTAL_DISCOUNT_AMT,      --传 null 本币计的价格调整/折扣总金额
                    ORIGINAL_STORE_CD,       --传 null 退换货匹配原始门店编码
                    ORIGINAL_ORDER_NBR,      --传 null 退换货匹配原始交易流水号/单据号
                    ORIGINAL_TRANSACTION_DT, --传 null 退换货匹配原始交易日期
                    ORIGINAL_LINE_ITEM_NBR,  --传 null 退换货匹配原始商品行项目序号
                    --LAST_UPDATED_TMST,     --记录最后更新的时间戳 YYYY-MM-DD HH:mm:ss
                    IsImport,                --是否导入
                    IsSuccess,               --是否成功
                    --ModifyDtm,             --提报时间
                    PushBillNo               --推送单号
                )
                SELECT
                    @SolutionId                       AS SolutionId,         --方案
                    a.CompanyID                       AS CompanyID,          --公司
                    a.BillNo                          AS BillNo,             --单号
                    @CardID                           AS CardID,             --品牌
                    a.ShopID                          AS ShopID,             --店铺
                    ISNULL(NULLIF(k.TradeCode, ''), k.ShopCode)   AS ShopCode,           --店铺代码
                    ISNULL(NULLIF(k.TradeCode, ''), k.ShopCode)   AS STORE_CD,           --店铺代码
                    CASE WHEN ISNULL(b.BCSourceBillNo, '') = '' AND ISNULL(b.OMSBillNo, '') = '' THEN N'线下'  ELSE N'线上'  END    AS BusinessType,       --订单行级别业务分类 线下或者线上
                    b.BillDate                        AS BillDate,           --销售日期
                    N'销售'                           AS OrderType,          --销售类型销售、退货、换货
                    a.BarCode                         AS UPC,                --条码
                    a.MaterialID                      AS MaterialID,         --货号ID
                    a.MaterialCode                    AS PRODUCT_CD,         --货号
                    a.MaterialShortName               AS PRODUCT_NM,         --货号名称
                    s.SizeCode                        AS SIZE_CD,            --尺码代码
                    a.Sequence                        AS LINE_ITEM_NBR,      --POS明细行号
                    a.BalaPrice                       AS NET_SALES_PRC_INCL_TAX, --结算价
                    NULL                              AS REBATE_AMT,         --商场补贴金额 传 null
                    a.PayAmount                       AS NET_SALES_AMT_INCL_TAX, --结算金额
                    ISNULL(a.Qty, 0)                  AS SALES_QTY,          --数量
                    a.Price                           AS MSRP_LC,            --零售价
                    a.Price * ISNULL(a.Qty, 0)        AS TOTAL_PARTNER_MSRP_AMT, --零售金额
                    NULL                              AS TOTAL_DISCOUNT_AMT, --传 null 本币计的价格调整/折扣总金额
                    NULL                              AS ORIGINAL_STORE_CD,  --传 null 退换货匹配原始门店编码
                    NULL                              AS ORIGINAL_ORDER_NBR, --传 null 退换货匹配原始交易流水号/单据号
                    NULL                              AS ORIGINAL_TRANSACTION_DT, --传 null 退换货匹配原始交易日期
                    NULL                              AS ORIGINAL_LINE_ITEM_NBR, --传 null 退换货匹配原始商品行项目序号
                    --LAST_UPDATED_TMST,            --记录最后更新的时间戳 YYYY-MM-DD HH:mm:ss
                    0                                 AS IsImport,           --是否导入
                    0                                 AS IsSuccess,          --是否成功
                    --ModifyDtm,                   --提报时间
                    a.BillNo                          AS PushBillNo          --推送单号
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
                    INNER JOIN #tempBillCodes bill --关联单号查询
                        ON bill.CompanyID = a.CompanyID
                           AND bill.PushBillNo = a.BillNo
                    LEFT JOIN SD_Mat_Size s
                        ON s.SizeID = a.SizeID
                WHERE a.CompanyID = @CompanyID
                      AND c.CardID = @CardID;
            END;
            ELSE
            BEGIN
                SET @Start = @StartTime;
                SET @End   = @EndTime;

                IF @Start < @RepeatStartTime
                BEGIN
                    SELECT @Start = @RepeatStartTime;
                END;

                INSERT INTO #SaleDetail
                (
                    SolutionId,              --方案
                    CompanyID,               --公司
                    BillNo,                  --单号
                    CardID,                  --品牌
                    ShopID,                  --店铺
                    ShopCode,                --店铺代码
                    STORE_CD,                --店铺代码
                    BusinessType,            --订单行级别业务分类 线下或者线上
                    BillDate,                --销售日期
                    OrderType,               --销售类型销售、退货、换货
                    UPC,                     --条码
                    MaterialID,              --货号ID
                    PRODUCT_CD,              --货号
                    PRODUCT_NM,              --货号名称
                    SIZE_CD,                 --尺码代码
                    LINE_ITEM_NBR,           --POS明细行号
                    NET_SALES_PRC_INCL_TAX,  --结算价
                    REBATE_AMT,              --商场补贴金额 传 null
                    NET_SALES_AMT_INCL_TAX,  --结算金额
                    SALES_QTY,               --数量
                    MSRP_LC,                 --零售价
                    TOTAL_PARTNER_MSRP_AMT,  --零售金额
                    TOTAL_DISCOUNT_AMT,      --传 null 本币计的价格调整/折扣总金额
                    ORIGINAL_STORE_CD,       --传 null 退换货匹配原始门店编码
                    ORIGINAL_ORDER_NBR,      --传 null 退换货匹配原始交易流水号/单据号
                    ORIGINAL_TRANSACTION_DT, --传 null 退换货匹配原始交易日期
                    ORIGINAL_LINE_ITEM_NBR,  --传 null 退换货匹配原始商品行项目序号
                    --LAST_UPDATED_TMST,     --记录最后更新的时间戳 YYYY-MM-DD HH:mm:ss
                    IsImport,                --是否导入
                    IsSuccess,               --是否成功
                    --ModifyDtm,             --提报时间
                    PushBillNo               --推送单号
                )
                SELECT
                    @SolutionId                       AS SolutionId,         --方案
                    a.CompanyID                       AS CompanyID,          --公司
                    a.BillNo                          AS BillNo,             --单号
                    @CardID                           AS CardID,             --品牌
                    a.ShopID                          AS ShopID,             --店铺
                    ISNULL(NULLIF(k.TradeCode, ''), k.ShopCode)   AS ShopCode,           --店铺代码
                    ISNULL(NULLIF(k.TradeCode, ''), k.ShopCode)   AS STORE_CD,           --店铺代码
                    CASE WHEN ISNULL(b.BCSourceBillNo, '') = '' AND ISNULL(b.OMSBillNo, '') = '' THEN N'线下'  ELSE N'线上'  END                               AS BusinessType,       --订单行级别业务分类 线下或者线上
                    b.BillDate                        AS BillDate,           --销售日期
                    N'销售'                           AS OrderType,          --销售类型销售、退货、换货
                    a.BarCode                         AS UPC,                --条码
                    a.MaterialID                      AS MaterialID,         --货号ID
                    a.MaterialCode                    AS PRODUCT_CD,         --货号
                    a.MaterialShortName               AS PRODUCT_NM,         --货号名称
                    s.SizeCode                        AS SIZE_CD,            --尺码代码
                    a.Sequence                        AS LINE_ITEM_NBR,      --POS明细行号
                    a.BalaPrice                       AS NET_SALES_PRC_INCL_TAX, --结算价
                    NULL                              AS REBATE_AMT,         --商场补贴金额 传 null
                    a.PayAmount                       AS NET_SALES_AMT_INCL_TAX, --结算金额
                    ISNULL(a.Qty, 0)                  AS SALES_QTY,          --数量
                    a.Price                           AS MSRP_LC,            --零售价
                    a.Price * ISNULL(a.Qty, 0)        AS TOTAL_PARTNER_MSRP_AMT, --零售金额
                    NULL                              AS TOTAL_DISCOUNT_AMT, --传 null 本币计的价格调整/折扣总金额
                    NULL                              AS ORIGINAL_STORE_CD,  --传 null 退换货匹配原始门店编码
                    NULL                              AS ORIGINAL_ORDER_NBR, --传 null 退换货匹配原始交易流水号/单据号
                    NULL                              AS ORIGINAL_TRANSACTION_DT, --传 null 退换货匹配原始交易日期
                    NULL                              AS ORIGINAL_LINE_ITEM_NBR, --传 null 退换货匹配原始商品行项目序号
                    --LAST_UPDATED_TMST,            --记录最后更新的时间戳 YYYY-MM-DD HH:mm:ss
                    0                                 AS IsImport,           --是否导入
                    0                                 AS IsSuccess,          --是否成功
                    --ModifyDtm,                   --提报时间
                    a.BillNo                          AS PushBillNo          --推送单号
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
                           AND k.isShop = 1
                    LEFT JOIN SD_Mat_Size s
                        ON s.SizeID = a.SizeID
                WHERE a.CompanyID = @CompanyID
                      AND c.CardID = @CardID
                      AND b.BillTime > @Start
                      AND b.BillTime <= @End;

                INSERT INTO #SaleDetail
                (
                    SolutionId,              --方案
                    CompanyID,               --公司
                    BillNo,                  --单号
                    CardID,                  --品牌
                    ShopID,                  --店铺
                    ShopCode,                --店铺代码
                    STORE_CD,                --店铺代码
                    BusinessType,            --订单行级别业务分类 线下或者线上
                    BillDate,                --销售日期
                    OrderType,               --销售类型销售、退货、换货
                    UPC,                     --条码
                    MaterialID,              --货号ID
                    PRODUCT_CD,              --货号
                    PRODUCT_NM,              --货号名称
                    SIZE_CD,                 --尺码代码
                    LINE_ITEM_NBR,           --POS明细行号
                    NET_SALES_PRC_INCL_TAX,  --结算价
                    REBATE_AMT,              --商场补贴金额 传 null
                    NET_SALES_AMT_INCL_TAX,  --结算金额
                    SALES_QTY,               --数量
                    MSRP_LC,                 --零售价
                    TOTAL_PARTNER_MSRP_AMT,  --零售金额
                    TOTAL_DISCOUNT_AMT,      --传 null 本币计的价格调整/折扣总金额
                    ORIGINAL_STORE_CD,       --传 null 退换货匹配原始门店编码
                    ORIGINAL_ORDER_NBR,      --传 null 退换货匹配原始交易流水号/单据号
                    ORIGINAL_TRANSACTION_DT, --传 null 退换货匹配原始交易日期
                    ORIGINAL_LINE_ITEM_NBR,  --传 null 退换货匹配原始商品行项目序号
                    --LAST_UPDATED_TMST,     --记录最后更新的时间戳 YYYY-MM-DD HH:mm:ss
                    IsImport,                --是否导入
                    IsSuccess,               --是否成功
                    --ModifyDtm,             --提报时间
                    PushBillNo               --推送单号
                )
                SELECT
                    @SolutionId                       AS SolutionId,         --方案
                    a.CompanyID                       AS CompanyID,          --公司
                    a.BillNo                          AS BillNo,             --单号
                    @CardID                           AS CardID,             --品牌
                    a.ShopID                          AS ShopID,             --店铺
                    ISNULL(NULLIF(k.TradeCode, ''), k.ShopCode)   AS ShopCode,           --店铺代码
                    ISNULL(NULLIF(k.TradeCode, ''), k.ShopCode)   AS STORE_CD,           --店铺代码
                    CASE WHEN ISNULL(b.BCSourceBillNo, '') = '' AND ISNULL(b.OMSBillNo, '') = '' THEN N'线下'  ELSE N'线上'  END                               AS BusinessType,       --订单行级别业务分类 线下或者线上
                    b.BillDate                        AS BillDate,           --销售日期
                    N'销售'                           AS OrderType,          --销售类型销售、退货、换货
                    a.BarCode                         AS UPC,                --条码
                    a.MaterialID                      AS MaterialID,         --货号ID
                    a.MaterialCode                    AS PRODUCT_CD,         --货号
                    a.MaterialShortName               AS PRODUCT_NM,         --货号名称
                    s.SizeCode                        AS SIZE_CD,            --尺码代码
                    a.Sequence                        AS LINE_ITEM_NBR,      --POS明细行号
                    a.BalaPrice                       AS NET_SALES_PRC_INCL_TAX, --结算价
                    NULL                              AS REBATE_AMT,         --商场补贴金额 传 null
                    a.PayAmount                       AS NET_SALES_AMT_INCL_TAX, --结算金额
                    ISNULL(a.Qty, 0)                  AS SALES_QTY,          --数量
                    a.Price                           AS MSRP_LC,            --零售价
                    a.Price * ISNULL(a.Qty, 0)        AS TOTAL_PARTNER_MSRP_AMT, --零售金额
                    NULL                              AS TOTAL_DISCOUNT_AMT, --传 null 本币计的价格调整/折扣总金额
                    NULL                              AS ORIGINAL_STORE_CD,  --传 null 退换货匹配原始门店编码
                    NULL                              AS ORIGINAL_ORDER_NBR, --传 null 退换货匹配原始交易流水号/单据号
                    NULL                              AS ORIGINAL_TRANSACTION_DT, --传 null 退换货匹配原始交易日期
                    NULL                              AS ORIGINAL_LINE_ITEM_NBR, --传 null 退换货匹配原始商品行项目序号
                    --LAST_UPDATED_TMST,            --记录最后更新的时间戳 YYYY-MM-DD HH:mm:ss
                    0                                 AS IsImport,           --是否导入
                    0                                 AS IsSuccess,          --是否成功
                    --ModifyDtm,                   --提报时间
                    a.BillNo                          AS PushBillNo          --推送单号
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
                           AND k.isShop = 1
                    LEFT JOIN SD_Mat_Size s
                        ON s.SizeID = a.SizeID
                WHERE a.CompanyID = @CompanyID
                      AND c.CardID = @CardID
                      AND b.BillTime > @Start
                      AND b.BillTime <= @End;
            END;
        END;

        --删除已经同步成功的历史单据
        DELETE a
        FROM #SaleDetail a
        WHERE EXISTS
        (
            SELECT 1
            FROM HK_ERP_SG_DataGather.dbo.Gat_NikeApi_Log b
            WHERE b.SolutionId = @SolutionId
                  AND b.IsSuccess = 1
                  AND b.CompanyID = a.CompanyID
                  AND b.BillNo = a.BillNo
        );

        --不上传货号处理
        DELETE a
        FROM #SaleDetail a
            INNER JOIN #MaterialRange b
                ON b.MaterialCode = a.PRODUCT_CD;

        --销售类型处理 销售、退货、换货
        BEGIN
            UPDATE a
            SET a.OrderType = N'销售'
            FROM #SaleDetail a;

            UPDATE a
            SET a.OrderType = N'退货'
            FROM #SaleDetail a
            WHERE EXISTS
            (
                SELECT TOP 1 1
                FROM #SaleDetail b
                WHERE b.CompanyID = a.CompanyID
                      AND b.BillNo = a.BillNo
                      AND SALES_QTY < 0
            );

            UPDATE a
            SET a.OrderType = N'换货'
            FROM #SaleDetail a
            WHERE EXISTS
            (
                SELECT TOP 1 1
                FROM #SaleDetail b
                WHERE b.CompanyID = a.CompanyID
                      AND b.BillNo = a.BillNo
                      AND SALES_QTY < 0
            )
            AND EXISTS
            (
                SELECT TOP 1 1
                FROM #SaleDetail b
                WHERE b.CompanyID = a.CompanyID
                      AND b.BillNo = a.BillNo
                      AND SALES_QTY > 0
            );
        END;

        --删除已存在数据，避免重复记录
        BEGIN
            DELETE a
            FROM HK_ERP_SG_DataGather.dbo.Gat_NikeApi_PosSale a
                INNER JOIN #SaleDetail b
                    ON b.CompanyID = a.CompanyID
                       AND b.BillNo = a.BillNo
            WHERE a.SolutionID = @SolutionId;

            DELETE a
            FROM HK_ERP_SG_DataGather.dbo.Gat_NikeApi_Log a
                INNER JOIN #SaleDetail b
                    ON b.CompanyID = a.CompanyID
                       AND b.BillNo = a.BillNo
            WHERE a.SolutionID = @SolutionId;
        END;

        --更新修改时间
        UPDATE a
        SET LAST_UPDATED_TMST = @Now,
            ModifyDtm         = @Now
        FROM #SaleDetail a;

        INSERT INTO #Log
        (
            SolutionId ,
            CompanyID ,
            BillType ,
            BillTypeID ,
            BillTypeName ,
            BillDate ,
            BillNo ,
            CardID ,
            ShopID ,
            ShopCode ,
            StockID ,
            StockCode ,
            PostJaon ,
            ReturnJson ,
            IsSuccess ,
            CreateDtm ,
            ModifyDtm ,
            Qty ,
            Amount ,
            IsImport ,
            PushBillNo
        )
        SELECT MAX(SolutionId)                        AS SolutionId,
               CompanyID,
               'POS'                                  AS BillType,
               'POS'                                  AS BillTypeID,
               N'POS销售单' BillTypeName,
               MAX(BillDate)                          AS BillDate,
               BillNo,
               MAX(CardID)                            AS CardID,
               MAX(ShopID)                            AS ShopID,
               MAX(ShopCode)                          AS ShopCode,
               ''                                     AS  StockID ,
               ''                                     AS StockCode,
               ''                                     AS PostJaon,
               ''                                     AS ReturnJson,
               0                                      AS IsSuccess,
               @Now                                   AS CreateDtm,
               @Now                                   AS ModifyDtm,
               SUM(ISNULL(SALES_QTY, 0))              AS Qty,
               SUM(ISNULL(NET_SALES_AMT_INCL_TAX, 0)) AS Amount,
               0                                      AS IsImport,
               MAX(PushBillNo)                        AS PushBillNo
        FROM #SaleDetail
        WHERE SolutionId = @SolutionId
        GROUP BY CompanyID, BillNo;

        INSERT INTO HK_ERP_SG_DataGather.dbo.Gat_NikeApi_PosSale
        (
            SolutionId,              --方案
            CompanyID,               --公司
            BillNo,                  --单号
            CardID,                  --品牌
            ShopID,                  --店铺
            ShopCode,                --店铺代码
            STORE_CD,                --店铺代码
            BusinessType,            --订单行级别业务分类 线下或者线上
            BillDate,                --销售日期
            OrderType,               --销售类型销售、退货、换货
            UPC,                     --条码
            MaterialID,              --货号ID
            PRODUCT_CD,              --货号
            PRODUCT_NM,              --货号名称
            SIZE_CD,                 --尺码代码
            LINE_ITEM_NBR,           --POS明细行号
            NET_SALES_PRC_INCL_TAX,  --结算价
            REBATE_AMT,              --商场补贴金额 传 null
            NET_SALES_AMT_INCL_TAX,  --结算金额
            SALES_QTY,               --数量
            MSRP_LC,                 --零售价
            TOTAL_PARTNER_MSRP_AMT,  --零售金额
            TOTAL_DISCOUNT_AMT,      --传 null 本币计的价格调整/折扣总金额
            ORIGINAL_STORE_CD,       --传 null 退换货匹配原始门店编码
            ORIGINAL_ORDER_NBR,      --传 null 退换货匹配原始交易流水号/单据号
            ORIGINAL_TRANSACTION_DT, --传 null 退换货匹配原始交易日期
            ORIGINAL_LINE_ITEM_NBR,  --传 null 退换货匹配原始商品行项目序号
            LAST_UPDATED_TMST,       --记录最后更新的时间戳 YYYY-MM-DD HH:mm:ss
            IsImport,                --是否导入
            IsSuccess,               --是否成功
            ModifyDtm,               --提报时间
            PushBillNo               --推送单号
        )
        SELECT SolutionId,              --方案
               CompanyID,               --公司
               BillNo,                  --单号
               CardID,                  --品牌
               ShopID,                  --店铺
               ShopCode,                --店铺代码
               STORE_CD,                --店铺代码
               BusinessType,            --订单行级别业务分类 线下或者线上
               BillDate,                --销售日期
               OrderType,               --销售类型销售、退货、换货
               UPC,                     --条码
               MaterialID,              --货号ID
               PRODUCT_CD,              --货号
               PRODUCT_NM,              --货号名称
               SIZE_CD,                 --尺码代码
               LINE_ITEM_NBR,           --POS明细行号
               NET_SALES_PRC_INCL_TAX,  --结算价
               REBATE_AMT,              --商场补贴金额 传 null
               NET_SALES_AMT_INCL_TAX,  --结算金额
               SALES_QTY,               --数量
               MSRP_LC,                 --零售价
               TOTAL_PARTNER_MSRP_AMT,  --零售金额
               TOTAL_DISCOUNT_AMT,      --传 null 本币计的价格调整/折扣总金额
               ORIGINAL_STORE_CD,       --传 null 退换货匹配原始门店编码
               ORIGINAL_ORDER_NBR,      --传 null 退换货匹配原始交易流水号/单据号
               ORIGINAL_TRANSACTION_DT, --传 null 退换货匹配原始交易日期
               ORIGINAL_LINE_ITEM_NBR,  --传 null 退换货匹配原始商品行项目序号
               LAST_UPDATED_TMST,       --记录最后更新的时间戳 YYYY-MM-DD HH:mm:ss
               IsImport,                --是否导入
               IsSuccess,               --是否成功
               ModifyDtm,               --提报时间
               PushBillNo               --推送单号
        FROM #SaleDetail a;
        SELECT SolutionId,              --方案
               CompanyID,               --公司
               BillNo,                  --单号
               CardID,                  --品牌
               ShopID,                  --店铺
               ShopCode,                --店铺代码
               STORE_CD,                --店铺代码
               BusinessType,            --订单行级别业务分类 线下或者线上
               BillDate,                --销售日期
               OrderType,               --销售类型销售、退货、换货
               UPC,                     --条码
               MaterialID,              --货号ID
               PRODUCT_CD,              --货号
               PRODUCT_NM,              --货号名称
               SIZE_CD,                 --尺码代码
               LINE_ITEM_NBR,           --POS明细行号
               NET_SALES_PRC_INCL_TAX,  --结算价
               REBATE_AMT,              --商场补贴金额 传 null
               NET_SALES_AMT_INCL_TAX,  --结算金额
               SALES_QTY,               --数量
               MSRP_LC,                 --零售价
               TOTAL_PARTNER_MSRP_AMT,  --零售金额
               TOTAL_DISCOUNT_AMT,      --传 null 本币计的价格调整/折扣总金额
               ORIGINAL_STORE_CD,       --传 null 退换货匹配原始门店编码
               ORIGINAL_ORDER_NBR,      --传 null 退换货匹配原始交易流水号/单据号
               ORIGINAL_TRANSACTION_DT, --传 null 退换货匹配原始交易日期
               ORIGINAL_LINE_ITEM_NBR,  --传 null 退换货匹配原始商品行项目序号
               LAST_UPDATED_TMST,       --记录最后更新的时间戳 YYYY-MM-DD HH:mm:ss
               IsImport,                --是否导入
               IsSuccess,               --是否成功
               ModifyDtm,               --提报时间
               PushBillNo               --推送单号
        FROM #SaleDetail;

        IF OBJECT_ID('tempdb..#SaleDetail', 'U') IS NOT NULL
            DROP TABLE #SaleDetail;
    END;

    --库存
    IF @Type = 'INV'
    BEGIN
        BEGIN
            IF OBJECT_ID('tempdb..#Inv', 'U') IS NOT NULL
                DROP TABLE #Inv;

            CREATE TABLE #Inv
            (
                [ID]                 [int] IDENTITY(1, 1) NOT NULL,
                [SolutionId]         [int] NOT NULL,
                [CompanyID]          [varchar](20) NOT NULL,
                [CardID]             [varchar](20) NULL, --品牌ID
                [StockID]            VARCHAR(50) NULL,   --仓库代码
                [StockCode]          VARCHAR(50) NULL,   --仓库代码
                [SITE_CD]            [varchar](50)  NULL, --SITE_CD 门店/仓库编码 String 是 客户门店或仓库唯一标识编码
                [UPC]                NVARCHAR(40)  NULL,  --UPC 条形码 String 是 条形码，商品唯一识别码
                [MaterialID]         VARCHAR(40) NULL,       --货号ID
                [PRODUCT_CD]         NVARCHAR(40) NULL,      --PRODUCT_CD 货号 String 是 商品编码包含款号和颜色号
                [PRODUCT_NM]         NVARCHAR(60) NULL,      --PRODUCT_NM 商品名称 String 选填 商品名称
                [SIZE_CD]            NVARCHAR(40) NULL,      --SIZE_CD 尺码 String 是 商品尺码代码
                [INVENTORY_DT]       [datetime] NULL,        --INVENTORY_DT 库存日期 Date 是 库存快照日期YYYY-MM-DD
                [TOTAL_QTY]          [int] NULL,             --TOTAL_QTY 总库存 Integer 是 总库存
                [ON_HAND_QTY]        [int] NULL,             --ON_HAND_QTY 在库库存 Integer 是 在库库存
                [IN_TRANSIT_QTY]     [int] NULL,             --IN_TRANSIT_QTY 在途库存 Integer 是 仓库/门店未入库的在途库存数量
                [DEFECTIVE_PCS_QTY]  [int] NULL,             --DEFECTIVE_PCS_QTY 残次品库存 Integer 是 残次品库存
                [MSRP_LC]            [numeric](24, 6) NULL,  --MSRP_LC 建议零售单价(吊牌价) Decimal 选填 建议零售单价(吊牌价)
                [LAST_UPDATED_TMST]  DATETIME2(0)  NULL,  --LAST_UPDATED_TMST 最后更新时间 Timestamp 是 记录最后更新的时间戳YYYY-MM-DD HH:mm:ss
                [IsImport]           [bit] NULL,
                [IsSuccess]          [bit] NULL,
                [ModifyDtm]          [datetime] NULL,
            );

            IF ISNULL(@Ids, '') > ''
            BEGIN
                INSERT INTO #Inv
                (
                    SolutionId,        -- 方案
                    CompanyID,         --公司
                    CardID,            --品牌
                    StockID,           --仓库代码
                    StockCode,         --仓库代码
                    SITE_CD,           --仓库代码
                    UPC,               --条码
                    MaterialID,        --货号
                    PRODUCT_CD,        --货号
                    PRODUCT_NM,        --货号名称
                    SIZE_CD,           --尺码代码
                    --INVENTORY_DT,    --库存日期
                    TOTAL_QTY,         --总库存
                    ON_HAND_QTY,       --在库库存
                    IN_TRANSIT_QTY,    --在途库存
                    DEFECTIVE_PCS_QTY, --残次品库存
                    MSRP_LC            -- 建议零售单价
                    --LAST_UPDATED_TMST, --最后更新时间
                    --IsImport,        --是否成功
                    --IsSuccess,       --是否导入
                    --ModifyDtm        --修改时间
                )
                SELECT
                    @SolutionID AS SolutionID, -- 方案
                    a.CompanyID,               --公司
                    @CardID CardID,            --品牌
                    a.StockID,                 --仓库代码
                    ISNULL(NULLIF(k.TradeCode, ''), k.StockCode) StockCode, --仓库代码
                    CASE k.isShop WHEN 1 THEN ISNULL(NULLIF(k.TradeCode, ''), k.ShopCode) ELSE ISNULL(NULLIF(k.TradeCode, ''), k.StockCode) END SITE_CD, --店铺/仓库代码
                    d.BarCode UPC,             --条码
                    a.MaterialID,              --货号
                    f.MaterialCode PRODUCT_CD, --货号
                    f.MaterialShortName PRODUCT_NM, --货号名称
                    n.SizeCode SIZE_CD,        --尺码代码
                    --INVENTORY_DT,            --库存日期
                    ISNULL(b.ActualQty, 0) + ISNULL(b.PathQty, 0) TOTAL_QTY, --总库存
                    ISNULL(b.ActualQty, 0) ON_HAND_QTY,        --在库库存
                    ISNULL(b.PathQty, 0) IN_TRANSIT_QTY,       --在途库存
                    0 DEFECTIVE_PCS_QTY,                       --残次品库存
                    f.ProRetailPrice MSRP_LC                                  -- 建议零售单价
                    --LAST_UPDATED_TMST,       --最后更新时间
                    --IsImport,                --是否成功
                    --IsSuccess,               --是否导入
                    --ModifyDtm                --修改时间
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
                    LEFT JOIN dbo.SD_Mat_Size n
                        ON b.SizeID = n.SizeID
                    INNER JOIN #Shop k
                        ON a.StockID = k.StockID
                WHERE a.CompanyID = @CompanyID
                      AND f.CardID = @CardID;

            END;
            ELSE
            BEGIN
                INSERT INTO #Inv
                (
                    SolutionId,        -- 方案
                    CompanyID,         --公司
                    CardID,            --品牌
                    StockID,           --仓库代码
                    StockCode,         --仓库代码
                    SITE_CD,           --仓库代码
                    UPC,               --条码
                    MaterialID,        --货号
                    PRODUCT_CD,        --货号
                    PRODUCT_NM,        --货号名称
                    SIZE_CD,           --尺码代码
                    --INVENTORY_DT,    --库存日期
                    TOTAL_QTY,         --总库存
                    ON_HAND_QTY,       --在库库存
                    IN_TRANSIT_QTY,    --在途库存
                    DEFECTIVE_PCS_QTY, --残次品库存
                    MSRP_LC            -- 建议零售单价
                    --LAST_UPDATED_TMST, --最后更新时间
                    --IsImport,        --是否成功
                    --IsSuccess,       --是否导入
                    --ModifyDtm        --修改时间
                )
                SELECT
                    @SolutionID AS SolutionID, -- 方案
                    a.CompanyID,               --公司
                    @CardID CardID,            --品牌
                    a.StockID,                 --仓库代码
                    ISNULL(NULLIF(k.TradeCode, ''), k.StockCode) StockCode, --仓库代码
                    CASE k.isShop WHEN 1 THEN ISNULL(NULLIF(k.TradeCode, ''), k.ShopCode) ELSE ISNULL(NULLIF(k.TradeCode, ''), k.StockCode) END SITE_CD, --店铺/仓库代码
                    d.BarCode UPC,             --条码
                    a.MaterialID,              --货号
                    f.MaterialCode PRODUCT_CD, --货号
                    f.MaterialShortName PRODUCT_NM, --货号名称
                    n.SizeCode SIZE_CD,        --尺码代码
                    --INVENTORY_DT,            --库存日期
                    ISNULL(b.ActualQty, 0) + ISNULL(b.PathQty, 0) TOTAL_QTY, --总库存
                    ISNULL(b.ActualQty, 0) ON_HAND_QTY,        --在库库存
                    ISNULL(b.PathQty, 0) IN_TRANSIT_QTY,       --在途库存
                    0 DEFECTIVE_PCS_QTY,                       --残次品库存
                    f.ProRetailPrice MSRP_LC                                  -- 建议零售单价
                    --LAST_UPDATED_TMST,       --最后更新时间
                    --IsImport,                --是否成功
                    --IsSuccess,               --是否导入
                    --ModifyDtm                --修改时间
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
                    LEFT JOIN dbo.SD_Mat_Size n
                        ON b.SizeID = n.SizeID
                    INNER JOIN #Shop k
                        ON a.StockID = k.StockID
                WHERE a.CompanyID = @CompanyID
                      AND f.CardID = @CardID;

            END;
        END;

        --删除已经同步成功的历史单据
        DELETE a
        FROM HK_ERP_SG_DataGather.dbo.Gat_NikeApi_Inv a
        WHERE EXISTS
        (
            SELECT 1
            FROM #Inv b
            WHERE b.SolutionId = @SolutionId
                  AND b.CompanyID = a.CompanyID
                  AND b.StockID = a.StockID
        );

        --不上传货号处理
        DELETE a
        FROM #Inv a
            INNER JOIN #MaterialRange b
                ON b.MaterialCode = a.PRODUCT_CD;
        --更新零售价

        --更新修改时间
        UPDATE a
        SET LAST_UPDATED_TMST = @Now,
            ModifyDtm         = @Now,
            INVENTORY_DT      = @Now,
            IsImport=0,
            IsSuccess=0
        FROM #Inv a;

        INSERT INTO #Log
        (
            SolutionId,
            CompanyID,
            BillType,
            BillTypeID,
            BillTypeName,
            BillDate,
            BillNo,
            CardID,
            ShopID,
            ShopCode,
            StockID ,
            StockCode,
            PostJaon,
            ReturnJson,
            IsSuccess,
            CreateDtm,
            ModifyDtm,
            Qty,
            Amount,
            IsImport,
            PushBillNo
        )
        SELECT MAX(SolutionId)           AS SolutionId,
               CompanyID,
               'INV'                     AS BillType,
               'INV'                     AS BillTypeID,
               N'库存明细' BillTypeName,
               @Now                      AS BillDate,
               MAX(StockID)              AS BillNo,
               @CardID                   AS CardID,
               ''                        AS ShopID,
               ''                        AS ShopCode,
               ''                        AS StockID ,
               MAX(SITE_CD)              AS StockCode,
               ''                        AS PostJaon,
               ''                        AS ReturnJson,
               0                         AS IsSuccess,
               @Now                      AS CreateDtm,
               @Now                      AS ModifyDtm,
               SUM(ISNULL(TOTAL_QTY, 0)) AS Qty,
               0                         AS Amount,
               0                         AS IsImport,
               MAX(StockID)              AS PushBillNo
        FROM #Inv
        WHERE SolutionId = @SolutionId
        GROUP BY CompanyID, StockID;

        INSERT INTO HK_ERP_SG_DataGather.dbo.Gat_NikeApi_Inv
        (
            SolutionId,        -- 方案
            CompanyID,         --公司
            CardID,            --品牌
            StockID,           --仓库代码
            StockCode,         --仓库代码
            SITE_CD,           --仓库代码
            UPC,               --条码
            MaterialID,        --货号
            PRODUCT_CD,        --货号
            PRODUCT_NM,        --货号名称
            SIZE_CD,           --尺码代码
            INVENTORY_DT,      --库存日期
            TOTAL_QTY,         --总库存
            ON_HAND_QTY,       --在库库存
            IN_TRANSIT_QTY,    --在途库存
            DEFECTIVE_PCS_QTY, --残次品库存
            MSRP_LC,           -- 建议零售单价
            LAST_UPDATED_TMST, --最后更新时间
            IsImport,          --是否成功
            IsSuccess,         --是否导入
            ModifyDtm          --修改时间
        )
        SELECT SolutionId,        -- 方案
               CompanyID,         --公司
               CardID,            --品牌
               StockID,           --仓库代码
               StockCode,         --仓库代码
               SITE_CD,           --仓库代码
               UPC,               --条码
               MaterialID,        --货号
               PRODUCT_CD,        --货号
               PRODUCT_NM,        --货号名称
               SIZE_CD,           --尺码代码
               INVENTORY_DT,      --库存日期
               TOTAL_QTY,         --总库存
               ON_HAND_QTY,       --在库库存
               IN_TRANSIT_QTY,    --在途库存
               DEFECTIVE_PCS_QTY, --残次品库存
               MSRP_LC,           -- 建议零售单价
               LAST_UPDATED_TMST, --最后更新时间
               IsImport,          --是否成功
               IsSuccess,         --是否导入
               ModifyDtm          --修改时间
        FROM #Inv a;

        SELECT SolutionId,        -- 方案
               CompanyID,         --公司
               CardID,            --品牌
               StockID,           --仓库代码
               StockCode,         --仓库代码
               SITE_CD,           --仓库代码
               UPC,               --条码
               MaterialID,        --货号
               PRODUCT_CD,        --货号
               PRODUCT_NM,        --货号名称
               SIZE_CD,           --尺码代码
               INVENTORY_DT,      --库存日期
               TOTAL_QTY,         --总库存
               ON_HAND_QTY,       --在库库存
               IN_TRANSIT_QTY,    --在途库存
               DEFECTIVE_PCS_QTY, --残次品库存
               MSRP_LC,           -- 建议零售单价
               LAST_UPDATED_TMST, --最后更新时间
               IsImport,          --是否成功
               IsSuccess,         --是否导入
               ModifyDtm          --修改时间
        FROM #Inv;

        IF OBJECT_ID('tempdb..#Inv', 'U') IS NOT NULL
            DROP TABLE #Inv;
    END;

    --出入库
    IF @Type = 'OutIn'
    BEGIN
        BEGIN
            IF OBJECT_ID('tempdb..#OutIn', 'U') IS NOT NULL
                DROP TABLE #OutIn;

            CREATE TABLE #OutIn
            (
                [ID] [int] IDENTITY(1,1) NOT NULL,
                [SolutionId] [int] NOT NULL,
                [CompanyID] [varchar](20) NOT NULL,
                [BillNo] [varchar](40) NOT NULL, --单号
                [BillDate] [datetime] NOT NULL, --单据日期
                [CardID] [varchar](20) NULL, --品牌ID
                [SITE_CD] [varchar](50) NOT NULL, --SITE_CD 门店/仓库编码 String 是 客户门店或仓库唯一标识编码
                [SITE_NM] NVARCHAR(50) NULL, --SITE_NM 门店/仓库名称 String 是 位置名称
                [UPC] NVARCHAR(40) NOT NULL, --UPC 条形码 String 是 条形码，商品唯一识别码
                [MaterialID] VARCHAR(40) NULL, --货号ID
                [PRODUCT_CD] NVARCHAR(40) NULL, --PRODUCT_CD 货号 String 是 商品编码包含款号和颜色号
                [PRODUCT_NM] NVARCHAR(60) NULL, --PRODUCT_NM 商品名称 String 选填 商品名称
                [SIZE_CD] NVARCHAR(40) NULL, --SIZE_CD 尺码 String 是 商品尺码代码
                [DIRECTION_DESC] [varchar](50) NULL, --DIRECTION_DESC 流转方向 String 是 交易方向：出库或入库
                [WMS_ORDER_NO] [varchar](40) NOT NULL, --WMS_ORDER_NO 仓库作业流水单号 String 是 仓库作业流水单号，仅应用于仓库
                [PROOF_OF_DELIVERY_NO] [varchar](40) NULL, --PROOF_OF_DELIVERY_NO 发货单号 String 是 关联耐克发货单和退还耐克发货单， 耐克发货为 装箱单号EPL， 耐克退货为 耐克退货订单号 RSO
                [STATUS_TYPE_DESC] [varchar](40) NULL, --STATUS_TYPE_DESC 状态类型描述 String 是 客户出入库类型，枚举值：耐克发货、退还耐克、加盟发货、加盟退货
                [TRANSACTION_TMST] DATETIME2(0) NOT NULL, --TRANSACTION_TMST 流水作业时间戳 Timestamp 是 流水作业时间戳包含日期和具体时间YYYY-MM-DD HH:mm:ss
                [TRANSACTION_QTY] [int] NULL, --TRANSACTION_QTY 流水处理件数 Integer 是 流水处理件数
                [SOURCE_FROM_SITE_CD] [varchar](50) NULL, --SOURCE_FROM_SITE_CD 源位置编码 String 选填 入库产品源头仓库位置唯一标识
                [SOURCE_FROM_SITE_NM] NVARCHAR(50) NULL, --SOURCE_FROM_SITE_NM 源位置名称 String 选填 入库产品源头仓库位置名称
                [DESTINATION_TO_SITE_CD] [varchar](50) NULL, --DESTINATION_TO_SITE_CD 目的位置编码 String 选填 出库产品目的仓库位置唯一标识
                [DESTINATION_TO_SITE_NM] NVARCHAR(50) NULL, --DESTINATION_TO_SITE_NM 目的位置名称 String 选填 出库产品目的仓库位置名称
                [LAST_UPDATED_TMST] DATETIME2(0)  NULL, --LAST_UPDATED_TMST 最后更新时间 Timestamp 是 记录最后更新的时间戳YYYY-MM-DD HH:mm:ss
                [IsImport] [bit] NULL,
                [IsSuccess] [bit] NULL,
                [ModifyDtm] [datetime] NULL,
                [PushBillNo] VARCHAR(40) NULL,             --推送单号
                [BillTypeName] NVARCHAR(40) NOT NULL, --单据类型名称
                [BillTypeID] VARCHAR(20) NULL,
            );

            BEGIN
                SET @Start = @StartTime;
                SET @End   = @EndTime;

                IF @Start < @RepeatStartTime
                BEGIN
                    SELECT @Start = @RepeatStartTime;
                END;

                INSERT INTO #OutIn
                (
                    SolutionId,        --方案
                    CompanyID,         --公司
                    BillNo,
                    BillDate,          --单据日期
                    CardID,            --品牌
                    SITE_CD,           --仓库编码
                    SITE_NM,           --仓库名称
                    UPC,               --条码
                    MaterialID,        --货号
                    PRODUCT_CD,        --货号
                    PRODUCT_NM,        --可为空 货号名称
                    SIZE_CD,           --尺码代码
                    DIRECTION_DESC,    ----交易方向：出库或入库
                    WMS_ORDER_NO,      --单号
                    PROOF_OF_DELIVERY_NO, --来源单单号
                    STATUS_TYPE_DESC,  --待定  客户出入库类型
                    TRANSACTION_TMST,  --单据生效时间
                    TRANSACTION_QTY,   --数量
                    SOURCE_FROM_SITE_CD, --传空 入库产品源头仓库位置唯一标识
                    SOURCE_FROM_SITE_NM, --传空  入库产品源头仓库位置名称
                    DESTINATION_TO_SITE_CD, --传空 出库产品目的仓库位置唯一标识
                    DESTINATION_TO_SITE_NM, --传空  出库产品目的仓库位置名称
                    --LAST_UPDATED_TMST, --最后更新时间
                    --IsImport,
                    --IsSuccess,
                    --ModifyDtm,
                    PushBillNo,
                    BillTypeName, --单据类型
                    BillTypeID                --单据类型
                )
                SELECT
                    @SolutionID  SolutionId, --方案
                    a.CompanyID     CompanyID, --公司
                    a.BillNo,
                    m.CheckDate     BillDate, --单据日期
                    @CardID    CardID, --品牌
                    d.StockCode   SITE_CD, --仓库编码
                    d.StockCode  SITE_NM, --仓库名称
                    ms.BarCode   UPC, --条码
                    c.MaterialID  MaterialID, --货号
                    c.MaterialCode   PRODUCT_CD, --货号
                    ''  PRODUCT_NM, --可为空 货号名称
                    f.SizeCode   SIZE_CD, --尺码代码
                    (CASE
                        WHEN m.BillTypeID = 'Inv_Ssck' THEN
                            N'出库'
                        WHEN m.BillTypeID = 'Inv_Thck' THEN
                            N'出库'
                        ELSE
                            N'入库'
                    END
                    )  DIRECTION_DESC, ----交易方向：出库或入库
                    a.BillNo   WMS_ORDER_NO, --单号
                    m.SourceBillNo   PROOF_OF_DELIVERY_NO, --来源单单号
                    (CASE
                        WHEN m.BillTypeID = 'Inv_Cgrk' THEN
                            N'耐克发货'
                        WHEN m.BillTypeID = 'Inv_Thck' THEN
                            N'退还耐克'
                        WHEN m.BillTypeID = 'Inv_Ssck' THEN
                            N'加盟发货'
                        WHEN m.BillTypeID = 'Inv_Thrk' THEN
                            N'加盟退货'
                        ELSE
                            ''
                    END
                    )  STATUS_TYPE_DESC, --待定  客户出入库类型
                    m.CheckDate  TRANSACTION_TMST, --单据生效时间
                    a.Qty  TRANSACTION_QTY, --数量
                    '' SOURCE_FROM_SITE_CD, --传空 入库产品源头仓库位置唯一标识
                    '' SOURCE_FROM_SITE_NM, --传空  入库产品源头仓库位置名称
                    '' DESTINATION_TO_SITE_CD, --传空 出库产品目的仓库位置唯一标识
                    '' DESTINATION_TO_SITE_NM, --传空  出库产品目的仓库位置名称
                    -- LAST_UPDATED_TMST, --最后更新时间
                    --  IsImport,
                    -- IsSuccess,
                    --  ModifyDtm,
                    a.BillNo  PushBillNo,
                    bt.BillTypeName BillTypeName, --单据类型
                    m.BillTypeID                 --单据类型
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
                    INNER JOIN #Shop d
                        ON d.StockID = m.StockID
                    INNER JOIN dbo.SD_Mat_Size f
                        ON a.SizeID = f.SizeID
                    LEFT JOIN dbo.Sys_BillType bt
                        ON bt.BillTypeID = m.BillTypeID
                    JOIN dbo.SD_Mat_MaterialSize ms
                        ON ms.SizeID = a.SizeID
                           AND ms.MaterialID = a.MaterialID
                WHERE m.CompanyID = @CompanyID
                      AND c.CardID = @CardID
                      AND m.CheckDate > @Start
                      AND m.CheckDate <= @End
                      AND m.BillTypeID IN ('Inv_Cgrk', 'Inv_Thrk', 'Inv_Ssck', 'Inv_Thck')
                      AND m.BillStatus=4;

            END;
        END;

        --删除已经同步成功的历史单据
        DELETE a
        FROM #OutIn a
        WHERE EXISTS
        (
            SELECT 1
            FROM HK_ERP_SG_DataGather.dbo.Gat_NikeApi_Log b
            WHERE b.SolutionId = @SolutionId
                  AND b.IsSuccess = 1
                  AND b.CompanyID = a.CompanyID
                  AND b.BillNo = a.BillNo
                  AND b.BillTypeID=a.BillTypeID
        );

        --不上传货号处理
        DELETE a
        FROM #OutIn a
            INNER JOIN #MaterialRange b
                ON b.MaterialCode = a.PRODUCT_CD;

        --删除已存在数据，避免重复记录
        BEGIN
            DELETE a
            FROM HK_ERP_SG_DataGather.dbo.Gat_NikeApi_OutIn a
                INNER JOIN #OutIn b
                    ON b.CompanyID = a.CompanyID
                       AND b.BillNo = a.BillNo
                       AND b.BillTypeID=a.BillTypeID
            WHERE a.SolutionID = @SolutionId;

            DELETE a
            FROM HK_ERP_SG_DataGather.dbo.Gat_NikeApi_Log a
                INNER JOIN #OutIn b
                    ON b.CompanyID = a.CompanyID
                       AND b.BillNo = a.BillNo
                       AND b.BillTypeID=a.BillTypeID
            WHERE a.SolutionID = @SolutionId;
        END;

        --更新修改时间
        UPDATE a
        SET LAST_UPDATED_TMST = @Now,
            ModifyDtm         = @Now,
            IsImport=0,
            IsSuccess=0
        FROM #OutIn a;

        --记录本次上报日志（按单记一条）
        INSERT INTO #Log
        (
            SolutionId,
            CompanyID,
            BillType,
            BillTypeID,
            BillTypeName,
            BillDate,
            BillNo,
            CardID,
            ShopID,
            ShopCode,
            StockID ,
            StockCode,
            PostJaon,
            ReturnJson,
            IsSuccess,
            CreateDtm,
            ModifyDtm,
            Qty,
            Amount,
            IsImport,
            PushBillNo
        )
        SELECT MAX(SolutionId)                        AS SolutionId,
               CompanyID,
               @Type BillType,
               BillTypeID                            AS BillTypeID,
               MAX(BillTypeName) BillTypeName,
               MAX(BillDate)                          AS BillDate,
               BillNo,
               @CardID                            AS CardID,
               ''                            AS ShopID,
               ''                          AS ShopCode,
               ''                          AS StockID ,
               MAX(SITE_CD)                          AS StockCode,
               ''                                     AS PostJaon,
               ''                                     AS ReturnJson,
               0                                      AS IsSuccess,
               @Now                                   AS CreateDtm,
               @Now                                   AS ModifyDtm,
               SUM(ISNULL(TRANSACTION_QTY, 0))              AS Qty,
               0 AS Amount,
               0                                      AS IsImport,
               MAX(PushBillNo)                        AS PushBillNo
        FROM #OutIn
        WHERE SolutionId = @SolutionId
        GROUP BY CompanyID, BillNo,BillTypeID

        INSERT INTO HK_ERP_SG_DataGather.dbo.Gat_NikeApi_OutIn
        (
            SolutionId,        --方案
            CompanyID,         --公司
            BillNo,
            BillDate,          --单据日期
            CardID,            --品牌
            SITE_CD,           --仓库编码
            SITE_NM,           --仓库名称
            UPC,               --条码
            MaterialID,        --货号
            PRODUCT_CD,        --货号
            PRODUCT_NM,        --可为空 货号名称
            SIZE_CD,           --尺码代码
            DIRECTION_DESC,    ----交易方向：出库或入库
            WMS_ORDER_NO,      --单号
            PROOF_OF_DELIVERY_NO, --来源单单号
            STATUS_TYPE_DESC,  --待定  客户出入库类型
            TRANSACTION_TMST,  --单据生效时间
            TRANSACTION_QTY,   --数量
            SOURCE_FROM_SITE_CD, --传空 入库产品源头仓库位置唯一标识
            SOURCE_FROM_SITE_NM, --传空  入库产品源头仓库位置名称
            DESTINATION_TO_SITE_CD, --传空 出库产品目的仓库位置唯一标识
            DESTINATION_TO_SITE_NM, --传空  出库产品目的仓库位置名称
            LAST_UPDATED_TMST, --最后更新时间
            IsImport,
            IsSuccess,
            ModifyDtm,
            PushBillNo,
            BillTypeName, --单据类型
            BillTypeID                --单据类型
        )
        SELECT SolutionId,        --方案
               CompanyID,         --公司
               BillNo,
               BillDate,          --单据日期
               CardID,            --品牌
               SITE_CD,           --仓库编码
               SITE_NM,           --仓库名称
               UPC,               --条码
               MaterialID,        --货号
               PRODUCT_CD,        --货号
               PRODUCT_NM,        --可为空 货号名称
               SIZE_CD,           --尺码代码
               DIRECTION_DESC,    ----交易方向：出库或入库
               WMS_ORDER_NO,      --单号
               PROOF_OF_DELIVERY_NO, --来源单单号
               STATUS_TYPE_DESC,  --待定  客户出入库类型
               TRANSACTION_TMST,  --单据生效时间
               TRANSACTION_QTY,   --数量
               SOURCE_FROM_SITE_CD, --传空 入库产品源头仓库位置唯一标识
               SOURCE_FROM_SITE_NM, --传空  入库产品源头仓库位置名称
               DESTINATION_TO_SITE_CD, --传空 出库产品目的仓库位置唯一标识
               DESTINATION_TO_SITE_NM, --传空  出库产品目的仓库位置名称
               LAST_UPDATED_TMST, --最后更新时间
               IsImport,
               IsSuccess,
               ModifyDtm,
               PushBillNo,
               BillTypeName, --单据类型
               BillTypeID                --单据类型
        FROM #OutIn a;

        SELECT SolutionId,        --方案
               CompanyID,         --公司
               BillNo,
               BillDate,          --单据日期
               CardID,            --品牌
               SITE_CD,           --仓库编码
               SITE_NM,           --仓库名称
               UPC,               --条码
               MaterialID,        --货号
               PRODUCT_CD,        --货号
               PRODUCT_NM,        --可为空 货号名称
               SIZE_CD,           --尺码代码
               DIRECTION_DESC,    ----交易方向：出库或入库
               WMS_ORDER_NO,      --单号
               PROOF_OF_DELIVERY_NO, --来源单单号
               STATUS_TYPE_DESC,  --待定  客户出入库类型
               TRANSACTION_TMST,  --单据生效时间
               TRANSACTION_QTY,   --数量
               SOURCE_FROM_SITE_CD, --传空 入库产品源头仓库位置唯一标识
               SOURCE_FROM_SITE_NM, --传空  入库产品源头仓库位置名称
               DESTINATION_TO_SITE_CD, --传空 出库产品目的仓库位置唯一标识
               DESTINATION_TO_SITE_NM, --传空  出库产品目的仓库位置名称
               LAST_UPDATED_TMST, --最后更新时间
               IsImport,
               IsSuccess,
               ModifyDtm,
               PushBillNo,
               BillTypeName, --单据类型
               BillTypeID                --单据类型
        FROM #OutIn;

        IF OBJECT_ID('tempdb..#OutIn', 'U') IS NOT NULL
            DROP TABLE #OutIn;
    END;

    INSERT INTO HK_ERP_SG_DataGather.dbo.Gat_NikeApi_Log
    (
        SolutionId,
        CompanyID,
        BillType,
        BillTypeID,
        BillTypeName,
        BillDate,
        BillNo,
        CardID,
        ShopID,
        ShopCode,
        StockCode,
        PostJaon,
        ReturnJson,
        IsSuccess,
        CreateDtm,
        ModifyDtm,
        Qty,
        Amount,
        IsImport,
        PushBillNo
    )
    SELECT  SolutionId,
        CompanyID,
        BillType,
        BillTypeID,
        BillTypeName,
        BillDate,
        BillNo,
        CardID,
        ShopID,
        ShopCode,
        StockCode,
        PostJaon,
        ReturnJson,
        IsSuccess,
        CreateDtm,
        ModifyDtm,
        Qty,
        Amount,
        IsImport,
        PushBillNo
    FROM #Log

    SELECT  SolutionId,
        CompanyID,
        BillType,
        BillTypeID,
        BillTypeName,
        BillDate,
        BillNo,
        --CardID,
        --ShopID,
        --ShopCode,
        --StockCode,
        --PostJaon,
        --ReturnJson,
        --IsSuccess,
        --CreateDtm,
        --ModifyDtm,
        Qty,
        Amount,
        --IsImport,
        PushBillNo
    FROM #Log

    IF OBJECT_ID('tempdb..#tempIds', 'U') IS NOT NULL
        DROP TABLE #tempIds;
    IF OBJECT_ID('tempdb..#tempBillCodes', 'U') IS NOT NULL
        DROP TABLE #tempBillCodes;
	IF OBJECT_ID('tempdb..#Log', 'U') IS NOT NULL
        DROP TABLE #Log;
END;
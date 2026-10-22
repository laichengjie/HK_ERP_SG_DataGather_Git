/*
 drop table  Gat_NikeApi_Log
 drop table  Gat_NikeApi_PosSale
 drop table  Gat_NikeApi_Inv
 drop table  Gat_NikeApi_OutIn
*/

IF NOT EXISTS (SELECT 1 FROM sysObjects WHERE Id = OBJECT_ID(N'Gat_NikeApi_Log') AND xtype = 'U')
BEGIN
CREATE TABLE [dbo].[Gat_NikeApi_Log](
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
 CONSTRAINT [PK_Gat_NikeApi_Log] PRIMARY KEY CLUSTERED
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Log_BillType_IsSuccess_Date' AND object_id = OBJECT_ID(N'[dbo].[Gat_NikeApi_Log]'))
BEGIN
	CREATE NONCLUSTERED INDEX IX_Log_BillType_IsSuccess_Date
	ON [dbo].[Gat_NikeApi_Log] ([BillTypeID], [IsSuccess], [BillDate])
	INCLUDE ([BillNo], [SolutionId], [CompanyID], [ShopCode], [IsImport], [Qty], [Amount], [ModifyDtm]);
END

IF NOT EXISTS (SELECT 1 FROM sysObjects WHERE Id = OBJECT_ID(N'Gat_NikeApi_PosSale') AND xtype = 'U')
BEGIN
CREATE TABLE [dbo].[Gat_NikeApi_PosSale](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SolutionId] [int] NOT NULL,
	[CompanyID] [varchar](20) NOT NULL,
	[BillNo] [varchar](40) NOT NULL, --ORDER_NBR	交易/订单号	String	是	交易唯一键
	[CardID] [varchar](20) NULL, --品牌ID
	[ShopID] [varchar](50) NULL,
	[ShopCode] [varchar](50) NULL,
	[STORE_CD] [varchar](50) NULL, --STORE_CD	门店编码	String	是	订单发生地点的客户门店主数据编码
	[BusinessType] [Nvarchar](50) NULL, --BUSINESS_TYPE	业务类型	String	是	订单行级别业务分类
	[BillDate] [datetime] NULL, --TRANSACTION_DT	交易时间	Timestamp	是	线下：订单在收银台完成交易的时间戳.线上： 发货时间
	[OrderType] [Nvarchar](50) NULL, --ORDER_TYPE	订单类型	String	选填	枚举值：销售、退货、换货
	[UPC] NVARCHAR(40) NULL, --UPC	条形码	String	是	条形码，商品唯一识别码
	[MaterialID] VARCHAR(40) NULL, --货号ID
	[PRODUCT_CD] NVARCHAR(40) NULL, --PRODUCT_CD	货号	String	是	商品编码包含款号和颜色号
	[PRODUCT_NM] NVARCHAR(60) NULL, --PRODUCT_NM	商品名称	String	选填	商品名称
	[SIZE_CD] NVARCHAR(40) NULL, --SIZE_CD	尺码	String	是	商品尺码代码
	[LINE_ITEM_NBR] NVARCHAR(10) NULL, --LINE_ITEM_NBR	行项目序号	String	是	商品的行项目序号
	[NET_SALES_PRC_INCL_TAX] [numeric](24, 6) NULL, --NET_SALES_PRC_INCL_TAX	实际销售单价	Decimal	是	实际销售单价
	[REBATE_AMT] [numeric](24, 6) NULL, --REBATE_AMT	商场补贴金额	Decimal	选填	商场补贴金额
	[NET_SALES_AMT_INCL_TAX] [numeric](24, 6) NULL, --NET_SALES_AMT_INCL_TAX	实际销售金额	Decimal	是	客户实收金额：由（实际销售单价含税金额 × 数量）派生得出的实收含税总金额；若为退货则为负数
	[SALES_QTY] [int] NULL, --SALES_QTY	销售数量	Integer	是	消费者实际购买的商品数量；若为退货则为负数
	[MSRP_LC] [numeric](24, 6) NULL, --MSRP_LC	建议零售单价(吊牌价)	Decimal	选填	建议零售单价(吊牌价)
	[TOTAL_PARTNER_MSRP_AMT] [numeric](24, 6) NULL, --TOTAL_PARTNER_MSRP_AMT	吊牌总金额	Decimal	选填	吊牌总金额
	[TOTAL_DISCOUNT_AMT] [numeric](24, 6) NULL, --TOTAL_DISCOUNT_AMT	折扣总金额	Decimal	选填	本币计的价格调整/折扣总金额
	[ORIGINAL_STORE_CD] [varchar](50) NULL, --ORIGINAL_STORE_CD	退换货原始门店编码	String	选填	退换货匹配原始门店编码
	[ORIGINAL_ORDER_NBR] [varchar](40) NULL, --ORIGINAL_ORDER_NBR	退换货原始交易/订单号	String	选填	退换货匹配原始交易流水号/单据号
	[ORIGINAL_TRANSACTION_DT] [datetime] NULL, --ORIGINAL_TRANSACTION_DT	退换货原始交易日期	Timestamp	选填	退换货匹配原始交易日期YYYY-MM-DD HH:mm:ss
	[ORIGINAL_LINE_ITEM_NBR] NVARCHAR(10) NULL, --ORIGINAL_LINE_ITEM_NBR	退换货原始行项目序号	String	选填	退换货匹配原始商品行项目序号
	[LAST_UPDATED_TMST] DATETIME2(0) NULL, --LAST_UPDATED_TMST	最后更新时间	Timestamp	是	记录最后更新的时间戳YYYY-MM-DD HH:mm:ss
	[IsImport] [bit] NULL,
	[IsSuccess] [bit] NULL,
	[ModifyDtm] [datetime] NULL,
	[PushBillNo] VARCHAR(40) NULL             --推送单号
 CONSTRAINT [PK_Gat_NikeApi_PosSale] PRIMARY KEY CLUSTERED
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PosSale_BillNo' AND object_id = OBJECT_ID(N'[dbo].[Gat_NikeApi_PosSale]'))
BEGIN
	CREATE NONCLUSTERED INDEX IX_PosSale_BillNo
	ON [dbo].[Gat_NikeApi_PosSale] ([BillNo])
	INCLUDE ([LINE_ITEM_NBR], [STORE_CD], [BillDate], [UPC], [SALES_QTY]);
END

IF NOT EXISTS (SELECT 1 FROM sysObjects WHERE Id = OBJECT_ID(N'Gat_NikeApi_Inv') AND xtype = 'U')
BEGIN
CREATE TABLE [dbo].[Gat_NikeApi_Inv](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SolutionId] [int] NOT NULL,
	[CompanyID] [varchar](20) NOT NULL,
	[CardID] [varchar](20) NULL, --品牌ID
	[StockID] VARCHAR(50) NULL,   --仓库代码
    [StockCode] VARCHAR(50) NULL, --仓库代码
	[SITE_CD] [varchar](50) NOT NULL, --SITE_CD	门店/仓库编码	String	是	客户门店或仓库唯一标识编码
	[UPC] NVARCHAR(40) NOT NULL, --UPC	条形码	String	是	条形码，商品唯一识别码
	[MaterialID] VARCHAR(40) NULL, --货号ID
	[PRODUCT_CD] NVARCHAR(40) NULL, --PRODUCT_CD	货号	String	是	商品编码包含款号和颜色号
	[PRODUCT_NM] NVARCHAR(60) NULL, --PRODUCT_NM	商品名称	String	选填	商品名称
	[SIZE_CD] NVARCHAR(40) NULL, --SIZE_CD	尺码	String	是	商品尺码代码
	[INVENTORY_DT] [datetime] NULL, --INVENTORY_DT	库存日期	Date	是	库存快照日期YYYY-MM-DD
	[TOTAL_QTY] [int] NULL, --TOTAL_QTY	总库存	Integer	是	总库存
	[ON_HAND_QTY] [int] NULL, --ON_HAND_QTY	在库库存	Integer	是	在库库存
	[IN_TRANSIT_QTY] [int] NULL, --IN_TRANSIT_QTY	在途库存	Integer	是	仓库/门店未入库的在途库存数量
	[DEFECTIVE_PCS_QTY] [int] NULL, --DEFECTIVE_PCS_QTY	残次品库存	Integer	是	残次品库存
	[MSRP_LC] [numeric](24, 6) NULL, --MSRP_LC	建议零售单价(吊牌价)	Decimal	选填	建议零售单价(吊牌价)
	[LAST_UPDATED_TMST] DATETIME2(0) NOT NULL, --LAST_UPDATED_TMST	最后更新时间	Timestamp	是	记录最后更新的时间戳YYYY-MM-DD HH:mm:ss
	[IsImport] [bit] NULL,
	[IsSuccess] [bit] NULL,
	[ModifyDtm] [datetime] NULL,
 CONSTRAINT [PK_Gat_NikeApi_Inv] PRIMARY KEY CLUSTERED
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Gat_NikeApi_Inv' AND object_id = OBJECT_ID(N'[dbo].[Gat_NikeApi_Inv]'))
BEGIN
	CREATE NONCLUSTERED INDEX IX_Gat_NikeApi_Inv
	ON [dbo].[Gat_NikeApi_Inv] ([SolutionId], [SITE_CD], [UPC])
	INCLUDE ([INVENTORY_DT], [TOTAL_QTY], [ON_HAND_QTY], [IN_TRANSIT_QTY], [DEFECTIVE_PCS_QTY], [MSRP_LC], [LAST_UPDATED_TMST]);
END

IF NOT EXISTS (SELECT 1 FROM sysObjects WHERE Id = OBJECT_ID(N'Gat_NikeApi_OutIn') AND xtype = 'U')
BEGIN
CREATE TABLE [dbo].[Gat_NikeApi_OutIn](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SolutionId] [int] NOT NULL,
	[CompanyID] [varchar](20) NOT NULL,
	[BillNo] [varchar](40) NOT NULL, --单号
	[BillDate] [datetime] NOT NULL, --单据日期
	[CardID] [varchar](20) NULL, --品牌ID
	[SITE_CD] [varchar](50) NOT NULL, --SITE_CD	门店/仓库编码	String	是	客户门店或仓库唯一标识编码
	[SITE_NM] NVARCHAR(50) NULL, --SITE_NM	门店/仓库名称	String	是	位置名称
	[UPC] NVARCHAR(40) NOT NULL, --UPC	条形码	String	是	条形码，商品唯一识别码
	[MaterialID] VARCHAR(40) NULL, --货号ID
	[PRODUCT_CD] NVARCHAR(40) NULL, --PRODUCT_CD	货号	String	是	商品编码包含款号和颜色号
	[PRODUCT_NM] NVARCHAR(60) NULL, --PRODUCT_NM	商品名称	String	选填	商品名称
	[SIZE_CD] NVARCHAR(40) NULL, --SIZE_CD	尺码	String	是	商品尺码代码
	[DIRECTION_DESC] [varchar](50) NULL, --DIRECTION_DESC	流转方向	String	是	交易方向：出库或入库
	[WMS_ORDER_NO] [varchar](40) NOT NULL, --WMS_ORDER_NO	仓库作业流水单号	String	是	仓库作业流水单号，仅应用于仓库
	[PROOF_OF_DELIVERY_NO] [varchar](40) NULL, --PROOF_OF_DELIVERY_NO	发货单号	String	是	关联耐克发货单和退还耐克发货单， 耐克发货为 装箱单号EPL， 耐克退货为 耐克退货订单号 RSO
	[STATUS_TYPE_DESC] [varchar](40) NULL, --STATUS_TYPE_DESC	状态类型描述	String	是	客户出入库类型，枚举值：耐克发货、退还耐克、加盟发货、加盟退货
	[TRANSACTION_TMST] DATETIME2(0) NOT NULL, --TRANSACTION_TMST	流水作业时间戳	Timestamp	是	流水作业时间戳包含日期和具体时间YYYY-MM-DD HH:mm:ss
	[TRANSACTION_QTY] [int] NULL, --TRANSACTION_QTY	流水处理件数	Integer	是	流水处理件数
	[SOURCE_FROM_SITE_CD] [varchar](50) NULL, --SOURCE_FROM_SITE_CD	源位置编码	String	选填	入库产品源头仓库位置唯一标识
	[SOURCE_FROM_SITE_NM] NVARCHAR(50) NULL, --SOURCE_FROM_SITE_NM	源位置名称	String	选填	入库产品源头仓库位置名称
	[DESTINATION_TO_SITE_CD] [varchar](50) NULL, --DESTINATION_TO_SITE_CD	目的位置编码	String	选填	出库产品目的仓库位置唯一标识
	[DESTINATION_TO_SITE_NM] NVARCHAR(50) NULL, --DESTINATION_TO_SITE_NM	目的位置名称	String	选填	出库产品目的仓库位置名称
	[LAST_UPDATED_TMST] DATETIME2(0)  NULL, --LAST_UPDATED_TMST	最后更新时间	Timestamp	是	记录最后更新的时间戳YYYY-MM-DD HH:mm:ss
	[IsImport] [bit] NULL,
	[IsSuccess] [bit] NULL,
	[ModifyDtm] [datetime] NULL,
	[PushBillNo] VARCHAR(40) NULL,             --推送单号
	[BillTypeName] NVARCHAR(40) NOT NULL, --单据类型名称
    [BillTypeID] VARCHAR(20) NULL,       
 CONSTRAINT [PK_Gat_NikeApi_OutIn] PRIMARY KEY CLUSTERED
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Gat_NikeApi_OutIn' AND object_id = OBJECT_ID(N'[dbo].[Gat_NikeApi_OutIn]'))
BEGIN
	CREATE NONCLUSTERED INDEX IX_Gat_NikeApi_OutIn
	ON [dbo].[Gat_NikeApi_OutIn] ([SolutionId], [WMS_ORDER_NO])
	INCLUDE ([SITE_CD], [UPC], [BillDate], [TRANSACTION_TMST], [TRANSACTION_QTY]);
END

IF NOT Exists(select top 1 * from sysObjects where Id=OBJECT_ID(N'Gat_Burgeon_PosSale') and xtype='U')
BEGIN
CREATE TABLE [dbo].[Gat_Burgeon_PosSale](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SolutionId] [int] NOT NULL,
	[CompanyID] [varchar](20) NOT NULL,
	[BillDate]  [datetime] NOT NULL,--订单时间
	[BillNo] [varchar](40) NOT NULL,--线上订单号
	[ExternalBillNo] [varchar](40)  NULL,--外部单号
	[Salesrep] VARCHAR(40)   NULL,--营业员
	[Rate] VARCHAR(10)   NULL,--销售比例
	[CardID] [varchar](20) NULL,--品牌ID
	[ShopID] [varchar](50) NULL,
	[ShopCode] [varchar](50) NULL,--店铺代码
	[OrigCode] [varchar](50) NULL,--发货店仓
	[DestCode] [varchar](50) NULL,--收货店仓
	[PostJaon] [text] NULL,
	[ReturnJson] [text] NULL,
	[IsSuccess] [bit]  NULL,
	[CreateDtm] [datetime]  NULL,
	[ModifyDtm] [datetime]  NULL,
	[Qty] [int] NULL,--数量
	[Amount] [decimal](24, 6) NULL,--订单总金额（POS单总结算金额）
	[IsImport] [bit] NULL,
	PushBillNo VARCHAR(40)  NULL,--推送单号
 CONSTRAINT [PK_Gat_Burgeon_PosSale] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[SolutionId] ASC,
	[CompanyID] ASC,
	[BillNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
end

--明细表
IF NOT Exists(select top 1 * from sysObjects where Id=OBJECT_ID(N'Gat_Burgeon_PosSale_Detail') and xtype='U')
BEGIN
CREATE TABLE [dbo].[Gat_Burgeon_PosSale_Detail](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SolutionId] [int] NOT NULL,
	[CompanyID] [varchar](20) NOT NULL,
	[BillNo] [varchar](40) NOT NULL,--线上订单号
	[ExternalBillNo] [varchar](40) NOT NULL,--外部单号
	[BillDate]  [datetime] NOT NULL,--订单时间
	[Salesrep] VARCHAR(40)   NULL,--营业员
	[Rate] VARCHAR(10)   NULL,--销售比例
	[CardID] [varchar](20) NULL,--品牌ID
	[ShopID] [varchar](50) NULL,
	[ShopCode] [varchar](50) NULL,--店铺代码
	[MaterialID] VARCHAR(40) NULL,     --货号ID
	[MaterialCode] NVARCHAR(40) NULL,     --货号ID
	[BarCode] NVARCHAR(40) NULL,--条码
	[Qty] [int] NOT NULL,--销售数量
	[BalaPrice] [numeric](24, 6) NOT NULL,--成交单价
	[PayAmount] [numeric](24, 6) NOT NULL,--成交金额
	[Type] [int] NOT NULL,--1销售 2退货 3赠品
	[Sequence] VARCHAR(10)  NOT NULL,
	[IsImport] [bit] NULL,
	[IsSuccess] [bit]  NULL,
 CONSTRAINT [PK_Gat_Burgeon_PosSale_Detail] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[SolutionId] ASC,
	[CompanyID] ASC,
	[BillNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
end

--收款明细
IF NOT Exists(select top 1 * from sysObjects where Id=OBJECT_ID(N'Gat_Burgeon_PosSale_Receipt') and xtype='U')
BEGIN
CREATE TABLE [dbo].[Gat_Burgeon_PosSale_Receipt](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SolutionId] [int] NOT NULL,
	[CompanyID] [varchar](20) NOT NULL,
	[BillNo] [varchar](40) NOT NULL,--线上订单号
	[ExternalBillNo] [varchar](40) NOT NULL,--外部单号
	[Sequence] VARCHAR(10)   NULL,
	[PaymentMethod] [Nvarchar](60)  NULL,--支付方式 cash现金 coupon购物券 weixinpay微信 unipay银行卡 alipay支付宝
	[Amount] [numeric](24, 6) NULL,--支付金额
	[BalanceNumber] [Nvarchar](60) NULL,--券号
 CONSTRAINT [PK_Gat_Burgeon_PosSale_Receipt] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[SolutionId] ASC,
	[CompanyID] ASC,
	[BillNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END












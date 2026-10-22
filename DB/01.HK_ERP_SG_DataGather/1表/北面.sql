IF NOT Exists(select top 1 1 from sysObjects where Id=OBJECT_ID(N'Gat_TheNorthFace_MaterialInv') and xtype='U')
BEGIN
CREATE TABLE [dbo].[Gat_TheNorthFace_MaterialInv](
	[ShopCode] [NVARCHAR](50) NULL,
	[UpdateDate] [DATETIME] NULL,
	[Barcode] [NVARCHAR](50) NULL,
	[MaterialCode] [VARCHAR](40) NULL,
	[MaterialName] [NVARCHAR](200) NULL,
	[SizeName] [VARCHAR](100) NULL,
	[Qty] [NUMERIC](24, 6) NULL,
	[CompanyID] [VARCHAR](20) NULL,
	[CompanyName] [NVARCHAR](50) NULL,
	[CardID] [VARCHAR](20) NULL,
	[CardName] [VARCHAR](50) NULL,
	[ShopName] [VARCHAR](100) NULL,
	[StockName] [VARCHAR](100) NULL,
	[KindID] [VARCHAR](20) NULL,
	[KindName] [NVARCHAR](40) NULL,
	[ItemID] [VARCHAR](20) NULL,
	[ItemName] [NVARCHAR](40) NULL,
	[SexID] [VARCHAR](20) NULL,
	[SexName] [NVARCHAR](40) NULL,
	[YearNo] [VARCHAR](10) NULL,
	[SeasonID] [VARCHAR](20) NULL,
	[SeasonName] [NVARCHAR](40) NULL,
	[QuotePrice] [DECIMAL](24, 6) NULL,
	[IsImport] [BIT]  DEFAULT (0) NULL,
	[SolutionID] [INT] NULL,
) ON [PRIMARY]
end
GO



IF NOT Exists(select top 1 1 from sysObjects where Id=OBJECT_ID(N'Gat_TheNorthFace_Meterial_Transfer') and xtype='U')
BEGIN
CREATE TABLE [dbo].[Gat_TheNorthFace_Meterial_Transfer](
	[CompanyID] [VARCHAR](20) NOT NULL,
	[ShopCode] [NVARCHAR](50) NOT NULL,
	[UpdateDate] [DATETIME] NULL,
	[Barcode] [NVARCHAR](50) NULL,
	[MaterialCode] [VARCHAR](40) NULL,
	[MaterialName] [NVARCHAR](200) NULL,
	[SizeName] [VARCHAR](100) NULL,
	[Qty] [NUMERIC](24, 6) NULL,
	[CompanyName] [NVARCHAR](50) NULL,
	[CardID] [VARCHAR](20) NULL,
	[CardName] [VARCHAR](50) NULL,
	[ShopName] [VARCHAR](100) NULL,
	[StockName] [VARCHAR](100) NULL,
	[SolutionID] [INT] NULL,
) ON [PRIMARY]
end
GO


IF NOT Exists(select top 1 1 from sysObjects where Id=OBJECT_ID(N'Gat_TheNorthFace_PosSale') and xtype='U')
begin
CREATE TABLE [dbo].[Gat_TheNorthFace_PosSale](
    [id] [INT] IDENTITY(1,1) NOT NULL,
	[ShopCode] [NVARCHAR](50) NULL,
	[BillDate] [DATETIME] NOT NULL,
	[BillTime] [DATETIME] NULL,
	[MaterialCode] [NVARCHAR](50) NULL,
	[Qty] [DECIMAL](24, 6) NULL,
	[BillNo] [VARCHAR](40) NULL,
	[SalesAmount] [DECIMAL](24, 6) NULL,
	[PersonnelID] [VARCHAR](20) NULL,
	[BarCode] [NVARCHAR](128) NULL,
	[SizeName] [VARCHAR](50) NULL,
	[CompanyID] [VARCHAR](20) NULL,
	[CompanyName] [NVARCHAR](50) NULL,
	[CardID] [VARCHAR](20) NULL,
	[CardName] [VARCHAR](20) NULL,
	[SolutionID] [INT] DEFAULT (0) NULL,
	[BalaPrice] [DECIMAL](24, 6) DEFAULT (0.00) NULL,
	[ShopName] [VARCHAR](100) NULL,
	[StockName] [VARCHAR](100) NULL,
	[Remark] [NVARCHAR](100) NULL,
	[KindID] [VARCHAR](20) NULL,
	[KindName] [NVARCHAR](40) NULL,
	[ItemID] [VARCHAR](20) NULL,
	[ItemName] [NVARCHAR](40) NULL,
	[SexID] [VARCHAR](20) NULL,
	[SexName] [NVARCHAR](40) NULL,
	[YearNo] [VARCHAR](10) NULL,
	[SeasonName] [NVARCHAR](40) NULL,
	[QuotePrice] [DECIMAL](24, 6) NULL,
	[BillYear] [VARCHAR](10) NULL,
	[BillMonthDay] [VARCHAR](10) NULL,
	[QuotePriceAmount] [DECIMAL](24, 6) NULL,
	[SeasonID] [VARCHAR](20) NULL,
	[IsImport] [BIT]  DEFAULT (0) NULL,
) ON [PRIMARY]
end
GO




IF NOT Exists(select top 1 1 from sysObjects where Id=OBJECT_ID(N'Gat_TheNorthFace_PosSale_transfer') and xtype='U')
begin
CREATE TABLE [dbo].[Gat_TheNorthFace_PosSale_transfer](
	[TransID] [INT] IDENTITY(1,1) NOT NULL,
	[ShopCode] [NVARCHAR](50) NULL,
	[BillDate] [DATETIME] NOT NULL,
	[BillTime] [DATETIME] NULL,
	[MaterialCode] [NVARCHAR](50) NULL,
	[Qty] [DECIMAL](24, 6) NULL,
	[BillNo] [VARCHAR](40) NULL,
	[SalesAmount] [DECIMAL](24, 6) NULL,
	[PersonnelID] [VARCHAR](20) NULL,
	[BarCode] [NVARCHAR](128) NULL,
	[SizeName] [VARCHAR](50) NULL,
	[CompanyID] [VARCHAR](20) NULL,
	[CompanyName] [NVARCHAR](50) NULL,
	[CardID] [VARCHAR](20) NULL,
	[CardName] [VARCHAR](20) NULL,
	[SolutionID] [INT] NULL,
	[BalaPrice] [DECIMAL](24, 6)   DEFAULT (0.00)NULL,
	[ShopName] [VARCHAR](100) NULL,
	[StockName] [VARCHAR](100) NULL,
	[Remark] [NVARCHAR](100) NULL,
 CONSTRAINT [PK_Gat_TheNorthFace_PosSale_transfer] PRIMARY KEY CLUSTERED 
(
	[TransID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
end
GO

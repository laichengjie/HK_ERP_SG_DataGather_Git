IF NOT Exists(select top 1 * from sysObjects where Id=OBJECT_ID(N'Gat_Ad_MeterialINV') and xtype='U')
BEGIN
CREATE TABLE [dbo].[Gat_Ad_MeterialINV](
	[ShopCode] [VARCHAR](208) NOT NULL,
	[UpdateDate] [DATETIME] NULL,
	[BarCode] [VARCHAR](128) NOT NULL,
	[MaterialCode] [VARCHAR](128) NOT NULL,
	[SizeName] [VARCHAR](50) NULL,
	[Qty] [NUMERIC](24, 6) NULL,
	[CompanyID] [VARCHAR](20) NULL,
	[CompanyName] [VARCHAR](20) NULL,
	[CardID] [VARCHAR](20) NULL,
	[CardName] [VARCHAR](20) NULL
) ON [PRIMARY]

END



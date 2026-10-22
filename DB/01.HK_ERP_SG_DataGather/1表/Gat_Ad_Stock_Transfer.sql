IF NOT Exists(select top 1 * from sysObjects where Id=OBJECT_ID(N'Gat_Ad_Stock_Transfer') and xtype='U')
BEGIN
CREATE TABLE [dbo].[Gat_Ad_Stock_Transfer](
	[ShopCode] [VARCHAR](208) NOT NULL,
	[StockCode] [VARCHAR](50) NOT NULL,
	[UpdateDate] [DATETIME] NULL,
	[BarCode] [VARCHAR](128) NOT NULL,
	[MaterialCode] [VARCHAR](128) NOT NULL,
	[SizeCode] [VARCHAR](50) NULL,
	[Qty] [NUMERIC](24, 6) NULL
) ON [PRIMARY]

END




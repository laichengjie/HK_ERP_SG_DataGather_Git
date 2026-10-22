If Exists(Select Top 1 1 From sysobjects Where id = object_id('[Gat_Asics_MeterialINV]'))
  Drop Table [HK_ERP_SG_DataGather].dbo.[Gat_Asics_MeterialINV]
CREATE TABLE [dbo].[Gat_Asics_MeterialINV](
	[ShopCode] [varchar](208) NOT NULL,
	[StockName] [varchar](208) NOT NULL,
	[UpdateDate] [datetime] NULL,
	[BarCode] [varchar](128) NOT NULL,
	[MaterialCode] [varchar](128) NOT NULL,
	[SizeName] [varchar](50) NULL,
	[Qty] [numeric](24, 6) NULL,
	[CompanyID] [varchar](20) NULL,
	[CompanyName] [varchar](20) NULL,
	[CardID] [varchar](20) NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO



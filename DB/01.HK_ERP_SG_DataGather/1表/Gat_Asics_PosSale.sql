If Exists(Select Top 1 1 From sysobjects Where id = object_id('[Gat_Asics_PosSale]'))
  Drop Table [HK_ERP_SG_DataGather].dbo.[Gat_Asics_PosSale]

CREATE TABLE [dbo].[Gat_Asics_PosSale](
	[BillCode] [varchar](128) NULL,
	[BillDate] [datetime] NULL,
	[StoreName] [nvarchar](128) NULL,
	[StoreCode] [varchar](128) NULL,
	[ProdCode] [varchar](128) NULL,
	[ProdName] [nvarchar](256) NULL,
	[ProdUnit] [varchar](128) NULL,
	[ProdQuantity] [decimal](24, 6) NULL,
	[ProdAmount] [decimal](24, 6) NULL,
	[CreateDTM] [datetime] NULL,
	[StockCode] [nvarchar](256) NULL,
	[Remark] [nvarchar](max) NULL,
	[ModifyDTM] [datetime] NULL,
	[CompanyID] [varchar](20) NULL,
	[CompanyName] [nvarchar](50) NULL,
	[CardID] [varchar](20) NULL,
	[ProdBarCode] [varchar](256) NULL,
	[ProdSpec] [varchar](256) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO



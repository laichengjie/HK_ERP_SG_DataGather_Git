CREATE TABLE [dbo].[Gat_Lee_MaterialInv](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[SolutionID] [int] NOT NULL,
	[CardID] [varchar](20) NULL,
	[CardName] [varchar](20) NULL,
	[CompanyID] [varchar](20) NOT NULL,
	[CompanyName] [nvarchar](50) NULL,
	[ShopID] [nvarchar](50) NOT NULL,
	[ShopCode] [varchar](50) NOT NULL,
	[ShopName] [nvarchar](100) NULL,
	[MaterialCode] [nvarchar](60) NOT NULL,
	[Barcode] [nvarchar](100) NOT NULL,
	[ActualQty] [numeric](24, 6) NULL,
	[PathQty] [numeric](24, 6) NULL,
	[IsImport] [bit] NULL,
	[ModifyDtm] [datetime] NULL,
	[SizeName] [nvarchar](50) NULL,
 CONSTRAINT [PK_Gat_Lee_MaterialInv] PRIMARY KEY CLUSTERED 
(
	[CompanyID] ASC,
	[ShopCode] ASC,
	[MaterialCode] ASC,
	[Barcode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
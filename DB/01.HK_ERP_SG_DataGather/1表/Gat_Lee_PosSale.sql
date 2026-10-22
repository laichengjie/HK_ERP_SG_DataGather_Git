
CREATE TABLE [dbo].[Gat_Lee_PosSale](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[SolutionID] [int] NOT NULL,
	[CardID] [varchar](20) NULL,
	[CardName] [varchar](20) NULL,
	[CompanyID] [varchar](20) NOT NULL,
	[CompanyName] [nvarchar](50) NULL,
	[ShopID] [nvarchar](50) NOT NULL,
	[ShopCode] [varchar](50) NOT NULL,
	[ShopName] [nvarchar](100) NULL,
	[BillNo] [varchar](40) NOT NULL,
	[SourceBillNo] [varchar](40) NOT NULL,
	[BillDate] [datetime] NOT NULL,
	[BillTime] [datetime] NULL,
	[MaterialCode] [nvarchar](60) NOT NULL,
	[BarCode] [varchar](100) NOT NULL,
	[Qty] [decimal](24, 6) NULL,
	[RetailPrice] [decimal](24, 6) NULL,
	[PayAmount] [decimal](24, 6) NULL,
	[IsImport] [bit] NULL,
	[ModifyDtm] [datetime] NULL,
	[SizeName] [nvarchar](50) NULL,
 CONSTRAINT [PK_Gat_Lee_PosSale] PRIMARY KEY CLUSTERED 
(
	[id] ASC,
	[SolutionID] ASC,
	[CompanyID] ASC,
	[BillNo] ASC,
	[ShopCode] ASC,
	[MaterialCode] ASC,
	[BarCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



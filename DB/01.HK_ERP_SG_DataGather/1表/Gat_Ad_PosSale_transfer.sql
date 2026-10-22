IF NOT Exists(select top 1 * from sysObjects where Id=OBJECT_ID(N'Gat_Ad_PosSale_transfer') and xtype='U')
BEGIN
CREATE TABLE [dbo].[Gat_Ad_PosSale_transfer](
	[TransID] [int] IDENTITY(1,1) NOT NULL,
	[ShopCode] [nvarchar](50) NULL,
	[BillDate] [datetime] NOT NULL,
	[BillTime] [datetime] NULL,
	[MaterialCode] [nvarchar](50) NULL,
	[Qty] [decimal](24, 6) NULL,
	[BillNo] [varchar](40) NULL,
	[SalesAmount] [decimal](24, 6) NULL,
	[PersonnelID] [varchar](20) NULL,
	[BarCode] [nvarchar](128) NULL,
	[SizeName] [varchar](50) NULL,
	[CompanyID] [varchar](20) NULL,
	[CompanyName] [nvarchar](50) NULL,
	[CardID] [varchar](20) NULL,
	[CardName] [varchar](20) NULL,
	[SolutionID] [int] NULL,
	[BalaPrice] [decimal](24, 6) NULL,
	[ShopName] [varchar](100) NULL,
	[StockName] [varchar](100) NULL,
	[SmallTicketNo] [nvarchar](40) NULL,
 CONSTRAINT [PK_Gat_Ad_PosSale_transfer] PRIMARY KEY CLUSTERED 
(
	[TransID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END

IF NOT EXISTS(select   *   from   syscolumns   where   id=object_id('Gat_Ad_PosSale_transfer')   and   name='SmallTicketNo' )
BEGIN
    ALTER TABLE Gat_Ad_PosSale_transfer ADD SmallTicketNo NVARCHAR(40) NULL
END

IF NOT EXISTS(select   *   from   syscolumns   where   id=object_id('Gat_Ad_PosSale_transfer')   and   name='Remark' )
BEGIN
    ALTER TABLE Gat_Ad_PosSale_transfer ADD Remark NVARCHAR(500) NULL
END



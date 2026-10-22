IF NOT Exists(select top 1 * from sysObjects where Id=OBJECT_ID(N'Gat_Columbia_PosSale_transfer') and xtype='U')
BEGIN
CREATE TABLE [dbo].[Gat_Columbia_PosSale_transfer](
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
	[BalaPrice] [DECIMAL](24, 6) NULL,
	[ShopName] [VARCHAR](100) NULL,
	[StockName] [VARCHAR](100) NULL,
 CONSTRAINT [PK_Gat_Columbia_PosSale_transfer] PRIMARY KEY CLUSTERED 
(
	[TransID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [dbo].[Gat_Columbia_PosSale_transfer] ADD  DEFAULT ((0.00)) FOR [BalaPrice]

END

IF NOT EXISTS(select   *   from   syscolumns   where   id=object_id('Gat_Columbia_PosSale_transfer')   and   name='Remark' )
BEGIN
    ALTER TABLE Gat_Columbia_PosSale_transfer ADD Remark nvarchar(100) NULL
END

IF NOT EXISTS(select   *   from   syscolumns   where   id=object_id('Gat_Columbia_PosSale_transfer')   and   name='ShopName' )
BEGIN
    ALTER TABLE Gat_Columbia_PosSale_transfer ADD [ShopName] [VARCHAR](100) NULL
END

IF NOT EXISTS(select   *   from   syscolumns   where   id=object_id('Gat_Columbia_PosSale_transfer')   and   name='StockName' )
BEGIN
    ALTER TABLE Gat_Columbia_PosSale_transfer ADD StockName [VARCHAR](100) NULL
END



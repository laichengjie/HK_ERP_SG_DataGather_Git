IF NOT Exists(select top 1 * from sysObjects where Id=OBJECT_ID(N'Gat_Columbia_PosSale') and xtype='U')
BEGIN
	CREATE TABLE [dbo].[Gat_Columbia_PosSale](
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
		[StockName] [varchar](100) NULL
	) ON [PRIMARY]
	
ALTER TABLE [dbo].[Gat_Columbia_PosSale] ADD  DEFAULT ((0)) FOR [SolutionID]


ALTER TABLE [dbo].[Gat_Columbia_PosSale] ADD  DEFAULT ((0.00)) FOR [BalaPrice]

END


IF NOT EXISTS(select   *   from   syscolumns   where   id=object_id('Gat_Columbia_PosSale')   and   name='Remark' )
BEGIN
    ALTER TABLE Gat_Columbia_PosSale ADD Remark nvarchar(100) NULL
END



IF NOT Exists(select top 1 * from sysObjects where Id=OBJECT_ID(N'Gat_Ad_PosSale') and xtype='U')
BEGIN
	CREATE TABLE [dbo].[Gat_Ad_PosSale](
		[ShopCode] [VARCHAR](20) NOT NULL,
		[BillDate] [DATETIME] NOT NULL,
		[BillTime] [DATETIME] NOT NULL,
		[MaterialCode] [VARCHAR](20) NULL,
		[Qty] [NUMERIC](10, 2) NOT NULL,
		[BillNo] [VARCHAR](40) NOT NULL,
		[SaleAmount] [DECIMAL](24, 6) NULL,
		[PersonelName] [VARCHAR](50) NULL,
		[BarCode] [VARCHAR](128) NULL,
		[SizeName] [VARCHAR](50) NULL,
		[CompanyID] [VARCHAR](20) NULL,
		[CompanyName] [VARCHAR](20) NULL,
		[CardID] [VARCHAR](20) NULL,
		[CardName] [VARCHAR](20) NULL,
		[SolutionID] [INT] NULL,
		[BalaPrice] [NUMERIC](24, 6) NULL,
		[ShopName] [VARCHAR](100) NULL,
		[StockName] [VARCHAR](100) NULL,
		[SmallTicketNo] [NVARCHAR](40) NULL,
		[PersonnelID] [NVARCHAR](20) NULL,
		[id] [INT] IDENTITY(1,1) NOT NULL,
	PRIMARY KEY CLUSTERED 
	(
		[id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END

IF NOT EXISTS(select   *   from   syscolumns   where   id=object_id('Gat_Ad_PosSale')   and   name='id' )
BEGIN
    ALTER TABLE [Gat_Ad_PosSale] ADD id INT IDENTITY(1,1) NOT NULL
END

IF NOT EXISTS(select   *   from   syscolumns   where   id=object_id('Gat_Ad_PosSale')   and   name='Remark' )
BEGIN
    ALTER TABLE [Gat_Ad_PosSale] ADD Remark NVARCHAR(500) NULL
END

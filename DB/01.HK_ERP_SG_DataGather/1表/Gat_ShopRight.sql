IF NOT Exists(select top 1 * from sysObjects where Id=OBJECT_ID(N'Gat_ShopRight') and xtype='U')
BEGIN
CREATE TABLE [dbo].[Gat_ShopRight](
	[ID] [INT] IDENTITY(1,1) NOT NULL,
	[PastVersionVendorID] [NVARCHAR](20) NOT NULL,
	[CompanyID] [NVARCHAR](20) NOT NULL,
	[ShopID] [NVARCHAR](50) NOT NULL,
	[CardID] [NVARCHAR](20) NOT NULL,
	[AllowUsed] [BIT] NOT NULL,
	[ModifyDTM] [DATETIME] NOT NULL,
	[VendorID] [INT] NULL,
	[ManualGatStartTime] [DATETIME] NULL,
	[ManualGatEndTime] [DATETIME] NULL,
	[DefaultDiscount] [BIT] NOT NULL,
	[Discount] [NUMERIC](14, 2) NOT NULL,
	[TradeCode] [NVARCHAR](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

END

IF NOT EXISTS(select   *   from   syscolumns   where   id=object_id('Gat_ShopRight')   and   name='DefaultDiscount' )
BEGIN
    ALTER TABLE Gat_ShopRight ADD DefaultDiscount bit DEFAULT 1 NOT NULL
END

IF NOT EXISTS(select   *   from   syscolumns   where   id=object_id('Gat_ShopRight')   and   name='Discount' )
BEGIN
    ALTER TABLE Gat_ShopRight ADD Discount [NUMERIC](14, 2) DEFAULT 1 NOT NULL
END

IF NOT EXISTS(select   *   from   syscolumns   where   id=object_id('Gat_ShopRight')   and   name='TradeCode' )
BEGIN
    ALTER TABLE Gat_ShopRight ADD TradeCode [NVARCHAR](50) NULL
END


IF NOT EXISTS(select   *   from   syscolumns   where   id=object_id('Gat_ShopRight')   and   name='TradeCode2' )
BEGIN
    ALTER TABLE Gat_ShopRight ADD TradeCode2 [NVARCHAR](50) NULL
END



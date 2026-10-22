
IF NOT Exists(select top 1 * from sysObjects where Id=OBJECT_ID(N'Gat_VendCustRight') and xtype='U')
BEGIN
CREATE TABLE [dbo].[Gat_VendCustRight](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SolutionID] [int] NOT NULL,
	[CompanyID] [nvarchar](20) NOT NULL,
	[VendCustID] [nvarchar](50) NOT NULL,
	[VendCustCode] [nvarchar](50) NOT NULL,
	[VendCustName] [nvarchar](100) NOT NULL,
	[AllowUsed] [bit] NOT NULL,
	[ModifyDTM] [datetime] NOT NULL,
	[TradeCode] [nvarchar](50) NULL,
	[TradeCode2] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
end
GO



IF COL_LENGTH('Gat_Solution', 'CronExpression') IS NULL
    ALTER TABLE Gat_Solution ADD  CronExpression VARCHAR(20)  NULL;
GO

IF COL_LENGTH('Gat_Solution', 'IsAuto') IS NULL
BEGIN
    ALTER TABLE Gat_Solution ADD IsAuto BIT NULL;
	ALTER TABLE [dbo].[Gat_Solution] ADD  DEFAULT ((1)) FOR [IsAuto]
end
GO

UPDATE  Gat_Solution SET IsBackground =1 WHERE  IsAuto IS  null

IF COL_LENGTH('Gat_Solution', 'IsBackground') IS NULL
BEGIN
    ALTER TABLE Gat_Solution ADD IsBackground BIT NULL;
	ALTER TABLE [dbo].[Gat_Solution] ADD  DEFAULT ((0)) FOR [IsBackground]
end
GO

UPDATE  Gat_Solution SET IsBackground =0 WHERE  IsBackground IS  null



IF COL_LENGTH('Gat_Solution', 'ExpiryDate') IS NULL
    ALTER TABLE Gat_Solution ADD  ExpiryDate VARCHAR(30)  NULL;
GO

IF COL_LENGTH('Gat_Solution', 'AppKey') IS NULL
    ALTER TABLE Gat_Solution ADD  AppKey VARCHAR(100)  NULL;
GO


IF NOT Exists(select top 1 * from sysObjects where Id=OBJECT_ID(N'Gat_VendCustRight') and xtype='U')
BEGIN
CREATE TABLE [dbo].[Gat_VendCustRight](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SolutionID] [int] NOT NULL,
	[CompanyID] [nvarchar](20) NOT NULL,
	[VendCustID] [nvarchar](50) NOT NULL,
	[VendCustCode] [nvarchar](50) NOT NULL,
	[VendCustName] [nvarchar](100) NOT NULL,
	[AllowUsed] [bit] NOT NULL,
	[ModifyDTM] [datetime] NOT NULL,
	[TradeCode] [nvarchar](50) NULL,
	[TradeCode2] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
end
GO

IF NOT Exists(select top 1 * from sysObjects where Id=OBJECT_ID(N'Gat_MaterialRange') and xtype='U')
BEGIN
CREATE TABLE [dbo].[Gat_MaterialRange](
	[Id] [INT] IDENTITY(1,1) NOT NULL,
	[SolutionId] [INT] NOT NULL,
	[MaterialCode] [VARCHAR](128) NOT NULL,
	[MaterialName] [NVARCHAR](200) NULL,
	[CreateTime] [DATETIME] NOT NULL,
	[ModifyTime] [DATETIME] NOT NULL
) ON [PRIMARY]
end
GO

IF NOT Exists(select top 1 * from sysObjects where Id=OBJECT_ID(N'Gat_SpecialMaterialRange') and xtype='U')
BEGIN
CREATE TABLE [dbo].[Gat_SpecialMaterialRange](
	[Id] [INT] IDENTITY(1,1) NOT NULL,
	[SolutionId] [INT] NOT NULL,
	[MaterialCode] [VARCHAR](128) NOT NULL,
	[MaterialName] [NVARCHAR](200) NULL,
	[ShopCode] [VARCHAR](128) NOT NULL,
	[ShopName] [NVARCHAR](128) NULL,
	[CreateTime] [DATETIME] NOT NULL,
	[ModifyTime] [DATETIME] NOT NULL
) ON [PRIMARY]
end
GO





IF COL_LENGTH('Gat_ShopRight', 'DefaultDiscount') IS NULL
BEGIN
    ALTER TABLE Gat_ShopRight ADD DefaultDiscount BIT NULL;
	ALTER TABLE [dbo].[Gat_ShopRight] ADD  DEFAULT ((1)) FOR [DefaultDiscount]
end
GO

UPDATE Gat_ShopRight SET DefaultDiscount=1 WHERE DefaultDiscount IS NULL 


IF COL_LENGTH('Gat_ShopRight', 'Discount') IS NULL
BEGIN
    ALTER TABLE Gat_ShopRight ADD Discount [NUMERIC](14, 2) NULL;
	ALTER TABLE [dbo].[Gat_ShopRight] ADD  DEFAULT ((1)) FOR [Discount]
end
GO

UPDATE Gat_ShopRight SET Discount=1 WHERE Discount IS NULL 



IF COL_LENGTH('Gat_ShopRight', 'TradeCode') IS NULL
BEGIN
    ALTER TABLE Gat_ShopRight ADD TradeCode NVARCHAR(50) NULL;
	
end
GO

IF COL_LENGTH('Gat_ShopRight', 'TradeCode2') IS NULL
BEGIN
    ALTER TABLE Gat_ShopRight ADD TradeCode2 NVARCHAR(50) NULL;
	
end
GO

IF COL_LENGTH('Gat_ShopRight', 'TradeCode2') IS NULL
BEGIN
    ALTER TABLE Gat_ShopRight ADD TradeCode2 NVARCHAR(50) NULL;
	
end
GO

IF COL_LENGTH('Gat_StockRight', 'TradeCode') IS NULL
BEGIN
    ALTER TABLE Gat_StockRight ADD TradeCode NVARCHAR(50) NULL;
	
end
GO

--添加字段
IF COL_LENGTH('Bas_Vendor', 'ShopNumber') IS NULL
    ALTER TABLE Bas_Vendor ADD ShopNumber INT  DEFAULT 0;
GO

--是否导入
IF COL_LENGTH('Gat_JackWolfSkin_MeterialINV', 'IsImport') IS NULL
    ALTER TABLE Gat_JackWolfSkin_MeterialINV ADD IsImport bit  DEFAULT 0;
GO

update  Gat_JackWolfSkin_MeterialINV set IsImport=0 where isnull(IsImport,0)=0

--是否导入
IF COL_LENGTH('Gat_JackWolfSkin_PosSale', 'IsImport') IS NULL
    ALTER TABLE Gat_JackWolfSkin_PosSale ADD IsImport bit  DEFAULT 0;
GO

update  Gat_JackWolfSkin_PosSale set IsImport=0 where isnull(IsImport,0)=0


IF NOT EXISTS(select   *   from   syscolumns   where   id=object_id('Gat_Columbia_PosSale')   and   name='id' )
BEGIN
    ALTER TABLE Gat_Columbia_PosSale ADD id INT IDENTITY(1,1) NOT NULL
END

IF COL_LENGTH('Gat_Skechers_PosSale','ReturnJson') IS Not NULL
BEGIN
	ALTER TABLE Gat_Skechers_PosSale ALTER COLUMN ReturnJson nvarchar(2000)  NULL
END
GO


--是否不计算电商销售
IF COL_LENGTH('Gat_ShopRight', 'IsNoCalculationSale') IS NULL
BEGIN
    ALTER TABLE Gat_ShopRight ADD IsNoCalculationSale BIT DEFAULT(0)  NULL;
end
GO

UPDATE  Gat_ShopRight SET IsNoCalculationSale=0 WHERE IsNoCalculationSale IS NULL


 --销售金额取数 0 结算金额  1 ActualAmount实际结算金额
IF COL_LENGTH('Gat_Solution', 'SalesAmountType') IS NULL
    ALTER TABLE Gat_Solution ADD  SalesAmountType int  NULL;
GO

update Gat_Solution set  SalesAmountType=0 where ISNULL(SalesAmountType,0)=0

IF COL_LENGTH('Gat_Columbia_PosSale', 'id') IS NULL
BEGIN
    ALTER TABLE Gat_Columbia_PosSale ADD id int IDENTITY(1,1) ;
end
GO


IF COL_LENGTH('Gat_Columbia_MaterialInv', 'KindID') IS NULL
    ALTER TABLE Gat_Columbia_MaterialInv ADD KindID [VARCHAR](20) NULL;
GO

IF COL_LENGTH('Gat_Columbia_MaterialInv', 'KindName') IS NULL
    ALTER TABLE Gat_Columbia_MaterialInv ADD KindName [NVARCHAR](40) NULL;
GO

IF COL_LENGTH('Gat_Columbia_MaterialInv', 'ItemID') IS NULL
    ALTER TABLE Gat_Columbia_MaterialInv ADD ItemID [VARCHAR](20) NULL;
GO

IF COL_LENGTH('Gat_Columbia_MaterialInv', 'ItemName') IS NULL
    ALTER TABLE Gat_Columbia_MaterialInv ADD ItemName [NVARCHAR](40) NULL;
GO

IF COL_LENGTH('Gat_Columbia_MaterialInv', 'SexID') IS NULL
    ALTER TABLE Gat_Columbia_MaterialInv ADD SexID [VARCHAR](20) NULL;
GO

IF COL_LENGTH('Gat_Columbia_MaterialInv', 'SexName') IS NULL
    ALTER TABLE Gat_Columbia_MaterialInv ADD SexName [NVARCHAR](40) NULL;
GO

IF COL_LENGTH('Gat_Columbia_MaterialInv', 'YearNo') IS NULL
    ALTER TABLE Gat_Columbia_MaterialInv ADD YearNo [VARCHAR](10) NULL;
GO
IF COL_LENGTH('Gat_Columbia_MaterialInv', 'SeasonID') IS NULL
    ALTER TABLE Gat_Columbia_MaterialInv ADD SeasonID [VARCHAR](20) NULL;
GO

IF COL_LENGTH('Gat_Columbia_MaterialInv', 'SeasonName') IS NULL
    ALTER TABLE Gat_Columbia_MaterialInv ADD SeasonName [NVARCHAR](40) NULL;
GO

IF COL_LENGTH('Gat_Columbia_MaterialInv', 'QuotePrice') IS NULL
    ALTER TABLE Gat_Columbia_MaterialInv ADD QuotePrice [DECIMAL](24, 6) NULL;
GO

IF COL_LENGTH('Gat_Columbia_MaterialInv', 'QuotePrice') IS NULL
    ALTER TABLE Gat_Columbia_MaterialInv ADD QuotePrice [DECIMAL](24, 6) NULL;
GO
	

--是否导入
IF COL_LENGTH('Gat_Columbia_MaterialInv', 'IsImport') IS NULL
    ALTER TABLE Gat_Columbia_MaterialInv ADD IsImport bit  DEFAULT 0;
GO

update  Gat_Columbia_MaterialInv set IsImport=0 where isnull(IsImport,0)=0


IF NOT Exists(select top 1 * from sysObjects where Id=OBJECT_ID(N'Sys_SGLog') and xtype='U')
BEGIN
CREATE TABLE [dbo].[Sys_SGLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SolutionId] [int] NULL,
	[CompanyID] [varchar](20) NULL,
	[CardName] [nvarchar](2000) NULL,
	[Remark] [nvarchar](2000) NULL,
	[Message] [nvarchar](max) NULL,
	[ModifyDtm] [datetime] NULL,
 CONSTRAINT [PK_Sys_SGLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
end
GO


IF COL_LENGTH('Gat_Solution','Directory') IS Not NULL
BEGIN
	ALTER TABLE Gat_Solution ALTER COLUMN Directory nvarchar(500)  NULL
END
GO





--建表
IF not exists (select 1 from  sysobjects where  id = object_id('Gat_MaterialDiscount') and type = 'U')
Begin
	CREATE TABLE [dbo].[Gat_MaterialDiscount](
	    [Id] [int] IDENTITY(1,1) NOT NULL,
	    [SolutionId] [int] NOT NULL,
	    [MaterialCode] [varchar](128) NOT NULL,
	    [MaterialName] [nvarchar](200) NULL,
		[Discount] NUMERIC(24,6) NOT NULL,
	    [CreateTime] [datetime] NOT NULL,
	    [ModifyTime] [datetime] NOT NULL
	 CONSTRAINT [PK_Gat_MaterialDiscount] PRIMARY KEY CLUSTERED 
	(
	    [ID] ASC,
		[SolutionId] ASC,
		[MaterialCode] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
End
GO


--建表
IF not exists (select 1 from  sysobjects where  id = object_id('Gat_ShopRightDS') and type = 'U')
Begin
	CREATE TABLE [dbo].[Gat_ShopRightDS](
	    [ID] [int] IDENTITY(1,1) NOT NULL,
		[ShopRightID] [int] NOT NULL,
		[CompanyID] [nvarchar](20) NOT NULL,
	    [ShopID] [nvarchar](50) NOT NULL,
		[DSShopID] [nvarchar](50) NOT NULL,
	 CONSTRAINT [PK_Gat_ShopRightDS] PRIMARY KEY CLUSTERED 
	(
	    [ID] ASC,
		[CompanyID] ASC,
		[ShopID] ASC,
		[DSShopID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
End
GO


IF COL_LENGTH('Gat_Ua_PosSale','IsImport') IS  NULL
BEGIN
	 ALTER TABLE Gat_Ua_PosSale ADD IsImport BIT NOT NULL
	  CONSTRAINT DF_Gat_Ua_PosSale_IsImport DEFAULT 0;
END
GO

--是否正价电商销售
IF COL_LENGTH('Gat_ShopRight', 'IsECSale') IS NULL
BEGIN
    ALTER TABLE Gat_ShopRight ADD IsECSale BIT DEFAULT(0)  NULL
end
GO
UPDATE  Gat_ShopRight SET IsECSale=0 WHERE IsECSale IS NULL


--库存来源
IF COL_LENGTH('Gat_Ad_MeterialINV_OC', 'InventorySource') IS NULL
    ALTER TABLE Gat_Ad_MeterialINV_OC ADD InventorySource VARCHAR(20) NULL;
GO

IF COL_LENGTH('Gat_Ad_MeterialINV_OCTB', 'InventorySource') IS NULL
    ALTER TABLE Gat_Ad_MeterialINV_OCTB ADD InventorySource VARCHAR(20) NULL;
GO

IF COL_LENGTH('Gat_Ad_Stock_Transfer_OC', 'InventorySource') IS NULL
    ALTER TABLE Gat_Ad_Stock_Transfer_OC ADD InventorySource VARCHAR(20) NULL;
GO

IF COL_LENGTH('Gat_Ad_MeterialINV_OCTB','CompanyName') IS Not NULL
BEGIN
	ALTER TABLE Gat_Ad_MeterialINV_OCTB ALTER COLUMN CompanyName nvarchar(50)  NULL
END
GO

IF COL_LENGTH('Gat_Ad_MeterialINV_OC','CompanyName') IS Not NULL
BEGIN
	ALTER TABLE Gat_Ad_MeterialINV_OC ALTER COLUMN CompanyName nvarchar(50)  NULL
END
GO


--PCode
IF COL_LENGTH('Gat_Ad_MeterialINV_OC', 'PCode') IS NULL
    ALTER TABLE Gat_Ad_MeterialINV_OC ADD PCode VARCHAR(40) NULL;
GO

IF COL_LENGTH('Gat_Ad_MeterialINV_OCTB', 'PCode') IS NULL
    ALTER TABLE Gat_Ad_MeterialINV_OCTB ADD PCode VARCHAR(40) NULL;
GO

IF COL_LENGTH('Gat_Ad_Stock_Transfer_OC', 'PCode') IS NULL
    ALTER TABLE Gat_Ad_Stock_Transfer_OC ADD PCode VARCHAR(40) NULL;
GO


--仓库子款型 设置
IF not exists (select 1 from  sysobjects where  id = object_id('Gat_StockSubModelSet') and type = 'U')
Begin
	CREATE TABLE [dbo].[Gat_StockSubModelSet](
	    [Id] [int] IDENTITY(1,1) NOT NULL,
	    [SolutionId] [int] NOT NULL,
	    [StockCode] [varchar](40) NOT NULL,
	    [SubModel] [nvarchar](60) NOT NULL,
		[PCode] [varchar](40) NOT NULL,
	    [CreateTime] [datetime] NOT NULL,
	    [ModifyTime] [datetime] NOT NULL
	 CONSTRAINT [PK_Gat_StockSubModelSet] PRIMARY KEY CLUSTERED 
	(
	    [ID] ASC,
		[SolutionId] ASC,
		[StockCode] ASC,
		[SubModel] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
End
GO


--网店归属线下店铺POS设置 
IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE id = object_id('Gat_OnlineShopSet') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[Gat_OnlineShopSet](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [SolutionId] [int] NOT NULL,
        [OnlineShopCode] [varchar](40) NOT NULL,
        [OfflineShopCode] [nvarchar](60) NOT NULL,
        [CreateTime] [datetime] NOT NULL,
        [ModifyTime] [datetime] NOT NULL,
        CONSTRAINT [PK_Gat_OnlineShopSet] PRIMARY KEY CLUSTERED 
        (
            [ID] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
        CONSTRAINT [UQ_Gat_OnlineShopSet_Solution_Online_Offline] 
        UNIQUE ([SolutionId], [OnlineShopCode], [OfflineShopCode])
    ) ON [PRIMARY]
END
GO
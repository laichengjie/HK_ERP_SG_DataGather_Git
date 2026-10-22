
--
/*
drop table Gat_Nike_PosSale_OC
drop table Gat_Nike_PosSale_transfer_OC
drop table Gat_Nike_MeterialINV_OC
drop table Gat_Nike_Stock_Transfer_OC
drop table Gat_Nike_OutIn_OC
drop table Gat_Nike_Move_OC
drop table Gat_Nike_Move_Transfer_OC
*/

---Pos 零售
IF NOT Exists(select top 1 1 from sysObjects where Id=OBJECT_ID(N'Gat_Nike_PosSale_OC') and xtype='U')
BEGIN
    CREATE TABLE [dbo].[Gat_Nike_PosSale_OC](
        [id] [int] IDENTITY(1, 1) NOT NULL,
        [SolutionID] [int] NOT NULL,
        [CompanyID] [varchar](40) NULL,
        [CompanyName] [nvarchar](100) NULL,
        [CardID] [varchar](40) NULL,
        [CardName] [nvarchar](60) NULL,
        [ShopCode] [varchar](40) NOT NULL,                          -- SHOP_CODE 门店编码
        [ShopName] [nvarchar](60) NULL,
        [StockCode] [varchar](40) NULL,                             -- fulfill_shop_code 发货门店代码
        [StockName] [nvarchar](60) NULL,
        [BillNo] [varchar](40) NOT NULL,                            -- TRANSACTION_ID 交易流水号/单据号
		[Sequence] [varchar](10) NOT NULL,
        [BillDate] [datetime] NOT NULL,                             -- TRANSACTION_DT 销售日期
        [OutBillDate] [datetime]  NULL,                          -- shipment_DT 发货日期
        [BillTime] [datetime] NOT NULL,
        [MaterialCode] [varchar](50) NULL,                          -- PRODUCT_ID 货号
        [MaterialName] [nvarchar](200) NULL,                        -- PRODUCT_NM 商品名称
        [BarCode] [varchar](60) NULL,                               -- UPC 条形码
        [SizeCode] [varchar](40) NULL,                              -- SIZE_CODE 尺码
        [SizeName] [nvarchar](40) NULL,
        [Qty] [numeric](10, 2) NOT NULL,                            -- QUANTITY 销售数量
        [BalaPrice] [decimal](24, 6) NULL,                          -- PRICE 实际销售价
        [SaleAmount] [decimal](24, 6) NULL,                         -- SALES_AMOUNT 实际销售金额
        [RetailPrice] [decimal](24, 6) NULL,                        -- MSRP_VALUE 建议零售价(吊牌金额)
        [RetailAmount] [decimal](24, 6) NULL,                       -- TOTAL_MSRP_VALUE 吊牌总金额
        [DiscountAmount] [decimal](24, 6) NULL,                     -- TOTAL_DISCOUNT_AMT 折扣总金额
        [TradeType] [varchar](20) NOT NULL DEFAULT ('1'),           -- 交易类型
        [ModifyTime] [datetime] NULL,---- PROCESS_DT 数据处理时间戳
        [IsImport] [bit] NULL DEFAULT (0)
        CONSTRAINT [PK_Gat_Nike_PosSale_OC] PRIMARY KEY CLUSTERED (
            [id] ASC
        ) WITH (
            PAD_INDEX = OFF,
            STATISTICS_NORECOMPUTE = OFF,
            IGNORE_DUP_KEY = OFF,
            ALLOW_ROW_LOCKS = ON,
            ALLOW_PAGE_LOCKS = ON
        ) ON [PRIMARY]
    ) ON [PRIMARY]

	CREATE UNIQUE NONCLUSTERED INDEX [IX_Gat_Nike_PosSale_OC_Unique] 
    ON [dbo].[Gat_Nike_PosSale_OC] ([SolutionID], [CompanyID], [BillNo], [Sequence], [BarCode])
END
GO

-- Pos 导入中间表
IF NOT Exists(select top 1 1 from sysObjects where Id=OBJECT_ID(N'Gat_Nike_PosSale_transfer_OC') and xtype='U')
BEGIN
    CREATE TABLE [dbo].[Gat_Nike_PosSale_transfer_OC](
        [TransID] [int] IDENTITY(1, 1) NOT NULL,
        [SolutionID] [int] NOT NULL,
        [CompanyID] [varchar](40) NULL,
        [CompanyName] [nvarchar](100) NULL,
        [CardID] [varchar](40) NULL,
        [CardName] [nvarchar](60) NULL,
        [ShopCode] [varchar](40) NOT NULL,                          -- SHOP_CODE 门店编码
        [ShopName] [nvarchar](60) NULL,
        [StockCode] [varchar](40) NULL,                             -- fulfill_shop_code 发货门店代码
        [StockName] [nvarchar](60) NULL,
        [BillNo] [varchar](40) NOT NULL,                            -- TRANSACTION_ID 交易流水号/单据号
        [BillDate] [datetime] NOT NULL,                             -- TRANSACTION_DT 销售日期
        [OutBillDate] [datetime]  NULL,                          -- shipment_DT 发货日期
        [BillTime] [datetime] NOT NULL,
        [MaterialCode] [varchar](50) NULL,                          -- PRODUCT_ID 货号
        [MaterialName] [nvarchar](200) NULL,                        -- PRODUCT_NM 商品名称
        [BarCode] [varchar](60) NULL,                               -- UPC 条形码
        [SizeCode] [varchar](40) NULL,                              -- SIZE_CODE 尺码
        [SizeName] [nvarchar](40) NULL,
        [Qty] [numeric](10, 2) NOT NULL,                            -- QUANTITY 销售数量
        [BalaPrice] [decimal](24, 6) NULL,                          -- PRICE 实际销售价
        [SaleAmount] [decimal](24, 6) NULL,                         -- SALES_AMOUNT 实际销售金额
        [RetailPrice] [decimal](24, 6) NULL,                        -- MSRP_VALUE 建议零售价(吊牌金额)
        [RetailAmount] [decimal](24, 6) NULL,                       -- TOTAL_MSRP_VALUE 吊牌总金额
        [DiscountAmount] [decimal](24, 6) NULL,                     -- TOTAL_DISCOUNT_AMT 折扣总金额
        [TradeType] [varchar](20) NOT NULL DEFAULT ('1'),           -- 交易类型
        [ModifyTime] [datetime] NULL,---- PROCESS_DT 数据处理时间戳
        [IsImport] [bit] NULL DEFAULT (0)
        CONSTRAINT [PK_Gat_Nike_PosSale_transfer_OC] PRIMARY KEY CLUSTERED (
            [TransID] ASC,
            [SolutionID] ASC,
            [BillNo] ASC
        ) WITH (
            PAD_INDEX = OFF,
            STATISTICS_NORECOMPUTE = OFF,
            IGNORE_DUP_KEY = OFF,
            ALLOW_ROW_LOCKS = ON,
            ALLOW_PAGE_LOCKS = ON
        ) ON [PRIMARY]
    ) ON [PRIMARY]
END
GO


--库存
-- drop  table Gat_Nike_MeterialINV_OC
IF NOT EXISTS (SELECT TOP 1 1 FROM sysObjects WHERE Id = OBJECT_ID(N'Gat_Nike_MeterialINV_OC') AND xtype = 'U'
)
BEGIN
    CREATE TABLE [dbo].[Gat_Nike_MeterialINV_OC] (
        [SolutionID]        INT              NOT NULL,
        [CompanyID]         VARCHAR(40)     NOT NULL,
        [CompanyName]       NVARCHAR(50)     NULL,
        [CardID]            VARCHAR(40)     NOT NULL,
        [CardName]          VARCHAR(20)      NULL,
        [ShopCode]          VARCHAR(40)      NULL,   -- SITE_CODE 门店/仓库编码
        [ShopName]          NVARCHAR(60)     NULL,
        [StockCode]         VARCHAR(40)     NOT NULL,       -- fulfill_shop_code 发货门店代码
        [StockName]         NVARCHAR(200)    NULL,
        [MaterialCode]      VARCHAR(50)      NULL,       -- PRODUCT_ID(SKU) 货号
        [MaterialName]      NVARCHAR(200)    NULL,       -- PRODUCT_NM 商品名称
        [BarCode]           VARCHAR(60)      NOT  NULL,       -- UPC 条形码
        [SizeCode]          VARCHAR(40)      NULL,       -- SIZE_CODE 尺码
        [SizeName]          NVARCHAR(40)     NULL,
        [UpdateDate]        DATETIME         NOT NULL,       -- INVENTORY_DATE 库存日期
        [Qty]               NUMERIC(24, 6)   NULL,       -- QUANTITY 库存数量
		[PathQty]           NUMERIC(24, 6)   NULL,   --在途数量
        [InventoryTypeDesc] NVARCHAR(50)     NULL,       -- INVENTORY_TYPE_DESC 库存类型描述
        [RetailPrice]       DECIMAL(24, 6)   NULL,       -- MSRP_VALUE 建议零售价(吊牌金额)
        [ModifyTime]        DATETIME         NULL,      ---- LAST_UPDATED 最后更新时间
        [IsImport]          BIT              NULL DEFAULT (0)
    ) ON [PRIMARY]
END
GO



IF NOT EXISTS (SELECT TOP 1 1 FROM sysObjects WHERE Id = OBJECT_ID(N'PK_Gat_Nike_MeterialINV_OC') AND xtype = 'PK')
BEGIN
    ALTER TABLE [dbo].[Gat_Nike_MeterialINV_OC] 
    ADD CONSTRAINT [PK_Gat_Nike_MeterialINV_OC] PRIMARY KEY (
        [SolutionID],
        [CompanyID],
        [CardID],
        [StockCode],
        [BarCode],
        [UpdateDate]
    )
END
GO


-- 库存 导入中间表
IF NOT EXISTS (SELECT TOP 1 1 FROM sysObjects WHERE Id = OBJECT_ID(N'Gat_Nike_Stock_Transfer_OC') AND xtype = 'U'
)
BEGIN
    CREATE TABLE [dbo].[Gat_Nike_Stock_Transfer_OC] (
        [SolutionID]        INT              NOT NULL,
        [CompanyID]         VARCHAR(40)     NOT NULL,
        [CompanyName]       NVARCHAR(50)     NULL,
        [CardID]            VARCHAR(40)      NULL,
        [CardName]          VARCHAR(20)      NULL,
        [ShopCode]          VARCHAR(40)      NULL,   -- SITE_CODE 门店/仓库编码
        [ShopName]          NVARCHAR(60)     NULL,
        [StockCode]         VARCHAR(40)      NULL,       -- fulfill_shop_code 发货门店代码
        [StockName]         NVARCHAR(200)    NULL,
        [MaterialCode]      VARCHAR(50)      NULL,       -- PRODUCT_ID(SKU) 货号
        [MaterialName]      NVARCHAR(200)    NULL,       -- PRODUCT_NM 商品名称
        [BarCode]           VARCHAR(60)      NOT  NULL,       -- UPC 条形码
        [SizeCode]          VARCHAR(40)      NULL,       -- SIZE_CODE 尺码
        [SizeName]          NVARCHAR(40)     NULL,
        [UpdateDate]        DATETIME         NULL,       -- INVENTORY_DATE 库存日期
        [Qty]               NUMERIC(24, 6)   NULL,       -- QUANTITY 库存数量
		[PathQty]           NUMERIC(24, 6)   NULL,   --在途数量
        [InventoryTypeDesc] NVARCHAR(50)     NULL,       -- INVENTORY_TYPE_DESC 库存类型描述
        [RetailPrice]       DECIMAL(24, 6)   NULL,       -- MSRP_VALUE 建议零售价(吊牌金额)
        [ModifyTime]        DATETIME         NULL,      ---- LAST_UPDATED 最后更新时间
        [IsImport]          BIT              NULL DEFAULT (0)
    ) ON [PRIMARY]
END
GO

--出入库
IF NOT EXISTS (SELECT TOP 1 1 FROM sysObjects WHERE Id = OBJECT_ID(N'Gat_Nike_OutIn_OC') AND xtype = 'U')
BEGIN
    CREATE TABLE [dbo].[Gat_Nike_OutIn_OC](
        [id] [int] IDENTITY(1, 1) NOT NULL,
        [SolutionID] [int] NOT NULL,
        [CompanyID] [varchar](40) NULL,
        [CompanyName] [nvarchar](100) NULL,
        [CardID] [varchar](40) NULL,
        [CardName] [nvarchar](60) NULL,
        [BillNo] [varchar](40) NOT NULL, --Movement_Document_Number 出入库移动凭证 出入库同一张单据
		[Sequence] [varchar](10) NOT NULL,
        [BillDate] [datetime] NOT NULL,
        [MaterialCode] [VARCHAR](50) NULL, -- PRODUCT_ID 货号
        [MaterialName] [NVARCHAR](200) NULL, -- PRODUCT_NM 商品名称
        [BarCode] [VARCHAR](60) NOT NULL, --UPC 条形码
        [SizeCode] [VARCHAR](40) NULL, -- SIZE_CODE 尺码
        [SizeName] [NVARCHAR](40) NULL,
        [OutStockCode] [varchar](50) NULL, --Departure_SITE_CODE 发货店/仓编码 来自仓库DC或者其他门店的编码
        [OutStockName] [nvarchar](100) NULL,
        [InStockCode] [varchar](50) NULL, --Destination_SITE_CODE 目的店/仓编码 目的地，比如门店或者其他仓编码
        [InStockName] [nvarchar](100) NULL,
        [Qty] [numeric](10, 2) NULL, --Quantity 数量 出入库件数
        [Amount] [decimal](24, 6) NULL, -- 金额
        [MovementType] [varchar](10) NULL, --货物移动类型
        [PackingListNumber] [Nvarchar](1000) NULL, --装箱单号
        [ModifyTime] [datetime] NULL,--transfer_dt 出入库日期
		[IsImport]   [BIT]      NULL DEFAULT (0),
		[BillTypeID] [VARCHAR](20) NULL,
        [BillTypeName] [NVARCHAR](40) NULL      -- 单据类型名称
        CONSTRAINT [PK_Gat_Nike_OutIn_OC] PRIMARY KEY CLUSTERED (
            [id] ASC,
            [SolutionID] ASC,
            [BillNo] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]

	CREATE UNIQUE NONCLUSTERED INDEX [IX_Gat_Nike_OutIn_OC_Unique] 
    ON [dbo].[Gat_Nike_OutIn_OC] ([SolutionID], [CompanyID], [BillNo], [Sequence], [BarCode])
END
GO

--调拨

IF NOT EXISTS (SELECT TOP 1 1 FROM sysObjects WHERE Id = OBJECT_ID(N'Gat_Nike_Move_OC') AND xtype = 'U')
BEGIN
    CREATE TABLE [dbo].Gat_Nike_Move_OC(
        [id] [int] IDENTITY(1, 1) NOT NULL,
        [SolutionID] [int] NOT NULL,
        [CompanyID] [varchar](40) NULL,
        [CompanyName] [nvarchar](100) NULL,
        [CardID] [varchar](40) NULL,
        [CardName] [nvarchar](60) NULL,
        [BillNo] [varchar](40) NOT NULL, --Movement_Document_Number 出入库移动凭证 出入库同一张单据
		[Sequence] [varchar](10) NOT NULL,
        [BillDate] [datetime] NOT NULL,
        [MaterialCode] [VARCHAR](50) NULL, -- PRODUCT_ID 货号
        [MaterialName] [NVARCHAR](200) NULL, -- PRODUCT_NM 商品名称
        [BarCode] [VARCHAR](60) NOT NULL, --UPC 条形码
        [SizeCode] [VARCHAR](40) NULL, -- SIZE_CODE 尺码
        [SizeName] [NVARCHAR](40) NULL,
        [OutStockCode] [varchar](50) NULL, --Departure_SITE_CODE 发货店/仓编码 来自仓库DC或者其他门店的编码
        [OutStockName] [nvarchar](100) NULL,
        [InStockCode] [varchar](50) NULL, --Destination_SITE_CODE 目的店/仓编码 目的地，比如门店或者其他仓编码
        [InStockName] [nvarchar](100) NULL,
        [Qty] [numeric](10, 2) NULL, --Quantity 数量 出入库件数
        [MovementType] [varchar](10) NOT NULL, --货物移动类型
        [StockTransferStatus] [varchar](100) NULL, --Stock_Transfer_Status	商品调拨状态
        [ModifyTime] [datetime] NULL,--transfer_dt 出入库日期
		[IsImport]          BIT              NULL DEFAULT (0),
		[BillTypeID] [VARCHAR](20) NULL,
        [BillTypeName] [NVARCHAR](40) NULL      -- 单据类型名称
        CONSTRAINT [PK_Gat_Nike_Move_OC] PRIMARY KEY CLUSTERED (
            [id] ASC,
            [SolutionID] ASC,
            [BillNo] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]

	CREATE UNIQUE NONCLUSTERED INDEX [IX_Gat_Nike_Move_OC_Unique] 
    ON [dbo].[Gat_Nike_Move_OC] ([SolutionID], [CompanyID], [BillNo], [Sequence], [BarCode],[MovementType])
END
GO


--调拨 导入中间表
IF NOT EXISTS (SELECT TOP 1 1 FROM sysObjects WHERE Id = OBJECT_ID(N'Gat_Nike_Move_Transfer_OC') AND xtype = 'U')
BEGIN
    CREATE TABLE [dbo].Gat_Nike_Move_Transfer_OC(
        [TransID] [int] IDENTITY(1, 1) NOT NULL,
        [SolutionID] [int] NOT NULL,
        [CompanyID] [varchar](40) NULL,
        [CompanyName] [nvarchar](100) NULL,
        [CardID] [varchar](40) NULL,
        [CardName] [nvarchar](60) NULL,
        [BillNo] [varchar](40) NOT NULL, --Movement_Document_Number 出入库移动凭证 出入库同一张单据
        [BillDate] [datetime] NOT NULL,
        [MaterialCode] [VARCHAR](50) NULL, -- PRODUCT_ID 货号
        [MaterialName] [NVARCHAR](200) NULL, -- PRODUCT_NM 商品名称
        [BarCode] [VARCHAR](60) NOT NULL, --UPC 条形码
        [SizeCode] [VARCHAR](40) NULL, -- SIZE_CODE 尺码
        [SizeName] [NVARCHAR](40) NULL,
        [OutStockCode] [varchar](50) NULL, --Departure_SITE_CODE 发货店/仓编码 来自仓库DC或者其他门店的编码
        [OutStockName] [nvarchar](100) NULL,
        [InStockCode] [varchar](50) NULL, --Destination_SITE_CODE 目的店/仓编码 目的地，比如门店或者其他仓编码
        [InStockName] [nvarchar](100) NULL,
        [Qty] [numeric](10, 2) NULL, --Quantity 数量 出入库件数
        [MovementType] [varchar](10) NULL, --货物移动类型
        [StockTransferStatus] [varchar](100) NULL, --Stock_Transfer_Status	商品调拨状态
        [ModifyTime] [datetime] NULL,--transfer_dt 出入库日期
		[IsImport]          BIT              NULL DEFAULT (0),
		[BillTypeID] [VARCHAR](20) NULL,
        [BillTypeName] [NVARCHAR](40) NULL      -- 单据类型名称
        CONSTRAINT [PK_Gat_Nike_Move_Transfer_OC] PRIMARY KEY CLUSTERED (
            [TransID] ASC,
            [SolutionID] ASC,
            [BillNo] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
END
GO























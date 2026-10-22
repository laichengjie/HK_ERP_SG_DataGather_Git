IF OBJECT_ID ('Gat_MaterialRange', 'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[Gat_MaterialRange](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[SolutionId] [INT] NOT NULL,--提报方案Id
		[MaterialCode] [varchar](128) NOT NULL,--货号
		[MaterialName] [NVarchar](128) NULL, --货品名称
		[CreateTime] datetime not null,--新增时间
		[ModifyTime] datetime not null,--修改时间
	) ON [PRIMARY]
	END
GO

IF OBJECT_ID ('Gat_SpecialMaterialRange', 'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[Gat_SpecialMaterialRange](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[SolutionId] [INT] NOT NULL,--提报方案Id
		[MaterialCode] [varchar](128) NOT NULL,--货号
		[MaterialName] [NVarchar](128) NULL, --货品名称
		[ShopCode] [varchar](128) NOT NULL,--店铺编码
		[ShopName] [NVarchar](128) NULL,--店铺名称
		[CreateTime] datetime not null,--新增时间
		[ModifyTime] datetime not null,--修改时间
	) ON [PRIMARY]
	END
GO
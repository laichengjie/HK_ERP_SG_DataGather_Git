
IF OBJECT_ID('Bas_VendorParameter') IS NULL
BEGIN
    CREATE TABLE [dbo].[Bas_VendorParameter]
    (
        [ID] [INT] IDENTITY(1, 1) NOT NULL,
        [ParameterCode] [VARCHAR](100) NULL,
    	[ParameterName] [NVARCHAR](100) NULL,
        [ReportPath] [TINYINT] NULL,
        CONSTRAINT [PK_Bas_VendorParameter] PRIMARY KEY CLUSTERED ([ID] ASC)
    );

    CREATE UNIQUE NONCLUSTERED INDEX [UX_Bas_VendorParameter]
    ON [dbo].[Bas_VendorParameter] ([ReportPath] ASC, [ParameterCode] ASC);
END;
GO


IF OBJECT_ID('Bas_VendParameterValue') IS NULL
BEGIN
    CREATE TABLE [dbo].[Bas_VendParameterValue]
    (
        [ID] [INT] IDENTITY(1, 1) NOT NULL,
        [VendorID] [INT] NOT NULL,
        [ParameterCode] [VARCHAR](100) NULL,
	    [ParameterValue] [VARCHAR](250) NULL,
        [ModifyDate] [DATETIME] NOT NULL,
        CONSTRAINT [PK_Bas_VendParameterValue] PRIMARY KEY CLUSTERED ([ID] ASC)
    );

    CREATE UNIQUE NONCLUSTERED INDEX [UX_Bas_VendParameterValue]
    ON [dbo].[Bas_VendParameterValue] ([VendorID] ASC, [ParameterCode] ASC);
END;
GO


-- NikeApi 供应商配置
begin

-- 获取Toke地址
IF NOT EXISTS (SELECT 1 FROM dbo.Bas_VendorParameter WHERE ParameterCode = 'GetTokenUrl')
BEGIN
    INSERT INTO dbo.Bas_VendorParameter (ParameterCode, ParameterName, ReportPath)
    VALUES ('GetTokenUrl', '获取Token地址', 29);
END


--API 接口地址
IF NOT EXISTS (SELECT 1 FROM dbo.Bas_VendorParameter WHERE ParameterCode = 'ApiUrl')
BEGIN
    INSERT INTO dbo.Bas_VendorParameter (ParameterCode, ParameterName, ReportPath)
    VALUES ('ApiUrl', 'API接口地址', 29);
END


-- 客户端标识
IF NOT EXISTS (SELECT 1 FROM dbo.Bas_VendorParameter WHERE ParameterCode = 'ClientId')
BEGIN
    INSERT INTO dbo.Bas_VendorParameter (ParameterCode, ParameterName, ReportPath)
    VALUES ('ClientId', '客户端标识', 29);
END


-- 2客户端密钥
IF NOT EXISTS (SELECT 1 FROM dbo.Bas_VendorParameter WHERE ParameterCode = 'ClientSecret')
BEGIN
    INSERT INTO dbo.Bas_VendorParameter (ParameterCode, ParameterName, ReportPath)
    VALUES ('ClientSecret', '客户端密钥', 29);
END


-- 合作方编码
IF NOT EXISTS (SELECT 1 FROM dbo.Bas_VendorParameter WHERE ParameterCode = 'PartnerCode')
BEGIN
    INSERT INTO dbo.Bas_VendorParameter (ParameterCode, ParameterName, ReportPath)
    VALUES ('PartnerCode', '合作方编码', 29);
END


--  数据来源系统名称（自营/加盟）
IF NOT EXISTS (SELECT 1 FROM dbo.Bas_VendorParameter WHERE ParameterCode = 'SourceSystem')
BEGIN
    INSERT INTO dbo.Bas_VendorParameter (ParameterCode, ParameterName, ReportPath)
    VALUES ('SourceSystem', '数据来源系统名称', 29);
END
END



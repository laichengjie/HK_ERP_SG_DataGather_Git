
/*
SELECT a.name,type_name(xtype), a.length,b.name FROM sys.syscolumns  a 
INNER JOIN sys.tables b  ON b.object_id=a.id
WHERE a.name IN('Materialcode','MaterialID') AND a.length<40
*/
--货号长度调整
IF EXISTS(SELECT TOP  1 1 FROM syscolumns  WHERE (id = OBJECT_ID('Gat_LiNing_PosSale'))  AND    syscolumns.name='MaterialCode' AND  syscolumns.length<40 AND  type_name(xtype)='varchar')
BEGIN
IF COL_LENGTH('dbo.Gat_LiNing_PosSale','MaterialCode') IS not NULL 
begin
	alter table Gat_LiNing_PosSale alter column MaterialCode Varchar(40) not null 
end
END



IF EXISTS(SELECT TOP  1 1 FROM syscolumns  WHERE (id = OBJECT_ID('Gat_Ad_PosSale'))  AND    syscolumns.name='MaterialCode' AND  syscolumns.length<40 AND  type_name(xtype)='varchar')
BEGIN
IF COL_LENGTH('dbo.Gat_Ad_PosSale','MaterialCode') IS not NULL 
begin
	alter table Gat_Ad_PosSale alter column MaterialCode Varchar(40) not null 
end
END


IF EXISTS(SELECT TOP  1 1 FROM syscolumns  WHERE (id = OBJECT_ID('Gat_JackWolfSkin_PosSale'))  AND    syscolumns.name='MaterialCode' AND  syscolumns.length<40 AND  type_name(xtype)='varchar')
BEGIN
IF COL_LENGTH('dbo.Gat_JackWolfSkin_PosSale','MaterialCode') IS not NULL 
begin
	alter table Gat_JackWolfSkin_PosSale alter column MaterialCode Varchar(40) not null 
end
END

/*
----- 货品简称------------------------
SELECT a.name,type_name(xtype), a.length,b.name FROM sys.syscolumns  a 
INNER JOIN sys.tables b  ON b.object_id=a.id
WHERE a.name ='MaterialShortName' --AND a.length<40
*/

/*
---货品全称-------------------------------
SELECT a.name,type_name(xtype), a.length,b.name FROM sys.syscolumns  a 
INNER JOIN sys.tables b  ON b.object_id=a.id
WHERE a.name ='MaterialName' --AND a.length<40

*/
IF COL_LENGTH('dbo.Gat_Columbia_MaterialInv','MaterialName') IS not NULL 
begin
	alter table Gat_Columbia_MaterialInv alter column MaterialName NVarchar(200)  
END

IF COL_LENGTH('dbo.Gat_MaterialRange','MaterialName') IS not NULL 
begin
	alter table Gat_MaterialRange alter column MaterialName NVarchar(200)  
END

IF COL_LENGTH('dbo.Gat_SpecialMaterialRange','MaterialName') IS not NULL 
begin
	alter table Gat_SpecialMaterialRange alter column MaterialName NVarchar(200)  
END

IF COL_LENGTH('dbo.Gat_North_MaterialInv','MaterialName') IS not NULL 
begin
	alter table Gat_North_MaterialInv alter column MaterialName NVarchar(200)  
END


IF COL_LENGTH('dbo.Gat_North_PosSale','MaterialName') IS not NULL 
begin
	alter table Gat_North_PosSale alter column MaterialName NVarchar(200)  
END




/*

SELECT a.name,type_name(xtype), a.length,b.name FROM sys.syscolumns  a 
INNER JOIN sys.tables b  ON b.object_id=a.id
WHERE a.name ='BarCode' --AND a.length<40

*/








UPDATE Bas_Vendor SET ShopNumber=0 WHERE ISNULL(ShopNumber,0)=0

IF NOT EXISTS (SELECT TOP  1 1 FROM Sys_User WHERE UserCode='HKshop')
BEGIN
INSERT INTO dbo.Sys_User
        ( UserCode ,
          UserName ,
          Password ,
          RoleType
        )
VALUES  ( N'HKshop' , -- UserCode - nvarchar(20)
          N'HK' , -- UserName - nvarchar(50)
          N'hkabc.123' , -- Password - nvarchar(20)
          3  -- RoleType - tinyint
        )
END
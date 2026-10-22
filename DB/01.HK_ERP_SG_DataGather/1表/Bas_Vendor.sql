--Ìí¼Ó×Ö¶Î
IF COL_LENGTH('Bas_Vendor', 'ShopNumber') IS NULL
    ALTER TABLE Bas_Vendor ADD ShopNumber INT  DEFAULT 0;
GO




IF COL_LENGTH('Gat_Ad_Move_OC', 'ManualBillNo') IS NULL
    ALTER TABLE Gat_Ad_Move_OC ADD ManualBillNo NVARCHAR(80) NULL;
GO

IF COL_LENGTH('Gat_Ad_Move_OCTB', 'ManualBillNo') IS NULL
    ALTER TABLE Gat_Ad_Move_OCTB ADD ManualBillNo NVARCHAR(80) NULL;
GO

IF COL_LENGTH('Gat_Ad_Move_Transfer_OC', 'ManualBillNo') IS NULL
    ALTER TABLE Gat_Ad_Move_Transfer_OC ADD ManualBillNo NVARCHAR(80) NULL;
GO


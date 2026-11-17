-- 1 线下 ，2 线上，  4 团购
IF COL_LENGTH('Gat_Ad_PosSale', 'TradeType') IS NULL
    ALTER TABLE Gat_Ad_PosSale ADD  TradeType  int  NULL;
GO
-- 1 线下 ，2 线上，  4 团购
IF COL_LENGTH('Gat_Ad_PosSale', 'TradeTypeName') IS NULL
    ALTER TABLE Gat_Ad_PosSale ADD  TradeTypeName  NVARCHAR(20)  NULL;
GO
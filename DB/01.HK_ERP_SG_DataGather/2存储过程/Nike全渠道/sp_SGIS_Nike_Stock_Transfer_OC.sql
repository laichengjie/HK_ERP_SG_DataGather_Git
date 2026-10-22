IF EXISTS (SELECT *
           FROM sys.objects
           WHERE object_id = OBJECT_ID(N'[dbo].[sp_SGIS_Nike_Stock_Transfer_OC]')
                 AND type IN (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_SGIS_Nike_Stock_Transfer_OC];
GO

/* ==========================================================================================
描    述：Nike全渠道 POS库存中转数据导入
创 建 人：
创建日期：
说明：
更新说明：
  - 2026-06-24: 添加事务处理保证数据一致性
-- ==========================================================================================*/
CREATE PROCEDURE [dbo].[sp_SGIS_Nike_Stock_Transfer_OC]
    @SolutionId INT = 0,
    @TransIDstr NVARCHAR(2000) = ''
AS
BEGIN
    BEGIN TRANSACTION;

    -- 更新目标表的中转信息（公司、卡等）
    UPDATE a
    SET a.CompanyID = b.CompanyID,
        a.CompanyName = c.CompanyName,
        a.CardID = b.CardID,
        a.CardName = d.CardName
    FROM Gat_Nike_Stock_Transfer_OC a
        INNER JOIN Gat_Solution b ON b.id = a.SolutionID
        LEFT JOIN Bas_Company c ON c.CompanyID = b.CompanyID
        LEFT JOIN Bas_Card d ON d.CardID = b.CardID
    WHERE ISNULL(a.CompanyID, '') = '';

    -- 将中转数据插入到正式库存表
    INSERT INTO [dbo].[Gat_Nike_MeterialINV_OC]
    (
        [SolutionID],
        [CompanyID],
        [CompanyName],
        [CardID],
        [CardName],
        [ShopCode],
        [ShopName],
        [StockCode],
        [StockName],
        [MaterialCode],
        [MaterialName],
        [BarCode],
        [SizeCode],
        [SizeName],
        [UpdateDate],
        [Qty],
		PathQty,
        [InventoryTypeDesc],
        [RetailPrice],
        [ModifyTime],
        [IsImport]
    )
    SELECT
        [SolutionID],
        [CompanyID],
        [CompanyName],
        [CardID],
        [CardName],
        [ShopCode],
        [ShopName],
        [StockCode],
        [StockName],
        [MaterialCode],
        [MaterialName],
        [BarCode],
        [SizeCode],
        [SizeName],
        [UpdateDate],
        [Qty],
		PathQty,
        [InventoryTypeDesc],
        [RetailPrice],
        GETDATE() AS [ModifyTime],
        1 AS [IsImport]
    FROM Gat_Nike_Stock_Transfer_OC;

    DELETE FROM [dbo].[Gat_Nike_Stock_Transfer_OC] WHERE SolutionId = @SolutionId;

    COMMIT TRANSACTION;
END
GO
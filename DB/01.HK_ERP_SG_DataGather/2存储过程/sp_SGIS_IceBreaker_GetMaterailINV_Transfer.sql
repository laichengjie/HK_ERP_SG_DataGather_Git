IF EXISTS
(
    SELECT *
    FROM sys.objects
    WHERE object_id = OBJECT_ID(N'[dbo].[sp_SGIS_IceBreaker_GetMaterailINV_Transfer]')
          AND type IN ( N'P', N'PC' )
)
    DROP PROCEDURE [dbo].sp_SGIS_IceBreaker_GetMaterailINV_Transfer;
GO

/* ==========================================================================================
描    述：
创 建 人：
创建日期：
说明：
更新说明：
-- ==========================================================================================*/
CREATE PROCEDURE [dbo].[sp_SGIS_IceBreaker_GetMaterailINV_Transfer]
    @SolutionId INT = 0,
    @TransIDstr NVARCHAR(2000) = '',
    @Type INT = 0,                     -- 0 转上传   1删除  2 上传成功回写状态
    @OperationType VARCHAR(40) = 'Pos' --Pos单  Stock库存
AS
BEGIN


    DECLARE @CompanyID VARCHAR(20) = '';
    DECLARE @CompanyName NVARCHAR(40) = N'';
    DECLARE @CardID NVARCHAR(20) = N'';
    DECLARE @CardName NVARCHAR(40) = N'';

    IF @OperationType = 'Stock'
    BEGIN
        IF @Type = 0
        BEGIN
            SELECT @CompanyName = CompanyName,
                   @CardID = CardID,
                   @SolutionId = ID
            FROM Gat_Solution a,
                 Bas_Company b
            WHERE a.CompanyID = b.CompanyID
                  AND a.ID = @SolutionId;

            SELECT *
            FROM Gat_Solution;
            SELECT @CardName = CardName
            FROM Bas_Card
            WHERE CardID = @CardID;

            --覆盖库存
            --DELETE a
            --FROM Gat_IceBreaker_Meterial_Transfer a
            --WHERE SolutionId=@SolutionId

            INSERT INTO dbo.Gat_IceBreaker_MaterialInv
            (
                SolutionID,
                CardID,
                CardName,
                CompanyID,
                CompanyName,
                ShopID,
                ShopCode,
                ShopName,
                MaterialCode,
                Barcode,
                ActualQty,
                PathQty,
                IsImport,
                ModifyDtm
            )
            SELECT @SolutionId SolutionID,
                   @CardID CardID,
                   @CardName CardName,
                   @CompanyID CompanyID,
                   @CompanyName CompanyName,
                   NULL ShopID,
                   ShopCode,
                   ShopName,
                   MaterialCode,
                   Barcode,
                   ActualQty,
                   PathQty,
                   IsImport,
                   ModifyDtm
            FROM dbo.Gat_IceBreaker_MaterialInv_Transfer
            WHERE SolutionID = @SolutionId;

            DELETE dbo.Gat_IceBreaker_MaterialInv_Transfer
            WHERE SolutionID = @SolutionId;
        END;
        IF @Type = 1
        BEGIN
            DELETE dbo.Gat_IceBreaker_MaterialInv_Transfer
            WHERE SolutionID = @SolutionId;
        END;
    END;
END;


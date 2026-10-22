IF EXISTS
(
    SELECT *
    FROM sys.objects
    WHERE object_id = OBJECT_ID(N'[dbo].[sp_SGIS_GaIceBreakertransferAsPos]')
          AND type IN ( N'P', N'PC' )
)
    DROP PROCEDURE [dbo].sp_SGIS_GaIceBreakertransferAsPos;
GO

/* ==========================================================================================
描    述：
创 建 人：
创建日期：
说明：
更新说明：
-- ==========================================================================================*/
CREATE PROC [dbo].[sp_SGIS_GaIceBreakertransferAsPos]
    @SolutionId INT = 0,
    @TransIDstr NVARCHAR(2000) = ''
AS
BEGIN
    --
    IF (@TransIDstr = '') --没传表示保存全部至销售中间库
    BEGIN

        ----更新已存在的
        --UPDATE a
        --SET a.[Qty] = (a.[Qty] + b.Qty),
        --    a.[RetailPrice] = b.RetailPrice,
        --    a.[PayAmount] = (a.Qty + b.Qty) * b.RetailPrice
        --FROM [Gat_IceBreaker_PosSale] a
        --    INNER JOIN [dbo].[Gat_IceBreaker_PosSale_transfer] b
        --        ON a.[SolutionID] = b.SolutionID
        --           AND a.[CompanyID] = b.CompanyID
        --           AND a.[BillNo] = b.BillNo
        --           AND a.[BarCode] = b.BarCode
        --WHERE a.SolutionID = @SolutionId;
        --插入不存在的数据
        INSERT INTO [dbo].[Gat_IceBreaker_PosSale]
        (
            SolutionID,
            CardID,
            CardName,
            CompanyID,
            CompanyName,
            ShopID,
            ShopCode,
            ShopName,
            BillNo,
            SourceBillNo,
            BillDate,
            BillTime,
            MaterialCode,
            BarCode,
            Qty,
            RetailPrice,
            PayAmount,
            IsImport,
            ModifyDtm,
			SizeName
        )
        SELECT a.SolutionID,
               a.CardID,
               a.CardName,
               a.CompanyID,
               a.CompanyName,
               a.ShopID,
               a.ShopCode,
               a.ShopName,
               a.BillNo,
               a.SourceBillNo,
               a.BillDate,
               a.BillTime,
               a.MaterialCode,
               a.BarCode,
               a.Qty,
               a.RetailPrice,
               a.PayAmount,
               1 IsImport,
               GETDATE() ModifyDtm,
			   SizeName
        FROM [dbo].[Gat_IceBreaker_PosSale_transfer] a
        WHERE a.SolutionID = @SolutionId;

        --执行删除----------===============================
        DELETE Gat_IceBreaker_PosSale_transfer
        WHERE SolutionID = @SolutionId;
    END;
    ELSE
    BEGIN
        DECLARE @TransCount INT = 0;
        DECLARE @TransIndex INT = 1;
        DECLARE @TransID INT = 0;
        --获取总数量
        SELECT @TransCount = COUNT(1)
        FROM [fnSplitStr](@TransIDstr, ',');
        WHILE @TransIndex <= @TransCount -- 循环每一条解决方案	
        BEGIN
            BEGIN TRY
                BEGIN TRANSACTION ExecBatch;
                SELECT @TransID = ikey
                FROM fnSplitStr(@TransIDstr, ',')
                WHERE iIndex = @TransIndex;

                ------------------------------------------
                --DECLARE @SolutionID INT = 0;
                DECLARE @CompanyID VARCHAR(20) = '';
                DECLARE @BillNo VARCHAR(40) = '';
                DECLARE @BarCode NVARCHAR(128) = N'';
                DECLARE @Qty DECIMAL(24, 6) = 0.0;
                DECLARE @RetailPrice DECIMAL(24, 6) = 0.0;
                ------------------------------------------
                ------------------------------------------==========================
                --获取数据
                SELECT @SolutionId = [SolutionID],
                       @CompanyID = [CompanyID],
                       @BillNo = [BillNo],
                       @BarCode = [BarCode],
                       @Qty = [Qty],
                       @RetailPrice = [RetailPrice]
                FROM [dbo].[Gat_IceBreaker_PosSale_transfer]
                WHERE [TransID] = @TransID;
                ------------------------------------------==========================
                ----判断条件（方案ID+公司名+单号+条码（货品条码））
                --IF EXISTS
                --(
                --    SELECT *
                --    FROM [dbo].[Gat_IceBreaker_PosSale]
                --    WHERE [SolutionID] = @SolutionId
                --          AND [CompanyID] = @CompanyID
                --          AND [BillNo] = @BillNo
                --          AND [BarCode] = @BarCode
                --)
                --BEGIN --1.存在  （叠加 数量 和销售金额）
                --    UPDATE [Gat_IceBreaker_PosSale]
                --    SET [Qty] = ([Qty] + @Qty),
                --        [RetailPrice] = @RetailPrice,
                --        [PayAmount] = (Qty + @Qty) * @RetailPrice
                --    WHERE [SolutionID] = @SolutionId
                --          AND [CompanyID] = @CompanyID
                --          AND [BillNo] = @BillNo
                --          AND [BarCode] = @BarCode;
                --    --执行删除----------===============================
                --    DELETE Gat_IceBreaker_PosSale_transfer
                --    WHERE [TransID] = @TransID;
                ----执行删除----------===============================
                --END;
                --ELSE
                BEGIN --2.不存在
                    INSERT INTO [dbo].[Gat_IceBreaker_PosSale]
                    (
                        SolutionID,
                        CardID,
                        CardName,
                        CompanyID,
                        CompanyName,
                        ShopID,
                        ShopCode,
                        ShopName,
                        BillNo,
                        SourceBillNo,
                        BillDate,
                        BillTime,
                        MaterialCode,
                        BarCode,
                        Qty,
                        RetailPrice,
                        PayAmount,
                        IsImport,
                        ModifyDtm,
						SizeName
                    )
                    SELECT a.SolutionID,
                           a.CardID,
                           a.CardName,
                           a.CompanyID,
                           a.CompanyName,
                           a.ShopID,
                           a.ShopCode,
                           a.ShopName,
                           a.BillNo,
                           a.SourceBillNo,
                           a.BillDate,
                           a.BillTime,
                           a.MaterialCode,
                           a.BarCode,
                           a.Qty,
                           a.RetailPrice,
                           a.PayAmount,
                           1 IsImport,
                           GETDATE() ModifyDtm,
						   SizeName
                    FROM [dbo].[Gat_IceBreaker_PosSale_transfer] a
                    WHERE [TransID] = @TransID;
                    --执行删除----------===============================
                    DELETE Gat_IceBreaker_PosSale_transfer
                    WHERE [TransID] = @TransID;
                --执行删除----------===============================
                END;
                COMMIT TRANSACTION ExecBatch;
            END TRY
            BEGIN CATCH
                --错误
                IF XACT_STATE() <> 0
                    ROLLBACK TRANSACTION;
            END CATCH;
            SET @TransIndex = @TransIndex + 1; -- 
        END;
    END;
END;


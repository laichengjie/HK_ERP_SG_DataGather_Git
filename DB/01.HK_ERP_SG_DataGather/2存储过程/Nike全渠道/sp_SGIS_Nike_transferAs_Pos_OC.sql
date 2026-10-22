IF EXISTS (SELECT *
           FROM sys.objects
           WHERE object_id = OBJECT_ID(N'[dbo].[sp_SGIS_Nike_transferAs_Pos_OC]')
                 AND type IN (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_SGIS_Nike_transferAs_Pos_OC];
GO


/* ==========================================================================================
描    述：Nike全渠道 POS中转数据导入
创 建 人：
创建日期：
说明：
更新说明：
-- ==========================================================================================*/
CREATE PROCEDURE [dbo].[sp_SGIS_Nike_transferAs_Pos_OC]
    @SolutionId INT = 0,
    @TransIDstr NVARCHAR(2000) = ''
AS
BEGIN
    
	 update  a set  a.CompanyID=b.CompanyID,
	 a.CompanyName=c.CompanyName,
	 a. CardID=b.CardID,
     a.CardName=d.CardName
	 from Gat_Nike_PosSale_transfer_OC a 
	 inner join  Gat_Solution b on b.id=a.SolutionID
	 left join Bas_Company c on c.CompanyID=b.CompanyID
	 left join Bas_Card d on d.CardID=b.CardID
	 where isnull(a.CompanyID,'')=''


    IF (@TransIDstr = '') --没传表示保存全部至销售中间库
    BEGIN
        --更新已存在的
        UPDATE a
        SET a.[Qty] = (a.[Qty] + b.Qty),
            a.[BalaPrice] = b.BalaPrice,
            a.[SaleAmount] = a.SaleAmount + b.SaleAmount,
            a.[ModifyTime] = GETDATE()
        FROM [Gat_Nike_PosSale_OC] a
            INNER JOIN [dbo].[Gat_Nike_PosSale_transfer_OC] b
                ON a.[SolutionID] = b.SolutionID
                   AND a.[CompanyID] = b.CompanyID
                   AND a.[BillNo] = b.BillNo
                   AND a.[BarCode] = b.BarCode
        WHERE a.SolutionID = @SolutionId;

        --插入不存在的数据
        INSERT INTO [dbo].[Gat_Nike_PosSale_OC]
        (
            SolutionID,
            CompanyID,
            CompanyName,
            CardID,
            CardName,
            ShopCode,
            ShopName,
            StockCode,
            StockName,
            BillNo,
			Sequence,
            BillDate,
            OutBillDate,
            BillTime,
            MaterialCode,
            MaterialName,
            BarCode,
            SizeCode,
            SizeName,
            Qty,
            BalaPrice,
            SaleAmount,
            RetailPrice,
            RetailAmount,
            DiscountAmount,
            TradeType,
            ModifyTime,
            IsImport
        )
        SELECT a.SolutionID,
               a.CompanyID,
               a.CompanyName,
               a.CardID,
               a.CardName,
               a.ShopCode,
               a.ShopName,
               a.StockCode,
               a.StockName,
               a.BillNo,
			   a.TransID,
               a.BillDate,
               a.OutBillDate,
               a.BillTime,
               a.MaterialCode,
               a.MaterialName,
               a.BarCode,
               a.SizeCode,
               a.SizeName,
               a.Qty,
               ISNULL(a.[BalaPrice], 0),
               ISNULL(a.SaleAmount, 0),
               ISNULL(a.RetailPrice, 0),
               ISNULL(a.RetailAmount, 0),
               ISNULL(a.DiscountAmount, 0),
               ISNULL(a.TradeType, '1'),
               GETDATE(),
                1 IsImport
        FROM [dbo].[Gat_Nike_PosSale_transfer_OC] a
            LEFT JOIN [Gat_Nike_PosSale_OC] b
                ON a.[SolutionID] = b.SolutionID
                   AND a.[CompanyID] = b.CompanyID
                   AND a.[BillNo] = b.BillNo
                   AND a.[BarCode] = b.BarCode
        WHERE ISNULL(b.BarCode, '') = ''
              AND a.SolutionID = @SolutionId;

        --执行删除
        DELETE Gat_Nike_PosSale_transfer_OC
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
        
        WHILE @TransIndex <= @TransCount
        BEGIN
            BEGIN TRY
                BEGIN TRANSACTION ExecBatch;
                
                SELECT @TransID = ikey
                FROM fnSplitStr(@TransIDstr, ',')
                WHERE iIndex = @TransIndex;

                --声明变量
                DECLARE @CompanyID VARCHAR(40) = '';
                DECLARE @BillNo VARCHAR(40) = '';
                DECLARE @BarCode VARCHAR(60) = '';
                DECLARE @Qty DECIMAL(10, 2) = 0.0;
                DECLARE @BalaPrice DECIMAL(24, 6) = 0.0;
                DECLARE @SaleAmount DECIMAL(24, 6) = 0.0;
                DECLARE @RetailPrice DECIMAL(24, 6) = 0.0;
                DECLARE @RetailAmount DECIMAL(24, 6) = 0.0;
                DECLARE @DiscountAmount DECIMAL(24, 6) = 0.0;
                DECLARE @OutBillDate DATETIME = NULL;
                DECLARE @MaterialName NVARCHAR(200) = '';
                DECLARE @SizeCode VARCHAR(40) = '';
                DECLARE @TradeType VARCHAR(20) = '1';
                
                --获取数据
                SELECT @SolutionId = [SolutionID],
                       @CompanyID = [CompanyID],
                       @BillNo = [BillNo],
                       @BarCode = [BarCode],
                       @Qty = [Qty],
                       @BalaPrice = [BalaPrice],
                       @SaleAmount = [SaleAmount],
                       @RetailPrice = [RetailPrice],
                       @RetailAmount = [RetailAmount],
                       @DiscountAmount = [DiscountAmount],
                       @OutBillDate = [OutBillDate],
                       @MaterialName = [MaterialName],
                       @SizeCode = [SizeCode],
                       @TradeType = [TradeType]
                FROM [dbo].[Gat_Nike_PosSale_transfer_OC]
                WHERE [TransID] = @TransID;

                --判断条件（方案ID+公司+单号+条码）
                IF EXISTS
                (
                    SELECT *
                    FROM [dbo].[Gat_Nike_PosSale_OC]
                    WHERE [SolutionID] = @SolutionId
                          AND [CompanyID] = @CompanyID
                          AND [BillNo] = @BillNo
                          AND [BarCode] = @BarCode
                )
                BEGIN
                    --存在：更新数量、金额等信息
                    UPDATE [Gat_Nike_PosSale_OC]
                    SET [Qty] = ([Qty] + @Qty),
                        [BalaPrice] = @BalaPrice,
                        [SaleAmount] = (Qty + @Qty) * @BalaPrice,
                        [ModifyTime] = GETDATE()
                    WHERE [SolutionID] = @SolutionId
                          AND [CompanyID] = @CompanyID
                          AND [BillNo] = @BillNo
                          AND [BarCode] = @BarCode;
                    
                    --删除中转数据
                    DELETE Gat_Nike_PosSale_transfer_OC
                    WHERE [TransID] = @TransID;
                END;
                ELSE
                BEGIN
                    --不存在：插入新数据
                    INSERT INTO [dbo].[Gat_Nike_PosSale_OC]
                    (
                        SolutionID,
                        CompanyID,
                        CompanyName,
                        CardID,
                        CardName,
                        ShopCode,
                        ShopName,
                        StockCode,
                        StockName,
                        BillNo,
                        BillDate,
                        OutBillDate,
                        BillTime,
                        MaterialCode,
                        MaterialName,
                        BarCode,
                        SizeCode,
                        SizeName,
                        Qty,
                        BalaPrice,
                        SaleAmount,
                        RetailPrice,
                        RetailAmount,
                        DiscountAmount,
                        TradeType,
                        ModifyTime,
                        IsImport
                    )
                    SELECT a.SolutionID,
                           a.CompanyID,
                           a.CompanyName,
                           a.CardID,
                           a.CardName,
                           a.ShopCode,
                           a.ShopName,
                           a.StockCode,
                           a.StockName,
                           a.BillNo,
                           a.BillDate,
                           a.OutBillDate,
                           a.BillTime,
                           a.MaterialCode,
                           a.MaterialName,
                           a.BarCode,
                           a.SizeCode,
                           a.SizeName,
                           a.Qty,
                           ISNULL(a.[BalaPrice], 0),
                           ISNULL(a.SaleAmount, 0),
                           ISNULL(a.RetailPrice, 0),
                           ISNULL(a.RetailAmount, 0),
                           ISNULL(a.DiscountAmount, 0),
                           ISNULL(a.TradeType, '1'),
                           GETDATE(),
                           1 IsImport
                    FROM [dbo].[Gat_Nike_PosSale_transfer_OC] a
                    WHERE [TransID] = @TransID;
                    
                    --删除中转数据
                    DELETE Gat_Nike_PosSale_transfer_OC
                    WHERE [TransID] = @TransID;
                END;
                
                COMMIT TRANSACTION ExecBatch;
            END TRY
            BEGIN CATCH
                IF XACT_STATE() <> 0
                    ROLLBACK TRANSACTION;
            END CATCH;
            
            SET @TransIndex = @TransIndex + 1;
        END;
    END;
END
GO
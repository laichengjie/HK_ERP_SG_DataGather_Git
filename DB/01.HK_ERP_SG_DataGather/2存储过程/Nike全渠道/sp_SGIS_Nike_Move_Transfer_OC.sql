IF EXISTS (SELECT *
           FROM sys.objects
           WHERE object_id = OBJECT_ID(N'[dbo].[sp_SGIS_Nike_Move_Transfer_OC]')
                 AND type IN (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_SGIS_Nike_Move_Transfer_OC];
GO

/* ==========================================================================================
描    述：Nike全渠道 POS中转数据导入
创 建 人：
创建日期：
说明：
更新说明：
-- ==========================================================================================*/
CREATE PROCEDURE [dbo].[sp_SGIS_Nike_Move_Transfer_OC]
    @SolutionId INT = 0,
    @TransIDstr NVARCHAR(2000) = ''
AS
BEGIN
    -- 更新中转表中的公司、卡信息
    UPDATE a
    SET a.CompanyID = b.CompanyID,
        a.CompanyName = c.CompanyName,
        a.CardID = b.CardID,
        a.CardName = d.CardName
    FROM Gat_Nike_Move_Transfer_OC a
        INNER JOIN Gat_Solution b ON b.id = a.SolutionID
        LEFT JOIN Bas_Company c ON c.CompanyID = b.CompanyID
        LEFT JOIN Bas_Card d ON d.CardID = b.CardID
    WHERE ISNULL(a.CompanyID, '') = '';

    -- 未传递TransID，表示保存全部至销售中间库
    IF (@TransIDstr = '')
    BEGIN
        -- 更新已存在的记录
        UPDATE a
        SET a.[Qty] = (a.[Qty] + b.Qty),
            a.[ModifyTime] = GETDATE()
        FROM [Gat_Nike_Move_OC] a
            INNER JOIN [dbo].[Gat_Nike_Move_Transfer_OC] b
                ON a.[SolutionID] = b.SolutionID
                   AND a.[CompanyID] = b.CompanyID
                   AND a.[BillNo] = b.BillNo
                   AND a.[BarCode] = b.BarCode
                   AND a.BillTypeID = b.BillTypeID
        WHERE a.SolutionID = @SolutionId;

        -- 插入不存在的数据
        INSERT INTO [dbo].[Gat_Nike_Move_OC]
        (
            SolutionID,
            CompanyID,
            CompanyName,
            CardID,
            CardName,
            BillNo,
			Sequence,
            BillDate,
            MaterialCode,
            MaterialName,
            BarCode,
            SizeCode,
            SizeName,
            OutStockCode,
            OutStockName,
            InStockCode,
            InStockName,
            Qty,
            MovementType,
            StockTransferStatus,
            ModifyTime,
            IsImport,
            BillTypeID,
            BillTypeName
        )
        SELECT a.SolutionID,
               a.CompanyID,
               a.CompanyName,
               a.CardID,
               a.CardName,
               a.BillNo,
			   a.TransID Sequence,
               a.BillDate,
               a.MaterialCode,
               a.MaterialName,
               a.BarCode,
               a.SizeCode,
               a.SizeName,
               a.OutStockCode,
               a.OutStockName,
               a.InStockCode,
               a.InStockName,
               a.Qty,
               a.MovementType,
               a.StockTransferStatus,
               GETDATE() AS ModifyTime,
               1 AS IsImport,
               a.BillTypeID,
               a.BillTypeName
        FROM [dbo].[Gat_Nike_Move_Transfer_OC] a
            LEFT JOIN [Gat_Nike_Move_OC] b
                ON a.[SolutionID] = b.SolutionID
                   AND a.[CompanyID] = b.CompanyID
                   AND a.[BillNo] = b.BillNo
                   AND a.[BarCode] = b.BarCode
                   AND a.BillTypeID = b.BillTypeID
        WHERE ISNULL(b.BarCode, '') = ''
              AND a.SolutionID = @SolutionId;

        -- 删除已处理的中转数据
        DELETE FROM Gat_Nike_Move_Transfer_OC
        WHERE SolutionID = @SolutionId;
    END;
    ELSE
    BEGIN
        -- 按TransID逐个处理
        DECLARE @TransCount INT = 0;
        DECLARE @TransIndex INT = 1;
        DECLARE @TransID INT = 0;

        -- 获取总数量
        SELECT @TransCount = COUNT(1)
        FROM [fnSplitStr](@TransIDstr, ',');

        WHILE @TransIndex <= @TransCount
        BEGIN
            BEGIN TRY
                BEGIN TRANSACTION ExecBatch;

                SELECT @TransID = ikey
                FROM fnSplitStr(@TransIDstr, ',')
                WHERE iIndex = @TransIndex;

                -- 声明变量
                DECLARE @CompanyID VARCHAR(40) = '';
                DECLARE @BillNo VARCHAR(40) = '';
                DECLARE @BarCode VARCHAR(60) = '';
                DECLARE @Qty DECIMAL(10, 2) = 0.0;
                DECLARE @SizeCode VARCHAR(40) = '';
                DECLARE @BillTypeID VARCHAR(20) = '';

                -- 获取数据
                SELECT @SolutionId = [SolutionID],
                       @CompanyID = [CompanyID],
                       @BillNo = [BillNo],
                       @BarCode = [BarCode],
                       @Qty = [Qty],
                       @BillTypeID = [BillTypeID]
                FROM [dbo].[Gat_Nike_Move_Transfer_OC]
                WHERE [TransID] = @TransID;

                -- 判断条件（方案ID + 公司 + 单号 + 条码 + 单据类型）
                IF EXISTS
                (
                    SELECT *
                    FROM [dbo].[Gat_Nike_Move_OC]
                    WHERE [SolutionID] = @SolutionId
                          AND [CompanyID] = @CompanyID
                          AND [BillNo] = @BillNo
                          AND [BarCode] = @BarCode
                          AND BillTypeID = @BillTypeID
                )
                BEGIN
                    -- 存在：更新数量
                    UPDATE [Gat_Nike_Move_OC]
                    SET [Qty] = ([Qty] + @Qty),
                        [ModifyTime] = GETDATE()
                    WHERE [SolutionID] = @SolutionId
                          AND [CompanyID] = @CompanyID
                          AND [BillNo] = @BillNo
                          AND [BarCode] = @BarCode
                          AND BillTypeID = @BillTypeID;

                    -- 删除中转数据
                    DELETE FROM Gat_Nike_Move_Transfer_OC
                    WHERE [TransID] = @TransID;
                END;
                ELSE
                BEGIN
                    -- 不存在：插入新数据
                    INSERT INTO [dbo].[Gat_Nike_Move_OC]
                    (
                        SolutionID,
                        CompanyID,
                        CompanyName,
                        CardID,
                        CardName,
                        BillNo,
                        BillDate,
                        MaterialCode,
                        MaterialName,
                        BarCode,
                        SizeCode,
                        SizeName,
                        OutStockCode,
                        OutStockName,
                        InStockCode,
                        InStockName,
                        Qty,
                        MovementType,
                        StockTransferStatus,
                        ModifyTime,
                        IsImport,
                        BillTypeID,
                        BillTypeName
                    )
                    SELECT a.SolutionID,
                           a.CompanyID,
                           a.CompanyName,
                           a.CardID,
                           a.CardName,
                           a.BillNo,
                           a.BillDate,
                           a.MaterialCode,
                           a.MaterialName,
                           a.BarCode,
                           a.SizeCode,
                           a.SizeName,
                           a.OutStockCode,
                           a.OutStockName,
                           a.InStockCode,
                           a.InStockName,
                           a.Qty,
                           a.MovementType,
                           a.StockTransferStatus,
                           GETDATE() AS ModifyTime,
                           1 AS IsImport,
                           a.BillTypeID,
                           a.BillTypeName
                    FROM [dbo].[Gat_Nike_Move_Transfer_OC] a
                    WHERE [TransID] = @TransID;

                    -- 删除中转数据
                    DELETE FROM Gat_Nike_Move_Transfer_OC
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
END;
GO
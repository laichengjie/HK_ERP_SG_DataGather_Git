IF EXISTS (SELECT *
           FROM sys.objects
           WHERE object_id = OBJECT_ID(N'[dbo].[sp_SGIS_ExecBatch_OC_Nike]')
                 AND type IN (N'P', N'PC'))
    DROP PROCEDURE [dbo].[sp_SGIS_ExecBatch_OC_Nike];
GO

/* ==========================================================================================
描    述：Nike全渠道POS数据
创 建 人：lcj
创建日期：
说明：
更新说明：
-- ==========================================================================================*/
CREATE PROCEDURE [dbo].[sp_SGIS_ExecBatch_OC_Nike]
    @VendorCode NVARCHAR(50),     -- 供应商代码
    @GatType INT = 1,             -- 采集类型，1：正常采集；0：补采
    @CompanyID NVARCHAR(20) = '', -- 公司ID，补采的时候使用
    @CardID NVARCHAR(20) = '',    -- 品牌ID，补采的时候使用
    @StartTime DATETIME,          -- 补采时开始时间，补采的时候使用
    @EndTime DATETIME,            -- 补采结束时间，补采的时候使用
    @IsTB INT = 0                 -- 是否月结数据
AS
BEGIN
    DECLARE @DateNow DATETIME = GETDATE();          -- 本次进行数据捞取的时间
    DECLARE @LogScript NVARCHAR(MAX);               -- 写入日志的脚本
    DECLARE @VendorID INT;                          -- 供应商ID
    DECLARE @ReportMap TINYINT = 0;                 -- 关联的报表
    DECLARE @Error NVARCHAR(MAX);                   -- 错误信息
    DECLARE @SolutionCount INT = 0;                 -- 提报方案计数
    DECLARE @SolutionIndex INT = 1;                 -- 提报方案循环索引
    DECLARE @SolutionID INT = -1;                   -- 提报方案ID
    DECLARE @Remark NVARCHAR(50);                   -- 写入日志的脚本
	DECLARE @RepeatStartTime DATETIME = GETDATE(); --开始同步时间

    CREATE TABLE #Solution
    ( -- 解决方案
        ID INT IDENTITY(1, 1),
        SolutionID INT NOT NULL,
        CompanyID NVARCHAR(20),
        CardID NVARCHAR(20),
		RepeatStartTime DATETIME
    );

    -- 初始化供应商ID和供应商关联的报表
    SELECT TOP 1
        @VendorID = ID,
        @ReportMap = ReportPath
    FROM HK_ERP_SG_DataGather.dbo.Bas_Vendor
    WHERE VendorCode = @VendorCode;

    -- ============================================================
    -- 取解决方案
    -- ============================================================
    IF ISNULL(@CompanyID, '') = ''
    BEGIN
        INSERT INTO #Solution
        (
            SolutionID,
            CompanyID,
            CardID,
			RepeatStartTime
        )
        SELECT DISTINCT
            ID,
            CompanyID,
            CardID,
			ISNULL(RepeatStartTime, GETDATE()) RepeatStartTime
        FROM HK_ERP_SG_DataGather.dbo.Gat_Solution
        WHERE [Enabled] = 1
              AND ISNULL(VendorID, -1) = @VendorID;
    END
    ELSE
    BEGIN
        INSERT INTO #Solution
        (
            SolutionID,
            CompanyID,
            CardID,
			RepeatStartTime
        )
        SELECT DISTINCT
            ID,
            CompanyID,
            CardID,
			ISNULL(RepeatStartTime, GETDATE()) RepeatStartTime
        FROM HK_ERP_SG_DataGather.dbo.Gat_Solution
        WHERE [Enabled] = 1
              AND ISNULL(VendorID, -1) = @VendorID
              AND CardID = @CardID
              AND CompanyID = @CompanyID;
    END

    -- 初始化解决方案计数
    SELECT @SolutionCount = COUNT(*)
    FROM #Solution;

    IF @GatType = 0 -- 补采
    BEGIN
        SET @Remark = N'Nike全渠道补采';
    END
    ELSE
    BEGIN
        SET @Remark = N'Nike全渠道自动采集';
    END

    -- ============================================================
    -- 执行采集
    -- ============================================================
    IF @ReportMap = 25
    BEGIN
        -- 事务明细到品牌，所以事务在循环体内做
        WHILE @SolutionIndex <= @SolutionCount -- 循环每一条解决方案
        BEGIN
            SELECT @CompanyID = CompanyID,
			       @RepeatStartTime = ISNULL(RepeatStartTime, GETDATE()),
                   @CardID = CardID
            FROM #Solution a
            WHERE ID = @SolutionIndex;
			
            -- 自动采集时计算开始时间
            IF @GatType = 1 and @StartTime<='1900-01-01'
            BEGIN
                SET @EndTime = @DateNow;
                IF NOT EXISTS
                (
                    SELECT TOP 1 1
                    FROM HK_ERP_SG_DataGather.dbo.Sys_Log
                    WHERE IsSucceed = 1
                          AND LogType = 1
                          AND ISNULL(VendorID, 0) = @VendorID
                          AND CompanyID = @CompanyID
                          AND CardID = @CardID
                ) -- 在这之前没有插入过数据
                BEGIN
                    SELECT TOP 1
                        @StartTime = RepeatStartTime
                    FROM HK_ERP_SG_DataGather.dbo.Gat_Solution
                    WHERE ISNULL(VendorID, -2) = @VendorID
                          AND CompanyID = @CompanyID
                          AND CardID = @CardID;
                END
                ELSE
                BEGIN
                    SELECT
                        @StartTime = MAX(ModifyDTM)
                    FROM HK_ERP_SG_DataGather.dbo.Sys_Log
                    WHERE IsSucceed = 1
                          AND LogType = 1
                          AND ISNULL(VendorID, -2) = @VendorID
                          AND CompanyID = @CompanyID
                          AND CardID = @CardID; -- 成功提取数据的日志
                END
            END;
			IF @StartTime < @RepeatStartTime
            BEGIN
            SELECT @StartTime = @RepeatStartTime;
            END;
            -- 开始事务
            BEGIN TRY
                BEGIN TRANSACTION ExecBatch;

                -- ============================================================
                -- 构建日志脚本
                -- ============================================================
                SET @LogScript
                    = N'; Exec dbo.sp_SGIS_Nike_GetPosSale_OC ' + CONVERT(NVARCHAR(20), @VendorID) + N','
                      + CONVERT(NVARCHAR(4), @GatType) + N',' + @CompanyID + N',' + @CardID + N','
                      + CONVERT(NVARCHAR(50), @StartTime, 120) + N',' + CONVERT(NVARCHAR(50), @EndTime, 120) + N','
                      + CONVERT(NVARCHAR(50), @DateNow, 120) + N',' + CONVERT(NVARCHAR(4), @IsTB);

                SET @LogScript += N'; Exec dbo.sp_SGIS_Nike_GetMove_OC ' + CONVERT(NVARCHAR(20), @VendorID) + N','
                                  + CONVERT(NVARCHAR(4), @GatType) + N',' + @CompanyID + N',' + @CardID + N','
                                  + CONVERT(NVARCHAR(50), @StartTime, 120) + N','
                                  + CONVERT(NVARCHAR(50), @EndTime, 120) + N','
                                  + CONVERT(NVARCHAR(50), @DateNow, 120) + N',' + CONVERT(NVARCHAR(4), @IsTB);

                SET @LogScript += N'; Exec dbo.sp_SGIS_Nike_GetOutIn_OC ' + CONVERT(NVARCHAR(20), @VendorID) + N','
                                  + CONVERT(NVARCHAR(4), @GatType) + N',' + @CompanyID + N',' + @CardID + N','
                                  + CONVERT(NVARCHAR(50), @StartTime, 120) + N','
                                  + CONVERT(NVARCHAR(50), @EndTime, 120) + N','
                                  + CONVERT(NVARCHAR(50), @DateNow, 120) + N',' + CONVERT(NVARCHAR(4), @IsTB);

                SET @LogScript += N'; Exec dbo.sp_SGIS_Nike_GetMaterailINV_OC ' + CONVERT(NVARCHAR(20), @VendorID)
                                  + N',' + CONVERT(NVARCHAR(4), @GatType) + N',' + @CompanyID + N',' + @CardID
                                  + N',' + CONVERT(NVARCHAR(50), @StartTime, 120) + N','
                                  + CONVERT(NVARCHAR(50), @EndTime, 120) + N','
                                  + CONVERT(NVARCHAR(50), @DateNow, 120) + N',' + CONVERT(NVARCHAR(4), @IsTB);

                -- ============================================================
                -- 开始执行捞取存储过程
                -- ============================================================
                -- 销售
                EXEC dbo.sp_SGIS_Nike_GetPosSale_OC
                    @VendorID = @VendorID,
                    @GatType = @GatType,
                    @CompanyID = @CompanyID,
                    @CardID = @CardID,
                    @StartTime = @StartTime,
                    @EndTime = @EndTime,
                    @DateNow = @DateNow,
                    @IsMC = @IsTB;

                -- 调拨
                EXEC dbo.sp_SGIS_Nike_GetMove_OC
                    @VendorID = @VendorID,
                    @GatType = @GatType,
                    @CompanyID = @CompanyID,
                    @CardID = @CardID,
                    @StartTime = @StartTime,
                    @EndTime = @EndTime,
                    @DateNow = @DateNow,
                    @IsMC = @IsTB;

                -- 出入库
                EXEC dbo.sp_SGIS_Nike_GetOutIn_OC
                    @VendorID = @VendorID,
                    @GatType = @GatType,
                    @CompanyID = @CompanyID,
                    @CardID = @CardID,
                    @StartTime = @StartTime,
                    @EndTime = @EndTime,
                    @DateNow = @DateNow,
                    @IsMC = @IsTB;

                -- 库存
                EXEC dbo.sp_SGIS_Nike_GetMaterailINV_OC
                    @VendorID = @VendorID,
                    @GatType = @GatType,
                    @CompanyID = @CompanyID,
                    @CardID = @CardID,
                    @StartTime = @StartTime,
                    @EndTime = @EndTime,
                    @DateNow = @DateNow,
                    @IsMC = @IsTB;

                -- ============================================================
                -- 写入成功日志
                -- ============================================================
                INSERT INTO HK_ERP_SG_DataGather.dbo.Sys_Log
                (
                    CompanyID,
                    CardID,
                    IsSucceed,
                    Script,
                    ModifyDTM,
                    Remark,
                    LogType,
                    SolutionID,
                    ShopID,
                    VendorID
                )
                VALUES
                (
                    @CompanyID,
                    @CardID,
                    1,
                    @LogScript,
                    @DateNow,
                    @Remark + '成功！',
                    1,
                    NULL,
                    NULL,
                    @VendorID
                );

                SELECT '1' AS Code,
                       'OK' AS [Message];

                COMMIT TRANSACTION ExecBatch;
            END TRY
            BEGIN CATCH
                IF XACT_STATE() <> 0
                    ROLLBACK TRANSACTION;

                SET @Error
                    = N'Error : ' + CONVERT(VARCHAR(50), ERROR_NUMBER()) + N', Severity : '
                      + CONVERT(VARCHAR(5), ERROR_SEVERITY()) + N', State :' + CONVERT(VARCHAR(5), ERROR_STATE())
                      + N', Line :' + CONVERT(VARCHAR(5), ERROR_LINE()) + N', Message : ' + ERROR_MESSAGE();

                -- ============================================================
                -- 写入失败日志
                -- ============================================================
                INSERT INTO HK_ERP_SG_DataGather.dbo.Sys_Log
                (
                    CompanyID,
                    CardID,
                    IsSucceed,
                    Script,
                    ModifyDTM,
                    Remark,
                    LogType,
                    SolutionID,
                    ShopID,
                    VendorID
                )
                VALUES
                (
                    @CompanyID,
                    @CardID,
                    0,
                    @LogScript,
                    @DateNow,
                    @Remark + '失败，详细信息：' + @Error,
                    1,
                    NULL,
                    NULL,
                    @VendorID
                );

                SELECT '0' AS Code,
                       @Error AS [Message];
            END CATCH;

            SET @SolutionIndex = @SolutionIndex + 1; -- 即使某一公司、品牌出错也进行公司、品牌的采集
        END; -- 循环结束
    END;

    -- ============================================================
    -- 清理临时表
    -- ============================================================
    TRUNCATE TABLE #Solution;
    DROP TABLE #Solution;

END;
GO
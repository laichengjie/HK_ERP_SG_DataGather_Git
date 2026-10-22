

IF Exists(select 1 from sys.procedures WHERE name='sp_SGIS_MaterialBalaPrice')
BEGIN 
	DROP PROCEDURE sp_SGIS_MaterialBalaPrice
END 
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* ==========================================================================================
描    述：货号结算导入
创 建 人：lcj
创建日期：2025-04-15
说明：
更新说明：
-- ==========================================================================================*/
Create PROCEDURE [dbo].[sp_SGIS_MaterialBalaPrice]
    @CompanyID VARCHAR(20) = '',
    @MaterialCode VARCHAR(60) = '',
	@BalaPrice numeric(24, 6)=0,
	@TransIDstr NVARCHAR(2000) = '',
    @Type INT = 0 -- 0 导入 1删除 
AS
BEGIN
    DECLARE @TransCount INT = 0;
    DECLARE @TransIndex INT = 1;
    DECLARE @TransID INT = 0;
	
	begin
    --导入
    IF @Type = 0
    BEGIN
        IF  exists(select top 1 1  from dbo.Gat_Material_BalaPrice where CompanyID=@CompanyID and  MaterialCode=@MaterialCode)
        BEGIN
            --更新已存在的
            UPDATE a SET BalaPrice=@BalaPrice, ModifyDtm=GETDATE() from dbo.Gat_Material_BalaPrice a where CompanyID=@CompanyID and  MaterialCode=@MaterialCode
        END;
        ELSE
        BEGIN
		  insert into dbo.Gat_Material_BalaPrice(CompanyID,MaterialCode,BalaPrice,ModifyDtm)
		  values (@CompanyID,@MaterialCode,@BalaPrice,GETDATE())
        END;
    END;
    --删除
    IF @Type = 1
    BEGIN
        IF (@TransIDstr = '') --没传表示保存全部至销售中间库
        BEGIN
            DELETE a FROM dbo.Gat_Material_BalaPrice a
        END;
        ELSE
        BEGIN

            --获取总数量
            SELECT @TransCount = COUNT(1)
            FROM fnSplitStr(@TransIDstr, ',');
            WHILE @TransIndex <= @TransCount -- 循环每一条解决方案	
            BEGIN
                BEGIN TRY
                    BEGIN TRANSACTION ExecBatch;
                    SELECT @TransID = ikey
                    FROM fnSplitStr(@TransIDstr, ',')
                    WHERE iIndex = @TransIndex;

                    DELETE a
                    FROM dbo.Gat_Material_BalaPrice a
                    WHERE a.TransID = @TransID

                    COMMIT TRANSACTION ExecBatch;

                END TRY
                BEGIN CATCH
                    --错误
                    IF XACT_STATE() <> 0
                        ROLLBACK TRANSACTION;
                END CATCH;
                SET @TransIndex = @TransIndex + 1;
            END;
        END;
    END;
	end
END;


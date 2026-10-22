IF OBJECT_ID ('Gat_LiNing_PosSale', 'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[Gat_LiNing_PosSale](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[SolutionId] [INT] NOT NULL,
		[ShopCode] [varchar](20) NOT NULL,
		[BillDate] [datetime] NOT NULL,
		[BillTime] [datetime] NOT NULL,
		[MaterialCode] [varchar](20) NULL,
		[Qty] [numeric](10, 2) NOT NULL,
		[BillNo] [varchar](40) NOT NULL,
		[SaleAmount] [decimal](24, 6) NULL,
		[PersonelName] [varchar](50) NULL,
		[BarCode] [varchar](128) NULL,
		[SizeName] [varchar](50) NULL,
		[CompanyID] [varchar](20) NULL,
		[CompanyName] [varchar](20) NULL,
		[CardID] [varchar](20) NULL,
		[CardName] [varchar](20) NULL,
		[BalaPrice] [numeric](24, 6) NULL,
		[ShopName] [varchar](100) NULL,
		[StockName] [varchar](100) NULL,
		[SmallTicketNo] [nvarchar](40) NULL,
		[PersonnelID] [nvarchar](20) NULL,
	PRIMARY KEY CLUSTERED 
	(
		[id] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

--销售类型
if not exists(select * from syscolumns where id=object_id('Gat_LiNing_PosSale') and name='SaleType')
begin
    alter table Gat_LiNing_PosSale
    add SaleType NVARCHAR(50) not null default('')
end
--会员手机号
if not exists(select * from syscolumns where id=object_id('Gat_LiNing_PosSale') and name='MemberMobile')
begin
    alter table Gat_LiNing_PosSale
    add MemberMobile NVARCHAR(50) not null default('')
end
go

Create PROC [dbo].[sp_SGIS_LiNing_GetPosSale]
	@VendorID INT = -1 ,			--供应商ID
	@GatType Int = 1,				--采集类型，1：正常采集；0：补采
	@CompanyID NVARCHAR(20) = '',	--公司ID 
	@CardID NVARCHAR(20) = '',		-- 品牌ID 
	@StartTime  DateTime ,			-- 补采时开始时间
	@EndTime DateTime ,				--补采结束时间
	@DateNow DateTime,				--当前开始采集的时间
	@SolutionID int
AS
BEGIN
	
	Create Table #Shop( ShopID Nvarchar(50),tradeStoreCode NVarchar(50) ) --提报店铺
	Create Table #VendShop(   -- 供应商店铺
		VendShopID INT Not Null, --
		ShopCode VARCHAR(50) Null,
		ShopName VARCHAR(100) Null,
		StockCode VARCHAR(50) Null,
		StockName VARCHAR(100) Null,
	)
	Create Table #ShopMap(	-- 店铺关系1
		VendShopID INT Not Null,
		ShopID Varchar(20) Not Null,
		CompanyID Varchar(20)
	)
	
	Declare @Start DATETIME = GETDATE() --本次捞取数据的开始时间
	Declare @End DATETIME = GETDATE()   --本次捞取数据的结束时间
	Declare @HisDays Int = 60	-- 历史数据保留60天
	
	-- 供应商店铺
	Insert Into	#VendShop
	Select		ID,ShopCode,ShopName,StockCode,StockName 
	From		HK_ERP_SG_DataGather_XAJX.dbo.Bas_VendShop 
	Where		VendorID = @VendorID
		
	-- 店铺映射关系
	Insert Into  #ShopMap
	Select		 VendShopID,ShopID,CompanyID 
	From		 HK_ERP_SG_DataGather_XAJX.dbo.Bas_ShopMap a
	Inner Join	HK_ERP_SG_DataGather_XAJX.dbo.Bas_VendShop b On a.VendShopID = b.ID
	Where		b.VendorID = @VendorID
	
	Delete From #Shop --清空上一次循环的数据（取店铺）
	Insert Into #Shop(ShopID,tradeStoreCode)
	Select Distinct ShopID,TradeCode
	From			HK_ERP_SG_DataGather_XAJX.dbo.Gat_ShopRight
	Where			AllowUsed = 1 And CompanyID = @CompanyID And CardID = @CardID And ISNULL(VendorID,-2) = @VendorID

	If @GatType = 0 --补采
	Begin
		Set @Start = @StartTime
		Set @End = @EndTime
	End
	Else
	Begin
		Set @End = @DateNow
		If Not Exists (
				Select Top 1 1 From HK_ERP_SG_DataGather_XAJX.dbo.Sys_Log  Where IsSucceed = 1 And LogType = 1 And ISNULL(VendorID,0) = @VendorID AND CompanyID = @CompanyID AND CardID = @CardID
			) --在这之前没有插入过数据
			Begin
				Select Top 1 @Start = RepeatStartTime 
				From	HK_ERP_SG_DataGather_XAJX.dbo.Gat_Solution 
				Where	IsNULL(VendorID,-2) = @VendorID And CompanyID = @CompanyID And CardID = @CardID
			End
		Else
		Begin
			Select  @Start = MAX(ModifyDTM) 
			From	HK_ERP_SG_DataGather_XAJX.dbo.Sys_Log 
			Where	IsSucceed = 1 And LogType = 1 AND IsNULL(VendorID,-2) = @VendorID And CompanyID = @CompanyID AND CardID = @CardID--成功提取数据的日志
		End
	End	
	
	--删除指定范围之内的数据，防止补采产生重复数据
	Delete From HK_ERP_SG_DataGather_XAJX.dbo.Gat_LiNing_PosSale 
	Where CompanyID=@CompanyID And CardID = @CardID And	BillDate >@Start And BillDate <=@End	
	--删除历史数据
	Delete From HK_ERP_SG_DataGather_XAJX.dbo.Gat_LiNing_PosSale 
	Where CompanyID=@CompanyID And CardID = @CardID And	BillDate <= DATEADD(DAY,@HisDays * -1,CONVERT(varchar(10),GETDATE(),120))
	-- SELECT * FROM HK_ERP_SG_DataGather_XAJX.dbo.Gat_LiNing_PosSale

	-- 开始插入数据
	Insert Into HK_ERP_SG_DataGather_XAJX.dbo.Gat_LiNing_PosSale(SolutionID,ShopCode,ShopName,BillNo,BillDate,BillTime,MaterialCode,SizeName,
	BarCode,qty,SaleAmount,BalaPrice,CompanyID,CompanyName,CardID,CardName,PersonnelID,PersonelName)
	Select		@SolutionID,m.tradeStoreCode,d.ShopName,b.BillNo,b.BillDate,b.BillTime,c.MaterialCode,s.SizeName,s.BarCode,
				a.Qty,a.PayAmount,a.BalaPrice,k.CompanyID,k.CompanyName,c.CardID,l.CardName,b.PersonnelID,pl.PersonnelName
	FROM		SD_Pos_SaleDetail a
	Inner Join	SD_Pos_SaleMaster b On a.CompanyID = b.CompanyID And a.BillNo = b.BillNo
	Inner Join	SD_Mat_Material c On a.MaterialID  = c.MaterialID
	--Inner Join  SD_Mat_MaterialSize s1 ON s1.MaterialID=c.MaterialID
	Inner JOin	Bas_Shop d On a.ShopID = d.ShopID And a.CompanyID = d.CompanyID
	Inner JOin	SD_Bas_ShopCard e On d.CompanyID = e.CompanyID And d.ShopID = e.ShopID AND c.CardID = e.CardID
	Inner Join  #Shop m On a.ShopID = m.ShopID
	Inner Join	SD_Mat_MaterialParam h On a.CompanyID = h.CompanyID And c.MaterialID = h.MaterialID
	Inner Join	Bas_Stock  g On a.CompanyID = g.CompanyID And d.StockID = g.StockID
	Left JOIN  dbo.Bas_Unit z ON c.UnitID=z.UnitID
	Left Join	Bas_Company k On a.CompanyID = k.CompanyID
	Left Join	SD_Mat_Card l On c.CardID = l.CardID
	Left Join   SD_Mat_Size s On a.SizeID = s.SizeID 
	left join Bas_Personnel pl on b.PersonnelID=pl.PersonnelID and b.CompanyID=pl.CompanyID
	Left Join	#ShopMap i On a.CompanyID = i.CompanyID And a.ShopID = i.ShopID -- 关联到店铺映射
	Left Join	#VendShop j On i.VendShopID = j.VendShopID
	Where		a.CompanyID = @CompanyID And e.CardID = @CardID And b.BillTime >@Start And b.BillTime <=@End
	Union All
	Select		@SolutionID,m.tradeStoreCode,d.ShopName,b.BillNo,b.BillDate,b.BillTime,c.MaterialCode,s.SizeName,s.BarCode,
				a.Qty,a.PayAmount,a.BalaPrice,k.CompanyID,k.CompanyName,c.CardID,l.CardName,b.PersonnelID,pl.PersonnelName
	FROM		SD_Pos_SaleDetail2 a
	Inner Join	SD_Pos_SaleMaster2 b On a.CompanyID = b.CompanyID And a.BillNo = b.BillNo
	Inner Join	SD_Mat_Material c On a.MaterialID  = c.MaterialID
	--Inner Join  SD_Mat_MaterialSize s1 ON s1.MaterialID=c.MaterialID
	Inner JOin	Bas_Shop d On a.ShopID = d.ShopID And a.CompanyID = d.CompanyID
	Inner JOin	SD_Bas_ShopCard e On d.CompanyID = e.CompanyID And d.ShopID = e.ShopID AND c.CardID = e.CardID
	Inner Join  #Shop m On a.ShopID = m.ShopID
	Inner Join	SD_Mat_MaterialParam h On a.CompanyID = h.CompanyID And c.MaterialID = h.MaterialID
	Inner Join	Bas_Stock  g On a.CompanyID = g.CompanyID And d.StockID = g.StockID
	Left JOIN  dbo.Bas_Unit z ON c.UnitID=z.UnitID
	Left Join	Bas_Company k On a.CompanyID = k.CompanyID
	Left Join	SD_Mat_Card l On c.CardID = l.CardID
	Left Join   SD_Mat_Size s On a.SizeID = s.SizeID 
	left join Bas_Personnel pl on b.PersonnelID=pl.PersonnelID and b.CompanyID=pl.CompanyID
	Left Join	#ShopMap i On a.CompanyID = i.CompanyID And a.ShopID = i.ShopID -- 关联到店铺映射
	Left Join	#VendShop j On i.VendShopID = j.VendShopID
	Where		a.CompanyID = @CompanyID And e.CardID = @CardID And b.BillTime >@Start And b.BillTime <=@End
			  
	
	Truncate Table #Shop
	Truncate Table #VendShop
	Truncate Table #ShopMap
	Drop Table #Shop
	Drop Table #ShopMap
	Drop Table #VendShop

END


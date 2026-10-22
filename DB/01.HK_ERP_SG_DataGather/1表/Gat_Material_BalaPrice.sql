IF NOT Exists(select top 1 * from sysObjects where Id=OBJECT_ID(N'Gat_Material_BalaPrice') and xtype='U')
BEGIN
CREATE TABLE [dbo].[Gat_Material_BalaPrice](
	[TransID] [int] IDENTITY(1,1) NOT NULL,
	[CompanyID] [varchar](20) NOT NULL,
	[MaterialCode] VARCHAR(40) NOT NULL,    --ªı∫≈
	[BalaPrice] [numeric](24, 6) NOT NULL,--Ω·À„º€£©
    [ModifyDtm] [datetime] NOT NULL,
 CONSTRAINT [PK_Gat_Material_BalaPricel] PRIMARY KEY CLUSTERED 
(
	[TransID] ASC,
	[CompanyID] ASC,
	[MaterialCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE Gat_Material_BalaPrice ADD CONSTRAINT UQ_Gat_Material_BalaPrice UNIQUE (CompanyID, MaterialCode);
end



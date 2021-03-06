/****** Object:  ForeignKey [FK_dato_predio_predio]    Script Date: 07/11/2012 04:46:23 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dato_predio_predio]') AND parent_object_id = OBJECT_ID(N'[dbo].[dato_predio]'))
ALTER TABLE [dbo].[dato_predio] DROP CONSTRAINT [FK_dato_predio_predio]
GO
/****** Object:  ForeignKey [FK_detalle_resultado_calculo]    Script Date: 07/11/2012 04:46:23 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_detalle_resultado_calculo]') AND parent_object_id = OBJECT_ID(N'[dbo].[detalle_res]'))
ALTER TABLE [dbo].[detalle_res] DROP CONSTRAINT [FK_detalle_resultado_calculo]
GO
/****** Object:  ForeignKey [FK_predio_municipio]    Script Date: 07/11/2012 04:46:23 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_predio_municipio]') AND parent_object_id = OBJECT_ID(N'[dbo].[predio]'))
ALTER TABLE [dbo].[predio] DROP CONSTRAINT [FK_predio_municipio]
GO
/****** Object:  Default [DF_dato_predio_num_construccion]    Script Date: 07/11/2012 04:46:23 ******/
IF  EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_dato_predio_num_construccion]') AND parent_object_id = OBJECT_ID(N'[dbo].[dato_predio]'))
Begin
ALTER TABLE [dbo].[dato_predio] DROP CONSTRAINT [DF_dato_predio_num_construccion]

End
GO
/****** Object:  View [dbo].[v_estadistica_predio]    Script Date: 07/11/2012 04:46:23 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[v_estadistica_predio]'))
DROP VIEW [dbo].[v_estadistica_predio]
GO
/****** Object:  View [dbo].[v_predio]    Script Date: 07/11/2012 04:46:23 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[v_predio]'))
DROP VIEW [dbo].[v_predio]
GO
/****** Object:  Table [dbo].[dato_predio]    Script Date: 07/11/2012 04:46:23 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[dato_predio]') AND type in (N'U'))
DROP TABLE [dbo].[dato_predio]
GO
/****** Object:  Table [dbo].[detalle_res]    Script Date: 07/11/2012 04:46:23 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[detalle_res]') AND type in (N'U'))
DROP TABLE [dbo].[detalle_res]
GO
/****** Object:  Table [dbo].[predio]    Script Date: 07/11/2012 04:46:23 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[predio]') AND type in (N'U'))
DROP TABLE [dbo].[predio]
GO
/****** Object:  Table [dbo].[resultado]    Script Date: 07/11/2012 04:46:23 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[resultado]') AND type in (N'U'))
DROP TABLE [dbo].[resultado]
GO
/****** Object:  Table [dbo].[ciudad]    Script Date: 07/11/2012 04:46:23 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ciudad]') AND type in (N'U'))
DROP TABLE [dbo].[ciudad]
GO
/****** Object:  Table [dbo].[ciudad]    Script Date: 07/11/2012 04:46:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ciudad]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ciudad](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[codigo] [varchar](50) COLLATE Modern_Spanish_CI_AI NULL,
	[nombre] [varchar](80) COLLATE Modern_Spanish_CI_AI NOT NULL,
	[contacto] [varchar](200) COLLATE Modern_Spanish_CI_AI NULL,
	[script_load] [varchar](100) COLLATE Modern_Spanish_CI_AI NULL,
	[observaciones] [varchar](300) COLLATE Modern_Spanish_CI_AI NULL,
 CONSTRAINT [PK_municipio] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO
/****** Object:  Table [dbo].[resultado]    Script Date: 07/11/2012 04:46:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[resultado]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[resultado](
	[id] [uniqueidentifier] NOT NULL,
	[id_ciudad] [int] NULL,
	[canton] [varchar](100) COLLATE Modern_Spanish_CI_AI NULL,
	[tipo_item] [varchar](50) COLLATE Modern_Spanish_CI_AI NULL,
	[id_item] [int] NULL,
	[proceso] [varchar](100) COLLATE Modern_Spanish_CI_AI NULL,
	[nombre] [varchar](200) COLLATE Modern_Spanish_CI_AI NULL,
	[codigo] [varchar](50) COLLATE Modern_Spanish_CI_AI NULL,
	[codigo2] [varchar](50) COLLATE Modern_Spanish_CI_AI NULL,
	[codigo3] [varchar](50) COLLATE Modern_Spanish_CI_AI NULL,
	[indice] [varchar](30) COLLATE Modern_Spanish_CI_AI NULL,
	[informacion] [varchar](200) COLLATE Modern_Spanish_CI_AI NULL,
	[completo] [float] NULL,
	[numEvaluados] [int] NULL,
	[numComponentes] [int] NULL,
	[indicador1] [float] NULL,
	[indicador2] [float] NULL,
	[indicador3] [float] NULL,
	[indicador4] [float] NULL,
	[indicador5] [float] NULL,
	[indicador6] [float] NULL,
	[fecha] [datetime] NULL,
	[serializado] [varchar](3000) COLLATE Modern_Spanish_CI_AI NULL,
	[area] [float] NULL,
 CONSTRAINT [PK_resultado] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[resultado]') AND name = N'IX_resultado_codigos')
CREATE NONCLUSTERED INDEX [IX_resultado_codigos] ON [dbo].[resultado] 
(
	[canton] ASC,
	[tipo_item] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = ON, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
GO
/****** Object:  Table [dbo].[predio]    Script Date: 07/11/2012 04:46:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[predio]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[predio](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_ciudad] [int] NOT NULL,
	[clave] [varchar](50) COLLATE Modern_Spanish_CI_AI NULL,
	[c_zona] [varchar](50) COLLATE Modern_Spanish_CI_AI NULL,
	[c_sector] [varchar](50) COLLATE Modern_Spanish_CI_AI NULL,
	[c_manzana] [varchar](50) COLLATE Modern_Spanish_CI_AI NULL,
	[c_predio] [varchar](50) COLLATE Modern_Spanish_CI_AI NULL,
	[c_horizontal] [varchar](50) COLLATE Modern_Spanish_CI_AI NULL,
	[sitio] [varchar](200) COLLATE Modern_Spanish_CI_AI NULL,
	[calle] [varchar](200) COLLATE Modern_Spanish_CI_AI NULL,
	[numero] [varchar](20) COLLATE Modern_Spanish_CI_AI NULL,
	[nombres] [varchar](200) COLLATE Modern_Spanish_CI_AI NULL,
	[ruc_ci] [varchar](20) COLLATE Modern_Spanish_CI_AI NULL,
	[nombreciudad] [varchar](100) COLLATE Modern_Spanish_CI_AI NULL,
	[direccion] [varchar](200) COLLATE Modern_Spanish_CI_AI NULL,
	[telefonos] [varchar](80) COLLATE Modern_Spanish_CI_AI NULL,
	[area_total] [float] NULL,
	[area_construccion] [float] NULL,
	[fr_princ] [float] NULL,
	[dominio] [varchar](80) COLLATE Modern_Spanish_CI_AI NULL,
	[traslacion_dominio] [varchar](80) COLLATE Modern_Spanish_CI_AI NULL,
	[ocupacion] [varchar](80) COLLATE Modern_Spanish_CI_AI NULL,
	[suelo] [varchar](80) COLLATE Modern_Spanish_CI_AI NULL,
	[topologia] [varchar](80) COLLATE Modern_Spanish_CI_AI NULL,
	[localizacion] [varchar](80) COLLATE Modern_Spanish_CI_AI NULL,
	[forma] [varchar](80) COLLATE Modern_Spanish_CI_AI NULL,
	[vias_u] [varchar](80) COLLATE Modern_Spanish_CI_AI NULL,
	[vias_m] [varchar](80) COLLATE Modern_Spanish_CI_AI NULL,
	[electricidad] [varchar](80) COLLATE Modern_Spanish_CI_AI NULL,
	[agua] [varchar](80) COLLATE Modern_Spanish_CI_AI NULL,
	[alcantarillado] [varchar](80) COLLATE Modern_Spanish_CI_AI NULL,
	[otras_inf] [varchar](80) COLLATE Modern_Spanish_CI_AI NULL,
	[estado] [varchar](50) COLLATE Modern_Spanish_CI_AI NULL,
 CONSTRAINT [PK_predio] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[predio]') AND name = N'ix_predio')
CREATE NONCLUSTERED INDEX [ix_predio] ON [dbo].[predio] 
(
	[id_ciudad] ASC,
	[clave] ASC,
	[c_manzana] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'predio', N'COLUMN',N'fr_princ'))
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'por determinar nombre' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'predio', @level2type=N'COLUMN',@level2name=N'fr_princ'
GO
/****** Object:  Table [dbo].[detalle_res]    Script Date: 07/11/2012 04:46:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[detalle_res]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[detalle_res](
	[id] [uniqueidentifier] NOT NULL,
	[id_padre] [uniqueidentifier] NOT NULL,
	[id_componente] [varchar](100) COLLATE Modern_Spanish_CI_AI NULL,
	[es_base] [bit] NULL,
	[contexto] [varchar](100) COLLATE Modern_Spanish_CI_AI NULL,
	[variable] [varchar](100) COLLATE Modern_Spanish_CI_AI NULL,
	[valor] [varchar](100) COLLATE Modern_Spanish_CI_AI NULL,
	[valor_numerico] [decimal](18, 4) NULL,
	[calculado] [decimal](18, 4) NULL,
 CONSTRAINT [PK_detalle_res] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[detalle_res]') AND name = N'IX_detalle_res')
CREATE NONCLUSTERED INDEX [IX_detalle_res] ON [dbo].[detalle_res] 
(
	[contexto] ASC,
	[variable] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = ON, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
GO
/****** Object:  Table [dbo].[dato_predio]    Script Date: 07/11/2012 04:46:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[dato_predio]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[dato_predio](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[id_predio] [int] NOT NULL,
	[num_construccion] [int] NULL,
	[clave] [varchar](80) COLLATE Modern_Spanish_CI_AI NOT NULL,
	[valor_texto] [varchar](80) COLLATE Modern_Spanish_CI_AI NULL,
	[valor_numero] [float] NULL,
 CONSTRAINT [PK_dato_predio] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[dato_predio]') AND name = N'ix_dato_predio')
CREATE NONCLUSTERED INDEX [ix_dato_predio] ON [dbo].[dato_predio] 
(
	[id_predio] ASC,
	[num_construccion] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
GO
/****** Object:  View [dbo].[v_predio]    Script Date: 07/11/2012 04:46:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[v_predio]'))
EXEC dbo.sp_executesql @statement = N'create view [dbo].[v_predio] as
select p.*, d.id as id_dato, d.num_construccion, d.clave as clave_dato, d.valor_texto, d.valor_numero
from predio p
left join dato_predio d on p.id = d.id_predio'
GO
/****** Object:  View [dbo].[v_estadistica_predio]    Script Date: 07/11/2012 04:46:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[v_estadistica_predio]'))
EXEC dbo.sp_executesql @statement = N'create view [dbo].[v_estadistica_predio] as
select c.nombre, p.id_ciudad, count(p.id_ciudad) as predios
from predio p join ciudad c on p.id_ciudad = c.id
group by p.id_ciudad, c.nombre'
GO
/****** Object:  Default [DF_dato_predio_num_construccion]    Script Date: 07/11/2012 04:46:23 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_dato_predio_num_construccion]') AND parent_object_id = OBJECT_ID(N'[dbo].[dato_predio]'))
Begin
ALTER TABLE [dbo].[dato_predio] ADD  CONSTRAINT [DF_dato_predio_num_construccion]  DEFAULT (1) FOR [num_construccion]

End
GO
/****** Object:  ForeignKey [FK_dato_predio_predio]    Script Date: 07/11/2012 04:46:23 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_dato_predio_predio]') AND parent_object_id = OBJECT_ID(N'[dbo].[dato_predio]'))
ALTER TABLE [dbo].[dato_predio]  WITH NOCHECK ADD  CONSTRAINT [FK_dato_predio_predio] FOREIGN KEY([id_predio])
REFERENCES [dbo].[predio] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[dato_predio] CHECK CONSTRAINT [FK_dato_predio_predio]
GO
/****** Object:  ForeignKey [FK_detalle_resultado_calculo]    Script Date: 07/11/2012 04:46:23 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_detalle_resultado_calculo]') AND parent_object_id = OBJECT_ID(N'[dbo].[detalle_res]'))
ALTER TABLE [dbo].[detalle_res]  WITH CHECK ADD  CONSTRAINT [FK_detalle_resultado_calculo] FOREIGN KEY([id_padre])
REFERENCES [dbo].[resultado] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[detalle_res] CHECK CONSTRAINT [FK_detalle_resultado_calculo]
GO
/****** Object:  ForeignKey [FK_predio_municipio]    Script Date: 07/11/2012 04:46:23 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_predio_municipio]') AND parent_object_id = OBJECT_ID(N'[dbo].[predio]'))
ALTER TABLE [dbo].[predio]  WITH NOCHECK ADD  CONSTRAINT [FK_predio_municipio] FOREIGN KEY([id_ciudad])
REFERENCES [dbo].[ciudad] ([id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[predio] CHECK CONSTRAINT [FK_predio_municipio]
GO

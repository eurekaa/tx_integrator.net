USE [TXTemp]
GO
/****** Object:  User [TXUser]    Script Date: 12/14/2011 18:33:41 ******/
CREATE USER [TXUser] FOR LOGIN [TXUser] WITH DEFAULT_SCHEMA=[TX]
GO
/****** Object:  Schema [TX]    Script Date: 12/14/2011 18:33:41 ******/
CREATE SCHEMA [TX] AUTHORIZATION [TXUser]
GO
/****** Object:  Table [TX].[TariffeCarburante]    Script Date: 12/14/2011 18:33:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [TX].[TariffeCarburante](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Distributore] [varchar](50) NOT NULL,
	[Nazione] [varchar](2) NOT NULL,
	[DataDa] [datetime] NOT NULL,
	[PrezzoLt] [decimal](8, 3) NOT NULL,
	[Valuta] [varchar](50) NOT NULL,
	[TipoCarburante] [varchar](50) NOT NULL,
 CONSTRAINT [PK_TariffeCarburante2] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [TX].[Spedizioni]    Script Date: 12/14/2011 18:33:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [TX].[Spedizioni](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[KeySpedizione] [char](30) NULL,
	[Ordinamento] [int] NULL,
	[Tipo] [char](3) NULL,
	[Societa] [varchar](2) NULL,
	[Filiale] [varchar](2) NULL,
	[Anno] [decimal](4, 0) NULL,
	[Progressivo] [decimal](13, 0) NULL,
	[Servizio] [char](6) NULL,
	[Reparto] [char](6) NULL,
	[MittenteRagSoc] [char](35) NULL,
	[DestinazioneRagSoc] [char](35) NULL,
	[DestinazioneIndirizzo] [char](35) NULL,
	[DestinazioneCAP] [char](8) NULL,
	[DestinazioneLocalita] [char](35) NULL,
	[DestinazioneProvincia] [char](6) NULL,
	[DestinazioneNazione] [char](3) NULL,
	[Colli] [decimal](7, 0) NULL,
	[Peso] [decimal](17, 4) NULL,
	[Volume] [decimal](17, 4) NULL,
	[Note] [char](255) NULL,
	[DestinazioneGeoLoc] [varchar](max) NULL,
 CONSTRAINT [PK_IDSPE] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [TX].[Log]    Script Date: 12/14/2011 18:33:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [TX].[Log](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[logDate] [datetime] NOT NULL,
	[logLevel] [nvarchar](50) NOT NULL,
	[logger] [nvarchar](255) NOT NULL,
	[logMessage] [varchar](max) NULL,
	[logInfo] [nvarchar](4000) NOT NULL,
 CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [TX].[Viaggi]    Script Date: 12/14/2011 18:33:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [TX].[Viaggi](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[KeyViaggio] [char](30) NULL,
	[Societa] [char](2) NULL,
	[Filiale] [char](2) NULL,
	[Anno] [decimal](4, 0) NULL,
	[Progressivo] [decimal](13, 0) NULL,
	[DataViaggio] [decimal](8, 0) NULL,
	[Reparto] [char](6) NULL,
	[Servizio] [char](6) NULL,
	[CodiceMezzo] [char](6) NOT NULL,
	[CodiceAutista] [char](6) NOT NULL,
	[DestinazioneViaggio] [char](20) NULL,
	[Note] [char](254) NULL,
	[UtenteCompetenza] [char](40) NULL,
	[MailUtenteCompetenza] [char](50) NULL,
	[DataInizio] [datetime2](7) NULL,
	[DataFine] [datetime2](7) NULL,
	[KmInizio] [decimal](10, 3) NULL,
	[KmFine] [decimal](10, 3) NULL,
	[KmViaggio] [decimal](10, 3) NULL,
	[ConsumoLt] [decimal](10, 3) NULL,
	[VelocitaMedia] [decimal](10, 3) NULL,
	[OreGuida] [decimal](10, 3) NULL,
	[LuogoPartenza] [varchar](max) NULL,
	[LuogoArrivo] [varchar](max) NULL,
 CONSTRAINT [PK_Viaggi] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [TX].[TariffeTransiti]    Script Date: 12/14/2011 18:33:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [TX].[TariffeTransiti](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nome] [varchar](50) NOT NULL,
	[Indirizzo] [varchar](200) NOT NULL,
	[Cap] [varchar](10) NULL,
	[Citta] [varchar](50) NOT NULL,
	[Provincia] [varchar](5) NOT NULL,
	[Nazione] [varchar](50) NOT NULL,
	[GeoCoordinate] [varchar](50) NULL,
	[Prezzo] [decimal](8, 3) NOT NULL,
	[Valuta] [varchar](10) NOT NULL,
 CONSTRAINT [PK_TariffeGeopunti] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [TX].[TariffeCarburante_GetTariffa]    Script Date: 12/14/2011 18:33:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [TX].[TariffeCarburante_GetTariffa]
	@DataDa datetime = NULL,
	@Distributore varchar(50) = NULL,
	@Nazione varchar(50) = NULL,	
	@TipoCarburante varchar(50) = NULL
AS
BEGIN	
	SET NOCOUNT ON;
				
	SELECT * FROM TariffeCarburante 
	WHERE DataDa = ( 
		SELECT MAX(DataDa) FROM TX.TariffeCarburante WHERE DataDa <= @DataDa
		AND UPPER(Distributore) = UPPER(@Distributore)		
		AND UPPER(TipoCarburante) = UPPER(@TipoCarburante)		
		AND UPPER(Nazione) = UPPER(@Nazione)	
	)
	AND UPPER(Distributore) = UPPER(@Distributore)		
	AND UPPER(TipoCarburante) = UPPER(@TipoCarburante)		
	AND UPPER(Nazione) = UPPER(@Nazione)	
	
END
GO
/****** Object:  StoredProcedure [TX].[__Log_Wx]    Script Date: 12/14/2011 18:33:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [TX].[__Log_Wx]
@@ACTION varchar(10) = NULL,
@Id int = NULL,
@logDate datetime = NULL,
@logLevel nvarchar(50) = NULL,
@logger nvarchar(255) = NULL,
@logMessage varchar(MAX) = NULL,
@logInfo nvarchar(4000) = NULL


AS BEGIN 
SET NOCOUNT ON; 

IF @@ACTION = 'INSERT' BEGIN
INSERT INTO [TX].[Log] (logDate,logLevel,logger,logMessage,logInfo) 
OUTPUT INSERTED.Id 
VALUES (@logDate,@logLevel,@logger,@logMessage,@logInfo)
END

ELSE IF @@ACTION = 'UPDATE' BEGIN
UPDATE [TX].[Log] SET 
logDate = @logDate,
logLevel = @logLevel,
logger = @logger,
logMessage = @logMessage,
logInfo = @logInfo 
OUTPUT INSERTED.Id 
WHERE TX.Log.Id = @Id 
END 

ELSE IF @@ACTION = 'DELETE' BEGIN
DELETE FROM TX.Log 
OUTPUT DELETED.Id 
WHERE TX.Log.Id = @Id 
END 

END
GO
/****** Object:  StoredProcedure [TX].[__Log_Rx]    Script Date: 12/14/2011 18:33:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [TX].[__Log_Rx]
@Id int = NULL,
@logDate datetime = NULL,
@logLevel nvarchar(50) = NULL,
@logger nvarchar(255) = NULL,
@logMessage varchar(MAX) = NULL,
@logInfo nvarchar(4000) = NULL 


AS BEGIN 
SET NOCOUNT ON; 

SELECT * FROM TX.Log WHERE 1=1 
AND (Id = @Id OR @Id IS NULL) 
AND (logDate = @logDate OR @logDate IS NULL) 
AND (logLevel = @logLevel OR @logLevel IS NULL) 
AND (logger = @logger OR @logger IS NULL) 
AND (logMessage = @logMessage OR @logMessage IS NULL) 
AND (logInfo = @logInfo OR @logInfo IS NULL) 

END
GO
/****** Object:  StoredProcedure [TX].[__Viaggi_Wx]    Script Date: 12/14/2011 18:33:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [TX].[__Viaggi_Wx]
@@ACTION varchar(10) = NULL,
@Id int = NULL,
@KeyViaggio char(30) = NULL,
@Societa char(2) = NULL,
@Filiale char(2) = NULL,
@Anno decimal = NULL,
@Progressivo decimal = NULL,
@DataViaggio decimal = NULL,
@Reparto char(6) = NULL,
@Servizio char(6) = NULL,
@CodiceMezzo char(6) = NULL,
@CodiceAutista char(6) = NULL,
@DestinazioneViaggio char(20) = NULL,
@Note char(254) = NULL,
@UtenteCompetenza char(40) = NULL,
@MailUtenteCompetenza char(50) = NULL,
@DataInizio datetime2 = NULL,
@DataFine datetime2 = NULL,
@KmInizio decimal = NULL,
@KmFine decimal = NULL,
@KmViaggio decimal = NULL,
@ConsumoLt decimal = NULL,
@VelocitaMedia decimal = NULL,
@OreGuida decimal = NULL,
@LuogoPartenza varchar(MAX) = NULL,
@LuogoArrivo varchar(MAX) = NULL


AS BEGIN 
SET NOCOUNT ON; 

IF @@ACTION = 'INSERT' BEGIN
INSERT INTO [TX].[Viaggi] (KeyViaggio,Societa,Filiale,Anno,Progressivo,DataViaggio,Reparto,Servizio,CodiceMezzo,CodiceAutista,DestinazioneViaggio,Note,UtenteCompetenza,MailUtenteCompetenza,DataInizio,DataFine,KmInizio,KmFine,KmViaggio,ConsumoLt,VelocitaMedia,OreGuida,LuogoPartenza,LuogoArrivo) 
OUTPUT INSERTED.Id 
VALUES (@KeyViaggio,@Societa,@Filiale,@Anno,@Progressivo,@DataViaggio,@Reparto,@Servizio,@CodiceMezzo,@CodiceAutista,@DestinazioneViaggio,@Note,@UtenteCompetenza,@MailUtenteCompetenza,@DataInizio,@DataFine,@KmInizio,@KmFine,@KmViaggio,@ConsumoLt,@VelocitaMedia,@OreGuida,@LuogoPartenza,@LuogoArrivo)
END

ELSE IF @@ACTION = 'UPDATE' BEGIN
UPDATE [TX].[Viaggi] SET 
KeyViaggio = @KeyViaggio,
Societa = @Societa,
Filiale = @Filiale,
Anno = @Anno,
Progressivo = @Progressivo,
DataViaggio = @DataViaggio,
Reparto = @Reparto,
Servizio = @Servizio,
CodiceMezzo = @CodiceMezzo,
CodiceAutista = @CodiceAutista,
DestinazioneViaggio = @DestinazioneViaggio,
Note = @Note,
UtenteCompetenza = @UtenteCompetenza,
MailUtenteCompetenza = @MailUtenteCompetenza,
DataInizio = @DataInizio,
DataFine = @DataFine,
KmInizio = @KmInizio,
KmFine = @KmFine,
KmViaggio = @KmViaggio,
ConsumoLt = @ConsumoLt,
VelocitaMedia = @VelocitaMedia,
OreGuida = @OreGuida,
LuogoPartenza = @LuogoPartenza,
LuogoArrivo = @LuogoArrivo 
OUTPUT INSERTED.Id 
WHERE TX.Viaggi.Id = @Id 
END 

ELSE IF @@ACTION = 'DELETE' BEGIN
DELETE FROM TX.Viaggi 
OUTPUT DELETED.Id 
WHERE TX.Viaggi.Id = @Id 
END 

END
GO
/****** Object:  StoredProcedure [TX].[__Viaggi_Rx]    Script Date: 12/14/2011 18:33:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [TX].[__Viaggi_Rx]
@Id int = NULL,
@KeyViaggio char(30) = NULL,
@Societa char(2) = NULL,
@Filiale char(2) = NULL,
@Anno decimal = NULL,
@Progressivo decimal = NULL,
@DataViaggio decimal = NULL,
@Reparto char(6) = NULL,
@Servizio char(6) = NULL,
@CodiceMezzo char(6) = NULL,
@CodiceAutista char(6) = NULL,
@DestinazioneViaggio char(20) = NULL,
@Note char(254) = NULL,
@UtenteCompetenza char(40) = NULL,
@MailUtenteCompetenza char(50) = NULL,
@DataInizio datetime2 = NULL,
@DataFine datetime2 = NULL,
@KmInizio decimal = NULL,
@KmFine decimal = NULL,
@KmViaggio decimal = NULL,
@ConsumoLt decimal = NULL,
@VelocitaMedia decimal = NULL,
@OreGuida decimal = NULL,
@LuogoPartenza varchar(MAX) = NULL,
@LuogoArrivo varchar(MAX) = NULL 


AS BEGIN 
SET NOCOUNT ON; 

SELECT * FROM TX.Viaggi WHERE 1=1 
AND (Id = @Id OR @Id IS NULL) 
AND (KeyViaggio = @KeyViaggio OR @KeyViaggio IS NULL) 
AND (Societa = @Societa OR @Societa IS NULL) 
AND (Filiale = @Filiale OR @Filiale IS NULL) 
AND (Anno = @Anno OR @Anno IS NULL) 
AND (Progressivo = @Progressivo OR @Progressivo IS NULL) 
AND (DataViaggio = @DataViaggio OR @DataViaggio IS NULL) 
AND (Reparto = @Reparto OR @Reparto IS NULL) 
AND (Servizio = @Servizio OR @Servizio IS NULL) 
AND (CodiceMezzo = @CodiceMezzo OR @CodiceMezzo IS NULL) 
AND (CodiceAutista = @CodiceAutista OR @CodiceAutista IS NULL) 
AND (DestinazioneViaggio = @DestinazioneViaggio OR @DestinazioneViaggio IS NULL) 
AND (Note = @Note OR @Note IS NULL) 
AND (UtenteCompetenza = @UtenteCompetenza OR @UtenteCompetenza IS NULL) 
AND (MailUtenteCompetenza = @MailUtenteCompetenza OR @MailUtenteCompetenza IS NULL) 
AND (DataInizio = @DataInizio OR @DataInizio IS NULL) 
AND (DataFine = @DataFine OR @DataFine IS NULL) 
AND (KmInizio = @KmInizio OR @KmInizio IS NULL) 
AND (KmFine = @KmFine OR @KmFine IS NULL) 
AND (KmViaggio = @KmViaggio OR @KmViaggio IS NULL) 
AND (ConsumoLt = @ConsumoLt OR @ConsumoLt IS NULL) 
AND (VelocitaMedia = @VelocitaMedia OR @VelocitaMedia IS NULL) 
AND (OreGuida = @OreGuida OR @OreGuida IS NULL) 
AND (LuogoPartenza = @LuogoPartenza OR @LuogoPartenza IS NULL) 
AND (LuogoArrivo = @LuogoArrivo OR @LuogoArrivo IS NULL) 

END
GO
/****** Object:  StoredProcedure [TX].[__TariffeTransiti_Wx]    Script Date: 12/14/2011 18:33:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [TX].[__TariffeTransiti_Wx]
@@ACTION varchar(10) = NULL,
@Id int = NULL,
@Nome varchar(50) = NULL,
@Indirizzo varchar(200) = NULL,
@Cap varchar(10) = NULL,
@Citta varchar(50) = NULL,
@Provincia varchar(5) = NULL,
@Nazione varchar(50) = NULL,
@GeoCoordinate varchar(50) = NULL,
@Prezzo decimal = NULL,
@Valuta varchar(10) = NULL


AS BEGIN 
SET NOCOUNT ON; 

IF @@ACTION = 'INSERT' BEGIN
INSERT INTO [TX].[TariffeTransiti] (Nome,Indirizzo,Cap,Citta,Provincia,Nazione,GeoCoordinate,Prezzo,Valuta) 
OUTPUT INSERTED.Id 
VALUES (@Nome,@Indirizzo,@Cap,@Citta,@Provincia,@Nazione,@GeoCoordinate,@Prezzo,@Valuta)
END

ELSE IF @@ACTION = 'UPDATE' BEGIN
UPDATE [TX].[TariffeTransiti] SET 
Nome = @Nome,
Indirizzo = @Indirizzo,
Cap = @Cap,
Citta = @Citta,
Provincia = @Provincia,
Nazione = @Nazione,
GeoCoordinate = @GeoCoordinate,
Prezzo = @Prezzo,
Valuta = @Valuta 
OUTPUT INSERTED.Id 
WHERE TX.TariffeTransiti.Id = @Id 
END 

ELSE IF @@ACTION = 'DELETE' BEGIN
DELETE FROM TX.TariffeTransiti 
OUTPUT DELETED.Id 
WHERE TX.TariffeTransiti.Id = @Id 
END 

END
GO
/****** Object:  StoredProcedure [TX].[__TariffeTransiti_Rx]    Script Date: 12/14/2011 18:33:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [TX].[__TariffeTransiti_Rx]
@Id int = NULL,
@Nome varchar(50) = NULL,
@Indirizzo varchar(200) = NULL,
@Cap varchar(10) = NULL,
@Citta varchar(50) = NULL,
@Provincia varchar(5) = NULL,
@Nazione varchar(50) = NULL,
@GeoCoordinate varchar(50) = NULL,
@Prezzo decimal = NULL,
@Valuta varchar(10) = NULL 


AS BEGIN 
SET NOCOUNT ON; 

SELECT * FROM TX.TariffeTransiti WHERE 1=1 
AND (Id = @Id OR @Id IS NULL) 
AND (Nome = @Nome OR @Nome IS NULL) 
AND (Indirizzo = @Indirizzo OR @Indirizzo IS NULL) 
AND (Cap = @Cap OR @Cap IS NULL) 
AND (Citta = @Citta OR @Citta IS NULL) 
AND (Provincia = @Provincia OR @Provincia IS NULL) 
AND (Nazione = @Nazione OR @Nazione IS NULL) 
AND (GeoCoordinate = @GeoCoordinate OR @GeoCoordinate IS NULL) 
AND (Prezzo = @Prezzo OR @Prezzo IS NULL) 
AND (Valuta = @Valuta OR @Valuta IS NULL) 

END
GO
/****** Object:  StoredProcedure [TX].[__TariffeCarburante_Wx]    Script Date: 12/14/2011 18:33:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [TX].[__TariffeCarburante_Wx]
@@ACTION varchar(10) = NULL,
@Id int = NULL,
@Distributore varchar(50) = NULL,
@Nazione varchar(2) = NULL,
@DataDa datetime = NULL,
@PrezzoLt decimal = NULL,
@Valuta varchar(50) = NULL,
@TipoCarburante varchar(50) = NULL


AS BEGIN 
SET NOCOUNT ON; 

IF @@ACTION = 'INSERT' BEGIN
INSERT INTO [TX].[TariffeCarburante] (Distributore,Nazione,DataDa,PrezzoLt,Valuta,TipoCarburante) 
OUTPUT INSERTED.Id 
VALUES (@Distributore,@Nazione,@DataDa,@PrezzoLt,@Valuta,@TipoCarburante)
END

ELSE IF @@ACTION = 'UPDATE' BEGIN
UPDATE [TX].[TariffeCarburante] SET 
Distributore = @Distributore,
Nazione = @Nazione,
DataDa = @DataDa,
PrezzoLt = @PrezzoLt,
Valuta = @Valuta,
TipoCarburante = @TipoCarburante 
OUTPUT INSERTED.Id 
WHERE TX.TariffeCarburante.Id = @Id 
END 

ELSE IF @@ACTION = 'DELETE' BEGIN
DELETE FROM TX.TariffeCarburante 
OUTPUT DELETED.Id 
WHERE TX.TariffeCarburante.Id = @Id 
END 

END
GO
/****** Object:  StoredProcedure [TX].[__TariffeCarburante_Rx]    Script Date: 12/14/2011 18:33:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [TX].[__TariffeCarburante_Rx]
@Id int = NULL,
@Distributore varchar(50) = NULL,
@Nazione varchar(2) = NULL,
@DataDa datetime = NULL,
@PrezzoLt decimal = NULL,
@Valuta varchar(50) = NULL,
@TipoCarburante varchar(50) = NULL 


AS BEGIN 
SET NOCOUNT ON; 

SELECT * FROM TX.TariffeCarburante WHERE 1=1 
AND (Id = @Id OR @Id IS NULL) 
AND (Distributore = @Distributore OR @Distributore IS NULL) 
AND (Nazione = @Nazione OR @Nazione IS NULL) 
AND (DataDa = @DataDa OR @DataDa IS NULL) 
AND (PrezzoLt = @PrezzoLt OR @PrezzoLt IS NULL) 
AND (Valuta = @Valuta OR @Valuta IS NULL) 
AND (TipoCarburante = @TipoCarburante OR @TipoCarburante IS NULL) 

END
GO
/****** Object:  StoredProcedure [TX].[__Spedizioni_Wx]    Script Date: 12/14/2011 18:33:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [TX].[__Spedizioni_Wx]
@@ACTION varchar(10) = NULL,
@Id int = NULL,
@KeySpedizione char(30) = NULL,
@Ordinamento int = NULL,
@Tipo char(3) = NULL,
@Societa varchar(2) = NULL,
@Filiale varchar(2) = NULL,
@Anno decimal = NULL,
@Progressivo decimal = NULL,
@Servizio char(6) = NULL,
@Reparto char(6) = NULL,
@MittenteRagSoc char(35) = NULL,
@DestinazioneRagSoc char(35) = NULL,
@DestinazioneIndirizzo char(35) = NULL,
@DestinazioneCAP char(8) = NULL,
@DestinazioneLocalita char(35) = NULL,
@DestinazioneProvincia char(6) = NULL,
@DestinazioneNazione char(3) = NULL,
@Colli decimal = NULL,
@Peso decimal = NULL,
@Volume decimal = NULL,
@Note char(255) = NULL,
@DestinazioneGeoLoc varchar(MAX) = NULL


AS BEGIN 
SET NOCOUNT ON; 

IF @@ACTION = 'INSERT' BEGIN
INSERT INTO [TX].[Spedizioni] (KeySpedizione,Ordinamento,Tipo,Societa,Filiale,Anno,Progressivo,Servizio,Reparto,MittenteRagSoc,DestinazioneRagSoc,DestinazioneIndirizzo,DestinazioneCAP,DestinazioneLocalita,DestinazioneProvincia,DestinazioneNazione,Colli,Peso,Volume,Note,DestinazioneGeoLoc) 
OUTPUT INSERTED.Id 
VALUES (@KeySpedizione,@Ordinamento,@Tipo,@Societa,@Filiale,@Anno,@Progressivo,@Servizio,@Reparto,@MittenteRagSoc,@DestinazioneRagSoc,@DestinazioneIndirizzo,@DestinazioneCAP,@DestinazioneLocalita,@DestinazioneProvincia,@DestinazioneNazione,@Colli,@Peso,@Volume,@Note,@DestinazioneGeoLoc)
END

ELSE IF @@ACTION = 'UPDATE' BEGIN
UPDATE [TX].[Spedizioni] SET 
KeySpedizione = @KeySpedizione,
Ordinamento = @Ordinamento,
Tipo = @Tipo,
Societa = @Societa,
Filiale = @Filiale,
Anno = @Anno,
Progressivo = @Progressivo,
Servizio = @Servizio,
Reparto = @Reparto,
MittenteRagSoc = @MittenteRagSoc,
DestinazioneRagSoc = @DestinazioneRagSoc,
DestinazioneIndirizzo = @DestinazioneIndirizzo,
DestinazioneCAP = @DestinazioneCAP,
DestinazioneLocalita = @DestinazioneLocalita,
DestinazioneProvincia = @DestinazioneProvincia,
DestinazioneNazione = @DestinazioneNazione,
Colli = @Colli,
Peso = @Peso,
Volume = @Volume,
Note = @Note,
DestinazioneGeoLoc = @DestinazioneGeoLoc 
OUTPUT INSERTED.Id 
WHERE TX.Spedizioni.Id = @Id 
END 

ELSE IF @@ACTION = 'DELETE' BEGIN
DELETE FROM TX.Spedizioni 
OUTPUT DELETED.Id 
WHERE TX.Spedizioni.Id = @Id 
END 

END
GO
/****** Object:  StoredProcedure [TX].[__Spedizioni_Rx]    Script Date: 12/14/2011 18:33:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [TX].[__Spedizioni_Rx]
@Id int = NULL,
@KeySpedizione char(30) = NULL,
@Ordinamento int = NULL,
@Tipo char(3) = NULL,
@Societa varchar(2) = NULL,
@Filiale varchar(2) = NULL,
@Anno decimal = NULL,
@Progressivo decimal = NULL,
@Servizio char(6) = NULL,
@Reparto char(6) = NULL,
@MittenteRagSoc char(35) = NULL,
@DestinazioneRagSoc char(35) = NULL,
@DestinazioneIndirizzo char(35) = NULL,
@DestinazioneCAP char(8) = NULL,
@DestinazioneLocalita char(35) = NULL,
@DestinazioneProvincia char(6) = NULL,
@DestinazioneNazione char(3) = NULL,
@Colli decimal = NULL,
@Peso decimal = NULL,
@Volume decimal = NULL,
@Note char(255) = NULL,
@DestinazioneGeoLoc varchar(MAX) = NULL 


AS BEGIN 
SET NOCOUNT ON; 

SELECT * FROM TX.Spedizioni WHERE 1=1 
AND (Id = @Id OR @Id IS NULL) 
AND (KeySpedizione = @KeySpedizione OR @KeySpedizione IS NULL) 
AND (Ordinamento = @Ordinamento OR @Ordinamento IS NULL) 
AND (Tipo = @Tipo OR @Tipo IS NULL) 
AND (Societa = @Societa OR @Societa IS NULL) 
AND (Filiale = @Filiale OR @Filiale IS NULL) 
AND (Anno = @Anno OR @Anno IS NULL) 
AND (Progressivo = @Progressivo OR @Progressivo IS NULL) 
AND (Servizio = @Servizio OR @Servizio IS NULL) 
AND (Reparto = @Reparto OR @Reparto IS NULL) 
AND (MittenteRagSoc = @MittenteRagSoc OR @MittenteRagSoc IS NULL) 
AND (DestinazioneRagSoc = @DestinazioneRagSoc OR @DestinazioneRagSoc IS NULL) 
AND (DestinazioneIndirizzo = @DestinazioneIndirizzo OR @DestinazioneIndirizzo IS NULL) 
AND (DestinazioneCAP = @DestinazioneCAP OR @DestinazioneCAP IS NULL) 
AND (DestinazioneLocalita = @DestinazioneLocalita OR @DestinazioneLocalita IS NULL) 
AND (DestinazioneProvincia = @DestinazioneProvincia OR @DestinazioneProvincia IS NULL) 
AND (DestinazioneNazione = @DestinazioneNazione OR @DestinazioneNazione IS NULL) 
AND (Colli = @Colli OR @Colli IS NULL) 
AND (Peso = @Peso OR @Peso IS NULL) 
AND (Volume = @Volume OR @Volume IS NULL) 
AND (Note = @Note OR @Note IS NULL) 
AND (DestinazioneGeoLoc = @DestinazioneGeoLoc OR @DestinazioneGeoLoc IS NULL) 

END
GO
/****** Object:  Table [TX].[Pianificazioni]    Script Date: 12/14/2011 18:33:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [TX].[Pianificazioni](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdViaggio] [int] NOT NULL,
	[IdSpedizione] [int] NULL,
	[Stato] [varchar](50) NULL,
	[SyncStato] [varchar](50) NULL,
	[SyncTask] [varchar](50) NULL,
	[SyncData] [datetime2](7) NULL,
 CONSTRAINT [PK_IDPRO] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [TX].[NoteSpese]    Script Date: 12/14/2011 18:33:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [TX].[NoteSpese](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdViaggio] [int] NOT NULL,
	[Data] [datetime2](7) NULL,
	[Tipo] [varchar](50) NULL,
	[Codice] [varchar](50) NULL,
	[Descrizione] [varchar](max) NULL,
	[Indirizzo] [varchar](max) NULL,
	[GeoCoordinate] [varchar](50) NULL,
	[Prezzo] [decimal](8, 3) NULL,
	[Valuta] [varchar](50) NULL,
	[Note] [varchar](max) NULL,
 CONSTRAINT [PK_NoteSpese] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [TX].[Pianificazioni_GetTXTempToSync]    Script Date: 12/14/2011 18:33:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TX].[Pianificazioni_GetTXTempToSync]
	@TipoPianificazione varchar(50) = null,
	@StatoFinished varchar(50) = null,
	@StatoClosed varchar(50) = null,
	@StatoCancelled varchar(50) = null,
	@StatoNotDelivered varchar(50) = null
AS
BEGIN	
	SET NOCOUNT ON;
    IF @TipoPianificazione = 'VIAGGIO' BEGIN    
		SELECT * FROM Pianificazioni 
		WHERE Stato != @StatoClosed
		AND Stato != @StatoFinished
		AND Stato != @StatoCancelled
		AND Stato != @StatoNotDelivered
		AND IdSpedizione IS NULL
	END
	
	ELSE IF @TipoPianificazione = 'SPEDIZIONE' BEGIN    
		SELECT * FROM Pianificazioni 
		WHERE Stato != @StatoFinished
		AND Stato != @StatoCancelled
		AND Stato != @StatoNotDelivered
		AND IdSpedizione IS NOT NULL
	END
			
END
GO
/****** Object:  StoredProcedure [TX].[Pianificazioni_GetTXTempToReport]    Script Date: 12/14/2011 18:33:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TX].[Pianificazioni_GetTXTempToReport]
	@ReportDelay int = null,
	@StatoFinished varchar(50) = null
	
AS
BEGIN	
	SET NOCOUNT ON;
    
	select * from TX.Pianificazioni p where
	p.SyncData <= DATEADD(day, -@ReportDelay, GETDATE())
	and p.Stato = @StatoFinished
	and p.IdSpedizione IS NULL
			
END
GO
/****** Object:  StoredProcedure [TX].[Pianificazioni_GetTXTangoToSync]    Script Date: 12/14/2011 18:33:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TX].[Pianificazioni_GetTXTangoToSync]
	@TipoPianificazione varchar(50) = null,
	@NewSyncStato varchar(50) = null,
	@OldSyncStato varchar(50) = null				
AS
BEGIN	
	SET NOCOUNT ON;
    IF @TipoPianificazione = 'VIAGGIO' BEGIN   
    
		UPDATE Pianificazioni SET 
		SyncStato = @NewSyncStato
		WHERE SyncStato = @OldSyncStato
		AND SyncTask IS NOT NULL 
		AND IdSpedizione IS NULL
     
		SELECT * FROM Pianificazioni
		WHERE SyncStato = @NewSyncStato
		AND IdSpedizione IS NULL
		
	END
	
	ELSE IF @TipoPianificazione = 'SPEDIZIONE' BEGIN    
		UPDATE Pianificazioni SET 
		SyncStato = @NewSyncStato
		WHERE SyncStato = @OldSyncStato
		AND SyncTask IS NOT NULL 
		AND IdSpedizione IS NOT NULL
	
		SELECT * FROM Pianificazioni
		WHERE SyncStato = @NewSyncStato
		AND IdSpedizione IS NOT NULL
	END
			
END
GO
/****** Object:  StoredProcedure [TX].[__Pianificazioni_Wx]    Script Date: 12/14/2011 18:33:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [TX].[__Pianificazioni_Wx]
@@ACTION varchar(10) = NULL,
@Id int = NULL,
@IdViaggio int = NULL,
@IdSpedizione int = NULL,
@Stato varchar(50) = NULL,
@SyncStato varchar(50) = NULL,
@SyncTask varchar(50) = NULL,
@SyncData datetime2 = NULL


AS BEGIN 
SET NOCOUNT ON; 

IF @@ACTION = 'INSERT' BEGIN
INSERT INTO [TX].[Pianificazioni] (IdViaggio,IdSpedizione,Stato,SyncStato,SyncTask,SyncData) 
OUTPUT INSERTED.Id 
VALUES (@IdViaggio,@IdSpedizione,@Stato,@SyncStato,@SyncTask,@SyncData)
END

ELSE IF @@ACTION = 'UPDATE' BEGIN
UPDATE [TX].[Pianificazioni] SET 
IdViaggio = @IdViaggio,
IdSpedizione = @IdSpedizione,
Stato = @Stato,
SyncStato = @SyncStato,
SyncTask = @SyncTask,
SyncData = @SyncData 
OUTPUT INSERTED.Id 
WHERE TX.Pianificazioni.Id = @Id 
END 

ELSE IF @@ACTION = 'DELETE' BEGIN
DELETE FROM TX.Pianificazioni 
OUTPUT DELETED.Id 
WHERE TX.Pianificazioni.Id = @Id 
END 

END
GO
/****** Object:  StoredProcedure [TX].[__Pianificazioni_Rx]    Script Date: 12/14/2011 18:33:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [TX].[__Pianificazioni_Rx]
@Id int = NULL,
@IdViaggio int = NULL,
@IdSpedizione int = NULL,
@Stato varchar(50) = NULL,
@SyncStato varchar(50) = NULL,
@SyncTask varchar(50) = NULL,
@SyncData datetime2 = NULL 


AS BEGIN 
SET NOCOUNT ON; 

SELECT * FROM TX.Pianificazioni WHERE 1=1 
AND (Id = @Id OR @Id IS NULL) 
AND (IdViaggio = @IdViaggio OR @IdViaggio IS NULL) 
AND (IdSpedizione = @IdSpedizione OR @IdSpedizione IS NULL) 
AND (Stato = @Stato OR @Stato IS NULL) 
AND (SyncStato = @SyncStato OR @SyncStato IS NULL) 
AND (SyncTask = @SyncTask OR @SyncTask IS NULL) 
AND (SyncData = @SyncData OR @SyncData IS NULL) 

END
GO
/****** Object:  StoredProcedure [TX].[__NoteSpese_Wx]    Script Date: 12/14/2011 18:33:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [TX].[__NoteSpese_Wx]
@@ACTION varchar(10) = NULL,
@Id int = NULL,
@IdViaggio int = NULL,
@Data datetime2 = NULL,
@Tipo varchar(50) = NULL,
@Codice varchar(50) = NULL,
@Descrizione varchar(MAX) = NULL,
@Indirizzo varchar(MAX) = NULL,
@GeoCoordinate varchar(50) = NULL,
@Prezzo decimal = NULL,
@Valuta varchar(50) = NULL,
@Note varchar(MAX) = NULL


AS BEGIN 
SET NOCOUNT ON; 

IF @@ACTION = 'INSERT' BEGIN
INSERT INTO [TX].[NoteSpese] (IdViaggio,Data,Tipo,Codice,Descrizione,Indirizzo,GeoCoordinate,Prezzo,Valuta,Note) 
OUTPUT INSERTED.Id 
VALUES (@IdViaggio,@Data,@Tipo,@Codice,@Descrizione,@Indirizzo,@GeoCoordinate,@Prezzo,@Valuta,@Note)
END

ELSE IF @@ACTION = 'UPDATE' BEGIN
UPDATE [TX].[NoteSpese] SET 
IdViaggio = @IdViaggio,
Data = @Data,
Tipo = @Tipo,
Codice = @Codice,
Descrizione = @Descrizione,
Indirizzo = @Indirizzo,
GeoCoordinate = @GeoCoordinate,
Prezzo = @Prezzo,
Valuta = @Valuta,
Note = @Note 
OUTPUT INSERTED.Id 
WHERE TX.NoteSpese.Id = @Id 
END 

ELSE IF @@ACTION = 'DELETE' BEGIN
DELETE FROM TX.NoteSpese 
OUTPUT DELETED.Id 
WHERE TX.NoteSpese.Id = @Id 
END 

END
GO
/****** Object:  StoredProcedure [TX].[__NoteSpese_Rx]    Script Date: 12/14/2011 18:33:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [TX].[__NoteSpese_Rx]
@Id int = NULL,
@IdViaggio int = NULL,
@Data datetime2 = NULL,
@Tipo varchar(50) = NULL,
@Codice varchar(50) = NULL,
@Descrizione varchar(MAX) = NULL,
@Indirizzo varchar(MAX) = NULL,
@GeoCoordinate varchar(50) = NULL,
@Prezzo decimal = NULL,
@Valuta varchar(50) = NULL,
@Note varchar(MAX) = NULL 


AS BEGIN 
SET NOCOUNT ON; 

SELECT * FROM TX.NoteSpese WHERE 1=1 
AND (Id = @Id OR @Id IS NULL) 
AND (IdViaggio = @IdViaggio OR @IdViaggio IS NULL) 
AND (Data = @Data OR @Data IS NULL) 
AND (Tipo = @Tipo OR @Tipo IS NULL) 
AND (Codice = @Codice OR @Codice IS NULL) 
AND (Descrizione = @Descrizione OR @Descrizione IS NULL) 
AND (Indirizzo = @Indirizzo OR @Indirizzo IS NULL) 
AND (GeoCoordinate = @GeoCoordinate OR @GeoCoordinate IS NULL) 
AND (Prezzo = @Prezzo OR @Prezzo IS NULL) 
AND (Valuta = @Valuta OR @Valuta IS NULL) 
AND (Note = @Note OR @Note IS NULL) 

END
GO
/****** Object:  Table [TX].[Eventi]    Script Date: 12/14/2011 18:33:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [TX].[Eventi](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdPianificazione] [int] NOT NULL,
	[Stato] [varchar](50) NULL,
	[Data] [datetime2](7) NULL,
	[SyncData] [datetime2](7) NULL,
	[SyncTipo] [varchar](50) NULL,
	[SyncTask] [varchar](50) NULL,
	[SyncStato] [varchar](50) NULL,
	[XmlRequest] [varchar](max) NULL,
	[XmlResponse] [varchar](max) NULL,
	[Note] [varchar](max) NULL,
 CONSTRAINT [PK_Eventi] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  View [TX].[ViewViaggiPianificati]    Script Date: 12/14/2011 18:33:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [TX].[ViewViaggiPianificati]
AS
SELECT     TOP (300) TX.Pianificazioni.Id AS IdPianificazione, TX.Pianificazioni.IdViaggio, TX.Viaggi.KeyViaggio, TX.Viaggi.DataViaggio, TX.Pianificazioni.Stato, 
                      TX.Viaggi.CodiceMezzo, TX.Viaggi.CodiceAutista, TX.Viaggi.DestinazioneViaggio, TX.Viaggi.DataInizio, TX.Viaggi.DataFine, TX.Viaggi.KmInizio, 
                      TX.Viaggi.KmFine, TX.Viaggi.KmViaggio, TX.Viaggi.ConsumoLt, TX.Viaggi.VelocitaMedia, TX.Viaggi.OreGuida
FROM         TX.Pianificazioni INNER JOIN
                      TX.Viaggi ON TX.Pianificazioni.IdViaggio = TX.Viaggi.Id
WHERE     (TX.Pianificazioni.IdSpedizione IS NULL)
ORDER BY IdPianificazione DESC
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Pianificazioni (TX)"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 121
               Right = 213
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Viaggi (TX)"
            Begin Extent = 
               Top = 6
               Left = 251
               Bottom = 202
               Right = 481
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'TX', @level1type=N'VIEW',@level1name=N'ViewViaggiPianificati'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'TX', @level1type=N'VIEW',@level1name=N'ViewViaggiPianificati'
GO
/****** Object:  View [TX].[ViewSpedizioniPianificate]    Script Date: 12/14/2011 18:33:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [TX].[ViewSpedizioniPianificate]
AS
SELECT     TOP (100) PERCENT TX.Pianificazioni.Id AS IdPianificazione, TX.Pianificazioni.IdViaggio, TX.Pianificazioni.IdSpedizione, TX.Pianificazioni.Stato, 
                      TX.Spedizioni.Tipo, TX.Spedizioni.DestinazioneRagSoc, TX.Spedizioni.MittenteRagSoc, TX.Spedizioni.KeySpedizione, 
                      TX.Spedizioni.DestinazioneLocalita
FROM         TX.Pianificazioni INNER JOIN
                      TX.Spedizioni ON TX.Pianificazioni.IdSpedizione = TX.Spedizioni.Id
WHERE     (TX.Pianificazioni.IdSpedizione IS NOT NULL)
ORDER BY IdPianificazione DESC
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[42] 4[19] 2[28] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Pianificazioni (TX)"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 121
               Right = 213
            End
            DisplayFlags = 280
            TopColumn = 2
         End
         Begin Table = "Spedizioni (TX)"
            Begin Extent = 
               Top = 6
               Left = 251
               Bottom = 121
               Right = 436
            End
            DisplayFlags = 280
            TopColumn = 17
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1785
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'TX', @level1type=N'VIEW',@level1name=N'ViewSpedizioniPianificate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'TX', @level1type=N'VIEW',@level1name=N'ViewSpedizioniPianificate'
GO
/****** Object:  StoredProcedure [TX].[__Eventi_Wx]    Script Date: 12/14/2011 18:33:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [TX].[__Eventi_Wx]
@@ACTION varchar(10) = NULL,
@Id int = NULL,
@IdPianificazione int = NULL,
@Stato varchar(50) = NULL,
@Data datetime2 = NULL,
@SyncData datetime2 = NULL,
@SyncTipo varchar(50) = NULL,
@SyncTask varchar(50) = NULL,
@SyncStato varchar(50) = NULL,
@XmlRequest varchar(MAX) = NULL,
@XmlResponse varchar(MAX) = NULL,
@Note varchar(MAX) = NULL


AS BEGIN 
SET NOCOUNT ON; 

IF @@ACTION = 'INSERT' BEGIN
INSERT INTO [TX].[Eventi] (IdPianificazione,Stato,Data,SyncData,SyncTipo,SyncTask,SyncStato,XmlRequest,XmlResponse,Note) 
OUTPUT INSERTED.Id 
VALUES (@IdPianificazione,@Stato,@Data,@SyncData,@SyncTipo,@SyncTask,@SyncStato,@XmlRequest,@XmlResponse,@Note)
END

ELSE IF @@ACTION = 'UPDATE' BEGIN
UPDATE [TX].[Eventi] SET 
IdPianificazione = @IdPianificazione,
Stato = @Stato,
Data = @Data,
SyncData = @SyncData,
SyncTipo = @SyncTipo,
SyncTask = @SyncTask,
SyncStato = @SyncStato,
XmlRequest = @XmlRequest,
XmlResponse = @XmlResponse,
Note = @Note 
OUTPUT INSERTED.Id 
WHERE TX.Eventi.Id = @Id 
END 

ELSE IF @@ACTION = 'DELETE' BEGIN
DELETE FROM TX.Eventi 
OUTPUT DELETED.Id 
WHERE TX.Eventi.Id = @Id 
END 

END
GO
/****** Object:  StoredProcedure [TX].[__Eventi_Rx]    Script Date: 12/14/2011 18:33:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [TX].[__Eventi_Rx]
@Id int = NULL,
@IdPianificazione int = NULL,
@Stato varchar(50) = NULL,
@Data datetime2 = NULL,
@SyncData datetime2 = NULL,
@SyncTipo varchar(50) = NULL,
@SyncTask varchar(50) = NULL,
@SyncStato varchar(50) = NULL,
@XmlRequest varchar(MAX) = NULL,
@XmlResponse varchar(MAX) = NULL,
@Note varchar(MAX) = NULL 


AS BEGIN 
SET NOCOUNT ON; 

SELECT * FROM TX.Eventi WHERE 1=1 
AND (Id = @Id OR @Id IS NULL) 
AND (IdPianificazione = @IdPianificazione OR @IdPianificazione IS NULL) 
AND (Stato = @Stato OR @Stato IS NULL) 
AND (Data = @Data OR @Data IS NULL) 
AND (SyncData = @SyncData OR @SyncData IS NULL) 
AND (SyncTipo = @SyncTipo OR @SyncTipo IS NULL) 
AND (SyncTask = @SyncTask OR @SyncTask IS NULL) 
AND (SyncStato = @SyncStato OR @SyncStato IS NULL) 
AND (XmlRequest = @XmlRequest OR @XmlRequest IS NULL) 
AND (XmlResponse = @XmlResponse OR @XmlResponse IS NULL) 
AND (Note = @Note OR @Note IS NULL) 

END
GO
/****** Object:  StoredProcedure [TX].[__ViewViaggiPianificati_Rx]    Script Date: 12/14/2011 18:33:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [TX].[__ViewViaggiPianificati_Rx]
@IdPianificazione int = NULL,
@IdViaggio int = NULL,
@KeyViaggio char(30) = NULL,
@DataViaggio decimal = NULL,
@Stato varchar(50) = NULL,
@CodiceMezzo char(6) = NULL,
@CodiceAutista char(6) = NULL,
@DestinazioneViaggio char(20) = NULL,
@DataInizio datetime2 = NULL,
@DataFine datetime2 = NULL,
@KmInizio decimal = NULL,
@KmFine decimal = NULL,
@KmViaggio decimal = NULL,
@ConsumoLt decimal = NULL,
@VelocitaMedia decimal = NULL,
@OreGuida decimal = NULL 


AS BEGIN 
SET NOCOUNT ON; 

SELECT * FROM TX.ViewViaggiPianificati WHERE 1=1 
AND (IdPianificazione = @IdPianificazione OR @IdPianificazione IS NULL) 
AND (IdViaggio = @IdViaggio OR @IdViaggio IS NULL) 
AND (KeyViaggio = @KeyViaggio OR @KeyViaggio IS NULL) 
AND (DataViaggio = @DataViaggio OR @DataViaggio IS NULL) 
AND (Stato = @Stato OR @Stato IS NULL) 
AND (CodiceMezzo = @CodiceMezzo OR @CodiceMezzo IS NULL) 
AND (CodiceAutista = @CodiceAutista OR @CodiceAutista IS NULL) 
AND (DestinazioneViaggio = @DestinazioneViaggio OR @DestinazioneViaggio IS NULL) 
AND (DataInizio = @DataInizio OR @DataInizio IS NULL) 
AND (DataFine = @DataFine OR @DataFine IS NULL) 
AND (KmInizio = @KmInizio OR @KmInizio IS NULL) 
AND (KmFine = @KmFine OR @KmFine IS NULL) 
AND (KmViaggio = @KmViaggio OR @KmViaggio IS NULL) 
AND (ConsumoLt = @ConsumoLt OR @ConsumoLt IS NULL) 
AND (VelocitaMedia = @VelocitaMedia OR @VelocitaMedia IS NULL) 
AND (OreGuida = @OreGuida OR @OreGuida IS NULL) 

END
GO
/****** Object:  StoredProcedure [TX].[__ViewSpedizioniPianificate_Rx]    Script Date: 12/14/2011 18:33:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [TX].[__ViewSpedizioniPianificate_Rx]
@IdPianificazione int = NULL,
@IdViaggio int = NULL,
@IdSpedizione int = NULL,
@Stato varchar(50) = NULL,
@Tipo char(3) = NULL,
@DestinazioneRagSoc char(35) = NULL,
@MittenteRagSoc char(35) = NULL,
@KeySpedizione char(30) = NULL,
@DestinazioneLocalita char(35) = NULL 


AS BEGIN 
SET NOCOUNT ON; 

SELECT * FROM TX.ViewSpedizioniPianificate WHERE 1=1 
AND (IdPianificazione = @IdPianificazione OR @IdPianificazione IS NULL) 
AND (IdViaggio = @IdViaggio OR @IdViaggio IS NULL) 
AND (IdSpedizione = @IdSpedizione OR @IdSpedizione IS NULL) 
AND (Stato = @Stato OR @Stato IS NULL) 
AND (Tipo = @Tipo OR @Tipo IS NULL) 
AND (DestinazioneRagSoc = @DestinazioneRagSoc OR @DestinazioneRagSoc IS NULL) 
AND (MittenteRagSoc = @MittenteRagSoc OR @MittenteRagSoc IS NULL) 
AND (KeySpedizione = @KeySpedizione OR @KeySpedizione IS NULL) 
AND (DestinazioneLocalita = @DestinazioneLocalita OR @DestinazioneLocalita IS NULL) 

END
GO
/****** Object:  ForeignKey [FK_Pianificazioni_Spedizioni]    Script Date: 12/14/2011 18:33:49 ******/
ALTER TABLE [TX].[Pianificazioni]  WITH CHECK ADD  CONSTRAINT [FK_Pianificazioni_Spedizioni] FOREIGN KEY([IdSpedizione])
REFERENCES [TX].[Spedizioni] ([Id])
GO
ALTER TABLE [TX].[Pianificazioni] CHECK CONSTRAINT [FK_Pianificazioni_Spedizioni]
GO
/****** Object:  ForeignKey [FK_Pianificazioni_Viaggi]    Script Date: 12/14/2011 18:33:49 ******/
ALTER TABLE [TX].[Pianificazioni]  WITH CHECK ADD  CONSTRAINT [FK_Pianificazioni_Viaggi] FOREIGN KEY([IdViaggio])
REFERENCES [TX].[Viaggi] ([Id])
GO
ALTER TABLE [TX].[Pianificazioni] CHECK CONSTRAINT [FK_Pianificazioni_Viaggi]
GO
/****** Object:  ForeignKey [FK_NoteSpese_Viaggi]    Script Date: 12/14/2011 18:33:49 ******/
ALTER TABLE [TX].[NoteSpese]  WITH CHECK ADD  CONSTRAINT [FK_NoteSpese_Viaggi] FOREIGN KEY([IdViaggio])
REFERENCES [TX].[Viaggi] ([Id])
GO
ALTER TABLE [TX].[NoteSpese] CHECK CONSTRAINT [FK_NoteSpese_Viaggi]
GO
/****** Object:  ForeignKey [FK_Eventi_Pianificazioni]    Script Date: 12/14/2011 18:33:49 ******/
ALTER TABLE [TX].[Eventi]  WITH CHECK ADD  CONSTRAINT [FK_Eventi_Pianificazioni] FOREIGN KEY([IdPianificazione])
REFERENCES [TX].[Pianificazioni] ([Id])
GO
ALTER TABLE [TX].[Eventi] CHECK CONSTRAINT [FK_Eventi_Pianificazioni]
GO

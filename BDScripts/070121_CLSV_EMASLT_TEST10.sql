USE [CLVS_CL_EMASLT_TEST]
GO
/****** Object:  View [dbo].[CLVS_D_EMA_RPT_REPORTS]    Script Date: 7/1/2022 08:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO







CREATE VIEW [dbo].[CLVS_D_EMA_RPT_REPORTS]
AS
SELECT SUBSTRING(T0.ReportPath, 48, 250) AS [Value], 1 AS [Key] FROM Companys T0
UNION
SELECT SUBSTRING(T0.ReportPathQuotation, 48, 250) AS [Value], 2 AS [Key] FROM Companys T0
UNION
SELECT SUBSTRING(T0.ReportPathInventory, 48, 250) AS [Value], 3 AS [Key] FROM Companys T0
UNION
SELECT SUBSTRING(T0.ReportBalance, 48, 250) AS [Value], 4 AS [Key] FROM Companys T0
UNION
SELECT SUBSTRING(T0.ReportPathSO, 48, 250) AS [Value], 5 AS [Key] FROM Companys T0
GO
/****** Object:  View [dbo].[CLVS_D_EMA_SLT_ACCEPTEDMARGINS]    Script Date: 7/1/2022 08:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






CREATE VIEW [dbo].[CLVS_D_EMA_SLT_ACCEPTEDMARGINS] AS

	SELECT
		1 AS Id, '01' AS Code,'ODPN' AS [Name], 'Entrada de marcaderia' AS [Description]
UNION
	SELECT
		2 AS Id, '02' AS Code,'ORPD' AS [Name], 'Devolución de mercaderia' AS [Description]
--UNION
--	SELECT
--		3 AS Id, '03' AS Code,'ORDR' AS [Name], 'Orden de venta' AS [Description] 
--UNION
--	SELECT
--		4 AS Id, '04' AS Code,'OCRD' AS [Name], 'Pagos' AS [Description] 
--UNION
--	SELECT
--		5 AS Id, '05' AS Code,'OPOR' AS [Name], 'Orden de compra' AS [Description]
--UNION
--	SELECT
--		6 AS Id, '06' AS Code,'MEH' AS [Name], 'TEST' AS [Description]
		
GO
/****** Object:  View [dbo].[CLVS_D_EMA_SLT_CURRENCYTYPE]    Script Date: 7/1/2022 08:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[CLVS_D_EMA_SLT_CURRENCYTYPE] AS
SELECT 'USD' Id,	'Dólares' Name,'$'	Symbol
UNION
	SELECT 'COL' Id,	'Colones' Name,'¢'	Symbol	
		
GO
/****** Object:  View [dbo].[CLVS_D_EMA_SLT_SALESMAN]    Script Date: 7/1/2022 08:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[CLVS_D_EMA_SLT_SALESMAN] AS
SELECT 
	US.FullName AS [SlpName],
	SA.SAPUser AS [SlpCode]
FROM USERS US
JOIN UserAssigns SA ON SA.UserId = US.Id
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_CRT_LOG]    Script Date: 7/1/2022 08:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_D_EMA_CRT_LOG]
		   @TypeDocument varchar(max),
		   @Document varchar(max),
		   @StartTimeDocument Datetime
AS
BEGIN
	INSERT INTO [dbo].[Logs]
	       ([TypeDocument]
           ,[Document]
           ,[StartTimeDocument]
           ,[EndTimeDocument]
           ,[ElapsedTimeCreateDocument]
           ,[StartTimeCompany]
           ,[EndTimeCompany]
           ,[ElapsedTimeCompany]
           ,[StartTimeSapDocument]
           ,[EndTimeSapDocument]
           ,[ElapsedTimeSapDocument]
		   )
	VALUES(
		 @TypeDocument,
		   @Document,
		   @StartTimeDocument,
		   NULL,
		   NULL,
		   NULL,
		   NULL,
		   NULL,
		   NULL,
		   NULL,
		   NULL
		   );
	
	SELECT IDENT_CURRENT('Logs') as LogId;
END;
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_CRT_PAYDESKBALANCE]    Script Date: 7/1/2022 08:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CLVS_D_EMA_CRT_PAYDESKBALANCE]
@UserId NVARCHAR(MAX),
@UserSignature INT,
@Cash FLOAT,
@Cards FLOAT,
@CardsPinpad FLOAT,
@Transfer FLOAT,
@CashflowIncomme FLOAT,
@CashflowEgress FLOAT
AS
BEGIN TRY
	BEGIN TRANSACTION;

	INSERT INTO	PaydeskBalances(
		 [UserId],
		 [UserSignature]
		,[CreationDate]
		,[Cash]
		,[Cards]
		,[CardsPinpad]
		,[Transfer]
		,[CashflowIncomme]
		,[CashflowEgress])
	VALUES(
		@UserId,
		@UserSignature,
		GETDATE(),
		@Cash,
		@Cards,
		@CardsPinpad,
		@Transfer,
		@CashflowIncomme,
		@CashflowEgress);

	IF @@TRANCOUNT > 0 COMMIT TRANSACTION;
END TRY
BEGIN CATCH
	IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
	THROW;
END CATCH
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_RPT_REPORTPATH]    Script Date: 7/1/2022 08:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_D_EMA_RPT_REPORTPATH]
@ReportKey INT
AS
SELECT
	(CASE @ReportKey
		WHEN 1 THEN T0.ReportPath
		WHEN 2 THEN T0.ReportPathQuotation
		WHEN 3 THEN T0.ReportPathInventory
		WHEN 4 THEN T0.ReportBalance
		WHEN 5 THEN T0.ReportPathSO
	END) AS [Path]
FROM Companys T0;
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_DBOBJECTNAMES]    Script Date: 7/1/2022 08:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_DBOBJECTNAMES]

AS
SELECT
	*	
FROM 
	[DBObjectNames] 

GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_LOGGEDUSER]    Script Date: 7/1/2022 08:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_LOGGEDUSER]
@Email NVARCHAR(200),
@Password NVARCHAR(200)
AS
SELECT
	T0.Email Email,
	T0.Id UserId,
	T2.WhCode,
	T2.WhName,
	CONCAT('U',T1.Id, 'T', T4.Serie) AS [PrefixId]
FROM 
	Users T0 JOIN UserAssigns T1 ON T0.Id = T1.UserId
	JOIN Stores T2 ON T1.StoreId = T2.Id
	JOIN SeriesByUsers T3 ON T3.UsrMappId = T1.Id
	JOIN Series T4 
		ON T4.Id = T3.SerieId 
		AND T4.DocType = 1 -- Representa el documento invoice
		AND (T4.Type = 1 OR T4.Type = 2) -- Representa la serie de tipo online
WHERE
	T0.Email = @Email AND
	T0.PasswordHash = @Password;
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_OBJECTNAME]    Script Date: 7/1/2022 08:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_OBJECTNAME]
@Name NVARCHAR(200)
AS
SELECT
	T0.[DbObject]	
FROM 
	[DBObjectNames] T0
WHERE
	T0.[Name] = @Name;
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_PAYDESKBALANCE]    Script Date: 7/1/2022 08:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_PAYDESKBALANCE]
@UserId NVARCHAR(MAX),
@CreationDate DATETIME
AS
SELECT TOP 1
	T0.*
FROM
	PaydeskBalances T0
WHERE
	T0.UserId = @UserId
	AND CAST(T0.CreationDate AS DATE) = CAST(@CreationDate AS DATE)
ORDER BY Id DESC;
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_SAP_CREDENTIALS]    Script Date: 7/1/2022 08:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_SAP_CREDENTIALS]
@Id VARCHAR(50)
AS
BEGIN
	SELECT 
		c.DBCode,
		sc.DBUser,
		sc.DBPass,
		sc.[Server],
		sc.DST,
		ua.SAPUser,
		ua.SAPPass,
		sc.DBUser,
		sc.DBPass,
		sc.ODBCType,
		sc.ServerType
	FROM
		[UserAssigns] ua
		JOIN [Companys] c ON ua.CompanyId = c.Id
		JOIN [SapConnections] sc ON c.SapConnectionId = sc.Id
	WHERE
		ua.UserId = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_SERIESBYUSER]    Script Date: 7/1/2022 08:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_SERIESBYUSER]
@UsrMappId int
AS
BEGIN
SELECT  
      T0.Id,
      T0.SerieId,
      T0.UsrMappId ,
      T1.Name,
      T1.DocType AS type   
  FROM SeriesByUsers T0
  INNER JOIN Series T1 ON T0.SerieId =  T1.Id
  --INNER JOIN UserAssigns T2 ON T2.Id = T0.SerieId
 AND T0.UsrMappId = @UsrMappId
END


GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_USERASSINGS]    Script Date: 7/1/2022 08:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_USERASSINGS] 
@CompanyId VARCHAR(max)
AS
BEGIN
SELECT 
	  T0.Id,
      T0.UserId,
      T0.SuperUser,
      T0.SAPUser,
      T0.SAPPass,
      T0.SlpCode,
      T0.StoreId,
      T0.minDiscount,
      T0.CenterCost,
      T0.Active,
      T0.PriceListDef,
      T0.CompanyId,
	  T1.FullName AS UserName,
	  T2.DBCode AS  CompanyName,
	  T3.Name AS StoreName
  FROM UserAssigns T0
  INNER JOIN Users T1 ON T0.UserId = T1.Id
  INNER JOIN Companys T2 ON T2.Id = T0.CompanyId
  INNER JOIN Stores T3 ON T3.Id = T0.StoreId
  WHERE T0.CompanyId = @CompanyId
		
END


GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_UPT_LOG]    Script Date: 7/1/2022 08:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CLVS_D_EMA_UPT_LOG]
	@LogId integer = -1,
	@TypeDocument varchar(max) = '',
	@Document varchar(max) = '',
	@StartTimeDocument Datetime = NULL,
	@EndTimeDocument Datetime = NULL,
	@ElapsedTimeCreateDocument varchar(max) = '',
	@StartTimeCompany Datetime,
	@EndTimeCompany Datetime,
	@ElapsedTimeCompany varchar(max) = '',
	@StartTimeSapDocument Datetime =  NULL,
	@EndTimeSapDocument Datetime = NULL,
	@ElapsedTimeSapDocument varchar(max) = '',
	@ErrorDetail varchar(max) = ''
AS
BEGIN
--Este sp usa iff para ordenar un poco el poder llamar el mismo sp
--sin tener que preocuparse que campo actualizar.
--Solo se actualizara los campos que se le envien en el sp

	UPDATE [dbo].[Logs]
	SET
		TypeDocument =  IIF(@TypeDocument = '', TypeDocument, @TypeDocument),
		
		Document = IIF(@Document = '', Document, @Document),
		
		StartTimeDocument = IIF(@StartTimeDocument IS NULL, StartTimeDocument, @StartTimeDocument),
		
		EndTimeDocument = IIF(@EndTimeDocument IS NULL, EndTimeDocument, @EndTimeDocument),
		
		ElapsedTimeCreateDocument = IIF(@ElapsedTimeCreateDocument = '', ElapsedTimeCreateDocument, @ElapsedTimeCreateDocument),
		
		StartTimeCompany = IIF(@StartTimeCompany IS NULL, StartTimeCompany, @StartTimeCompany),
		
		EndTimeCompany = IIF(@EndTimeCompany IS NULL, EndTimeCompany, @EndTimeCompany),
		
		ElapsedTimeCompany = IIF(@ElapsedTimeCompany = '', ElapsedTimeCompany, @ElapsedTimeCompany),
		
		StartTimeSapDocument = IIF(@StartTimeSapDocument IS NULL, StartTimeSapDocument, @StartTimeSapDocument),
		
		EndTimeSapDocument = IIF(@EndTimeSapDocument IS NULL, EndTimeSapDocument, @EndTimeSapDocument),
		
		ElapsedTimeSapDocument =IIF(@ElapsedTimeSapDocument = '', ElapsedTimeSapDocument, @ElapsedTimeSapDocument),
		
		ErrorDetail = IIF(@ErrorDetail = '', ErrorDetail, @ErrorDetail)
	
	WHERE Logs.Id = @LogId

END;
GO

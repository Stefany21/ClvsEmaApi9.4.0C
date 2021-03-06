USE [TST_CL_SUPERLT10]
GO
/****** Object:  View [dbo].[CLVS_D_EMA_SLT_ACCOUNTS]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[CLVS_D_EMA_SLT_ACCOUNTS] AS
	SELECT 
		CONCAT(CONCAT(T0."Segment_0", '-'), T0."AcctName") AS ACCOUNTNAME,
		T0."AcctCode" AS ACCOUNT
	FROM 
		OACT T0 
	WHERE 
		T0."Finanse" = 'Y';
GO
/****** Object:  View [dbo].[CLVS_D_EMA_SLT_AllPRICELIST]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[CLVS_D_EMA_SLT_AllPRICELIST]
AS
SELECT        ListNum, ListName
FROM            dbo.OPLN
GO
/****** Object:  View [dbo].[CLVS_D_EMA_SLT_BANKS]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[CLVS_D_EMA_SLT_BANKS] AS
SELECT
	BankCodes.BankCode,
	BankCodes.BankName 
FROM
	ODSC BankCodes;
GO
/****** Object:  View [dbo].[CLVS_D_EMA_SLT_BPS_CUSTOMERS]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[CLVS_D_EMA_SLT_BPS_CUSTOMERS]
AS
SELECT
	CardCode, 
	CardName, 
	Currency, 
	'EXE' AS TaxCode, 
	CreditLine - Balance AS Available, 
	CreditLine, 
	Balance, 
	Phone1, 
	Phone2, 
	Fax,
	Cellular,
	E_Mail,
	E_Mail AS MailAddres,
	Discount,
	CUS.ListNum,
	CUS.GroupNum,
	Address, 
    '0' AS Lat, 
	'0' AS Lng,
	QryGroup1,
	QryGroup2 AS EditPrice,
	LicTradNum AS Cedula,
	CntctPrsn AS ContactPerson,
	U_TipoIdentificacion,
	U_provincia, 
	U_canton,
	U_distrito,
	U_barrio,
	U_direccion,
	CASE 
		WHEN UPPER(PymntGroup) = 'CONTADO' THEN 2 
		ELSE 1 END ClienteContado
	--CASE 
	--	WHEN UPPER(PymntGroup) = 'CONTADO' THEN 2 
	--	ELSE 1 END GroupNum
FROM
	dbo.OCRD AS CUS (NOLOCK)
		INNER JOIN dbo.OCTG T1 (NOLOCK) 
			ON CardType = 'C' AND 
				frozenFor = 'N' AND 
				CUS.GroupNum = T1.GroupNum

GO
/****** Object:  View [dbo].[CLVS_D_EMA_SLT_BPS_SUPPLIERS]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE VIEW [dbo].[CLVS_D_EMA_SLT_BPS_SUPPLIERS]
AS
SELECT
	CardCode, 
	CardName, 
	Currency, 
	'EXE' AS TaxCode, 
	CreditLine - Balance AS Available, 
	CreditLine, 
	Balance, 
	Phone1, 
	Phone2, 
	Fax,
	Cellular,
	E_Mail,
	E_Mail AS MailAddres,
	Discount,
	CUS.ListNum,
	CUS.GroupNum,
	Address, 
    '0' AS Lat, 
	'0' AS Lng,
	LicTradNum AS Cedula,
	CntctPrsn AS ContactPerson
FROM
	dbo.OCRD AS CUS (NOLOCK)
		INNER JOIN dbo.OCTG T1 (NOLOCK) 
			ON CardType = 'S' AND 
				frozenFor = 'N' AND 
				CUS.GroupNum = T1.GroupNum;
GO
/****** Object:  View [dbo].[CLVS_D_EMA_SLT_CASHFLOWREASONS]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[CLVS_D_EMA_SLT_CASHFLOWREASONS]
AS
	SELECT 1 Id, 'Pagos realizados' [Name], 1 [Type]
	UNION ALL
	SELECT 2 Id, 'Otros cargos' [Name], 1 [Type]
	UNION ALL
	SELECT 3 Id, 'Descargos' [Name], 1 [Type]
	UNION ALL
	SELECT 4 Id, 'BN Servicios' [Name], 2 [Type];
	
GO
/****** Object:  View [dbo].[CLVS_D_EMA_SLT_CREDITCARDS]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[CLVS_D_EMA_SLT_CREDITCARDS] AS
SELECT 
	CONCAT(CONCAT(CreditCard.CreditCard, ' '), CreditCard.CardName) AS CardName,
	CreditCard.AcctCode 
FROM 
	OCRC AS CreditCard;
GO
/****** Object:  View [dbo].[CLVS_D_EMA_SLT_DISCGROUP]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[CLVS_D_EMA_SLT_DISCGROUP] AS
SELECT 
	DRG.AbsEntry,
	OED.ObjCode CardCode,
	DRG.ObjKey ItemGroup,
	DRG.Discount 
FROM	
	OEDG OED
	JOIN EDG1 DRG
ON
	OED.AbsEntry=DRG.AbsEntry
WHERE
	DRG.DiscType='D'
	AND OED.ObjType=2
	AND DRG.ObjType=52	
GO
/****** Object:  View [dbo].[CLVS_D_EMA_SLT_EXRATE]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[CLVS_D_EMA_SLT_EXRATE] AS
SELECT
	T0.RateDate,
	T0.Rate
FROM
	ORTT T0 
WHERE 
	T0.RateDate = CONVERT(date, GETDATE()) 
	AND Currency = 'USD';
GO
/****** Object:  View [dbo].[CLVS_D_EMA_SLT_FIRMSLIST]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[CLVS_D_EMA_SLT_FIRMSLIST] AS

SELECT FirmCode, FirmName
FROM OMRC
GO
/****** Object:  View [dbo].[CLVS_D_EMA_SLT_INVOICETYPE]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE VIEW [dbo].[CLVS_D_EMA_SLT_INVOICETYPE]
AS
	SELECT
		'' AS [Name]
		,'' AS [Description]
		,CAST(0 AS BIT) AS [IsDefault]
	UNION ALL
	SELECT
		'FE' AS [Name]
		,'Factura electrónica' AS [Description]
		,CAST(0 AS BIT) AS [IsDefault]
	UNION ALL
	SELECT 
		'TE' AS [Name]
		,'Tiquete electrónico' AS [Description]
		,CAST(1 AS BIT) AS [IsDefault]
GO
/****** Object:  View [dbo].[CLVS_D_EMA_SLT_ITEMGROUPLIST]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[CLVS_D_EMA_SLT_ITEMGROUPLIST] AS

SELECT ItmsGrpNam, ItmsGrpCod
FROM OITB 
GO
/****** Object:  View [dbo].[CLVS_D_EMA_SLT_ITEMS]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO









CREATE VIEW [dbo].[CLVS_D_EMA_SLT_ITEMS]
AS
	--SELECT DISTINCT 
 --       T0.ItemCode,
 --       T0.ItemName,
 --       OB.BcdCode AS CodeBars
	--	,0 as Available
 --   FROM           
 --       dbo.OITM AS T0 WITH (NOLOCK)
 --   JOIN
 --       dbo.OITW AS T1 WITH (NOLOCK) ON T0.frozenFor = 'N' AND T0.SellItem = 'Y' AND T1.ItemCode = T0.ItemCode
 --   LEFT JOIN
 --       dbo.OBCD AS OB ON OB.ItemCode = T0.ItemCode
	--UNION
	--SELECT DISTINCT 
 --       T0.ItemCode,
 --       T0.ItemName,
 --       OB.BcdCode AS CodeBars
	--	,0 as Available
 --   FROM           
 --       dbo.OITM AS T0 WITH (NOLOCK)
 --   JOIN
 --       dbo.OITW AS T1 WITH (NOLOCK) ON T0.frozenFor = 'N' AND T0.SellItem = 'Y' AND T1.ItemCode = T0.ItemCode
 --   LEFT JOIN
 --       dbo.OBCD AS OB ON OB.ItemCode = T0.ItemCode
	--	--WHERE OB.BcdCode = '03400500'

	SELECT DISTINCT TOP 40000
        T0.ItemCode,
        T0.ItemName,
        OB.BcdCode AS CodeBars,
		LEN(OB.BcdCode) AS [Len], OB.BcdCode
    FROM           
        dbo.OITM AS T0 WITH (NOLOCK)
    JOIN
        dbo.OITW AS T1 WITH (NOLOCK) ON T0.frozenFor = 'N' AND T0.SellItem = 'Y' AND T1.ItemCode = T0.ItemCode
    LEFT JOIN
        dbo.OBCD AS OB ON OB.ItemCode = T0.ItemCode
	--where ob.BcdCode not in ('340050090')
	ORDER BY LEN(OB.BcdCode)
GO
/****** Object:  View [dbo].[CLVS_D_EMA_SLT_OTCX]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[CLVS_D_EMA_SLT_OTCX]
AS
SELECT LnTaxCode, StrVal1 
FROM OTCX
WHERE BusArea = 0;
GO
/****** Object:  View [dbo].[CLVS_D_EMA_SLT_PAYTERMS]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[CLVS_D_EMA_SLT_PAYTERMS] AS

SELECT GroupNum, 
		PymntGroup,
		CASE --Type: 1-Credito, 2-Contado, se valida de esta manera para identificar el tipo de pago y disparar o no el modal de pago en el UI del App
			WHEN GroupNum = 1 THEN '1'
			ELSE '2'
		END AS Type
FROM OCTG
GO
/****** Object:  View [dbo].[CLVS_D_EMA_SLT_PRICELIST]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE VIEW [dbo].[CLVS_D_EMA_SLT_PRICELIST] AS

SELECT 
	ListNum, ListName
FROM OPLN PL
WHERE PL.ValidFor = 'Y'
GO
/****** Object:  View [dbo].[CLVS_D_EMA_SLT_SALESMAN]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[CLVS_D_EMA_SLT_SALESMAN] AS
SELECT 
	SM.SlpCode,
	SM.SlpName
FROM 
	OSLP SM 
WHERE 
	SM.Active = 'Y'
GO
/****** Object:  View [dbo].[CLVS_D_EMA_SLT_TAXES]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[CLVS_D_EMA_SLT_TAXES] AS
SELECT 
	Code,
	Rate
FROM
	OSTC;
GO
/****** Object:  View [dbo].[CLVS_D_EMA_SLT_UDFCATEGORIES]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
















CREATE    VIEW [dbo].[CLVS_D_EMA_SLT_UDFCATEGORIES]
AS
	SELECT
		'OINV' AS [Name] -- Nombre de la tabla
		,'Facturación' [Description] -- Descripcion para el usuario
		,'DocEntry' [Key] --Debido a que es una busqueda dinamica, la llave del objeto puede cambiar, ejemplo docentry, cardcode, itemcode
	UNION ALL
	SELECT 
		'ORDR' AS [Name]
		,'Orden de venta' AS [Description]
		,'DocEntry' [Key]
		UNION ALL
	SELECT 
		'OQUT' AS [Name]
		,'Oferta de venta' as [Description]
		,'DocEntry' [Key]
		UNION ALL
	SELECT
		'ORCT' AS [Name]
		,'Pagos recibidos' as [Description]
		,'DocEntry' [Key]
	UNION ALL
	SELECT 
		'OCRD' AS [Name]
		,'Socios de negocio' AS [Description]
		,'CardCode' [Key]
	UNION ALL
	SELECT
		'OITM' AS [Name]
		,'Artículos' as [Description]
		,'ItemCode' [Key]
	UNION ALL
	SELECT
		'OIGN' AS [Name]
		,'Entrada inventario' as [Description]
		,'DocEntry' [Key]
	UNION ALL
	SELECT
		'OIGE' AS [Name]
		,'Salida inventario' as [Description]
		,'DocEntry' [Key]
	UNION ALL
	SELECT
		'OPDN' AS [Name]
		,'Entrada mercadería' as [Description]
		,'DocEntry' [Key]
    UNION ALL
	SELECT
		'ORPD' AS [Name]
		,'Salida mercadería' as [Description]
		,'DocEntry' [Key]
    UNION ALL
	SELECT
		'OPOR' AS [Name]
		,'Orden de compra' as [Description]
		,'DocEntry' [Key]
    UNION ALL
	SELECT
		'OPCH' AS [Name]
		,'Factura proveedor' as [Description]
		,'DocEntry' [Key]
		 UNION ALL
	SELECT
		'ORIN' AS [Name]
		,'Nota crédito' as [Description]
		,'DocEntry' [Key]
    UNION ALL
	SELECT
		'OVPM' AS [Name]
		,'Pagos efectuados' as [Description]
		,'DocEntry' [Key]
	
GO
/****** Object:  View [dbo].[CLVS_D_EMA_SLT_UDFDEVELOPMENT]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






CREATE VIEW [dbo].[CLVS_D_EMA_SLT_UDFDEVELOPMENT]
AS
---------------------ESTE REGISTRO NO SE MODIFICA--------------------------
	SELECT
		'BASE_UDF' AS [Name]
		,'' [Description]
---------------------FIN--------------------------
	--UNION ALL
	--SELECT
	--	'RCT3' AS [Name]
	--	,'U_PPPrintags' [Description]
GO
/****** Object:  View [dbo].[CLVS_D_EMA_SLT_WHAREHOUSES]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[CLVS_D_EMA_SLT_WHAREHOUSES] AS

SELECT WH.WhsCode, WH.WhsName
FROM OWHS WH 
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_CRT_INVOICESAPTOPRINT]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_D_EMA_CRT_INVOICESAPTOPRINT]
@DocEntry int
AS
BEGIN
IF((SELECT T0.DocCur FROM OINV T0 WHERE T0.DocEntry = @DocEntry) ='COL')
	IF((SELECT DISTINCT T0.Currency FROM INV1 T0 WHERE T0.DocEntry = @DocEntry)= 'COL')
		BEGIN
			SELECT Header.DocEntry, 
				   Header.DocNum, 
				   bp.U_direccion,  
				   Header.CardCode, 
				   Header.CardName, 
				   Header.DocCur, 
				   Header.DocRate, 
				   Header.DocTotal AS TOTAL, 
				   (select
						case when len(Header.doctime)=3 then
							convert (datetime,
								convert(varchar,( 
									convert(varchar,Header.DocDate,103) + ' '+ (left(Header.DocTime,1)) + ':'+ (right(Header.DocTime,2))) ,114),103)
						when len (Header.DocTime)=4 then
							convert (datetime,
								convert(varchar,( 
									convert(varchar,Header.DocDate,103) + ' '+ (left(Header.DocTime,2)) + ':'+ (right(Header.DocTime,2))) ,114),103)
						when len (Header.DocTime)= 2 then
							convert (datetime,
								convert(varchar,( 
									convert(varchar,Header.DocDate,103) + ' '+ '00' + ':'+ (right(Header.DocTime,2))) ,114),103)
						when len (Header.DocTime)= 1 then
							convert (datetime,
								convert(varchar,( 
									convert(varchar,Header.DocDate,103) + ' '+ '00' + ':'+ (right(Header.DocTime,1))) ,114),103)
				   end) AS DocDate ,
				   Clave.U_NConsecFE AS U_NumFE,
				   Clave.U_NClaveFE,
				   Header.Comments,
				   Header.DocStatus, 
				   Header.NumAtCard, 
				   (select SUM(((line1.VatPrcnt / 100) * (line1.Price * line1.Quantity))) 
						   from INV1 line1 
						   where line1.DocEntry = Header.DocEntry) 
						   AS Taxes,
				   (select SUM(((line1.DiscPrcnt / 100) * (line1.Price * line1.Quantity))) 
						   from INV1 line1 
						   where line1.DocEntry = Header.DocEntry) 
						   AS Discount,
					Lines.ItemCode, 
					Lines.Dscription, 
					Lines.Price, 
					Lines.Quantity, 
					Lines.TaxCode, 
					Lines.DiscPrcnt, 
					Lines.LineTotal AS LINETOTAL  
			FROM OINV Header 
				    JOIN INV1 Lines ON Header.DocEntry = Lines.DocEntry
				    JOIN OCRD bp ON bp.CardCode = Header.CardCode
					LEFT JOIN [@NCLAVEFE] Clave ON CAST(Header.DocEntry as varchar) = CAST(Clave.U_DocEntry AS VARCHAR) and Clave."U_TipoDoc" = 'FE'
			WHERE Header.DocEntry = @DocEntry
		END;
	ELSE
		BEGIN 
			SELECT Header.DocEntry, 
				   Header.DocNum, 
				   bp.U_direccion, 
				   Header.CardCode, 
				   Header.CardName, 
				   Header.DocCur, 
				   Header.DocRate, 
				   Header.DocTotal AS TOTAL, 
				   (select
						case when len(Header.doctime)=3 then
							convert (datetime,
								convert(varchar,( 
									convert(varchar,Header.DocDate,103) + ' '+ (left(Header.DocTime,1)) + ':'+ (right(Header.DocTime,2))) ,114),103)
						when len (Header.DocTime)=4 then
							convert (datetime,
								convert(varchar,( 
									convert(varchar,Header.DocDate,103) + ' '+ (left(Header.DocTime,2)) + ':'+ (right(Header.DocTime,2))) ,114),103)
						when len (Header.DocTime)= 2 then
							convert (datetime,
								convert(varchar,( 
									convert(varchar,Header.DocDate,103) + ' '+ '00' + ':'+ (right(Header.DocTime,2))) ,114),103)
						when len (Header.DocTime)= 1 then
							convert (datetime,
								convert(varchar,( 
									convert(varchar,Header.DocDate,103) + ' '+ '00' + ':'+ (right(Header.DocTime,1))) ,114),103)
					end) AS DocDate ,
				   Clave.U_NConsecFE AS U_NumFE,
				   Clave.U_NClaveFE,
				   Header.Comments,
				   Header.DocStatus, 
				   Header.NumAtCard, 
				   (select SUM(((line1.VatPrcnt / 100) * (line1.Price * line1.Quantity))) 
						   from INV1 line1 
						   where line1.DocEntry = Header.DocEntry)
						   AS Taxes,
					(select SUM(((line1.DiscPrcnt / 100) * (line1.Price * line1.Quantity))) 
						   from INV1 line1 
						   where line1.DocEntry = Header.DocEntry) 
						   AS Discount ,  
					Lines.ItemCode, 
					Lines.Dscription, 
					Lines.Price, 
					Lines.Quantity, 
					Lines.TaxCode, 
					Lines.DiscPrcnt, 
					Lines.TotalSumSy AS LINETOTAL  
			FROM OINV Header 
				    JOIN INV1 Lines ON Header.DocEntry = Lines.DocEntry
				    JOIN OCRD bp ON bp.CardCode = Header.CardCode
					LEFT JOIN [@NCLAVEFE] Clave ON CAST(Header.DocEntry as varchar) = CAST(Clave.U_DocEntry AS VARCHAR) and Clave."U_TipoDoc" = 'FE'
			WHERE Header.DocEntry = @DocEntry
		END;
ELSE
	IF((SELECT DISTINCT T0.Currency FROM INV1 T0 WHERE T0.DocEntry = @DocEntry)= 'COL')
		BEGIN
			SELECT Header.DocEntry, 
				   Header.DocNum, 
				   bp.U_direccion, 
				   Header.CardCode, 
				   Header.CardName,
				   Header.DocCur, 
				   Header.DocRate, 
				   Header.DocTotalFC AS TOTAL, 
				   (select
						case when len(Header.doctime)=3 then
							convert (datetime,
								convert(varchar,( 
									convert(varchar,Header.DocDate,103) + ' '+ (left(Header.DocTime,1)) + ':'+ (right(Header.DocTime,2))) ,114),103)
						when len (Header.DocTime)=4 then
							convert (datetime,
								convert(varchar,( 
									convert(varchar,Header.DocDate,103) + ' '+ (left(Header.DocTime,2)) + ':'+ (right(Header.DocTime,2))) ,114),103)
						when len (Header.DocTime)= 2 then
							convert (datetime,
								convert(varchar,( 
									convert(varchar,Header.DocDate,103) + ' '+ '00' + ':'+ (right(Header.DocTime,2))) ,114),103)
						when len (Header.DocTime)= 1 then
							convert (datetime,
								convert(varchar,( 
									convert(varchar,Header.DocDate,103) + ' '+ '00' + ':'+ (right(Header.DocTime,1))) ,114),103)
					end) AS DocDate ,
				   Clave.U_NConsecFE AS U_NumFE,
				   Clave.U_NClaveFE,
				   Header.Comments,
				   Header.DocStatus, 
				   Header.NumAtCard, 
				   (select SUM(((line1.VatPrcnt / 100) * (line1.Price * line1.Quantity))) 
						from INV1 line1  
						where line1.DocEntry = Header.DocEntry) 
						AS Taxes,
				   (select SUM(((line1.DiscPrcnt / 100) * (line1.Price * line1.Quantity))) 
						from INV1 line1 
						where line1.DocEntry = Header.DocEntry) 
						AS Discount , 
				   Lines.ItemCode, 
				   Lines.Dscription, 
				   Lines.Price, 
				   Lines.Quantity, 
				   Lines.TaxCode, 
				   Lines.DiscPrcnt, 
				   Lines.LineTotal AS LINETOTAL  
			FROM OINV Header 
				   JOIN INV1 Lines ON Header.DocEntry = Lines.DocEntry
				   JOIN OCRD bp ON bp.CardCode = Header.CardCode
				   LEFT JOIN [@NCLAVEFE] Clave ON CAST(Header.DocEntry as varchar) = CAST(Clave.U_DocEntry AS VARCHAR) and Clave."U_TipoDoc" = 'FE'
			WHERE Header.DocEntry = @DocEntry 
		END;
	ELSE
		BEGIN
			SELECT Header.DocEntry, 
				   Header.DocNum, 
				   bp.U_direccion, 
				   Header.CardCode, 
				   Header.CardName, 
				   Header.DocCur, 
				   Header.DocRate, 
				   Header.DocTotalFC AS TOTAL, 
				  (select
						case when len(Header.doctime)=3 then
							convert (datetime,
								convert(varchar,( 
									convert(varchar,Header.DocDate,103) + ' '+ (left(Header.DocTime,1)) + ':'+ (right(Header.DocTime,2))) ,114),103)
						when len (Header.DocTime)=4 then
							convert (datetime,
								convert(varchar,( 
									convert(varchar,Header.DocDate,103) + ' '+ (left(Header.DocTime,2)) + ':'+ (right(Header.DocTime,2))) ,114),103)
						when len (Header.DocTime)= 2 then
							convert (datetime,
								convert(varchar,( 
									convert(varchar,Header.DocDate,103) + ' '+ '00' + ':'+ (right(Header.DocTime,2))) ,114),103)
						when len (Header.DocTime)= 1 then
							convert (datetime,
								convert(varchar,( 
									convert(varchar,Header.DocDate,103) + ' '+ '00' + ':'+ (right(Header.DocTime,1))) ,114),103)
					end) AS DocDate ,
					Clave.U_NConsecFE AS U_NumFE,
				   Clave.U_NClaveFE,
				   Header.Comments,
				   Header.DocStatus, 
				   Header.NumAtCard, 
				   (select SUM(((line1.VatPrcnt / 100) * (line1.Price * line1.Quantity))) 
						   from INV1 line1 
						   where line1.DocEntry = Header.DocEntry) 
						   AS Taxes,
			       (select SUM(((line1.DiscPrcnt / 100) * (line1.Price * line1.Quantity))) 
						   from INV1 line1 
						   where line1.DocEntry = Header.DocEntry) 
						   AS Discount ,  
				   Lines.ItemCode, 
				   Lines.Dscription, 
				   Lines.Price, 
				   Lines.Quantity, 
				   Lines.TaxCode, 
				   Lines.DiscPrcnt, 
				   Lines.TotalSumSy AS LINETOTAL  
			FROM OINV Header 
				   JOIN INV1 Lines ON Header.DocEntry = Lines.DocEntry
				   JOIN OCRD bp ON bp.CardCode = Header.CardCode
				   LEFT JOIN [@NCLAVEFE] Clave ON CAST(Header.DocEntry as varchar) = CAST(Clave.U_DocEntry AS VARCHAR) and Clave."U_TipoDoc" = 'FE'
			WHERE Header.DocEntry = @DocEntry
		END;
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_ACCOUNTSBYTYPE]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_ACCOUNTSBYTYPE]
	@AccounType VARCHAR(MAX)

AS
BEGIN

	SET NOCOUNT ON;


	IF @AccounType = 'Cash'
		BEGIN
		SELECT 
		CONCAT(CONCAT(T0."Segment_0", '-'), T0."AcctName") AS ACCOUNTNAME,
		T0."AcctCode" AS ACCOUNT
	FROM 
		OACT T0 
	WHERE 
		T0."Finanse" = 'Y'
		AND T0.FormatCode IN (1010101001,1010101002);
		END
	ELSE
		IF @AccounType = 'Transfer'
			BEGIN
			SELECT 
			CONCAT(CONCAT(T0."Segment_0", '-'), T0."AcctName") AS ACCOUNTNAME,
				T0."AcctCode" AS ACCOUNT
			FROM 
				OACT T0 
			WHERE 
				T0."Finanse" = 'Y'
				AND T0.FormatCode IN (1010101005);
			END
	ELSE
		
			BEGIN
			SELECT 
			CONCAT(CONCAT(T0."Segment_0", '-'), T0."AcctName") AS ACCOUNTNAME,
				T0."AcctCode" AS ACCOUNT
			FROM 
				OACT T0 
			WHERE 
				T0."Finanse" = 'Y'
				AND T0.FormatCode IN (1010101004);
			END
	

END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_AVAILABLEWHITEMS]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_AVAILABLEWHITEMS] 
@ItemCode varchar(20)
AS

SET NOCOUNT ON
SET ANSI_WARNINGS OFF

BEGIN
	SELECT ItemsWH.WhsCode, WareHouse.WhsName, ItemsWH.ItemCode, ItemsWH.OnHand, 
			ItemsWH.IsCommited, ItemsWH.OnOrder, ItemsWH.AvgPrice, (ItemsWH.OnHand) as Disponible, OITM.InvntItem			
	FROM dbo.OITW ItemsWH (NOLOCK) 
	   INNER JOIN dbo.OITM (NOLOCK)
		ON ItemsWH.ItemCode = @ItemCode AND
	       ItemsWH.OnHand > CASE WHEN OITM.InvntItem = 'Y' THEN 0 ELSE -1 END AND
		   ItemsWH.ItemCode = OITM.ItemCode
	   INNER JOIN dbo.OWHS WareHouse (NOLOCK)
	    ON WareHouse.WhsCode = ItemsWH.WhsCode	
	ORDER BY ItemsWH.WhsCode;
END

GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_BARCODESBYITEM]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_BARCODESBYITEM]
@ITEM_CODE NVARCHAR(16)
AS
BEGIN
	SELECT 
	OB.BcdEntry, 
	OB.BcdCode, 
	OB.BcdName, 
	OB.UomEntry 
FROM 
	OBCD OB
WHERE 
	OB.ItemCode = @ITEM_CODE;
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_BPPURCHASEORDER]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_BPPURCHASEORDER]
@DocEntry varchar(20)
AS
BEGIN
SELECT T1.CardCode, T1.CardName, T1.Comments From OPOR T1 WHERE T1.DocNum = @DocEntry
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_BUSINESSPARTNER]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_BUSINESSPARTNER]	
AS
BEGIN
	SELECT 
	  o.CardCode,
      o.CardName,
	  o.CardType,
      o.Phone1,    
      o.LicTradNum,    
      o.E_Mail,    
      o.U_TipoIdentificacion,    
      o.U_provincia,          
      o.U_canton,    
      o.U_distrito,    
      o.U_barrio,    
      o.U_direccion     
  FROM OCRD o;
	
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_BUSINESSPARTNER_BYCODE]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_BUSINESSPARTNER_BYCODE]
@CardCode varchar(15)
AS
BEGIN
	SELECT 
	 
      o.CardName,         
      o.Phone1,    
      o.LicTradNum, 
	  o.CardType,
      o.E_Mail,    
      o.U_TipoIdentificacion,    
      o.U_provincia,        
      o.U_canton,    
      o.U_distrito,    
      o.U_barrio,    
      o.U_direccion     
  FROM OCRD o
  WHERE o.CardCode= @CardCode;
	
END

GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_CANCELPAYMENT]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_CANCELPAYMENT]
@CardCode varchar(20),
@FIni VARCHAR(20),
@FFin VARCHAR(20)
AS
BEGIN
SELECT 
	T2.DocNum ,
	T2.DocEntry AS InvoDocEntry,
	T0.DocEntry, 
	T0.DocNum As DocNumPago,
	FORMAT(T0.DocDate,'dd-MM-yyyy') As DocDate,
	T0.DocTotal,
	T2.DocTotalFC,
	T2.DocCur ,
	T2.CardCode,
	T2.CardName ,
	T2.DocStatus ,
	0 As Selected  
FROM ORCT T0 INNER JOIN 
	(OINV T2 INNER JOIN RCT2 T1 ON T2.DocEntry = T1.DocEntry
	AND (T2.CardCode = @CardCode OR @CardCode ='') -- SE AGREGA EL OR PARA FLEXIBILIZAR LA BUSQUEDA, HACIENDO QUE SE FILTRE SOLO POR FECHAS
	AND (T2.DocDate BETWEEN CONVERT(DATE, @FIni) AND CONVERT(DATE, @FFin)) )
	ON T0.DocEntry = T1.DocNum AND T0.Canceled ='N'
END


SET ANSI_NULLS ON
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_CASHFLOWTOTAL]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_CASHFLOWTOTAL]
@UserSignature INT
AS
BEGIN
	SELECT 
		COALESCE(SUM(T0.U_Amount), 0)  Total
	FROM 
		[@CASHFLOW] T0 
	WHERE
		T0.U_INTERNAL_K = @UserSignature
		AND CAST(T0.U_CreationDate AS DATE) = CAST(GETDATE() AS DATE)
		AND T0.U_Type = 1
	UNION ALL
	SELECT 
		COALESCE(SUM(T0.U_Amount), 0) Total
	FROM 
		[@CASHFLOW] T0 
	WHERE
		T0.U_INTERNAL_K = @UserSignature
		AND CAST(T0.U_CreationDate AS DATE) = CAST(GETDATE() AS DATE)
		AND T0.U_Type = 2;
END;
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_CHECK_HASPAYMENT]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Create date: <01/09/2021>
-- Description:	Obtiene los pagos realizados a un documento
-- =============================================
CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_CHECK_HASPAYMENT] 
@DocEntry int
AS
BEGIN
	SELECT
		--T1.DocEntry,
		T0.DocTotal,
		T0.DocEntry,
		t0.DocNum
	FROM
		ORCT T0 
	JOIN
		RCT2 T1 ON T0.DocEntry = T1.DocNum 
	WHERE
		T1.DocEntry IN (@DocEntry) AND T0.Canceled = 'N';
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_CHECK_UNIQUEINVID]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_CHECK_UNIQUEINVID]
@UniqueInvId VARCHAR(254)
AS
BEGIN
	SELECT U_CLVS_POS_UniqueInvId FROM OINV
	WHERE U_CLVS_POS_UniqueInvId = @UniqueInvId
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_CHECK_UNIQUEINVID_RETURNINFO]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Alejandro Mora P.>
-- Create date: <02/09/2021>
-- Description:	<SP utilizado para checkear si el UNIQUEINVID existe, a diferencia del sp 'CLVS_POS_CHECK_UNIQUEINVID' este retorna mas info del documento>
-- =============================================
CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_CHECK_UNIQUEINVID_RETURNINFO] 
	@UniqueInvId VARCHAR(254)
AS
BEGIN
	
	SET NOCOUNT ON;

SELECT 
	[INVOICE].U_CLVS_POS_UniqueInvId
	,[INVOICE].DocNum
	,[INVOICE].DocEntry,
	'INVOICE' AS Comments
FROM
	OINV [INVOICE]
WHERE
	[INVOICE].U_CLVS_POS_UniqueInvId = @UniqueInvId
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_CHECK_UNIQUEINVID_SUPPLIER]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_CHECK_UNIQUEINVID_SUPPLIER]
@UniqueInvId VARCHAR(254)
AS
BEGIN
	SELECT U_CLVS_POS_UniqueInvId FROM OPCH
	WHERE U_CLVS_POS_UniqueInvId = @UniqueInvId
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_CHECK_UNIQUEINVIDBYTABLE]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Alejandro Mora P.>
-- Create date: <03/01/2022>
-- Description:	<SP utilizado para checkear si el UNIQUEINVID existe en un documento>
-- =============================================
CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_CHECK_UNIQUEINVIDBYTABLE] 
@UniqueInvId VARCHAR(MAX),
@Table VARCHAR(6)
AS
BEGIN
DECLARE @Query VARCHAR(MAX)
	


SET @Query = 'SELECT T.DocEntry , T.DocNum FROM TABLE_NAME T WHERE T.U_CLVS_POS_UniqueInvId = ''pUniqueInvId''';

SET @Query = REPLACE(@Query,'TABLE_NAME',@Table);

SET @Query = REPLACE(@Query,'pUniqueInvId',@UniqueInvId);

 
EXEC (@Query);

END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_DEFAULTPRICELIST]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_DEFAULTPRICELIST]
	@CardCode VARCHAR(30)
AS
BEGIN
	SELECT PL.ListNum, PL.ListName FROM OCRD CR
	JOIN OPLN PL ON PL.ListNum = CR.ListNum
	WHERE CR.CardCode = @CardCode
END

SET ANSI_NULLS ON
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_DOCNUM_BYTABLE]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_DOCNUM_BYTABLE]
@DocEntry INT,
@Table VARCHAR(6)
AS
BEGIN

	DECLARE 
		@Query VARCHAR (MAX);

	SET @Query = 'SELECT T.DocNum as DocNum FROM TABLE_NAME T WHERE T.DocEntry = pDocEntry';
	SET @Query = REPLACE(@Query,'TABLE_NAME',@Table);
	SET @Query = REPLACE(@Query,'pDocEntry',@DocEntry);

	EXEC(@Query);
END


GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_DOCNUM_OPDN]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_DOCNUM_OPDN]
@DocEntry int
AS
BEGIN
	SELECT T0.DocNum as DocNum		
	FROM OPDN T0	
	WHERE T0.DocEntry = @DocEntry
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_DOCNUM_OQUT]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_DOCNUM_OQUT]
@DocEntry int
AS
BEGIN
	SELECT oqut.DocNum 		
	FROM OQUT oqut	
	WHERE DocEntry = @DocEntry
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_DOCNUM_ORCT]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_DOCNUM_ORCT]
@DocEntry int
AS
BEGIN
	SELECT pay.DocNum 		
	FROM ORCT pay	
	WHERE DocEntry = @DocEntry
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_DOCNUM_ORDR]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_DOCNUM_ORDR]
@DocEntry int
AS
BEGIN
	SELECT ordr.DocNum 		
	FROM ORDR ordr	
	WHERE DocEntry = @DocEntry
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_DOCNUM_ORPD]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_DOCNUM_ORPD]
@DocEntry int
AS
BEGIN
	SELECT T0.DocNum as DocNum		
	FROM ORPD T0	
	WHERE T0.DocEntry = @DocEntry
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_FATHERCARD]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_FATHERCARD]
@CardCode varchar(20)
AS
BEGIN

SELECT FatherCard FROM dbo.OCRD WHERE CardCode = @CardCode

END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_FEINFORMATIONFROMINV]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Alejandro Mora P.>
-- Create date: <20/09/2021>
-- Description:	<Obtiene data de FE desde una factura>
-- =============================================
CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_FEINFORMATIONFROMINV] 
	@idType VARCHAR(2),
	@idNumber VARCHAR (15)
AS
BEGIN
	
	SET NOCOUNT ON;
SELECT 
	invoice.CardCode,
	invoice.CardName,
	ISNULL(invoice.U_TipoIdentificacion, '') U_TipoIdentificacion,
	ISNULL(invoice.U_NumIdentFE, '') U_NumIdentFE,
	ISNULL(invoice.U_CorreoFE, '') U_CorreoFE,
	ISNULL(invoice.U_Provincia, '') U_Provincia,
	ISNULL(invoice.U_Canton, '') U_Canton,
	ISNULL(invoice.U_Distrito, '') U_Distrito,
	ISNULL(invoice.U_Direccion, '') U_Direccion,
	ISNULL(invoice.DocNum, '') MaxDocNum,
	invoice.DocDate
FROM OINV invoice 
JOIN (
		SELECT MAX(DocEntry) DocEntry FROM OINV
		GROUP BY U_NumIdentFE
	) RC ON RC.DocEntry = invoice.DocEntry
WHERE
	RTRIM(LTRIM(U_TipoIdentificacion)) = @idType
	AND RTRIM(LTRIM(U_NumIdentFE)) = @idNumber;
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_INVOICE_PAYMENTDETAIL]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_INVOICE_PAYMENTDETAIL]
@DocEntry integer
AS
BEGIN
	SELECT -- ENCABEZADO DEL DOCUMENTO EL PRIMER DATASET
		CashSum, 
		CashSumFC, 
		TrsfrSum, 
		TrsfrSumFC
	FROM ORCT 
	WHERE
		DocEntry = @DocEntry
	
	SELECT -- TARJETAS VAN EL SEGUNDO DATASET
		VoucherNum, 
		CreditSum, 
		LineID, 
		CardValid, 
		FirstDue, 
		CreditAcct,
		CreditCard,
		U_ManualEntry as [IsManualEntry]
	FROM RCT3
	WHERE DocNum = @DocEntry
END

GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_INVPRINTLIST]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<>
-- Create date: <>
-- Description:	<Obtiene la lista de documentos (Facturas,Ordenes de ventas o cotizaciones) dependiendo del parametro type para ser reimpresas>
-- =============================================
CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_INVPRINTLIST] 
@slpCode varchar(10),
@DocEntry varchar(25),
@FechaIni varchar(10),
@FechaFin varchar(10),
@type int
AS
BEGIN
	IF @type = 1
		SELECT 
			fact.DocEntry,
			fact.DocNum,
			(CASE WHEN len(fact.DocTime)=3 THEN 
			convert (datetime,convert(varchar,( convert(varchar,fact.DocDate,103) + ' '+ (left(fact.DocTime,1)) + ':'+ (right(fact.DocTime,2))) ,114),103) ELSE
			convert (datetime,convert(varchar,( convert(varchar,fact.DocDate,103) + ' '+ (left(fact.DocTime,2)) + ':'+ (right(fact.DocTime,2))) ,114),103) END) AS DocDate, 
			fact.CardName, 
			fact.DocStatus,
			1 AS IsManualEntry,
			fact.U_CLVS_POS_UniqueInvId AS InvoiceNumber,
			fact.DocTotal AS DocTotal,
			fact.DocCur AS DocCur
		FROM 
			ORDR fact
		WHERE
			fact.SlpCode = @slpCode 
			AND (CONVERT(DATE, CONVERT(VARCHAR, fact.DocDate, 111)) BETWEEN CONVERT(DATE, CONVERT(VARCHAR, @FechaIni, 111)) 
			AND CONVERT(DATE, CONVERT(VARCHAR, @FechaFin, 111)))
			AND fact.DocEntry like @DocEntry+'%'
	ELSE IF @type = 2
		SELECT 
			fact.DocEntry,
			fact.DocNum,
			(CASE WHEN len(fact.DocTime)=3 THEN 
			convert (datetime,convert(varchar,( convert(varchar,fact.DocDate,103) + ' '+ (left(fact.DocTime,1)) + ':'+ (right(fact.DocTime,2))) ,114),103) ELSE
			convert (datetime,convert(varchar,( convert(varchar,fact.DocDate,103) + ' '+ (left(fact.DocTime,2)) + ':'+ (right(fact.DocTime,2))) ,114),103) END) AS DocDate, 
			fact.CardName, 
			fact.DocStatus,
			1 AS IsManualEntry,
			fact.U_CLVS_POS_UniqueInvId AS InvoiceNumber,
			fact.DocTotal AS DocTotal,
			fact.DocCur AS DocCur
		FROM
			OQUT fact
		WHERE
			fact.SlpCode = @slpCode 
			AND (CONVERT(DATE, CONVERT(VARCHAR, fact.DocDate, 111)) BETWEEN CONVERT(DATE, CONVERT(VARCHAR, @FechaIni, 111)) 
			AND CONVERT(DATE, CONVERT(VARCHAR, @FechaFin, 111)))
			AND fact.DocEntry like @DocEntry+'%'
	ELSE IF @type = 5
		SELECT
			fact.DocEntry,
			fact.DocNum,
			--Clave.U_NConsecFE AS U_NumFE,
			'' AS U_NumFE,
			(CASE WHEN len(fact.DocTime)=3 THEN 
			convert (datetime,convert(varchar,( convert(varchar,fact.DocDate,103) + ' '+ (left(fact.DocTime,1)) + ':'+ (right(fact.DocTime,2))) ,114),103) ELSE
			convert (datetime,convert(varchar,( convert(varchar,fact.DocDate,103) + ' '+ (left(fact.DocTime,2)) + ':'+ (right(fact.DocTime,2))) ,114),103) END) AS DocDate,
			fact.CardName,
			fact.DocStatus,
			COALESCE((SELECT TOP 1
				T2.U_ManualEntry	
				FROM ORCT T0 
				LEFT JOIN RCT2 T1 ON T0.DocEntry = T1.DocNum
				LEFT JOIN RCT3 T2 ON T2.DocNum = T0.DocNum
			WHERE T1.DocEntry = fact.DocEntry), 1) AS IsManualEntry,
			fact.U_CLVS_POS_UniqueInvId AS InvoiceNumber,
			fact.DocTotal AS DocTotal,
			fact.DocCur AS DocCur
		FROM 
			OINV  fact
			--JOIN [@NCLAVEFE] Clave ON fact.DocEntry = Clave."Code" and Clave."U_TipoDoc" = 'FE'
		WHERE
			(
				fact.SlpCode = @slpCode or fact.SlpCode in ('1')
			)
			AND
			(
				(CONVERT(DATE, CONVERT(VARCHAR, fact.DocDate, 111)) BETWEEN CONVERT(DATE, CONVERT(VARCHAR, @FechaIni, 111))
				AND CONVERT(DATE, CONVERT(VARCHAR, @FechaFin, 111)))
				OR @DocEntry NOT IN('')
			)
			AND
			(
				(fact.DocNum = @DocEntry) OR @DocEntry = ''
			)
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_ITEM_AVGPRICE]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Alejandro Mora P.>
-- Create date: <07/09/2021>
-- Description:	<SP que obtiene el avgPrice de un item basado en su itemCode se utiliza para la ventana modal de ajuste de inventario en facturacion.>
-- =============================================
CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_ITEM_AVGPRICE] 
	@ItemCode VARCHAR(30)
AS
BEGIN

	SET NOCOUNT ON;
	
	SELECT
	SIT.AvgPrice AS 'AvgPrice'
FROM
	OITW SIT
WHERE
	SIT.ItemCode= @ItemCode 

END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_ITEM_BY_CODEBAR]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_ITEM_BY_CODEBAR]
@ItemBarcode varchar(100)
AS
BEGIN
SELECT
	OI.ItemCode,
	OI.ItemName,
	OB.BcdEntry, 
	OB.BcdCode, 
	OB.BcdName, 
	OB.UomEntry 
FROM 
	OBCD OB
JOIN
	OITM OI ON OI.ItemCode = OB.ItemCode
WHERE 
	OB.BcdCode = @ItemBarcode;
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_ITEM_INFO]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_ITEM_INFO]
@ItemCode varchar(100),
@Discount decimal(12,2),
@PriceList int,
@CardCode VARCHAR(100),
@WhsCode VARCHAR (12)
AS
BEGIN
-- cambiar el 0 por item.U_maxDiscount
SELECT 
	item.ItemCode, 
	item.ItemName,
	(CASE 
		WHEN U_IVA='100EXO' THEN 'EXO'
		ELSE U_IVA
	END) AS TaxCode, 
	item.InvntItem, 
	(CASE 
		WHEN U_IVA='100EXO' THEN 0
		WHEN U_IVA='1IVA' THEN 1
		WHEN U_IVA='2IVA' THEN 2
		WHEN U_IVA='4IVA' THEN 4
		WHEN U_IVA='13IVA' THEN 13
	END) AS TaxRate,
	pricelist.Price AS UnitPrice,
	--(CASE 
	--	WHEN @CardCode = 'C0001' THEN 5
	--	ELSE 0 
	--END ) AS U_Discount,
	DG.Discount AS U_Discount,
	(SELECT 
		T1.[CardName] 
	FROM OCRD T1 
	WHERE 
		item.[CardCode] = T1.[CardCode]) 
	AS PreferredVendor, 
	item.OnHand as OnHand,
	item.FrgnName as ForeingName,
	LastPurCur,
	LastPurDat,	
	(CASE
	  WHEN (SELECT df.MainCurncy FROM OADM df) = (SELECT curr.PrimCurr FROM OPLN curr WHERE curr.ListNum = @PriceList)
	  THEN 
	  (SELECT SIT.AvgPrice as LastPurPrc FROM OITW SIT WHERE SIT.ItemCode= @ItemCode AND SIT.WhsCode=@WhsCode)	
	  ELSE
	  (SELECT SIT.AvgPrice/(SELECT T0.Rate FROM ORTT T0 WHERE T0.RateDate = CONVERT(date, GETDATE()) AND T0.Currency = (SELECT curr.PrimCurr FROM OPLN curr WHERE curr.ListNum = @PriceList))
	  as LastPurPrc FROM OITW SIT WHERE SIT.ItemCode= @ItemCode AND SIT.WhsCode=@WhsCode)
	  END) as LastPurPrc
	 
FROM 
	OITM item LEFT JOIN ITM1 pricelist ON item.ItemCode = pricelist.ItemCode
	LEFT JOIN dbo.CLVS_D_EMA_SLT_DISCGROUP DG ON DG.CardCode = @CardCode AND DG.ItemGroup = ITEM.ItmsGrpCod
		--JOIN OBCD OB ON OB.ItemCode = ITEM.ItemCode
WHERE 
	item."ItemCode" = @ItemCode 
	and pricelist.PriceList = @PriceList
END

/****** Object:  StoredProcedure [dbo].[CLVS_POS_GETITEMS_BY_WH]    Script Date: 3/12/2020 09:37:44 ******/
SET ANSI_NULLS ON
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_ITEM_LASTPRICE]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Alejandro Mora P.>
-- Create date: <07/09/2021>
-- Description:	<SP que obtiene el ultimo precio de entrada de un item basado en su itemCode se utiliza para la ventana modal de ajuste de inventario en facturacion.>
-- =============================================
CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_ITEM_LASTPRICE] 
	@ItemCode VARCHAR(30)
AS
BEGIN

	SET NOCOUNT ON;



SELECT TOP 1
	DN.Price AS 'LastPrice'
FROM
	OPDN PD 
		JOIN PDN1  DN ON DN.DocEntry = pd.DocEntry 
		JOIN OITM IT ON IT.ItemCode = DN.ItemCode 
WHERE
	DN.ItemCode = @ItemCode
ORDER BY 
	PD.DocNum DESC





END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_ITEM_NAME_BY_CODEBAR]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_ITEM_NAME_BY_CODEBAR]
@ItemBarcode varchar(100)
AS
BEGIN
SELECT
	OI.ItemCode,
	OI.ItemName

FROM 
	OBCD OB
JOIN
	OITM OI ON OI.ItemCode = OB.ItemCode
WHERE 
	OB.BcdCode = @ItemBarcode;
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_ITEM_PURCHASE_DETAIL]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_ITEM_PURCHASE_DETAIL]
@ItemCode VARCHAR(35),
@Rows INT,
@TableCode VARCHAR(10)
AS
BEGIN

DECLARE 
	@sql VARCHAR (MAX)

SET @sql = 'SELECT
	DISTINCT TOP (NROWS)
	 PD.DocNum AS DocNum
	,IT.U_IVA AS U_IVA
	,IT.LastPurPrc AS LastPurPrc
	,PD.DocEntry   AS DocEntry
	,PD.CardName   AS CardName 
	,PD.CardCode   AS CardCode
	,DN.ItemCode   AS itemCode
	,PD.Comments   AS Comments
	,IT.ItemName   AS ItemName
	,(IW.OnHand - IW.IsCommited) as Available
	,IW.OnHand     AS Stock
	,IW.OnOrder    AS Requested
	,IW.IsCommited AS IsCommited
	,DN.Quantity   AS Quantity
	,DN.DocDate    AS DocDate
	,PD.DocTotal   AS DocTotal
	,DN.WhsCode    AS WhsCode
	,DN.TaxCode    AS TaxCode
	,DN.Price      AS Price
	,WH.WhsName    AS WhsName

FROM
	TABLE_NAME  PD 
JOIN PDN1  DN ON DN.DocEntry = pd.DocEntry 
JOIN OITM IT ON IT.ItemCode = DN.ItemCode 
JOIN OITW IW ON IW.ItemCode = IT.ItemCode
JOIN OWHS WH ON WH.WhsCode = DN.WhsCode 
WHERE
	DN.ItemCode = ''ITEM_CODE''
ORDER BY
	PD.DocNum DESC'

SET @sql = replace(@sql,'NROWS',@Rows)

SET @sql = replace(@sql,'ITEM_CODE',@ItemCode)

SET @sql = replace(@sql,'TABLE_NAME',@TableCode)

EXEC(@sql);


--	SELECT
--	DISTINCT TOP (@Rows)
--	 PD.DocNum AS 'DocNum'
--	,IT.U_IVA AS 'U_IVA' --TaxRate
--	,IT.LastPurPrc AS 'LastPurPrc' -- ultimo precio venta
--	,PD.DocEntry   AS 'DocEntry'
--	,PD.CardName   AS 'CardName' -- Nombre del proveedor? 
--	,PD.CardCode   AS 'CardCode' -- Codigo provedor
--	,DN.ItemCode   AS 'ItemCode'-- Codigo del articulo
--	,PD.Comments   AS 'Comments'
--	,IT.ItemName   AS 'ItemName' --Nombre del articulo
--	,(IW.OnHand - IW.IsCommited) as 'Available' --Disponible
--	,IW.OnHand     AS 'Stock' -- En stock
--	,IW.OnOrder    AS 'Requested'
--	,IW.IsCommited AS 'IsCommited'
--	,DN.Quantity   AS 'Quantity' -- Este es el Total del item (cantidad unidades)
--	,DN.DocDate    AS 'DocDate' -- Fecha de la entrada
--	,PD.DocTotal   AS 'DocTotal' -- precio de la entrada en total
--	,DN.WhsCode    AS 'WhsCode'
--	,DN.TaxCode    AS 'TaxCode'
--	,DN.Price      AS 'Price'      --- Este si es el precio del item en la entrada 
--	,WH.WhsName    AS 'WhsName' -- Nombre almacen

--FROM
--	OPDN PD -- Tabla de las entradas
--JOIN PDN1  DN ON DN.DocEntry = pd.DocEntry -- Lineas de la entrada
--JOIN OITM IT ON IT.ItemCode = DN.ItemCode -- Tabla de items
--JOIN OITW IW ON IW.ItemCode = IT.ItemCode -- Tabla de stock
--JOIN OWHS WH ON WH.WhsCode = DN.WhsCode -- tabla de almacenes
--WHERE
--	DN.ItemCode = @ItemCode
--ORDER BY
--	PD.DocNum DESC




	

END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_ITEMBYITEMXML]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_ITEMBYITEMXML]
	@ItemNameXml VARCHAR(MAX)
AS
BEGIN
SELECT TOP 1
T0.ItemCode,
T0.ItemName

FROM OITM T0
INNER JOIN PDN1 T1 ON  T0.ItemCode = t1.ItemCode
WHERE T1.U_DescriptionItemXml LIKE '%'+RTRIM(@ItemNameXml)+'%'
--GROUP BY T0.ItemCode,T0.ItemName
ORDER BY 
T1.DocEntry desc

END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_ITEMLIST]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_ITEMLIST]
@ItemCode varchar(100)
AS
BEGIN

--SELECT OP.

SELECT 
	IT.ItemCode, 
	OP.ListNum, 
	OP.ListName,
	IT.Price
FROM ITM1 IT
JOIN 
	OPLN OP ON OP.ListNum = IT.PriceList
WHERE 
	IT.ItemCode=@ItemCode

ORDER BY OP.ListNum
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_NUMAPINVOICE]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_NUMAPINVOICE]
@DocEntry int
AS
BEGIN
	SELECT apinv.DocNum		
	FROM OPCH apinv	
	WHERE DocEntry = @DocEntry
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_NUMFEONLINE]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_NUMFEONLINE]
@DocEntry int
AS
BEGIN
	SELECT oinv.DocNum,
		fe.U_NConsecFE AS NumFE,
		fe.U_NClaveFE AS ClaveFE 		
	FROM OINV oinv
		JOIN "@NCLAVEFE" AS fe ON CAST(@DocEntry AS VARCHAR(1000)) = CAST(fe.U_DocEntry AS VARCHAR(1000))
	WHERE
		DocEntry = @DocEntry
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_PAYDOCUMENTS]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_PAYDOCUMENTS]

@CardCode varchar(20),
@Sede varchar(20),
@Currency varchar(3)

AS
BEGIN
SELECT T0.DocEntry, T0.DocNum, 'Anticipo' as Tipo,T0.CardCode, T0.CardName,
   T0.NumAtCard, T0.DocCur, T1.InsTotal AS Total, T1.InsTotal - T1.PaidToDate AS Saldo,
   T1.InsTotalFC AS TotalFC, T1.InsTotalFC - T1.PaidFC AS SaldoFC,
   CONVERT (date, T0.DocDate) AS DocDate, 
   CONVERT (date, T0.DocDueDate) AS DocDueDate, T1.InstlmntID
FROM ODPI T0
JOIN DPI6 T1 ON T0.DocEntry = T1.DocEntry
WHERE T0.DocStatus='O' AND T0.CardCode = @CardCode AND T0.DocCur = @Currency

UNION

SELECT T2.DocEntry, T2.DocNum, 'Factura' as Tipo,T2.CardCode, T2.CardName,
   T2.NumAtCard, T2.DocCur, T3.InsTotal AS Total,
   T3.InsTotal - T3.PaidToDate AS Saldo, T3.InsTotalFC AS TotalFC,
   T3.InsTotalFC - T3.PaidFC AS SaldoFC,
   CONVERT (date, T2.DocDate) AS DocDate, 
   CONVERT (date, T2.DocDueDate) AS DocDueDate, T3.InstlmntID
FROM OINV T2
JOIN INV6 T3 ON T2.DocEntry = T3.DocEntry
WHERE T2.DocStatus='O' AND T2.CardCode = @CardCode AND T2.DocCur = @Currency
ORDER BY Tipo ASC, DocDate DESC;

END

GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_PAYDOCUMENTS_SUPPLIER]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_PAYDOCUMENTS_SUPPLIER]
@CardCode varchar(20),
@Sede varchar(20),
@Currency varchar(3)
AS
BEGIN
SELECT T0.DocEntry, T0.DocNum, 'Factura' as Tipo,T0.CardCode, T0.CardName,
   T0.NumAtCard, T0.DocCur, T0.DocTotal AS Total, T0.DocTotal - T0.PaidToDate AS Saldo,
   T0.DocTotalFC AS TotalFC, T0.DocTotalFC - T0.PaidFC AS SaldoFC,
   CONVERT (date, T0.DocDate) AS DocDate ,
   CONVERT (date, T0.DocDueDate) AS DocDueDate
FROM OPCH T0
WHERE T0.DocStatus='O' AND T0.CardCode = @CardCode AND T0.DocCur = @Currency AND T0.Series = @Sede
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_PURCHASEORDERBYCODE]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_PURCHASEORDERBYCODE]
@DocEntry varchar(20)
AS
BEGIN

SELECT
	item.ItemCode,
	(CASE 
		WHEN T0.TaxCode='100EXO'
		THEN 0
		WHEN T0.TaxCode='1IVA'
		THEN 1
		WHEN T0.TaxCode='2IVA'
		THEN 2
		WHEN T0.TaxCode='4IVA'
		THEN 4
		WHEN T0.TaxCode='13IVA'
		THEN 13
		WHEN T0.TaxCode='EXE'
		THEN 0
		END
	) AS TaxRate,
	T0.TaxCode,
	T0.ItemCode,
	T0.Dscription as ItemName,
	T0.WhsCode,
	T0.Quantity,
	t0.PriceBefDi AS UnitPrice,
	T0.DiscPrcnt,
	T0.Rate,
	T0.LineTotal,
	T0.TaxOnly AS TaxOnly
	
FROM POR1 T0 
	Inner JOIN  OITM item on item.ItemCode = T0.ItemCode
	
WHERE T0.DocEntry = @DocEntry

END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_PURCHASEORDERLIST]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_PURCHASEORDERLIST]
(
@CardCode varchar(20),
@FIni VARCHAR(20),
@FFin VARCHAR(20))
AS
BEGIN

SELECT 
	--T0.CardCode,
	T0.CardName,
	T0.DocNum,
	T0.DocDate,
	--CAST(FORMAT(T0.DocDate,'yyyy/MM/dd hh:mm:ss ')AS varchar) as DocDate,
	T0.DocTotal
FROM OPOR T0
WHERE 
	T0.DocStatus = 'O'
	AND (T0.CardCode = @CardCode OR @CardCode = '')
	AND (T0.DocDate BETWEEN CONVERT(DATE, @FIni) AND CONVERT(DATE, @FFin))

	--AND
	--		(
	--			(CONVERT(DATE, CONVERT(VARCHAR, T0.DocDate, 111)) BETWEEN CONVERT(DATE, CONVERT(VARCHAR, @FIni, 111))
	--			AND CONVERT(DATE, CONVERT(VARCHAR, @FFin, 111)))
	--			OR @CardCode NOT IN('')
	--		)
	--		AND
	--		(
	--			(T0.CardCode = @CardCode) OR @CardCode = ''
	--		)

END


	

GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_QUOTATIONEDIT_HEADER]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_QUOTATIONEDIT_HEADER]
@DocEntry int
AS
BEGIN
	--SELECT
	--	CAST(Quot.GroupNum AS varchar) PayTerms,
 --       Quot.DocEntry,
 --       Quot.DocNum,
 --       Quot.DocDate,
	--	Quot.CardCode,
	--	Quot.CardName,		
	--	Quot.DocTotal AS DocTotal,
	--	Quot.DocTotalFC AS DocTotalFC,
	--	Quot.DocStatus,
	--	Quot.Comments AS Comment, 
	--	Quot.DocCur AS Currency,
	--	Oslp.SlpCode AS SlpCode,
	--	Oslp.SlpName AS SalesMan
				
 --   FROM           
 --       dbo.OQUT AS Quot WITH (NOLOCK)
 --   JOIN
 --       dbo.OSLP AS Oslp WITH (NOLOCK) ON Quot.SlpCode = Oslp.SlpCode
	--WHERE		
	--	Quot.DocEntry = @DocEntry  

	SELECT
		CAST(Quot.GroupNum AS varchar) PayTerms,
        Quot.DocEntry,
        Quot.DocNum,
        Quot.DocDate,
		Quot.CardCode,
		Quot.CardName,		
		Quot.DocTotal AS DocTotal,
		Quot.DocTotalFC AS DocTotalFC,
		Quot.DocStatus,
		Quot.Comments AS Comment, 
		Quot.DocCur AS Currency,
		Oslp.SlpCode AS SlpCode,
		Oslp.SlpName AS SalesMan,
		Quot.U_TipoIdentificacion AS IdType,
		Quot.U_NumIdentFE AS Identification,
		Quot.U_CorreoFE AS Email,
		Quot.U_ObservacionFE AS U_ObservacionFE,
		COALESCE(Quot.NumAtCard, '') AS NumAtCard,
		Quot.U_TipoDocE AS DocumentType
    FROM           
        dbo.OQUT AS Quot WITH (NOLOCK)
    JOIN
        dbo.OSLP AS Oslp WITH (NOLOCK) ON Quot.SlpCode = Oslp.SlpCode
	JOIN
		dbo.OCRD AS Bp WITH (NOLOCK) ON Quot.CardCode = Bp.CardCode
	WHERE		
		Quot.DocEntry = @DocEntry  
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_QUOTATIONEDIT_LINES]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_QUOTATIONEDIT_LINES]
@DocEntry int,
@CardCode varchar(30),
@AllLines int
AS
BEGIN
	SELECT
        Qut1.ItemCode,
        Qut1.Dscription AS ItemName,
        CAST(Qut1.Quantity as float) AS Quantity,
		CAST(Qut1.PriceBefDi AS float) AS UnitPrice,
		--CAST(Item.LastPurPrc AS float) AS LastPurchasePrice,		
		CAST(Qut1.DiscPrcnt AS float) AS Discount,
		Qut1.TaxCode,
		CAST(Qut1.VatPrcnt AS float) AS TaxRate,
		Qut1.WhsCode,
		WH.WhsName,
		Qut1.LineNum as BaseLine,
		Qut1.LineNum,
		Qut1.LineStatus,
		CAST(Item.ItmsGrpCod AS INT) AS ItemGroupCode,		
		(SELECT SIT.AvgPrice FROM OITW SIT WHERE SIT.ItemCode= Item.ItemCode AND SIT.WhsCode= Qut1.WhsCode) AS LastPurchasePrice
    FROM           
        dbo.QUT1 AS Qut1 WITH (NOLOCK)
    JOIN dbo.OITM AS Item WITH (NOLOCK) ON Qut1.ItemCode = Item.ItemCode
	JOIN dbo.ITM1 AS PriceList WITH (NOLOCK) ON Qut1.ItemCode = PriceList.ItemCode
	JOIN dbo.OCRD AS Customer WITH (NOLOCK) ON PriceList.PriceList = Customer.ListNum
	JOIN dbo.OWHS WH ON WH.WhsCode = Qut1.WhsCode	
	WHERE		
		Qut1.DocEntry = @DocEntry AND
        Customer.CardCode = @CardCode 
		--AND Qut1.LineStatus = 'O'
	--	(@AllLines = 0 OR Qut1.LineStatus = 'O') ORDER BY Qut1.LineNum
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_QUOTATIONS]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_QUOTATIONS]
@SlpCode int,
@DocStatus VARCHAR(2),
@DocNum int,
@CardCode VARCHAR(10),
@Fini date,
@Ffin date
--@U_Almacen NVARCHAR(10)
AS
BEGIN
	SELECT
        Quot.DocEntry,
        Quot.DocNum,
        Quot.DocDate,
		Quot.CardCode,
		Quot.CardName AS CardName,		
		Quot.DocTotal AS DocTotal,
		Quot.DocTotalFC AS DocTotalFC,
		Quot.DocStatus,
		Oslp.SlpCode AS SlpCode,
		Oslp.SlpName AS SalesMan,
		Quot.Comments AS Comment, 
		Quot.DocCur AS Currency
    FROM           
        dbo.OQUT AS Quot WITH (NOLOCK)
    JOIN
        dbo.OSLP AS Oslp WITH (NOLOCK) ON Quot.SlpCode = Oslp.SlpCode
	WHERE
		Quot.DocDate BETWEEN @Fini AND @Ffin AND
		Quot.DocStatus = (CASE WHEN LEN(@DocStatus) > 0 THEN @DocStatus ELSE Quot.DocStatus END) AND
		Quot.DocNum = (CASE WHEN @DocNum > 0 THEN @DocNum ELSE Quot.DocNum END) AND
		Quot.CardCode = (CASE WHEN LEN(@CardCode) > 0 THEN @CardCode ELSE Quot.CardCode END) AND
		--Quot.U_Almacen = @U_Almacen AND
		Oslp.SlpCode = (CASE WHEN @SlpCode > 0 THEN @SlpCode ELSE Oslp.SlpCode END)
	ORDER BY 
		Quot.DocEntry
  END;
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_SALEORDEREDIT_HEADER]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_SALEORDEREDIT_HEADER]
@DocEntry int
AS
BEGIN
	SELECT
		CAST(Ord.GroupNum AS varchar) PayTerms,
        Ord.DocEntry,
        Ord.DocNum,
        Ord.DocDate,
		Ord.CardCode,
		Ord.CardName,		
		Ord.DocTotal AS DocTotal,
		Ord.DocTotalFC AS DocTotalFC,
		Ord.DocStatus,
		Ord.Comments AS Comment, 
		Ord.DocCur AS Currency,
		----Ord.U_Facturacion AS U_Facturacion,
		----Ord.U_FacturaVencida AS U_FacturaVencida,
		----ord.U_NVT_Medio_Pago,
		Oslp.SlpCode AS SlpCode,
		Oslp.SlpName AS SalesMan,
		Ord.U_TipoIdentificacion AS IdType,
		Ord.U_NumIdentFE AS Identification,
		Ord.U_CorreoFE AS Email,
		Ord.U_ObservacionFE AS U_ObservacionFE,
		----Ord.U_Almacen,
		----Bp.QryGroup1 AS QryGroup1,
		----CAST (ISNULL(Ord.NumAtCard, 0 ) AS INT) AS NumAtCard,
		COALESCE(Ord.NumAtCard, '') AS NumAtCard,
		Ord.U_TipoDocE AS DocumentType
    FROM           
        dbo.ORDR AS Ord WITH (NOLOCK)
    JOIN
        dbo.OSLP AS Oslp WITH (NOLOCK) ON Ord.SlpCode = Oslp.SlpCode
	JOIN
		dbo.OCRD AS Bp WITH (NOLOCK) ON Ord.CardCode = Bp.CardCode
	WHERE		
		Ord.DocEntry = @DocEntry  
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_SALEORDEREDIT_LINES]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_SALEORDEREDIT_LINES]
@DocEntry int,
@CardCode varchar(30)
AS
BEGIN

	SELECT
        Lines.ItemCode,
        Lines.Dscription AS ItemName,
        CAST(Lines.Quantity as float) AS Quantity,
		CAST(Lines.PriceBefDi AS float) AS UnitPrice,
		--CAST(Item.LastPurPrc AS float) AS LastPurchasePrice,
		CAST(Lines.DiscPrcnt AS float) AS Discount,
		Lines.TaxCode,
		CAST(Lines.VatPrcnt AS float) AS TaxRate,
		Lines.WhsCode,
		WH.WhsName,
		Lines.BaseLine as BaseLine
		,CAST(Item.ItmsGrpCod AS INT) AS ItemGroupCode
		,Lines.LineNum,
		Lines.BaseEntry,
		Lines.BaseType,
		Lines.LineStatus,
		--COALESCE(Lines.LineStatus, 'O') as LineStatus,
		--COALESCE(Lines.U_NVT_ServicioMedico, 0) as U_NVT_ServicioMedico,
		--[dbo].[CLVS_EMA_GETTAXCODE](@CardCode, Lines.ItemCode) AS TaxCode_BCK,
		--st.Rate as TaxRate_BCK
		----[dbo].[CLVS_EMA_GETTAXCODE](@CardCode, Lines.ItemCode) AS TaxRate_BCK
		(SELECT SIT.AvgPrice FROM OITW SIT WHERE SIT.ItemCode= Item.ItemCode AND SIT.WhsCode= Lines.WhsCode) AS LastPurchasePrice
    FROM
        dbo.RDR1 AS Lines WITH (NOLOCK)
    JOIN dbo.OITM AS Item WITH (NOLOCK) ON Lines.ItemCode = Item.ItemCode
	JOIN dbo.ITM1 AS PriceList WITH (NOLOCK) ON Lines.ItemCode = PriceList.ItemCode
	JOIN dbo.OCRD AS Customer WITH (NOLOCK) ON PriceList.PriceList = Customer.ListNum
	JOIN dbo.OWHS WH ON WH.WhsCode = Lines.WhsCode
	--LEFT JOIN OSTC ST ON ST.Code = [dbo].[CLVS_EMA_GETTAXCODE](@CardCode, Lines.ItemCode)
	WHERE		
		Lines.DocEntry = @DocEntry AND
        Customer.CardCode = @CardCode 
		--AND Lines.LineStatus = 'O'	
END

GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_SALEORDERS]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_SALEORDERS]
@SlpCode int,
@DocStatus VARCHAR(2),
@DocNum int,
@CardCode VARCHAR(10),
@Fini date,
@Ffin date
--@U_Almacen NVARCHAR(10)
AS
BEGIN
	SELECT
        SOrder.DocEntry,
        SOrder.DocNum,
        SOrder.DocDate,
		SOrder.CardCode,
		SOrder.CardName,		
		SOrder.DocTotal AS DocTotal,
		SOrder.DocTotalFC AS DocTotalFC,
		SOrder.DocStatus,
		Oslp.SlpCode AS SlpCode,
		Oslp.SlpName AS SalesMan,
        (SELECT DISTINCT TOP(1) Inv.DocNum 
	     FROM OINV AS Inv
		 JOIN INV1 AS InvLine WITH (NOLOCK) ON Inv.DocEntry = InvLine.DocEntry
		 JOIN ORDR AS SOrder2 WITH (NOLOCK) ON InvLine.BaseEntry = SOrder2.DocEntry
		 WHERE InvLine.BaseType = 17 AND
			SOrder2.DocEntry = SOrder.DocEntry) AS InvDocNum,
		SOrder.Comments,
		SOrder.DocCur as Currency,
		--SOrder.U_Almacen,
		SOrder.U_TipoDocE DocumentType
		--SOrder.U_Facturacion
    FROM           
        dbo.ORDR AS SOrder WITH (NOLOCK)
    JOIN
        dbo.OSLP AS Oslp WITH (NOLOCK) ON SOrder.SlpCode = Oslp.SlpCode
	WHERE
		SOrder.DocDate BETWEEN @Fini AND @Ffin AND
		SOrder.DocStatus = (CASE WHEN LEN(@DocStatus) > 0 THEN @DocStatus ELSE SOrder.DocStatus END) AND
		SOrder.DocNum = (CASE WHEN @DocNum > 0 THEN @DocNum ELSE SOrder.DocNum END) AND
		SOrder.CardCode = (CASE WHEN LEN(@CardCode) > 0 THEN @CardCode ELSE SOrder.CardCode END) AND
		--SOrder.U_Almacen = @U_Almacen AND
		Oslp.SlpCode = (CASE WHEN @SlpCode > 0 THEN @SlpCode ELSE Oslp.SlpCode END)
	ORDER BY 
		SOrder.DocEntry
END;
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_UDFS]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_UDFS]
@CATEGORY VARCHAR(4),
@PageNumber INT,
@RowsOfPage INT
AS
BEGIN
	SELECT
		0 Id,
		TableID,
		CONCAT('U_', T0.AliasID) [Name],
		Descr as [Description],

		(CASE 
			WHEN T0.TypeID = 'A' OR T0.TypeID = 'M' THEN 'String'
			WHEN T0.TypeID = 'B' THEN 'Double'
			WHEN T0.TypeID = 'N' THEN 'Int32'
			WHEN T0.TypeID = 'D' THEN 'DateTime'
			ELSE 'db_set'
		END) FieldType,
		[dbo].[CLVS_D_EMA_SLT_UDFVALIDVALUES](T0.TableID, T0.FieldID) [Values],
		0 CompanyId
	FROM
		CUFD T0
	WHERE
		T0.TableID = @CATEGORY
	ORDER BY TableID
	OFFSET (@PageNumber-1)*@RowsOfPage ROWS
	FETCH NEXT @RowsOfPage ROWS ONLY

	 SELECT count(*) AS Size FROM CUFD T0 WHERE T0.TableID = @CATEGORY

END;


GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_USRBALANCE]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Alan Jose Arias Herrera>
-- alter date: <2019/08/26>
-- Description:	<SP para el cierre de caja>
-- =============================================
CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_USRBALANCE]
@FIni VARCHAR(20),
@FFin VARCHAR(20),
@SlpCode int
AS
BEGIN
	SET NOCOUNT ON;
	(SELECT 
	DATEADD(SECOND, 
		(CASE LEN(Pay."DocTime") 
			WHEN '4' THEN ((LEFT(Pay."DocTime",	2) * 3600) + (RIGHT(Pay."DocTime",2) * 60))
			WHEN '3' THEN ((LEFT(Pay."DocTime",	1) * 3600) + (RIGHT(Pay."DocTime",2) * 60))
			WHEN '2' THEN (RIGHT(Pay."DocTime",2) * 60)
			WHEN '1' THEN (RIGHT(Pay."DocTime",1) * 60) 
			ELSE '0' END), Pay."DocDate") AS "DocDate",
		CONVERT(VARCHAR,Pay."DocNum") AS "DocNumP", 
		CONVERT(VARCHAR,Fact."DocNum") AS "DocNumF",
		CONVERT(VARCHAR,Fact."DocEntry") AS "DocEntF", 
		Pay."DocCurr" AS "DocCur",
		Pay."CardName",
		ISNULL((CASE Lines."InvType" WHEN 13 THEN 'Factura' WHEN 203 THEN 'Anticipo' WHEN NULL THEN 'Cuenta' END),'Cuenta') AS "PayType",
		0 "Balance",
		(CASE Pay."DocCurr" 
			WHEN 'COL' THEN (MAX(Pay."CashSum")+MAX(Pay."TrsfrSum")+MAX(Pay."CheckSum")+MAX(Pay."CreditSum"))
			WHEN 'USD' THEN (MAX(Pay."CashSumFC")+MAX(Pay."TrsfrSumFC")+MAX(Pay."CheckSumFC")+MAX(Pay."CredSumFC")) END) AS "PayTotal",
		MAX(Pay."CashSum") AS "CashSum",
		MAX(Pay."CashSumFC") AS "CashSumFC",
		MAX(Pay."TrsfrSum") AS "TrsfrSum",
		MAX(Pay."TrsfrSumFC") AS "TrsfrSumFC",
		MAX(Pay."CheckSumFC") AS "CheckSumFC", 
		MAX(Pay."CheckSum") AS "CheckSum",
		MAX(Pay."CreditSum") AS "CreditSum", 
		MAX(Pay."CredSumFC") AS "CredSumFC", 
		COUNT(Lines."DocEntry") AS "TotalDoc",
		--WHEN Fact."DocCur"='USD' AND Pay."DocCurr"='COL' THEN (SUM(FLines."U_DevImpuesto")*FACT."DocRate")
		--WHEN Fact."DocCur"='COL' AND Pay."DocCurr"='USD' THEN (SUM(FLines."U_DevImpuesto")/Pay."DocRate")
		--ELSE SUM(FLines."U_DevImpuesto")
		--END) "CardsIVA", 
		Fact."DocCur" AS DocCurrInv, 
		Fact."DocRate" AS DocRateInv
	FROM ORCT Pay
	LEFT JOIN RCT2 Lines
	ON Pay."DocEntry"=Lines."DocNum"
	LEFT JOIN OINV Fact
	ON Lines."DocEntry"=Fact."DocEntry"
	LEFT JOIN INV1 FLines
	ON Fact."DocEntry"=FLines."DocEntry"
	--JOIN OUSR users 
	--ON users."INTERNAL_K" = pay."UserSign"
	WHERE
	Fact."Canceled"='N' 
	 AND (DATEADD(SECOND, 
		(CASE LEN(Pay."DocTime") 
		WHEN '4' THEN ((LEFT(Pay."DocTime",	2) * 3600) + (RIGHT(Pay."DocTime",2) * 60))
		WHEN '3' THEN ((LEFT(Pay."DocTime",	1) * 3600) + (RIGHT(Pay."DocTime",2) * 60))
		WHEN '2' THEN (RIGHT(Pay."DocTime",2) * 60)
		WHEN '1' THEN (RIGHT(Pay."DocTime",1) * 60) 
		ELSE '0' END), Pay."DocDate") BETWEEN @FIni AND @FFin)
   AND Fact."SlpCode"= CAST(@SlpCode AS CHAR)
 -- AND Pay."U_UDF_NombreCajero" = CAST(SlpCode AS CHAR)
 -- AND (users."USER_CODE"='manager' OR users."USER_CODE"='cl.clavis.iis' OR users."USER_CODE"='gm.misposicem' OR users."USER_CODE"='gm.misposicem2')
	GROUP BY Fact."DocNum", Pay."DocNum", Fact."DocEntry", Pay."DocCurr", Pay."CardName",Pay."DocDate", Pay."CreditSum", Pay."DocTime", Lines."InvType", Fact."DocCur",  Fact."DocRate")
	UNION
	   (SELECT 
	   DATEADD(SECOND, 
			(CASE LEN(Fact."DocTime") 
				WHEN '4' THEN ((LEFT(Fact."DocTime",	2) * 3600) + (RIGHT(Fact."DocTime",2) * 60))
				WHEN '3' THEN ((LEFT(Fact."DocTime",	1) * 3600) + (RIGHT(Fact."DocTime",2) * 60))
				WHEN '2' THEN (RIGHT(Fact."DocTime",2) * 60)
				WHEN '1' THEN (RIGHT(Fact."DocTime",1) * 60) 
				ELSE '0' END), Fact."DocDate") AS "DocDate",
			'' AS "DocNumP",
			CONVERT(VARCHAR,Fact."DocNum") AS "DocNumF",
			Fact."DocEntry" AS "DocEntF",
			Fact."DocCur",
			Fact."CardName",
			'Crédito' AS "PayType",
			(CASE Fact."DocCur" 
				WHEN 'COL' THEN ("DocTotal"-"PaidToDate") 
				WHEN 'USD' THEN ("DocTotalFC"-"PaidFC") END) "Balance",
			0 AS "PayTotal",
			0 AS "CashSum",
			0 AS "CashSumFC",
			0 AS "TrsfrSum",
			0 AS "TrsfrSumFC",
			0 AS "CheckSumFC", 
			0 AS "CheckSum",
			0 AS "CreditSum",
			0 AS "CredSumFC",
			1 AS "TotalDoc",
			--0 "CardsIVA", 
			Fact."DocCur" AS DocCurrInv, 
			Fact."DocRate" AS DocRateInv
	FROM OINV Fact
	LEFT JOIN OUSR users 
	ON users."INTERNAL_K" = Fact."UserSign"
	/*LEFT JOIN INV1 FLines
	ON Fact."DocEntry"=FLines."DocEntry"*/
	WHERE
    Fact."CANCELED"='N' AND
	((Fact."DocTotal" > Fact."PaidToDate") OR (Fact."DocTotalFC" > Fact."PaidFC"))
	AND (DATEADD(SECOND, 
		(CASE LEN(Fact."DocTime") 
		WHEN '4' THEN ((LEFT(Fact."DocTime",	2) * 3600) + (RIGHT(Fact."DocTime",2) * 60))
		WHEN '3' THEN ((LEFT(Fact."DocTime",	1) * 3600) + (RIGHT(Fact."DocTime",2) * 60))
		WHEN '2' THEN (RIGHT(Fact."DocTime",2) * 60)
		WHEN '1' THEN (RIGHT(Fact."DocTime",1) * 60) 
		ELSE '0' END), Fact."DocDate") BETWEEN @FIni AND @FFin)
	AND Fact."SlpCode"= CAST(@SlpCode AS CHAR)
	--AND (users."USER_CODE"='manager' OR users."USER_CODE"='cl.clavis.iis' 
	--OR users."USER_CODE"='gm.misposicem' OR users."USER_CODE"='gm.misposicem2'));
	)
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_USRBALANCECREDITNOTES]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--SELECT * FROM ORIN
--where DocDate > '2020-08-18'
--order by DocEntry

CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_USRBALANCECREDITNOTES]
@FIni VARCHAR(20),
@FFin VARCHAR(20)
AS
BEGIN
	SET NOCOUNT ON;
	(SELECT CreditNote.DocDate AS "DocDate",
		CONVERT(VARCHAR,0) AS "DocNumP", 
		CONVERT(VARCHAR,CreditNote."DocNum") AS "DocNumF",
		CONVERT(VARCHAR,CreditNote."DocEntry") AS "DocEntF", 
		CreditNote."DocCur" AS "DocCur",
		CreditNote."CardName",
		'Nota de Credito' AS "PayType",
		0 "Balance",
		(CASE CreditNote."DocCur" 
			WHEN 'COL' THEN (CreditNote.DocTotal)
			WHEN 'USD' THEN (CreditNote.DocTotalFC) END) AS "PayTotal",
		0 AS "CashSum",
		0 AS "CashSumFC",
		0 AS "TrsfrSum",
		0 AS "TrsfrSumFC",
		0 AS "CheckSumFC", 
		0 AS "CheckSum",
		0 AS "CreditSum", 
		0 AS "CredSumFC", 
		0 AS "TotalDoc"
	FROM ORIN CreditNote
	WHERE
	 CreditNote."DocStatus"='C' 
	 AND (CreditNote."DocDate" BETWEEN CONVERT(DATE, @FIni) AND CONVERT(DATE, @FFin))
	)
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_VALIDATE_DEVIATION]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Alejandro Mora P.>
-- Create date: <08/09/2021>
-- Description:	<Valida si un producto cuenta con desviacion en su precio a la hora de realizar el ajuste de inventario desde facturacion>
-- =============================================
CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_VALIDATE_DEVIATION]
	@ItemCode VARCHAR(30),
	@AvgPrice INT,
	@LastPPrice INT
AS
BEGIN

	DECLARE 
		@maxAcceptedDeviation FLOAT,
		@minAcceptedDeviation FLOAT,
		@DeviationPercent FLOAT,
		@Message VARCHAR(50)

	SET @DeviationPercent = 20;
	SET @Message = '';

	SET @maxAcceptedDeviation = (@AvgPrice + (@AvgPrice*(@DeviationPercent/100)));
	SET @minAcceptedDeviation = (@AvgPrice - (@AvgPrice*(@DeviationPercent/100)));
	
	SET NOCOUNT ON;
	
	SELECT
		(CASE
			WHEN (@LastPPrice>@maxAcceptedDeviation OR @LastPPrice<@minAcceptedDeviation) THEN 0
			ELSE 1
			END
		) AS 'DeviationStatus',
		(CASE
			WHEN (@LastPPrice>@maxAcceptedDeviation OR @LastPPrice<@minAcceptedDeviation) THEN CONCAT('Este articulo se excede en un ',@DeviationPercent,'%')
			ELSE @Message
			END) AS 'Message';

	

END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_D_EMA_SLT_VOUCHERDETAIL]    Script Date: 6/1/2022 15:01:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_D_EMA_SLT_VOUCHERDETAIL]
@DocEntry int
AS
BEGIN
	SELECT TOP 1
		[IN].DocEntry
		,CRC.CardName as CardName
		,CT3.CrCardNum + 'encrypted' AS NumberCard
		,CAST([IN].DocNum AS VARCHAR) AS InvoiceNumber
		,[IN].DocDate DocumentDate
		,CT3.OwnerIdNum AS [Authorization]
		,'HardCoded_Terminal' AS Terminal
		,CT3.CreditSum AS Amount
		--,SUM(CT3.CreditSum) as Total
	FROM OINV [IN]
	JOIN RCT2 CT on CT.DocEntry = [IN].DocEntry
	JOIN orct RC on RC.DocNum = CT.DocNum
	JOIN RCT3 CT3 on CT3.DocNum = rc.DocEntry
	JOIN ACRC CRC ON CRC.CreditCard = CT3.CreditCard
	where [IN].DocEntry=@DocEntry
END
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[18] 4[30] 2[36] 3) )"
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
         Begin Table = "OPLN (dbo)"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 208
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
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 3225
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CLVS_D_EMA_SLT_AllPRICELIST'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CLVS_D_EMA_SLT_AllPRICELIST'
GO

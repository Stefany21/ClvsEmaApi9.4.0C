USE [SBO_CL_SUPERLT]
GO
/****** Object:  View [dbo].[CLVS_FE_SYNC_SN43]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[CLVS_FE_SYNC_SN43] AS
SELECT
'SUPERTORM' "Compannia"
,T0."CardCode" "ClienteOCRD"
,T0."CardName" "NombreClienteOCRD"
,T0."FatherCard" AS "ClienteConsolidador"
,(SELECT    A."CardName" FROM OCRD A WHERE A."CardCode" = T0."FatherCard") "NombreClienteConsolidador"
,(SELECT LEFT(A."LicTradNum",10) FROM OCRD A WHERE A."CardCode" = T0."FatherCard") "CedulaClienteConsolidador" 

,CASE
WHEN T0."U_TipoIdentificacion" = '' OR T0."U_TipoIdentificacion" IS NULL THEN '01' 
ELSE T0."U_TipoIdentificacion" 
END AS "TipoIdentificacion"

,CASE
WHEN T0."U_TipoIdentificacion" = '01' THEN LEFT(T0."LicTradNum",9)
WHEN T0."U_TipoIdentificacion" = '02' THEN LEFT(T0."LicTradNum",10)
WHEN T0."U_TipoIdentificacion" = '03' THEN T0."LicTradNum"
WHEN T0."U_TipoIdentificacion" = '04' THEN LEFT(T0."LicTradNum",10)
WHEN T0."U_TipoIdentificacion" = '99' THEN T0."LicTradNum"
WHEN T0."U_TipoIdentificacion" IS NULL THEN '000000000'
END AS "CedulaCliente"

,CASE
WHEN T0."Phone1" IS NULL THEN ''
ELSE LEFT(RTRIM(LTRIM(T0."Phone1")),15)
END AS "TelefonoCliente"

,CASE
WHEN CHARINDEX ('@',T0."E_Mail") > 0 THEN LEFT(RTRIM(LTRIM(T0."E_Mail")),80)
ELSE 'cguitierrez@clavisco.com'
END AS "EmailCliente"

,CASE
WHEN (T0."U_provincia" = '' OR T0."U_provincia" IS NULL OR CHARINDEX('-',T0."U_provincia") = 0) THEN '1'
ELSE LEFT(T0."U_provincia",CHARINDEX('-',T0."U_provincia")-1)
END AS "ProvinciaCliente"

,CASE
WHEN (T0."U_canton" = '' OR T0."U_canton" IS NULL OR CHARINDEX('-',T0."U_canton") = 0) THEN '1'
ELSE LEFT(T0."U_canton",CHARINDEX('-',T0."U_canton")-1)
END AS "CantonCliente"

,CASE
WHEN (T0."U_distrito" = '' OR T0."U_distrito" IS NULL OR CHARINDEX('-',T0."U_distrito") = 0) THEN '1'
ELSE LEFT(T0."U_distrito",CHARINDEX('-',T0."U_distrito")-1)
END AS "DistritoCliente"

-- BARRIO? FACEL NO LO PIDE

,CASE
WHEN (T0."U_direccion" = '' OR T0."U_direccion" IS NULL) THEN '' 
ELSE T0."U_direccion" 
END AS "DireccionCliente"

,T0."CreateDate" 
FROM OCRD T0 
Where T0."CardType" = 'C';
GO
/****** Object:  View [dbo].[CLVS_FE_SYNC_MEDIOPAGO43]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[CLVS_FE_SYNC_MEDIOPAGO43] AS
SELECT
'01' "DocType"
,T0."DocNum" "NumeroFactura"
,T0."DocEntry" "DocEntryFactura"
--,T0."DocTotal" 

--,T1."DocNum" 
--,T1."DocEntry"

,T2."DocNum" "NumeroPago"
,T2."DocEntry" "DocEntryPago"
,T2."CreditSum" "Tarjeta"
,T2."TrsfrSum" "Transferencia"
,T2."CashSum" "Efectivo"
,T2."CheckSum" "Cheque"
,CASE
WHEN T2."CreditSum" > 0 THEN '02'
WHEN T2."TrsfrSum" > 0 THEN '04'
WHEN T2."CashSum" > 0 THEN '01'
WHEN T2."CheckSum" > 0 THEN '03'
ELSE '04' END "MedioPago"

FROM OINV T0
LEFT JOIN RCT2 T1 ON T0."DocEntry" = T1."DocEntry"
LEFT JOIN ORCT T2 ON T1."DocNum" = T2."DocEntry"
WHERE T0."DocDate" >= '20190701';
GO
/****** Object:  View [dbo].[CLVS_FE_SYNC_CABBASE43]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO







CREATE VIEW [dbo].[CLVS_FE_SYNC_CABBASE43] AS
SELECT 
'01' AS "DocType"
,T0."DocEntry"
,T0."DocNum" AS "Consecutivo"
,1 AS "Sucursal"
,CASE T0."Series"
WHEN 4 THEN '1'
WHEN 75 THEN '2'
END AS "Terminal"
,1 AS "Situacion"
,T0."CardName" AS "RcprNombre"
,T0."DocDate"
,CASE LEN(T0."DocTime") WHEN 4 THEN LEFT(T0."DocTime",2) * 3600 WHEN 3 THEN LEFT(T0."DocTime", 1) * 3600 ELSE 0 END AS "HorasEnSegundos"
,RIGHT(T0."DocTime",2) * 60 AS "MinutosEnSegundos"
,CASE LEN(T0."DocTime") WHEN 4 THEN LEFT(T0."DocTime",2) * 3600 WHEN 3 THEN LEFT(T0."DocTime",1) * 3600 ELSE 0 END + RIGHT(T0."DocTime",2) * 60 AS "SegundosTotales"
,DATEDIFF (DAY,T0."DocDate",T0."DocDueDate") AS "PlazoCredito"
,CASE T0."DocCur" WHEN 'COL' THEN 'CRC' ELSE 'USD' END "CodigoMoneda"
,T0."DocRate" AS "TipoCambio"
,COALESCE(T0."U_TipoIdentificacion",T1."TipoIdentificacion") AS "RcprIdeTipo"
,COALESCE(T0."U_NumIdentFE",T1."CedulaCliente") AS "RcprIdeNumero"
,COALESCE(T0."U_CorreoFE",T1."EmailCliente",'') AS "RcprCorreoElectronico"
,CASE WHEN T0."DocTotal" - T0."PaidToDate" <=10 THEN '01' ELSE '02' END AS "CondicionVenta"
,(SELECT TOP 1 A."MedioPago" FROM CLVS_FE_SYNC_MEDIOPAGO43 A WHERE T0."DocEntry" = A."DocEntryFactura" AND '01' = A."DocType") "MedioPago"
,'' as "RcprNombreComercial" 
,T1."ProvinciaCliente" as "RcprUbProvincia"
,CASE LEN(T1."CantonCliente")
WHEN 1 THEN CONCAT(0,T1."CantonCliente")
ELSE T1."CantonCliente"
END as "RcprUbCanton"
,CASE LEN(T1."DistritoCliente")
WHEN 1 THEN CONCAT(0,T1."DistritoCliente")
ELSE T1."DistritoCliente"
END as "RcprUbDistrito"
,'' as "RcprUbBarrio"
,T1."DireccionCliente" as "RcprUbOtrasSenas"
,506 as "RcprTlfCodigoPais"
,REPLACE(T1."TelefonoCliente", '-', '') as "RcprTlfNumTelefono"
,506 as "RcprFaxCodigoPais"
,'' as "RcprFaxNumTelefono" 
--Tipo de documento de referencia. 01 Factura electrÃ³nica, 02 Nota de dÃ©bito electrÃ³nica, 03 nota de crÃ©dito electrÃ³nica, 04 Tiquete electrÃ³nico, 05 Nota de despacho, 06 Contrato, 07 Procedimiento, 08 Comprobante emitido en contigencia, 99 Otros
,null "InfRefTipoDoc"
,null "InfRefNumero"
,null "InfRefFechaEmision"
--CÃ³digo de referencia. 01 Anula documento de referencia, 02 Corrige texto de documento de referencia, 03 Corrige monto, 04 Referencia a otro documento, 05 Sustituye comprobante provisional por contigencia, 99 Otros 
,null "InfRefCodigo"
--TEXTO COMENTARIO ANULACION
,null "InfRefRazon"
,ISNULL(T0."U_ObservacionFE",'-') AS "Observacion" 
,COALESCE(T0.U_ClaveFE, T2."U_NClaveFE") As "ClaveE"
,COALESCE(T0.U_NumFE, T2."U_NConsecFE") As "ConsecutivoE"

FROM OINV T0 
LEFT JOIN CLVS_FE_SYNC_SN43 T1 
ON T0."CardCode"=T1."ClienteOCRD"
LEFT JOIN [@NCLAVEFE]  T2 
ON T0."DocEntry" = T2."Code" and T2."U_TipoDoc" = 'FE'
WHERE T0."DocDate" >= '20190701' 
AND T0."DocTotal" <> 0
AND T0."DocSubType" != 'DN'
AND T0."CANCELED" = 'N' 
AND T0."DocType" = 'I'
AND T0."Series" NOT IN (Select A."Series" from NNM1 A where A."SeriesName" IN ('NDSinFE','NCSinFE'))

UNION ALL

SELECT 
'02' AS "DocType"
,T0."DocEntry"
,T0."DocNum" AS "Consecutivo"
,1 AS "Sucursal"
,1 AS "Terminal"
,1 AS "Situacion"
,T0."CardName" AS "RcprNombre"
,T0."DocDate"
,CASE LEN(T0."DocTime") WHEN 4 THEN LEFT(T0."DocTime",2) * 3600 WHEN 3 THEN LEFT(T0."DocTime", 1) * 3600 ELSE 0 END AS "HorasEnSegundos"
,RIGHT(T0."DocTime",2) * 60 AS "MinutosEnSegundos"
,CASE LEN(T0."DocTime") WHEN 4 THEN LEFT(T0."DocTime",2) * 3600 WHEN 3 THEN LEFT(T0."DocTime",1) * 3600 ELSE 0 END + RIGHT(T0."DocTime",2) * 60 AS "SegundosTotales"
,DATEDIFF (DAY,T0."DocDate",T0."DocDueDate") AS "PlazoCredito"
,CASE T0."DocCur" WHEN 'COL' THEN 'CRC' ELSE 'USD' END "CodigoMoneda"
,T0."DocRate" AS "TipoCambio"
,COALESCE(T0."U_TipoIdentificacion",T1."TipoIdentificacion") AS "RcprIdeTipo"
,COALESCE(T0."U_NumIdentFE",T1."CedulaCliente") AS "RcprIdeNumero"
,COALESCE(T0."U_CorreoFE",T1."EmailCliente",'') AS "RcprCorreoElectronico"
,CASE WHEN T0."DocTotal" - T0."PaidToDate" <=10 THEN '01' ELSE '02' END AS "CondicionVenta"
,'04' as "MedioPago"
,'' as "RcprNombreComercial" 
,T1."ProvinciaCliente" as "RcprUbProvincia"
,CASE LEN(T1."CantonCliente")
WHEN 1 THEN CONCAT(0,T1."CantonCliente")
ELSE T1."CantonCliente"
END as "RcprUbCanton"
,CASE LEN(T1."DistritoCliente")
WHEN 1 THEN CONCAT(0,T1."DistritoCliente")
ELSE T1."DistritoCliente"
END as "RcprUbDistrito"
,'' as "RcprUbBarrio"
,T1."DireccionCliente" as "RcprUbOtrasSenas"
,506 as "RcprTlfCodigoPais"
,REPLACE(T1."TelefonoCliente", '-', '') as "RcprTlfNumTelefono"
,506 as "RcprFaxCodigoPais"
,'' as "RcprFaxNumTelefono" 
--Tipo de documento de referencia. 01 Factura electrÃ³nica, 02 Nota de dÃ©bito electrÃ³nica, 03 nota de crÃ©dito electrÃ³nica, 04 Tiquete electrÃ³nico, 05 Nota de despacho, 06 Contrato, 07 Procedimiento, 08 Comprobante emitido en contigencia, 99 Otros
,'01' "InfRefTipoDoc"
,T0."NumAtCard" "InfRefNumero"
,T0."DocDate" "InfRefFechaEmision"
--CÃ³digo de referencia. 01 Anula documento de referencia, 02 Corrige texto de documento de referencia, 03 Corrige monto, 04 Referencia a otro documento, 05 Sustituye comprobante provisional por contigencia, 99 Otros 
,'01' "InfRefCodigo"
--TEXTO COMENTARIO ANULACION
,ISNULL(T0."U_ObservacionFE",'-') AS "InfRefRazon"
,ISNULL(T0."U_ObservacionFE",'-') AS "Observacion"
,COALESCE(T0.U_ClaveFE, T2."U_NClaveFE") As "ClaveE"
,COALESCE(T0.U_NumFE, T2."U_NConsecFE") As "ConsecutivoE"

FROM OINV T0 
LEFT JOIN CLVS_FE_SYNC_SN43 T1 
ON T0."CardCode"=T1."ClienteOCRD" 
LEFT JOIN [@NCLAVEFE]  T2 
ON T0."DocEntry" = T2."Code" and T2."U_TipoDoc" = 'FE'
WHERE T0."DocDate" >= '20190701' 
AND T0."DocTotal" <> 0
AND T0."DocSubType" = 'DN' 
AND T0."CANCELED" = 'N' 
AND T0."DocType" = 'I'
AND T0."Series" NOT IN (Select A."Series" from NNM1 A where A."SeriesName" IN ('NDSinFE','NCSinFE'))

UNION ALL

SELECT 
'03' AS "DocType"
,T0."DocEntry"
,T0."DocNum" AS "Consecutivo"
,1 AS "Sucursal"
,1 AS "Terminal"
,1 AS "Situacion"
,T0."CardName" AS "RcprNombre"
,T0."DocDate"
,CASE LEN(T0."DocTime") WHEN 4 THEN LEFT(T0."DocTime",2) * 3600 WHEN 3 THEN LEFT(T0."DocTime", 1) * 3600 ELSE 0 END AS "HorasEnSegundos"
,RIGHT(T0."DocTime",2) * 60 AS "MinutosEnSegundos"
,CASE LEN(T0."DocTime") WHEN 4 THEN LEFT(T0."DocTime",2) * 3600 WHEN 3 THEN LEFT(T0."DocTime",1) * 3600 ELSE 0 END + RIGHT(T0."DocTime",2) * 60 AS "SegundosTotales"
,DATEDIFF (DAY,T0."DocDate",T0."DocDueDate") AS "PlazoCredito"
,CASE T0."DocCur" WHEN 'COL' THEN 'CRC' ELSE 'USD' END "CodigoMoneda"
,T0."DocRate" AS "TipoCambio"
,COALESCE(T0."U_TipoIdentificacion",T1."TipoIdentificacion") AS "RcprIdeTipo"
,COALESCE(T0."U_NumIdentFE",T1."CedulaCliente") AS "RcprIdeNumero"
,COALESCE(T0."U_CorreoFE",T1."EmailCliente",'') AS "RcprCorreoElectronico"
,CASE WHEN T0."DocTotal" - T0."PaidToDate" <=10 THEN '01' ELSE '02' END AS "CondicionVenta"
,'04' as "MedioPago"
,'' as "RcprNombreComercial" 
,T1."ProvinciaCliente" as "RcprUbProvincia"
,CASE LEN(T1."CantonCliente") WHEN 1 THEN CONCAT(0,T1."CantonCliente") ELSE T1."CantonCliente" END as "RcprUbCanton"
,CASE LEN(T1."DistritoCliente") WHEN 1 THEN CONCAT(0,T1."DistritoCliente") ELSE T1."DistritoCliente" END as "RcprUbDistrito"
,'' as "RcprUbBarrio"
,T1."DireccionCliente" as "RcprUbOtrasSenas"
,506 as "RcprTlfCodigoPais"
,REPLACE(T1."TelefonoCliente", '-', '') as "RcprTlfNumTelefono" 
,506 as "RcprFaxCodigoPais"
,'' as "RcprFaxNumTelefono" 
--Tipo de documento de referencia. 01 Factura electrÃ³nica, 02 Nota de dÃ©bito electrÃ³nica, 03 nota de crÃ©dito electrÃ³nica, 04 Tiquete electrÃ³nico, 05 Nota de despacho, 06 Contrato, 07 Procedimiento, 08 Comprobante emitido en contigencia, 99 Otros
,'01' "InfRefTipoDoc"
,T0."NumAtCard" "InfRefNumero"
,T0."DocDate" "InfRefFechaEmision"
--CÃ³digo de referencia. 01 Anula documento de referencia, 02 Corrige texto de documento de referencia, 03 Corrige monto, 04 Referencia a otro documento, 05 Sustituye comprobante provisional por contigencia, 99 Otros 
,'01' "InfRefCodigo"
--TEXTO COMENTARIO ANULACION
,ISNULL(T0."U_ObservacionFE",'-') AS "InfRefRazon"
,ISNULL(T0."U_ObservacionFE",'-') AS "Observacion"
,COALESCE(T0.U_ClaveFE, T2."U_NClaveFE") As "ClaveE"
,COALESCE(T0.U_NumFE, T2."U_NConsecFE") As "ConsecutivoE"

FROM ORIN T0 
LEFT JOIN CLVS_FE_SYNC_SN43 T1 
ON T0."CardCode"=T1."ClienteOCRD" 
LEFT JOIN [@NCLAVEFE]  T2 
ON T0."DocEntry" = T2."Code" and T2."U_TipoDoc" = 'NC'
WHERE T0."DocDate" >= '20190701'
AND T0."DocTotal" <> 0 
AND T0."CANCELED" = 'N' 
AND T0."DocType" = 'I'
AND T0."Series" NOT IN (Select A."Series" from NNM1 A where A."SeriesName" IN ('NDSinFE','NCSinFE'))
;
GO
/****** Object:  View [dbo].[CLVS_FE_SYNC_LINES43]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[CLVS_FE_SYNC_LINES43] AS
SELECT
--Lineas facturas
'01' "TipoDocumento"
,T0."DocEntry"
,T0."DocNum"
,T0."DocCur" "MonedaDocumento"

,(T1."LineNum" + 1) "NumeroLinea"
,'' "PartidaArancelaria"

,'9999999999999' "Codigo"
,'04' "CodTipo"
,LEFT(T1."ItemCode",20) as "CodCodigo"
,T1."Quantity" AS "Cantidad"
,CASE (SELECT T2."InvntItem" FROM OITM T2 WHERE T1."ItemCode"=T2."ItemCode") WHEN 'Y' THEN 'Unid' ELSE 'Sp' END "UnidadMedida"
,'' "UnidadMedidaComercial"
,T1."Dscription" as "Detalle"
,T1."PriceBefDi" AS "PrecioUnitario"
,CONVERT(DECIMAL(18,5),T1."PriceBefDi" * T1."Quantity") "MontoTotal"
,CONVERT(DECIMAL(18,5),(T1."PriceBefDi"*(COALESCE(T1."DiscPrcnt",0)/100))*T1."Quantity") AS "MontoDescuento"
,CASE WHEN T1."DiscPrcnt" <> 0 THEN 'Desc. comercial'
WHEN T1."DiscPrcnt" = 0 THEN 'NA'
WHEN T1."DiscPrcnt" IS NULL THEN 'NA'
END AS "NaturalezaDescuento"
,CONVERT(DECIMAL(18,5),(T1."PriceBefDi" -(T1."PriceBefDi" * (COALESCE(T1."DiscPrcnt",0)/100)))*T1."Quantity") as "SubTotal"
,'' "BaseImponible"
,CASE WHEN T1."TaxCode" IN ('EXE','EX') THEN NULL ELSE '01' END "ImpCodigo"
,CASE 
WHEN T1."TaxCode" in ('EXE','EX') THEN '01' 
WHEN T1."TaxCode" in ('1IVA') THEN '02'
WHEN T1."TaxCode" in ('2IVA') THEN '03'
WHEN T1."TaxCode" in ('4IVA') THEN '04'
WHEN T1."TaxCode" in ('13IVA','IV') THEN '08'
END AS "ImpCodigoTarifa"
,CASE 
WHEN T1."TaxCode" in ('EXE','EX') THEN 0.00
WHEN T1."TaxCode" in ('1IVA') THEN 1.00
WHEN T1."TaxCode" in ('2IVA') THEN 2.00
WHEN T1."TaxCode" in ('4IVA') THEN 4.00
WHEN T1."TaxCode" in ('13IVA','IV') THEN 13.00
END AS "ImpTarifa"
,'' "ImpFactorIVA" --Para la venta de bienes usados
,CASE T0."DocCur" WHEN 'COL' THEN T1."VatSum" ELSE T1."VatSumSy" END "ImpMonto"

,NULL as "ETipoDocumento"
,NULL as "ENumeroDocumento"
,NULL as "ENombreInstitucion"
,NULL as "EFechaEmision"
,NULL as "EPorcentajeExoneracion"
,NULL as "EMontoExoneracion"

,CASE T0."DocCur" WHEN 'COL' THEN T1."VatSum" ELSE T1."VatSumSy" END "ImpuestoNeto"

,CONVERT(DECIMAL(18,6),CASE T0."DocCur" WHEN 'COL' THEN (((T1."PriceBefDi" -(T1."PriceBefDi" * (T1."DiscPrcnt"/100))) * T1."Quantity")+ T1."VatSum") ELSE (((T1."PriceBefDi" -(T1."PriceBefDi" * (T1."DiscPrcnt"/100))) * T1."Quantity")+ T1."VatSumSy") END) "MontoTotalLinea"

--INFORMATIVO
,T1."PriceAfVAT" "PrecioCImpCdisc"
,COALESCE(T1."DiscPrcnt",0) AS "PorcentajeDescuento"
,'' As "OtrosDatos"

FROM OINV T0 
LEFT JOIN INV1 T1 ON T0."DocEntry" = T1."DocEntry" 
WHERE T0."DocDate" >= '20190701' 
AND T0."DocSubType" != 'DN' 
AND T0."CANCELED" = 'N' 
AND T0."DocType" = 'I'
AND T1."PriceBefDi" <> 0
AND T1."Quantity" <> 0
AND T0."Series" NOT IN (Select A."Series" from NNM1 A where A."SeriesName" IN ('NDSinFE','NCSinFE'))

UNION ALL

SELECT
--Lineas ND
'02' "TipoDocumento"
,T0."DocEntry"
,T0."DocNum"
,T0."DocCur" "MonedaDocumento"

,(T1."LineNum" + 1) "NumeroLinea"
,'' "PartidaArancelaria"

,'9999999999999' "Codigo"
,'04' "CodTipo"
,LEFT(T1."ItemCode",20) as "CodCodigo"
,T1."Quantity" AS "Cantidad"
,CASE (SELECT T2."InvntItem" FROM OITM T2 WHERE T1."ItemCode"=T2."ItemCode") WHEN 'Y' THEN 'Unid' ELSE 'Sp' END "UnidadMedida"
,'' "UnidadMedidaComercial"
,T1."Dscription" as "Detalle"
,T1."PriceBefDi" AS "PrecioUnitario"
,CONVERT(DECIMAL(18,5),T1."PriceBefDi" * T1."Quantity") "MontoTotal"
,CONVERT(DECIMAL(18,5),(T1."PriceBefDi"*(COALESCE(T1."DiscPrcnt",0)/100))*T1."Quantity") AS "MontoDescuento"
,CASE WHEN T1."DiscPrcnt" <> 0 THEN 'Desc. comercial'
WHEN T1."DiscPrcnt" = 0 THEN 'NA'
WHEN T1."DiscPrcnt" IS NULL THEN 'NA'
END AS "NaturalezaDescuento"
,CONVERT(DECIMAL(18,5),(T1."PriceBefDi" -(T1."PriceBefDi" * (COALESCE(T1."DiscPrcnt",0)/100)))*T1."Quantity") as "SubTotal"
,'' "BaseImponible"
,CASE WHEN T1."TaxCode" IN ('EXE','EX') THEN NULL ELSE '01' END "ImpCodigo"
,CASE 
WHEN T1."TaxCode" in ('EXE','EX') THEN '01' 
WHEN T1."TaxCode" in ('1IVA') THEN '02'
WHEN T1."TaxCode" in ('2IVA') THEN '03'
WHEN T1."TaxCode" in ('4IVA') THEN '04'
WHEN T1."TaxCode" in ('13IVA','IV') THEN '08'
END AS "ImpCodigoTarifa"
,CASE 
WHEN T1."TaxCode" in ('EXE','EX') THEN 0.00
WHEN T1."TaxCode" in ('1IVA') THEN 1.00
WHEN T1."TaxCode" in ('2IVA') THEN 2.00
WHEN T1."TaxCode" in ('4IVA') THEN 4.00
WHEN T1."TaxCode" in ('13IVA','IV') THEN 13.00
END AS "ImpTarifa"
,'' "ImpFactorIVA" --Para la venta de bienes usados
,CASE T0."DocCur" WHEN 'COL' THEN T1."VatSum" ELSE T1."VatSumSy" END "ImpMonto"

,NULL as "ETipoDocumento"
,NULL as "ENumeroDocumento"
,NULL as "ENombreInstitucion"
,NULL as "EFechaEmision"
,NULL as "EPorcentajeExoneracion"
,NULL as "EMontoExoneracion"

,CASE T0."DocCur" WHEN 'COL' THEN T1."VatSum" ELSE T1."VatSumSy" END "ImpuestoNeto"
,CONVERT(DECIMAL(18,6),CASE T0."DocCur" WHEN 'COL' THEN (((T1."PriceBefDi" -(T1."PriceBefDi" * (T1."DiscPrcnt"/100))) * T1."Quantity")+ T1."VatSum") ELSE (((T1."PriceBefDi" -(T1."PriceBefDi" * (T1."DiscPrcnt"/100))) * T1."Quantity")+ T1."VatSumSy") END) "MontoTotalLinea"

--INFORMATIVO
,T1."PriceAfVAT" "PrecioCImpCdisc"
,COALESCE(T1."DiscPrcnt",0) AS "PorcentajeDescuento"
,'' As "OtrosDatos"

FROM OINV T0 
LEFT JOIN INV1 T1 ON T0."DocEntry" = T1."DocEntry" 
WHERE T0."DocDate" >= '20190701' 
AND T0."DocSubType" = 'DN' 
AND T0."CANCELED" = 'N' 
AND T0."DocType" = 'I'
AND T1."PriceBefDi" <> 0
AND T1."Quantity" <> 0 
AND T0."Series" NOT IN (Select A."Series" from NNM1 A where A."SeriesName" IN ('NDSinFE','NCSinFE'))

UNION ALL

SELECT
--Lineas NC
'03' "TipoDocumento"
,T0."DocEntry"
,T0."DocNum"
,T0."DocCur" "MonedaDocumento"

,(T1."LineNum" + 1) "NumeroLinea"
,'' "PartidaArancelaria"

,'9999999999999' "Codigo"
,'04' "CodTipo"
,LEFT(T1."ItemCode",20) as "CodCodigo"
,T1."Quantity" AS "Cantidad"
,CASE (SELECT T2."InvntItem" FROM OITM T2 WHERE T1."ItemCode"=T2."ItemCode") WHEN 'Y' THEN 'Unid' ELSE 'Sp' END "UnidadMedida"
,'' "UnidadMedidaComercial"
,T1."Dscription" as "Detalle"
,T1."PriceBefDi" AS "PrecioUnitario"
,CONVERT(DECIMAL(18,5),T1."PriceBefDi" * T1."Quantity") "MontoTotal"
,CONVERT(DECIMAL(18,5),(T1."PriceBefDi"*(COALESCE(T1."DiscPrcnt",0)/100))*T1."Quantity") AS "MontoDescuento"
,CASE WHEN T1."DiscPrcnt" <> 0 THEN 'Desc. comercial'
WHEN T1."DiscPrcnt" = 0 THEN 'NA'
WHEN T1."DiscPrcnt" IS NULL THEN 'NA'
END AS "NaturalezaDescuento"
,CONVERT(DECIMAL(18,5),(T1."PriceBefDi" -(T1."PriceBefDi" * (COALESCE(T1."DiscPrcnt",0)/100)))*T1."Quantity") as "SubTotal"
,'' "BaseImponible"
,CASE WHEN T1."TaxCode" IN ('EXE','EX') THEN NULL ELSE '01' END "ImpCodigo"
,CASE 
WHEN T1."TaxCode" in ('EXE','EX') THEN '01' 
WHEN T1."TaxCode" in ('1IVA') THEN '02'
WHEN T1."TaxCode" in ('2IVA') THEN '03'
WHEN T1."TaxCode" in ('4IVA') THEN '04'
WHEN T1."TaxCode" in ('13IVA','IV') THEN '08'
END AS "ImpCodigoTarifa"
,CASE 
WHEN T1."TaxCode" in ('EXE','EX') THEN 0.00
WHEN T1."TaxCode" in ('1IVA') THEN 1.00
WHEN T1."TaxCode" in ('2IVA') THEN 2.00
WHEN T1."TaxCode" in ('4IVA') THEN 4.00
WHEN T1."TaxCode" in ('13IVA','IV') THEN 13.00
END AS "ImpTarifa"
,'' "ImpFactorIVA" --Para la venta de bienes usados
,CASE T0."DocCur" WHEN 'COL' THEN T1."VatSum" ELSE T1."VatSumSy" END "ImpMonto"

,NULL as "ETipoDocumento"
,NULL as "ENumeroDocumento"
,NULL as "ENombreInstitucion"
,NULL as "EFechaEmision"
,NULL as "EPorcentajeExoneracion"
,NULL as "EMontoExoneracion"

,CASE T0."DocCur" WHEN 'COL' THEN T1."VatSum" ELSE T1."VatSumSy" END "ImpuestoNeto"
,CONVERT(DECIMAL(18,6),CASE T0."DocCur" WHEN 'COL' THEN (((T1."PriceBefDi" -(T1."PriceBefDi" * (T1."DiscPrcnt"/100))) * T1."Quantity")+ T1."VatSum") ELSE (((T1."PriceBefDi" -(T1."PriceBefDi" * (T1."DiscPrcnt"/100))) * T1."Quantity")+ T1."VatSumSy") END) "MontoTotalLinea"

--INFORMATIVO
,T1."PriceAfVAT" "PrecioCImpCdisc"
,COALESCE(T1."DiscPrcnt",0) AS "PorcentajeDescuento"
,'' As "OtrosDatos"

FROM ORIN T0 
LEFT JOIN RIN1 T1 ON T0."DocEntry" = T1."DocEntry" 
WHERE T0."DocDate" >= '20190701' 
AND T0."CANCELED" = 'N' 
AND T0."DocType" = 'I'
AND T1."PriceBefDi" <> 0
AND T1."Quantity" <> 0
AND T0."Series" NOT IN (Select A."Series" from NNM1 A where A."SeriesName" IN ('NDSinFE','NCSinFE'))
;
GO
/****** Object:  View [dbo].[CLVS_FE_SYNC_DESCIMP43]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[CLVS_FE_SYNC_DESCIMP43] AS
SELECT
T0."TipoDocumento"
,T0."DocNum"

,SUM(CASE WHEN T0."ETipoDocumento" IS NOT NULL AND "UnidadMedida" = 'Sp' THEN "MontoTotal" ELSE 0 END) "VentasServEXO"
,SUM(CASE WHEN T0."ETipoDocumento" IS NOT NULL AND "UnidadMedida" = 'Sp' THEN "MontoDescuento" ELSE 0 END) "DescServEXO"
,SUM(CASE WHEN T0."ImpTarifa" > 0 AND "UnidadMedida" = 'Sp' THEN "MontoTotal" ELSE 0 END) "VentasServGRAV"
,SUM(CASE WHEN T0."ImpTarifa" > 0 AND "UnidadMedida" = 'Sp' THEN "MontoDescuento" ELSE 0 END) "DescServGRAV"
,SUM(CASE WHEN T0."ImpTarifa" <= 0 AND "UnidadMedida" = 'Sp' THEN "MontoTotal" ELSE 0 END) "VentasServEXE"
,SUM(CASE WHEN T0."ImpTarifa" <= 0 AND "UnidadMedida" = 'Sp' THEN "MontoDescuento" ELSE 0 END) "DescServEXE"

,SUM(CASE WHEN T0."ETipoDocumento" IS NOT NULL AND "UnidadMedida" != 'Sp' THEN "MontoTotal" ELSE 0 END) "VentasMercEXO"
,SUM(CASE WHEN T0."ETipoDocumento" IS NOT NULL AND "UnidadMedida" != 'Sp' THEN "MontoDescuento" ELSE 0 END) "DescMercEXO"
,SUM(CASE WHEN T0."ImpTarifa" > 0 AND "UnidadMedida" != 'Sp' THEN "MontoTotal" ELSE 0 END) "VentasMercGRAV"
,SUM(CASE WHEN T0."ImpTarifa" > 0 AND "UnidadMedida" != 'Sp' THEN "MontoDescuento" ELSE 0 END) "DescMercGRAV"
,SUM(CASE WHEN T0."ImpTarifa" <= 0 AND "UnidadMedida" != 'Sp' THEN "MontoTotal" ELSE 0 END) "VentasMercEXE"
,SUM(CASE WHEN T0."ImpTarifa" <= 0 AND "UnidadMedida" != 'Sp' THEN "MontoDescuento" ELSE 0 END) "DescMercEXE"

,SUM(T0."ImpMonto") "TotalImpuesto"
FROM CLVS_FE_SYNC_LINES43 T0 
GROUP BY T0."TipoDocumento",T0."DocNum"
;
GO
/****** Object:  View [dbo].[CLVS_FE_SYNC_CABECERA43]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[CLVS_FE_SYNC_CABECERA43] AS
SELECT

'SUPERTORM' "Comp"
,'012103' "CodigoActividad"
,DATEADD(SECOND,T0."SegundosTotales",T0."DocDate") "FechaFact"
,T0."DocType"
,T0."DocEntry"
,T0."Consecutivo"
,T0."Sucursal"
,T0."Terminal"
,T0."Situacion"

,T0."RcprNombre"
,T0."RcprIdeTipo" 
,T0."RcprIdeNumero"
,T0."RcprNombreComercial"
,CONVERT(VARCHAR,T0."RcprUbProvincia") "RcprUbProvincia"
,CONVERT(VARCHAR,T0."RcprUbCanton") "RcprUbCanton"
,CONVERT(VARCHAR,T0."RcprUbDistrito") "RcprUbDistrito"
,T0."RcprUbBarrio"
,T0."RcprUbOtrasSenas"
,T0."RcprTlfCodigoPais"
,CONVERT(VARCHAR,T0."RcprTlfNumTelefono") "RcprTlfNumTelefono"
,T0."RcprFaxCodigoPais"
,T0."RcprFaxNumTelefono"
,CASE T0."RcprCorreoElectronico" WHEN '' THEN 'cguitierrez@clavisco.com' ELSE T0."RcprCorreoElectronico" END AS "RcprCorreoElectronico"
,'' "RcprCorreoElectronicoCC"

,T0."CondicionVenta"
,T0."PlazoCredito"
,T0."MedioPago"

--RESUMEN
,T0."CodigoMoneda"
,T0."TipoCambio" 

,CONVERT(DECIMAL(18,6),T1."VentasServGRAV") AS "TotalServGravados"
,CONVERT(DECIMAL(18,6),T1."VentasServEXE") AS "TotalServExentos"
,CONVERT(DECIMAL(18,6),T1."VentasServEXO") AS "TotalServExonerado"

,CONVERT(DECIMAL(18,6),T1."VentasMercGRAV") AS "TotalMercanciasGravadas"
,CONVERT(DECIMAL(18,6),T1."VentasMercEXE") AS "TotalMercanciasExentas"
,CONVERT(DECIMAL(18,6),T1."VentasMercEXO") AS "TotalMercExonerada"

,CONVERT(DECIMAL(18,6),T1."VentasMercGRAV" + T1."VentasServGRAV") AS "TotalGravado"
,CONVERT(DECIMAL(18,6),T1."VentasMercEXE" + T1."VentasServEXE") AS "TotalExento"
,CONVERT(DECIMAL(18,6),T1."VentasMercEXO" + T1."VentasServEXO") AS "TotalExonerado"

,CONVERT(DECIMAL(18,6),
T1."VentasServGRAV" + T1."VentasServEXE" + T1."VentasServEXO" +
T1."VentasMercGRAV" + T1."VentasMercEXE" + T1."VentasMercEXO"
) AS "TotalVenta"

,CONVERT(DECIMAL(18,6),
T1."DescServGRAV" + T1."DescServEXE" +
T1."DescMercGRAV" + T1."DescMercEXE" 
) AS "TotalDescuentos"

,CONVERT(DECIMAL(18,6),
T1."VentasServGRAV" + T1."VentasServEXE" + T1."VentasServEXO" +
T1."VentasMercGRAV" + T1."VentasMercEXE" + T1."VentasMercEXO" -
T1."DescServGRAV" - T1."DescServEXE" -
T1."DescMercGRAV" - T1."DescMercEXE" 
) AS "TotalVentaNeta"

,CONVERT(DECIMAL(18,6),T1."TotalImpuesto") AS "TotalImpuesto"

,0 "TotalIVADevuelto"

,CONVERT(DECIMAL(18,6),
T1."VentasServGRAV" + T1."VentasServEXE" + T1."VentasServEXO" +
T1."VentasMercGRAV" + T1."VentasMercEXE" + T1."VentasMercEXO" -
T1."DescServGRAV" - T1."DescServEXE" -
T1."DescMercGRAV" - T1."DescMercEXE" +
T1."TotalImpuesto") AS "TotalComprobante"

,T0."InfRefTipoDoc"
,T0."InfRefNumero"
,T0."InfRefFechaEmision"
,T0."InfRefCodigo"
,T0."InfRefRazon"

,T0."Observacion"
,'' "OtrosDatos"
,T0."ClaveE"
,T0."ConsecutivoE"

FROM CLVS_FE_SYNC_CABBASE43 T0 
LEFT JOIN CLVS_FE_SYNC_DESCIMP43 T1 ON T0."DocType"=T1."TipoDocumento" AND T0."Consecutivo" = T1."DocNum" 
WHERE T0."DocDate" >= '20200330';


GO
/****** Object:  View [dbo].[CLVS_INVENTARIO_GEN_VENTAS_15D]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE   VIEW [dbo].[CLVS_INVENTARIO_GEN_VENTAS_15D] as
Select COALESCE(T2.ItemCode,T1.ItemCode) [ItemCode], --T1.ItemCode, T1.Quantity, T1.LineNum, T2.ItemCode, T2.TotalQty, T2.LineNum,
SUM(COALESCE(T2.TotalQty,T1.Quantity)) "Cant D ULT15"
FROM
OINV T0 
LEFT JOIN INV1 T1 ON T0."DocEntry" = T1."DocEntry"
LEFT JOIN INV14 T2 ON T1.DocEntry = T2.DocEntry AND T1.LineNum = T2.LineNum
WHERE 
--T0.DocEntry = 6686 AND 
T0."CANCELED" = 'N'
AND T0."DocDate" BETWEEN DATEADD(day,-15,GETDATE()) AND GETDATE()
GROUP BY
COALESCE(T2.ItemCode,T1.ItemCode) 
GO
/****** Object:  View [dbo].[CLVS_INVENTARIO_GEN_TEST]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW	[dbo].[CLVS_INVENTARIO_GEN_TEST] AS
SELECT
T0."WhsCode"    	            "Cod Almacen"
, T1."WhsName"    	            "Almacen"
, T5."FirmName"   	            "Marca"
, T2."ItmsGrpCod" 	            "Cod Grupo Articulos"
, T4."ItmsGrpNam" 	            "Grupo Articulos"
, T0."ItemCode"   	            "Cod Articulo"
, T2."CodeBars"					"Cod Barras"
, T2."ItemName"   	            "Articulo"
, CAST(T2."CreateDate" AS DATE)	"Fecha creacion articulo"
, T2.[InvntryUom]				"Unidad medida"
, T0."OnOrder"    	            "Cant en Pedido"
, T0."OnHand"     	            "Cant en Stock"
, T6."Price"					"Precio"
, CASE T2."U_IVA"
	WHEN '13IVA' THEN T6."Price"*1.13
	WHEN '2IVA' THEN T6."Price"*1.02
	WHEN 'EXE' THEN T6."Price"*1
	END [PrecioIVA]
, T2."U_IVA"					"Cod Imp"
, T2."U_proveedor"				"Proveedor"
, T7."CardName"					"Nombre proveedor"
, T0."AvgPrice"   	            "Costo Unitario"
, T0."OnHand"*T0."AvgPrice"	    "Costo Stock"
, T8."Cant D ULT15" "Cant vendida D ULT15"

FROM	    OITW	T0
LEFT JOIN	OWHS	T1	ON	T0."WhsCode"	=	T1."WhsCode"
LEFT JOIN	OITM	T2	ON	T0."ItemCode"	=	T2."ItemCode"
LEFT JOIN	OITB	T4	ON	T2."ItmsGrpCod"	=	T4."ItmsGrpCod"
LEFT JOIN	OMRC	T5	ON	T2."FirmCode"	=	T5."FirmCode"
LEFT JOIN	ITM1	T6	ON	T2."ItemCode"	=	T6."ItemCode" AND T6.PriceList = '01'
LEFT JOIN	OCRD	T7	ON	T2."U_proveedor"=	T7."CardCode"
LEFT JOIN	CLVS_INVENTARIO_GEN_VENTAS_15D T8 ON T2.ItemCode = T8.ItemCode
GO
/****** Object:  View [dbo].[CLVS_MAXPRICE_PDN_PCH]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[CLVS_MAXPRICE_PDN_PCH] AS
SELECT A.ItemCode, MAX(A.[PrecioMax]) [PrecioMax] FROM
(
SELECT [ItemCode], MAX([Price]) [PrecioMax] FROM PCH1
WHERE [DocDate] >= DATEADD(MONTH,-3,GETDATE())
GROUP BY ItemCode
UNION ALL
SELECT ItemCode, MAX(Price) [PrecioMax] FROM PDN1
WHERE [DocDate] >= DATEADD(MONTH,-3,GETDATE())
GROUP BY ItemCode
) A
GROUP BY A.ItemCode
GO
/****** Object:  View [dbo].[CLVS_ANAL_PRECIOS]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[CLVS_ANAL_PRECIOS] AS
SELECT 
T0.[CreateDate]
, T0.[UpdateDate]
, T0.[CodeBars]
, T0.[ItemCode]
, T0.[ItemName]
, T2.[AvgPrice] [Costo Prom]
--, T1.[PriceList]
--, T1.[Currency] 
, T4.[CardCode]
, T4.[CardName]
, T1.[Price]
, T0.[U_IVA]
, T0.[U_Color]
, CASE T3.PrecioMax WHEN 0 THEN 999 ELSE T1.[Price]/T3.PrecioMax END [Margen]

, T3.PrecioMax CostoMax
, CASE WHEN T3.[PrecioMax]=T2.[AvgPrice] THEN 'SI' ELSE 'NO' END AS [CostoEstable]
,T4.[U_margen] [MargenProveedor]
, CASE T0.[U_IVA] 
WHEN '13IVA' THEN ROUND((T3.[PrecioMax]*(1+(T4.[U_margen]/100)) * 1.13)/25,0,0)*25/1.13
WHEN '2IVA' THEN ROUND((T3.[PrecioMax]*(1+(T4.[U_margen]/100)) * 1.02)/25,0,0)*25/1.02
WHEN '1IVA' THEN ROUND((T3.[PrecioMax]*(1+(T4.[U_margen]/100)) * 1.01)/25,0,0)*25/1.01
WHEN 'EXE' THEN ROUND((T3.[PrecioMax]*(1+(T4.[U_margen]/100)) * 1.00)/25,0,0)*25/1.00
ELSE 99999 END [Precio Sugerido]

, CASE T0.[U_IVA] 
WHEN '13IVA' THEN T1.[Price]*1.13 
WHEN '2IVA' THEN T1.[Price]*1.02
WHEN '1IVA' THEN T1.[Price]*1.01
WHEN 'EXE' THEN T1.[Price]
ELSE 99999 END [Precio Consum]

FROM OITM T0  
LEFT JOIN ITM1 T1 ON T0.[ItemCode] = T1.[ItemCode]
LEFT JOIN OITW T2 ON T0.[ItemCode] = T2.[ItemCode]
LEFT JOIN CLVS_MAXPRICE_PDN_PCH T3 ON T0.[ItemCode] = T3.[ItemCode]
LEFT JOIN OCRD T4 ON T0.[U_proveedor] = T4.[CardCode]
WHERE T1.[PriceList] = 1
AND T0.[ItmsGrpCod] NOT IN (101)

GO
/****** Object:  View [dbo].[CLVS_IMS_CALENDAR_ARTICULO]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CLVS_IMS_CALENDAR_ARTICULO] AS
Select calendar.fecha, oitm.ItemCode from
(Select distinct docdate Fecha from oinv
where DocDate >= DATEADD(day,-15,GETDATE())) [Calendar], oitm
GO
/****** Object:  View [dbo].[CLVS_IMS_VENTA_ARTICULO]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CLVS_IMS_VENTA_ARTICULO] AS
Select DocDate,ItemCode,SUM(Quantity) qv from
INV1
Group by DocDate,ItemCode;
GO
/****** Object:  View [dbo].[CLVS_IMS_VENTADIARIA_ARTICULO]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CLVS_IMS_VENTADIARIA_ARTICULO] AS
Select CALENDAR.Fecha, CALENDAR.ItemCode, COALESCE(QV.qv,0) qv
FROM
CLVS_IMS_CALENDAR_ARTICULO CALENDAR
LEFT JOIN CLVS_IMS_VENTA_ARTICULO QV on CALENDAR.Fecha = QV.DocDate AND CALENDAR.ItemCode = QV.ItemCode
--WHERE CALENDAR.itemcode in ('100001','100002','100512','100527','101560') 
--ORDER BY calendar.itemcode, calendar.Fecha
GO
/****** Object:  View [dbo].[CLVS_IMS_STDEV_ARTICULO]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CLVS_IMS_STDEV_ARTICULO] AS
SELECT ItemCode, stdev(qv) [STDEV]
FROM CLVS_IMS_VENTADIARIA_ARTICULO
Group by ItemCode
GO
/****** Object:  View [dbo].[CLVS_INVENTARIO_GEN]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW	[dbo].[CLVS_INVENTARIO_GEN] AS
SELECT
T0."WhsCode"    	            "Cod Almacen"
, T1."WhsName"    	            "Almacen"
, T5."FirmName"   	            "Marca"
, T2."ItmsGrpCod" 	            "Cod Grupo Articulos"
, T4."ItmsGrpNam" 	            "Grupo Articulos"
, T0."ItemCode"   	            "Cod Articulo"
, T2."CodeBars"					"Cod Barras"
, T2."ItemName"   	            "Articulo"
, CAST(T2."CreateDate" AS DATE)	"Fecha creacion articulo"
, T2.[InvntryUom]				"Unidad medida"
, T0."OnOrder"    	            "Cant en Pedido"
, T0."OnHand"     	            "Cant en Stock"
, T8."STDEV"
, T6."Price"					"Precio"
, CASE T2."U_IVA"
	WHEN '13IVA' THEN T6."Price"*1.13
	WHEN '2IVA' THEN T6."Price"*1.02
	WHEN 'EXE' THEN T6."Price"*1
	END [PrecioIVA]
, T2."U_IVA"					"Cod Imp"
, T2."U_proveedor"				"Proveedor"
, T7."CardName"					"Nombre proveedor"
, T0."AvgPrice"   	            "Costo Unitario"
, T0."OnHand"*T0."AvgPrice"	    "Costo Stock"
, T3."Cant M-3" "Cant vendida M-3"
, T3."Cant M-2" "Cant vendida M-2"
, T3."Cant M-1" "Cant vendida M-1"
, T3."Cant M ACT" "Cant vendida M ACT"
, T3."Cant M ULT3" "Cant vendida M ULT3"
, T3."Cant D ULT15" "Cant vendida D ULT15"
, T3."Cost D ULT15" "Cost ventas D ULT15"

FROM	    OITW	T0
LEFT JOIN	OWHS	T1	ON	T0."WhsCode"	=	T1."WhsCode"
LEFT JOIN	OITM	T2	ON	T0."ItemCode"	=	T2."ItemCode"
LEFT JOIN
(
SELECT A."ItemCode", A."WhsCode",
SUM(A."Cant M-3") "Cant M-3",
SUM(A."Cant M-2") "Cant M-2",
SUM(A."Cant M-1") "Cant M-1" ,
SUM(A."Cant M ACT") "Cant M ACT",
SUM(A."Cant M ULT3") "Cant M ULT3",
SUM(A."Cant D ULT15") "Cant D ULT15",
SUM(A."Cost D ULT15") "Cost D ULT15"
FROM
(
SELECT
T1."ItemCode", T1."WhsCode",
SUM( CASE WHEN FORMAT( T0."DocDate",'YYYYMM') BETWEEN  FORMAT(DATEADD(month,-3,GETDATE()),'YYYYMM') AND  FORMAT(DATEADD(month,-3,GETDATE()),'YYYYMM') THEN T1."Quantity" ELSE 0 END)	 "Cant M-3",
SUM( CASE WHEN FORMAT( T0."DocDate",'YYYYMM') BETWEEN  FORMAT(DATEADD(month,-2,GETDATE()),'YYYYMM') AND  FORMAT(DATEADD(month,-2,GETDATE()),'YYYYMM') THEN T1."Quantity" ELSE 0 END)	 "Cant M-2",
SUM( CASE WHEN FORMAT( T0."DocDate",'YYYYMM') BETWEEN  FORMAT(DATEADD(month,-1,GETDATE()),'YYYYMM') AND  FORMAT(DATEADD(month,-1,GETDATE()),'YYYYMM') THEN T1."Quantity" ELSE 0 END)	 "Cant M-1",
SUM( CASE WHEN FORMAT( T0."DocDate",'YYYYMM') =  FORMAT(GETDATE(),'YYYYMM') THEN T1."Quantity" ELSE 0 END)	 "Cant M ACT",
SUM( CASE WHEN FORMAT( T0."DocDate",'YYYYMM') BETWEEN  FORMAT(DATEADD(month,-3,GETDATE()),'YYYYMM') AND  FORMAT(DATEADD(month,-1,GETDATE()),'YYYYMM') THEN T1."Quantity" ELSE 0 END )	 "Cant M ULT3",
SUM( CASE WHEN T0."DocDate" BETWEEN DATEADD(day,-15,GETDATE()) AND GETDATE() THEN T1."Quantity" ELSE 0 END)	 "Cant D ULT15",
SUM( CASE WHEN T0."DocDate" BETWEEN DATEADD(day,-15,GETDATE()) AND GETDATE() THEN T1."Quantity"*T1."StockPrice" ELSE 0 END)	 "Cost D ULT15"
FROM
OINV T0 INNER JOIN INV1 T1 ON T0."DocEntry" = T1."DocEntry"
WHERE
T1."Quantity" > 0 AND T0."CANCELED" = 'N'
GROUP BY
T1."ItemCode", T1."WhsCode"

UNION ALL

SELECT
T1."ItemCode", T1."WhsCode",
SUM( CASE WHEN FORMAT( T0."DocDate",'YYYYMM') BETWEEN  FORMAT(DATEADD(month,-3,GETDATE()),'YYYYMM') AND  FORMAT(DATEADD(month,-3,GETDATE()),'YYYYMM') THEN T1."Quantity" ELSE 0 END)*-1	 "Cant M-3",
SUM( CASE WHEN FORMAT( T0."DocDate",'YYYYMM') BETWEEN  FORMAT(DATEADD(month,-2,GETDATE()),'YYYYMM') AND  FORMAT(DATEADD(month,-2,GETDATE()),'YYYYMM') THEN T1."Quantity" ELSE 0 END)*-1	 "Cant M-2",
SUM( CASE WHEN FORMAT( T0."DocDate",'YYYYMM') BETWEEN  FORMAT(DATEADD(month,-1,GETDATE()),'YYYYMM') AND  FORMAT(DATEADD(month,-1,GETDATE()),'YYYYMM') THEN T1."Quantity" ELSE 0 END)*-1	 "Cant M-1",
SUM( CASE WHEN FORMAT( T0."DocDate",'YYYYMM') =  FORMAT(GETDATE(),'YYYYMM') THEN T1."Quantity" ELSE 0 END)*-1	 "Cant M ACT",
SUM( CASE WHEN FORMAT( T0."DocDate",'YYYYMM') BETWEEN  FORMAT(DATEADD(month,-3,GETDATE()),'YYYYMM') AND  FORMAT(DATEADD(month,-3,GETDATE()),'YYYYMM') THEN T1."Quantity" ELSE 0 END)*-1	 "Cant M ULT3",
SUM( CASE WHEN T0."DocDate" BETWEEN DATEADD(day,-15,GETDATE()) AND GETDATE() THEN T1."Quantity" ELSE 0 END)*-1	 "Cant D ULT15",
SUM( CASE WHEN T0."DocDate" BETWEEN DATEADD(day,-15,GETDATE()) AND GETDATE() THEN T1."Quantity"*T1."StockPrice" ELSE 0 END)*-1	 "Cost D ULT15"
FROM
ORIN T0 INNER JOIN RIN1 T1 ON T0."DocEntry" = T1."DocEntry"
WHERE
T1."Quantity" > 0 AND T0."CANCELED" = 'N'
GROUP BY
T1."ItemCode", T1."WhsCode"
) A GROUP BY A."ItemCode", A."WhsCode"
)

T3	ON	T0."ItemCode"	=	T3."ItemCode"	AND	T0."WhsCode"	=	T3."WhsCode"
LEFT JOIN	OITB	T4	ON	T2."ItmsGrpCod"	=	T4."ItmsGrpCod"
LEFT JOIN	OMRC	T5	ON	T2."FirmCode"	=	T5."FirmCode"
LEFT JOIN	ITM1	T6	ON	T2."ItemCode"	=	T6."ItemCode" AND T6.PriceList = '01'
LEFT JOIN	OCRD	T7	ON	T2."U_proveedor"=	T7."CardCode"
LEFT JOIN	[CLVS_IMS_STDEV_ARTICULO] T8 ON T2."ItemCode" = T8.ItemCode

WHERE	(T0."OnHand" > 0 OR T0."OnOrder" <> 0 OR T3."Cant M-3" > 0
OR T3."Cant M-2" > 0
OR T3."Cant M-1" > 0
OR T3."Cant M ACT" > 0
OR T3."Cant M ULT3" > 0
OR T3."Cant D ULT15" > 0
OR T3."Cost D ULT15" > 0)

GO
/****** Object:  View [dbo].[CLVS_ANAL_DUPLICATES]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[CLVS_ANAL_DUPLICATES] AS
SELECT T0.[DocDate],T0.[DocNum],T0.[DocTime],T0.[DocTotal],T0.[PaidSum], COUNT(T1.[DocEntry]) [cuenta] FROM OINV T0  
INNER JOIN INV1 T1 ON T0.[DocEntry] = T1.[DocEntry]
group by T0.[DocDate],T0.[DocNum],T0.[DocTime],T0.[DocTotal],T0.[PaidSum]
GO
/****** Object:  View [dbo].[CLVS_COBROS_GEN_AS_EST]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW	[dbo].[CLVS_COBROS_GEN_AS_EST] AS
SELECT
T0."CardCode"	 "Cod Cliente",
T2."CardName"	 "Cliente",
T0."CardName"	 "Cliente en Factura",
T7."GroupName"	 "Grupo cliente",
T5."USER_CODE"	 "Usuario",
T6."SlpName"	 "Agente",
T3."ExtraDays"	 "Dias Cliente",
DATEDIFF(DAY,T1."DueDate",GETDATE())	 "Dias atraso",
T0."CANCELED"	 "Cancelado",
CASE T1."Status" WHEN 'O' THEN 'Abierto' WHEN 'C' THEN 'Cerrado' END		 "Estado",
DATEPART(YEAR, T0.[DocDate])	"Año Conta",					
DATEPART(MONTH, T0.[DocDate])	"Mes Conta",					
T0."DocDate"	 "Fecha",
DATEPART(YEAR, T1.[DueDate])	"Año Vence",
DATEPART(MONTH, T1.[DueDate])	"Mes Vence",
T1."DueDate"	 "Fecha vencimiento",
 CASE WHEN T4."SeriesName" IS NULL THEN 'Manual' ELSE T4."SeriesName" END	 "Serie Numeracion",
T0."DocNum"	 "# Doc",
T0."NumAtCard"	 "Referencia",
T0."DocCur"	 "Moneda",
T1."InstlmntID"	 "#Pagos",
CASE WHEN T0."ObjType"= 13 THEN 'Factura' ELSE 'Nota de credito' END	 "Tipo de documento",
CASE WHEN T2.Balance < 0 THEN 'A favor' ELSE 'Por cobrar' END "Tipo de Saldo",
T8.[AcctName] "Cuenta Contable",
T2.Balance "Balance Cliente COL",
T2.BalanceSys "Balance Cliente USD",
T1."InsTotal" "Bruto COL",
T1."PaidToDate" "Aplicado COL",
(T1."InsTotal" - T1."PaidToDate")  "Balance COL",
T1."InsTotalSy" "Bruto USD",
T1."PaidSys" "Aplicado USD",
(T1."InsTotalSy" - T1."PaidSys") "Balance USD",
CASE WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 0 AND T0."DocCur" = 'COL' THEN (T1."InsTotal" - T1."PaidToDate")WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 0 AND T0."DocCur" = 'USD' THEN (T1."InsTotalSy" - T1."PaidSys") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 8 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=1 AND T0."DocCur" = 'EUR' THEN (T1."InsTotalFC" - T1."PaidFC") END	 "Sin vencer",
CASE WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 30 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=1 AND T0."DocCur" = 'COL' 
THEN (T1."InsTotal" - T1."PaidToDate") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 30 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=1 AND T0."DocCur" = 'USD' 
THEN (T1."InsTotalSy" - T1."PaidSys") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 30 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=1 AND T0."DocCur" = 'EUR' 
THEN (T1."InsTotalFC" - T1."PaidFC") END	 "1-30 días",

CASE WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <=60 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=31 AND T0."DocCur" = 'COL' 
THEN (T1."InsTotal" - T1."PaidToDate") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 60 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=31 AND T0."DocCur" = 'USD' 
THEN (T1."InsTotalSy" - T1."PaidSys") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 60 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=31 AND T0."DocCur" = 'EUR' 
THEN (T1."InsTotalFC" - T1."PaidFC") END "31-60 días",

CASE WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 90 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=61 AND T0."DocCur" = 'COL' 
THEN (T1."InsTotal" - T1."PaidToDate") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 90 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=61 AND T0."DocCur" = 'USD' 
THEN (T1."InsTotalSy" - T1."PaidSys") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 90 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=61 AND T0."DocCur" = 'EUR' 
THEN (T1."InsTotalFC" - T1."PaidFC") END	 "61-90 días",
CASE WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 120 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=91 AND T0."DocCur" = 'COL' 
THEN (T1."InsTotal" - T1."PaidToDate") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 120 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=91 AND T0."DocCur" = 'USD' 
THEN (T1."InsTotalSy" - T1."PaidSys") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 120 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=91 AND T0."DocCur" = 'EUR' 
THEN (T1."InsTotalFC" - T1."PaidFC") END	 "91-120 días",
CASE WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 150 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=121 AND T0."DocCur" = 'COL' 
THEN (T1."InsTotal" - T1."PaidToDate") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 150 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=121 AND T0."DocCur" = 'USD' 
THEN (T1."InsTotalSy" - T1."PaidSys") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 150 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=121 AND T0."DocCur" = 'EUR' 
THEN (T1."InsTotalFC" - T1."PaidFC") END	 "121-150 días",
CASE WHEN DATEDIFF(DAY,T1."DueDate", GETDATE())>150 AND T0."DocCur" = 'COL' 
THEN (T1."InsTotal" - T1."PaidToDate") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE())>150 AND T0."DocCur" = 'USD' 
THEN (T1."InsTotalSy" - T1."PaidSys") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE())>150 AND T0."DocCur" = 'EUR' 
THEN (T1."InsTotalFC" - T1."PaidFC") END	 "150+ días",

T0.U_NumFE AS 'Numeros de FE'

FROM	OINV	T0
LEFT JOIN	INV6	T1	ON	T0."DocEntry"	=	T1."DocEntry"
LEFT JOIN	OCRD	T2	ON	T0."CardCode"	=	T2."CardCode"
LEFT JOIN	OCTG	T3	ON	T2."GroupNum"	=	T3."GroupNum"
LEFT JOIN	NNM1	T4	ON	T0."Series"	=	T4."Series"
LEFT JOIN	OUSR	T5	ON	T0."UserSign"	=	T5."USERID"
LEFT JOIN	OSLP	T6	ON	T0."SlpCode"	=	T6."SlpCode"
LEFT JOIN	OCRG	T7	ON	T2."GroupCode"	=	T7."GroupCode"
LEFT JOIN	OACT	T8	ON	T2.[DebPayAcct] =	T8.[AcctCode]

WHERE	T1."TotalBlck" <> T1."InsTotalSy" AND T1."Status" = 'O'
UNION ALL
SELECT

T0."CardCode"	 "Cod Cliente",
T2."CardName"	 "Cliente",
T0."CardName"	 "Cliente en Factura",
T7."GroupName"	 "Grupo cliente",
T5."USER_CODE"	 "Usuario",
T6."SlpName"	 "Agente",
T3."ExtraDays"	 "Dias Cliente",
DATEDIFF(DAY,T1."DueDate",GETDATE())	 "Dias atraso",
T0."CANCELED"	 "Cancelado",
CASE T1."Status" WHEN 'O' THEN 'Abierto' WHEN 'C' THEN 'Cerrado' END		 "Estado",
DATEPART(YEAR, T0.[DocDate])	"Año Conta",					
DATEPART(MONTH, T0.[DocDate])	"Mes Conta",					
T0."DocDate"	 "Fecha",
DATEPART(YEAR, T1.[DueDate])	"Año Vence",
DATEPART(MONTH, T1.[DueDate])	"Mes Vence",
T1."DueDate"	 "Fecha vencimiento",
 CASE WHEN T4."SeriesName" IS NULL THEN 'Manual' ELSE T4."SeriesName" END	 "Serie Numeracion",
T0."DocNum"	 "# Doc",
T0."NumAtCard"	 "Referencia",
T0."DocCur"	 "Moneda",
T1."InstlmntID"	 "#Pagos",
CASE WHEN T0."ObjType"= 13 THEN 'Factura' ELSE 'Nota de credito' END	 "Tipo de documento",
CASE WHEN T2.Balance < 0 THEN 'A favor' ELSE 'Por cobrar' END "Tipo de Saldo",
T8.[AcctName] "Cuenta Contable",
T2.Balance "Balance Cliente COL",
T2.BalanceSys "Balance Cliente USD",
T1."InsTotal"*-1 "Bruto COL",
T1."PaidToDate"*-1 "Aplicado COL",
(T1."InsTotal" - T1."PaidToDate")*-1 "Balance COL",
T1."InsTotalSy"*-1 "Bruto USD",
T1."PaidSys"*-1 "Aplicado USD",
(T1."InsTotalSy" - T1."PaidSys")*-1 "Balance USD",
CASE WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 0 AND T0."DocCur" = 'COL' THEN (T1."InsTotal" - T1."PaidToDate")*-1 WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 0 AND T0."DocCur" = 'USD' THEN (T1."InsTotalSy" - T1."PaidSys")*-1 WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 8 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=1 AND T0."DocCur" = 'EUR' THEN (T1."InsTotalFC" - T1."PaidFC")*-1 END	 "Sin vencer",
CASE WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 30 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=1 AND T0."DocCur" = 'COL' 
THEN (T1."InsTotal" - T1."PaidToDate")*-1 WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 30 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=1 AND T0."DocCur" = 'USD' 
THEN (T1."InsTotalSy" - T1."PaidSys")*-1 WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 30 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=1 AND T0."DocCur" = 'EUR' 
THEN (T1."InsTotalFC" - T1."PaidFC")*-1 END	 "1-30 días",
CASE WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <=60 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=31 AND T0."DocCur" = 'COL' 
THEN (T1."InsTotal" - T1."PaidToDate")*-1 WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 60 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=31 AND T0."DocCur" = 'USD' 
THEN (T1."InsTotalSy" - T1."PaidSys")*-1 WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 60 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=31 AND T0."DocCur" = 'EUR' 
THEN (T1."InsTotalFC" - T1."PaidFC")*-1 END	 "31-60 días",
CASE WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 90 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=61 AND T0."DocCur" = 'COL' 
THEN (T1."InsTotal" - T1."PaidToDate")*-1 WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 90 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=61 AND T0."DocCur" = 'USD' 
THEN (T1."InsTotalSy" - T1."PaidSys")*-1 WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 90 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=61 AND T0."DocCur" = 'EUR' 
THEN (T1."InsTotalFC" - T1."PaidFC")*-1 END	 "61-90 días",
CASE WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 120 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=91 AND T0."DocCur" = 'COL' 
THEN (T1."InsTotal" - T1."PaidToDate")*-1 WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 120 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=91 AND T0."DocCur" = 'USD' 
THEN (T1."InsTotalSy" - T1."PaidSys")*-1 WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 120 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=91 AND T0."DocCur" = 'EUR' 
THEN (T1."InsTotalFC" - T1."PaidFC")*-1 END	 "91-120 días",
CASE WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 150 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=121 AND T0."DocCur" = 'COL' 
THEN (T1."InsTotal" - T1."PaidToDate")*-1 WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 150 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=121 AND T0."DocCur" = 'USD' 
THEN (T1."InsTotalSy" - T1."PaidSys")*-1 WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 150 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=121 AND T0."DocCur" = 'EUR' 
THEN (T1."InsTotalFC" - T1."PaidFC")*-1 END	 "121-150 días",
CASE WHEN DATEDIFF(DAY,T1."DueDate", GETDATE())>150 AND T0."DocCur" = 'COL' 
THEN (T1."InsTotal" - T1."PaidToDate")*-1 WHEN DATEDIFF(DAY,T1."DueDate", GETDATE())>150 AND T0."DocCur" = 'USD' 
THEN (T1."InsTotalSy" - T1."PaidSys")*-1 WHEN DATEDIFF(DAY,T1."DueDate", GETDATE())>150 AND T0."DocCur" = 'EUR' 
THEN (T1."InsTotalFC" - T1."PaidFC")*-1 END	 "150+ días",

T0.U_NumFE AS 'Numeros de FE'

FROM	ORIN	T0
LEFT JOIN	RIN6	T1	ON	T0."DocEntry"	=	T1."DocEntry"
LEFT JOIN	OCRD	T2	ON	T0."CardCode"	=	T2."CardCode"
LEFT JOIN	OCTG	T3	ON	T2."GroupNum"	=	T3."GroupNum"
LEFT JOIN	NNM1	T4	ON	T0."Series"	=	T4."Series"
LEFT JOIN	OUSR	T5	ON	T0."UserSign"	=	T5."USERID"
LEFT JOIN	OSLP	T6	ON	T0."SlpCode"	=	T6."SlpCode"
LEFT JOIN	OCRG	T7	ON	T2."GroupCode"	=	T7."GroupCode"
LEFT JOIN	OACT	T8	ON	T2.[DebPayAcct] =	T8.[AcctCode]
WHERE	T1."TotalBlck" <> T1."InsTotalSy" AND T1."Status" = 'O'

UNION ALL

SELECT

T0."CardCode"	 "Cod Cliente",
T1."CardName"	 "Cliente",
T0."CardName"	 "Cliente en Factura",
T5."GroupName"	 "Grupo cliente",
'NA'	 "Usuario",
'NA'	 "Agente",
T2."ExtraDays"	 "Dias Cliente",
DATEDIFF(DAY,T0."DocDueDate", GETDATE())	 "Dias atraso",
T0."Canceled"	 "Cancelado",
CASE WHEN T0."OpenBal"<>0 THEN 'Abierto' WHEN T0."OpenBal"=0 THEN 'Cerrado' END		 "Estado",
DATEPART(YEAR, T0.[DocDate])	"Año Conta",					
DATEPART(MONTH, T0.[DocDate])	"Mes Conta",					
T0."DocDate"	 "Fecha",
DATEPART(YEAR, T0.[DocDueDate])	"Año Vence",
DATEPART(MONTH, T0.[DocDueDate])	"Mes Vence",
T0."DocDueDate"	 "Fecha vencimiento",
 CASE WHEN T4."SeriesName" IS NULL THEN 'Manual' ELSE T4."SeriesName" END	 "Serie Numeracion",
T0."DocNum"	 "# Doc",
T0."CounterRef"	 "Referencia",
T0."DocCurr"	 "Moneda",
0	 "#Pagos",
CASE WHEN T0."DocType" = 'C' THEN 'Recibo' END	 "Tipo de documento",
CASE WHEN T1.Balance < 0 THEN 'A favor' ELSE 'Por cobrar' END "Tipo de Saldo",
T8.[AcctName] "Cuenta Contable",
T1.Balance "Balance Cliente COL",
T1.BalanceSys "Balance Cliente USD",
T0."DocTotal" *-1 "Bruto COL",
(T0."DocTotal"-T0."OpenBal")*-1 "Aplicado COL",
T0."OpenBal"*-1 "Balance COL",
T0."DocTotalSy" *-1 "Bruto USD",
(T0."DocTotalSy"-T0."OpenBalSc")*-1 "Aplicado USD",
T0."OpenBalSc"*-1 "Balance USD",
CASE WHEN T0."DocCurr" = 'COL' THEN T0."OpenBal"*-1 WHEN T0."DocCurr" = 'USD' THEN T0."OpenBalSc"*-1 WHEN T0."DocCurr" = 'EUR' THEN T0."OpenBalFc"*-1 END	 "Sin Vencer",
0	 "1-30 días",
0	 "31-60 días",
0	 "61-90 días",
0	 "91-120 días",
0	 "121-150 días",
0	 "150+ días",

'' AS 'Numeros de FE'
--T0.U_NumFE AS 'Numeros de FE'

FROM	ORCT	T0
LEFT JOIN	OCRD	T1	ON	T0."CardCode"	=	T1."CardCode"
LEFT JOIN	OCTG	T2	ON	T1."GroupNum"	=	T2."GroupNum"
LEFT JOIN	NNM1	T4	ON	T0."Series"	=	T4."Series"
LEFT JOIN	OCRG	T5	ON	T1."GroupCode"	=	T5."GroupCode"
LEFT JOIN	OACT	T8	ON	T1.[DebPayAcct] =	T8.[AcctCode]
WHERE	T0."OpenBal"<>0


UNION ALL
SELECT

T0."CardCode"	 "Cod Cliente",
T1."CardName"	 "Cliente",
T0."CardName"	 "Cliente en Factura",
T5."GroupName"	 "Grupo cliente",
'NA'	 "Usuario",
'NA'	 "Agente",
T2."ExtraDays"	 "Dias Cliente",
DATEDIFF(DAY,T0."DocDueDate", GETDATE())	 "Dias atraso",
T0."Canceled"	 "Cancelado",
CASE WHEN T0."OpenBal"<>0 THEN 'Abierto' WHEN T0."OpenBal"=0 THEN 'Cerrado' END		 "Estado",
DATEPART(YEAR, T0.[DocDate])	"Año Conta",					
DATEPART(MONTH, T0.[DocDate])	"Mes Conta",					
T0."DocDate"	 "Fecha",
DATEPART(YEAR, T0.[DocDueDate])	"Año Vence",
DATEPART(MONTH, T0.[DocDueDate])	"Mes Vence",
T0."DocDueDate"	 "Fecha vencimiento",
 CASE WHEN T4."SeriesName" IS NULL THEN 'Manual' ELSE T4."SeriesName" END	 "Serie Numeracion",
T0."DocNum"	 "# Doc",
T0."CounterRef"	 "Referencia",
T0."DocCurr"	 "Moneda",
0	 "#Pagos",
CASE WHEN T0."DocType" = 'C' THEN 'Recibo' END	 "Tipo de documento",
CASE WHEN T1.Balance < 0 THEN 'A favor' ELSE 'Por cobrar' END "Tipo de Saldo",
T8.[AcctName] "Cuenta Contable",
T1.Balance "Balance Cliente COL",
T1.BalanceSys "Balance Cliente USD",
T0."DocTotal" *1 "Bruto COL",
(T0."DocTotal"-T0."OpenBal")*1 "Aplicado COL",
T0."OpenBal"*1 "Balance COL",
T0."DocTotalSy" *1 "Bruto USD",
(T0."DocTotalSy"-T0."OpenBalSc")*1 "Aplicado USD",
T0."OpenBalSc"*1 "Balance USD",
CASE WHEN T0."DocCurr" = 'COL' THEN T0."OpenBal"*1 WHEN T0."DocCurr" = 'USD' THEN T0."OpenBalSc"*1 WHEN T0."DocCurr" = 'EUR' THEN T0."OpenBalFc"*1 END	 "Sin Vencer",
0	 "1-30 días",
0	 "31-60 días",
0	 "61-90 días",
0	 "91-120 días",
0	 "121-150 días",
0	 "150+ días",

'' AS 'Numeros de FE'

FROM	OVPM	T0
LEFT JOIN	OCRD	T1	ON	T0."CardCode"	=	T1."CardCode"
LEFT JOIN	OCTG	T2	ON	T1."GroupNum"	=	T2."GroupNum"
LEFT JOIN	NNM1	T4	ON	T0."Series"	=	T4."Series"
LEFT JOIN	OCRG	T5	ON	T1."GroupCode"	=	T5."GroupCode"
LEFT JOIN	OACT	T8	ON	T1.[DebPayAcct] =	T8.[AcctCode]
WHERE	T1."CardType" = 'C'

UNION ALL


SELECT

T2.CardCode "Cod Cliente",
                    T2.cardname "Cliente",
					T2.cardname "Cliente en Factura",
T7."GroupName"	 "Grupo cliente",
T5."USER_CODE"	 "Usuario",
'NA'	 "Agente",
T3."ExtraDays"	 "Dias Cliente",
DATEDIFF(DAY,T1."DueDate",GETDATE())	 "Dias atraso",
'N'	 "Cancelado",
'Abierto'	 "Estado",
DATEPART(YEAR, T1.[RefDate])	"Año Conta",					
DATEPART(MONTH, T1.[RefDate])	"Mes Conta",	
T1."RefDate"	 "Fecha",
DATEPART(YEAR, T1.[DueDate])	"Año Vence",
DATEPART(MONTH, T1.[DueDate])	"Mes Vence",
T1."DueDate"	 "Fecha vencimiento",
CASE WHEN T4."SeriesName" IS NULL THEN 'Manual' ELSE T4."SeriesName" END	 "Serie Numeracion",
T0."TransId"	 "# Doc",
CAST (T0."TransId"	As NVARCHAR(30)) "Referencia",
CASE WHEN T0.TransCurr	= 'USD' THEN 'USD' ELSE 'COL' END "Moneda",
'1'	 "#Pagos",
'Asiento'	 "Tipo de Documento",
CASE WHEN T2.Balance < 0 THEN 'A favor' ELSE 'Por cobrar' END "Tipo de Saldo",
T8.[AcctName] "Cuenta Contable",
T2.Balance "Balance Cliente COL",
T2.BalanceSys "Balance Cliente USD",
case
when Credit <> 0 then isnull(-Credit,0)
else isnull(Debit,0)
end "Bruto COL",
case
when Credit <> 0 then isnull(-(Credit-BalDueCred),0)
else isnull((Debit-BalDueDeb),0)
end  "Aplicado COL",
case
when Credit <> 0 then isnull(-BalDueCred,0)
else isnull(BalDueDeb,0)
end "Balance COL",
case
when Credit <> 0 then isnull(-SYSCred,0)
else isnull(SYSDeb,0)
end "Bruto USD",
case
when Credit <> 0 then isnull(-(Credit-BalScCred),0)
else isnull((Debit-BalScDeb),0)
end  "Aplicado USD",
case
when Credit <> 0 then isnull(-BalScCred,0)					
else isnull(BalScDeb,0)
end "Balance USD",

CASE
when (DATEDIFF(dd,T1.RefDate,current_timestamp)) = 0
then
(case
when Credit <> 0 and T0.TransCurr = 'USD' then isnull(-FCCredit,0)
when Credit <> 0 then isnull(-Credit,0)
when T0.TransCurr = 'USD' then isnull(FCDebit,0)
else isnull(Debit,0)
end) end "Sin vencer",

--T2.Phone1,
--T1.TransType,
--case 
--          when T1.TransType = '13' then "Factura"
--          when T1.TransType = '14' then "Nota Credito"
--          when T1.TransType = '24' then "Pagos"
--          else "Otro"
--          end "Tipo Trans",
--T1.Ref1 "Referencia",
CASE
when (DATEDIFF(dd,T1.RefDate,current_timestamp))+1 < 31 and (DATEDIFF(dd,T1.RefDate,current_timestamp))+1 > 1
then
(case
when Credit <> 0 and T0.TransCurr = 'USD' then isnull(-FCCredit,0)
when Credit <> 0 then isnull(-Credit,0)
when T0.TransCurr = 'USD' then isnull(FCDebit,0)
else isnull(Debit,0)
end) end "1-30 días",
case when ((datediff(dd,T1.RefDate,current_timestamp))+1 > 30
and (datediff(dd,T1.RefDate,current_timestamp))+1< 61)
then
(case
when Credit <> 0 and T0.TransCurr = 'USD' then isnull(-FCCredit,0)
when Credit <> 0 then isnull(-Credit,0)
when T0.TransCurr = 'USD' then isnull(FCDebit,0)
else isnull(Debit,0)
end) END  "31-60 días",
case when ((datediff(dd,T1.RefDate,current_timestamp))+1 > 60
and (datediff(dd,T1.RefDate,current_timestamp))+1< 91)
then
(case
when Credit <> 0 and T0.TransCurr = 'USD' then isnull(-FCCredit,0)
when Credit <> 0 then isnull(-Credit,0)
when T0.TransCurr = 'USD' then isnull(FCDebit,0)
else isnull(Debit,0)
end) END  "61-90 días",
case when ((datediff(dd,T1.RefDate,current_timestamp))+1 > 90
and (datediff(dd,T1.RefDate,current_timestamp))+1< 121)
then
(case
when Credit <> 0 and T0.TransCurr = 'USD' then isnull(-FCCredit,0)
when Credit <> 0 then isnull(-Credit,0)
when T0.TransCurr = 'USD' then isnull(FCDebit,0)
else isnull(Debit,0)
end) END  "91-120 días",
case when ((datediff(dd,T1.RefDate,current_timestamp))+1 > 120
and (datediff(dd,T1.RefDate,current_timestamp))+1< 151)
then
(case
when Credit <> 0 and T0.TransCurr = 'USD' then isnull(-FCCredit,0)
when Credit <> 0 then isnull(-Credit,0)
when T0.TransCurr = 'USD' then isnull(FCDebit,0)
else isnull(Debit,0)
end) END  "121-150 días",
CASE
when (DATEDIFF(dd,T1.RefDate,current_timestamp))+1 > 150
then
(case
when Credit <> 0 and T0.TransCurr = 'USD' then isnull(-FCCredit,0)
when Credit <> 0 then isnull(-Credit,0)
when T0.TransCurr = 'USD' then isnull(FCDebit,0)
else isnull(Debit,0)
end)
end "150+ días",

'' AS 'Numeros de FE'

from OJDT T0        
INNER JOIN JDT1 T1 ON T0.[TransId] = T1.[TransId]
INNER JOIN OCRD T2 ON T1.shortname = T2.cardcode and T2.cardtype = 'C'
LEFT JOIN	OCTG	T3	ON	T2."GroupNum"	=	T3."GroupNum"
LEFT JOIN	NNM1	T4	ON	T0."Series"	=	T4."Series"
LEFT JOIN	OUSR	T5	ON	T0."UserSign"	=	T5."USERID"
LEFT JOIN	OCRG	T7	ON	T2."GroupCode"	=	T7."GroupCode"
LEFT JOIN	OACT	T8	ON	T2.[DebPayAcct] =	T8.[AcctCode]

where 
T1.intrnmatch = 0
and T1.BALDUEDEB != T1.BALDUECRED and T1.TransType = 30
GO
/****** Object:  View [dbo].[CLVS_COMPRAS_DET_EST]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW	[dbo].[CLVS_COMPRAS_DET_EST] AS
SELECT	
T0.DocEntry,					
T0.[DocDate]					"Fecha",					
DATEPART(YEAR, T0.[DocDate])	"Año",					
DATEPART(MONTH, T0.[DocDate])	"Mes",					
DATEPART(DAY, T0.[DocDate])		"Dia",
T12.[Fecha]						"Fecha ult conciliacion",					
DATEPART(YEAR, T12.[Fecha])		"Año ult conciliacion",		
DATEPART(MONTH, T12.[Fecha])	"Mes ult conciliacion",				
T0.[DocTime]					"Hora",					
T5.[WhsName]					"Almacen",					
T0.[CardCode]					"Cod Proveedor",					
T8.[CardName]					"Proveedor",					
T0.[CardName]					"Proveedor en Factura",					

CASE WHEN T9.[SeriesName] IS NULL THEN 'Manual' ELSE T9.[SeriesName] END	"Serie Numeracion",					
T0.[DocNum]						"# Doc",
T0.NumAtCard	                "Referencia",
CASE T0.[DocStatus] WHEN 'O' THEN 'Abierto' WHEN 'C' THEN 'Cerrado' END	"Estado Documento",
CASE T0.[CANCELED] WHEN 'N' THEN 'No' WHEN 'Y' THEN 'Si' WHEN 'C' THEN 'Cancelacion' END	"Cancelado",
'Factura'						"Clase de Documento",					
CASE T0.[DocType] WHEN 'S' THEN 'Servicio' WHEN 'I' THEN 'Articulo' END 	"Tipo de documento",					
						
T4.[USER_CODE]		"Usuario",	
T6.[SlpName]		"Agente",
T1.[Currency]		"Moneda",
T0.[Comments]		"Comentarios",	
T7.[FirmName]		"Marca",
T3.[ItmsGrpNam]		"Grupo Articulos",
T1.[ItemCode]		"Cod Articulo",
T2.[ItemName]		"Articulo",
T1.[Dscription]		"Articulo en Factura",	
T1.[Quantity]		"Cantidad",
T1.[TaxCode]		"Impuesto",

T1.[LineTotal]	 "Subtotal LOC",					
(T1.[LineTotal]* (COALESCE(T0.[DiscPrcnt],0)/100))	 "Descuento CAB LOC",					

(T1.[LineTotal] - --Subtotal
(T1.[LineTotal]* (COALESCE(T0.[DiscPrcnt],0)/100)))	 --Descuento CAB LOC
"Total Neto LOC",

(T1.[LineTotal] - --Subtotal
(T1.[LineTotal]* (COALESCE(T0.[DiscPrcnt],0)/100))) -	 --Descuento CAB LOC
T1.[GrssProfit]	 "Costo LOC",					

T1.[GrssProfit]	 "Utilidad LOC",
T1.[GrssProfit]*.1	 "Comision LOC",
T1.[VatSum]		 "Impuesto LOC",

T1.[TotalSumSy]	 "Subtotal USD",					
(T1.[TotalSumSy]* (COALESCE(T0.[DiscPrcnt],0)/100))	 "Descuento CAB USD",					

(T1.[TotalSumSy] - --Subtotal
(T1.[TotalSumSy]* (COALESCE(T0.[DiscPrcnt],0)/100)))	 --Descuento CAB USD
"Total Neto USD",

(T1.[TotalSumSy] - --Subtotal
(T1.[TotalSumSy]* (COALESCE(T0.[DiscPrcnt],0)/100))) -	 --Descuento CAB USD
T1.[GrssProfSC]	 "Costo USD",

T1.[GrssProfSC]	 "Utilidad USD",
T1.[GrssProfSC]*.1	 "Comision USD",					
T1.[VatSumSy]	 "Impuesto USD",

T10.[Segment_0]		"Cuenta contable",
T10.[AcctName]		"Nombre cuenta contable"


FROM		OPCH	T0				
LEFT JOIN	PCH1	T1	ON	T0.[DocEntry]	=	T1.[DocEntry]
LEFT JOIN	OITM	T2	ON	T1.[ItemCode]	=	T2.[ItemCode]
LEFT JOIN	OITB	T3	ON	T2.[ItmsGrpCod]	=	T3.[ItmsGrpCod]
LEFT JOIN	OUSR	T4	ON	T0.[UserSign]	=	T4.[USERID]
LEFT JOIN	OWHS	T5	ON	T1.[WhsCode]	=	T5.[WhsCode]
LEFT JOIN	OSLP	T6	ON	T0.[SlpCode]	=	T6.[SlpCode]
LEFT JOIN	OMRC	T7	ON	T2.[FirmCode]	=	T7.[FirmCode]
LEFT JOIN	OCRD	T8	ON	T0.[CardCode]	=	T8.[CardCode]
LEFT JOIN	NNM1	T9	ON	T0.[Series]		=	T9.[Series]
LEFT JOIN	OACT	T10	ON	T1.[AcctCode]	=	T10.[AcctCode]
LEFT JOIN	OACT	T11	ON	T1.[CogsAcct]	=	T11.[AcctCode]
LEFT JOIN 
(
SELECT
T0.DocEntry "Llave",
T0.DocNum,
MAX(T3."ReconDate") AS "Fecha"

FROM OPCH T0  
LEFT JOIN ITR1 T2 on T0."DocEntry"  = T2."SrcObjAbs" AND T0."ObjType"  = T2."SrcObjTyp"
INNER JOIN OITR T3 ON T2."ReconNum" = T3."ReconNum"

GROUP BY
T0.DocEntry,
T0.DocNum
) T12 ON T0.[DocEntry] = T12.[Llave]

UNION ALL

SELECT
T0.DocEntry,					
T0.[DocDate]					"Fecha",					
DATEPART(YEAR, T0.[DocDate])	"Año",					
DATEPART(MONTH, T0.[DocDate])	"Mes",					
DATEPART(DAY, T0.[DocDate])		"Dia",
T12.[Fecha]						"Fecha ult conciliacion",					
DATEPART(YEAR, T12.[Fecha])		"Año ult conciliacion",		
DATEPART(MONTH, T12.[Fecha])	"Mes ult conciliacion",						
T0.[DocTime]					"Hora",					
T5.[WhsName]					"Almacen",					
T0.[CardCode]					"Cod Proveedor",					
T8.[CardName]					"Proveedor",					
T0.[CardName]					"Proveedor en Factura",					

CASE WHEN T9.[SeriesName] IS NULL THEN 'Manual' ELSE T9.[SeriesName] END	"Serie Numeracion",
T0.[DocNum]						"# Doc",
T0.NumAtCard	                "Referencia",
CASE T0.[DocStatus] WHEN 'O' THEN 'Abierto' WHEN 'C' THEN 'Cerrado' END  "Estado Documento",
			
CASE T0.[CANCELED] WHEN 'N' THEN 'No' WHEN 'Y' THEN 'Si' WHEN 'C' THEN 'Cancelacion' END 						"Cancelado",

'NC'						"Clase de Documento",
					
CASE T0.[DocType] WHEN 'S' THEN 'Servicio' WHEN 'I' THEN 'Articulo' END  "Tipo de documento",					
						
T4.[USER_CODE]		"Usuario",	
T6.[SlpName]		"Agente",
T1.[Currency]		"Moneda",
T0.[Comments]		"Comentarios",	
T7.[FirmName]		"Marca",
T3.[ItmsGrpNam]		"Grupo Articulos",
T1.[ItemCode]		"Cod Articulo",
T2.[ItemName]		"Articulo",
T1.[Dscription]		"Articulo en Factura",	
T1.[Quantity]		"Cantidad",
T1.[TaxCode]		"Impuesto",

T1.[LineTotal]*-1	 "Subtotal LOC",					
(T1.[LineTotal]* (COALESCE(T0.[DiscPrcnt],0)/100))*-1	 "Descuento CAB LOC",					

(T1.[LineTotal] - --Subtotal
(T1.[LineTotal]* (COALESCE(T0.[DiscPrcnt],0)/100)))*-1	 --Descuento CAB LOC
"Total Neto LOC",

((T1.[LineTotal] - --Subtotal
(T1.[LineTotal]* (COALESCE(T0.[DiscPrcnt],0)/100))) -	 --Descuento CAB LOC
T1.[GrssProfit])*-1	 "Costo LOC",					

T1.[GrssProfit]*-1	 "Utilidad LOC",
T1.[GrssProfit]*-1*.1	 "Comision LOC",
T1.[VatSum]*-1		 "Impuesto LOC",

T1.[TotalSumSy]*-1	 "Subtotal USD",					
(T1.[TotalSumSy]* (COALESCE(T0.[DiscPrcnt],0)/100))*-1	 "Descuento CAB USD",					

(T1.[TotalSumSy] - --Subtotal
(T1.[TotalSumSy]* (COALESCE(T0.[DiscPrcnt],0)/100)))*-1	 --Descuento CAB USD
"Total Neto USD",

((T1.[TotalSumSy] - --Subtotal
(T1.[TotalSumSy]* (COALESCE(T0.[DiscPrcnt],0)/100))) -	 --Descuento CAB USD
T1.[GrssProfSC])*-1	 "Costo USD",

T1.[GrssProfSC]*-1	 "Utilidad USD",
T1.[GrssProfSC]*-1*.1	 "Comision USD",					
T1.[VatSumSy]*-1	 "Impuesto USD",

T10.[Segment_0]		"Cuenta gastos",
T10.[AcctName]		"Nombre cuenta gastos"
						
FROM		ORPC	T0				
LEFT JOIN	RPC1	T1	ON	T0.[DocEntry]	=	T1.[DocEntry]
LEFT JOIN	OITM	T2	ON	T1.[ItemCode]	=	T2.[ItemCode]
LEFT JOIN	OITB	T3	ON	T2.[ItmsGrpCod]	=	T3.[ItmsGrpCod]
LEFT JOIN	OUSR	T4	ON	T0.[UserSign]	=	T4.[USERID]
LEFT JOIN	OWHS	T5	ON	T1.[WhsCode]	=	T5.[WhsCode]
LEFT JOIN	OSLP	T6	ON	T0.[SlpCode]	=	T6.[SlpCode]
LEFT JOIN	OMRC	T7	ON	T2.[FirmCode]	=	T7.[FirmCode]
LEFT JOIN	OCRD	T8	ON	T0.[CardCode]	=	T8.[CardCode]
LEFT JOIN	NNM1	T9	ON	T0.[Series]		=	T9.[Series]
LEFT JOIN	OACT	T10	ON	T1.[AcctCode]	=	T10.[AcctCode]
LEFT JOIN	OACT	T11	ON	T1.[CogsAcct]	=	T11.[AcctCode]
LEFT JOIN 
(
SELECT
T0.DocEntry "Llave",
T0.DocNum,
MAX(T3."ReconDate") AS "Fecha"

FROM ORPC T0  
LEFT JOIN ITR1 T2 on T0."DocEntry"  = T2."SrcObjAbs" AND T0."ObjType"  = T2."SrcObjTyp"
INNER JOIN OITR T3 ON T2."ReconNum" = T3."ReconNum"

GROUP BY
T0.DocEntry,
T0.DocNum
) T12 ON T0.[DocEntry] = T12.[Llave]

GO
/****** Object:  View [dbo].[CLVS_CONTEO_INVENTARIO]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[CLVS_CONTEO_INVENTARIO] AS
SELECT 
T0.[Code] [Fecha]
,MONTH(T0.[Code]) [Mes]
,DAY(T0.[Code]) [Dia]
,CASE WHEN T0.[U_Articulo] IN
(SELECT DISTINCT [U_Articulo]
FROM [@CONTEOS]
WHERE [U_Cantidad] <> [U_CantidadContada]) THEN 'SI' ELSE 'NO'
END [REVISAR]
,CASE WHEN T3.U_Articulo IS NOT NULL THEN 'SI' ELSE 'NO' END 'ULTIMA'
,T2.[CardCode]
,T2.[CardName]
,T1.[ItemCode]
,T0.[U_Articulo] [Articulo]
,LEFT(T0.[U_Articulo],CHARINDEX('-',T0.[U_Articulo])-1) [Barras]
,RIGHT(T0.[U_Articulo],LEN(T0.[U_Articulo])-CHARINDEX('-',T0.[U_Articulo])) [Descripcion]
,T0.[U_Cantidad] [Teorico]
,T0.[U_CantidadContada] [Real]
,CASE WHEN T0.[U_Cantidad] > T0.[U_CantidadContada] THEN T0.[U_Cantidad] - T0.[U_CantidadContada] END [Salida]
,CASE WHEN T0.[U_Cantidad] < T0.[U_CantidadContada] THEN T0.[U_CantidadContada] - T0.[U_Cantidad] END [Entrada]
FROM [@CONTEOS] T0
LEFT JOIN OITM T1 ON LEFT(T0.[U_Articulo],CHARINDEX('-',T0.[U_Articulo])-1) = T1.[CodeBars]
LEFT JOIN OCRD T2 ON T1.[U_proveedor] = T2.CardCode
LEFT JOIN 
(
Select u_articulo, max(Convert(datetime,code)) [Code] from [@conteos]
GROUP BY U_Articulo) T3 ON T0.U_Articulo = T3.U_Articulo AND T0.Code = T3.Code
--WHERE [U_Cantidad] <> [U_CantidadContada] and T0.U_Articulo in ('764009009555-BOHEMIA 350 ML LATA','764009020918-PILSEN 473 ML  LATA')


GO
/****** Object:  View [dbo].[CLVS_ENT-DEV_DET]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW	[dbo].[CLVS_ENT-DEV_DET] AS
SELECT	
T0.DocEntry,					
T0.[DocDate]					"Fecha",					
DATEPART(YEAR, T0.[DocDate])	"Año",					
DATEPART(MONTH, T0.[DocDate])	"Mes",					
DATEPART(DAY, T0.[DocDate])		"Dia",
T12.[Fecha]						"Fecha ult conciliacion",					
DATEPART(YEAR, T12.[Fecha])		"Año ult conciliacion",		
DATEPART(MONTH, T12.[Fecha])	"Mes ult conciliacion",				
T0.[DocTime]					"Hora",					
T5.[WhsName]					"Almacen",					
T0.[CardCode]					"Cod Proveedor",					
T8.[CardName]					"Proveedor",					
T0.[CardName]					"Proveedor en Factura",					

CASE WHEN T9.[SeriesName] IS NULL THEN 'Manual' ELSE T9.[SeriesName] END	"Serie Numeracion",					
T0.[DocNum]						"# Doc",
T0.NumAtCard	                "Referencia",
CASE T0.[DocStatus] WHEN 'O' THEN 'Abierto' WHEN 'C' THEN 'Cerrado' END	"Estado Documento",
CASE T0.[CANCELED] WHEN 'N' THEN 'No' WHEN 'Y' THEN 'Si' WHEN 'C' THEN 'Cancelacion' END	"Cancelado",
'ENTRADA'						"Clase de Documento",					
CASE T0.[DocType] WHEN 'S' THEN 'Servicio' WHEN 'I' THEN 'Articulo' END 	"Tipo de documento",					
						
T4.[USER_CODE]		"Usuario",	
T6.[SlpName]		"Agente",
T1.[Currency]		"Moneda",
T0.[Comments]		"Comentarios",	
T7.[FirmName]		"Marca",
T3.[ItmsGrpNam]		"Grupo Articulos",
T1.[ItemCode]		"Cod Articulo",
T2.[ItemName]		"Articulo",
T1.[Dscription]		"Articulo en Factura",	
T1.[Quantity]		"Cantidad",
T1.[TaxCode]		"Impuesto",

T1.[LineTotal]	 "Subtotal LOC",					
(T1.[LineTotal]* (COALESCE(T0.[DiscPrcnt],0)/100))	 "Descuento CAB LOC",					

(T1.[LineTotal] - --Subtotal
(T1.[LineTotal]* (COALESCE(T0.[DiscPrcnt],0)/100)))	 --Descuento CAB LOC
"Total Neto LOC",

(T1.[LineTotal] - --Subtotal
(T1.[LineTotal]* (COALESCE(T0.[DiscPrcnt],0)/100))) -	 --Descuento CAB LOC
T1.[GrssProfit]	 "Costo LOC",					

T1.[GrssProfit]	 "Utilidad LOC",
T1.[GrssProfit]*.1	 "Comision LOC",
T1.[VatSum]		 "Impuesto LOC",

T1.[TotalSumSy]	 "Subtotal USD",					
(T1.[TotalSumSy]* (COALESCE(T0.[DiscPrcnt],0)/100))	 "Descuento CAB USD",					

(T1.[TotalSumSy] - --Subtotal
(T1.[TotalSumSy]* (COALESCE(T0.[DiscPrcnt],0)/100)))	 --Descuento CAB USD
"Total Neto USD",

(T1.[TotalSumSy] - --Subtotal
(T1.[TotalSumSy]* (COALESCE(T0.[DiscPrcnt],0)/100))) -	 --Descuento CAB USD
T1.[GrssProfSC]	 "Costo USD",

T1.[GrssProfSC]	 "Utilidad USD",
T1.[GrssProfSC]*.1	 "Comision USD",					
T1.[VatSumSy]	 "Impuesto USD",

T10.[Segment_0]		"Cuenta contable",
T10.[AcctName]		"Nombre cuenta contable"


FROM		OPDN	T0				
LEFT JOIN	PDN1	T1	ON	T0.[DocEntry]	=	T1.[DocEntry]
LEFT JOIN	OITM	T2	ON	T1.[ItemCode]	=	T2.[ItemCode]
LEFT JOIN	OITB	T3	ON	T2.[ItmsGrpCod]	=	T3.[ItmsGrpCod]
LEFT JOIN	OUSR	T4	ON	T0.[UserSign]	=	T4.[USERID]
LEFT JOIN	OWHS	T5	ON	T1.[WhsCode]	=	T5.[WhsCode]
LEFT JOIN	OSLP	T6	ON	T0.[SlpCode]	=	T6.[SlpCode]
LEFT JOIN	OMRC	T7	ON	T2.[FirmCode]	=	T7.[FirmCode]
LEFT JOIN	OCRD	T8	ON	T0.[CardCode]	=	T8.[CardCode]
LEFT JOIN	NNM1	T9	ON	T0.[Series]		=	T9.[Series]
LEFT JOIN	OACT	T10	ON	T1.[AcctCode]	=	T10.[AcctCode]
LEFT JOIN	OACT	T11	ON	T1.[CogsAcct]	=	T11.[AcctCode]
LEFT JOIN 
(
SELECT
T0.DocEntry "Llave",
T0.DocNum,
MAX(T3."ReconDate") AS "Fecha"

FROM OPDN T0  
LEFT JOIN ITR1 T2 on T0."DocEntry"  = T2."SrcObjAbs" AND T0."ObjType"  = T2."SrcObjTyp"
INNER JOIN OITR T3 ON T2."ReconNum" = T3."ReconNum"

GROUP BY
T0.DocEntry,
T0.DocNum
) T12 ON T0.[DocEntry] = T12.[Llave]

UNION ALL

SELECT
T0.DocEntry,					
T0.[DocDate]					"Fecha",					
DATEPART(YEAR, T0.[DocDate])	"Año",					
DATEPART(MONTH, T0.[DocDate])	"Mes",					
DATEPART(DAY, T0.[DocDate])		"Dia",
T12.[Fecha]						"Fecha ult conciliacion",					
DATEPART(YEAR, T12.[Fecha])		"Año ult conciliacion",		
DATEPART(MONTH, T12.[Fecha])	"Mes ult conciliacion",						
T0.[DocTime]					"Hora",					
T5.[WhsName]					"Almacen",					
T0.[CardCode]					"Cod Proveedor",					
T8.[CardName]					"Proveedor",					
T0.[CardName]					"Proveedor en Factura",					

CASE WHEN T9.[SeriesName] IS NULL THEN 'Manual' ELSE T9.[SeriesName] END	"Serie Numeracion",
T0.[DocNum]						"# Doc",
T0.NumAtCard	                "Referencia",
CASE T0.[DocStatus] WHEN 'O' THEN 'Abierto' WHEN 'C' THEN 'Cerrado' END  "Estado Documento",
			
CASE T0.[CANCELED] WHEN 'N' THEN 'No' WHEN 'Y' THEN 'Si' WHEN 'C' THEN 'Cancelacion' END 						"Cancelado",

'DEVOLUCION'						"Clase de Documento",
					
CASE T0.[DocType] WHEN 'S' THEN 'Servicio' WHEN 'I' THEN 'Articulo' END  "Tipo de documento",					
						
T4.[USER_CODE]		"Usuario",	
T6.[SlpName]		"Agente",
T1.[Currency]		"Moneda",
T0.[Comments]		"Comentarios",	
T7.[FirmName]		"Marca",
T3.[ItmsGrpNam]		"Grupo Articulos",
T1.[ItemCode]		"Cod Articulo",
T2.[ItemName]		"Articulo",
T1.[Dscription]		"Articulo en Factura",	
T1.[Quantity]		"Cantidad",
T1.[TaxCode]		"Impuesto",

T1.[LineTotal]*-1	 "Subtotal LOC",					
(T1.[LineTotal]* (COALESCE(T0.[DiscPrcnt],0)/100))*-1	 "Descuento CAB LOC",					

(T1.[LineTotal] - --Subtotal
(T1.[LineTotal]* (COALESCE(T0.[DiscPrcnt],0)/100)))*-1	 --Descuento CAB LOC
"Total Neto LOC",

((T1.[LineTotal] - --Subtotal
(T1.[LineTotal]* (COALESCE(T0.[DiscPrcnt],0)/100))) -	 --Descuento CAB LOC
T1.[GrssProfit])*-1	 "Costo LOC",					

T1.[GrssProfit]*-1	 "Utilidad LOC",
T1.[GrssProfit]*-1*.1	 "Comision LOC",
T1.[VatSum]*-1		 "Impuesto LOC",

T1.[TotalSumSy]*-1	 "Subtotal USD",					
(T1.[TotalSumSy]* (COALESCE(T0.[DiscPrcnt],0)/100))*-1	 "Descuento CAB USD",					

(T1.[TotalSumSy] - --Subtotal
(T1.[TotalSumSy]* (COALESCE(T0.[DiscPrcnt],0)/100)))*-1	 --Descuento CAB USD
"Total Neto USD",

((T1.[TotalSumSy] - --Subtotal
(T1.[TotalSumSy]* (COALESCE(T0.[DiscPrcnt],0)/100))) -	 --Descuento CAB USD
T1.[GrssProfSC])*-1	 "Costo USD",

T1.[GrssProfSC]*-1	 "Utilidad USD",
T1.[GrssProfSC]*-1*.1	 "Comision USD",					
T1.[VatSumSy]*-1	 "Impuesto USD",

T10.[Segment_0]		"Cuenta gastos",
T10.[AcctName]		"Nombre cuenta gastos"
						
FROM		ORPD	T0				
LEFT JOIN	RPD1	T1	ON	T0.[DocEntry]	=	T1.[DocEntry]
LEFT JOIN	OITM	T2	ON	T1.[ItemCode]	=	T2.[ItemCode]
LEFT JOIN	OITB	T3	ON	T2.[ItmsGrpCod]	=	T3.[ItmsGrpCod]
LEFT JOIN	OUSR	T4	ON	T0.[UserSign]	=	T4.[USERID]
LEFT JOIN	OWHS	T5	ON	T1.[WhsCode]	=	T5.[WhsCode]
LEFT JOIN	OSLP	T6	ON	T0.[SlpCode]	=	T6.[SlpCode]
LEFT JOIN	OMRC	T7	ON	T2.[FirmCode]	=	T7.[FirmCode]
LEFT JOIN	OCRD	T8	ON	T0.[CardCode]	=	T8.[CardCode]
LEFT JOIN	NNM1	T9	ON	T0.[Series]		=	T9.[Series]
LEFT JOIN	OACT	T10	ON	T1.[AcctCode]	=	T10.[AcctCode]
LEFT JOIN	OACT	T11	ON	T1.[CogsAcct]	=	T11.[AcctCode]
LEFT JOIN 
(
SELECT
T0.DocEntry "Llave",
T0.DocNum,
MAX(T3."ReconDate") AS "Fecha"

FROM ORPD T0  
LEFT JOIN ITR1 T2 on T0."DocEntry"  = T2."SrcObjAbs" AND T0."ObjType"  = T2."SrcObjTyp"
INNER JOIN OITR T3 ON T2."ReconNum" = T3."ReconNum"

GROUP BY
T0.DocEntry,
T0.DocNum
) T12 ON T0.[DocEntry] = T12.[Llave]

GO
/****** Object:  View [dbo].[CLVS_ESTADOS_GEN_EST]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW	[dbo].[CLVS_ESTADOS_GEN_EST] AS
SELECT

T0.[RefDate]                            "Fecha",
T6.Name									"Perido",
T6.[Category]					        "Año fiscal",
DATEPART(YEAR, T0.[RefDate])	        "Año",
DATEPART(MONTH, T0.[RefDate])			"Mes",
DATEPART(DAY, T0.[RefDate])				"Dia",
CONCAT(DATEPART(YEAR, T0.[RefDate]),'-',DATEPART(MONTH, T0.[RefDate]))	"Año-Mes",
CASE WHEN T4.[SeriesName] IS NULL 
THEN 'Manual' ELSE T4.[SeriesName]
END										"Serie Numeración",
T0.[Number]                             "# Asiento",
T0.[TransType]                          "Clase documento",
T0.[BaseRef]                            "# Documento",
T0.[TransId]                            "# Transaccion",
T0.[Memo]                               "Comentarios",
T0.[Ref1]								"Ref 1",
T0.[Ref2]								"Ref 2",
T0.[Ref3]								"Ref 3",

SUBSTRING(T2.[Segment_0],1,1) + 
CASE 
WHEN SUBSTRING(T2.[Segment_0],1,1) = 1 THEN ' Activos' 
WHEN SUBSTRING(T2.[Segment_0],1,1) = 2 THEN ' Pasivos' 
WHEN SUBSTRING(T2.[Segment_0],1,1) = 3 THEN ' Patrimonio' 
WHEN SUBSTRING(T2.[Segment_0],1,1) = 4 THEN ' Ventas' 
WHEN SUBSTRING(T2.[Segment_0],1,1) = 5 THEN ' Costos' 
WHEN SUBSTRING(T2.[Segment_0],1,1) = 6 THEN ' Gastos' 
WHEN SUBSTRING(T2.[Segment_0],1,1) = 7 THEN ' Ingresos indirectos' 
WHEN SUBSTRING(T2.[Segment_0],1,1) = 8 THEN ' Gastos indirectos' 
ELSE '-' END		"Niv 1",					
SUBSTRING(T2.[Segment_0],1,3)	"Niv 2",					
(SELECT [AcctName] FROM OACT WHERE SUBSTRING(T2.[Segment_0],1,3) = [AcctCode])	 "Niv 2 N",					
SUBSTRING(T2.[Segment_0],1,5)	"Niv 3",					
(SELECT [AcctName] FROM OACT WHERE SUBSTRING(T2.[Segment_0],1,5) = [AcctCode])	 "Niv 3 N",					
SUBSTRING(T2.[Segment_0],1,7)	 "Niv 4",					
(SELECT [AcctName] FROM OACT WHERE SUBSTRING(T2.[Segment_0],1,7) = [AcctCode])	 "Niv 4 N",					

T2.[Segment_0]							"Cuenta",
T2.[AcctName]	                        "Nombre Cuenta",
T1.[ProfitCode]	                        "Dim1",
T1.[OcrCode2]	                        "Dim2",
T1.[OcrCode3]	                        "Dim3",
T1.[OcrCode4]	                        "Dim4",
T1.[OcrCode5]	                        "Dim5",
T1.[Project]	                        "Proyecto",	

ISNULL(T3.[Segment_0],T1.[ContraAct])	"Cuenta Contrapartida",
ISNULL(T3.[AcctName],T5."CardName")		"Nombre Cuenta Contrapartida",

T1.[Debit]                              "Debe LOC",
T1.[Credit]                             "Haber LOC",
T1.[Debit]-T1.[Credit]                  "Neto LOC",
T1.[SYSDeb]                             "Debe DOL",
T1.[SYSCred]                            "Haber DOL",
T1.[SYSDeb]-T1.[SYSCred]                "Neto DOL"

FROM		OJDT	T0				
LEFT JOIN	JDT1	T1	ON	T0.[TransId]	=	T1.[TransId]
LEFT JOIN	OACT	T2	ON	T1.[Account]	=	T2.[AcctCode]
LEFT JOIN	OACT	T3	ON	T1.[ContraAct]	=	T3.[AcctCode]
LEFT JOIN	NNM1	T4  ON  T0.[Series]	    =   T4.[Series]
LEFT JOIN 	OCRD	T5	ON	T1.[ContraAct] 	=	T5.[CardCode]
LEFT JOIN	OFPR	T6  ON  T6.[AbsEntry]	=   T0.[FinncPriod]
			
WHERE	T1."Debit"-T1."Credit" <> 0										


GO
/****** Object:  View [dbo].[CLVS_FE_GETTAXCODES]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[CLVS_FE_GETTAXCODES] AS
SELECT TAXES.Code AS 'TaxCode',
	   TAXES.Rate AS 'TaxRate'
FROM OSTC AS TAXES
WHERE TAXES.Lock = 'N'
GO
/****** Object:  View [dbo].[CLVS_FE_SYNC_OTROSCARGOS43]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[CLVS_FE_SYNC_OTROSCARGOS43] AS
SELECT
--Lineas facturas
'04' "TipoDocumento"
,T0."DocEntry"
,'NULL' AS "NumeroIdentidadTercero"
,'NULL' AS "NombreTercero"
,'NULL' as "Detalle"
,'NULL' AS "Porcentaje"
,'NULL' AS "MontoCargo"

FROM OINV T0
LEFT JOIN INV1 T1 ON T0."DocEntry" = T1."DocEntry"
WHERE T0."DocDate" >= '20190701'
AND T0."DocSubType" != 'DN'
AND T0."CANCELED" = 'N'
AND T0."DocType" = 'I'
AND T1."PriceBefDi" <> 0
AND T1."Quantity" <> 0
--AND T1."U_OtrosCargos" = 1
AND T0."Series" NOT IN (Select A."Series" from NNM1 A where A."SeriesName" IN ('NDSinFE','NCSinFE'))

UNION ALL

SELECT
--Lineas ND
'04' "TipoDocumento"
,T0."DocEntry"
,'NULL' AS "NumeroIdentidadTercero"
,'NULL' AS "NombreTercero"
,'NULL' as "Detalle"
,'NULL' AS "Porcentaje"
,'NULL' AS "MontoCargo"

FROM OINV T0
LEFT JOIN INV1 T1 ON T0."DocEntry" = T1."DocEntry"
WHERE T0."DocDate" >= '20190701'
AND T0."DocSubType" = 'DN'
AND T0."CANCELED" = 'N'
AND T0."DocType" = 'I'
AND T1."PriceBefDi" <> 0
AND T1."Quantity" <> 0
--AND T1."U_OtrosCargos" = 1
AND T0."Series" NOT IN (Select A."Series" from NNM1 A where A."SeriesName" IN ('NDSinFE','NCSinFE'))
AND T0."Series" NOT IN (72, -1)

UNION ALL

SELECT
--Lineas NC
'04' "TipoDocumento"
,T0."DocEntry"
,'NULL' AS "NumeroIdentidadTercero"
,'NULL' AS "NombreTercero"
,'NULL' as "Detalle"
,'NULL' AS "Porcentaje"
,'NULL' AS "MontoCargo"

FROM ORIN T0
LEFT JOIN RIN1 T1 ON T0."DocEntry" = T1."DocEntry"
WHERE T0."DocDate" >= '20190701'
AND T0."CANCELED" = 'N'
AND T0."DocType" = 'I'
AND T1."PriceBefDi" <> 0
AND T1."Quantity" <> 0
--AND T1."U_OtrosCargos" = 1
AND T0."Series" NOT IN (Select A."Series" from NNM1 A where A."SeriesName" IN ('NDSinFE','NCSinFE'))
;

GO
/****** Object:  View [dbo].[CLVS_FE_SYNC_SN_REAL43]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[CLVS_FE_SYNC_SN_REAL43] AS
SELECT
'SUMEDICA' "COMP"
,T0."CardCode" "Cliente"
,T0."CardName" "NombreCliente2"
,T0."U_TipoIdentificacion"
,T0."LicTradNum" "CedulaCliente"
,T0."Phone1" "TelefonoCliente"
,T0."E_Mail" "EmailCliente"
,T0."U_provincia"
,T0."U_canton"
,T0."U_distrito"
,T0."U_barrio"
,T0."U_direccion" 
,DATEPART(YEAR, T0.[CreateDate])	"Año creacion"					
,DATEPART(MONTH, T0.[CreateDate])	"Mes creacion"					
,DATEPART(DAY, T0.[CreateDate])		"Dia creacion"
,T0.[CreateDate]					"Fecha creacion"
,DATEPART(YEAR,(SELECT MAX("DocDate") FROM OINV A WHERE A."CardCode" = T0."CardCode")) "Año AUlt Venta"
,DATEPART(MONTH,(SELECT MAX("DocDate") FROM OINV A WHERE A."CardCode" = T0."CardCode")) "Mes AUlt Venta"
,DATEPART(DAY,(SELECT MAX("DocDate") FROM OINV A WHERE A."CardCode" = T0."CardCode")) "Dia AUlt Venta"
,(SELECT MAX("DocDate") FROM OINV A WHERE A."CardCode" = T0."CardCode") "Fecha AUlt Venta"
From OCRD T0 
Where T0."CardType" = 'C';
GO
/****** Object:  View [dbo].[CLVS_INVENTARIO_GEN_EST]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW	[dbo].[CLVS_INVENTARIO_GEN_EST] AS
SELECT
T0."WhsCode"    	            "Cod Almacen"
, T1."WhsName"    	            "Almacen"
, T5."FirmName"   	            "Marca"
, T2."ItmsGrpCod" 	            "Cod Grupo Articulos"
, T4."ItmsGrpNam" 	            "Grupo Articulos"
, T0."ItemCode"   	            "Cod Articulo"
, T2."ItemName"   	            "Articulo"
, CAST(T2."CreateDate" AS DATE)	"Fecha creacion articulo"
, T2.[InvntryUom]				"Unidad medida"
, T0."OnOrder"    	            "Cant en Pedido"
, T0."OnHand"     	            "Cant en Stock"
, T2."AvgPrice"   	            "Costo Unitario"
, T0."OnHand"*T2."AvgPrice"	    "Costo Stock"
, T3."Cant M-3" "Cant vendida M-3"
, T3."Cant M-2" "Cant vendida M-2"
, T3."Cant M-1" "Cant vendida M-1"
, T3."Cant M ACT" "Cant vendida M ACT"
, T3."Cant M ULT3" "Cant vendida M ULT3"
, T3."Cant D ULT90" "Cant vendida D ULT90"
, T3."Cost D ULT90" "Cost ventas D ULT90"

FROM	    OITW	T0
LEFT JOIN	OWHS	T1	ON	T0."WhsCode"	=	T1."WhsCode"
LEFT JOIN	OITM	T2	ON	T0."ItemCode"	=	T2."ItemCode"
LEFT JOIN
(
SELECT A."ItemCode", A."WhsCode",
SUM(A."Cant M-3") "Cant M-3",
SUM(A."Cant M-2") "Cant M-2",
SUM(A."Cant M-1") "Cant M-1" ,
SUM(A."Cant M ACT") "Cant M ACT",
SUM(A."Cant M ULT3") "Cant M ULT3",
SUM(A."Cant D ULT90") "Cant D ULT90",
SUM(A."Cost D ULT90") "Cost D ULT90"
FROM
(
SELECT
T1."ItemCode", T1."WhsCode",
SUM( CASE WHEN FORMAT( T0."DocDate",'YYYYMM') BETWEEN  FORMAT(DATEADD(month,-3,GETDATE()),'YYYYMM') AND  FORMAT(DATEADD(month,-3,GETDATE()),'YYYYMM') THEN T1."Quantity" ELSE 0 END)	 "Cant M-3",
SUM( CASE WHEN FORMAT( T0."DocDate",'YYYYMM') BETWEEN  FORMAT(DATEADD(month,-2,GETDATE()),'YYYYMM') AND  FORMAT(DATEADD(month,-2,GETDATE()),'YYYYMM') THEN T1."Quantity" ELSE 0 END)	 "Cant M-2",
SUM( CASE WHEN FORMAT( T0."DocDate",'YYYYMM') BETWEEN  FORMAT(DATEADD(month,-1,GETDATE()),'YYYYMM') AND  FORMAT(DATEADD(month,-1,GETDATE()),'YYYYMM') THEN T1."Quantity" ELSE 0 END)	 "Cant M-1",
SUM( CASE WHEN FORMAT( T0."DocDate",'YYYYMM') =  FORMAT(GETDATE(),'YYYYMM') THEN T1."Quantity" ELSE 0 END)	 "Cant M ACT",
SUM( CASE WHEN FORMAT( T0."DocDate",'YYYYMM') BETWEEN  FORMAT(DATEADD(month,-3,GETDATE()),'YYYYMM') AND  FORMAT(DATEADD(month,-1,GETDATE()),'YYYYMM') THEN T1."Quantity" ELSE 0 END )	 "Cant M ULT3",
SUM( CASE WHEN T0."DocDate" BETWEEN DATEADD(day,-90,GETDATE()) AND GETDATE() THEN T1."Quantity" ELSE 0 END)	 "Cant D ULT90",
SUM( CASE WHEN T0."DocDate" BETWEEN DATEADD(day,-90,GETDATE()) AND GETDATE() THEN T1."Quantity"*T1."StockPrice" ELSE 0 END)	 "Cost D ULT90"
FROM
OINV T0 INNER JOIN INV1 T1 ON T0."DocEntry" = T1."DocEntry"
WHERE
T1."Quantity" > 0 AND T0."CANCELED" = 'N'
GROUP BY
T1."ItemCode", T1."WhsCode"

UNION ALL

SELECT
T1."ItemCode", T1."WhsCode",
SUM( CASE WHEN FORMAT( T0."DocDate",'YYYYMM') BETWEEN  FORMAT(DATEADD(month,-3,GETDATE()),'YYYYMM') AND  FORMAT(DATEADD(month,-3,GETDATE()),'YYYYMM') THEN T1."Quantity" ELSE 0 END)*-1	 "Cant M-3",
SUM( CASE WHEN FORMAT( T0."DocDate",'YYYYMM') BETWEEN  FORMAT(DATEADD(month,-2,GETDATE()),'YYYYMM') AND  FORMAT(DATEADD(month,-2,GETDATE()),'YYYYMM') THEN T1."Quantity" ELSE 0 END)*-1	 "Cant M-2",
SUM( CASE WHEN FORMAT( T0."DocDate",'YYYYMM') BETWEEN  FORMAT(DATEADD(month,-1,GETDATE()),'YYYYMM') AND  FORMAT(DATEADD(month,-1,GETDATE()),'YYYYMM') THEN T1."Quantity" ELSE 0 END)*-1	 "Cant M-1",
SUM( CASE WHEN FORMAT( T0."DocDate",'YYYYMM') =  FORMAT(GETDATE(),'YYYYMM') THEN T1."Quantity" ELSE 0 END)*-1	 "Cant M ACT",
SUM( CASE WHEN FORMAT( T0."DocDate",'YYYYMM') BETWEEN  FORMAT(DATEADD(month,-3,GETDATE()),'YYYYMM') AND  FORMAT(DATEADD(month,-3,GETDATE()),'YYYYMM') THEN T1."Quantity" ELSE 0 END)*-1	 "Cant M ULT3",
SUM( CASE WHEN T0."DocDate" BETWEEN DATEADD(day,-90,GETDATE()) AND GETDATE() THEN T1."Quantity" ELSE 0 END)*-1	 "Cant D ULT90",
SUM( CASE WHEN T0."DocDate" BETWEEN DATEADD(day,-90,GETDATE()) AND GETDATE() THEN T1."Quantity"*T1."StockPrice" ELSE 0 END)*-1	 "Cost D ULT90"
FROM
ORIN T0 INNER JOIN RIN1 T1 ON T0."DocEntry" = T1."DocEntry"
WHERE
T1."Quantity" > 0 AND T0."CANCELED" = 'N'
GROUP BY
T1."ItemCode", T1."WhsCode"
) A GROUP BY A."ItemCode", A."WhsCode"
)

T3	ON	T0."ItemCode"	=	T3."ItemCode"	AND	T0."WhsCode"	=	T3."WhsCode"
LEFT JOIN	OITB	T4	ON	T2."ItmsGrpCod"	=	T4."ItmsGrpCod"
LEFT JOIN	OMRC	T5	ON	T2."FirmCode"	=	T5."FirmCode"

WHERE	(T0."OnHand" > 0 OR T0."OnOrder" <> 0 OR T3."Cant M-3" > 0
OR T3."Cant M-2" > 0
OR T3."Cant M-1" > 0
OR T3."Cant M ACT" > 0
OR T3."Cant M ULT3" > 0
OR T3."Cant D ULT90" > 0
OR T3."Cost D ULT90" > 0)

GO
/****** Object:  View [dbo].[CLVS_PAGOS_GEN_EST]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW	[dbo].[CLVS_PAGOS_GEN_EST] AS
SELECT
T0."CardCode"	 "Cod Proveedor",
T2."CardName"	 "Proveedor",
T0."CardName"	 "Proveedor en Factura",
T7."GroupName"	 "Grupo Proveedor",
T5."USER_CODE"	 "Usuario",
T6."SlpName"	 "Agente",
T3."ExtraDays"	 "Dias Proveedor",
DATEDIFF(DAY,T1."DueDate",GETDATE())	 "Dias atraso",
T0."CANCELED"	 "Cancelado",
CASE T1."Status" WHEN 'O' THEN 'Abierto' WHEN 'C' THEN 'Cerrado' END		 "Estado",
DATEPART(YEAR, T0.[DocDate])	"Año Conta",					
DATEPART(MONTH, T0.[DocDate])	"Mes Conta",					
T0."DocDate"	 "Fecha",
DATEPART(YEAR, T1.[DueDate])	"Año Vence",
DATEPART(MONTH, T1.[DueDate])	"Mes Vence",
T0."DocDueDate"	 "Fecha vencimiento",
CASE WHEN T4."SeriesName" IS NULL THEN 'Manual' ELSE T4."SeriesName" END	 "Serie Numeracion",
T0."DocNum"	 "# Doc",
T0."NumAtCard"	 "Referencia",
T0."DocCur"	 "Moneda",
T1."InstlmntID"	 "#Pagos",
CASE WHEN T0."ObjType"= 18 THEN 'Pago' ELSE 'Nota de debito' END	 "Tipo de documento",
CASE WHEN T0."DocCur" = 'COL' THEN T1."InsTotal" WHEN T0."DocCur" = 'USD' THEN T1."InsTotalSy" WHEN T0."DocCur" = 'EUR' THEN T1."InsTotalFC" END	 "Bruto",
CASE WHEN T0."DocCur" = 'COL' THEN T1."PaidToDate" WHEN T0."DocCur" = 'USD' THEN T1."PaidSys" WHEN T0."DocCur" = 'EUR' THEN T1."PaidFC" END	 "Aplicado",
CASE WHEN T0."DocCur" = 'COL' THEN (T1."InsTotal" - T1."PaidToDate") WHEN T0."DocCur" = 'USD' THEN (T1."InsTotalSy" - T1."PaidSys") WHEN T0."DocCur" = 'EUR' THEN (T1."InsTotalFC" - T1."PaidFC") END	 "Balance",
CASE WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 0 AND T0."DocCur" = 'COL' THEN (T1."InsTotal" - T1."PaidToDate") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 0 AND T0."DocCur" = 'USD' THEN (T1."InsTotalSy" - T1."PaidSys") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 8 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=1 AND T0."DocCur" = 'EUR' THEN (T1."InsTotalFC" - T1."PaidFC") END	 "Sin vencer",
CASE WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 8 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=1 AND T0."DocCur" = 'COL' 
THEN (T1."InsTotal" - T1."PaidToDate") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 8 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=1 AND T0."DocCur" = 'USD' 
THEN (T1."InsTotalSy" - T1."PaidSys") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 8 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=1 AND T0."DocCur" = 'EUR' 
THEN (T1."InsTotalFC" - T1."PaidFC") END	 "1-8 días",
CASE WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <=15 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=9 AND T0."DocCur" = 'COL' 
THEN (T1."InsTotal" - T1."PaidToDate") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 15 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=9 AND T0."DocCur" = 'USD' 
THEN (T1."InsTotalSy" - T1."PaidSys") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 15 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=9 AND T0."DocCur" = 'EUR' 
THEN (T1."InsTotalFC" - T1."PaidFC") END	 "9-15 días",
CASE WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 30 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=16 AND T0."DocCur" = 'COL' 
THEN (T1."InsTotal" - T1."PaidToDate") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 30 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=16 AND T0."DocCur" = 'USD' 
THEN (T1."InsTotalSy" - T1."PaidSys") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 30 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=16 AND T0."DocCur" = 'EUR' 
THEN (T1."InsTotalFC" - T1."PaidFC") END	 "16-30 días",
CASE WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 60 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=31 AND T0."DocCur" = 'COL' 
THEN (T1."InsTotal" - T1."PaidToDate") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 60 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=31 AND T0."DocCur" = 'USD' 
THEN (T1."InsTotalSy" - T1."PaidSys") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 60 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=31 AND T0."DocCur" = 'EUR' 
THEN (T1."InsTotalFC" - T1."PaidFC") END	 "31-60 días",
CASE WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 90 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=61 AND T0."DocCur" = 'COL' 
THEN (T1."InsTotal" - T1."PaidToDate") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 90 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=61 AND T0."DocCur" = 'USD' 
THEN (T1."InsTotalSy" - T1."PaidSys") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 90 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=61 AND T0."DocCur" = 'EUR' 
THEN (T1."InsTotalFC" - T1."PaidFC") END	 "61-90 días",
CASE WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 120 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=91 AND T0."DocCur" = 'COL' 
THEN (T1."InsTotal" - T1."PaidToDate") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 120 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=91 AND T0."DocCur" = 'USD' 
THEN (T1."InsTotalSy" - T1."PaidSys") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 120 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=91 AND T0."DocCur" = 'EUR' 
THEN (T1."InsTotalFC" - T1."PaidFC") END	 "91-120 días",
CASE WHEN DATEDIFF(DAY,T1."DueDate", GETDATE())>120 AND T0."DocCur" = 'COL' 
THEN (T1."InsTotal" - T1."PaidToDate") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE())>120 AND T0."DocCur" = 'USD' 
THEN (T1."InsTotalSy" - T1."PaidSys") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE())>120 AND T0."DocCur" = 'EUR' 
THEN (T1."InsTotalFC" - T1."PaidFC") END	 "120+ días"
FROM	OPCH	T0
LEFT JOIN	PCH6	T1	ON	T0."DocEntry"	=	T1."DocEntry"
LEFT JOIN	OCRD	T2	ON	T0."CardCode"	=	T2."CardCode"
LEFT JOIN	OCTG	T3	ON	T2."GroupNum"	=	T3."GroupNum"
LEFT JOIN	NNM1	T4	ON	T0."Series"	=	T4."Series"
LEFT JOIN	OUSR	T5	ON	T0."UserSign"	=	T5."USERID"
LEFT JOIN	OSLP	T6	ON	T0."SlpCode"	=	T6."SlpCode"
LEFT JOIN	OCRG	T7	ON	T2."GroupCode"	=	T7."GroupCode"
WHERE	T1."TotalBlck" <> T1."InsTotalSy" AND T1."Status" = 'O'

UNION ALL
SELECT
T0."CardCode"	 "Cod Proveedor",
T2."CardName"	 "Proveedor",
T0."CardName"	 "Proveedor en Factura",
T7."GroupName"	 "Grupo Proveedor",
T5."USER_CODE"	 "Usuario",
T6."SlpName"	 "Agente",
T3."ExtraDays"	 "Dias Proveedor",
DATEDIFF(DAY,T1."DueDate",GETDATE())	 "Dias atraso",
T0."CANCELED"	 "Cancelado",
CASE T1."Status" WHEN 'O' THEN 'Abierto' WHEN 'C' THEN 'Cerrado' END		 "Estado",
DATEPART(YEAR, T0.[DocDate])	"Año Conta",					
DATEPART(MONTH, T0.[DocDate])	"Mes Conta",					
T0."DocDate"	 "Fecha",
DATEPART(YEAR, T1.[DueDate])	"Año Vence",
DATEPART(MONTH, T1.[DueDate])	"Mes Vence",
T0."DocDueDate"	 "Fecha vencimiento",
 CASE WHEN T4."SeriesName" IS NULL THEN 'Manual' ELSE T4."SeriesName" END	 "Serie Numeracion",
T0."DocNum"	 "# Doc",
T0."NumAtCard"	 "Referencia",
T0."DocCur"	 "Moneda",
T1."InstlmntID"	 "#Pagos",
CASE WHEN T0."ObjType"= 18 THEN 'Pago' ELSE 'Nota de debito' END	 "Tipo de documento",
CASE WHEN T0."DocCur" = 'COL' THEN T1."InsTotal"*-1 WHEN T0."DocCur" = 'USD' THEN T1."InsTotalSy"*-1 WHEN T0."DocCur" = 'EUR' THEN T1."InsTotalFC"*-1 END	 "Bruto",
CASE WHEN T0."DocCur" = 'COL' THEN T1."PaidToDate"*-1 WHEN T0."DocCur" = 'USD' THEN T1."PaidSys"*-1 WHEN T0."DocCur" = 'EUR' THEN T1."PaidFC"*-1 END	 "Aplicado",
CASE WHEN T0."DocCur" = 'COL' THEN (T1."InsTotal" - T1."PaidToDate")*-1 WHEN T0."DocCur" = 'USD' THEN (T1."InsTotalSy" - T1."PaidSys")*-1 WHEN T0."DocCur" = 'EUR' THEN (T1."InsTotalFC" - T1."PaidFC")*-1 END	 "Balance",
CASE WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 0 AND T0."DocCur" = 'COL' THEN (T1."InsTotal" - T1."PaidToDate")*-1 WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 0 AND T0."DocCur" = 'USD' THEN (T1."InsTotalSy" - T1."PaidSys")*-1 WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 0 AND T0."DocCur" = 'EUR' THEN (T1."InsTotalFC" - T1."PaidFC")*-1 END	 "Sin vencer",
CASE WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 8 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=1 AND T0."DocCur" = 'COL' 
THEN (T1."InsTotal" - T1."PaidToDate") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 8 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=1 AND T0."DocCur" = 'USD' 
THEN (T1."InsTotalSy" - T1."PaidSys") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 8 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=1 AND T0."DocCur" = 'EUR' 
THEN (T1."InsTotalFC" - T1."PaidFC") END	 "1-8 días",
CASE WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <=15 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=9 AND T0."DocCur" = 'COL' 
THEN (T1."InsTotal" - T1."PaidToDate") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 15 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=9 AND T0."DocCur" = 'USD' 
THEN (T1."InsTotalSy" - T1."PaidSys") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 15 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=9 AND T0."DocCur" = 'EUR' 
THEN (T1."InsTotalFC" - T1."PaidFC") END	 "9-15 días",
CASE WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 30 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=16 AND T0."DocCur" = 'COL' 
THEN (T1."InsTotal" - T1."PaidToDate") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 30 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=16 AND T0."DocCur" = 'USD' 
THEN (T1."InsTotalSy" - T1."PaidSys") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 30 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=16 AND T0."DocCur" = 'EUR' 
THEN (T1."InsTotalFC" - T1."PaidFC") END	 "16-30 días",
CASE WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 60 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=31 AND T0."DocCur" = 'COL' 
THEN (T1."InsTotal" - T1."PaidToDate") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 60 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=31 AND T0."DocCur" = 'USD' 
THEN (T1."InsTotalSy" - T1."PaidSys") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 60 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=31 AND T0."DocCur" = 'EUR' 
THEN (T1."InsTotalFC" - T1."PaidFC") END	 "31-60 días",
CASE WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 90 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=61 AND T0."DocCur" = 'COL' 
THEN (T1."InsTotal" - T1."PaidToDate") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 90 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=61 AND T0."DocCur" = 'USD' 
THEN (T1."InsTotalSy" - T1."PaidSys") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 90 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=61 AND T0."DocCur" = 'EUR' 
THEN (T1."InsTotalFC" - T1."PaidFC") END	 "61-90 días",
CASE WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 120 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=91 AND T0."DocCur" = 'COL' 
THEN (T1."InsTotal" - T1."PaidToDate") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 120 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=91 AND T0."DocCur" = 'USD' 
THEN (T1."InsTotalSy" - T1."PaidSys") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE()) <= 120 AND DATEDIFF(DAY,T1."DueDate", GETDATE()) >=91 AND T0."DocCur" = 'EUR' 
THEN (T1."InsTotalFC" - T1."PaidFC") END	 "91-120 días",
CASE WHEN DATEDIFF(DAY,T1."DueDate", GETDATE())>120 AND T0."DocCur" = 'COL' 
THEN (T1."InsTotal" - T1."PaidToDate") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE())>120 AND T0."DocCur" = 'USD' 
THEN (T1."InsTotalSy" - T1."PaidSys") WHEN DATEDIFF(DAY,T1."DueDate", GETDATE())>120 AND T0."DocCur" = 'EUR' 
THEN (T1."InsTotalFC" - T1."PaidFC") END	 "120+ días"
FROM	ORPC	T0
LEFT JOIN	RPC6	T1	ON	T0."DocEntry"	=	T1."DocEntry"
LEFT JOIN	OCRD	T2	ON	T0."CardCode"	=	T2."CardCode"
LEFT JOIN	OCTG	T3	ON	T2."GroupNum"	=	T3."GroupNum"
LEFT JOIN	NNM1	T4	ON	T0."Series"	=	T4."Series"
LEFT JOIN	OUSR	T5	ON	T0."UserSign"	=	T5."USERID"
LEFT JOIN	OSLP	T6	ON	T0."SlpCode"	=	T6."SlpCode"
LEFT JOIN	OCRG	T7	ON	T2."GroupCode"	=	T7."GroupCode"
WHERE	T1."TotalBlck" <> T1."InsTotalSy" AND T1."Status" = 'O'

UNION ALL
SELECT
T0."CardCode"	 "Cod Proveedor",
T1."CardName"	 "Proveedor",
T0."CardName"	 "Proveedor en Factura",
T5."GroupName"	 "Grupo Proveedor",
'NA'	 "Usuario",
'NA'	 "Agente",
T2."ExtraDays"	 "Dias Proveedor",
DATEDIFF(DAY,T0."DocDueDate", GETDATE())	 "Dias atraso",
T0."Canceled"	 "Cancelado",
CASE WHEN T0."OpenBal"<>0 THEN 'Abierto' WHEN T0."OpenBal"=0 THEN 'Cerrado' END		 "Estado",
DATEPART(YEAR, T0.[DocDate])	"Año Conta",					
DATEPART(MONTH, T0.[DocDate])	"Mes Conta",					
T0."DocDate"	 "Fecha",
DATEPART(YEAR, T0.[DocDueDate])	"Año Vence",
DATEPART(MONTH, T0.[DocDueDate])	"Mes Vence",
T0."DocDueDate"	 "Fecha vencimiento",
 CASE WHEN T4."SeriesName" IS NULL THEN 'Manual' ELSE T4."SeriesName" END	 "Serie Numeracion",
T0."DocNum"	 "# Doc",
T0."CounterRef"	 "Referencia",
T0."DocCurr"	 "Moneda",
0	 "#Pagos",
CASE WHEN T0."DocType" = 'S' THEN 'Anticipo' END	 "Tipo de documento",     
CASE WHEN T0."DocCurr" = 'COL' THEN T0."DocTotal" *-1 WHEN T0."DocCurr" = 'USD' THEN T0."DocTotalSy" *-1 WHEN T0."DocCurr" = 'EUR' THEN T0."DocTotalFC" *-1 END	 "Bruto",
CASE WHEN T0."DocCurr" = 'COL' THEN (T0."DocTotal"-T0."OpenBal")*-1 WHEN T0."DocCurr" = 'USD' THEN (T0."DocTotalSy"-T0."OpenBalSc")*-1 WHEN T0."DocCurr" = 'EUR' THEN (T0."DocTotalFC"-T0."OpenBalFc")*-1 END	 "Aplicado",
CASE WHEN T0."DocCurr" = 'COL' THEN T0."OpenBal"*-1 WHEN T0."DocCurr" = 'USD' THEN T0."OpenBalSc"*-1 WHEN T0."DocCurr" = 'EUR' THEN T0."OpenBalFc"*-1 END	 "Balance",
CASE WHEN T0."DocCurr" = 'COL' THEN T0."OpenBal"*-1 WHEN T0."DocCurr" = 'USD' THEN T0."OpenBalSc"*-1 WHEN T0."DocCurr" = 'EUR' THEN T0."OpenBalFc"*-1 END	 "Sin Vencer",
0	 "1-8 días",
0	 "9-15 días",
0	 "16-30 días",
0	 "31-60 días",
0	 "61-90 días",
0	 "91-120 días",
0	 "120+ días"
FROM	OVPM	T0
LEFT JOIN	OCRD	T1	ON	T0."CardCode"	=	T1."CardCode"
LEFT JOIN	OCTG	T2	ON	T1."GroupNum"	=	T2."GroupNum"
LEFT JOIN	NNM1	T4	ON	T0."Series"	=	T4."Series"
LEFT JOIN	OCRG	T5	ON	T1."GroupCode"	=	T5."GroupCode"
WHERE	T0."OpenBal"<>0

GO
/****** Object:  View [dbo].[CLVS_POS_ACCOUNTS_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[CLVS_POS_ACCOUNTS_SPR] AS
	SELECT 
		CONCAT(CONCAT(T0."Segment_0", '-'), T0."AcctName") AS ACCOUNTNAME,
		T0."AcctCode" AS ACCOUNT
	FROM 
		OACT T0 
	WHERE 
		T0."Finanse" = 'Y';
GO
/****** Object:  View [dbo].[CLVS_POS_BPS_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[CLVS_POS_BPS_SPR]
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
		WHEN UPPER(PymntGroup) = 'CONTADO' THEN 1 
		ELSE 0 END ClienteContado
FROM
	dbo.OCRD AS CUS (NOLOCK)
		INNER JOIN dbo.OCTG T1 (NOLOCK) 
			ON CardType = 'C' AND 
				frozenFor = 'N' AND 
				CUS.GroupNum = T1.GroupNum;
GO
/****** Object:  View [dbo].[CLVS_POS_BPS_SUPPLIERS_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[CLVS_POS_BPS_SUPPLIERS_SPR]
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
/****** Object:  View [dbo].[CLVS_POS_CREDITCARDS_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[CLVS_POS_CREDITCARDS_SPR] AS
SELECT 
	CONCAT(CONCAT(CreditCard.CreditCard, ' '), CreditCard.CardName) AS CardName,
	CreditCard.AcctCode 
FROM 
	OCRC AS CreditCard;
GO
/****** Object:  View [dbo].[CLVS_POS_EXRATE_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[CLVS_POS_EXRATE_SPR] AS
SELECT
	T0.RateDate,
	T0.Rate
FROM
	ORTT T0 
WHERE 
	T0.RateDate = CONVERT(date, GETDATE()) 
	AND Currency = 'USD';
GO
/****** Object:  View [dbo].[CLVS_POS_GETAllPRICELIST_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CLVS_POS_GETAllPRICELIST_SPR]
AS
SELECT        ListNum, ListName
FROM            dbo.OPLN
GO
/****** Object:  View [dbo].[CLVS_POS_GETBANKS_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[CLVS_POS_GETBANKS_SPR] AS
SELECT
	BankCodes.BankCode,
	BankCodes.BankName 
FROM
	ODSC BankCodes;
GO
/****** Object:  View [dbo].[CLVS_POS_GETFEINFORMATIONFROMINV_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[CLVS_POS_GETFEINFORMATIONFROMINV_SPR]
AS
SELECT	MAX(CardCode) CardCode, MAX(CardName) CardName, ISNULL(U_TipoIdentificacion,'') U_TipoIdentificacion, ISNULL(U_NumIdentFE,'') U_NumIdentFE, MAX(ISNULL(U_CorreoFE,'')) U_CorreoFE,
		MAX(ISNULL(U_Provincia,'')) U_provincia, MAX(ISNULL(U_Canton,'')) U_canton, MAX(ISNULL(U_Distrito,'')) U_distrito, 
		MAX(ISNULL(U_Barrio,'')) U_barrio, MAX(ISNULL(U_Direccion,'')) U_direccion,MAX(DocNum) MaxDocNum
FROM	dbo.OINV (NOLOCK)
GROUP BY ISNULL(U_TipoIdentificacion,''), ISNULL(U_NumIdentFE,'')
GO
/****** Object:  View [dbo].[CLVS_POS_GETFIRMSLIST_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[CLVS_POS_GETFIRMSLIST_SPR] AS

SELECT FirmCode, FirmName
FROM OMRC
GO
/****** Object:  View [dbo].[CLVS_POS_GETGROUPLIST_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[CLVS_POS_GETGROUPLIST_SPR] AS

SELECT ItmsGrpNam, ItmsGrpCod
FROM OITB 
GO
/****** Object:  View [dbo].[CLVS_POS_GETOTCX_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[CLVS_POS_GETOTCX_SPR]
AS
SELECT LnTaxCode, StrVal1 
FROM TST_CL_SUPERTORM.dbo.OTCX
WHERE BusArea = 0;
GO
/****** Object:  View [dbo].[CLVS_POS_GETPRICELIST_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[CLVS_POS_GETPRICELIST_SPR] AS

SELECT ListNum, ListName
FROM OPLN
WHERE ListNum=1
GO
/****** Object:  View [dbo].[CLVS_POS_GETSALESMAN_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[CLVS_POS_GETSALESMAN_SPR] AS
SELECT 
	SM.SlpCode,
	SM.SlpName
FROM 
	OSLP SM 
WHERE 
	SM.Active = 'Y'
GO
/****** Object:  View [dbo].[CLVS_POS_GETTAXES_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[CLVS_POS_GETTAXES_SPR] AS
SELECT 
	Code,
	Rate
FROM
	OSTC;
GO
/****** Object:  View [dbo].[CLVS_POS_GETWHAREHOUSES]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[CLVS_POS_GETWHAREHOUSES] AS

SELECT WH.WhsCode, WH.WhsName
FROM OWHS WH 
GO
/****** Object:  View [dbo].[CLVS_POS_ITEMS_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CLVS_POS_ITEMS_SPR]
AS
SELECT 
	T0.ItemCode, 
	T0.ItemName, 
	--ISNULL(
		--OB.BcdCode, 
		OB.BcdCode as CodeBars,
		--(SELECT top 1 BcdCode FROM OBCD WHERE ItemCode = ItemCode)) as CodeBars, 
	T1.OnHand AS Available -- buscar utilidad o eliminar
	-- RESPALDO SUM(T1.OnHand) AS Available -- buscar utilidad o eliminar
FROM 
	OITM AS T0  (NOLOCK)
		INNER JOIN dbo.OITW AS T1 (NOLOCK) 
			ON T0.frozenFor = 'N' AND 
			T0.SellItem = 'Y' AND
			T1.ItemCode = T0.ItemCode
	JOIN OBCD OB ON OB.ItemCode = T0.ItemCode
--GROUP BY 
--T0.ItemCode, 
--T0.ItemName, 
--T0.CodeBars;

GO
/****** Object:  View [dbo].[CLVS_POS_PAYTERMS_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CLVS_POS_PAYTERMS_SPR] AS

SELECT GroupNum, 
		PymntGroup,
		CASE --Type: 1-Credito, 2-Contado, se valida de esta manera para identificar el tipo de pago y disparar o no el modal de pago en el UI del App
			WHEN GroupNum = 1 THEN '1'
			WHEN GroupNum = 2 THEN '2'
			ELSE '0'
		END AS Type
FROM OCTG

GO
/****** Object:  View [dbo].[CLVS_SUPMD_GETITEMS]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE VIEW [dbo].[CLVS_SUPMD_GETITEMS]
AS
	SELECT TOP 10000
		CB.BcdCode AS CodeBars,
		T0.ItemCode AS Code,
		T0.ItemName AS ItemName,
		T0.FrgnName AS ForeignName,
		COALESCE(P.Price, 0) AS Price,
		COALESCE(T0.OnHand, 0) AS Quantity,
		(CASE
			WHEN T0.U_IVA IS NULL THEN 'EXE'
			WHEN T0.U_IVA = '' THEN 'EXE'
			ELSE T0.U_IVA
		END) AS TaxCode, 
		(CASE
			WHEN T0.U_IVA IS NULL THEN 0
			WHEN T0.U_IVA = '' THEN 0
			WHEN T0.U_IVA='100EXE' THEN 0
			WHEN T0.U_IVA='EXE' THEN 0
			WHEN T0.U_IVA='1IVA' THEN 1
			WHEN T0.U_IVA='2IVA' THEN 2
			WHEN T0.U_IVA='4IVA' THEN 4
			WHEN T0.U_IVA='13IVA' THEN 13
		END) AS TaxRate
	FROM 
		OITM T0 JOIN ITM1 P ON T0.ItemCode = P.ItemCode AND P.PriceList = 1
			JOIN OBCD CB ON CB.ItemCode = T0.ItemCode
	ORDER BY T0.ItemName ASC;
GO
/****** Object:  View [dbo].[CLVS_VENTAS_DET]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW	[dbo].[CLVS_VENTAS_DET] AS
SELECT	
T0.DocEntry,					
T0.[DocDate]					"Fecha",					
DATEPART(YEAR, T0.[DocDate])	"Año",					
DATEPART(MONTH, T0.[DocDate])	"Mes",					
DATEPART(DAY, T0.[DocDate])		"Dia",
DATEPART(MONTH, T11.[Fecha])	"Mes ult conciliacion",				
T0.[DocTime]					"Hora",					
T5.[WhsName]					"Almacen",					
T0.[CardCode]					"Cod Cliente",					
T8.[CardName]					"Cliente",					
T0.[CardName]					"Cliente en Factura",					

CASE 
WHEN T9.[SeriesName] IS NULL THEN 'Manual' 
ELSE T9.[SeriesName] 
END								"Serie Numeracion",
					
T0.[DocNum]						"# Doc",
					
CASE T0.[DocStatus]
WHEN 'O' THEN 'Abierto'
WHEN 'C' THEN 'Cerrado'
END								"Estado Documento",
			
CASE T0.[CANCELED] 
WHEN 'N' THEN 'No' 
WHEN 'Y' THEN 'Si' 
WHEN 'C' THEN 'Cancelacion' 
END								"Cancelado",

'Factura'						"Clase de Documento",
					
CASE T0.[DocType] 
WHEN 'S' THEN 'Servicio' 
WHEN 'I' THEN 'Articulo' 
END								"Tipo de documento",					
						
T4.[USER_CODE]		"Usuario",	
T6.[SlpName]		"Agente",
T1.[Currency]		"Moneda",
T0.[Comments]		"Comentarios",	
T7.[FirmName]		"Marca",
T3.[ItmsGrpNam]		"Grupo Articulos",
T2.[U_categoria1]	"Categoria1",
T10.[CardName]		"Proveedor",
T1.[ItemCode]		"Cod Articulo",
T2.[ItemName]		"Articulo",
T1.[Dscription]		"Articulo en Factura",	
T1.[Quantity]		"Cantidad",
T1.[TaxCode]		"Impuesto",

T1.[LineTotal]	 "Subtotal LOC",					
(T1.[LineTotal]* (COALESCE(T0.[DiscPrcnt],0)/100))	 "Descuento CAB LOC",					

(T1.[LineTotal] - --Subtotal
(T1.[LineTotal]* (COALESCE(T0.[DiscPrcnt],0)/100)))	 --Descuento CAB LOC
"Total Neto LOC",

(T1.[LineTotal] - --Subtotal
(T1.[LineTotal]* (COALESCE(T0.[DiscPrcnt],0)/100))) -	 --Descuento CAB LOC
T1.[GrssProfit]	 "Costo LOC",					

T1.[GrssProfit]	 "Utilidad LOC",
T1.[GrssProfit]*.1	 "Comision LOC",
T1.[VatSum]		 "Impuesto LOC",

T1.[TotalSumSy]	 "Subtotal USD",					
(T1.[TotalSumSy]* (COALESCE(T0.[DiscPrcnt],0)/100))	 "Descuento CAB USD",					

(T1.[TotalSumSy] - --Subtotal
(T1.[TotalSumSy]* (COALESCE(T0.[DiscPrcnt],0)/100)))	 --Descuento CAB USD
"Total Neto USD",

(T1.[TotalSumSy] - --Subtotal
(T1.[TotalSumSy]* (COALESCE(T0.[DiscPrcnt],0)/100))) -	 --Descuento CAB USD
T1.[GrssProfSC]	 "Costo USD",

T1.[GrssProfSC]	 "Utilidad USD",
T1.[GrssProfSC]*.1	 "Comision USD",					
T1.[VatSumSy]	 "Impuesto USD",

T1.[AcctCode]		"Cuenta ingresos",
--T10.[AcctName]		"Nombre cuenta ingresos",
T1.[OcrCode]		"CC Ingresos", 
T1.[CogsAcct]		"Cuenta costos",
--T11.[AcctName]		"Nombre cuenta costos",
T1.[CogsOcrCod]		"CC Costo"

FROM		OINV	T0				
LEFT JOIN	INV1	T1	ON	T0.[DocEntry]	=	T1.[DocEntry]
LEFT JOIN	OITM	T2	ON	T1.[ItemCode]	=	T2.[ItemCode]
LEFT JOIN	OITB	T3	ON	T2.[ItmsGrpCod]	=	T3.[ItmsGrpCod]
LEFT JOIN	OUSR	T4	ON	T0.[UserSign]	=	T4.[USERID]
LEFT JOIN	OWHS	T5	ON	T1.[WhsCode]	=	T5.[WhsCode]
LEFT JOIN	OSLP	T6	ON	T0.[SlpCode]	=	T6.[SlpCode]
LEFT JOIN	OMRC	T7	ON	T2.[FirmCode]	=	T7.[FirmCode]
LEFT JOIN	OCRD	T8	ON	T0.[CardCode]	=	T8.[CardCode]
LEFT JOIN	NNM1	T9	ON	T0.[Series]		=	T9.[Series]
LEFT JOIN	OCRD	T10 ON	T2.[U_proveedor]	=	T10.[CardCode]
--LEFT JOIN	OACT	T10	ON	T1.[AcctCode]	=	T10.[AcctCode]
--LEFT JOIN	OACT	T11	ON	T1.[CogsAcct]	=	T11.[AcctCode]
LEFT JOIN 
(
SELECT
T0.DocEntry "Llave",
T0.DocNum,
MAX(T3."ReconDate") AS "Fecha"

FROM OINV T0  
LEFT JOIN ITR1 T2 on T0."DocEntry"  = T2."SrcObjAbs" AND T0."ObjType"  = T2."SrcObjTyp"
INNER JOIN OITR T3 ON T2."ReconNum" = T3."ReconNum"

GROUP BY
T0.DocEntry,
T0.DocNum
) T11 ON T0.[DocEntry] = T11.[Llave]

UNION ALL

SELECT
T0.DocEntry,					
T0.[DocDate]					"Fecha",					
DATEPART(YEAR, T0.[DocDate])	"Año",					
DATEPART(MONTH, T0.[DocDate])	"Mes",					
DATEPART(DAY, T0.[DocDate])		"Dia",
DATEPART(MONTH, T11.[Fecha])	"Mes ult conciliacion",					
T0.[DocTime]					"Hora",					
T5.[WhsName]					"Almacen",					
T0.[CardCode]					"Cod Cliente",					
T8.[CardName]					"Cliente",					
T0.[CardName]					"Cliente en Factura",					

CASE 
WHEN T9.[SeriesName] IS NULL THEN 'Manual' 
ELSE T9.[SeriesName] 
END								"Serie Numeracion",
					
T0.[DocNum]						"# Doc",
					
CASE T0.[DocStatus]
WHEN 'O' THEN 'Abierto'
WHEN 'C' THEN 'Cerrado'
END								"Estado Documento",
			
CASE T0.[CANCELED] 
WHEN 'N' THEN 'No' 
WHEN 'Y' THEN 'Si' 
WHEN 'C' THEN 'Cancelacion' 
END								"Cancelado",

'NC'						"Clase de Documento",
					
CASE T0.[DocType] 
WHEN 'S' THEN 'Servicio' 
WHEN 'I' THEN 'Articulo' 
END								"Tipo de documento",					
						
T4.[USER_CODE]		"Usuario",	
T6.[SlpName]		"Agente",
T1.[Currency]		"Moneda",
T0.[Comments]		"Comentarios",	
T7.[FirmName]		"Marca",
T3.[ItmsGrpNam]		"Grupo Articulos",
T2.[U_categoria1]	"Categoria1",
T10.[CardName]		"Proveedor",
T1.[ItemCode]		"Cod Articulo",
T2.[ItemName]		"Articulo",
T1.[Dscription]		"Articulo en Factura",	
T1.[Quantity]		"Cantidad",
T1.[TaxCode]		"Impuesto",

T1.[LineTotal]*-1	 "Subtotal LOC",					
(T1.[LineTotal]* (COALESCE(T0.[DiscPrcnt],0)/100))*-1	 "Descuento CAB LOC",					

(T1.[LineTotal] - --Subtotal
(T1.[LineTotal]* (COALESCE(T0.[DiscPrcnt],0)/100)))*-1	 --Descuento CAB LOC
"Total Neto LOC",

((T1.[LineTotal] - --Subtotal
(T1.[LineTotal]* (COALESCE(T0.[DiscPrcnt],0)/100))) -	 --Descuento CAB LOC
T1.[GrssProfit])*-1	 "Costo LOC",					

T1.[GrssProfit]*-1	 "Utilidad LOC",
T1.[GrssProfit]*-1*.1	 "Comision LOC",
T1.[VatSum]*-1		 "Impuesto LOC",

T1.[TotalSumSy]*-1	 "Subtotal USD",					
(T1.[TotalSumSy]* (COALESCE(T0.[DiscPrcnt],0)/100))*-1	 "Descuento CAB USD",					

(T1.[TotalSumSy] - --Subtotal
(T1.[TotalSumSy]* (COALESCE(T0.[DiscPrcnt],0)/100)))*-1	 --Descuento CAB USD
"Total Neto USD",

((T1.[TotalSumSy] - --Subtotal
(T1.[TotalSumSy]* (COALESCE(T0.[DiscPrcnt],0)/100))) -	 --Descuento CAB USD
T1.[GrssProfSC])*-1	 "Costo USD",

T1.[GrssProfSC]*-1	 "Utilidad USD",
T1.[GrssProfSC]*-1*.1	 "Comision USD",					
T1.[VatSumSy]*-1	 "Impuesto USD",

T1.[AcctCode]		"Cuenta ingresos",
--T10.[AcctName]		"Nombre cuenta ingresos",
T1.[OcrCode]		"CC Ingresos", 
T1.[CogsAcct]		"Cuenta costos",
--T11.[AcctName]		"Nombre cuenta costos",
T1.[CogsOcrCod]		"CC Costos"
						
FROM		ORIN	T0				
LEFT JOIN	RIN1	T1	ON	T0.[DocEntry]	=	T1.[DocEntry]
LEFT JOIN	OITM	T2	ON	T1.[ItemCode]	=	T2.[ItemCode]
LEFT JOIN	OITB	T3	ON	T2.[ItmsGrpCod]	=	T3.[ItmsGrpCod]
LEFT JOIN	OUSR	T4	ON	T0.[UserSign]	=	T4.[USERID]
LEFT JOIN	OWHS	T5	ON	T1.[WhsCode]	=	T5.[WhsCode]
LEFT JOIN	OSLP	T6	ON	T0.[SlpCode]	=	T6.[SlpCode]
LEFT JOIN	OMRC	T7	ON	T2.[FirmCode]	=	T7.[FirmCode]
LEFT JOIN	OCRD	T8	ON	T0.[CardCode]	=	T8.[CardCode]
LEFT JOIN	NNM1	T9	ON	T0.[Series]		=	T9.[Series]
LEFT JOIN	OCRD	T10 ON	T2.[U_proveedor]	=	T10.[CardCode]
--LEFT JOIN	OACT	T10	ON	T1.[AcctCode]	=	T10.[AcctCode]
--LEFT JOIN	OACT	T11	ON	T1.[CogsAcct]	=	T11.[AcctCode]
LEFT JOIN 
(
SELECT
T0.DocEntry "Llave",
T0.DocNum,
MAX(T3."ReconDate") AS "Fecha"

FROM ORIN T0  
LEFT JOIN ITR1 T2 on T0."DocEntry"  = T2."SrcObjAbs" AND T0."ObjType"  = T2."SrcObjTyp"
INNER JOIN OITR T3 ON T2."ReconNum" = T3."ReconNum"

GROUP BY
T0.DocEntry,
T0.DocNum
) T11 ON T0.[DocEntry] = T11.[Llave]


GO
/****** Object:  View [dbo].[CLVS_VENTAS_DET_EST]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW	[dbo].[CLVS_VENTAS_DET_EST] AS
SELECT	
T0.DocEntry,					
T0.[DocDate]					"Fecha",					
DATEPART(YEAR, T0.[DocDate])	"Año",					
DATEPART(MONTH, T0.[DocDate])	"Mes",					
DATEPART(DAY, T0.[DocDate])		"Dia",
DATEPART(MONTH, T11.[Fecha])	"Mes ult conciliacion",				
T0.[DocTime]					"Hora",					
T5.[WhsName]					"Almacen",					
T0.[CardCode]					"Cod Cliente",					
T8.[CardName]					"Cliente",					
T0.[CardName]					"Cliente en Factura",					

CASE 
WHEN T9.[SeriesName] IS NULL THEN 'Manual' 
ELSE T9.[SeriesName] 
END								"Serie Numeracion",
					
T0.[DocNum]						"# Doc",
					
CASE T0.[DocStatus]
WHEN 'O' THEN 'Abierto'
WHEN 'C' THEN 'Cerrado'
END								"Estado Documento",
			
CASE T0.[CANCELED] 
WHEN 'N' THEN 'No' 
WHEN 'Y' THEN 'Si' 
WHEN 'C' THEN 'Cancelacion' 
END								"Cancelado",

'Factura'						"Clase de Documento",
					
CASE T0.[DocType] 
WHEN 'S' THEN 'Servicio' 
WHEN 'I' THEN 'Articulo' 
END								"Tipo de documento",					
						
T4.[USER_CODE]		"Usuario",	
T6.[SlpName]		"Agente",
T1.[Currency]		"Moneda",
T0.[Comments]		"Comentarios",	
T7.[FirmName]		"Marca",
T3.[ItmsGrpNam]		"Grupo Articulos",
T1.[ItemCode]		"Cod Articulo",
T2.[ItemName]		"Articulo",
T1.[Dscription]		"Articulo en Factura",	
T1.[Quantity]		"Cantidad",
T1.[TaxCode]		"Impuesto",

T1.[LineTotal]	 "Subtotal LOC",					
(T1.[LineTotal]* (COALESCE(T0.[DiscPrcnt],0)/100))	 "Descuento CAB LOC",					

(T1.[LineTotal] - --Subtotal
(T1.[LineTotal]* (COALESCE(T0.[DiscPrcnt],0)/100)))	 --Descuento CAB LOC
"Total Neto LOC",

(T1.[LineTotal] - --Subtotal
(T1.[LineTotal]* (COALESCE(T0.[DiscPrcnt],0)/100))) -	 --Descuento CAB LOC
T1.[GrssProfit]	 "Costo LOC",					

T1.[GrssProfit]	 "Utilidad LOC",
T1.[GrssProfit]*.1	 "Comision LOC",
T1.[VatSum]		 "Impuesto LOC",

T1.[TotalSumSy]	 "Subtotal USD",					
(T1.[TotalSumSy]* (COALESCE(T0.[DiscPrcnt],0)/100))	 "Descuento CAB USD",					

(T1.[TotalSumSy] - --Subtotal
(T1.[TotalSumSy]* (COALESCE(T0.[DiscPrcnt],0)/100)))	 --Descuento CAB USD
"Total Neto USD",

(T1.[TotalSumSy] - --Subtotal
(T1.[TotalSumSy]* (COALESCE(T0.[DiscPrcnt],0)/100))) -	 --Descuento CAB USD
T1.[GrssProfSC]	 "Costo USD",

T1.[GrssProfSC]	 "Utilidad USD",
T1.[GrssProfSC]*.1	 "Comision USD",					
T1.[VatSumSy]	 "Impuesto USD",

T1.[AcctCode]		"Cuenta ingresos",
--T10.[AcctName]		"Nombre cuenta ingresos",
T1.[OcrCode]		"CC Ingresos", 
T1.[CogsAcct]		"Cuenta costos",
--T11.[AcctName]		"Nombre cuenta costos",
T1.[CogsOcrCod]		"CC Costo"

FROM		OINV	T0				
LEFT JOIN	INV1	T1	ON	T0.[DocEntry]	=	T1.[DocEntry]
LEFT JOIN	OITM	T2	ON	T1.[ItemCode]	=	T2.[ItemCode]
LEFT JOIN	OITB	T3	ON	T2.[ItmsGrpCod]	=	T3.[ItmsGrpCod]
LEFT JOIN	OUSR	T4	ON	T0.[UserSign]	=	T4.[USERID]
LEFT JOIN	OWHS	T5	ON	T1.[WhsCode]	=	T5.[WhsCode]
LEFT JOIN	OSLP	T6	ON	T0.[SlpCode]	=	T6.[SlpCode]
LEFT JOIN	OMRC	T7	ON	T2.[FirmCode]	=	T7.[FirmCode]
LEFT JOIN	OCRD	T8	ON	T0.[CardCode]	=	T8.[CardCode]
LEFT JOIN	NNM1	T9	ON	T0.[Series]		=	T9.[Series]
--LEFT JOIN	OACT	T10	ON	T1.[AcctCode]	=	T10.[AcctCode]
--LEFT JOIN	OACT	T11	ON	T1.[CogsAcct]	=	T11.[AcctCode]
LEFT JOIN 
(
SELECT
T0.DocEntry "Llave",
T0.DocNum,
MAX(T3."ReconDate") AS "Fecha"

FROM OINV T0  
LEFT JOIN ITR1 T2 on T0."DocEntry"  = T2."SrcObjAbs" AND T0."ObjType"  = T2."SrcObjTyp"
INNER JOIN OITR T3 ON T2."ReconNum" = T3."ReconNum"

GROUP BY
T0.DocEntry,
T0.DocNum
) T11 ON T0.[DocEntry] = T11.[Llave]

UNION ALL

SELECT
T0.DocEntry,					
T0.[DocDate]					"Fecha",					
DATEPART(YEAR, T0.[DocDate])	"Año",					
DATEPART(MONTH, T0.[DocDate])	"Mes",					
DATEPART(DAY, T0.[DocDate])		"Dia",
DATEPART(MONTH, T11.[Fecha])	"Mes ult conciliacion",					
T0.[DocTime]					"Hora",					
T5.[WhsName]					"Almacen",					
T0.[CardCode]					"Cod Cliente",					
T8.[CardName]					"Cliente",					
T0.[CardName]					"Cliente en Factura",					

CASE 
WHEN T9.[SeriesName] IS NULL THEN 'Manual' 
ELSE T9.[SeriesName] 
END								"Serie Numeracion",
					
T0.[DocNum]						"# Doc",
					
CASE T0.[DocStatus]
WHEN 'O' THEN 'Abierto'
WHEN 'C' THEN 'Cerrado'
END								"Estado Documento",
			
CASE T0.[CANCELED] 
WHEN 'N' THEN 'No' 
WHEN 'Y' THEN 'Si' 
WHEN 'C' THEN 'Cancelacion' 
END								"Cancelado",

'NC'						"Clase de Documento",
					
CASE T0.[DocType] 
WHEN 'S' THEN 'Servicio' 
WHEN 'I' THEN 'Articulo' 
END								"Tipo de documento",					
						
T4.[USER_CODE]		"Usuario",	
T6.[SlpName]		"Agente",
T1.[Currency]		"Moneda",
T0.[Comments]		"Comentarios",	
T7.[FirmName]		"Marca",
T3.[ItmsGrpNam]		"Grupo Articulos",
T1.[ItemCode]		"Cod Articulo",
T2.[ItemName]		"Articulo",
T1.[Dscription]		"Articulo en Factura",	
T1.[Quantity]		"Cantidad",
T1.[TaxCode]		"Impuesto",

T1.[LineTotal]*-1	 "Subtotal LOC",					
(T1.[LineTotal]* (COALESCE(T0.[DiscPrcnt],0)/100))*-1	 "Descuento CAB LOC",					

(T1.[LineTotal] - --Subtotal
(T1.[LineTotal]* (COALESCE(T0.[DiscPrcnt],0)/100)))*-1	 --Descuento CAB LOC
"Total Neto LOC",

((T1.[LineTotal] - --Subtotal
(T1.[LineTotal]* (COALESCE(T0.[DiscPrcnt],0)/100))) -	 --Descuento CAB LOC
T1.[GrssProfit])*-1	 "Costo LOC",					

T1.[GrssProfit]*-1	 "Utilidad LOC",
T1.[GrssProfit]*-1*.1	 "Comision LOC",
T1.[VatSum]*-1		 "Impuesto LOC",

T1.[TotalSumSy]*-1	 "Subtotal USD",					
(T1.[TotalSumSy]* (COALESCE(T0.[DiscPrcnt],0)/100))*-1	 "Descuento CAB USD",					

(T1.[TotalSumSy] - --Subtotal
(T1.[TotalSumSy]* (COALESCE(T0.[DiscPrcnt],0)/100)))*-1	 --Descuento CAB USD
"Total Neto USD",

((T1.[TotalSumSy] - --Subtotal
(T1.[TotalSumSy]* (COALESCE(T0.[DiscPrcnt],0)/100))) -	 --Descuento CAB USD
T1.[GrssProfSC])*-1	 "Costo USD",

T1.[GrssProfSC]*-1	 "Utilidad USD",
T1.[GrssProfSC]*-1*.1	 "Comision USD",					
T1.[VatSumSy]*-1	 "Impuesto USD",

T1.[AcctCode]		"Cuenta ingresos",
--T10.[AcctName]		"Nombre cuenta ingresos",
T1.[OcrCode]		"CC Ingresos", 
T1.[CogsAcct]		"Cuenta costos",
--T11.[AcctName]		"Nombre cuenta costos",
T1.[CogsOcrCod]		"CC Costos"
						
FROM		ORIN	T0				
LEFT JOIN	RIN1	T1	ON	T0.[DocEntry]	=	T1.[DocEntry]
LEFT JOIN	OITM	T2	ON	T1.[ItemCode]	=	T2.[ItemCode]
LEFT JOIN	OITB	T3	ON	T2.[ItmsGrpCod]	=	T3.[ItmsGrpCod]
LEFT JOIN	OUSR	T4	ON	T0.[UserSign]	=	T4.[USERID]
LEFT JOIN	OWHS	T5	ON	T1.[WhsCode]	=	T5.[WhsCode]
LEFT JOIN	OSLP	T6	ON	T0.[SlpCode]	=	T6.[SlpCode]
LEFT JOIN	OMRC	T7	ON	T2.[FirmCode]	=	T7.[FirmCode]
LEFT JOIN	OCRD	T8	ON	T0.[CardCode]	=	T8.[CardCode]
LEFT JOIN	NNM1	T9	ON	T0.[Series]		=	T9.[Series]
--LEFT JOIN	OACT	T10	ON	T1.[AcctCode]	=	T10.[AcctCode]
--LEFT JOIN	OACT	T11	ON	T1.[CogsAcct]	=	T11.[AcctCode]
LEFT JOIN 
(
SELECT
T0.DocEntry "Llave",
T0.DocNum,
MAX(T3."ReconDate") AS "Fecha"

FROM ORIN T0  
LEFT JOIN ITR1 T2 on T0."DocEntry"  = T2."SrcObjAbs" AND T0."ObjType"  = T2."SrcObjTyp"
INNER JOIN OITR T3 ON T2."ReconNum" = T3."ReconNum"

GROUP BY
T0.DocEntry,
T0.DocNum
) T11 ON T0.[DocEntry] = T11.[Llave]


GO
/****** Object:  StoredProcedure [dbo].[CLVS_GET_BP_PURCHASEORDER]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_GET_BP_PURCHASEORDER]
@DocEntry varchar(20)
AS
BEGIN
SELECT T1.CardCode, T1.CardName From OPOR T1 WHERE T1.DocNum = @DocEntry
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_GET_PURCHASEORDER_BYCODE]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_GET_PURCHASEORDER_BYCODE]
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
	T0.Price,
	T0.DiscPrcnt,
	T0.Rate,
	T0.LineTotal
FROM POR1 T0 
	Inner JOIN  OITM item on item.ItemCode = T0.ItemCode
WHERE T0.DocEntry = @DocEntry;
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_GET_PURCHASEORDERLIST]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_GET_PURCHASEORDERLIST]
(
@CardCode varchar(20),
@FIni VARCHAR(20),
@FFin VARCHAR(20))
AS
BEGIN

SELECT 
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

--IF( @CardCode <> '')
--BEGIN

--SELECT
--T0.CardName,
--T0.DocNum,
--	--FORMAT(T0.DocDate,'dd-MM-yyyy') As DocDate,
--	T0.DocDate,
--	T0.DocTotal	
--FROM OPOR T0
--WHERE T0.DocStatus = 'O'
--AND T0.CardCode = @CardCode
--END
--ELSE
--BEGIN
--SELECT
--T0.CardName,
--T0.DocNum,
--	--T0.DocDate,'dd-MM-yyyy') As DocDate,
--	T0.DocDate,
--	T0.DocTotal	
--FROM OPOR T0
--WHERE T0.DocStatus = 'O'
--AND (T0.DocDate BETWEEN CONVERT(DATE, @FIni) AND CONVERT(DATE, @FFin))

END


		--@FIni = N'2020-10-19',
		--@FFin = N'2020-10-19'

GO
/****** Object:  StoredProcedure [dbo].[CLVS_POS_CANCELPAYMENT_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_POS_CANCELPAYMENT_SPR]
@CardCode varchar(20),
@FIni VARCHAR(20),
@FFin VARCHAR(20)
AS
BEGIN
SELECT 
 T2.DocNum ,
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
 
 from ORCT T0 INNER JOIN 
 (OINV T2 INNER JOIN RCT2 T1 ON T2.DocEntry = T1.DocEntry
 AND T2.CardCode = @CardCode 
 AND (T2.DocDate BETWEEN CONVERT(DATE, @FIni) AND CONVERT(DATE, @FFin)) )
 ON T0.DocEntry = T1.DocNum AND T0.Canceled ='N'
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_POS_CHECK_UNIQUEINVID]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CLVS_POS_CHECK_UNIQUEINVID]
@UniqueInvId VARCHAR(254)
AS
BEGIN
	SELECT U_CLVS_POS_UniqueInvId FROM OINV
	WHERE U_CLVS_POS_UniqueInvId = @UniqueInvId
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_POS_CHECK_UNIQUEINVID_SUPPLIER]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CLVS_POS_CHECK_UNIQUEINVID_SUPPLIER]
@UniqueInvId VARCHAR(254)
AS
BEGIN
	SELECT U_CLVS_POS_UniqueInvId FROM OPCH
	WHERE U_CLVS_POS_UniqueInvId = @UniqueInvId
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_POS_GET_GETCUSTOMER]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_POS_GET_GETCUSTOMER]	
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
/****** Object:  StoredProcedure [dbo].[CLVS_POS_GET_GETCUSTOMER_BYCODE]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[CLVS_POS_GET_GETCUSTOMER_BYCODE]
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
/****** Object:  StoredProcedure [dbo].[CLVS_POS_GETAVAILABLEWHITEM_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_POS_GETAVAILABLEWHITEM_SPR]
@ItemCode VARCHAR(100)
AS
BEGIN
SELECT ITEM.ItemCode, ITEM.ItemName, WHITEM.WhsCode, WHS.WhsName, WHITEM.OnHand - WHITEM.IsCommited as Available
FROM dbo.OITM AS ITEM
JOIN OITW AS WHITEM ON ITEM.ItemCode = WHITEM.ItemCode
JOIN OWHS AS WHS ON WHITEM.WhsCode = WHS.WhsCode
WHERE (frozenFor = 'N') AND ((WHITEM.OnHand - WHITEM.IsCommited)>0) AND (ITEM.ItemCode = @ItemCode)
ORDER BY ITEM.ItemCode, WHITEM.WhsCode
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_POS_GETAVAILABLEWHITEMS_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_POS_GETAVAILABLEWHITEMS_SPR] 
@ItemCode varchar(20)
AS

SET NOCOUNT ON
SET ANSI_WARNINGS OFF

BEGIN
	SELECT ItemsWH.WhsCode, WareHouse.WhsName, ItemsWH.ItemCode, ItemsWH.OnHand, 
			ItemsWH.IsCommited, ItemsWH.OnOrder, ItemsWH.AvgPrice, (ItemsWH.OnHand - ItemsWH.IsCommited) as Disponible, OITM.InvntItem			
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
/****** Object:  StoredProcedure [dbo].[CLVS_POS_GETBARCODESBYITEM_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CLVS_POS_GETBARCODESBYITEM_SPR]
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
/****** Object:  StoredProcedure [dbo].[CLVS_POS_GETCHANGEITEM_MD_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_POS_GETCHANGEITEM_MD_SPR]
@ItemCode varchar(100),
@Discount decimal(12,2),
@PriceList int
AS
BEGIN
-- cambiar el 0 por item.U_maxDiscount
SELECT item.ItemCode, item.ItemName,
(CASE 
WHEN U_IVA='100EXO'
THEN 'EXO'
ELSE U_IVA
END)
AS TaxCode, item.InvntItem, 
--(SELECT Rate FROM OSTA WHERE Code=TaxCode) AS TaxRate, 
(CASE 
WHEN U_IVA='100EXO'
THEN 0
WHEN U_IVA='1IVA'
THEN 1
WHEN U_IVA='2IVA'
THEN 2
WHEN U_IVA='4IVA'
THEN 4
WHEN U_IVA='13IVA'
THEN 13
END) AS TaxRate,
pricelist.Price AS UnitPrice,
(CASE WHEN 0 > 0
  THEN CASE WHEN @Discount >= 0
  THEN 0
  ELSE  @Discount END
  ELSE 0 END ) AS U_Discount,
(SELECT T1.[CardName] FROM OCRD T1 WHERE item.[CardCode] = T1.[CardCode]) AS PreferredVendor, item.OnHand as OnHand,
item.FrgnName as ForeingName
FROM OITM item
LEFT JOIN ITM1 pricelist
ON item.ItemCode = pricelist.ItemCode
--LEFT JOIN OTCX Taxes
--ON LTRIM(RTRIM(Taxes.StrVal1)) = CAST(item.ItmsGrpCod AS VARCHAR(255))  ITMR
JOIN OBCD OB ON OB.ItemCode = ITEM.ItemCode
WHERE item."ItemCode" = @ItemCode and pricelist.PriceList = @PriceList
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_POS_GETDOCNUM_OQUT_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CLVS_POS_GETDOCNUM_OQUT_SPR]
@DocEntry int
AS
BEGIN
	SELECT oqut.DocNum 		
	FROM OQUT oqut	
	WHERE DocEntry = @DocEntry
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_POS_GETDOCNUM_ORDR_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CLVS_POS_GETDOCNUM_ORDR_SPR]
@DocEntry int
AS
BEGIN
	SELECT ordr.DocNum 		
	FROM ORDR ordr	
	WHERE DocEntry = @DocEntry
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_POS_GETINVENTORYREPORT_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_POS_GETINVENTORYREPORT_SPR]
@Articulo varchar(20),
@Marca varchar(20),
@Grupo varchar(30),
@subGrupo varchar (30) 
AS
BEGIN
select Item.ItemCode, Item.ItemName, firm.FirmName as Marca, grupo.ItmsGrpNam AS ItemGroup, precio.Price, sData.WhsCode,  wrhouse.WhsName as Almasen,
   serial.InDate as UltimoIngreso, sData.OnHand
from OITM Item
left join OITB grupo on Item.ItmsGrpCod = grupo.ItmsGrpCod
left join ITM1 precio on Item."ItemCode" = precio."ItemCode"
left join OITW sData on Item."ItemCode" = sData."ItemCode"
left join OSRN serial on Item."ItemCode" = serial.ItemCode
left join OWHS wrhouse on sData.WhsCode = wrhouse.WhsCode
left join OMRC firm on Item.FirmCode = firm.FirmCode 
where precio.PriceList = 1 AND sData.OnHand > 0
AND (@Articulo = '' OR (Item.ItemCode = @Articulo AND Item.ItemCode NOT LIKE 'AF0%' )) 
AND (@Grupo = '' OR Item.ItmsGrpCod = @Grupo) 
AND (@Marca = '' OR Item.FirmCode = @Marca)
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_POS_GETINVOICESAPTOPRINT_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_POS_GETINVOICESAPTOPRINT_SPR]
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
					LEFT JOIN [@NCLAVEFE] Clave ON Header.DocEntry = Clave."Code" and Clave."U_TipoDoc" = 'FE'
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
					LEFT JOIN [@NCLAVEFE] Clave ON Header.DocEntry = Clave."Code" and Clave."U_TipoDoc" = 'FE'
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
				   LEFT JOIN [@NCLAVEFE] Clave ON Header.DocEntry = Clave."Code" and Clave."U_TipoDoc" = 'FE'
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
				   LEFT JOIN [@NCLAVEFE] Clave ON Header.DocEntry = Clave."Code" and Clave."U_TipoDoc" = 'FE'
			WHERE Header.DocEntry = @DocEntry
		END;
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_POS_GETINVPRINTLIST_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_POS_GETINVPRINTLIST_SPR]
@slpCode varchar(10),
@DocEntry varchar(25),
@FechaIni varchar(10),
@FechaFin varchar(10),
@type int
AS
BEGIN
	If @type = 1
		select 
			fact.DocEntry, 
			fact.DocDate, 
			fact.CardName, 
			fact.DocStatus 
		from 
			ORDR fact
		where
			fact.SlpCode = @slpCode 
			AND (convert(date ,convert(varchar, fact.DocDate, 111)) BETWEEN convert (date,convert(varchar, @FechaIni, 111)) 
			AND convert (date,convert(varchar, @FechaFin, 111)))
			AND fact.DocEntry like @DocEntry+'%'
	ELSE If @type = 2
		select 
			fact.DocEntry, 
			fact.DocDate, 
			fact.CardName, 
			fact.DocStatus 
		from 
			OQUT fact
		where 
			fact.SlpCode = @slpCode 
			AND (convert(date ,convert(varchar, fact.DocDate, 111)) BETWEEN convert (date,convert(varchar, @FechaIni, 111)) 
			AND convert (date,convert(varchar, @FechaFin, 111)))
			AND fact.DocEntry like @DocEntry+'%'
	ELSE IF @type = 5
		select 
			fact.DocEntry,
			Clave.U_NConsecFE AS U_NumFE,
			fact.DocDate, 
			fact.CardName, 
			fact.DocStatus 
		from 
			OINV  fact
			JOIN [@NCLAVEFE] Clave ON fact.DocEntry = Clave."Code" and Clave."U_TipoDoc" = 'FE'
		where 
			fact.SlpCode = @slpCode 
			AND (convert(date ,convert(varchar, fact.DocDate, 111)) BETWEEN convert (date,convert(varchar, @FechaIni, 111)) 
			AND convert (date,convert(varchar, @FechaFin, 111)))
			AND fact.DocEntry like @DocEntry +'%'
END

GO
/****** Object:  StoredProcedure [dbo].[CLVS_POS_GETITEM_BY_CODEBAR_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_POS_GETITEM_BY_CODEBAR_SPR]
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
/****** Object:  StoredProcedure [dbo].[CLVS_POS_GETITEM_MD_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_POS_GETITEM_MD_SPR]
@ItemCode varchar(100),
@Discount decimal(12,2),
@PriceList int
AS
BEGIN
-- cambiar el 0 por item.U_maxDiscount
SELECT item.ItemCode, item.ItemName,
(CASE 
WHEN U_IVA='100EXO'
THEN 'EXO'
ELSE U_IVA
END)
AS TaxCode, item.InvntItem, 
--(SELECT Rate FROM OSTA WHERE Code=TaxCode) AS TaxRate, 
(CASE 
WHEN U_IVA='100EXO'
THEN 0
WHEN U_IVA='1IVA'
THEN 1
WHEN U_IVA='2IVA'
THEN 2
WHEN U_IVA='4IVA'
THEN 4
WHEN U_IVA='13IVA'
THEN 13
END) AS TaxRate,
pricelist.Price AS UnitPrice,
(CASE WHEN 0 > 0
  THEN CASE WHEN @Discount >= 0
  THEN 0
  ELSE  @Discount END
  ELSE 0 END ) AS U_Discount,
(SELECT T1.[CardName] FROM OCRD T1 WHERE item.[CardCode] = T1.[CardCode]) AS PreferredVendor, item.OnHand as OnHand,
item.FrgnName as ForeingName
, LastPurCur, LastPurDat, LastPurPrc -- SE AGREGA PARA OBTENER EL ULTIMO PRECIO DE VENTA
FROM OITM item
LEFT JOIN ITM1 pricelist
ON item.ItemCode = pricelist.ItemCode
--LEFT JOIN OTCX Taxes
--ON LTRIM(RTRIM(Taxes.StrVal1)) = CAST(item.ItmsGrpCod AS VARCHAR(255))  ITMR
JOIN OBCD OB ON OB.ItemCode = ITEM.ItemCode
WHERE item."ItemCode" = @ItemCode and pricelist.PriceList = @PriceList
END

GO
/****** Object:  StoredProcedure [dbo].[CLVS_POS_GETITEMLIST_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_POS_GETITEMLIST_SPR]
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
/****** Object:  StoredProcedure [dbo].[CLVS_POS_GETITEMS_WITH_DETAIL_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_POS_GETITEMS_WITH_DETAIL_SPR]
AS
BEGIN
SELECT 
	IT.ItemCode, 
	IT.ItemName, 
	(
		SELECT TOP 1  OB.BcdCode FROM OBCD OB
		WHERE OB.ItemCode = IT.ItemCode
	) AS BarCode,
	IT.U_IVA,
	IM.PriceList,
	IM.Price
FROM OITM IT
JOIN
	ITM1 IM ON IM.ItemCode = IT.ItemCode
JOIN OPLN OP ON OP.ListNum = IM.PriceList

WHERE OP.ListNum = 1--LA DEJO POR DEFECTO PARA PODER AVANZAR CON LA PARTE DE INGRESO DE MERCANCIA BY AGUILA+DE+CAMPO
--select * from OPLN
--select * from ITM1
--SELECT TOP 1  * FROM OBCD OB
--WHERE OB.ItemCode = '100002' --100001 -- 7441001000874
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_POS_GETNUMAPINVOICE_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CLVS_POS_GETNUMAPINVOICE_SPR]
@DocEntry int
AS
BEGIN
	SELECT apinv.DocNum		
	FROM OPCH apinv	
	WHERE DocEntry = @DocEntry
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_POS_GETNUMFEONLINE_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CLVS_POS_GETNUMFEONLINE_SPR]
@DocEntry int
AS
BEGIN
	SELECT oinv.DocNum, fe.U_NConsecFE AS NumFE, fe.U_NClaveFE AS ClaveFE 		
	FROM OINV oinv
	JOIN "@NCLAVEFE" AS fe ON @DocEntry = fe.Code
	WHERE DocEntry = @DocEntry
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_POS_GETPAYDOCUMENTS_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_POS_GETPAYDOCUMENTS_SPR]

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
/****** Object:  StoredProcedure [dbo].[CLVS_POS_GETPAYDOCUMENTS_SUPPLIER_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_POS_GETPAYDOCUMENTS_SUPPLIER_SPR]
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
/****** Object:  StoredProcedure [dbo].[CLVS_POS_GETQUOTATIONSAPTOPRINT_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_POS_GETQUOTATIONSAPTOPRINT_SPR]
@DocEntry int
AS
BEGIN
IF((SELECT T0.DocCur FROM OINV T0 WHERE T0.DocEntry = @DocEntry) ='COL')
IF((SELECT DISTINCT T0.Currency FROM RDR1 T0 WHERE T0.DocEntry = @DocEntry)= 'COL')
BEGIN
SELECT Header.DocEntry, Header.DocNum, bp.U_direccion,  Header.CardCode, Header.CardName, Header.DocCur, Header.DocRate, Header.DocTotal AS TOTAL, Header.DocDate, Header.Comments,
Header.DocStatus, Header.NumAtCard, (select SUM(((line1.VatPrcnt / 100) * (line1.Price * line1.Quantity))) from RDR1 line1 where line1.DocEntry = Header.DocEntry) AS Taxes,
(select SUM(((line1.DiscPrcnt / 100) * (line1.Price * line1.Quantity))) from RDR1 line1 where line1.DocEntry = Header.DocEntry) AS Discount, Lines.ItemCode, Lines.Dscription, Lines.Price, Lines.Quantity, Lines.TaxCode, Lines.DiscPrcnt, Lines.LineTotal AS LINETOTAL  
FROM OQUT Header 
JOIN QUT1 Lines ON Header.DocEntry = Lines.DocEntry
JOIN OCRD bp ON bp.CardCode = Header.CardCode
WHERE Header.DocEntry=@DocEntry
END;
ELSE
BEGIN
SELECT Header.DocEntry, Header.DocNum, bp.U_direccion, Header.CardCode, Header.CardName, Header.DocCur, Header.DocRate, Header.DocTotal AS TOTAL, Header.DocDate,Header.Comments,
Header.DocStatus, Header.NumAtCard, (select SUM(((line1.VatPrcnt / 100) * (line1.Price * line1.Quantity))) from RDR1 line1 where line1.DocEntry = Header.DocEntry) AS Taxes,
(select SUM(((line1.DiscPrcnt / 100) * (line1.Price * line1.Quantity))) from RDR1 line1 where line1.DocEntry = Header.DocEntry) AS Discount ,  Lines.ItemCode, Lines.Dscription, Lines.Price, Lines.Quantity, Lines.TaxCode, Lines.DiscPrcnt, Lines.TotalSumSy AS LINETOTAL  
FROM OQUT Header 
JOIN QUT1 Lines ON Header.DocEntry = Lines.DocEntry
JOIN OCRD bp ON bp.CardCode = Header.CardCode
WHERE Header.DocEntry=@DocEntry

END;
ELSE
IF((SELECT DISTINCT T0.Currency FROM RDR1 T0 WHERE T0.DocEntry = @DocEntry)= 'COL')
BEGIN
SELECT Header.DocEntry, Header.DocNum, bp.U_direccion, Header.CardCode, Header.CardName, Header.DocCur, Header.DocRate, Header.DocTotalFC AS TOTAL, Header.DocDate,Header.Comments,
Header.DocStatus, Header.NumAtCard, (select SUM(((line1.VatPrcnt / 100) * (line1.Price * line1.Quantity))) from RDR1 line1 where line1.DocEntry = Header.DocEntry) AS Taxes,
(select SUM(((line1.DiscPrcnt / 100) * (line1.Price * line1.Quantity))) from RDR1 line1 where line1.DocEntry = Header.DocEntry) AS Discount , Lines.ItemCode, Lines.Dscription, Lines.Price, Lines.Quantity, Lines.TaxCode, Lines.DiscPrcnt, Lines.LineTotal AS LINETOTAL  
FROM OQUT Header 
JOIN QUT1 Lines ON Header.DocEntry = Lines.DocEntry
JOIN OCRD bp ON bp.CardCode = Header.CardCode
WHERE Header.DocEntry=@DocEntry 
END;
ELSE
BEGIN
SELECT Header.DocEntry, Header.DocNum, bp.U_direccion, Header.CardCode, Header.CardName, Header.DocCur, Header.DocRate, Header.DocTotalFC AS TOTAL, Header.DocDate,Header.Comments,
Header.DocStatus, Header.NumAtCard, (select SUM(((line1.VatPrcnt / 100) * (line1.Price * line1.Quantity))) from RDR1 line1 where line1.DocEntry = Header.DocEntry) AS Taxes,
(select SUM(((line1.DiscPrcnt / 100) * (line1.Price * line1.Quantity))) from RDR1 line1 where line1.DocEntry = Header.DocEntry) AS Discount ,  Lines.ItemCode, Lines.Dscription, Lines.Price, Lines.Quantity, Lines.TaxCode, Lines.DiscPrcnt, Lines.TotalSumSy AS LINETOTAL  
FROM OQUT Header 
JOIN QUT1 Lines ON Header.DocEntry = Lines.DocEntry
JOIN OCRD bp ON bp.CardCode = Header.CardCode
WHERE Header.DocEntry=@DocEntry  
END;
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_POS_GETSOSAPTOPRINT_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create PROCEDURE [dbo].[CLVS_POS_GETSOSAPTOPRINT_SPR]
@DocEntry int
AS
BEGIN
IF((SELECT T0.DocCur FROM OINV T0 WHERE T0.DocEntry = @DocEntry) ='COL')
IF((SELECT DISTINCT T0.Currency FROM RDR1 T0 WHERE T0.DocEntry = @DocEntry)= 'COL')
BEGIN
SELECT Header.DocEntry, Header.DocNum, bp.U_direccion,  Header.CardCode, Header.CardName, Header.DocCur, Header.DocRate, Header.DocTotal AS TOTAL, Header.DocDate, Header.Comments,
Header.DocStatus, Header.NumAtCard, (select SUM(((line1.VatPrcnt / 100) * (line1.Price * line1.Quantity))) from RDR1 line1 where line1.DocEntry = Header.DocEntry) AS Taxes,
(select SUM(((line1.DiscPrcnt / 100) * (line1.Price * line1.Quantity))) from RDR1 line1 where line1.DocEntry = Header.DocEntry) AS Discount, Lines.ItemCode, Lines.Dscription, Lines.Price, Lines.Quantity, Lines.TaxCode, Lines.DiscPrcnt, Lines.LineTotal AS LINETOTAL  
FROM ORDR Header 
JOIN RDR1 Lines ON Header.DocEntry = Lines.DocEntry
JOIN OCRD bp ON bp.CardCode = Header.CardCode
WHERE Header.DocEntry=@DocEntry
END;
ELSE
BEGIN
SELECT Header.DocEntry, Header.DocNum, bp.U_direccion, Header.CardCode, Header.CardName, Header.DocCur, Header.DocRate, Header.DocTotal AS TOTAL, Header.DocDate,Header.Comments,
Header.DocStatus, Header.NumAtCard, (select SUM(((line1.VatPrcnt / 100) * (line1.Price * line1.Quantity))) from RDR1 line1 where line1.DocEntry = Header.DocEntry) AS Taxes,
(select SUM(((line1.DiscPrcnt / 100) * (line1.Price * line1.Quantity))) from RDR1 line1 where line1.DocEntry = Header.DocEntry) AS Discount ,  Lines.ItemCode, Lines.Dscription, Lines.Price, Lines.Quantity, Lines.TaxCode, Lines.DiscPrcnt, Lines.TotalSumSy AS LINETOTAL  
FROM ORDR Header 
JOIN RDR1 Lines ON Header.DocEntry = Lines.DocEntry
JOIN OCRD bp ON bp.CardCode = Header.CardCode
WHERE Header.DocEntry=@DocEntry

END;
ELSE
IF((SELECT DISTINCT T0.Currency FROM RDR1 T0 WHERE T0.DocEntry = @DocEntry)= 'COL')
BEGIN
SELECT Header.DocEntry, Header.DocNum, bp.U_direccion, Header.CardCode, Header.CardName, Header.DocCur, Header.DocRate, Header.DocTotalFC AS TOTAL, Header.DocDate,Header.Comments,
Header.DocStatus, Header.NumAtCard, (select SUM(((line1.VatPrcnt / 100) * (line1.Price * line1.Quantity))) from RDR1 line1 where line1.DocEntry = Header.DocEntry) AS Taxes,
(select SUM(((line1.DiscPrcnt / 100) * (line1.Price * line1.Quantity))) from RDR1 line1 where line1.DocEntry = Header.DocEntry) AS Discount , Lines.ItemCode, Lines.Dscription, Lines.Price, Lines.Quantity, Lines.TaxCode, Lines.DiscPrcnt, Lines.LineTotal AS LINETOTAL  
FROM ORDR Header 
JOIN RDR1 Lines ON Header.DocEntry = Lines.DocEntry
JOIN OCRD bp ON bp.CardCode = Header.CardCode
WHERE Header.DocEntry=@DocEntry 
END;
ELSE
BEGIN
SELECT Header.DocEntry, Header.DocNum, bp.U_direccion, Header.CardCode, Header.CardName, Header.DocCur, Header.DocRate, Header.DocTotalFC AS TOTAL, Header.DocDate,Header.Comments,
Header.DocStatus, Header.NumAtCard, (select SUM(((line1.VatPrcnt / 100) * (line1.Price * line1.Quantity))) from RDR1 line1 where line1.DocEntry = Header.DocEntry) AS Taxes,
(select SUM(((line1.DiscPrcnt / 100) * (line1.Price * line1.Quantity))) from RDR1 line1 where line1.DocEntry = Header.DocEntry) AS Discount ,  Lines.ItemCode, Lines.Dscription, Lines.Price, Lines.Quantity, Lines.TaxCode, Lines.DiscPrcnt, Lines.TotalSumSy AS LINETOTAL  
FROM ORDR Header 
JOIN RDR1 Lines ON Header.DocEntry = Lines.DocEntry
JOIN OCRD bp ON bp.CardCode = Header.CardCode
WHERE Header.DocEntry=@DocEntry  
END;
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_POS_GETUSRBALANCE_CREDITNOTES_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--SELECT * FROM ORIN
--where DocDate > '2020-08-18'
--order by DocEntry

CREATE PROCEDURE [dbo].[CLVS_POS_GETUSRBALANCE_CREDITNOTES_SPR]
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
/****** Object:  StoredProcedure [dbo].[CLVS_POS_GETUSRBALANCE_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Alan Jose Arias Herrera>
-- Create date: <2019/08/26>
-- Description:	<SP para el cierre de caja>
-- =============================================
CREATE PROCEDURE [dbo].[CLVS_POS_GETUSRBALANCE_SPR]
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
/****** Object:  StoredProcedure [dbo].[CLVS_POS_GETUSRBALANCECOL_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Alan Jose Arias Herrera>
-- Create date: <2019/08/26>
-- Description:	<SP para el cierre de caja>
-- =============================================
CREATE PROCEDURE [dbo].[CLVS_POS_GETUSRBALANCECOL_SPR]
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
		Fact."DocCur" AS DocCurrInv, 
		Fact."DocRate" AS DocRateInv
	FROM ORCT Pay
	LEFT JOIN RCT2 Lines
	ON Pay."DocEntry"=Lines."DocNum"
	LEFT JOIN OINV Fact
	ON Lines."DocEntry"=Fact."DocEntry"
	LEFT JOIN INV1 FLines
	ON Fact."DocEntry"=FLines."DocEntry"
	WHERE
	Pay."DocCurr" = 'COL' AND
	Fact."Canceled"='N' 
	 AND (DATEADD(SECOND, 
		(CASE LEN(Pay."DocTime") 
		WHEN '4' THEN ((LEFT(Pay."DocTime",	2) * 3600) + (RIGHT(Pay."DocTime",2) * 60))
		WHEN '3' THEN ((LEFT(Pay."DocTime",	1) * 3600) + (RIGHT(Pay."DocTime",2) * 60))
		WHEN '2' THEN (RIGHT(Pay."DocTime",2) * 60)
		WHEN '1' THEN (RIGHT(Pay."DocTime",1) * 60) 
		ELSE '0' END), Pay."DocDate") BETWEEN @FIni AND @FFin)
   AND Fact."SlpCode"= CAST(@SlpCode AS CHAR)
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
			Fact."DocCur" AS DocCurrInv, 
			Fact."DocRate" AS DocRateInv
	FROM OINV Fact
	LEFT JOIN OUSR users 
	ON users."INTERNAL_K" = Fact."UserSign"
	WHERE
	Fact."DocCur" = 'COL' AND
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
	)
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_POS_GETUSRBALANCEUSD_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Alan Jose Arias Herrera>
-- Create date: <2019/08/26>
-- Description:	<SP para el cierre de caja>
-- =============================================
CREATE PROCEDURE [dbo].[CLVS_POS_GETUSRBALANCEUSD_SPR]
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
		Fact."DocCur" AS DocCurrInv, 
		Fact."DocRate" AS DocRateInv
	FROM ORCT Pay
	LEFT JOIN RCT2 Lines
	ON Pay."DocEntry"=Lines."DocNum"
	LEFT JOIN OINV Fact
	ON Lines."DocEntry"=Fact."DocEntry"
	LEFT JOIN INV1 FLines
	ON Fact."DocEntry"=FLines."DocEntry"
	WHERE
	Pay."DocCurr" = 'USD' AND
	Fact."Canceled"='N' 
	 AND (DATEADD(SECOND, 
		(CASE LEN(Pay."DocTime") 
		WHEN '4' THEN ((LEFT(Pay."DocTime",	2) * 3600) + (RIGHT(Pay."DocTime",2) * 60))
		WHEN '3' THEN ((LEFT(Pay."DocTime",	1) * 3600) + (RIGHT(Pay."DocTime",2) * 60))
		WHEN '2' THEN (RIGHT(Pay."DocTime",2) * 60)
		WHEN '1' THEN (RIGHT(Pay."DocTime",1) * 60) 
		ELSE '0' END), Pay."DocDate") BETWEEN @FIni AND @FFin)
   AND Fact."SlpCode"= CAST(@SlpCode AS CHAR)
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
			Fact."DocCur" AS DocCurrInv, 
			Fact."DocRate" AS DocRateInv
	FROM OINV Fact
	LEFT JOIN OUSR users 
	ON users."INTERNAL_K" = Fact."UserSign"
	WHERE
	Fact."DocCur" = 'USD' AND
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
	)
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_POS_GETWHAVAILABLEITEM_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_POS_GETWHAVAILABLEITEM_SPR]
@ItemCode VARCHAR(100)
AS
BEGIN
SELECT ITEM.ItemCode, ITEM.ItemName, WHITEM.WhsCode, WHS.WhsName, WHITEM.OnHand - WHITEM.IsCommited as Disponible,  WHITEM.OnHand as OnHand, WHITEM.IsCommited as IsCommited,
	   WHITEM.OnOrder as OnOrder, ITEM.AvgPrice as AvgPrice
FROM dbo.OITM AS ITEM
JOIN OITW AS WHITEM ON ITEM.ItemCode = WHITEM.ItemCode
JOIN OWHS AS WHS ON WHITEM.WhsCode = WHS.WhsCode
WHERE (frozenFor = 'N') AND ((WHITEM.OnHand - WHITEM.IsCommited)>0) AND (ITEM.ItemCode = @ItemCode) 
ORDER BY ITEM.ItemCode, WHITEM.WhsCode
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_POS_GRAPHGETSUMCOMPANY_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CLVS_POS_GRAPHGETSUMCOMPANY_SPR]
as begin
select top 10 sum(doctotal), CardName from OINV group by CardName order by 1 desc
end
GO
/****** Object:  StoredProcedure [dbo].[CLVS_POS_GRAPHGETSUMDAY_SPR]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_POS_GRAPHGETSUMDAY_SPR]
as begin
select top 30 sum(doctotal) as total, Year(DocDate) as d_year, Month(DocDate) as d_month, Day(DocDate) as d_day from OINV group by Year(DocDate), Month(DocDate), Day(DocDate);
end
GO
/****** Object:  StoredProcedure [dbo].[CLVS_POS_RECEVIEDPAID]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_POS_RECEVIEDPAID]
@DocEntry INT
AS
BEGIN
	SELECT 
		DocEntry, 
		DocNum, 
		CardCode, 
		CardName, 
		DocTotal, 
		PaidToDate 
	FROM OINV 
	WHERE 
		DocEntry = @DocEntry
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_POS_RECEVIEDPAID_DETAIL]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_POS_RECEVIEDPAID_DETAIL]
@DocEntry INT
AS
BEGIN
	SELECT 
		RC.DocEntry, 
		RC.SumApplied,
		OI.DocTotal,
		OI.PaidToDate,
		(OI.DocTotal - OI.PaidToDate) Balance
	 FROM RCT2 RC
	 JOIN 
		OINV OI ON OI.DocEntry = RC.DocEntry
	 WHERE 
		RC.DocNum = @DocEntry
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_REG_VISTAS]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLVS_REG_VISTAS]
AS
BEGIN
	DECLARE 
		@view VARCHAR(MAX), 
		@db   VARCHAR(MAX),
		@count int;

	DECLARE cursor_view CURSOR
	FOR SELECT 
		v.name "VIEW", DB_NAME() "DB"
		FROM sys.views as v
		WHERE OBJECT_SCHEMA_NAME(v.object_id) = 'dbo'
		and v.name like '%CLVS%';

	OPEN cursor_view;

	FETCH NEXT FROM cursor_view INTO 
		@view, 
		@db;

	WHILE @@FETCH_STATUS = 0
		BEGIN
			EXEC ('INSERT INTO [CLVS_QRY_ADMIN].[dbo].[REG_VISTAS] Select '''+@db+''' [Base de Datos],'''+@view+''' [Vista], GETDATE() [Fecha], count(*) [Count] from '+@db+'.dbo.'+@view+';')
			--PRINT @db + ' ' +@view;
			FETCH NEXT FROM cursor_view INTO 
				@view, 
				@db;
		END;

	CLOSE cursor_view;

	DEALLOCATE cursor_view;
END
GO
/****** Object:  StoredProcedure [dbo].[CLVS_SUPMD_SELECTITEM]    Script Date: 23/10/2020 10:13:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CLVS_SUPMD_SELECTITEM]
@ITEMCODE NVARCHAR(30),
@CODEBAR NVARCHAR(30)
AS
BEGIN
	SELECT TOP 1
		T2.BcdCode AS CodeBars,
		T0.ItemCode AS Code,
		T0.ItemName AS ItemName,
		T0.FrgnName AS ForeignName,
		T1.Price AS Price,
		T0.OnHand AS Quantity,
		(CASE
			WHEN T0.U_IVA IS NULL THEN 'EXE'
			WHEN T0.U_IVA = '' THEN 'EXE'
			ELSE T0.U_IVA
		END) AS TaxCode, 
		(CASE
			WHEN T0.U_IVA IS NULL THEN 0
			WHEN T0.U_IVA = '' THEN 0
			WHEN T0.U_IVA='100EXE' THEN 0
			WHEN T0.U_IVA='EXE' THEN 0
			WHEN T0.U_IVA='1IVA' THEN 1
			WHEN T0.U_IVA='2IVA' THEN 2
			WHEN T0.U_IVA='4IVA' THEN 4
			WHEN T0.U_IVA='13IVA' THEN 13
		END) AS TaxRate
	FROM 
		OITM T0 JOIN ITM1 T1 ON T0.ItemCode = T1.ItemCode AND T1.PriceList = 1
			JOIN OBCD T2 ON T2.ItemCode = T0.ItemCode
	WHERE
		T2.BcdCode = (CASE @CODEBAR
							WHEN '' THEN T2.BcdCode
							WHEN NULL THEN T2.BcdCode
							ELSE @CODEBAR
						END) 
END;
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CLVS_POS_GETAllPRICELIST_SPR'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CLVS_POS_GETAllPRICELIST_SPR'
GO

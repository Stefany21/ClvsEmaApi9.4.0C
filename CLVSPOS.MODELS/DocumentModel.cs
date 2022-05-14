using CLVSSUPER.MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace CLVSPOS.MODELS
{

    /// <summary>
    /// Es el modelo base de los documentos
    /// </summary>
    public class DocumentModel
    {

        #region Modelo original
        ////this system internal invoice number
        //public int DocEntry { get; set; }
        ////this is the invoice number for the final user
        //public int DocNum { get; set; }
        //public int? BaseEntry { get; set; }
        ////this is the code of the customer
        //public string CardCode { get; set; }
        ////this is the name of the customer
        //public string CardName { get; set; }
        ////They type of document, 13 is the DocType for ARInvoice, 14 for A/R Credit Memo, 
        //public int DocType { get; set; }
        ////The date for the invoice with format 'YYYYMMDD'
        //public DateTime DocDate { get; set; }
        ////This is the currency of the document
        //public string Currency { get; set; }
        ////Estado del documento ( ... )
        //public int Status { get; set; }
        ////Registra la accion ocurrida con el documento al crerlo en SAP
        //public string StatusDetails { get; set; }
        ////Registra la compannia en la que se registro el documento
        //public string DBCode { get; set; }
        //public string NumAtCard { get; set; }
        //// terminos de pagos
        //public string PayTerms { get; set; }
        //// Comentarios
        //public string Comment { get; set; }
        ////codigo de vendedor
        //public int SlpCode { get; set; }
        //// tipo de documento
        //public string DocumentType { get; set; }
        ////campos para facturacion electronica
        //public string IdType { get; set; }        
        //public string Identification { get; set; }
        //public string Email { get; set; }
        //public string U_ObservacionFE { get; set; }
        //public string Provincia { get; set; }
        //public string Canton { get; set; }
        //public string Distritos { get; set; }
        //public string Direccion { get; set; }
        //public string U_Online { get; set; }
        //public List<UdfTarget> UdfTarget { get; set; }
        #endregion


        //this system internal invoice number
        public int DocEntry { get; set; }// No se usa en facturacion 
        public int DocNum { get; set; }
        //this is the invoice number for the final user
        public int? BaseEntry { get; set; }
        //this is the code of the customer
        public string CardCode { get; set; }
        //this is the name of the customer
        public string CardName { get; set; }
        //They type of document, 13 is the DocType for ARInvoice, 14 for A/R Credit Memo, 
        public string DocType { get; set; }
        //The date for the invoice with format 'YYYYMMDD'
        public DateTime DocDate { get; set; }
        public string DocDueDate { get; set; }

        //This is the currency of the document
        public string DocCurrency { get; set; }//Currency

        public string NumAtCard { get; set; } // No se utiliza en facturacion
        // terminos de pagos
        public string PaymentGroupCode { get; set; } //Payterms
        // Comentarios
        public string Comments { get; set; }
        //codigo de vendedor
        public int SalesPersonCode { get; set; } //SlpCode
        // tipo de documento
        public string U_TipoDocE { get; set; }//DocumentType
        //campos para facturacion electronica
        public string U_TipoIdentificacion { get; set; } // IdType
        public string U_NumIdentFE { get; set; }//Identification
        public string U_CorreoFE { get; set; }// Email
        public string U_ObservacionFE { get; set; }
        public string U_Online { get; set; }
        public int Series { get; set; }
        public string U_CLVS_POS_UniqueInvId { get; set; }
        public List<DocumentLines> DocumentLines { get; set; }

        // UdfsDinamicos
        public string U_Provincia { get; set; }

        public string U_Canton { get; set; }

        public string U_Distrito { get; set; }

        public string U_Barrio { get; set; }

        public string U_Direccion { get; set; }

        public string U_ClaveFE { get; set; }

        public string U_NumFE { get; set; }


        [ScriptIgnore]
        public List<UdfTarget> UdfTarget { get; set; }
        public int U_ListNum { get; set; }





    }

    /// <summary>
    /// Es el modelo base de las lineas de documentos
    /// </summary>
    public class LinesBaseModel
    {
        public int? LineNum { get; set; }
        //codigo del articulo en SAP
        public string ItemCode { get; set; }
        //precio del articulo
        public double UnitPrice { get; set; }
        //cantidad del articulo que se esta vendiendo
        public double Quantity { get; set; }
        //codigo de impuestos
        public string TaxCode { get; set; }
        //ratio de impuestos
        public double TaxRate { get; set; }
        //codigo de almacen
        public string WarehouseCode { get; set; } //WhCode
        //cantidad de descuento del articulo
        public double DiscountPercent { get; set; }//Discount
        //serie del articulo
        public string Serie { get; set; }
        public string TaxOnly { get; set; }

        public int? BaseType { get; set; }
        public int? BaseEntry { get; set; }
        public int? BaseLine { get; set; }

    }

    /// <summary>
    /// Es el modelo base de la factura
    /// </summary>
    public class InvoiceModelBase : DocumentModel
    {

    }

    /// <summary>
    /// Es el modelo para las lineas de factura
    /// </summary>
    public class InvoiceLinesModelBase : LinesBaseModel
    {
        public string ItemName { get; set; }
        public string WhsName { get; set; }
        //public int? BaseType { get; set; }
        //public int? BaseLine { get; set; }
        //public int? BaseEntry { get; set; }
        public int LineNum { get; set; }
    }

    /// <summary>
    /// Es el modelo base para cotizaciones
    /// </summary>
    public class QuotationsModelBase : DocumentModel
    {

    }

    public class PPTransactionsCanceledPrintSearch
    {

        public string UserPrefix { set; get; }
        public string FechaIni { set; get; }
        public string FechaFin { set; get; }

    }

    /// <summary>
    /// Es el modelo base para la linea de cotizacion
    /// </summary>
    public class QuotationsLinesModelBase : LinesBaseModel
    {
        public string ItemName { get; set; }
        public string WhsName { get; set; }
        public int? BaseType { get; set; }
        public int? BaseLine { get; set; }
        public int? BaseEntry { get; set; }
        public int LineNum { get; set; }
    }

    /// <summary>
    /// Es el modelo para las ordenes de venta
    /// </summary>
    public class SalesOrderModel : InvoiceModelBase
    {
        public string DocStatus { get; set; }
        public string UserId { get; set; }
        [ScriptIgnore]
        public decimal DocTotal { get; set; }
        [ScriptIgnore]
        public decimal DocTotalFC { get; set; }
        public string SalesMan { get; set; }
        public List<InvoiceLinesModelBase> SaleOrderLinesList { get; set; }
        public List<QuotationsLinesModelBase> BaseLines { get; set; }
    }

    /// <summary>
    /// Es el modelo de las cotizaciones
    /// </summary>
    public class QuotationsModel : DocumentModel
    {
        [ScriptIgnore]
        public decimal DocTotal { get; set; }
        [ScriptIgnore]
        public decimal DocTotalFC { get; set; }
        [ScriptIgnore]
        public string SalesMan { get; set; }
        [ScriptIgnore]
        public string DocStatus { get; set; }
        public List<QuotationsLinesModelBase> QuotationsLinesList { get; set; }
        public string UserId { get; set; }
    }

    public class InvPrintModel
    {
        public int DocEntry { set; get; }
        public int DocNum { get; set; }
        public string DocDate { set; get; }
        public string CardName { set; get; }
        public string DocStatus { set; get; }
        public bool IsManualEntry { get; set; }
        public string InvoiceNumber { get; set; }
        public double DocTotal { get; set; }
        public string DocCur { get; set; }
    }

    public class invPrintSearch
    {
        public string slpCode { set; get; }
        public string DocEntry { set; get; }
        public string FechaIni { set; get; }
        public string FechaFin { set; get; }
        public int InvType { set; get; }
    }

    /// <summary>
    /// Es el modelo para la creacion de la factura con pago en sap
    /// </summary>
    public class CreateInvoice
    {
        public InvoiceModel Invoice { get; set; }
        public CreatePaymentModel Payment { get; set; }
    }


    public class CreateSlInvoice
    {
        public IInvoiceDocument Invoice { get; set; }
        public BasePayment Payment { get; set; }
        public PPTransaction PPTransaction { get; set; }
    }

    public class InvoiceFEInfo
    {
        public string CardName { get; set; }
        public string IdType { get; set; }
        public string Identification { get; set; }
        public string Email { get; set; }
        public string Provincia { get; set; }
        public string Canton { get; set; }
        public string Distrito { get; set; }
        public string Barrio { get; set; }
        public string Direccion { get; set; }

    }

    public class InvoiceModel : DocumentModel
    {
        public InvoiceFEInfo FEInfo { get; set; }
        public string U_ClaveFe { get; set; }
        public string U_NumFE { get; set; }
        public string CLVS_POS_UniqueInvId { get; set; }
        public List<LinesInvoiceModel> InvoiceLinesList { get; set; }
        public List<LinesInvoiceModelBase> BaseLines { get; set; }
    }

    public class LinesInvoiceModel : LinesBaseModel
    {
        public bool Active { get; set; }
        public int Id { get; set; }
        public string Item { get; set; }
        public string ItemName { get; set; }
        public decimal Lintot { get; set; }
        public string WhsName { get; set; }
        public bool TaxOnly { get; set; }
    }

    public class LinesInvoiceModelBase : LinesBaseModel
    {
        public string ItemName { get; set; }
        public string WhsName { get; set; }
        public int? BaseType { get; set; }
        public int? BaseLine { get; set; }
        public int? BaseEntry { get; set; }
        public int LineNum { get; set; }
    }

    /// <summary>
    /// Recibe la informacion consultada de un documento DocNum, U_NumFE, U_ClaveFE,
    /// </summary>
    public class DocInfo
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string NumFE { get; set; }
        public string ClaveFE { get; set; }

    }
    public class WhatsappDocumentModel
    {
        public string numeroWhatsapp { get; set; }
        public string Message { get; set; }
        public GetBalanceModel_UsrOrDate modelReport { get; set; }

    }

    public class quotationSearch
    {
        public int SlpCode { get; set; }
        public string DocStatus { get; set; }
        public int DocNum { get; set; }
        public string CardCode { get; set; }
        public string U_Almacen { get; set; }
        public DateTime Fini { get; set; }
        public DateTime Ffin { get; set; }
    }

    public class saleOrderSearch
    {
        public int SlpCode { get; set; }
        public string DocStatus { get; set; }
        public int DocNum { get; set; }
        public string CardCode { get; set; }
        public DateTime Fini { get; set; }
        public DateTime Ffin { get; set; }
    }

    public class InvoiceType
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
    }

    #region document base 
    public class ISaleDocument : DocumentModel
    {
        [ScriptIgnore]
        public string DocStatus { get; set; }
        [ScriptIgnore]
        public string UserId { get; set; }
        [ScriptIgnore]
        public decimal DocTotal { get; set; }
        [ScriptIgnore]
        public decimal DocTotalFC { get; set; }
        [ScriptIgnore]
        public string SalesMan { get; set; }

    }
    public class IQuotDocument : DocumentModel
    {
        [ScriptIgnore]
        public string DocStatus { get; set; }
        [ScriptIgnore]
        public string UserId { get; set; }
        [ScriptIgnore]
        public decimal DocTotal { get; set; }
        [ScriptIgnore]
        public decimal DocTotalFC { get; set; }
        [ScriptIgnore]
        public string SalesMan { get; set; }


    }
    public class IDocument : DocumentModel
    {
        public string DocStatus { get; set; }
        public string UserId { get; set; }
        [ScriptIgnore]
        public decimal DocTotal { get; set; }
        [ScriptIgnore]
        public decimal DocTotalFC { get; set; }
        public string SalesMan { get; set; }
        //public List<DocumentLines> BaseLines { get; set; }



    }
    public class IInvoiceDocument : DocumentModel
    {
        [ScriptIgnore]
        public string DocStatus { get; set; }
        [ScriptIgnore]
        public string UserId { get; set; }
        [ScriptIgnore]
        public decimal DocTotal { get; set; }
        [ScriptIgnore]
        public decimal DocTotalFC { get; set; }
        [ScriptIgnore]
        public string SalesMan { get; set; }

    }
    public class DocumentLines : LinesBaseModel
    {
       
        [ScriptIgnore]
        public string ItemName { get; set; }

        [ScriptIgnore]
        public string WhsName { get; set; }
        [ScriptIgnore]
        public double LastPurchasePrice { get; set; }
        [ScriptIgnore]
        public string LineStatus { get; set; }
        [ScriptIgnore]
        public double OnHand { get; set; }
        public string InvntItem { get; set; }
        
    }



    #endregion
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

using System.Data.Entity;
using CLVSSUPER.MODELS;

namespace CLVSPOS.MODELS
{
    public class CompanysSAPModel
    {
        public string odbctype { get; set; }
        public string Server { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
        public string ServerType { get; set; }
    }

    public class CompanysModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string DBName { get; set; }
        [Required]
        [MaxLength(450)]
        [Index(IsUnique = true)]
        public string DBCode { get; set; }
        public int SAPConnectionId { get; set; }
        public string LogoPath { get; set; }
        [DefaultValue("true")]
        public bool Active { get; set; }
        public int? MailDataId { get; set; }
        // tipo de cambio a utilizar (1 = SAP, 2 = DB)
        public int ExchangeRate { get; set; }
        // valor del tipo de cambio
        [DefaultValue(0)]
        public decimal ExchangeRateValue { get; set; }
        // manejo de items (1 = codigo, 2 = Barras)
        public int HandleItem { get; set; }
        // facturacion de items (1 = codigo, 2 = Series)
        public int BillItem { get; set; }
        // SP par obtener la info de un articulo
        public string SP_ItemInfo { get; set; }
        // SP para obtener la info de una inv
        public string SP_InvoiceInfoPrint { get; set; }
        // View para obtener los clientes
        public string V_BPS { get; set; }
        // View para obtener los items
        public string V_Items { get; set; }
        // View para obtener los el tipo de cambio
        public string V_ExRate { get; set; }
        // View para obtener los tipos de impuestos con su valor
        public string V_Taxes { get; set; }
        // SP para obtener la info de disponibilidad de un item
        public string SP_WHAvailableItem { get; set; }
        // SP para obtener la info de las series por almacen de un item
        public string SP_SeriesByItem { get; set; }
        // SP para obtener la info de los pagos por cliente y sede
        public string SP_PayDocuments { get; set; }
        // Sp para obtener un item por su codigo de barras sin importar si tiene uno o mas codigos
        public string SP_GetItemByCodeBar { set; get; }
        // Sp para obtener la lista de precios a la que se encuentra asociado un producto
        public string SP_GetItemPriceList { set; get; }
        // Sp para obtener todos los codigos de barras registrados a un producto
        public string SP_GetBarcodesByItem { set; get; } 
        //SP para obtener Cliente y proveedores
        public string SP_GetCustomer { get; set; }
        //SP para obtener Cliente y proveedores segun su codigo
        public string SP_GetCustomerbyCode { get; set; }
        // Vista para obtener los proveedores
        public string V_GetSuppliers { set; get; }
        // View para obtener las cuentas
        public string V_GetAccounts { get; set; }
        // View para obtener las tarjetas
        public string V_GetCards { get; set; }
        // View para obtener los bancos
        public string V_GetBanks { get; set; }
        // View para obtener los vendedores
        public string V_GetSalesMan { get; set; }
        // SP para obtener la lista de pagos a cancelar
        public string SP_CancelPayment { get; set; }
        //SP para obtener Cliente y proveedores
        //Usuario SAP
        public string SAPUser { get; set; }
        //Pass SAP
        public string SAPPass { get; set; }
        //Direecion del reporte de Facturas
        public string ReportPath { get; set; }
        public string ReportPathPP { get; set; }
        //Direecion del reporte de Inventario
        public string ReportPathInventory { get; set; }
        //Direecion del reporte de reimprecion de Facturas
        public string ReportPathCopy { get; set; }
        //Direcion del reporte de reimprecion de cotizacion
        public string ReportPathQuotation { get; set; }
        //Direcion del reporte de reimprecion de SO
        public string ReportPathSO { get; set; }
        //Direcion del reporte de reimprecion de cierres de caja
        public string ReportBalance { set; get; }
        //Direcion del reporte de reimpresion de pagos recibidos
        public string ReportRecivedPaid { set; get; }
        public bool IsLinePriceEditable { get; set; }
        ////Scale Setup Properties
        public double ScaleMaxWeightToTreatAsZero { get; set; }
        public double ScaleWeightToSubstract { get; set; }
        [MaxLength(2)]
        public string FEIdType { get; set; }
        [MaxLength(12)]
        public string FEIdNumber { get; set; }
        //Cantidad de decimales configurables por compañia
        public int DecimalAmountPrice { get; set; }
        //Cantidad de decimales por total de linea
        public int DecimalAmountTotalLine { get; set; }
        //Cantidad de decimales por total del documento
        public int DecimalAmountTotalDocument { get; set; }
        // Permite deshabilitar el modo offline en la compania
        public bool HasOfflineMode { get; set; }
        // Permite guardar la configuracion de la impresora que usa el cliente en electron
        public string PrinterConfiguration { get; set; }
        // Permite verificar si la compania permite facturar con precio 0
        public bool HasZeroBilling { get; set; }

        // Define como se agregan las lineas en los documentos, True:Pila, False:Cola
        public bool LineMode { get; set; }

        //001 - Almacena los margenes aceptados para las vistas de la compañia
        public string AcceptedMargins { get; set;}

    }

    public class CompanysSapConnModel : CompanysModel
    {
        // servidor de conexion de SAP
        public string Server { get; set; }
    }
    public class ViewLineAgrupationModel : GroupLine
    {
        
    }
    public class ViewLinesAgrupationList
    {

        public List<GroupLine> viewLineAgrupationList { get; set; }
    }



    /// <summary>
    /// Es un modelo para realizar la creacion de la compania y el correo
    /// </summary>
    public class CompanyAndMail
    {
        public CompanysModel company { get; set; }
        public MailDataModel mail { get; set; }

    }

    /// <summary>
    /// Es un modelo para obtener las modenas desde la vista SQL
    /// </summary>
    public class CurrencyModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
    }


    public class CompanyMargins
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Value { get; set; }
    }


}
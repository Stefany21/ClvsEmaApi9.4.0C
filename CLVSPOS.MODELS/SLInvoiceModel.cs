using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSSUPER.MODELS
{
    public class SLInvoiceModel
    {
        //this system internal invoice number
        public int DocEntry { get; set; }
        //this is the invoice number for the final user
        public int DocNum { get; set; }

        public int? BaseEntry { get; set; }
        //this is the code of the customer
        public string CardCode { get; set; }
        //this is the name of the customer
        public string CardName { get; set; }
        //They type of document, 13 is the DocType for ARInvoice, 14 for A/R Credit Memo, 
        public int DocType { get; set; }
        //The date for the invoice with format 'YYYYMMDD'
        public DateTime DocDate { get; set; }
        //This is the currency of the document
        public string DocCurrency { get; set; }
        //Estado del documento ( ... )
        public int DocumentStatus { get; set; } 
        //Registra la accion ocurrida con el documento al crerlo en SAP
     //   public string StatusDetails { get; set; } 

      
        public string NumAtCard { get; set; }
        // terminos de pagos
        public string PaymentGroupCode { get; set; }
        // Comentarios
        public string Comments { get; set; }
        //codigo de vendedor
        public int SalesPersonCode { get; set; }
        // tipo de documento
        public string U_TipoDocE { get; set; }
        //campos para facturacion electronica
        public string U_TipoIdentificacion { get; set; }
        
        public string U_NumIdentFE { get; set; }
        
        public string U_CorreoFE { get; set; }
       
        public string U_ObservacionFE { get; set; }
       
        public string U_Provincia { get; set; }
       
        public string U_Canton { get; set; }
       
        public string U_Distrito { get; set; }
       
        public string U_Barrio { get; set; }
       
        public string U_Direccion { get; set; }
        
        public string U_ClaveFE { get; set; }
        
        public string U_Online { get; set; }
       
        public string U_CLVS_POS_UniqueInvId { get; set; }
        
        public string U_NumFE { get; set; }

    }


    public class SLDocumentLinesModel
    {
        //codigo del articulo en SAP
        public string ItemCode { get; set; }
        //precio del articulo
        public double UnitPrice { get; set; }
        //cantidad del articulo que se esta vendiendo
        public double Quantity { get; set; }
        //codigo de impuestos
        public string TaxCode { get; set; }
        //ratio de impuestos
        public double TaxRate { get; set; } // No se usa?
        //codigo de almacen
        public string WarehouseCode { get; set; }
        //cantidad de descuento del articulo
        public double DiscountPercent { get; set; }
        //serie del articulo
        public string Serie { get; set; } // No se usa?
        //sysnumber de la serie
        public int SysNumber { get; set; } // No se usa?
        //Precio sugerido
        public double U_SugPrice { get; set; } // No se usa?
        //Ultimo precio compra 
        public double LastPurchasePrice { get; set; } // No se usa?

        public string U_ultPrecio { get; set; }

        public string U_UDF_CodItemXML { get; set; }

        public string U_UDF_DescItemXML { get; set; }
    }
}
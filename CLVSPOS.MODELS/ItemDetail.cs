using CLVSPOS.MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSSUPER.MODELS
{
    public class ItemDetail
    {
        public double LastPurPrc { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int OnHand { get; set; } // Stock
        public int Available { get; set; }
        public string TaxRate { get; set; }

        public List<GoodReceipt> GoodsRecipts { get; set; }

    }

    public class GoodReceipt
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public DateTime? DocDate { get; set; }
        public int DocTotal { get; set; }
        public string Comment { get; set; }
        public BusinessPartnerModel BissnesPartner { get; set; }
        public StoresModel Store { get; set; }
        public string TaxCode { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }

    public class ItemDataForInvoiceGoodReceipt
    {
        public string ItemCode { get; set; }
        public double AVGPrice { get; set; }
        public double LastPrice { get; set; }
        public int DeviationStatus { get; set; }
        public string Message { get; set; }
    }



}
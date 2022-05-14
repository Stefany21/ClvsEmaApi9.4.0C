using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSPOS.MODELS
{
    public class EntryLineModel  
    {
        public string ItemCode { set; get; }
        public string WareHouse { set; get; }
        public double Quantity { set; get; } //VALOR INICAL INT LO CAMBIE double PARA PRUEBAS CON XML 
        public double UnitPrice { set; get; }
        public string ItemName { set; get; }
        public double LineTotal { set; get; }
        public double TaxRate { set; get; }
        public string TaxCode { set; get; }
        public double Discount { set; get; }
        public bool TaxOnly { get; set; }
        public string ItemNameXml { get; set; }
    }

}
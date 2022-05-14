using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSPOS.SAPDAO.Models
{
    public class ITM1
    {
        public int Id { get; set; }
        public int PriceList { get; set; }
        public string ItemCode { get; set; }
        public double Price { get; set; }
        public double? LastPurchasePrice { get; set; }
    }
}
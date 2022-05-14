using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSPOS.SAPDAO.Models
{
    public class OITW
    {
        public Int32 Id { get; set; }

        public string WhsCode { get; set; }

        public string ItemCode { get; set; }

        public double OnHand { get; set; }

        public double OnOrder { get; set; }

        public double IsCommited { get; set; }

        public double AvgPrice { get; set; }
    }
}
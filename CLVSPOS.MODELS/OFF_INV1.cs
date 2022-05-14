using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSPOS.MODELS
{
    public class OFF_INV1
    {
        public int Id { get; set; }

        public int DocEntry { get; set; }

        public int LineNum { get; set; }

        public string ItemCode { get; set; }

        public double Quantity { get; set; }

        public double UnitPrice { get; set; }

        public double DiscountPercent { get; set; }

        public string TaxCode { get; set; }

        public string WarehouseCode { get; set; }

        public string Currency { get; set; }

        public string Description { get; set; }

        public double VatPrcnt { get; set; }

        public double LineTotal { get; set; }

        public virtual OFF_OINV OFF_OINV { get; set; }
    }
}
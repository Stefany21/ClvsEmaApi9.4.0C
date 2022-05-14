using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSSUPER.MODELS
{
    public class InvoicePaymentDetail
    {
        public List<CardModel> Cards { get; set; }
        public Double CashSum { get; set; }
        public Double CashSumFC { get; set; }
        public Double TrsfrSum {get;set;}
		public Double TrsfrSumFC { get; set; }
    }
}
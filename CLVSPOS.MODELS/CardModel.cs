using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSSUPER.MODELS
{
    public class CardModel
    {
		public int LineID { get; set; }
		public String CreditCard { get; set; }
		public String VoucherNum { get; set; }
		public Double CreditSum { get; set; }
		public DateTime CardValid { get; set; }
		public DateTime FirstDue { get; set; }
		public String CreditAcct { get; set; }
		public bool IsManulEntry { get; set; }
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSPOS.MODELS
{
    public class OFF_Payment : OFF_PaymentLine
    {
        public OFF_Payment()
        {
            Checks = new List<Checks>();
            CreditCards = new List<CreditCards>();
        }

        public List<CreditCards> CreditCards { get; set; }
        public List<Checks> Checks { get; set; }
    }
}
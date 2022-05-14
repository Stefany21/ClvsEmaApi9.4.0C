using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSPOS.MODELS
{
    public class CashflowModel
    {
        public int UserSignature { get; set; }
        public DateTime CreationDate { get; set; }
        public double Amount { get; set; }
        public string Type { get; set; }
        public string Reason { get; set; }
        public string Details { get; set; }
    }
}
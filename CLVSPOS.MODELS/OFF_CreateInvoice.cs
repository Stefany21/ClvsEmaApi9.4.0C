using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSPOS.MODELS
{
    public class OFF_CreateInvoice
    {
        public OFF_OINV Invoice { get; set; }
        public List<OFF_Payment> Payments { get; set; } = new List<OFF_Payment>();
    }
}
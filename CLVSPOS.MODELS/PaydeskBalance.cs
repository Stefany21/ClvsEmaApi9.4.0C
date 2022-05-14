using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CLVSPOS.MODELS
{
    public class PaydeskBalance
    {        
            [Key]
            public int Id { get; set; }
            public string UserId { get; set; }
            public int UserSignature { get; set; }
            public DateTime CreationDate { get; set; }
            public double Cash { get; set; }
            public double Cards { get; set; }
            public double CardsPinpad { get; set; }
            public double Transfer { get; set; }
            public double CashflowIncomme { get; set; }
            public double CashflowEgress { get; set; }
        
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CLVSPOS.SAPDAO.Models
{
    public class OTCX
    {
        [Key]
        public string StrVal1 { get; set; }
        public string LnTaxCode { get; set; }
    }
}
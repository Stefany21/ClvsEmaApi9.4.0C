using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CLVSPOS.SAPDAO.Models
{
    public class OWHS
    {
        [Key]
        public string WhsCode { get; set; }

        public string WhsName { get; set; }
    }
}
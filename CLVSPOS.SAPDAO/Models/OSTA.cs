using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CLVSPOS.SAPDAO.Models
{
    public class OSTA
    {
        [Key]
        public string Code { get; set; }

        public string Name { get; set; }

        public double Rate { get; set; }
    }
}
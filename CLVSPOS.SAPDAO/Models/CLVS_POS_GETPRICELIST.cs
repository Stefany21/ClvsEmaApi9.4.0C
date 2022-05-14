using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CLVSPOS.SAPDAO.Models
{
    public class CLVS_POS_GETPRICELIST
    {
        [Key]
        public int ListNum { get; set; }

        public string ListName { get; set; }
    }
}
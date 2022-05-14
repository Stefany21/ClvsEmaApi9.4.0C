using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CLVSPOS.SAPDAO.Models
{
    public class CLVS_POS_ITEMS
    {
        [Key]
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string CodeBars { get; set; }
        public double Available { get; set; }
    }
}
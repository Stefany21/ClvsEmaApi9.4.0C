using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CLVSPOS.SAPDAO.Models
{
    public class CLVS_POS_PAYTERMS
    {
        [Key]
        public int GroupNum { get; set; }

        public string PymntGroup { get; set; }
        public int Type { get; set; }
    }
}
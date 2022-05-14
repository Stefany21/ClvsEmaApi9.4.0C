using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CLVSPOS.SAPDAO.Models
{
    public class CLVS_POS_GETTAXES
    {
        [Key]
        public string Code { get; set; }
        public string Rate { get; set; }
    }
}
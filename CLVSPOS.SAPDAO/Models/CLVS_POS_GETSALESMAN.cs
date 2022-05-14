using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CLVSPOS.SAPDAO
{
    public class CLVS_POS_GETSALESMAN
    {
        [Key]
        public int SlpCode { get; set; }
        public string SlpName { get; set; }
    }
}
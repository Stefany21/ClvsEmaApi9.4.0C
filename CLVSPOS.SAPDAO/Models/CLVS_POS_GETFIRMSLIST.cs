using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CLVSPOS.SAPDAO.Models
{
    public class CLVS_POS_GETFIRMSLIST
    {
        [Key]
        public int FirmCode { get; set; }
        public string FirmName { get; set; }
    }
}
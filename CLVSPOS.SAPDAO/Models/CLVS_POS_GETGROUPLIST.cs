using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CLVSPOS.SAPDAO.Models
{
    public class CLVS_POS_GETGROUPLIST
    {
        [Key]
        public int ItmsGrpCod { get; set; }
        public string ItmsGrpNam { get; set; }
    }
}
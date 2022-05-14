using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CLVSPOS.SAPDAO.Models
{
    public class OITM
    {
        [Key]
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public Double OnHand { get; set; }
        public Int32 ItmsGrpCod { get; set; }
        public string CardCode { get; set; }
        public string frozenFor { get; set; }
        [StringLength(1)]
        public string InvntItem { get; set; }
        public string CodeBars { get; set; }

        public string U_IVA { get; set; }


    }
}
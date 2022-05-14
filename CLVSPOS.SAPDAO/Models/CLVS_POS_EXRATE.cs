using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSPOS.SAPDAO.Models
{
    public class CLVS_POS_EXRATE
    {
        public int Id { get; set; }
        public DateTime RateDate { get; set; }
        public Double Rate { get; set; }
        public string UserId { get; set; }        
    }
}
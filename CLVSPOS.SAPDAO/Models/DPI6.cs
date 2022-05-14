using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSPOS.SAPDAO.Models
{
    public class DPI6
    {
        public int Id { get; set; }
        public int DocEntry { get; set; }
        public int InstlmntID { get; set; }
        public double InsTotalFC { get; set; }
        public double PaidFC { get; set; }
        public double PaidToDate { get; set; }
        public double InsTotal { get; set; }
    }
}
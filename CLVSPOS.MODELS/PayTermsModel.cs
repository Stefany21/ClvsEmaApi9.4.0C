using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSPOS.MODELS
{
    public class PayTermsModel
    {
        public int GroupNum { set; get; }
        public string PymntGroup { set; get; }
        public int Type { get; set; }
    }
}
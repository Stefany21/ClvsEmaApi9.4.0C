using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSPOS.MODELS
{
    public class StoresModel
    {
        public int Id { set; get; }
        public string StoreName { set; get; }
        public string StoreCode { set; get; }
        public bool StoreStatus { set; get; }
        public string Name { set; get; }
        public string companyName { set; get; }
    }
}
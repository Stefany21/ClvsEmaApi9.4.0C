using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSPOS.MODELS
{

    /// <summary>
    /// Modelo de la lista de precios
    /// </summary>
    public class PriceListModel
    {
        public int ListNum { set; get; }
        public string ListName { set; get; }
        public double Price { set; get; }
        public string PrimCurr { get; set; }
        public string AddCurr1 { get; set; }
        public string AddCurr2 { get; set; }

    }
}
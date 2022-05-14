using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSPOS.MODELS
{

    /// <summary>
    /// Modelo para traer el codigo y el monto del impuesto de una lista de items
    /// </summary>
    public class TaxModel
    {
        public string TaxCode { get; set; }
        public string TaxRate { get; set; }
    }
}
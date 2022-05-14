using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSPOS.MODELS
{
    /// <summary>
    /// Es un modelo de las tarjetas
    /// </summary>
    public class CardsModel
    {
        public string CardName { get; set; }
        public string CreditCard { get; set; }
        public string NumberCard { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime DocumentDate { get; set; }
        public string Authorization { get; set; }
        public string Terminal { get; set; }
        public double Amount { get; set; }
    }
}
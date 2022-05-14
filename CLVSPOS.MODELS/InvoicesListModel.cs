using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSPOS.MODELS
{
    /// <summary>
    /// Modelos para obtencion de facturas 
    /// </summary>
    public class InvoicesListModel
    {
        public int InvId { get; set; }
        public int DocNum { get; set; }
        public int DocEntry { get; set; }
        public string Customer { get; set; }
        public DateTime Date { get; set; }
        public string DateAndTime { get; set; }
        public string Status { get; set; }
        public bool Reimpr { get; set; }

        public string DocCur { get; set; }
        public DateTime DocDueDate { get; set; }//vencimiento
        public decimal DocTotal { get; set; }// total
        public decimal DocBalance { get; set; }//saldo

        public bool Selected { get; set; }
        public string type { get; set; }
        public double Pago { get; set; }
        public int InstlmntID { get; set; }
    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSPOS.MODELS
{
    public class OFF_PaymentLine
    {
        public int Id { get; set; }

        public int Series { get; set; }

        public int DocType { get; set; }

        public string DocCurrency { get; set; }

        public string CardCode { get; set; }

        public DateTime DocDate { get; set; }

        public DateTime DueDate { get; set; }

        public string CounterReference { get; set; }

        public string Remarks { get; set; }

        public double CashSum { get; set; }

        public string CashAccount { get; set; }

        public int DocEntry { get; set; }

        public double SumApplied { get; set; }

        public double AppliedFC { get; set; }

        public double U_MontoRecibido { get; set; }

        public double TransferSum { get; set; }

        public string TransferAccount { get; set; }

        public DateTime? TransferDate { get; set; }

        public string TransferReference { get; set; }

        public double DocRate { get; set; }

        public double Change { get; set; }
    }
}
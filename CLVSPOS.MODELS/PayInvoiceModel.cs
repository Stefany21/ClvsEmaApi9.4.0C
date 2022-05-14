using CLVSSUPER.MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace CLVSPOS.MODELS
{
    public class PayInvoiceModel
    {
        
    }
    public class BasePayment
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string CardCode { get; set; }//
        public string DocDate { get; set; }
        public string CashAccount { get; set; }
        public string CheckAccount { get; set; }
        public double CashSum { get; set; }
        public string Remarks { get; set; } //Comments
        public string DocCurrency { get; set; } // Currency
        public decimal DocRate { get; set; }
        public string CounterReference { get; set; } //Reference
        public decimal TransferSum { get; set; }//transfSum
        public string TransferAccount { get; set; } //trfsrAcct
        public string TransferDate { get; set; }//trfsrDate
        public string TransferReference { get; set; }//trfsrNum
        public string DocType { get; set; }
        public int Series { get; set; }
        public string DueDate { get; set; }
        public decimal U_MontoRecibido { get; set; }//ReceivedAmount
        public List<PaymentLines> PaymentInvoices { get; set; }
       // public List<Checks> PaymentChecks { get; set; }
        public List<CreditCards> PaymentCreditCards { get; set; }

        [ScriptIgnore]
        public List<UdfTarget> UDFs { get; set; }

        public string U_CLVS_POS_UniqueInvId { get; set; }

    }

    public class CreatePaymentModel : BasePayment
    {
        public List<Checks> V_Checks { get; set; }
        public List<CreditCards> V_CreditCards { get; set; }
        public List<PaymentLines> V_PaymentLines { get; set; }
         
       public List<UdfTarget> UdfTarget { get; set; }
    }

    public class CreateRecivedPaymentModel : BasePayment
    {
        public List<Checks> V_Checks { get; set; }
        public List<CreditCards> V_CreditCards { get; set; }
        public List<PaymentLines> V_PaymentLines { get; set; }
        public Boolean isPayAccount { get; set; }
        public List<UdfTarget> UdfTarget { get; set; }
    }

    public class CreateSLRecivedPaymentModel : BasePayment
    {
       
    }




    public class Checks
    {
        public string AcctNum { get; set; }
        public string BankCode { get; set; }
        public string CheckAccount { get; set; }//CheckAct
        public string CheckNumber { get; set; }//CheckNum
        public decimal CheckSum { get; set; }
        public string CountryCode { get; set; }
        public string Curr { get; set; }
        public string DueDate { get; set; }
    }

    public class CreditCards
    {
        [ScriptIgnore]
        public string CardValid { get; set; }
        public string CreditCardNumber { get; set; }//CrCardNum
        public string CardValidUntil { get; set; }
        public string CreditCard { get; set; }
        public decimal CreditSum { get; set; }
        [ScriptIgnore]
        public string FormatCode { get; set; }
        public string OwnerIdNum { get; set; }
        public string VoucherNum { get; set; }
        public string U_ManualEntry { get; set; }
        [ScriptIgnore]
        public string CreditAcct { get; set; }
    }

    public class PaymentLines
    {
        public int DocEntry { get; set; }
        public string InvoiceType { get; set; }//DocType
        public double AppliedFC { get; set; }
        public double SumApplied { get; set; }//InvTotal
      

        [ScriptIgnore]
        public string Currency { get; set; }

        [ScriptIgnore]
        public string PayTotal { get; set; }
        [ScriptIgnore]
        public decimal Change { get; set; }

    }

    public class CancelPaymentModel{
        public bool Selected { get; set; }
        public int DocNum { get; set; }
        public int DocEntry { get; set; }
        public string DocDate { get; set; }
        public double DocTotal { get; set; }
        public double DocTotalFC { get; set; }
        public string DocCurr { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string Status { get; set; }
        public int DocNumPago { get; set; }
        public int InvoDocEntry { get; set; }
        public string U_CLVS_POS_UniqueInvId { get; set; }
    }

    public class paymentSearchModel{
        public string CardCode { get; set; }
        public string FIni { get; set; }
        public string FFin { get; set; }
    }

    /// <summary>
    /// Modelo para la cancelacion del pago
    /// </summary>
    public class CancelPayModel
    {
        public int DocEntry { get; set; }
    }
}
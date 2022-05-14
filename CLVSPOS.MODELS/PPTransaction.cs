using CLVSPOS.MODELS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CLVSSUPER.MODELS
{
    public class PPTransaction
    {
        [Key]
        public int Id { get; set; }
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public string AuthorizationNumber { get; set; }
        public string EntryMode { get; set; }
        public string ExpirationDate { get; set; }
        public string ReferenceNumber { get; set; }
        public int TerminalId { get; set; }
        public string ResponseCode { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDocument { get; set; }
        public string SystemTrace { get; set; }
        public string TransactionId { get; set; }
        public string CharchedStatus { get; set; }
        public string ChargedResponse { get; set; }
        public string CanceledStatus { get; set; }
        public string CanceledResponse { get; set; }
        public string ReversedStatus { get; set; }
        public string ReversedResponse { get; set; }
        public string Currency { get; set; }
        public string BacId { get; set; }
        public string UserPrefix { get; set; }
        public double Amount { get; set; }
        public Boolean IsOnBalance { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? LastUpDate { get; set; }
        [NotMapped]
        public double QuickPayAmount { get; set; }
        // Indica si pertenece a un precierre
        public int AcqPrebalance { get; set; }
        // Indica si pertenece a un cierre
        public int AcqBalance { get; set; }
        [NotMapped]
        public PPDocuments ppDocument { get; set; }
        public double SaleAmount { get; set; }
        public int UpdatesCounter { get; set; }
    }

    public class SelfPPTransaction
    {
        public PPTransaction Transaction { get; set; }
        public string RawData { get; set; }
        public PPTerminal Terminal { get; set; }
        public string UserPrefix { get; set; }
    }
}
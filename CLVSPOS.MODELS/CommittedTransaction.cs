using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CLVSPOS.MODELS;

namespace CLVSSUPER.MODELS
{
	public class CommittedTransaction
	{
        public int Id { get; set; }
        public int DocEntry { get; set; }
        public string InvoiceNumber { get; set; }
        public string ReferenceNumber { get; set; }
        public string AuthorizationNumber { get; set; }
        public string SalesAmount { get; set; }
        public string HostDate { get; set; }
        public DateTime CreationDate { get; set; }
        // LLave foranea hacia la transaccion, la cual se termina de identificar con el tipo de transanccion
        public int ACQ { get; set; }
        public string TransactionType { get; set; }
        public string TerminalCode { get; set; }
        public PPTerminal Terminal { get; set; }
    }

    public class ACQTransaction
    {
        public PPTerminal Terminal { get; set; }
        public PPBalance OverACQ { get; set; }
        public PPBalanceRequest BalanceRequest { get; set; }
    }
}
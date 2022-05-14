using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSSUPER.MODELS
{
    public class TransactionPrint
    {
        public string PrintTags { get; set; }
        public string TerminalCode { get; set; }
        public string MaskedNumberCard { get; set; }
        public int DocEntry { get; set; }
        public Boolean IsSigned { get; set; }
    }
}
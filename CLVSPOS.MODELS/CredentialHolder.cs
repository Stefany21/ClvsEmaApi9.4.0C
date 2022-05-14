using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CLVSPOS.MODELS;

namespace CLVSSUPER.MODELS
{
    public class CredentialHolder
    {
        public string DBCode { get; set; }
        public string Server { get; set; }
        public string DST { get; set; }
        public string SAPUser { get; set; }
        public string SAPPass { get; set; }
        public string DBUser { get; set; }
        public string DBPass { get; set; }
        public string ODBCType { get; set; }
        public string ServerType { get; set; }
    }
}
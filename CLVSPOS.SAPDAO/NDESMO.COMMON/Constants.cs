using System.Collections.Generic;

namespace CLVSSUPER.COMMON
{
    public class Constants
    {
        public static string SQLSERVER_TRUSTED = "SQLSERVERT";

        public static Dictionary<string, string> ConnectionString = new Dictionary<string, string> {
            {"SQLSERVERT" , "Driver={#ODBCType#}; Server=#Server#;Trusted_Connection=Yes;" },
            {"SQLSERVER" , "Driver={#ODBCType#};Server=#Server#;Uid=#UserId#;Pwd=#Password#;" },
            {"HANASERVERT" , "Driver={#ODBCType#};SERVERNODE=#Server#;UID=#UserId#;PWD=#Password#;"},
            {"HANASERVER" , "Driver={#ODBCType#};SERVERNODE=#Server#;UID=#UserId#;PWD=#Password#;" }
        };

        public sealed class AccessSources
        {
            public const string CreateInvoice = "V_AInv";
            public const string Permissions = "V_Perm";
        }

        /// <summary>
        /// identificador de los tipos de Log a crear
        /// </summary>
        public enum LogTypes
        {
            General = 1,
            API = 2,
            SAP = 3
        }

        /// <summary>
        /// identificador de los tipos de series
        /// </summary>
        public enum SerialType
        {
            Invoice = 1,
            Quote = 2,
            Payment = 3,
            SOrder = 4
        }

        /// <summary>
        /// identificador de los tipos de numeraciones de serie
        /// </summary>
        public enum SerialNumberTypes
        {
            Serial = 1,
            Manual = 2
        }

        /// <summary>
        /// identificador de los tipos de documentos
        /// </summary>
        public enum DocumentTypes
        {
            Invoice = 1,
            AnticipatedPayment = 2,
            Quote = 3
        }

        /// <summary>
        /// identificador de los tipos de reporte
        /// </summary>
        public enum ReportTypes
        {
            SaleOrder = 1,
            Quotation = 2,
            Inventory = 3,
            ReprintARInvoice = 4,
            ArInvoice = 5
        }
    }

    

}
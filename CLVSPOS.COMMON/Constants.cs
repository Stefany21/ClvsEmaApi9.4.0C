using System.Collections.Generic;

namespace CLVSPOS.COMMON
{
    public class Constants
    {

        public const string SDBC = "SessionDBCode";
        //HANA ODBC TRUSTED
        public const string HANATODBCConFormat = "Driver={#ODBCType#};SERVERNODE=#Server#;UID=#UserId#;PWD=#Password#;";
        //SQL ODBC
        public const string HANAODBCConFormat = "Driver={#ODBCType#};SERVERNODE=#Server#;UID=#UserId#;PWD=#Password#;";
        //SQL ODBC TRUSTED
        public const string SQLTODBCConFormat = "Driver={#ODBCType#}; Server=#Server#;Trusted_Connection=Yes;";
        //SQL ODBC
        public const string SQLODBCConFormat = "Driver={#ODBCType#};Server=#Server#;Uid=#UserId#;Pwd=#Password#;";


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
            SAP = 3,
            Auto,
            STOCK,
            CompanyActive
        }

        /// <summary>
        /// identificador de los tipos de series
        /// </summary>
        public enum SerialType
        {
            Invoice = 1,
            CreditMemo=7,
            Quote = 2,
            Payment = 3,
            SOrder = 4,
            Customer = 5,
            Vendor= 6,
            ApInvoice = 8,
            OutgoinPayment = 9
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
            CreditMemo = 6,
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
            ArInvoice = 5,
            RecivedPaid = 6
        }

        public enum SerieNumberingType {
            Online = 1,
            Offline = 2
        }
        public enum ObjectSapCode
        {
            Invoice = 13
        }

        public enum VOUCHER_SIGN
        {
            SIGNED = 1,
            NO_SIGNED,
            SIGNED_WITH_COPY
        }

        #region Terminals
        public enum TransactionStatus
        {
            STARTED,
            FINISHED,
            FINISHED_WITH_ERRORS
        }

        public static class PPRequestType
        {
            public const string Sale = "SALE";
            public const string Cancel = "VOID";
            public const string Reverse = "REVERSE";
            public static string PBalance = "BATCH_INQUIRY";
            public static string Balance = "BATCH_SETTLEMENT";
            public static string TestConnect = "ECHO_TEST";
            public const string Create = "CREATE";
            public const string CancelRegister = "CANCELREGIS";
            public const string ReverseRegister = "REVERSEREGIS";
        }

        public enum DocumentTypeTransaction
        {
            PRE_BALANCE,
            BALANCE
        }

        #endregion








    }



}
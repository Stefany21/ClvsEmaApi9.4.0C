using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSPOS.MODELS
{

    /// <summary>
    /// Es un modelo que almacena informacion de las cuentas
    /// Permite obtener las cuentas
    /// </summary>
    public class AccountModel
    {
        public string AccountName { get; set; }
        public string Account { get; set; }
    }



    public class ContableAccounts
    {
        public List<AccountModel> CashAccounts { get; set; }
        public List<AccountModel> CheckAccounts { get; set; }
        public List<AccountModel> TransferAccounts { get; set; }
    }
}
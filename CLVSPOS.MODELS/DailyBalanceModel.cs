using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSPOS.MODELS
{
    public class DailyBalanceModel
    {
    }
    //Modelo para obtener el cierre por usuario por fechas
    public class GetBalanceModel_UsrOrDate
    {
        public DateTime FIni { get; set; }
        public DateTime FFin { get; set; }
        // public string Sede { get; set; }
        public string User { get; set; }
        public string DbCode { get; set; }      
       public SendMailModel SendMailModel { set; get; }

          
        
    }
    public class SendMailModel
    {
        public string subject { get; set; }
        public string emailsend { get; set; }
        public string message { get; set; }
        public string EmailCC { get; set; }
    }
   
    //modelo para losbalances por usuario 
    public class BalanceByUserDetails
    {
        public string DocDate { get; set; }
        public string DocNumP { get; set; }
        public string DocNumF { get; set; }
        public int DocEntry { get; set; }
        public string DocCur { get; set; }
        public string CardName { get; set; }
        public string Type { get; set; }
        public decimal? Balance { get; set; }
        public decimal? CashSum { get; set; }
        public decimal? CreditSum { get; set; }
        public decimal? CheckSum { get; set; }
        public decimal? TrsfrSum { get; set; }
        public decimal? CashSumFC { get; set; }
        public decimal? CredSumFC { get; set; }
        public decimal? CheckSumFC { get; set; }
        public decimal? TrsfrSumFC { get; set; }
        public decimal? PayTotal { get; set; }
        public double TotalDoc { get; set; }        
    }
    // modelo para las notas de credito
    public class BalanceByUserDetailsCN
    {
        public string DocDate { get; set; }
        public string DocNumP { get; set; }
        public string DocNumF { get; set; }
        public int DocEntry { get; set; }
        public string DocCur { get; set; }
        public string CardName { get; set; }
        public string Type { get; set; }
        public decimal? Balance { get; set; }
        public decimal? CashSum { get; set; }
        public decimal? CreditSum { get; set; }
        public decimal? CheckSum { get; set; }
        public decimal? TrsfrSum { get; set; }
        public decimal? CashSumFC { get; set; }
        public decimal? CredSumFC { get; set; }
        public decimal? CheckSumFC { get; set; }
        public decimal? TrsfrSumFC { get; set; }
        public decimal? PayTotal { get; set; }
        public double TotalDoc { get; set; }
    }
}
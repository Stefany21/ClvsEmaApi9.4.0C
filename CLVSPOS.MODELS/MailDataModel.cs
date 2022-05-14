using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSPOS.MODELS
{

    /// <summary>
    /// Modelo para el correo
    /// </summary>
    public class MailDataModel
    {
        public int Id { get; set; }
        public string subject { get; set; }
        public string from { get; set; }
        public string user { get; set; }
        public string pass { get; set; }
        public int port { get; set; }
        public string Host { get; set; }
        public bool SSL { get; set; }
    }
//    public class sendMailModel
//    {      
//        public string subject { get; set; }
//        public string emailsend { get; set; }
//        public string message { get; set; }   
    
//}
    //public class ListMailModel
    //{
    //    public List<sendMailModel> sendMailModel { set; get; }
    //    public List<GetBalanceModel_UsrOrDate> BalanceModel { set; get; }
    //}
}
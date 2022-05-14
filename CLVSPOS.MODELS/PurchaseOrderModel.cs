using CLVSPOS.MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSSUPER.MODELS
{
    public class PurchaseOrderModel
    {
        public int DocEntry { get; set; }
        public int DocNum { set; get; } 
        public BusinessPartnerModel BusinessPartner { set; get; }
        public string Comments { set; get; }
        public string NumAtCard { set; get; }
        public DateTime DocDueDate { set; get; }
        public DateTime DocDate { set; get; }
        public double DocTotal { get; set; }
        public List<EntryLineModel> Lines { set; get; }
        public int PriceList { set; get; }
        public List<UdfTarget> UdfTarget { get; set; }
        public string U_CLVS_POS_UniqueInvId { get; set; }
    }
    public class PurchaseOrderSearchModel
    {
        public string CardCode { get; set; }
        public string FIni { get; set; }
        public string FFin { get; set; }
    }

}
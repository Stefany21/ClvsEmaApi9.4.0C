using CLVSPOS.MODELS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CLVSSUPER.MODELS
{
    public class GoodsReceipt
    {
        public BusinessPartnerModel BusinessPartner { set; get; }
        public string Comments { set; get; }
        public string NumAtCard { set; get; }
        public DateTime DocDueDate { set; get; }
        public DateTime DocDate { set; get; }
        public List<EntryLineModel> Lines { set; get; }
        public int PriceList { set; get; }
        public string UserId { get; set; }
        public string GoodsReceiptAccount { get; set; }
        public List<UdfTarget> UdfTarget { get; set; }
        public string U_CLVS_POS_UniqueInvId { get; set; }
    } 
   
    

}
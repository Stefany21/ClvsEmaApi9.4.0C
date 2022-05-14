using CLVSSUPER.MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSPOS.MODELS
{
    /// <summary>
    /// Modelo para traer la infro de un item
    /// </summary>
    public class ItemsModel
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string TaxCode { get; set; }
        public double TaxRate { get; set; }
        public decimal Discount { get; set; }
        public decimal UnitPrice { get; set; }
        public string FirmName { get; set; }
        public string ForeingName { set; get; }
        public int OnHand { get; set; }
        public string InvntItem { get; set; }
        public Boolean Frozen { get; set; }
        public string BarCode { get; set; }
        public double? Quantity { get; set; }
        public DateTime? LastPurchaseDate { get; set; }
        public double? LastPurchasePrice { get; set; }
        public List<PriceListModel> PriceList { get; set; }
        public List<ItemsBarcodeModel> Barcodes { set; get; }
        public List<UdfTarget> UdfTarget { get; set; }
        public Boolean HasInconsistency { get; set; }
        public string InconsistencyMessage { get; set; }
        public string ItemClass { get; set; }
        public bool ShouldValidateStock { get; set; }
    }
    public class ItemsChangePriceModel
    {   public List<ItemsModel> ItemsList { set; get; }
        public int priceList { get; set; }             
    }

    /// <summary>
    /// Modelo para representar el codigo de barra del item
    /// </summary>
    public class ItemsBarcodeModel
    {
        public int BcdEntry { set; get; }
        public string BcdCode { set; get; }
        public string BcdName { set; get; }
        public int UomEntry { set; get; }
    }

    /// <summary>
    /// Modelo para traer el nombre y al codigo de una lista de items
    /// </summary>
    public class ItemNamesModel
    {
        public List<string> ItemCode { get; set; }
        public List<string> ItemName { get; set; }
        public List<string> ItembarCode { get; set; }
        public List<string> ItemCompleteName { get; set; }
        public List<ItemGroupModel> ItemGroupName { get; set; }
        public List<ItemFirmModel> ItemFirmName { get; set; }        

    }

    /// <summary>
    /// Modelo para la lista de Grupos de los items
    /// </summary>
    public class ItemGroupModel {
        public string GroupName { get; set; }
        public string GroupCode { get; set; }
    }

    /// <summary>
    /// Modelo de la lista de los fabricantes de lis items
    /// </summary>
    public class ItemFirmModel {
        public string FirmName { get; set; }
        public string FirmCode { get; set; }
    }

    public class SyncItemModel
    {
        public List<string> ItemCode { get; set; }
        public List<string> ItemName { get; set; }
        public List<string> CodeBars { get; set; }
        public List<string> Available { get; set; }        

    }

}
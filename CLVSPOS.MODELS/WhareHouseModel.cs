using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSPOS.MODELS
{

    /// <summary>
    /// Modelos para almacenes
    /// </summary>
    public class WhareHouseModel
    {
        public string WhCode { get; set; }
        public decimal? WhOnHand { get; set; }
    }

    /// <summary>
    /// Modelo para la obtencion de informacion del inventario
    /// </summary>
    public class WHInfoModel
    {
        public string WhsCode { get; set; }
        public string WhsName { get; set; }
        public decimal? OnHand { get; set; }
        public decimal? IsCommited { get; set; }
        public decimal? OnOrder { get; set; }
        public decimal? Disponible { get; set; }
        public decimal Price { get; set; }
        public string InvntItem { get; set; }
    }

    /// <summary>
    /// Modelo que guarda los datos del warehouse para procesar en la vista de almacenes
    /// </summary>
    public class WHplaceModel{
        public string WhsCode { get; set; }
        public string WhsName { get; set; }
    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSPOS.MODELS
{

    /// <summary>
    /// Modelo para las series por almacen
    /// </summary>
    public class SeriesModel
    {
        public string Motor { get; set; }
        public string PlacaChasis { get; set; }
        public decimal Quantity { get; set; }
        public decimal Comprometido { get; set; }
        public decimal Disponible { get; set; }
        public string Color { get; set; }
        public int Annio { get; set; }
        public string Ubicacion { get; set; }
        public DateTime InDate { get; set; }
        public decimal Precio { get; set; }
        public string Almacen { get; set; }
        public string SysNumber { get; set; }
    }

}
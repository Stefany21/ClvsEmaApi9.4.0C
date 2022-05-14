using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSSUPER.MODELS
{
    public class DocumentXml
    {
    }
    public class LineaDetalle
    {
        public string NumeroLinea { get; set; }
        public string Codigo { get; set; }
        public string CodigoComercial { get; set; }

        public string Cantidad { get; set; }

        public string UnidadMedida { get; set; }

        public string UnidadMedidaComercial { get; set; }

        public string Detalle { get; set; }

        public string PrecioUnitario { get; set; }

        public string MontoTotal { get; set; }

        public string MontoDescuento { get; set; }

        public string NaturalezaDescuento { get; set; }

        public string SubTotal { get; set; }

        public string Tarifa { get; set; }

        public string ImpuestoNeto { get; set; }

        public string MontoTotalLinea { get; set; }

        public string PorcentajeExoneracion { get; set; }

    }
    public class Emisor{
        public string Nombre { get; set; }
        public string Numero { get; set; }


    }
  
  
}
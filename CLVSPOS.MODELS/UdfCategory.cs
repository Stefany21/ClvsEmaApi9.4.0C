using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSSUPER.MODELS
{
    public class UdfCategory
    {
        // Nombre de la tabla en sap ejemplo: oinv
        public string Name { get; set; }
        // Descripcion que el usuario ve en la app ejemplo: facturacion
        public string Description { get; set; }
        // Representa el criterio de busqueda del objeto ejemplo: oinv -> DocEntry, ocrd -> cardcode
        public string Key { get; set; }
    }

    public class TransferUdf
    {
        public List<Udf> Udfs { get; set; }
        public string Category { get; set; }
    }
}
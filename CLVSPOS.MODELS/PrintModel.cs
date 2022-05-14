using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSPOS.MODELS
{
    public class PrintModel
    {
    }
    public class PrintInventoryModel
    {
        public string Articulo { get; set; }
        public string Marca { get; set; }
        public string Grupo { get; set; }
        public string subGrupo { get; set; }
    }
}
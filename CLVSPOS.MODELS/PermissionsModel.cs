using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSPOS.MODELS
{

    /// <summary>
    /// Modelo que se usa para obtener el permiso y el valor de este para ser analizado
    /// </summary>
    public class PermissionsModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
    }
}
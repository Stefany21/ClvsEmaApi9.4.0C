using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CLVSPOS.MODELS
{

    /// <summary>
    /// Modelo de conexion de SAP
    /// </summary>
    public class SAPConnectionModel
    {
        public int Id { get; set; }
        [Required]
        public string Server { get; set; }
        public string LicenseServer { get; set; }
        public string BoSuppLangs { get; set; }
        [Required]
        public string DST { get; set; }
        public string DBUser { get; set; }
        public string DBPass { get; set; }
        [Required]
        public bool UseTrusted { get; set; }
        public string ODBCType { get; set; }
        public string DBEngine { get; set; }
        public string ServerType { get; set; }
    }
}
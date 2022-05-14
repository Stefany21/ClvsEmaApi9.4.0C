using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CLVSPOS.SAPDAO.Models
{
    public class OCRD
    {
        [Key]
        public string CardCode { get; set; }

        public string CardName { get; set; }

        public string FatherCard { get; set; }

        public string U_TipoIdentificacion { get; set; }

        public string U_provincia { get; set; }

        public string U_canton { get; set; }

        public string U_distrito { get; set; }

        public string U_barrio { get; set; }

        public string U_direccion { get; set; }

        public Int16 GroupNum { get; set; }
    }
}
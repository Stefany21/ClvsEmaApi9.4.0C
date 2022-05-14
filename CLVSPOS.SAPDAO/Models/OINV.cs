using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSPOS.SAPDAO.Models
{
    public class OINV
    {
        public int Id { get; set; }

        public int DocEntry { get; set; }

        public int DocNum { get; set; }

        public string CardCode { get; set; }

        public string CardName { get; set; }

        public string NumAtCard { get; set; }

        public string DocCur { get; set; }

        public DateTime? DocDate { get; set; }

        public DateTime? DocDueDate { get; set; }

        public string DocStatus { get; set; }

        public string Series { get; set; }

        public string U_TipoIdentificacion { get; set; }

        public string U_NumIdentFE { get; set; }

        public string U_CorreoFE { get; set; }

        public string U_provincia { get; set; }

        public string U_canton { get; set; }

        public string U_distrito { get; set; }

        public string U_barrio { get; set; }

        public string U_direccion { get; set; }

        public string U_ClaveFE { get; set; }

        public string U_NumFE { get; set; }

        public string Canceled { get; set; }

        public int SlpCode { get; set; }

        public decimal DocRate { get; set; }

        public Int16? DocTime { get; set; }

        public decimal? DocTotal { get; set; }

        public decimal? DocTotalFC { get; set; }

        public decimal? PaidToDate { get; set; }

        public decimal? PaidFC { get; set; }

        public Int16? UserSign { get; set; }
    }
}
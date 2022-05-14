using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace CLVSPOS.SAPDAO.Models
{
    public class CLVS_POS_BPS
    {
        [Key]
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string Currency { get; set; }
        public string TaxCode { get; set; }
        public double Available { get; set; }
        public double CreditLine { get; set; }
        public double Balance { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Fax { get; set; }
        public string Cellular { get; set; }
        public string E_Mail { get; set; }
        public string MailAddres { get; set; }
        public double Discount { get; set; }
        public int ListNum { get; set; }
        public int GroupNum { get; set; }
        public string Address { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public string QryGroup1 { get; set; }
        public string EditPrice { get; set; }
        public string Cedula { get; set; }
        public string ContactPerson { get; set; }
        public string U_TipoIdentificacion { get; set; }
        public string U_provincia { get; set; }
        public string U_canton { get; set; }
        public string U_distrito { get; set; }
        public string U_barrio { get; set; }
        public string U_direccion { get; set; }
    }
}
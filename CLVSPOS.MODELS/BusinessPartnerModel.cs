using CLVSSUPER.MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSPOS.MODELS
{
    public class BusinessPartnerModel
    {
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string Address { get; set; }
        public string Phone1 { get; set; }
        public string Balance { get; set; }
        public string GroupNum { get; set; }
        public string Discount { get; set; }
        public string ListNum { get; set; }
        public string Currency { get; set; }
        public string E_mail { get; set; }
        public string QryGroup1 { get; set; }
        public string IdType { get; set;  }
        public string Provincia { get; set; }
        public string Canton { get; set; }
        public string Distrito { get; set; }
        public string Barrio { get; set; }
        public string Cedula { get; set; }
        public string Direccion { get; set; }  
        public bool ClienteContado { get; set; } 
       
    }

    public class GetCustomerModel
    {
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string CardType { get; set; }
        public string Phone1 { get; set; }
        public string LicTradNum { get; set; }
        public string E_Mail { get; set; }
        public string U_TipoIdentificacion { get; set; }
        public string U_provincia { get; set; }
        public string U_canton { get; set; }
        public string U_distrito { get; set; }
        public string U_barrio { get; set; }
        public string U_direccion { get; set; }
        public List<UdfTarget> UdfTarget { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSPOS.MODELS
{
    public class OFF_OINV
    {
        public OFF_OINV()
        {
            OFF_INV1 = new List<OFF_INV1>();
        }

        public int Id { get; set; }

        public int DocEntry { get; set; }

        public int DocNum { get; set; }

        public string CardCode { get; set; }

        public string CardName { get; set; }

        public string NumAtCard { get; set; }

        public string DocCur { get; set; }

        public DateTime? DocDate { get; set; }

        public DateTime? DocTime { get; set; }

        public DateTime? DocDueDate { get; set; }

        public string DocStatus { get; set; }

        public string Series { get; set; }

        public string PaymentGroupCode { get; set; }

        public string Comments { get; set; }

        public string DocType { get; set; }

        public string SalesPersonCode { get; set; }

        public string IdType { get; set; }

        public string U_CLVS_POS_UniqueInvId { get; set; }

        public string U_TipoIdentificacion { get; set; }

        public string U_NumIdenFE { get; set; }

        public string U_CorreoFE { get; set; }

        public string U_Provincia { get; set; }

        public string U_Canton { get; set; }

        public string U_Distrito { get; set; }

        public string U_Barrio { get; set; }

        public string U_Direccion { get; set; }

        public string FatherCard { get; set; }

        public double DocRate { get; set; }

        public double DocTotal { get; set; }

        public double? SAPDocEntry { get; set; }

        public string NumeroConsecutivo { get; set; }

        public string Clave { get; set; }

        public int CompanyByUsers_Id { get; set; }

        public virtual List<OFF_INV1> OFF_INV1 { get; set; }
    }
}
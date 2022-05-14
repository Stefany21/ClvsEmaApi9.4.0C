using System;
using System.ComponentModel.DataAnnotations;

namespace CLVSPOS.SAPDAO.Models
{
    public class ORDR
    {
        [Key]
        public int Id { get; set; }

        public int DocEntry { get; set; }

        public DateTime DocDate { get; set; }

        public string DocStatus { get; set; }

        public string CardName { get; set; }

        public int SlpCode { get; set; }

    }
}
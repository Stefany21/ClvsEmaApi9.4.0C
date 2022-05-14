using System;
using System.ComponentModel.DataAnnotations;

namespace CLVSPOS.SAPDAO.Models
{
    public class ORCT
    {
        [Key]
        public int Id { get; set; }

        public Int16 DocTime { get; set; }

        public DateTime DocDate { get; set; }

        public int DocNum { get; set; }

        public int DocEntry { get; set; }

        public string DocCurr { get; set; }

        public decimal? CashSum { get; set; }

        public decimal? CreditSum { get; set; }

        public decimal? CheckSum { get; set; }

        public decimal? TrsfrSum { get; set; }

        public decimal? CashSumFC { get; set; }

        public decimal? CredSumFC { get; set; }

        public decimal? CheckSumFC { get; set; }

        public decimal? TrsfrSumFC { get; set; }

        public decimal? DocTotal { get; set; }

        public decimal? DocTotalFC { get; set; }
    }
}
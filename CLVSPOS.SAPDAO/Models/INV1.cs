using System.ComponentModel.DataAnnotations;

namespace CLVSPOS.SAPDAO.Models
{
    public partial class INV1
    {
        [Key]
        public int Id { get; set; }

        public int DocEntry { get; set; }

        [StringLength(6)]
        public string Currency { get; set; }

        public double VatPrcnt { get; set; }

        public double TotalSumSy { get; set; }

        public double LineTotal { get; set; }

        public string ItemCode { get; set; }

        public double Quantity { get; set; }

        public double Price { get; set; }

        public double DiscPrcnt { get; set; }

        public string Dscription { get; set; }

        public string TaxCode { get; set; }
    }
}
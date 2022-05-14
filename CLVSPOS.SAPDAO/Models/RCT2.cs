using System.ComponentModel.DataAnnotations;

namespace CLVSPOS.SAPDAO.Models
{
    public partial class RCT2
    {
        [Key]
        public int Id { get; set; }

        public string InvType { get; set; }

        public int DocNum { get; set; }

        public int DocEntry { get; set; }
    }
}
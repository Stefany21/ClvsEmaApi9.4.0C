using System;
using System.ComponentModel.DataAnnotations;

namespace CLVSPOS.SAPDAO.Models
{
    public class OUSR
    {
        [Key]
        public int Id { get; set; }

        public Int16 INTERNAL_K { get; set; }
    }
}
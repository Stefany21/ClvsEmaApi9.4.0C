using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CLVSSUPER.MODELS
{
    public class CompanyUdfs
    {
        [Key]
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string TableId { get; set; }
        public string Udfs { get; set; }
    }
}
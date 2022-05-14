using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CLVSPOS.MODELS
{
	public class PPDocuments
	{
        [Key]
        public int Id { get; set; }
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string UniqueInvoCode { get; set; }
        public string BacId { get; set; }
        public string Status { get; set; }
        public string Document { get; set; } //Documento en formato JSON
        public string DocumentType { get; set; }
        public DateTime CreationDocTime { get; set; }
    }
}
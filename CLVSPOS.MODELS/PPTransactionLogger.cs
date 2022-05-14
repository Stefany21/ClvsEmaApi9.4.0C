using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace CLVSPOS.MODELS
{
	public class PPTransactionLogger
	{
        [Key]
        public int Id { get; set; }
        public int Attempts { get; set; }
        public string DocumentReference { get; set; }
        public string Type { get; set; }
        public string XmlDocumentResponse { get; set; }
        public string BACResponseCode { get; set; }
        public int Status { get; set; }
        public DateTime StarTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
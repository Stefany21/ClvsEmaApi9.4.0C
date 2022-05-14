using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSSUPER.MODELS
{
    public class Udf
    {
        public string TableId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string FieldType { get; set; }
        public string Values { get; set; }
        public bool IsActive { get; set; }
        public bool IsRequired { get; set; }
        public bool IsRendered { get; set; }
    }

    public class UdfTarget
    {
        public string Name { get; set; }
        public string FieldType { get; set; }
        public string Value { get; set; }
        
    }
    // Indica sobre que documento se realizararn las maniobras
    public class UdfSource
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string TableId { get; set; }
        public List<UdfTarget> UdfsTarget { get; set; }
    }
}
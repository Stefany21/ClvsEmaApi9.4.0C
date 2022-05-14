using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;


namespace CLVSPOS.MODELS
{
    public class File
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public Stream Content { get; set; }
        public string Base64 { get; set; }
    }   
}
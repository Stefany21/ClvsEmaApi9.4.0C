using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CLVSSUPER.MODELS
{
    public class DBObjectName
    {        
            [Key]
            public int Id { get; set; }

            public string Name { get; set; }
            public string Description { get; set; }
           public string DbObject { get; set; }
            public string Type { get; set; }

    }
}
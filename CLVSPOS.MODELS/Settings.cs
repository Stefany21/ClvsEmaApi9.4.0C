using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CLVSSUPER.MODELS
{
    public class Settings
    {
        [Key]
        public int Id { get; set; }

        public int Codigo { get; set; }

        public string Vista { get; set; }

        public string Json { get; set; }
    }
    public class SettingsJson
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }
    }

}
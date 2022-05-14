using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSPOS.MODELS
{
    public class ParamsModel
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int ViewsId { get; set; }
        public int ParamsId { get; set; }
        public string Descrip { get; set; }
        public int Order { get; set; }
        public bool Visibility { get; set; }
        public bool Display { get; set; }
        public string Text { get; set; }
        public string Name { get; set; }
        public int type { get; set; }
    }
}
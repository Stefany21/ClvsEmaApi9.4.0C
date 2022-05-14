using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CLVSPOS.MODELS
{
    public class BPDataModel : BaseResponse
    {
        public List<PadronModel> BPDataModelList { get; set; }
    }
    public class PadronModel
    {
        public string Id { get; set; }
        public string Codelec { get; set; }
        public int Sexo { get; set; }
        public string FechaCaduc { get; set; }
        public string Junta { get; set; }
        public string Nombre { get; set; }
        public string Apellido1 { get; set; }
        public string Apellido2 { get; set; }
    }
}
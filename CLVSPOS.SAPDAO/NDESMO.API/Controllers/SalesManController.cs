using CLVSSUPER.COMMON;
using CLVSSUPER.LOGGER;
using CLVSSUPER.MODELS;
using System;
using System.Net.Http;
using System.Web.Http;

namespace CLVSSUPER.API.Controllers
{
    public class SalesManController : ApiController
    {
        /// <summary>
        /// Obtiene la lista de los vendedores de SAP
        /// no recibe parametros
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/SalesMan/GetSalesMan")]
        [ActionName("GetSalesMan")]
        public HttpResponseMessage GetSalesMan()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSSUPER.PROCESS.Process.GetSalesMan());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (SalesManResponse)LogManager.HandleExceptionWithReturn(exc, "SalesManResponse", "api/SalesMan/GetSalesMan", (int)Constants.LogTypes.API));
            }
        }

    }
}

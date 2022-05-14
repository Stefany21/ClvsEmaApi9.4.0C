using CLVSSUPER.COMMON;
using CLVSSUPER.LOGGER;
using CLVSSUPER.MODELS;
using System;
using System.Net.Http;
using System.Web.Http;

namespace CLVSSUPER.API.Controllers
{
    public class TaxController : ApiController
    {
        /// <summary>
        /// devuelvo los tipos de impuestos desde SAP
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Tax/GetTaxes")]
        [ActionName("GetTaxes")]
        public HttpResponseMessage GetTaxes()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSSUPER.PROCESS.Process.GetTaxes());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (TaxesResponse)LogManager.HandleExceptionWithReturn(exc, "TaxesResponse", "api/Tax/GetTaxes", (int)Constants.LogTypes.API)); 
            }
        }
    }
}

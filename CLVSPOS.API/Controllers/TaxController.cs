using CLVSPOS.COMMON;
using CLVSPOS.LOGGER;
using CLVSPOS.MODELS;
using System;
using System.Net.Http;
using System.Web.Http;

namespace CLVSPOS.API.Controllers
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
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetTaxes());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (TaxesResponse)LogManager.HandleExceptionWithReturn(exc, "TaxesResponse", "api/Tax/GetTaxes", (int)Constants.LogTypes.API)); 
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/Tax/SyncGetTaxes")]
        [ActionName("SyncGetTaxes")]
        public HttpResponseMessage SyncGetTaxes(string userId)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.SyncGetTaxes(userId));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (TaxesResponse)LogManager.HandleExceptionWithReturn(exc, "TaxesResponse", "api/Tax/SyncGetTaxes", (int)Constants.LogTypes.API));
            }
        }
    }
}

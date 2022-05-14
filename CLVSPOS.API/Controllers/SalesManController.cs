using CLVSPOS.COMMON;
using CLVSPOS.LOGGER;
using CLVSPOS.MODELS;
using System;
using System.Net.Http;
using System.Web.Http;

namespace CLVSPOS.API.Controllers
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
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetSalesMan());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (SalesManResponse)LogManager.HandleExceptionWithReturn(exc, "SalesManResponse", "api/SalesMan/GetSalesMan", (int)Constants.LogTypes.API));
            }
        }
        [Authorize]
        [HttpGet]
        [Route("api/SalesMan/GetSalesManBalance")]
        [ActionName("GetSalesManBalance")]
        public HttpResponseMessage GetSalesManBalance()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetSalesManBalance());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (SalesManResponse)LogManager.HandleExceptionWithReturn(exc, "SalesManResponse", "api/SalesMan/GetSalesMan", (int)Constants.LogTypes.API));
            }
        }
        [Authorize]
        [HttpGet]
        [Route("api/SalesMan/SyncGetSalesMan")]
        [ActionName("SyncGetSalesMan")]
        public HttpResponseMessage SyncGetSalesMan(string userId)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.SyncGetSalesMan(userId));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (SalesManResponse)LogManager.HandleExceptionWithReturn(exc, "SalesManResponse", "api/SalesMan/SyncGetSalesMan", (int)Constants.LogTypes.API));
            }
        }

    }
}

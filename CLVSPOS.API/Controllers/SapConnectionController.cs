using CLVSPOS.COMMON;
using CLVSPOS.LOGGER;
using CLVSPOS.MODELS;
using System;
using System.Net.Http;
using System.Web.Http;


namespace CLVSPOS.API.Controllers
{
    public class SapConnectionController : ApiController
    {
        /// <summary>
        /// metodo para obtener las conexciones de SAP
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/SapConnection/GetSapConnection")]
        [ActionName("GetSapConnection")]
        public HttpResponseMessage GetSapConnection()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetSapConnection());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (SapConnectionResponse)LogManager.HandleExceptionWithReturn(exc, "SapConnectionResponse", "api/SapConnection/GetSapConnection", (int)Constants.LogTypes.API)); 
            }
        }


        [Authorize]
        [HttpGet]
        [Route("api/SapConnection/SyncGetSapConnections")]
        [ActionName("SyncGetSapConnections")]
        public HttpResponseMessage SyncGetSapConnections()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.SyncGetSapConnections());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (SapConnectionResponse)LogManager.HandleExceptionWithReturn(exc, "SapConnectionResponse", "api/SapConnection/SyncGetSapConnections", (int)Constants.LogTypes.API));
            }
        }

    }
}

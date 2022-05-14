using CLVSSUPER.COMMON;
using CLVSSUPER.LOGGER;
using CLVSSUPER.MODELS;
using System;
using System.Net.Http;
using System.Web.Http;


namespace CLVSSUPER.API.Controllers
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
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSSUPER.PROCESS.Process.GetSapConnection());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (SapConnectionResponse)LogManager.HandleExceptionWithReturn(exc, "SapConnectionResponse", "api/SapConnection/GetSapConnection", (int)Constants.LogTypes.API)); 
            }
        }

    }
}

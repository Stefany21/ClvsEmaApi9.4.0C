using CLVSPOS.COMMON;
using CLVSPOS.LOGGER;
using CLVSPOS.MODELS;
using System;
using System.Net.Http;
using System.Web.Http;

namespace CLVSPOS.API.Controllers
{
    public class CardsController : ApiController
    {
        /// <summary>
        /// trae las listas de las cuentas
        /// no recibe parametros
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Cards/GetCards")]
        [ActionName("GetCards")]
        public HttpResponseMessage GetCards()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetCards());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (CardsResponse)LogManager.HandleExceptionWithReturn(exc, "CardsResponse", "api/Cards/GetCards", (int)Constants.LogTypes.API));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/Cards/SyncGetCards")]
        [ActionName("SyncGetCards")]
        public HttpResponseMessage SyncGetCards(string userId)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.SyncGetCards(userId));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (CardsResponse)LogManager.HandleExceptionWithReturn(exc, "CardsResponse", "api/Cards/SyncGetCards", (int)Constants.LogTypes.API));
            }
        }
    }
}

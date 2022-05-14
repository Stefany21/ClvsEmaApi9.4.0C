using CLVSSUPER.COMMON;
using CLVSSUPER.LOGGER;
using CLVSSUPER.MODELS;
using System;
using System.Net.Http;
using System.Web.Http;

namespace CLVSSUPER.API.Controllers
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
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSSUPER.PROCESS.Process.GetCards());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (CardsResponse)LogManager.HandleExceptionWithReturn(exc, "CardsResponse", "api/Cards/GetCards", (int)Constants.LogTypes.API));
            }
        }
    }
}

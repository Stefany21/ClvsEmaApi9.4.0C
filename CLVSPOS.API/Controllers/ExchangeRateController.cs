using CLVSPOS.COMMON;
using CLVSPOS.LOGGER;
using CLVSPOS.MODELS;
using System;
using System.Net.Http;
using System.Web.Http;

namespace CLVSPOS.API.Controllers
{
    public class ExchangeRateController : ApiController
    {
        /// <summary>
        /// obtiene el tipo de cambio diario, en caso de error envia un modelo
        /// de error espesificando por que fue incorrecto ya sea por error de datos o por una execption.
        /// </summary>
        /// <returns></returns>
        // [Authorize]
        [HttpGet]
        [Route("api/ExchangeRate/GetExchangeRate")]
        [ActionName("GetExchangeRate")]
        public HttpResponseMessage GetExchangeRate()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetExchangeRate());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (ExchangeRateResponse)LogManager.HandleExceptionWithReturn(exc, "ExchangeRateResponse", "api/ExchangeRate/GetExchangeRate", (int)Constants.LogTypes.API)); // 
            }
        }

        /// <summary>
        /// obtiene el tipo de cambio diario, para ser sincronizado localmente
        /// </summary>
        /// <returns></returns>
        // [Authorize]
        [HttpGet]
        [Route("api/ExchangeRate/SyncGetExchangeRate")]
        [ActionName("SyncGetExchangeRate")]
        public HttpResponseMessage SyncGetExchangeRate(string userId)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.SyncGetExchangeRate(userId));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncGetExchangeRate", "api/ExchangeRate/SyncGetExchangeRate", (int)Constants.LogTypes.API)); // 
            }
        }
    }
}

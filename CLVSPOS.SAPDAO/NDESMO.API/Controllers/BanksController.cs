using CLVSSUPER.COMMON;
using CLVSSUPER.LOGGER;
using CLVSSUPER.MODELS;
using System;
using System.Net.Http;
using System.Web.Http;

namespace CLVSSUPER.API.Controllers
{
    public class BanksController : ApiController
    {
        /// <summary>
        /// obtiene la cuenta cuentas de los bancos
        /// no recibe parametros
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Banks/GetAccountsBank")]
        [ActionName("GetAccountsBank")]
        public HttpResponseMessage GetAccountsBank()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSSUPER.PROCESS.Process.GetAccountsBank());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (BankResponse)LogManager.HandleExceptionWithReturn(exc, "BankResponse", "api/Banks/GetAccountsBank", (int)Constants.LogTypes.API));
            }
        }
    }
}

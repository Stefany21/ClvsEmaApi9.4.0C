using System;
using System.Net.Http;
using System.Web.Http;
using CLVSSUPER.MODELS;
using CLVSSUPER.COMMON;
using CLVSSUPER.LOGGER;

namespace CLVSSUPER.API.Controllers
{
    public class BusinessPartnersController : ApiController
    {
        /// <summary>
        /// obtiene la lista total de todos BPS con los que trabaja la empresa, en caso de error envia un modelo de error especificando que fue lo que paso
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/BusinessPartners/GetBusinessPartners")]
        [ActionName("GetBusinessPartners")]
        public HttpResponseMessage GetBusinessPartner()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSSUPER.PROCESS.Process.GetBusinessPartners());
                
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (BaseResponse)LogManager.HandleExceptionWithReturn(exc, "BPSResponseModel", "api/BusinessPartners/GetBusinessPartners", (int)Constants.LogTypes.API));
            }
        }
    }
}


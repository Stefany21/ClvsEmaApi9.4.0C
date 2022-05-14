using CLVSSUPER.COMMON;
using CLVSSUPER.LOGGER;
using CLVSSUPER.MODELS;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace CLVSSUPER.API.Controllers
{
    public class ParamController : ApiController
    {
        /// <summary>
        /// retorna la vista de las parametrizaciones segun la vista
        /// lleva como parametro el numero de vista para los parametros
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Param/GetViewParam")]
        [ActionName("GetViewParam")]
        public HttpResponseMessage GetViewParam(int view)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSSUPER.PROCESS.Process.GetViewParam(view));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (ParamsViewResponse)LogManager.HandleExceptionWithReturn(exc, "ParamsViewResponse", "api/Param/GetViewParam", (int)Constants.LogTypes.API));                
            }
        }

        /// <summary>
        /// retorna la vista de las parametrizaciones segun la vista
        /// lleva como parametro el numero de vista para los parametros
        /// </summary>
        /// <param name="paramsModel"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Param/UpdateParamsViewState")]
        [ActionName("UpdateParamsViewState")]
        public HttpResponseMessage UpdateParamsViewState(List<ParamsModel> paramsModel)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSSUPER.PROCESS.Process.UpdateParamsViewState(paramsModel));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Param/UpdateParamsViewState", (int)Constants.LogTypes.API));                
            }
        }
    }
}

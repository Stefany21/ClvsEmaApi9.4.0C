using CLVSPOS.COMMON;
using CLVSPOS.LOGGER;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using CLVSPOS.MODELS;
using CLVSSUPER.MODELS;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Script.Serialization;


namespace CLVSPOS.API.Controllers
{
    public class SettingsController : ApiController
    {

        /// <summary>
        /// Obtiene configuraciones de campos de las vistas
        /// </summary>
        /// <returns></returns>
        /// [Authorize]
        [HttpGet]
        [Route("api/Settings/GetViewSettings")]
        [ActionName("GetViewSettings")]
        public HttpResponseMessage GetViewSettings()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.GetViewSettings());

            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Settings/GetViewSettings", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// Obtiene configuración de vista de acuerdo al codigo especificado por el parametro
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Settings/GetViewSettingbyId")]
        [ActionName("GetViewSettings")]
        public HttpResponseMessage GetViewSettingbyId(int Code)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.GetViewSettingbyId(Code));

            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Settings/GetViewSettingbyId", (int)Constants.LogTypes.API));
            }
        }
        /// <summary>
        /// Crea, actualiza configuracion de una vista 
        /// </summary>
        /// <param name="Settings"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Settings/SaveSettings")]
        public HttpResponseMessage SaveSettings(Settings Settings)
        {
            try
            {
              

                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.SaveSettings(Settings));

            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Settings/SaveSettings", (int)Constants.LogTypes.API));
            }
        }
     


    }
}
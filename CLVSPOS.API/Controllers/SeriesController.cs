using CLVSPOS.COMMON;
using CLVSPOS.LOGGER;
using CLVSPOS.MODELS;
using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace CLVSPOS.API.Controllers
{
    public class SeriesController : ApiController
    {
        /// <summary>
        /// obtiene la lista total del nombre de todos los items con los que trabaja la empresa
        /// en caso de error envia un modelo de error especificando que fue lo que paso
        /// no recibe parametros
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Series/GetSeries")]
        [ActionName("GetSeries")]
        public HttpResponseMessage GetSeries()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetSeries());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (NumberingSeriesModelResponse)LogManager.HandleExceptionWithReturn(exc, "NumberingSeriesModelResponse", "api/Series/GetSeries", (int)Constants.LogTypes.API)); 
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/Series/SyncGetSeries")]
        [ActionName("SyncGetSeries")]
        public HttpResponseMessage SyncGetSeries()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.SyncGetSeries());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (NumberingSeriesModelResponse)LogManager.HandleExceptionWithReturn(exc, "NumberingSeriesModelResponse", "api/Series/SyncGetSeries", (int)Constants.LogTypes.API));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/Series/SyncGetSeriesByUsers")]
        [ActionName("SyncGetSeriesByUsers")]
        public HttpResponseMessage SyncGetSeriesByUsers()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.SyncGetSeriesByUsers());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (NumberingSeriesModelResponse)LogManager.HandleExceptionWithReturn(exc, "NumberingSeriesModelResponse", "api/Series/SyncGetSeries", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// obtiene la lista de las enumeraciones con los tipos de series que hay ejemplo, facturacion - cotizacion - pagos
        /// no recive parametros
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Series/GetSeriesType")]
        [ActionName("GetSeriesType")]
        public HttpResponseMessage GetSeriesType()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetSeriesType());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (enumsResponse)LogManager.HandleExceptionWithReturn(exc, "enumsResponse", "api/Series/GetSeriesType", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// obtiene los tipos de serie de numeracion que hay  ejemplo, manual - automatico
        /// no recive parametros
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Series/GetSeriesTypeNumber")]
        [ActionName("GetSeriesTypeNumber")]
        public HttpResponseMessage GetSeriesTypeNumber()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetSeriesTypeNumber());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (enumsResponse)LogManager.HandleExceptionWithReturn(exc, "enumsResponse", "api/Series/GetSeriesTypeNumber", (int)Constants.LogTypes.API));               
            }
        }

        /// <summary>
        /// actualiza los cambios cuando se modifica una serie
        /// recibe como parametro el modelo de la serie
        /// </summary>
        /// <param name="serie"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Series/UpdateSerie")]
        [ActionName("UpdateSerie")]
        public HttpResponseMessage UpdateSerie(NumberingSeriesModel serie)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.UpdateSerie(serie));
                }
                else
                {
                    var modelToString = new JavaScriptSerializer().Serialize(serie);
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(new Exception(), string.Empty,
                                                                         string.Format("api/Series/UpdateSerie-- Objeto recibido: {0}", modelToString),
                                                                         (int)Constants.LogTypes.API, true));                    
                }
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty,"api/Series/UpdateSerie", (int)Constants.LogTypes.API));                
            }
        }

        /// <summary>
        /// Crea una nueva serie en la bd
        /// recibe como parametro el modelo de la serie
        /// </summary>
        /// <param name="serie"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Series/CreateNewSerie")]
        [ActionName("CreateNewSerie")]
        public HttpResponseMessage CreateNewSerie(NumberingSeriesModel serie)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.CreateNewSerie(serie));
                }
                else
                {
                    var modelToString = new JavaScriptSerializer().Serialize(serie);
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(new Exception(), string.Empty,
                                                                         string.Format("api/Series/CreateNewSerie-- Objeto recibido: {0}", modelToString),
                                                                         (int)Constants.LogTypes.API, true));                    
                }
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Series/CreateNewSerie", (int)Constants.LogTypes.API));                
            }
        }
    }
}

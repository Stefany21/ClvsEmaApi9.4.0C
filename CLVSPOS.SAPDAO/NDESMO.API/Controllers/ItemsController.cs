using CLVSSUPER.COMMON;
using CLVSSUPER.LOGGER;
using CLVSSUPER.MODELS;
using System;
using System.Net.Http;
using System.Web.Http;

namespace CLVSSUPER.API.Controllers
{
    public class ItemsController : ApiController
    {
        /// <summary>
        /// obtiene la lista total del nombre de todos los items con los que trabaja la empresa
        /// en caso de error envia un modelo de error especificando que fue lo que paso
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Items/GetItemNames")]
        [ActionName("GetItemNames")]
        public HttpResponseMessage GetItemNames()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSSUPER.PROCESS.Process.GetItemNames());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (ItemNamesResponse)LogManager.HandleExceptionWithReturn(exc, "ItemNamesResponse", "api/Items/GetItemNames", (int)Constants.LogTypes.API)); 
            }
        }

        /// <summary>
        /// obtienen la informacion general de un items especifico, en caso de error envia un modelo
        /// de error especificando porque fue incorrecto ya sea por error de datos o por una excepcion.
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="priceList"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Items/GetInfoItem")]
        [ActionName("GetInfoItem")]
        public HttpResponseMessage GetInfoItem(string itemCode, int priceList)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSSUPER.PROCESS.Process.GetInfoItem(itemCode, priceList));
                }
                else
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (ItemsResponse)LogManager.HandleExceptionWithReturn(new Exception(), "ItemsResponse",
                                                                         string.Format("api/Items/GetInfoItem-- Objeto recibido ItemCode: {0}", itemCode),
                                                                         (int)Constants.LogTypes.API,true));           
                }
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (ItemsResponse)LogManager.HandleExceptionWithReturn(exc, "ItemsResponse", "api/Items/GetInfoItem", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// funcion para realizar la peticion de informacion en SAP, de la disponibilidad de items en los almacenes
        /// recibe como parametro el codigo de item
        /// </summary>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Items/GetWHAvailableItem")]
        [ActionName("GetWHAvailableItem")]
        public HttpResponseMessage GetWHAvailableItem(string itemCode)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSSUPER.PROCESS.Process.GetWHAvailableItem(itemCode));
                }
                else
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (WHInfoResponse)LogManager.HandleExceptionWithReturn(new Exception(), "WHInfoResponse",
                                                                         string.Format("api/Items/GetWHAvailableItem-- Objeto recibido ItemCode:", itemCode),
                                                                         (int)Constants.LogTypes.API, true));
                }
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (WHInfoResponse)LogManager.HandleExceptionWithReturn(exc, "WHInfoResponse", "api/Items/GetWHAvailableItem", (int)Constants.LogTypes.API));  // 
            }
        }

        /// <summary>
        /// funcion para realizar la peticion de informacion en SAP, de la disponibilidad de items en los almacenes
        /// recibe como parametro el codigo de item
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="whsCode"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Items/GetSeriesByItem")]
        [ActionName("GetSeriesByItem")]
        public HttpResponseMessage GetSeriesByItem(string itemCode, string whsCode)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSSUPER.PROCESS.Process.GetSeriesByItem(itemCode, whsCode));
                }
                else
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (WHInfoResponse)LogManager.HandleExceptionWithReturn(new Exception(), "WHInfoResponse",
                                                                         string.Format("api/Items/GetSeriesByItem-- Objeto recibido ItemCode: {0}, WhsCode: {1}", itemCode, whsCode),
                                                                         (int)Constants.LogTypes.API, true));              
                }
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (WHInfoResponse)LogManager.HandleExceptionWithReturn(exc, "WHInfoResponse", "api/Items/GetSeriesByItem", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// funcion para realizar la peticion de una lista de las listas de precios
        /// no recibe parametros
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Items/GetPriceList")]
        [ActionName("GetPriceList")]
        public HttpResponseMessage GetPriceList()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSSUPER.PROCESS.Process.GetPriceList());

            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (WHInfoResponse)LogManager.HandleExceptionWithReturn(exc, "WHInfoResponse", "api/Items/GetPriceList", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// funcion para realizar la peticion de una lista de los terminos de pagos.
        /// no recibe parametros
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Items/GetPayTermsList")]
        [ActionName("GetPayTermsList")]
        public HttpResponseMessage GetPayTermsList()
        {
            try
            {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSSUPER.PROCESS.Process.GetPayTermsList());

            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (BaseResponse)LogManager.HandleExceptionWithReturn(exc, "WHInfoResponse", "api/Items/GetPayTermsList", (int)Constants.LogTypes.API));
            }
        }
    }
}

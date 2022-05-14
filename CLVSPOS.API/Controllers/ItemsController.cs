using CLVSPOS.COMMON;
using CLVSPOS.LOGGER;
using CLVSPOS.MODELS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace CLVSPOS.API.Controllers
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
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetItemNames());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (ItemNamesResponse)LogManager.HandleExceptionWithReturn(exc, "ItemNamesResponse", "api/Items/GetItemNames", (int)Constants.LogTypes.API));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/Items/SyncGetItems")]
        [ActionName("SyncGetItems")]
        public HttpResponseMessage SyncGetItems(string userId)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.SyncGetItems(userId));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (ItemNamesResponse)LogManager.HandleExceptionWithReturn(exc, "ItemNamesResponse", "api/Items/SyncGetItems", (int)Constants.LogTypes.API));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/Items/SyncGetPriceGroupList")]
        [ActionName("SyncGetPriceGroupList")]
        public HttpResponseMessage SyncGetPriceGroupList(string userId)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.SyncGetPriceGroupList(userId));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "ItemNamesResponse", "api/Items/SyncGetPriceGroupList", (int)Constants.LogTypes.API));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/Items/SyncGetFirms")]
        [ActionName("SyncGetFirms")]
        public HttpResponseMessage SyncGetFirms(string userId)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.SyncGetFirms(userId));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "ItemNamesResponse", "api/Items/SyncGetFirms", (int)Constants.LogTypes.API));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/Items/SyncGetOCRD")]
        [ActionName("SyncGetOCRD")]
        public HttpResponseMessage SyncGetOCRD(string userId)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.SyncGetOCRD(userId));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse", "api/Items/SyncGetOCRD", (int)Constants.LogTypes.API));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/Items/SyncGetOTCX")]
        [ActionName("SyncGetOTCX")]
        public HttpResponseMessage SyncGetOTCX(string userId)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.SyncGetOTCX(userId));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (ItemNamesResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse", "api/Items/SyncGetOTCX", (int)Constants.LogTypes.API));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/Items/SyncGetOSTA")]
        [ActionName("SyncGetOSTA")]
        public HttpResponseMessage SyncGetOSTA(string userId)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.SyncGetOSTA(userId));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (ItemNamesResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse", "api/Items/SyncGetOSTA", (int)Constants.LogTypes.API));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/Items/SyncGetOITM")]
        [ActionName("SyncGetOITM")]
        public HttpResponseMessage SyncGetOITM(string userId)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.SyncGetOITM(userId));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse", "api/Items/SyncGetOITM", (int)Constants.LogTypes.API));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/Items/SyncGetITM1")]
        [ActionName("SyncGetITM1")]
        public HttpResponseMessage SyncGetITM1(string userId)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.SyncGetITM1(userId));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse", "api/Items/SyncGetITM1", (int)Constants.LogTypes.API));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/Items/SyncGetOWHS")]
        [ActionName("SyncGetOWHS")]
        public HttpResponseMessage SyncGetOWHS(string userId)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.SyncGetOWHS(userId));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse", "api/Items/SyncGetOWHS", (int)Constants.LogTypes.API));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/Items/SyncGetOITW")]
        [ActionName("SyncGetOITW")]
        public HttpResponseMessage SyncGetOITW(string userId)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.SyncGetOITW(userId));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse", "api/Items/SyncGetOITW", (int)Constants.LogTypes.API));
            }
        }



        /// <summary>
        ///  Metodo para obtener la informacion de un item de la compania
        /// Recibe como parametro el codigo, lista de precio, cardcode, alamacen del item a consultar
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="priceList"></param>
        /// <param name="cardCode"></param>
        /// <param name="whCode"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Items/GetInfoItem")]
        [ActionName("GetInfoItem")]
        public HttpResponseMessage GetInfoItem(string itemCode, int priceList, string cardCode,string whCode, string documentType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetInfoItem(itemCode, priceList, cardCode,whCode, documentType));
                }
                else
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (ItemsResponse)LogManager.HandleExceptionWithReturn(new Exception(), "ItemsResponse",
                                                                         string.Format("api/Items/GetInfoItem-- Objeto recibido ItemCode: {0}", itemCode),
                                                                         (int)Constants.LogTypes.API, true));
                }
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (ItemsResponse)LogManager.HandleExceptionWithReturn(exc, "ItemsResponse", "api/Items/GetInfoItem", (int)Constants.LogTypes.API));
            }
        }


        //----#001 - 07/09/2021---
        /// <summary>
        /// obtienen la informacion del avgprice de un item que se mostrara en la ventana modal de ajuste de inventario en facturacion.
        /// </summary>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Items/GetItemAVGPrice")]
        [ActionName("GetItemAVGPrice")]
        public HttpResponseMessage GetItemAVGPrice(string itemCode)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.GetItemAVGPrice(itemCode));
                }
                else
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (ItemsResponse)LogManager.HandleExceptionWithReturn(new Exception(), "ItemsResponse",
                                                                         string.Format("api/Items/GetItemAVGPrice-- Objeto recibido ItemCode: {0}", itemCode),
                                                                         (int)Constants.LogTypes.API, true));
                }
            }
            catch (Exception exc)
            {
                string name = exc.TargetSite.DeclaringType.FullName + "." + exc.TargetSite.Name;
                int code = exc.InnerException != null ? exc.InnerException.InnerException != null ? exc.InnerException.InnerException.HResult : exc.InnerException.HResult : exc.HResult;
                string message = exc.InnerException != null ? exc.InnerException.InnerException != null ? exc.InnerException.InnerException.Message : exc.InnerException.Message : exc.Message;
                string err = message;
                message = $"{message} On {name}";

                LogManager.LogMessage($"api/Items/GetItemAVGPrice | Catch: {code} - {message} | ItemCode = {itemCode}", (int)Constants.LogTypes.API);

                return Request.CreateResponse(System.Net.HttpStatusCode.OK, new ApiResponse<double>()
                {
                    Result = false,
                    Error = new ErrorInfo()
                    {
                        Code = code,
                        Message = err
                    }
                });
            }
        }

        /// <summary>
        /// obtienen el precio de la ultima entrada de un item que se mostrara en la ventana modal de ajuste de inventario en facturacion.
        /// </summary>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Items/GetItemLastPurchagePrice")]
        [ActionName("GetItemLastPurchagePrice")]
        public HttpResponseMessage GetItemLastPurchagePrice(string itemCode)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.GetItemLastPurchagePrice(itemCode));
                }
                else
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (ItemsResponse)LogManager.HandleExceptionWithReturn(new Exception(), "ItemsResponse",
                                                                         string.Format("api/Items/GetItemLastPurchagePrice-- Objeto recibido ItemCode: {0}", itemCode),
                                                                         (int)Constants.LogTypes.API, true));
                }
            }
            catch (Exception exc)
            {
                string name = exc.TargetSite.DeclaringType.FullName + "." + exc.TargetSite.Name;
                int code = exc.InnerException != null ? exc.InnerException.InnerException != null ? exc.InnerException.InnerException.HResult : exc.InnerException.HResult : exc.HResult;
                string message = exc.InnerException != null ? exc.InnerException.InnerException != null ? exc.InnerException.InnerException.Message : exc.InnerException.Message : exc.Message;
                string err = message;
                message = $"{message} On {name}";

                LogManager.LogMessage($"api/Items/GetItemLastPurchagePrice | Catch: {code} - {message} | ItemCode = {itemCode}", (int)Constants.LogTypes.API);

                return Request.CreateResponse(System.Net.HttpStatusCode.OK, new ApiResponse<double>()
                {
                    Result = false,
                    Error = new ErrorInfo()
                    {
                        Code = code,
                        Message = err
                    }
                });
            }
        }




        //----#001 - 08/09/2021---

        [Authorize]
        [HttpPost]
        [Route("api/Items/GetItemDataForGoodReceiptInvoice")]
        [ActionName("GetItemDataForGoodReceiptInvoice")]
        public HttpResponseMessage GetItemDataForGoodReceiptInvoice(string WhsCode,List<string> itemCodes)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.GetItemDataForGoodReceiptInvoice(itemCodes,WhsCode));
            }
            catch (Exception exc)
            {
                string name = exc.TargetSite.DeclaringType.FullName + "." + exc.TargetSite.Name;
                int code = exc.InnerException != null ? exc.InnerException.InnerException != null ? exc.InnerException.InnerException.HResult : exc.InnerException.HResult : exc.HResult;
                string message = exc.InnerException != null ? exc.InnerException.InnerException != null ? exc.InnerException.InnerException.Message : exc.InnerException.Message : exc.Message;
                string err = message;
                message = $"{message} On {name}";

                LogManager.LogMessage($"api/Items/GetItemDataForGoodReceiptInvoice | Catch: {code} - {message} | ItemCodes = {JsonConvert.SerializeObject(itemCodes)}", (int)Constants.LogTypes.API);

                return Request.CreateResponse(System.Net.HttpStatusCode.OK, new ApiResponse<double>()
                {
                    Result = false,
                    Error = new ErrorInfo()
                    {
                        Code = code,
                        Message = err
                    }
                });
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
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetWHAvailableItem(itemCode));
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
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetSeriesByItem(itemCode, whsCode));
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
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetPriceList());

            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (WHInfoResponse)LogManager.HandleExceptionWithReturn(exc, "WHInfoResponse", "api/Items/GetPriceList", (int)Constants.LogTypes.API));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/Items/GetDefaultPriceList")]
        [ActionName("GetDefaultPriceList")]
        public HttpResponseMessage GetDefaultPriceList(string cardCode)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetDefaultPriceList(cardCode));

            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (WHInfoResponse)LogManager.HandleExceptionWithReturn(exc, "WHInfoResponse", "api/Items/GetPriceList", (int)Constants.LogTypes.API));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/Items/SyncGetPriceList")]
        [ActionName("SyncGetPriceList")]
        public HttpResponseMessage SyncGetPriceList(string userId)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.SyncGetPriceList(userId));

            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse", "api/Items/SyncGetPriceList", (int)Constants.LogTypes.API));
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
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetPayTermsList());

            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (BaseResponse)LogManager.HandleExceptionWithReturn(exc, "WHInfoResponse", "api/Items/GetPayTermsList", (int)Constants.LogTypes.API));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/Items/SyncGetPayTermsList")]
        [ActionName("SyncGetPayTermsList")]
        public HttpResponseMessage SyncGetPayTermsList(string userId)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.SyncGetPayTermsList(userId));

            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (BaseResponse)LogManager.HandleExceptionWithReturn(exc, "BaseResponse", "api/Items/SyncGetPayTermsList", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// Permite la creacion de un item usando su propio model y un codigo consecutivo
        /// </summary>
        /// <param name="_itemModel"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Items/Create")]
        [ActionName("Create")]
        public HttpResponseMessage CreateItem(ItemsModel _itemModel)
        {

            try
            {
                string parsedObject = new JavaScriptSerializer().Serialize(_itemModel);

                LogManager.LogMessage(string.Format("api/Items/Create. Start Time: {0}", DateTime.Now), (int)Constants.LogTypes.STOCK);

                LogManager.LogMessage(string.Format("Recived Object: {0}", parsedObject), (int)Constants.LogTypes.STOCK);

                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.CreateItem(_itemModel));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (BaseResponse)LogManager.HandleExceptionWithReturn(exc, "BaseResponse", "api/Items/Create", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// Actualiza el item mediante su propio model
        /// </summary>
        /// <param name="_itemModel"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Items/UpdateItem")]
        [ActionName("UpdateItem")]
        public HttpResponseMessage Update(ItemsModel _itemModel)
        {
            try
            {
                string parsedObject = new JavaScriptSerializer().Serialize(_itemModel);

                LogManager.LogMessage(string.Format("api/Items/UpdateItem. Start Time: {0}", DateTime.Now), (int)Constants.LogTypes.STOCK);

                LogManager.LogMessage(string.Format("Recived Object: {0}", parsedObject), (int)Constants.LogTypes.STOCK);

                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.UpdateItem(_itemModel));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (BaseResponse)LogManager.HandleExceptionWithReturn(exc, "BaseResponse", "api/Items/UpdateItem", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// Obtiene la lista de precios que tiene un item
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Items/GetItemPriceList")]
        [ActionName("GetItemPriceList")]
        public HttpResponseMessage GetItemPriceList(string _itemCode)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetItemPriceList(_itemCode));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (BaseResponse)LogManager.HandleExceptionWithReturn(exc, "BaseResponse", "api/Items/SyncGetPayTermsList", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// Obtiene código de barras de ítem consultado por parámetro 
        /// </summary>
        /// <param name="_itemCode"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Items/GetBarcodesByItem")]
        [ActionName("GetBarcodesByItem")]
        public HttpResponseMessage GetBarcodesByItem(string _itemCode)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetBarcodesByItem(_itemCode));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (ItemsResponse)LogManager.HandleExceptionWithReturn(exc, "ItemsResponse", "api/Items/SyncGetPayTermsList", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// <summary>
        /// Obtener todas las lista de Precios de Sap
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Items/GetAllPriceList")]
        [ActionName("GetAllPriceList")]
        public HttpResponseMessage GetAllPriceList()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetAllPriceList());

            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (WHInfoResponse)LogManager.HandleExceptionWithReturn(exc, "WHInfoResponse", "api/Items/GetPriceList", (int)Constants.LogTypes.API));
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/Items/GetItemChangePrice")]
        [ActionName("GetItemChangePrice")]
        public HttpResponseMessage GetItemChangePrice(ItemsChangePriceModel itemModel)
        {
            try
            {
                string parsedObject = new JavaScriptSerializer().Serialize(itemModel);

                LogManager.LogMessage(string.Format("api/Items/GetItemChangePrice. Start Time: {0}", DateTime.Now), (int)Constants.LogTypes.STOCK);

                LogManager.LogMessage(string.Format("Recived Object: {0}", parsedObject), (int)Constants.LogTypes.STOCK);

                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetItemChangePrice(itemModel));
                }
                else
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (ItemsResponse)LogManager.HandleExceptionWithReturn(new Exception(), "ItemsResponse",
                                                                         string.Format("api/Items/GetInfoItem-- Objeto recibido ItemCode: {0}"),
                                                                         (int)Constants.LogTypes.API, true));
                }
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (ItemsResponse)LogManager.HandleExceptionWithReturn(exc, "ItemsResponse", "api/Items/GetInfoItem", (int)Constants.LogTypes.API));
            }
        }


        /// <summary>
        /// Metodo para obtener detalles de un articulo y un numero de entradas  
        /// </summary>
        /// <param name="ItemCode"></param>
        /// <param name="NumeroEntradas"></param>
        /// <param name="DocType"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Items/GetItemDetail")]
        [ActionName("GetItemDetail")]
        public HttpResponseMessage GetItemDetail(String ItemCode, int NumeroEntradas, int DocType)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.GetItemDetails(ItemCode, NumeroEntradas, DocType));

            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (ItemsResponse)LogManager.HandleExceptionWithReturn(exc, "ItemsResponse", "api/Items/GetItemDetail", (int)Constants.LogTypes.API));
            }
        }
    }
}

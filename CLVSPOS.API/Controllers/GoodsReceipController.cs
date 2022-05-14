using CLVSPOS.COMMON;
using CLVSPOS.LOGGER;
using CLVSPOS.MODELS;
using CLVSSUPER.MODELS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace CLVSPOS.API.Controllers
{
    public class GoodsReceipController : ApiController
    {
        /// <summary>
        /// Genera la entrada de inventario
        /// </summary>
        /// <param name="goodsReceipt"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/GoodsReceipt/CreateGoodsReceipt")]
        [ActionName("CreateGoodsReceipt")]
        public HttpResponseMessage CreateGoodsReceipt(GoodsReceipt goodsReceipt)
        {
            try
            {
                string parsedObject = new JavaScriptSerializer().Serialize(goodsReceipt);

                if (ModelState.IsValid)
                {
                    LogManager.LogMessage(string.Format("api/GoodsReceipt/CreateGoodsReceipt. Start Time: {0}", DateTime.Now), (int)Constants.LogTypes.STOCK);

                    LogManager.LogMessage(string.Format("Recived Object: {0}", parsedObject), (int)Constants.LogTypes.STOCK);

                    ItemsResponse oItemsResponse = PROCESS.Process.CreateGoodsReceipt(goodsReceipt);

                    LogManager.LogMessage(string.Format("api/GoodsReceipt/CreateGoodsReceipt. End Time: {0}", DateTime.Now), (int)Constants.LogTypes.STOCK);

                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, oItemsResponse);
                }
                else
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest,
                                                 (InvoicesListResp)LogManager.HandleExceptionWithReturn(new Exception(), "ItemsResponse",
                                                   string.Format("api/GoodsReceipt/CreateGoodsReceipt-- Invalid Object: {0}", parsedObject),
                                                   (int)Constants.LogTypes.API, true));
                }
            }
            catch (Exception ex)
            {
                string name = ex.TargetSite.DeclaringType.FullName + "." + ex.TargetSite.Name;
                int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
                string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;

                message = $"{message} On {name}";

                LogManager.LogMessage($"api/GoodsReceipt/CreateGoodsReceipt | Catch: {code} - {message} | Model: {JsonConvert.SerializeObject(goodsReceipt)}", (int)Constants.LogTypes.API);

                return Request.CreateResponse(HttpStatusCode.OK,
                   new BaseResponse
                   {
                       Result = false,
                       Error = new ErrorInfo
                       {
                           Code = code,
                           Message = message
                       }
                   });


            }
        }

        /// <summary>
        /// Genera la entrada de inventario
        /// </summary>
        /// <param name="_goodsRecipt"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/GoodsReceipt/CreateGoodsReceiptReturn")]
        [ActionName("CreateGoodsReceiptReturn")]
        public HttpResponseMessage CreateGoodsReceiptReturn(GoodsReceipt _goodsRecipt)
        {
            try
            {
                string parsedObject = new JavaScriptSerializer().Serialize(_goodsRecipt);

                if (ModelState.IsValid)
                {
                    LogManager.LogMessage(string.Format("api/GoodsReceipt/CreateGoodsReceipt. Start Time: {0}", DateTime.Now), (int)Constants.LogTypes.STOCK);

                    LogManager.LogMessage(string.Format("Recived Object: {0}", parsedObject), (int)Constants.LogTypes.STOCK);
                    
                    ItemsResponse oItemsResponse = PROCESS.Process.CreateGoodsReceiptReturn(_goodsRecipt);

                    LogManager.LogMessage(string.Format("api/GoodsReceipt/CreateGoodsReceipt. End Time: {0}", DateTime.Now), (int)Constants.LogTypes.STOCK);

                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, oItemsResponse);
                }
                else
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest,
                                                 (InvoicesListResp)LogManager.HandleExceptionWithReturn(new Exception(), "ItemsResponse",
                                                   string.Format("api/GoodsReceipt/CreateGoodsReceiptReturn-- Invalid Object: {0}", parsedObject),
                                                   (int)Constants.LogTypes.API, true));
                }
            }
            catch (Exception ex)
            {
                string name = ex.TargetSite.DeclaringType.FullName + "." + ex.TargetSite.Name;
                int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
                string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;

                message = $"{message} On {name}";

                LogManager.LogMessage($"api/GoodsReceipt/CreateGoodsReceiptReturn | Catch: {code} - {message} | Model: {JsonConvert.SerializeObject(_goodsRecipt)}", (int)Constants.LogTypes.API);

                return Request.CreateResponse(HttpStatusCode.OK,
                   new BaseResponse
                   {
                       Result = false,
                       Error = new ErrorInfo
                       {
                           Code = code,
                           Message = message
                       }
                   });
            }
        }


        /// <summary>
        /// Crea Entrada de Mercaderia
        /// </summary>
        /// <param name="goodsReceipt"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/GoodsReceipt/CreateGoodsReceiptStock")]
        [ActionName("CreateGoodsReceiptStock")]
        public HttpResponseMessage CreateGoodsReceiptStock(GoodsReceipt goodsReceipt)
        {
            try
            {
                string parsedObject = new JavaScriptSerializer().Serialize(goodsReceipt);

                if (ModelState.IsValid)
                {
                    LogManager.LogMessage(string.Format("api/GoodsReceipt/CreateGoodsReceipt. Start Time: {0}", DateTime.Now), (int)Constants.LogTypes.STOCK);

                    LogManager.LogMessage(string.Format("Recived Object: {0}", parsedObject), (int)Constants.LogTypes.STOCK);

                    ItemsResponse oItemsResponse = PROCESS.Process.CreateGoodsReceiptStock(goodsReceipt);

                    LogManager.LogMessage(string.Format("api/GoodsReceipt/CreateGoodsReceipt. End Time: {0}", DateTime.Now), (int)Constants.LogTypes.STOCK);

                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, oItemsResponse);
                }
                else
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest,
                                                 (InvoicesListResp)LogManager.HandleExceptionWithReturn(new Exception(), "ItemsResponse",
                                                   string.Format("api/GoodsReceipt/CreateGoodsReceiptStock-- Invalid Object: {0}", parsedObject),
                                                   (int)Constants.LogTypes.API, true));
                }
            }
            catch (Exception ex)
            {
                string name = ex.TargetSite.DeclaringType.FullName + "." + ex.TargetSite.Name;
                int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
                string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;

                message = $"{message} On {name}";

                LogManager.LogMessage($"api/GoodsReceipt/CreateGoodsReceiptStock | Catch: {code} - {message} | Model: {JsonConvert.SerializeObject(goodsReceipt)}", (int)Constants.LogTypes.API);

                return Request.CreateResponse(HttpStatusCode.OK,
                   new BaseResponse
                   {
                       Result = false,
                       Error = new ErrorInfo
                       {
                           Code = code,
                           Message = message
                       }
                   });
            }
        }

        /// <summary>
        /// Genera la salida de inventario
        /// </summary>
        /// <param name="goodsIssue"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/GoodsReceipt/CreateGoodsIssueStock")]
        [ActionName("CreateGoodsIssueStock")]
        public HttpResponseMessage CreateGoodsIssueStock(GoodsReceipt goodsIssue)
        {
            try
            {
                string parsedObject = new JavaScriptSerializer().Serialize(goodsIssue);

                if (ModelState.IsValid)
                {
                    LogManager.LogMessage(string.Format("api/GoodsReceipt/CreateGoodsReceipt. Start Time: {0}", DateTime.Now), (int)Constants.LogTypes.STOCK);

                    LogManager.LogMessage(string.Format("Recived Object: {0}", parsedObject), (int)Constants.LogTypes.STOCK);

                    ItemsResponse oItemsResponse = PROCESS.Process.CreateGoodsIssueStock(goodsIssue);

                    LogManager.LogMessage(string.Format("api/GoodsReceipt/CreateGoodsReceipt. End Time: {0}", DateTime.Now), (int)Constants.LogTypes.STOCK);

                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, oItemsResponse);
                }
                else
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest,
                                                 (InvoicesListResp)LogManager.HandleExceptionWithReturn(new Exception(), "ItemsResponse",
                                                   string.Format("api/GoodsReceipt/CreateGoodsIssueStock-- Invalid Object: {0}", parsedObject),
                                                   (int)Constants.LogTypes.API, true));
                }
            }
            catch (Exception ex)
            {
                string name = ex.TargetSite.DeclaringType.FullName + "." + ex.TargetSite.Name;
                int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
                string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;

                message = $"{message} On {name}";

                LogManager.LogMessage($"api/GoodsReceipt/CreateGoodsIssueStock | Catch: {code} - {message} | Model: {JsonConvert.SerializeObject(goodsIssue)}", (int)Constants.LogTypes.API);

                return Request.CreateResponse(HttpStatusCode.OK,
                   new BaseResponse
                   {
                       Result = false,
                       Error = new ErrorInfo
                       {
                           Code = code,
                           Message = message
                       }
                   });
            }
        }

        /// <summary>
        ///Entrada mercaderia desde xml, obtiene archivo xml y retorna modelo GoodsReceipt con la lista de lineas del documento xml
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/GoodsReceipt/CreateGoodsReciptXml")]
        [ActionName("CreateGoodsReciptXml")]       
        public HttpResponseMessage CreateGoodsReciptXml()
        {
            try
            {
                HttpRequest oHttpRequest = HttpContext.Current.Request;
                return Request.CreateResponse(HttpStatusCode.OK, PROCESS.Process.CreateGoodsReciptXml(oHttpRequest));
            }
            catch (Exception ex)
            {
                string END_POINT = Request?.RequestUri?.AbsolutePath;
                string QUERY = Request?.RequestUri?.Query;

                string name = ex.TargetSite.DeclaringType.FullName + "." + ex.TargetSite.Name;
                int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
                string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;


                message = $"{message} On {name}";

                LogManager.LogMessage($"{END_POINT}{QUERY} | Catch: {code} - {message}", (int)Constants.LogTypes.API);

                return Request.CreateResponse(HttpStatusCode.OK, new BaseResponse()
                {
                    Result = false,
                    Error = new ErrorInfo()
                    {
                        Code = code,
                        Message = message
                    }
                });
            }
        }

       
    }
}

using System;
using System.Net.Http;
using System.Web.Http;
using CLVSPOS.MODELS;
using CLVSPOS.COMMON;
using CLVSPOS.LOGGER;
using CLVSSUPER.MODELS;
using System.Web.Script.Serialization;

namespace CLVSPOS.API.Controllers
{
    public class PurchaseOrderController : ApiController
    {
        [Authorize]
        [HttpPost]
        [Route("api/PurchaseOrder/CreatePurchaseOrder")]
        [ActionName("CreatePurchaseOrder")]
        public HttpResponseMessage CreatePurchaseOrder(PurchaseOrderModel _purchaseOrder)
        {          
           try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.CreatePurchaseOrder(_purchaseOrder));

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

                return Request.CreateResponse(System.Net.HttpStatusCode.OK, new BaseResponse()
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

        [Authorize]
        [HttpPost]
        [Route("api/PurchaseOrder/UpdatePurchaseOrder")]
        [ActionName("UpdatePurchaseOrder")]
        public HttpResponseMessage UpdatePurchaseOrder(PurchaseOrderModel _purchaseOrder)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.UpdatePurchaseOrder(_purchaseOrder));

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

                return Request.CreateResponse(System.Net.HttpStatusCode.OK, new BaseResponse()
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

        [Authorize]
        [HttpPost]
        [Route("api/PurchaseOrder/GetPurchaseOrderList")]
        [ActionName("GetPurchaseOrderList")]
        public HttpResponseMessage GetPurchaseOrderList(PurchaseOrderSearchModel _purchaseOrder)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetPurchaseOrderList(_purchaseOrder));

            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (BaseResponse)LogManager.HandleExceptionWithReturn(exc, "PurchaserOrderResponse", "api/PurchaseOrder/GetPurchaseOrderList", (int)Constants.LogTypes.API));
            }
        }
        [Authorize]
        [HttpGet]
        [Route("api/PurchaseOrder/GetPurchaseOrder")]
        [ActionName("GetPurchaseOrder")]
        public HttpResponseMessage GetPurchaseOrder(int _docNum)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetPurchaseOrder(_docNum));

            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (BaseResponse)LogManager.HandleExceptionWithReturn(exc, "PurchaserOrderResponse", "api/PurchaseOrder/GetPurchaseOrder", (int)Constants.LogTypes.API));
            }
        }

    }
}
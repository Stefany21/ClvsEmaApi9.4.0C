using System;
using System.Net.Http;
using System.Web.Http;
using CLVSPOS.MODELS;
using System.Web.Http.ModelBinding;
using CLVSPOS.COMMON;
using CLVSPOS.LOGGER;
using System.Web.Script.Serialization;
using System.Web;
using CLVSSUPER.MODELS;

namespace CLVSPOS.API.Controllers
{
    public class PaymentController : ApiController
    {
        /// <summary>
        /// trae las listas de todos los pagos que se deben hacer sobre las facturas
        /// recibe como parametro el cardcode del cliente y la sede
        /// </summary>
        /// <param name="cardCode"></param>
        /// <param name="sede"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Payment/GetPayInvoices")]
        [ActionName("GetPayInvoices")]
        public HttpResponseMessage GetPayInvoices(string cardCode, string sede, string currency)
        {
            try
            {
                // GetDataFact gd = new GetDataFact(dbName);
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.GetPayInvoices(cardCode, sede, currency));
                }
                else
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                                 (InvoicesListResp)LogManager.HandleExceptionWithReturn(new Exception(), "InvoicesListResp",
                                                   string.Format("api/Payment/GetPayInvoices-- Objeto recibido CardCode: {0}, Sede: {1}, Currency: {2}", cardCode, sede, currency),
                                                   (int)Constants.LogTypes.API, true));                    
                }
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (InvoicesListResp)LogManager.HandleExceptionWithReturn(exc, "InvoicesListResp", "api/Payment/GetPayInvoices", (int)Constants.LogTypes.API));                
            }
        }

        /// <summary>
        /// Obtener Fcaturas proveedores
        /// </summary>
        /// <param name="cardCode"></param>
        /// <param name="sede"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Payment/GetPayApInvoices")]
        [ActionName("GetPayApInvoices")]
        public HttpResponseMessage GetPayApInvoices(string cardCode, string sede, string currency)
        {
            try
            {               
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.GetPayApInvoices(cardCode, sede, currency));
                }
                else
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                                 (InvoicesListResp)LogManager.HandleExceptionWithReturn(new Exception(), "InvoicesListResp",
                                                  string.Format("api/Payment/GetPayApInvoices-- Objeto recibido CardCode: {0}, Sede: {1}, Currency: {2}", cardCode, sede, currency),
                                                  (int)Constants.LogTypes.API, true));
                }
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (InvoicesListResp)LogManager.HandleExceptionWithReturn(exc, "InvoicesListResp", "api/Payment/GetPayApInvoices", (int)Constants.LogTypes.API));
            }
        }


        /// <summary>
        /// envia la informacion de la cancelacion de las facturas para cancelar la misma en SAP
        /// recibe como parametro el cardcode del cliente y la sede
        /// </summary>
        /// <param name="canPay"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Payment/CancelPayment")]
        [ActionName("CancelPayment")]
        public HttpResponseMessage CancelPayment(CancelPayModel canPay)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.CancelPayment(canPay));
                }
                else
                {
                    var modelToString = new JavaScriptSerializer().Serialize(canPay);
                    LogManager.LogMessage("api/Payment/CancelPayment-- Objeto recibido: " + modelToString, (int)Constants.LogTypes.API);

                    string errors = string.Empty;
                    foreach (ModelState modelState in ModelState.Values)
                    {
                        foreach (ModelError error in modelState.Errors)
                        {
                            errors += error.ErrorMessage + ' ';
                        }
                    }
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, new BaseResponse
                    {
                        Result = false,
                        Error = new ErrorInfo
                        {
                            Code = -1,
                            Message = errors
                        }
                    });
                }
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Payment/CancelPayment", (int)Constants.LogTypes.API));                
            }
        }

        /// <summary>
        /// trae las listas de todos los pagos que se deven hacer sobre las facturas
        /// recibe como parametro el cardcode del cliente y la sede
        /// </summary>
        /// <param name="infoSearch"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Payment/GetPaymentList")]
        [ActionName("GetPaymentList")]
        public HttpResponseMessage GetPaymentList(paymentSearchModel infoSearch)
            {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.GetPaymentList(infoSearch));
                }
                else
                {
                    var modelToString = new JavaScriptSerializer().Serialize(infoSearch);
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (InvoicesListResp)LogManager.HandleExceptionWithReturn(new Exception(), "InvoicesListResp",
                                                                         string.Format("api/Payment/GetPaymentList-- Objeto recibido: {0}", modelToString),
                                                                         (int)Constants.LogTypes.API, true));                  
                }
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (InvoicesListResp)LogManager.HandleExceptionWithReturn(exc, "InvoicesListResp", "api/Payment/GetPaymentList", (int)Constants.LogTypes.API));  // InvoicesListResp
            }
        }





        [Authorize]
        [HttpGet]
        [Route("api/Payment/SyncGetODPI")]
        [ActionName("SyncGetODPI")]
        public HttpResponseMessage SyncGetODPI(string userId)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.SyncGetODPI(userId));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse", "api/Payment/SyncGetODPI", (int)Constants.LogTypes.API));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/Payment/SyncGetINV6")]
        [ActionName("SyncGetINV6")]
        public HttpResponseMessage SyncGetINV6(string userId)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.SyncGetINV6(userId));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse", "api/Payment/SyncGetINV6", (int)Constants.LogTypes.API));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/Payment/SyncGetDPI6")]
        [ActionName("SyncGetDPI6")]
        public HttpResponseMessage SyncGetDPI6(string userId)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.SyncGetDPI6(userId));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse", "api/Payment/SyncGetDPI6", (int)Constants.LogTypes.API));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/Payment/SyncGetOINV")]
        [ActionName("SyncGetOINV")]
        public HttpResponseMessage SyncGetOINV(string userId)
        {
            try
            {               
                //filter = HttpUtility.UrlDecode(filter);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.SyncGetOINV(userId, string.Empty));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse", "api/Payment/SyncGetOINV", (int)Constants.LogTypes.API));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/Payment/SyncGetORCT")]
        [ActionName("SyncGetORCTs")]
        public HttpResponseMessage SyncGetORCT(string userId)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.SyncGetORCT(userId));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse", "api/Payment/SyncGetORCT", (int)Constants.LogTypes.API));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/Payment/SyncGetRCT2")]
        [ActionName("SyncGetRCT2")]
        public HttpResponseMessage SyncGetRCT2(string userId)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.SyncGetRCT2(userId));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse", "api/Payment/SyncGetRCT2", (int)Constants.LogTypes.API));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/Payment/SyncGetINV1")]
        [ActionName("SyncGetINV1")]
        public HttpResponseMessage SyncGetINV1(string userId)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.SyncGetINV1(userId));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse", "api/Payment/SyncGetINV1", (int)Constants.LogTypes.API));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/Payment/SyncGetORDR")]
        [ActionName("SyncGetORDR")]
        public HttpResponseMessage SyncGetORDR(string userId)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.SyncGetORDR(userId));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse", "api/Payment/SyncGetORDR", (int)Constants.LogTypes.API));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/Payment/SyncGetOQUT")]
        [ActionName("SyncGetOQUT")]
        public HttpResponseMessage SyncGetOQUT(string userId)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.SyncGetOQUT(userId));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse", "api/Payment/SyncGetOQUT", (int)Constants.LogTypes.API));
            }
        }
        /// <summary>
        /// End point para obtener el detalle de pago de una factura
        /// </summary>
        /// <param name="_docEntry"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Payment/GetInvoicePaymentDetail")]
        [ActionName("GetInvoicePaymentDetail")]
        public HttpResponseMessage GetInvoicePaymentDetail(int _docEntry)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.GetInvoicePaymentDetail(_docEntry));
                }
                else
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                                 (InvoicePaymentDetailResponse)LogManager.HandleExceptionWithReturn(new Exception(), "InvoicesListResp",
                                                   string.Format("api/Payment/GetInvoicePaymentDetail-- Objeto recibido Invoice DocEntry: {0}", _docEntry),
                                                   (int)Constants.LogTypes.API, true));
                }
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (InvoicePaymentDetailResponse)LogManager.HandleExceptionWithReturn(exc, "InvoicesListResp", "api/Payment/GetInvoicePaymentDetail", (int)Constants.LogTypes.API));
            }
        }


        /// <summary>
        /// End point para obtener el detalle de pago de una factura
        /// </summary>
        /// <param name="_docEntry"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Payment/GetTransactionsByDocEntry")]
        [ActionName("GetTransactionsByDocEntry")]
        public HttpResponseMessage GetTransactionsByDocEntry(int _docEntry)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.GetTransactionsByDocEntryOpened(_docEntry));
                }
                else
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                                 (InvoicePaymentDetailResponse)LogManager.HandleExceptionWithReturn(new Exception(), "InvoicesListResp",
                                                   string.Format("api/Payment/GetTransactionsByDocEntry-- Objeto recibido Invoice DocEntry: {0}", _docEntry),
                                                   (int)Constants.LogTypes.API, true));
                }
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
        /// <summary>
        /// End point para obtener el detalle de pago de una factura
        /// </summary>
        /// <param name="_docEntry"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Payment/CommitCanceledCard")]
        [ActionName("CommitCanceledCard")]
        public HttpResponseMessage CommitCanceledCard(SelfPPTransaction _selfPPTransaction)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.CommitCanceledCard(_selfPPTransaction));
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
        [Route("api/Payment/GetPPTransactionCenceledStatus")]
        [ActionName("GetPPTransactionCenceledStatus")]
        public HttpResponseMessage GetPPTransactionCenceledStatus(PPTransactionsCanceledPrintSearch doc)
        {
            try
            {
                LogManager.LogMessage(string.Format("api/Payment/GetPPTransactionCenceledStatus"), (int)Constants.LogTypes.API);

                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.GetPPTransactionCenceledStatus(doc));
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
        [HttpGet]
        [Route("api/Payment/GetPPTransactionByDocumentKey")]
        [ActionName("GetPPTransactionByDocumentKey")]
        public HttpResponseMessage GetPPTransactionByDocumentKey(string _documentKey)
        {
            try
            {
                LogManager.LogMessage(string.Format("api/Payment/GetPPTransactionByDocumentKey"), (int)Constants.LogTypes.API);

                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.GetPPTransactionsByDocumentKey(_documentKey));
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
        [HttpGet]
        [Route("api/Payment/GetPPTransactionByInvoiceNumber")]
        [ActionName("GetPPTransactionByInvoiceNumber")]
        public HttpResponseMessage GetPPTransactionByInvoiceNumber(string _documentKey)
        {
            try
            {
                LogManager.LogMessage(string.Format("api/Payment/GetPPTransactionByInvoiceNumber"), (int)Constants.LogTypes.API);

                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.GetPPTransactionByInvoiceNumber(_documentKey));
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


















    }
}

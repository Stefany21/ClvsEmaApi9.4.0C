using CLVSPOS.COMMON;
using CLVSPOS.LOGGER;
using CLVSPOS.MODELS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Script.Serialization;

namespace CLVSPOS.API.Controllers
{
    public class DocumentController : ApiController
    {
        #region
        private string GetModelStateErrors(ModelStateDictionary modelState)
        {
            string error = string.Empty;

            List<ModelState> ModelStateList = modelState.Values.ToList(); //#REVISAR# modelDic no es un nombre representativo de la variable
            var modelErr = ModelStateList.Where(x => x.Errors.Count > 0).Select(x => x.Errors).FirstOrDefault();

            error = modelErr.Select(x => x).FirstOrDefault().ErrorMessage + modelErr.Select(x => x.Exception).FirstOrDefault();

            return error;
        }
        /// <summary>
        /// Crear Factura Proveedor
        /// </summary>
        /// <param name="CreateapInvoice"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Documents/CreateapInvoice")]
        [ActionName("CreateapInvoice")]
        public HttpResponseMessage CreateapInvoice(CreateInvoice CreateapInvoice)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.CreateapInvoice(CreateapInvoice));
                }
                else
                {
                    var modelToString = new JavaScriptSerializer().Serialize(CreateapInvoice);
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(new Exception(), string.Empty,
                                              string.Format("api/Documents/CreateapInvoice-- Objeto recibido: {0}", modelToString),
                                              (int)Constants.LogTypes.API, true));
                }
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Users/UpdateUser", (int)Constants.LogTypes.API));
            }
        }



        /// <summary>
        ///  Metodo para Realizar el pago de una factura en SAP
        /// </summary>
        /// <param name="_payment"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Documents/CreatePaymentRecived")]
        [ActionName("CreatePaymentRecived")]
        public async Task<HttpResponseMessage> CreatePaymentRecived(CreateSLRecivedPaymentModel _payment)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, await PROCESS.Process.CreatePaymentRecived(_payment));
                }
                else
                {
                    var modelToString = new JavaScriptSerializer().Serialize(_payment);
                    LogManager.LogMessage(string.Format("api/Documents/CreatePayInvoices-- Objeto recibido Payment: {0}, accountPayment: {1}", modelToString), (int)Constants.LogTypes.API);

                    throw new Exception(this.GetModelStateErrors(ModelState));
                }
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Documents/CreatePayInvoices", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// Metodo para obtener lista de facturas de SAP para reimprecion 
        /// Recibe como parametro el modelo del pago y el docEntry de la factura
        /// </summary>
        /// <param name="inv"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Documents/GetInvPrintList")]
        [ActionName("GetInvPrintList")]
        public HttpResponseMessage GetInvPrintList(invPrintSearch inv)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetInvPrintList(inv));
                }
                else
                {
                    var modelToString = new JavaScriptSerializer().Serialize(inv);
                    LogManager.LogMessage(string.Format("api/Documents/GetInvPrintList-- Objeto recibido: {0}", modelToString), (int)Constants.LogTypes.API);

                    throw new Exception(this.GetModelStateErrors(ModelState));
                }
            }

            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Documents/GetInvPrintList", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// Crea factura a sap
        /// </summary>
        /// <param name="createInvoice"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Documents/CreateInvoice")]
        [ActionName("CreateInvoice")]
        public async Task<HttpResponseMessage> CreateInvoice(CreateSlInvoice createInvoice)
        {
            try
            {

                string modelToString = new JavaScriptSerializer().Serialize(createInvoice);

                if (ModelState.IsValid)
                {
                    LogManager.LogMessage(string.Format("Documents/CreateInvoice. InvId {0}", createInvoice.Invoice.U_CLVS_POS_UniqueInvId), (int)Constants.LogTypes.API);

                    HttpResponseMessage response = Request.CreateResponse(System.Net.HttpStatusCode.OK, await PROCESS.Process.CreateInvoice(createInvoice));

                    return response;
                }
                else
                {

                    LogManager.LogMessage(string.Format("api/Documents/CreateInvoice-- Objeto recibido: {0}", modelToString), (int)Constants.LogTypes.API);

                    string errors = string.Empty;

                    foreach (ModelState modelState in ModelState.Values)
                    {
                        foreach (ModelError error in modelState.Errors)
                        {
                            errors += string.IsNullOrEmpty(error.ErrorMessage) ? error.Exception.Message : error.ErrorMessage;
                        }
                    }
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, new InvoiceSapResponse
                    {
                        Result = false,
                        Error = new ErrorInfo
                        {
                            Message = errors
                        }
                    });
                }
            }

            catch (Exception exc)
            {
                string name = exc.TargetSite.DeclaringType.FullName + "." + exc.TargetSite.Name;
                int code = exc.InnerException != null ? exc.InnerException.InnerException != null ? exc.InnerException.InnerException.HResult : exc.InnerException.HResult : exc.HResult;
                string message = exc.InnerException != null ? exc.InnerException.InnerException != null ? exc.InnerException.InnerException.Message : exc.InnerException.Message : exc.Message;
                string err = message;
                message = $"{message} On {name}";
                LogManager.LogMessage($"api/Documents/CreateInvoice | Catch: {code} - {message} | Model: {JsonConvert.SerializeObject(createInvoice)}", (int)Constants.LogTypes.API);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, new InvoiceSapResponse()
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
        /// Crea nota de credito a sap
        /// </summary>
        /// <param name="createInvoice"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Documents/CreateInvoiceNc")]
        [ActionName("CreateInvoiceNc")]
        public HttpResponseMessage CreateInvoiceNc(CreateInvoice createInvoice)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    LogManager.LogMessage(string.Format("Controller>CreateNC. InvId {0}", createInvoice.Invoice.CLVS_POS_UniqueInvId), (int)Constants.LogTypes.API);
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.CreateInvoiceNc(createInvoice));
                }
                else
                {
                    var modelToString = new JavaScriptSerializer().Serialize(createInvoice);
                    LogManager.LogMessage(string.Format("api/Documents/CreateInvoice-- Objeto recibido: {0}", modelToString), (int)Constants.LogTypes.API);

                    string errors = string.Empty;

                    foreach (ModelState modelState in ModelState.Values)
                    {
                        foreach (ModelError error in modelState.Errors)
                        {
                            errors += string.IsNullOrEmpty(error.ErrorMessage) ? error.Exception.Message : error.ErrorMessage;
                        }
                    }
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, new InvoiceSapResponse
                    {
                        Result = false,
                        Error = new ErrorInfo
                        {
                            Message = errors
                        }
                    });
                }
            }

            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (InvoiceSapResponse)LogManager.HandleExceptionWithReturn(exc, "InvoiceSapResponse", "api/Documents/CreateInvoice", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// Metodo para obtener lista de facturas de SAP para reimprecion
        /// Recibe como parametro el modelo del pago y el docEntry de la factura
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Documents/getDiscount")]
        [ActionName("getDiscount")]
        public HttpResponseMessage getDiscount()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetDiscount());
            }

            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (InvoiceSapResponse)LogManager.HandleExceptionWithReturn(exc, "InvoiceSapResponse", "api/Documents/getDiscount", (int)Constants.LogTypes.API)); // InvoiceSapResponse
            }
        }

        //cierre de caja detallado
        [Authorize]
        [HttpPost]
        [Route("api/Documents/GetBalanceInvoices_UsrOrTime")]
        [ActionName("GetBalanceInvoices_UsrOrTime")]
        //Trae la informacion del balance detallado de la empresa, en caso de error envia un modelo
        //de error especificando por que fue incorrecto ya sea por error de datos o por una execption.
        public HttpResponseMessage GetBalanceInvoices_UsrOrTime(GetBalanceModel_UsrOrDate BalanceModel)
        {
            try
            {
                // GetDataFact gd = new GetDataFact(dbName);
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.GetBalanceInvoices_UsrOrTime(BalanceModel));
                }
                else
                {
                    string errors = string.Empty;
                    foreach (ModelState modelState in ModelState.Values)
                    {
                        foreach (ModelError error in modelState.Errors)
                        {
                            errors += string.IsNullOrEmpty(error.ErrorMessage) ? error.Exception.Message : error.ErrorMessage;
                        }
                    }
                    throw new Exception(errors);
                }
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (BalanceByUserResponse)LogManager.HandleExceptionWithReturn(exc, "BalanceByUserResponse", "api/Documents/GetBalanceInvoices_UsrOrTime", (int)Constants.LogTypes.API)); // InvoiceSapResponse
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/Documents/SyncCreateInvoice")]
        [ActionName("SyncCreateInvoice")]
        public HttpResponseMessage SyncCreateInvoice(OFF_CreateInvoice createInvoice)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (PROCESS.Process.IsLocalInvoiceIdValid(createInvoice.Invoice.U_CLVS_POS_UniqueInvId))
                    {
                        return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.SyncCreateInvoice(createInvoice));
                    }
                    else
                    {
                        var error = string.Format("Discarded. Local/Offline Invoice with Id: {0}, was already processed", createInvoice.Invoice.Id.ToString());

                        return Request.CreateResponse(System.Net.HttpStatusCode.OK, new InvoiceSapResponse
                        {
                            Result = false,
                            Error = new ErrorInfo
                            {
                                Message = error
                            }
                        });

                    }
                }
                else
                {
                    var modelToString = new JavaScriptSerializer().Serialize(createInvoice);
                    LogManager.LogMessage(string.Format("api/Documents/SyncCreateInvoice-- Objeto recibido: {0}", modelToString), (int)Constants.LogTypes.API);

                    string errors = string.Empty;

                    foreach (ModelState modelState in ModelState.Values)
                    {
                        foreach (ModelError error in modelState.Errors)
                        {
                            errors += string.IsNullOrEmpty(error.ErrorMessage) ? error.Exception.Message : error.ErrorMessage;
                        }
                    }
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, new InvoiceSapResponse
                    {
                        Result = false,
                        Error = new ErrorInfo
                        {
                            Message = errors
                        }
                    });
                }
            }

            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (InvoiceSapResponse)LogManager.HandleExceptionWithReturn(exc, "InvoiceSapResponse", "api/Documents/CreateInvoice", (int)Constants.LogTypes.API));
            }
        }


        /// <summary>
        /// Efectuar pago a Proveedores
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Documents/CreatePayApInvoices")]
        [ActionName("CreatePayApInvoices")]
        public async Task<HttpResponseMessage> CreatePayApInvoices(CreateSLRecivedPaymentModel payment)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, await PROCESS.Process.CreatePayApInvoices(payment));
                }
                else
                {
                    var modelToString = new JavaScriptSerializer().Serialize(payment);
                    LogManager.LogMessage(string.Format("api/Documents/CreatePayInvoices-- Objeto recibido Payment: {0}", modelToString), (int)Constants.LogTypes.API);

                    throw new Exception(this.GetModelStateErrors(ModelState));
                }
            }

            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Documents/CreatePayInvoices", (int)Constants.LogTypes.API));
            }
        }
        #endregion

        #region COTIZACIONES
        /// <summary>
        /// Metodo para crear una cotizacion en SAP
        /// Recibe como parametro el modelo de la cotizacion
        /// </summary>
        /// <param name="quotation"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Documents/CreateQuotation")]
        [ActionName("CreateQuotation")]
        public async Task<HttpResponseMessage> CreateQuotation(IQuotDocument quotation)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, await CLVSPOS.PROCESS.Process.CreateQuotation(quotation));

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
        /// Obtener lista Ofertas de Venta
        /// </summary>
        /// <param name="quotationSearch"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Documents/GetQuotations")]
        [ActionName("GetQuotations")]
        public HttpResponseMessage GetQuotations(quotationSearch quotationSearch)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetQuotations(quotationSearch));
                }
                else
                {
                    LogManager.LogMessage($"api/Documents/GetQuotations-- Invalid Model | Objeto recibido quotationSearch: {JsonConvert.SerializeObject(quotationSearch)}", (int)Constants.LogTypes.API);

                    throw new Exception(this.GetModelStateErrors(ModelState));
                }
            }

            catch (Exception ex)
            {
                string name = ex.TargetSite.DeclaringType.FullName + "." + ex.TargetSite.Name;
                int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
                string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;

                message = $"{message} On {name}";

                LogManager.LogMessage($"api/Documents/GetQuotations | Catch: {code} - {message} | model: {JsonConvert.SerializeObject(quotationSearch)}", (int)Constants.LogTypes.API);

                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
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
        /// Obtiene la info de la Oferta de venta
        /// </summary>
        /// <param name="DocEntry"></param>
        /// <param name="allLines"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Documents/GetQuotation")]
        [ActionName("GetQuotation")]
        public HttpResponseMessage GetQuotation(int DocEntry, bool allLines = true)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetQuotationEdit(DocEntry, allLines));
                }
                else
                {
                    LogManager.LogMessage($"api/Documents/GetQuotation--Invalid model | Objetos recibidos: [DocEntry: {DocEntry} - AllLInes: {allLines}]", (int)Constants.LogTypes.API);

                    throw new Exception(this.GetModelStateErrors(ModelState));
                }
            }

            catch (Exception ex)
            {
                string name = ex.TargetSite.DeclaringType.FullName + "." + ex.TargetSite.Name;
                int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
                string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;

                message = $"{message} On {name}";

                LogManager.LogMessage($"api/Documents/GetQuotation | Catch: {code} - {message} | model: [DocEntry: {DocEntry} - AllLInes: {allLines}]", (int)Constants.LogTypes.API);

                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
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


        //------------------------------------------------ Update Quotation by DIAPI


        /// <summary>
        /// Metodo para crear una cotizacion en SAP
        /// Recibe como parametro el modelo de la cotizacion
        /// </summary>
        /// <param name="quotation"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Documents/UpdateQuotationDIAPI")]
        [ActionName("UpdateQuotationByDIAPI")]
        public HttpResponseMessage UpdateQuotationByDIAPI(IQuotDocument quotationEdit)
        {

            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.UpdateQuotationDIAPI(quotationEdit));

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




        //--------------------------------------- End Edit quotation by DIAPI


        //---------------------------------- Edit Sales Order By DIAPI

        /// <summary>
        /// Obtiene la info de la Oferta de venta
        /// </summary>
        /// <param name="DocEntry"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Documents/UpdateSaleOrderDIAPI")]
        [ActionName("UpdateSaleOrderDIAPI")]
        public HttpResponseMessage UpdateSaleOrderDIAPI(ISaleDocument saleOrder)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.UpdateSaleOrderDIAPI(saleOrder));

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














        //---------------------------------- END Edit Sales Order By DIAPI













        /// <summary>
        /// Metodo para crear una cotizacion en SAP
        /// </summary>
        /// <param name="quotationEdit"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Documents/UpdateQuotation")]
        [ActionName("UpdateQuotation")]
        public async Task<HttpResponseMessage> UpdateQuotation(IQuotDocument quotationEdit)
        {

            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, await CLVSPOS.PROCESS.Process.UpdateQuotation(quotationEdit));

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
        #endregion

        #region ORDENES DE VENTA

        /// <summary>
        /// Metodo para crear un SaleOrder
        /// Metodo para enviar las ventas a SAP
        /// Recibe como parametro el modelo del CreateSaleOrderModel
        /// </summary>
        /// <param name="saleOrder"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Documents/CreateSaleOrder")]
        [ActionName("CreateSaleOrder")]
        public async Task<HttpResponseMessage> CreateSaleOrder(ISaleDocument saleOrder)
        {

            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, await CLVSPOS.PROCESS.Process.CreateSaleOrder(saleOrder));

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
        /// Obtener Ordenes de Venta
        /// </summary>
        /// <param name="saleOrderSearch"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Documents/GetSaleOrders")]
        [ActionName("GetSaleOrders")]
        public HttpResponseMessage GetSaleOrders(saleOrderSearch saleOrderSearch)
        {           
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetSaleOrders(saleOrderSearch));
                }
                else  
                {                   
                    throw new Exception(this.GetModelStateErrors(ModelState));
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
        /// Obtiene la info de la Oferta de venta
        /// </summary>
        /// <param name="DocEntry"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Documents/GetSaleOrder")]
        [ActionName("GetSaleOrder")]
        public HttpResponseMessage GetSaleOrder(int DocEntry)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetSaleOrder(DocEntry));
                }
                else
                {
                    var modelToString = new JavaScriptSerializer().Serialize(DocEntry);
                    LogManager.LogMessage(string.Format("api/Documents/QuotationEdit-- Objeto recibido QuotationEdit: {0}", modelToString), (int)Constants.LogTypes.API);

                    throw new Exception(this.GetModelStateErrors(ModelState));
                }
            }

            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Documents/GetSaleOrder", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// Actualizar orden de venta a sap
        /// </summary>
        /// <param name="saleOrder"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Documents/UpdateSaleOrder")]
        [ActionName("UpdateSaleOrder")]
        public async Task<HttpResponseMessage> UpdateSaleOrder(ISaleDocument saleOrder)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, await CLVSPOS.PROCESS.Process.UpdateSaleOrder(saleOrder));

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
        #endregion

        #region Invoice
        /// <summary>
        /// Obtiene tipos de documentos configuradas en vista
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Documents/GetInvoiceTypes")]
        [ActionName("GetInvoiceTypes")]
        public HttpResponseMessage GetInvoiceTypes()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetInvoiceTypes());
            }

            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Documents/GetInvoiceTypes", (int)Constants.LogTypes.API));
            }
        }
        #endregion
        /// <summary>
        /// Sync tipo de documentos 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Documents/SyncGetInvoiceTypes")]
        [ActionName("SyncGetInvoiceTypes")]
        public HttpResponseMessage SyncGetInvoiceTypes()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.SyncGetInvoiceTypes());

            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse", "api/Documents/SyncGetInvoiceTypes", (int)Constants.LogTypes.API));
            }
        }

    }
}

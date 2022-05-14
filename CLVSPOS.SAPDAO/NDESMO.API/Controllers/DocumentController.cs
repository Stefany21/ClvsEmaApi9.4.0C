using CLVSSUPER.COMMON;
using CLVSSUPER.LOGGER;
using CLVSSUPER.MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Script.Serialization;

namespace CLVSSUPER.API.Controllers
{
    public class DocumentController : ApiController
    {
        private string GetModelStateErrors(ModelStateDictionary modelState)
        {
            string error = string.Empty;

            List<ModelState> ModelStateList = modelState.Values.ToList(); //#REVISAR# modelDic no es un nombre representativo de la variable
            var modelErr = ModelStateList.Where(x => x.Errors.Count > 0).Select(x => x.Errors).FirstOrDefault();

            error = modelErr.Select(x => x).FirstOrDefault().ErrorMessage + modelErr.Select(x => x.Exception).FirstOrDefault();

            return error;
        }

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
        public HttpResponseMessage CreateSaleOrder(SalesOrderModel saleOrder)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //Quotation.DocType = 13; Revicion con Randy
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSSUPER.PROCESS.Process.CreateSaleOrder(saleOrder));
                }
                else
                {
                    var modelToString = new JavaScriptSerializer().Serialize(saleOrder);
                    LogManager.LogMessage(string.Format("api/Documents/CreateSaleOrder-- Objeto recibido: ", modelToString), (int)Constants.LogTypes.API);
                   
                    throw new Exception(this.GetModelStateErrors(ModelState));
                }
            }

            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (SalesOrderToSAPResponse)LogManager.HandleExceptionWithReturn(exc, "SalesOrderToSAPResponse", "api/Documents/CreateSaleOrder", (int)Constants.LogTypes.API));
            }
        }

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
        public HttpResponseMessage CreateQuotation(QuotationsModel quotation)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSSUPER.PROCESS.Process.CreateQuotation(quotation));
                }
                else
                {
                    var modelToString = new JavaScriptSerializer().Serialize(quotation);
                    LogManager.LogMessage(string.Format("api/Documents/CreateQuotation-- Objeto recibido: {0}", modelToString), (int)Constants.LogTypes.API);

                    throw new Exception(this.GetModelStateErrors(ModelState));                    
                }
            }

            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (QuotationsToSAPResponse)LogManager.HandleExceptionWithReturn(exc, "QuotationsToSAPResponse", "api/Documents/CreateQuotation", (int)Constants.LogTypes.API)); 
            }
        }

        /// <summary>
        /// Metodo para Realizar el pago de una factura en SAP
        /// Recibe como parametro el modelo del pago y el docEntry de la factura
        /// </summary>
        /// <param name="payment"></param>
        /// <param name="accountPayment"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Documents/CreatePayInvoices")]
        [ActionName("CreatePayInvoices")]
        public HttpResponseMessage CreatePayInvoices(CreatePaymentModel payment, bool accountPayment)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSSUPER.PROCESS.Process.CancelPayInvoices(payment));
                }
                else
                {
                    var modelToString = new JavaScriptSerializer().Serialize(payment);
                    LogManager.LogMessage(string.Format("api/Documents/CreatePayInvoices-- Objeto recibido Payment: {0}, accountPayment: {1}",  modelToString, accountPayment), (int)Constants.LogTypes.API);

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
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSSUPER.PROCESS.Process.GetInvPrintList(inv));
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
        /// Metodo para crear factura
        /// </summary>
        /// <param name="createInvoice"></param>
        /// <returns></returns>
        /// <remarks>
        /// lleva los datos recopilados durante el proseso de recoleccion de datos de la factura 
        /// para la creacion de esta, en caso de error envia un modelo
        /// de error espesificando por que fue incorrecto ya sea por error de datos o por una execption.
        /// </remarks>
        [HttpPost]
        [Route("api/Documents/CreateInvoice")]
        [ActionName("CreateInvoice")]
        public HttpResponseMessage CreateInvoice(CreateInvoice createInvoice)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSSUPER.PROCESS.Process.CreateInvoice(createInvoice));
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
                        result = false,
                        errorInfo = new ErrorInfo
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
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSSUPER.PROCESS.Process.GetDiscount());
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

    }
}

using System;
using System.Net.Http;
using System.Web.Http;
using CLVSSUPER.MODELS;
using System.Web.Http.ModelBinding;
using CLVSSUPER.COMMON;
using CLVSSUPER.LOGGER;
using System.Web.Script.Serialization;

namespace CLVSSUPER.API.Controllers
{
    public class PaymentController : ApiController
    {
        /// <summary>
        /// trae las listas de todos los pagos que se deven hacer sobre las facturas
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
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSSUPER.PROCESS.Process.GetPayInvoices(cardCode, sede, currency));
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
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSSUPER.PROCESS.Process.CancelPayment(canPay));
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
                        result = false,
                        errorInfo = new ErrorInfo
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
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSSUPER.PROCESS.Process.GetPaymentList(infoSearch));
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
    }
}

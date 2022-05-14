using CLVSPOS.COMMON;
using CLVSPOS.LOGGER;
using CLVSPOS.MODELS;
using CLVSSUPER.MODELS;
using System;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
namespace CLVSPOS.API.Controllers
{
    public class BanksController : ApiController
    {
        /// <summary>
        /// obtiene la cuenta cuentas de los bancos
        /// no recibe parametros
        /// </summary>
        /// <returns></returns>
        [Authorize] 
        [HttpGet]
        [Route("api/Banks/GetAccountsBank")]
        [ActionName("GetAccountsBank")]
        public HttpResponseMessage GetAccountsBank()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetAccountsBank());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (BankResponse)LogManager.HandleExceptionWithReturn(exc, "BankResponse", "api/Banks/GetAccountsBank", (int)Constants.LogTypes.API));
            }
        }

        [Authorize] 
        [HttpGet]
        [Route("api/Banks/SyncGetAccountsBank")]
        [ActionName("SyncGetAccountsBank")]
        public HttpResponseMessage SyncGetAccountsBank(string userId)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.SyncGetAccountsBank(userId));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (BankResponse)LogManager.HandleExceptionWithReturn(exc, "BankResponse", "api/Banks/SyncGetAccountsBank", (int)Constants.LogTypes.API));
            }
        }


        /// <summary>
        /// Endpoint para guardar el resultado de una precierre
        /// </summary>
        /// <param name="_invoicePaymentDetail"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Banks/SavePreBalance")]
        [ActionName("SavePreBalance")]
        public HttpResponseMessage SavePreBalance(ACQTransaction _aCQTransaction)
        {
            try
            {
                LogManager.LogMessage(string.Format("api/Banks/PreBalance-- Terminal id: {0}", _aCQTransaction), (int)Constants.LogTypes.API);

                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.SavePreBalance(_aCQTransaction));
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
        /// Endpoint para guardar el resultado de un cierre
        /// </summary>
        /// <param name="_invoicePaymentDetail"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Banks/SaveBalance")]
        [ActionName("SaveBalance")]
        public HttpResponseMessage SaveBalance(ACQTransaction _aCQTransaction)
        {
            try
            {
               
                LogManager.LogMessage(string.Format("api/Banks/SaveBalance-- ACQTransaction: {0}", JsonConvert.SerializeObject( _aCQTransaction)), (int)Constants.LogTypes.API);

                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.SaveBalance(_aCQTransaction));
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
        /// End point para obtener las cierres o precierres realizados
        /// </summary>
        /// <param name="_invoicePaymentDetail"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Banks/PreBalanceOnRegister")]
        [ActionName("PreBalanceOnRegister")]
        public HttpResponseMessage PreBalanceOnRegister(PPBalanceRequest _pPBalanceRequest)
        {
            try
            {
                LogManager.LogMessage(string.Format("api/Banks/PreBalance-- Terminal id: {0}", _pPBalanceRequest), (int)Constants.LogTypes.API);

                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.PreBalanceOnRegisters(_pPBalanceRequest));
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
        /// Obtener el total de tarjetas de pinpad
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Banks/GetTransactionsPinpadTotal")]
        [ActionName("GetTransactionsPinpadTotal")]
        public HttpResponseMessage GetTransactionsPinpadTotal(int terminalId)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetTransactionsPinpadTotal(terminalId));

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

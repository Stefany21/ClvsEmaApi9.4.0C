using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CLVSPOS.COMMON;
using CLVSPOS.LOGGER;
using System.Web;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Script.Serialization;
using CLVSPOS.MODELS;

namespace CLVSPOS.API.Controllers
{
    public class PaydeskController : ApiController
    {   
        /// <summary>
        /// Obtiene reporte cierre de caja 
        /// </summary>
        /// <param name="creationDate"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Paydesk/GetPaydeskBalance")]
        [ActionName("GetPaydeskBalance")]
        public HttpResponseMessage GetPaydeskBalance(string creationDate)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.GetPaydeskBalance(creationDate));

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
        /// Crea cierre de caja, envia parametros de totales de efectivo,tarjeta manual,pinpad, transferencia
        /// </summary>
        /// <param name="paydeskBalance"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Paydesk/PostPaydeskBalance")]
        [ActionName("PostPaydeskBalance")]
        public HttpResponseMessage PostPaydeskBalance(PaydeskBalance paydeskBalance)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.PostPaydeskBalance(paydeskBalance));

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
        /// Obtiene motivos para generar entradas, salidas movimientos de dinero
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Paydesk/GetCashflowReasons")]
        [ActionName("GetCashflowReasons")]
        public HttpResponseMessage GetCashflowReasons()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.GetCashflowReasons());

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
        /// Crea una entrada o salida de diner en udt Cashflow
        /// </summary>
        /// <param name="cashflow"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Paydesk/PostCashflow")]
        [ActionName("PostCashflow")]
        public HttpResponseMessage PostCashflow(CashflowModel cashflow)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.CreateCashflow(cashflow));

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
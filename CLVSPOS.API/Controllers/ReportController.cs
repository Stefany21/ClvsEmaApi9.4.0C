using CLVSPOS.COMMON;
using CLVSPOS.LOGGER;
using CLVSPOS.MODELS;
using CLVSPOS.PROCESS;
using CLVSSUPER.MODELS;
using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace CLVSPOS.API.Controllers
{
    public class ReportController : ApiController
    {
        /// <summary>
        /// Metodo para crear un ARInvoice
        /// Metodo para enviar las ventas a SAP
        /// Recibe como parametro el modelo del ARInvoiceModel
        /// REVC [Authorize] activar cuando el login este funcando
        /// </summary>
        /// <param name="DocEntry"></param>
        /// <param name="ReportType"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Report/PrintReport")]
        [ActionName("PrintReport")]
        public HttpResponseMessage PrintReport(int DocEntry, int ReportType)
        {        
            try
            {

                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, ARInvoiceReport.PrintReport(DocEntry, ReportType));
                }
                else
                {
                    throw new Exception("Campos inexistentes o con valores incorrectos");
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
        /// Obtiene reporte impresion factura con pago pinpad con firma, sin firma
        /// </summary>
        /// <param name="transactionPrint"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Report/PrintReportPP")]
        [ActionName("PrintReportPP")]
        public HttpResponseMessage PrintReportPP(TransactionPrint transactionPrint)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, ARInvoiceReport.PrintReportPP(transactionPrint));
                }
                else
                {
                    throw new Exception("Campos inexistentes o con valores incorrectos");
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
        /// Metodo para crear un ARInvoice
        /// Metodo para enviar las ventas a SAP
        /// Recibe como parametro el modelo del ARInvoiceModel
        /// REVC [Authorize] activar cuando el login este funcando
        /// </summary>
        /// <param name="PrintInventory"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Report/PrintInventory")]
        [ActionName("PrintInventory")]
        public HttpResponseMessage PrintInventory(PrintInventoryModel PrintInventory)
        {          
            try
            {

                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, InventoryReport.InventoryReports(PrintInventory.Articulo, PrintInventory.Marca, PrintInventory.Grupo, PrintInventory.subGrupo));
                }
                else
                {
                    throw new Exception("Campos inexistentes o con valores incorrectos");
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
        /// Metodo para crear la reimprecion de un documento 
        /// </summary>
        /// <param name="DocEntry"></param>
        /// <param name="ReportType"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Report/GetARInvCopyReport")]
        [ActionName("GetARInvCopyReport")]
        public HttpResponseMessage GetARInvCopyReport(int DocEntry, int ReportType)
        {
            try
            {
               
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, ARInvCopyReport.GetARInvCopyReport(DocEntry, ReportType));
                }
                else
                {
                    throw new Exception("Campos inexistentes o con valores incorrectos");
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
        /// Obtener reporte cierre de caja
        /// </summary>
        /// <param name="BalanceModel"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Report/GetBalanceReport")]
        [ActionName("GetBalanceReport")]
        public HttpResponseMessage GetBalanceReport(GetBalanceModel_UsrOrDate BalanceModel)
        {
            try
            {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetBalanceReport(BalanceModel));               

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
        /// Obtiene nombre de reportes con su respectivo key configurado en la base de datos de aplicacion 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Report/GetReports")]
        public HttpResponseMessage GetReports()
        {      
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, Process.GetReports());

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
        /// Obtiene reporte para descargar desde vista companias 
        /// </summary>
        /// <param name="reportKey"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Report/DownloadReportFile")]
        public HttpResponseMessage DownloadReportFile(int reportKey)
        {          
            try
            {               
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, Process.DownloadReportFile(reportKey));              

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

        #region VOUCHER_SECTION
        /// <summary>
        /// Obtiene reporte para reimpresion de documentos 
        /// </summary>
        /// <param name="pPTransaction"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Report/PrintVoucher")]
        [ActionName("PrintVoucher")]
        public HttpResponseMessage PrintVoucher(PPTransaction pPTransaction)
        {
            try
            {
                    //if (ModelState.IsValid)
                    //{
                        return Request.CreateResponse(System.Net.HttpStatusCode.OK, ARInvoiceReport.PrintVoucher(pPTransaction));
                    //}
                    //else
                    //{
                    //    throw new Exception("Campos inexistentes o con valores incorrectos");
                    //}

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
    }
}

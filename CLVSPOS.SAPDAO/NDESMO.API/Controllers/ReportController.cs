using CLVSSUPER.COMMON;
using CLVSSUPER.LOGGER;
using CLVSSUPER.MODELS;
using CLVSSUPER.PROCESS;
using System;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace CLVSSUPER.API.Controllers
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
        public IHttpActionResult PrintReport(int DocEntry, int ReportType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var file = ARInvoiceReport.PrintReport(DocEntry, ReportType);
                    var response = file;
                    return Ok(response);
                }
                else
                {
                    LogManager.LogMessage( string.Format ("api/Report/PrintReport-- Objeto recibido DocEntry:  {0} ReportType: {1}", DocEntry,ReportType), (int)Constants.LogTypes.API);                    
                    LogManager.LogMessage("-API/PrintARInvoice - Campos inexistentes o con valores incorrectos", (int)Constants.LogTypes.API);

                    return null;
                }
            }

            catch (Exception exc)
            {
                LogManager.HandleException(exc, "api/Report/PrintReport", (int)Constants.LogTypes.API);

                return null;
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
        public IHttpActionResult PrintInventory(PrintInventoryModel PrintInventory)
         {
            try
            {
                if (ModelState.IsValid)
                {
                    var file = InventoryReport.InventoryReports(PrintInventory.Articulo, PrintInventory.Marca, PrintInventory.Grupo, PrintInventory.subGrupo);
                    var response = file;
                    return Ok(response);
                }
                else
                {
                    
                    var modelToString = new JavaScriptSerializer().Serialize(PrintInventory);
                    LogManager.LogMessage(string.Format("api/Report/PrintInventory-- Objeto recibido: {0}", modelToString), (int)Constants.LogTypes.API);                    
                    LogManager.LogMessage("-API/PrintInventory - Campos inexistentes o con valores incorrectos", (int)Constants.LogTypes.API);

                    return null;
                }
            }

            catch (Exception exc)
            {
                LogManager.HandleException(exc, "api/Report/PrintInventory", (int)Constants.LogTypes.API);
                //return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                //                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Report/PrintInventory", (int)Constants.LogTypes.API));
                return null;
            }
        }

        /// <summary>
        /// Metodo para crear la reimprecion de una factura
        /// Recibe como parametro el modelo del el docEntry de la factura de sap
        /// </summary>
        /// <param name="DocEntry"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Report/GetARInvCopyReport")]
        [ActionName("GetARInvCopyReport")]
        public IHttpActionResult GetARInvCopyReport(int DocEntry, int ReportType)
        {
            try
            {
                if (ModelState.IsValid)
                {           
                    var file = ARInvCopyReport.GetARInvCopyReport(DocEntry, ReportType);
                    var response = file;

                    return Ok(response);
                }
                else
                {
                    LogManager.LogMessage(string.Format("api/Report/GetARInvCopyReport-- Objeto recibido DocEntry: {0}", DocEntry), (int)Constants.LogTypes.API);
                    LogManager.LogMessage("-API/GetARInvCopyReport - Campos inexistentes o con valores incorrectos", (int)Constants.LogTypes.API);

                    return null;
                }
            }

            catch (Exception exc)
            {
                LogManager.HandleException(exc, "api/Report/GetARInvCopyReport", (int)Constants.LogTypes.API);
                return null;

                //int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
                //string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;
                //LogManager.LogMessage("api/Report/GetARInvCopyReport-- code: " + code + "-- message: " + message, (int)Constants.LogTypes.API);
                //return null;
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/Report/GetBalanceReport")]
        [ActionName("GetBalanceReport")]
        public IHttpActionResult GetBalanceReport()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var file = BalanceReport.GetBalanceReport();
                    var response = file;

                    return Ok(response);
                }
                else
                {
                    //LogManager.LogMessage(string.Format("api/Report/GetBalanceReport-- Objeto recibido DocEntry: {0}", ), (int)Constants.LogTypes.API);
                    LogManager.LogMessage("-API/GetBalanceReport - Campos inexistentes o con valores incorrectos", (int)Constants.LogTypes.API);

                    return null;
                }
            }

            catch (Exception exc)
            {
                LogManager.HandleException(exc, "api/Report/GetBalanceReport", (int)Constants.LogTypes.API);
                return null;
            }
        }
    }
}

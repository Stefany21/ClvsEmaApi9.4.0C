using CLVSPOS.COMMON;
using CLVSPOS.LOGGER;
using CLVSPOS.MODELS;
using CLVSSUPER.MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CLVSPOS.API.Controllers
{
    [Authorize]
    public class UdfController : ApiController
    {
        /// <summary>
        /// Devuelve los udfs de una tabla
        /// </summary>
        /// <param name="category"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Udf/GetUdfs")]
        [ActionName("GetUdfs")]
        public HttpResponseMessage GetUdfs(string category)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetUdfs(category));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (UdfsResponse) LogManager.HandleExceptionWithReturn(exc, "UdfsResponse", "api/Udf/GetUdfs", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// Devuelve todos los udfs que seran visibles para el usuario final
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Udf/GetConfiguredUdfs")]
        [ActionName("GetConfiguredUdfs")]
        public HttpResponseMessage GetConfiguredUdfs(string category)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetConfiguredUdfs(category));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (UdfsResponse)LogManager.HandleExceptionWithReturn(exc, "UdfsResponse", "api/Udf/GetUdfs", (int)Constants.LogTypes.API));
            }
        }
        /// <summary>
        /// Devuelve todos los udfs configurados desde una vista
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Udf/GetUdfCategories")]
        [ActionName("GetUdfs")]
        public HttpResponseMessage GetUdfCategories()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetUdfCategories());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (UdfsResponse)LogManager.HandleExceptionWithReturn(exc, "UdfCategoriesResponse", "api/Udf/GetUdfCategories", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// Devuelve todos los udfs configurados desde una vista
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Udf/GetUdfDevelopment")]
        [ActionName("GetUdfDevelopment")]
        public HttpResponseMessage GetUdfDevelopment()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetUdfDevelopment());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (UdfsResponse)LogManager.HandleExceptionWithReturn(exc, "UdfCategoriesResponse", "api/Udf/GetUdfDevelopment", (int)Constants.LogTypes.API));
            }
        }
        /// <summary>
        /// Retorna la data de la lista de udfs para ser procesados a nivel de ui
        /// </summary>
        /// <param name="udfSource"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Udf/GetUdfsData")]
        [ActionName("GetUdfsData")]
        public HttpResponseMessage GetUdfsData(UdfSource udfSource)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetUdfsData(udfSource));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (UdfsTargetResponse)LogManager.HandleExceptionWithReturn(exc, "UdfsTargetResponse", "api/Udf/GetUdfCategories", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// Guarda las configuraciones de udfs seleccionadas por el usuario de aplicacion
        /// </summary>
        /// <param name="transferUdf"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Udf/SaveUdfs")]
        [ActionName("SaveUdfs")]
        public HttpResponseMessage SaveUdfs(TransferUdf transferUdf)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.SaveUdfs(transferUdf.Udfs, transferUdf.Category));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (BaseResponse)LogManager.HandleExceptionWithReturn(exc, "BaseResponse", "api/Udf/SaveUdfs", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// Devuelve todos los udfs que seran visibles para el usuario final
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Udf/SyncGetConfiguredUdfs")]
        [ActionName("SyncGetConfiguredUdfs")]
        public HttpResponseMessage SyncGetConfiguredUdfs()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.SyncGetConfiguredUdfs());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse", "api/Udf/SyncGetConfiguredUdfs", (int)Constants.LogTypes.API));
            }
        }
    }
}

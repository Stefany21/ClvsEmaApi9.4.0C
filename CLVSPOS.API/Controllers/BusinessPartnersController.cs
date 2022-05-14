using System;
using System.Net.Http;
using System.Web.Http;
using CLVSPOS.MODELS;
using CLVSPOS.COMMON;
using CLVSPOS.LOGGER;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace CLVSPOS.API.Controllers
{
    public class BusinessPartnersController : ApiController
    {
        /// <summary>
        /// obtiene la lista total de todos BPS con los que trabaja la empresa, en caso de error envia un modelo de error especificando que fue lo que paso
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/BusinessPartners/GetBusinessPartners")]
        [ActionName("GetBusinessPartners")]
        public HttpResponseMessage GetBusinessPartner()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetBusinessPartners());
                
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (BaseResponse)LogManager.HandleExceptionWithReturn(exc, "BPSResponseModel", "api/BusinessPartners/GetBusinessPartners", (int)Constants.LogTypes.API));
            }
        }        

        /// <summary>
        /// obtiene la lista total de todos proveedores que tiene registrados la empresa, en caso de error envia un modelo de error especificando que fue lo que paso
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/BusinessPartners/GetSuppliers")]
        [ActionName("GetSuppliers")]
        public HttpResponseMessage GetSuppliers()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetSuppliers());

            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (BaseResponse)LogManager.HandleExceptionWithReturn(exc, "BPSResponseModel", "api/BusinessPartners/GetBusinessPartners", (int)Constants.LogTypes.API));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/BusinessPartners/SyncGetBusinessPartners")]
        [ActionName("SyncGetBusinessPartners")]
        public HttpResponseMessage SyncGetBusinessPartners(string userId)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.SyncGetBusinessPartners(userId));

            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (BaseResponse)LogManager.HandleExceptionWithReturn(exc, "BPSResponseModel", "api/BusinessPartners/SyncGetBusinessPartners", (int)Constants.LogTypes.API));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/BusinessPartners/SyncGetOUSR")]
        [ActionName("SyncGetOUSR")]
        public HttpResponseMessage SyncGetOUSR(string userId)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.SyncGetOUSR(userId));

            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (BaseResponse)LogManager.HandleExceptionWithReturn(exc, "BPSResponseModel", "api/BusinessPartners/SyncGetOUSR", (int)Constants.LogTypes.API));
            }
        }


        [Authorize]
        [HttpGet]
        [Route("api/BusinessPartners/GetBusinessPartnerFEInfo")]
        [ActionName("GetBusinessPartnerFEInfo")]
        public HttpResponseMessage GetBusinessPartnerFEInfo( string cardCode)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetBusinessPartnerFEInfo(cardCode));

            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (BaseResponse)LogManager.HandleExceptionWithReturn(exc, "BPFEInfoResponseModel", "api/BusinessPartners/GetBusinessPartnerFEInfo", (int)Constants.LogTypes.API));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/BusinessPartners/GetBusinessPartnerFEInfo")]
        [ActionName("GetBusinessPartnerFEInfo")]
        public HttpResponseMessage GetBusinessPartnerFEInfo(string idType, string idNumber)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetBusinessPartnerFEInfo(idType, idNumber));

            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (BaseResponse)LogManager.HandleExceptionWithReturn(exc, "BPFEInfoResponseModel", "api/BusinessPartners/GetBusinessPartnerFEInfo", (int)Constants.LogTypes.API));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/BusinessPartners/GetBusinessPartnerPadronInfo")]
        [ActionName("GetBusinessPartnerPadronInfo")]
        public HttpResponseMessage GetBusinessPartnerPadronInfo(string idNumber, string token)
        {
            try
            {                
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetBusinessPartnerPadronInfo(idNumber, token));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (BaseResponse)LogManager.HandleExceptionWithReturn(exc, "BPSResponseModel", "api/BusinessPartners/GetBusinessPartnerPadronInfo", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// Obtener lista de todos los Clientes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/BusinessPartners/GetCustomer")]
        [ActionName("GetCustomer")]
        public HttpResponseMessage GetCustomer()
        {
          
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetCustomer());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                           (BaseResponse)LogManager.HandleExceptionWithReturn(exc, "CustomerResponseModel", "api/BusinessPartners/GetCustomer", (int)Constants.LogTypes.API));

            }
        }

        /// <summary>
        /// Obtener lista de un Cliente por Codigo
        /// </summary>
        /// <param name="CardCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/BusinessPartners/GetCustomerbyCode")]
        [ActionName("GetCustomerbyCode")]
        public HttpResponseMessage GetCustomerbyCode(string CardCode)
        {
            
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.GetCustomerbyCode(CardCode));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                           (BaseResponse)LogManager.HandleExceptionWithReturn(exc, "CustomerResponseModel", "api/BusinessPartners/GetCustomer", (int)Constants.LogTypes.API));



            }
        }

        /// <summary>
        /// Actualizar Cliente
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/BusinessPartners/UpdateCustomer")]
        [ActionName("UpdateCustomer")]
        public HttpResponseMessage UpdateCustomer(GetCustomerModel customer)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.UpdateCustomer(customer));
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
        /// Crear nuevo Cliente
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/BusinessPartners/CreateCustomer")]
        [ActionName("CreateCustomer")]
        public HttpResponseMessage CreateCustomer(GetCustomerModel customer)
        {
            try
            {               
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.CreateCustomer(customer));
                             
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


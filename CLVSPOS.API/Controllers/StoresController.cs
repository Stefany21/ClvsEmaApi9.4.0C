using CLVSPOS.COMMON;
using CLVSPOS.LOGGER;
using CLVSPOS.MODELS;
using System;
using System.Net.Http;
using System.Web.Http;

namespace CLVSPOS.API.Controllers
{
    public class StoresController : ApiController
    {
        /// <summary>
        /// obtiene almacenes para ser sincronizados de forma local        
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Stores/SyncGetStores")]
        [ActionName("SyncGetStores")]
        public HttpResponseMessage SyncGetStores()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.SyncGetStores());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (StoreListModel)LogManager.HandleExceptionWithReturn(exc, "StoreListModel", "api/Stores/SyncGetStores", (int)Constants.LogTypes.API));
            }
        }


        /// <summary>
        /// obtiene la lista total del nombre de todos los items con los que trabaja la empresa
        /// en caso de error envia un modelo de error especificando que fue lo que paso
        /// el id de la compañia dese el front para buscar por compañia seleccionada... no logeada.
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Stores/GetStoresByCompany")]
        [ActionName("GetStoresByCompany")]
        public HttpResponseMessage GetStoresByCompany(int company)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetStoresByCompany(company));
                }
                else {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (ItemsResponse)LogManager.HandleExceptionWithReturn(new Exception(), "ItemsResponse",
                                                                         string.Format("api/Stores/GetStoresByCompany-- Objeto recibido Company: {0}", company),
                                                                         (int)Constants.LogTypes.API, true));                    
                }
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (StoreListModel)LogManager.HandleExceptionWithReturn(exc, "StoreListModel", "api/Stores/GetStoresByCompany", (int)Constants.LogTypes.API)); 
            }
        }

        /// <summary>
        /// obtiene la lista total del nombre de todos los items con los que trabaja la empresa
        /// en caso de error envia un modelo de error especificando que fue lo que paso
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Stores/GetAllStores")]
        [ActionName("GetAllStores")]
        public HttpResponseMessage GetAllStores()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetAllStores());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (StoreListModel)LogManager.HandleExceptionWithReturn(exc, "StoreListModel", "api/Stores/GetAllStores", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// obtiene la lista total del nombre de todos los items con los que trabaja la empresa
        /// en caso de error envia un modelo de error especificando que fue lo que paso
        /// sin parametros
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Stores/GetStoresList")]
        [ActionName("GetStoresList")]
        public HttpResponseMessage GetStoresList(int company)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetStoresList(company));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (StoreListModel)LogManager.HandleExceptionWithReturn(exc, "StoreListModel", "api/Stores/GetStoresList", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// crea un nuevo almecen para una compañia
        /// de parametro obtiene un modelo de tipo Store
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Stores/CreateStore")]
        [ActionName("CreateStore")]
        public HttpResponseMessage CreateStore(StoresModel store)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.CreateStore(store));
                }
                else {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, new ItemsResponse
                    {
                        Result = false,
                        Error = new ErrorInfo
                        {
                            Code = 1,
                            Message = "Item inexistente o valores de envio incorrectos"
                        }
                    });
                }
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (StoreListModel)LogManager.HandleExceptionWithReturn(exc, "StoreListModel", "api/Stores/CreateStore", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// actualiza un almecen en espesifico
        /// de parametro obtiene un modelo de tipo Store
        /// </summary>
        [Authorize]
        [HttpPost]
        [Route("api/Stores/UpdateStore")]
        [ActionName("UpdateStore")]
        public HttpResponseMessage UpdateStore(StoresModel store)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.UpdateStore(store));
                }
                else
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, new ItemsResponse
                    {
                        Result = false,
                        Error = new ErrorInfo
                        {
                            Code = 1,
                            Message = "Item inexistente o valores de envio incorrectos"
                        }
                    });
                }
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (StoreListModel)LogManager.HandleExceptionWithReturn(exc, "StoreListModel", "api/Stores/UpdateStore", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// obtiene un almacen por ID
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Stores/GetStorebyId")]
        [ActionName("GetStorebyId")]
        public HttpResponseMessage GetStorebyId(int store)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetStorebyId(store));
                }
                else
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, new ItemsResponse
                    {
                        Result = false,
                        Error = new ErrorInfo
                        {
                            Code = 1,
                            Message = "Item inexistente o valores de envio incorrectos"
                        }
                    });
                }
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (StoreListModel)LogManager.HandleExceptionWithReturn(exc, "StoreListModel", "api/Stores/GetStorebyId", (int)Constants.LogTypes.API));
            }
        }
    }
}

using CLVSPOS.DAO;
using System;
using System.Net.Http;
using System.Web.Http;
using CLVSPOS.LOGGER;
using CLVSPOS.COMMON;
using System.Web.Script.Serialization;

namespace CLVSPOS.API.Controllers
{
    public class PermsController : ApiController
    {
        /// <summary>
        /// va a la base de datos y devuelve una lista con todos los permisos por usuario para sincronizar        
        /// </summary>       
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Perms/SyncGetPermByUsers")]
        [ActionName("SyncGetPermByUsers")]
        public HttpResponseMessage SyncGetPermByUsers()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, DAO.GetData.SyncGetPermsByUser());
                }
                else
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(new Exception(), string.Empty,
                                                                         string.Format("api/Perms/SyncGetPermByUsers"),
                                                                         (int)Constants.LogTypes.API, true));
                }
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Perms/GetPermsByUser", (int)Constants.LogTypes.API));
            }
        }


        /// <summary>
        /// va a la base de datos y devuelve una lista con todos los permisos por usuario para sincronizar        
        /// </summary>       
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Perms/SyncGetPermissions")]
        [ActionName("SyncGetPermissions")]
        public HttpResponseMessage SyncGetPermissions()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, DAO.GetData.SyncGetPermissions());
                }
                else
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(new Exception(), string.Empty,
                                                                         string.Format("api/Perms/SyncGetPermissions"),
                                                                         (int)Constants.LogTypes.API, true));
                }
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Perms/GetPermsByUser", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// va a la base de datos y devuelve una losta con todos los permisos y los estados se estos 
        /// recive el ID de usuario.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Perms/GetPermsByUser")]
        [ActionName("GetPermsByUser")]
        public HttpResponseMessage GetPermsByUser(string user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, DAO.GetData.GetPermsByUser(user));
                }
                else
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(new Exception(), string.Empty,
                                                                         string.Format("api/Perms/GetPermsByUser-- Objeto recibido User: {0}", user),
                                                                         (int)Constants.LogTypes.API, true));                   
                }
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Perms/GetPermsByUser", (int)Constants.LogTypes.API));                
            }
        }

        /// <summary>
        /// va a la base de datos y devuelve una lista con todos los permisos y los estados se estos 
        /// recibe el ID de usuario.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Perms/GetPermsByUserMenu")]
        [ActionName("GetPermsByUserMenu")]
        public HttpResponseMessage GetPermsByUserMenu()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.GetPermsByUserMenu());
                }
                else
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(new Exception(), string.Empty,
                                                                         string.Format("api/Perms/GetPermsByUserMenu-- Error"),
                                                                         (int)Constants.LogTypes.API, true));                   
                }
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Perms/GetPermsByUserMenu", (int)Constants.LogTypes.API));                
            }
        }

        /// <summary>
        /// trae la lista de los permisos que se van a modificar y los envia al Dao para ser prosesados y cambiados en la BD
        /// devuelve un modelo base de respuesta para informar el estado de este.
        /// no se le asignan parametros
        /// </summary>
        /// <param name="permsUserEdit"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Perms/EditPermsByUser")]
        [ActionName("EditPermsByUser")]
        public HttpResponseMessage EditPermsByUser(PermsUserEdit permsUserEdit)
        {
            try
            {
                if (ModelState.IsValid) {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, DAO.PostData.EditPermsByUser(permsUserEdit));
                } else
                {
                    var modelToString = new JavaScriptSerializer().Serialize(permsUserEdit);
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(new Exception(), string.Empty,
                                                                         string.Format("api/Perms/EditPermsByUser-- Objeto recibido: {0}", modelToString),
                                                                         (int)Constants.LogTypes.API, true));                   
                }
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Perms/EditPermsByUser", (int)Constants.LogTypes.API));                
            }
        }

        /// <summary>
        /// trae la lista de usuarios a la cual se le aplican los permisos
        /// no se le asignan parametros
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Perms/getUserList")]
        [ActionName("getUserList")]
        public HttpResponseMessage getUserList()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, DAO.GetData.GetUserList());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Perms/getUserList", (int)Constants.LogTypes.API));                
            }
        }
    }
}

using CLVSPOS.COMMON;
using CLVSPOS.DAO;
using CLVSPOS.LOGGER;
using CLVSPOS.MODELS;
using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace CLVSPOS.API.Controllers
{
    public class UsersController : ApiController
    {

        /// <summary>
        /// va a la base de datos y devuelve una lista con los usuarios para la configuracion de usuarios
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Users/GetUserUserAssignList")]
        [ActionName("GetUserUserAssignList")]
        public HttpResponseMessage GetUserUserAssignList()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.GetUserUserAssignList());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Users/GetUserUserAssignList", (int)Constants.LogTypes.API));                
            }
        }

        /// <summary>
        /// va a la base de datos y devuelve una lista con los usuarios para la configuracion de usuarios
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Users/SyncGetUserUserAssign")]
        [ActionName("SyncGetUserUserAssign")]
        public HttpResponseMessage SyncGetUserUserAssign()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.SyncGetUserUserAssign());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Users/SyncGetUserUserAssign", (int)Constants.LogTypes.API));
            }
        }


        /// <summary>
        /// va a la base de datos y devuelve una lista con los usuarios para la configuracion de usuarios
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Users/SyncGetUsers")]
        [ActionName("SyncGetUsers")]
        public HttpResponseMessage SyncGetUsers()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.SyncGetUsers());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Users/SyncGetUsers", (int)Constants.LogTypes.API));
            }
        }




        /// <summary>
        /// lleva la informacion de un usuario especifico para ser modificada en BD
        /// recive como parametro el modelo de usuario.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Users/UpdateUser")]
        [ActionName("UpdateUser")]
        public HttpResponseMessage UpdateUser(UserAsingModel user)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.UpdateUser(user));
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
        /// Crear asignacion de usuaario
        /// recive como parametro el modelo de usuario.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Users/CreateNewUser")]
        [ActionName("CreateNewUser")]
        public HttpResponseMessage CreateNewUser(UserAsingModel user)
        {
            
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.CreateNewUser(user));
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
        /// Actualiza usuario registrado en bases de datos de aplicacion 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Users/UpdateUserApp")]
        [ActionName("UpdateUserApp")]
        public HttpResponseMessage UpdateUserApp(UserModel user)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.UpdateUserApp(user));
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
        /// Registro de nuevo usuario en base de datos de aplicacion 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Users/CreateUserApp")]
        [ActionName("CreateUserApp")]
        public HttpResponseMessage CreateUserApp(UserModel user)
        {           
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.CreateUserApp(user));
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

        //[Authorize]
        //[HttpPost]
        //[Route("api/Users/UpdateUserApp")]
        //[ActionName("UpdateUserApp")]
        //public HttpResponseMessage UpdateUserApp(UserModel user)
        //{
        //    try
        //    {
        //        return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.UpdateUserApp(user));
        //    }
        //    catch (Exception exc)
        //    {
        //        return Request.CreateResponse(System.Net.HttpStatusCode.OK,
        //                                      LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Users/UpdateUser", (int)Constants.LogTypes.API));
        //    }
        //}


        /// <summary>
        /// Obtiene información de usuario segun el id del parametro
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Users/GetUserApp")]
        [ActionName("GetUserApp")]
        public HttpResponseMessage GetUserApp(string id)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.GetUserApp(id));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Users/UpdateUser", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// Obtiene lista de usuarios registrados en base de datos de aplicación
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Users/GetUsersApp")]
        [ActionName("GetUsersApp")]
        public HttpResponseMessage GetUsersApp()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.GetUsersApp());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Users/UpdateUser", (int)Constants.LogTypes.API));
            }
        }
    }
}

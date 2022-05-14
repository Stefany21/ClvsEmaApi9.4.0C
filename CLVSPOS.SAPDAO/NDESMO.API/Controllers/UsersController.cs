using CLVSSUPER.COMMON;
using CLVSSUPER.LOGGER;
using CLVSSUPER.MODELS;
using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace CLVSSUPER.API.Controllers
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
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.UpdateUser(user));
                }
                else
                {
                    var modelToString = new JavaScriptSerializer().Serialize(user);
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(new Exception(), string.Empty,
                                                                         string.Format("api/Users/UpdateUser-- Objeto recibido: {0}", modelToString),
                                                                         (int)Constants.LogTypes.API, true));                    
                }
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Users/UpdateUser", (int)Constants.LogTypes.API));                
            }
        }

        /// <summary>
        /// lleva la informacion de un usuario espesifico para ser creado en BD
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
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.CreateNewUser(user));
                }
                else
                {
                    var modelToString = new JavaScriptSerializer().Serialize(user);
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(new Exception(), string.Empty,
                                                                         string.Format("api/Users/CreateNewUser-- Objeto recibido: {0}", modelToString),
                                                                         (int)Constants.LogTypes.API, true));                    
                }
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Users/CreateNewUser", (int)Constants.LogTypes.API));                
            }
        }


    }
}

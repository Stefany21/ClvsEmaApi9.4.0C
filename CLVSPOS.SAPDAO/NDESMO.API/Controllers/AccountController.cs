using CLVSSUPER.COMMON;
using CLVSSUPER.LOGGER;
using CLVSSUPER.MODELS;
using CLVSSUPER.PROCESS;
using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace CLVSSUPER.API.Controllers
{
    public class AccountController : ApiController
    {

        /// <summary>
        /// trae las listas de las cuentas
        /// no recibe parametros
        /// </summary>
        [Authorize]
        [HttpGet]
        [Route("api/Account/GetAccounts")]
        [ActionName("GetAccounts")]
        public HttpResponseMessage GetAccounts()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, Process.GetAccounts());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (AccountResponse)LogManager.HandleExceptionWithReturn(exc, "AccountResponse", 
                                              "api/Account/GetAccounts", (int)Constants.LogTypes.API));  
            }
        }
        /// <summary>
        /// metodo para el registro de usuarios de la app
        /// recibe como parametro un modelo con los objetos necesario (user)
        /// </summary>
        /// <param name="registerUser"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Account/RegisterUser")]
        [ActionName("RegisterUser")]
        public HttpResponseMessage RegisterUser(User registerUser)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, Process.RegisterUser(registerUser));
                }
                else
                {
                    var modelToString = new JavaScriptSerializer().Serialize(registerUser);
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(new Exception(), string.Empty,                                                              
                                                                         string.Format("api/Account/RegisterUser-- Objeto recibido: {0}", modelToString),
                                                                         (int)Constants.LogTypes.API, true));                    
                }
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Account/RegisterUser", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// metodo para el envio del correo para la recuperacion de la contrasenna de la cuenta del usuario
        /// recibe como parametro un modelo con los objetos necesario (userEmail)
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Account/SendRecoverPswdEmail")]
        [ActionName("SendRecoverPswdEmail")]
        public HttpResponseMessage SendRecoverPswdEmail(StringModel userEmail)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, Process.SendRecoverPswdEmail(userEmail));
                }
                else
                {
                    var modelToString = new JavaScriptSerializer().Serialize(userEmail);
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(new Exception(), string.Empty,
                                                                         string.Format("api/Account/SendRecoverPswdEmail-- Objeto recibido: {0}", modelToString),
                                                                         (int)Constants.LogTypes.API, true));                    
                }
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Account/SendRecoverPswdEmail", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// metodo para el envio de la informcion para el cambio de la contrasenna de la cuenta del usuario
        /// recibe como parametro un modelo con los objetos necesario (userEmail)
        /// </summary>
        /// <param name="recoverPswd"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Account/RecoverPswd")]
        [ActionName("RecoverPswd")]
        public HttpResponseMessage RecoverPswd(User recoverPswd)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, Process.RecoverPswd(recoverPswd));
                }
                else
                {
                    var modelToString = new JavaScriptSerializer().Serialize(recoverPswd);
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(new Exception(), string.Empty,
                                                                         string.Format("api/Account/RecoverPswd-- Objeto recibido: {0}", modelToString),
                                                                         (int)Constants.LogTypes.API, true));                    
                }
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Account/RecoverPswd", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// confirma el email
        /// </summary>
        /// <returns></returns>
        // [AllowAnonymous]
        [Authorize]
        [HttpGet]
        [Route("api/Account/ConfirmEmail")]
        [ActionName("ConfirmEmail")]
        public HttpResponseMessage ConfirmEmail()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, Process.ConfirmEmail());
                }
                else
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(new Exception(), string.Empty,
                                                                         "api/Account/ConfirmEmail",
                                                                         (int)Constants.LogTypes.API, true));                    
                }
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Account/ConfirmEmail", (int)Constants.LogTypes.API));               
            }
        }

    }
}

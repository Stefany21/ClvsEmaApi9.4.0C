using CLVSPOS.COMMON;
using CLVSPOS.LOGGER;
using CLVSPOS.MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CLVSPOS.API.Controllers
{
    public class MailsController : ApiController
    {
        /// <summary>
        /// Enviar documento por correo electronico
        /// </summary>
        /// <param name="_MailDataModel"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Mails/CreatePDFToSendMail")]
        [ActionName("CreatePDFToSendMail")]
        public HttpResponseMessage CreatePDFToSendMail(GetBalanceModel_UsrOrDate _MailDataModel)
        {           
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.CreatePDFToSendMail(_MailDataModel));

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
        /// Enviar PDF por Whatsapp
        /// </summary>
        /// <param name="_MailDataModel"></param>
        /// <returns></returns>
        //[Authorize]
        //[HttpPost]
        //[Route("api/Mails/CreatePDFToSendWhatsapp")]
        //[ActionName("CreatePDFToSendWhatsapp")]
        //public HttpResponseMessage CreatePDFToSendWhatsapp(WhatsappDocumentModel _MailDataModel)
        //{
        //    try
        //    {
        //        return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.CreatePDFToSendWhatsapp(_MailDataModel));
        //    }
        //    catch (Exception exc)
        //    {
        //        return Request.CreateResponse(System.Net.HttpStatusCode.OK,
        //                                      (BaseResponse)LogManager.HandleExceptionWithReturn(exc, "BaseResponse", "api/Items/SyncGetPayTermsList", (int)Constants.LogTypes.API));
        //    }
        //}

    }
}

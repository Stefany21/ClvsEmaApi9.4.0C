using CLVSPOS.COMMON;
using CLVSPOS.LOGGER;
using CLVSPOS.MODELS;
using CLVSSUPER.MODELS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace CLVSPOS.API.Controllers
{
    public class CompanyController : ApiController
    {
        /// <summary>
        /// metodo para obtener las companias registradas en la aplicacion con el fin de sincronizar localmente
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Company/SyncGetCompanies")]
        [ActionName("SyncGetCompanies")]
        public HttpResponseMessage SyncGetCompanies()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.SyncGetCompanies());

            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (SyncResponse)LogManager.HandleExceptionWithReturn(exc,
                                                                                                         "SyncResponse",
                                                                                                         "api/Company/SyncGetCompanies",
                                                                                                         (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// metodo para obtener las companias registradas en la aplicacion con el fin de sincronizar localmente
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Company/SyncGetCompaniesByUser")]
        [ActionName("SyncGetCompaniesByUser")]
        public HttpResponseMessage SyncGetCompaniesByUser()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.SyncGetCompaniesByUser());

            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse", "api/Company/SyncGetCompaniesByUser", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// metodo para obtener las companias registradas en la aplicacion
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Company/GetCompanies")]
        [ActionName("GetCompanies")]
        public HttpResponseMessage GetCompanies()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetCompanies());

            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (CompanyListResponse)LogManager.HandleExceptionWithReturn(exc,
                                                                                                         "CompanyListResponse",
                                                                                                         "api/Company/GetCompanies",
                                                                                                         (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// metodo para obtener las companias registradas en la aplicacion
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Company/GetDefaultCompany")]
        [ActionName("GetDefaultCompany")]
        public HttpResponseMessage GetDefaultCompany()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetDefaultCompany());

            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (CompanyListResponse)LogManager.HandleExceptionWithReturn(exc,
                                                                                                         "CompanyListResponse",
                                                                                                         "api/Company/GetCompanies",
                                                                                                         (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// metodo para obtener la informacion de una compannia por el id de la misma
        /// recibe como parametro el id de la compannia
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Company/GetCompanyById")]
        [ActionName("GetCompanyById")]
        public HttpResponseMessage GetCompanyById(int companyId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetCompanyById(companyId));
                }
                else
                {
                    var modelToString = new JavaScriptSerializer().Serialize(companyId);

                    return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (CompanyResponse)LogManager.HandleExceptionWithReturn(new Exception(), "CompanyResponse",
                                                                                             string.Format("api/Account/GetCompanyById-- Objeto recibido: {0}", modelToString),
                                                                                             (int)Constants.LogTypes.API, true));
                }
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (CompanyResponse)LogManager.HandleExceptionWithReturn(exc, "CompanyResponse",
                                                                                                    "api/Company/GetCompanyById", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// Metodo para crear una compania en la aplicacion
        /// Recibe como parametro un objeto con la compania y el correo
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Company/CreateCompany")]
        [ActionName("CreateCompany")]
        public HttpResponseMessage CreateCompany()
        {
            try
            {

                var httpPostedFile = HttpContext.Current.Request;
                var cm = httpPostedFile.Params["companyAndMail"];
                CompanyAndMail companyAndMail = new CompanyAndMail();
                using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(cm)))
                {
                    // Deserialization from JSON  
                    DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(CompanyAndMail));
                    companyAndMail = (CompanyAndMail)deserializer.ReadObject(ms);
                }
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.CreateCompany(companyAndMail, HttpContext.Current));

            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Company/CreateCompany", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// Metodo para modificar una compania en la aplicacion
        /// recibe como parametro un objeto con la compania y el correo
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Company/UpdateCompany")]
        [ActionName("UpdateCompany")]
        public HttpResponseMessage UpdateCompany()
        {
            try
            {
                var httpPostedFile = HttpContext.Current.Request;
                var cm = httpPostedFile.Params["companyAndMail"];
                CompanyAndMail companyAndMail = new CompanyAndMail();
                using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(cm)))
                {
                    // Deserialization from JSON  
                    DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(CompanyAndMail));
                    companyAndMail = (CompanyAndMail)deserializer.ReadObject(ms);
                }
                LogManager.LogMessage("UpdateCompany", (int)Constants.LogTypes.General);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.UpdateCompany(companyAndMail, HttpContext.Current));

            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Company/UpdateCompany", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// Metodo para obtener la informacion de una compania por el id de la misma
        /// recibe como parametro el id de la compania
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Company/GetCurrencyType")]
        [ActionName("GetCurrencyType")]
        public HttpResponseMessage GetCurrencyType()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, DAO.GetData.GetCurrencyType());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (CompanyResponse)LogManager.HandleExceptionWithReturn(exc, "CompanyResponse",
                                                                                "api /Company/GetCurrencyType", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// Funcion para obtener el logo de la compania desde la DBLocal
        /// no recibe parametros
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Company/GetCompanyLogo")]
        [ActionName("GetCompanyLogo")]
        public HttpResponseMessage GetCompanyLogo()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetCompanyLogo());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Company/GetCompanyLogo", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// metodo para obtener las companias registradas en la aplicacion con el fin de sincronizar localmente
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Company/SyncGetMailData")]
        [ActionName("SyncGetMailData")]
        public HttpResponseMessage SyncGetMailData()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.SyncGetMailData());

            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (SyncResponse)LogManager.HandleExceptionWithReturn(exc,
                                                                                                         "SyncResponse",
                                                                                                         "api/Company/SyncGetMailData",
                                                                                                         (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// Obtener Lista Vista y sus agrupaciones
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Company/GetViewGroupList")]
        [ActionName("GetViewGroupList")]
        public HttpResponseMessage GetViewGroupList()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.GetViewGroupList());

            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (ViewLineAgrupationResponse)LogManager.HandleExceptionWithReturn(exc,
                                                                                                         "ViewLineAgrupationResponse",
                                                                                                         "api/Company/GetViewGroupList",
                                                                                                         (int)Constants.LogTypes.API));
            }
        }
        /// <summary>
        /// Actualiza configuraciones a nivel de línea orden, agrupación.
        /// </summary>
        /// <param name="ViewGroupList"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Company/UpdateViewLineAgrupation")]
        [ActionName("UpdateViewLineAgrupation")]
        public HttpResponseMessage UpdateViewLineAgrupation(ViewLinesAgrupationList ViewGroupList)
        {
            try
            {

                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.UpdateViewLineAgrupation(ViewGroupList));

            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Company/UpdateViewLineAgrupation", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// Metodo para actualizar los margenes aceptados por vista para una compañia.
        /// </summary>
        /// <param name="IdCompany"></param>
        /// <param name="Margins"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Company/UpdateCompanyMargins")]
        public HttpResponseMessage UpdateCompanyViewsMargins(int IdCompany, List<CompanyMargins> Margins)
        {
            try
            {
                string JsonMarginsViewsModel = JsonConvert.SerializeObject(Margins);

                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.UpdateCompanyViewsMargins(IdCompany, JsonMarginsViewsModel));

            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Company/UpdateCompanyMargins", (int)Constants.LogTypes.API));
            }
        }
        /// <summary>
        /// Metodo para obtener la lista de margenes aceptados por vista.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Company/GetViewMargins")]
        public HttpResponseMessage GetViewMargins()
        {
            try
            {

                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.GetViewMargins());

            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Company/UpdateCompanyMargins", (int)Constants.LogTypes.API));
            }
        }
        #region DbObjectName
        /// <summary>
        /// Obtiene  descripción, nombre de objetos configurados en base de datos de aplicación 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Company/GetDbObjectName")]
        public HttpResponseMessage GetDbObjectNames()
        {
            try
            {

                return Request.CreateResponse(System.Net.HttpStatusCode.OK, PROCESS.Process.GetDbObjectNames());

            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Company/GetDbObjectName", (int)Constants.LogTypes.API));
            }
        }

        /// <summary>
        /// Actualiza nombre de objeto en base de datos de aplicación
        /// </summary>
        /// <param name="DBObjectNameList"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Company/UpdateDbObjectNames")]
        [ActionName("UpdateDbObjectNames")]
        public HttpResponseMessage UpdateDbObjectNames(List<DBObjectName> DBObjectNameList)
        {
            try
            {

                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSPOS.PROCESS.Process.UpdateDbObjectNames(DBObjectNameList));

            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Company/UpdateDbObjectNames", (int)Constants.LogTypes.API));
            }
        }

        #endregion
    }
}

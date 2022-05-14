using CLVSSUPER.COMMON;
using CLVSSUPER.LOGGER;
using CLVSSUPER.MODELS;
using System;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace CLVSSUPER.API.Controllers
{
    public class CompanyController : ApiController
    {
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
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSSUPER.PROCESS.Process.GetCompanies());
                
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
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSSUPER.PROCESS.Process.GetCompanyById(companyId));
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
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSSUPER.PROCESS.Process.CreateCompany(companyAndMail, HttpContext.Current));

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
                    return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSSUPER.PROCESS.Process.UpdateCompany(companyAndMail, HttpContext.Current));

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
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, CLVSSUPER.PROCESS.Process.GetCompanyLogo());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              LogManager.HandleExceptionWithReturn(exc, string.Empty, "api/Company/GetCompanyLogo", (int)Constants.LogTypes.API));                
            }
        }
    }
}

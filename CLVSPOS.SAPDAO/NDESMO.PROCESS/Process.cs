using CLVSSUPER.DAO;
using CLVSSUPER.LOGGER;
using CLVSSUPER.MODELS;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;

namespace CLVSSUPER.PROCESS
{
    public class Process
    {

        /// <summary>
        /// Funcion para obtener el id del usuario logueado
        /// </summary>
        /// <returns></returns>
        public static string GetUserId()
        {
            try
            {
                // se obtiene el userId, localizado en los Claims
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                return identity.Claims.Where(c => c.Type == "userId").Single().Value;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// Metodo para obtener los impuestos de la compania
        /// no recibe parametros
        /// </summary>
        /// <returns></returns>
        public static TaxesResponse GetTaxes()
        {
            try
            {
                var userId = GetUserId();

                var company = GetData.GetCompanyByUserId(userId);
                return CLVSSUPER.SAPDAO.GetSapData.GetTaxes(company);
            }
            catch (Exception exc)
            {
                return (TaxesResponse)LogManager.HandleExceptionWithReturn(exc, "TaxesResponse");
            }
        }

        /// <summary>
        /// Metodo para obtener los clientes de la compania
        /// No recibe parametros
        /// </summary>
        /// <returns></returns>
        public static BPSResponseModel GetBusinessPartners()
        {
            try
            {
                var userId = GetUserId();
                var company = GetData.GetCompanyByUserId(userId);
                return CLVSSUPER.SAPDAO.GetSapData.GetBusinessPartners(company);

            }
            catch (Exception exc)
            {
                return (BPSResponseModel)LogManager.HandleExceptionWithReturn(exc, "BPSResponseModel");
            }
        }

        /// <summary>
        /// Metodo para obtener los items de la compania
        /// No recibe parametros
        /// </summary>
        /// <returns></returns>
        public static ItemNamesResponse GetItemNames()
        {
            try
            {
                var userId = GetUserId();
                var company = GetData.GetCompanyByUserId(userId);
                return CLVSSUPER.SAPDAO.GetSapData.GetItemNames(company);

            }
            catch (Exception exc)
            {
                return (ItemNamesResponse)LogManager.HandleExceptionWithReturn(exc, "ItemNamesResponse");
            }
        }

        /// <summary>
        /// Metodo para obtener la informacion de un item de la compania
        /// Recibe como parametro el codigo del item a consultar
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="priceList"></param>
        /// <returns></returns>
        public static ItemsResponse GetInfoItem(string itemCode, int priceList)
        {
            try
            {
                var userId = GetUserId();
                var company = GetData.GetCompanyByUserId(userId);
                var userAssign = GetData.GetUserMappId(userId);
                return CLVSSUPER.SAPDAO.GetSapData.GetInfoItem(itemCode, Convert.ToDecimal(userAssign.minDiscount), company, priceList);

            }
            catch (Exception exc)
            {
                return (ItemsResponse)LogManager.HandleExceptionWithReturn(exc, "ItemsResponse");
            }
        }


        /// <summary>
        /// Metodo para obtener la informacion de un item de la compania
        /// No recibe parametros
        /// </summary>
        /// <returns></returns>
        public static ExchangeRateResponse GetExchangeRate()
        {
            try
            {
                var userId = GetUserId();
                var company = GetData.GetCompanyByUserId(userId);
                return CLVSSUPER.SAPDAO.GetSapData.GetExchangeRate(company);
            }
            catch (Exception exc)
            {
                return (ExchangeRateResponse)LogManager.HandleExceptionWithReturn(exc, "ExchangeRateResponse");
            }
        }

        /// <summary>
        /// Metodo para enviar una sale order a SAP
        /// No recibe parametros
        /// </summary>
        /// <param name="saleOrder"></param>
        /// <returns></returns>
        public static SalesOrderToSAPResponse CreateSaleOrder(SalesOrderModel saleOrder)
        {
            try
            {
                var userId = GetUserId();
                var company = GetData.GetCompanyByUserId(userId);
                return CLVSSUPER.SAPDAO.PostSapData.CreateSaleOrder(saleOrder, company, userId);
            }
            catch (Exception exc)
            {
                return (SalesOrderToSAPResponse)LogManager.HandleExceptionWithReturn(exc, "SalesOrderToSAPResponse");
            }
        }

        /// <summary>
        /// Metodo para enviar una cotizacion a SAP
        /// Los parametros que recibe son el modelo de Cotizacion que va a guardar en SAP
        /// </summary>
        /// <param name="quotations"></param>
        /// <returns></returns>
        public static QuotationsToSAPResponse CreateQuotation(QuotationsModel quotations)
        {
            try
            {
                var userId = GetUserId();
                var company = GetData.GetCompanyByUserId(userId);
                return CLVSSUPER.SAPDAO.PostSapData.CreateQuotations(quotations, company, userId);
            }
            catch (Exception exc)
            {
                return (QuotationsToSAPResponse)LogManager.HandleExceptionWithReturn(exc, "QuotationsToSAPResponse");
            }
        }

        /// <summary>
        /// Metodo para realizar un pago en SAP
        /// Recibe como parametros el modelo de pago
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        public static PaymentSapResponse CancelPayInvoices(CreatePaymentModel payment)
        {
            try
            {
                var userId = GetUserId();
                var company = GetData.GetCompanyByUserId(userId);
                return CLVSSUPER.SAPDAO.PostSapData.CreatePayInvoices(payment, company, userId);
            }
            catch (Exception exc)
            {
                return (PaymentSapResponse)LogManager.HandleExceptionWithReturn(exc, "PaymentSapResponse");
            }
        }

        /// <summary>
        /// Metodo para obtener la informacion de un item de la compania
        /// Recibe como parametro el codigo del item a consultar
        /// </summary>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        public static WHInfoResponse GetWHAvailableItem(string itemCode)
        {
            try
            {
                var userId = GetUserId();
                var company = GetData.GetCompanyByUserId(userId);
                return CLVSSUPER.SAPDAO.GetSapData.GetWHAvailableItem(itemCode, company);

            }
            catch (Exception exc)
            {
                return (WHInfoResponse)LogManager.HandleExceptionWithReturn(exc, "WHInfoResponse");
            }
        }

        /// <summary>
        /// metodo para obtener la de los parametros que lleva una determinada vista
        /// recibe como parametro el el valor de la vista de la cual va a traer los parametros
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public static ParamsViewResponse GetViewParam(int view)
        {
            try
            {
                return CLVSSUPER.DAO.GetData.GetViewParam(view);
            }
            catch (Exception exc)
            {
                return (ParamsViewResponse)LogManager.HandleExceptionWithReturn(exc, "ParamsViewResponse");
            }
        }

        /// <summary>
        /// metodo para obtener la informacion de de las seies por almacen
        /// recibe como parametro el codigo del item y el codigo del almacen a consultar
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="whsCode"></param>
        /// <returns></returns>
        public static SeriesResponse GetSeriesByItem(string itemCode, string whsCode)
        {
            try
            {
                var userId = GetUserId();
                var company = GetData.GetCompanyByUserId(userId);
                return CLVSSUPER.SAPDAO.GetSapData.GetSeriesByItem(itemCode, whsCode, company);

            }
            catch (Exception exc)
            {
                return (SeriesResponse)LogManager.HandleExceptionWithReturn(exc, "SeriesResponse");
            }
        }

        /// <summary>
        /// metodo para obtener las compannias registradas en la aplicacion
        /// no recibe parametros 
        /// </summary>
        /// <returns></returns>
        public static BaseResponse GetCompanies()
        {
            try
            {
                return CLVSSUPER.DAO.GetData.GetCompanies();
            }
            catch (Exception exc)
            {
                return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
            }
        }

        /// <summary>
        /// metodo para obtener la informacion de una compannia por el id de la misma
        /// recibe como parametro el id de la compannia
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public static CompanyResponse GetCompanyById(int companyId)
        {
            try
            {
                return CLVSSUPER.DAO.GetData.GetCompanyById(companyId);
            }
            catch (Exception)
            {
                throw new Exception(string.Format("Information for Company {0} Not Found", companyId));
            }
        }

        /// <summary>
        /// metodo para crear una compannia en la aplicacion
        /// recibe como parametro el modelo de compannia
        /// </summary>
        /// <param name="companyAndMail"></param>
        /// <param name="filesInfo"></param>
        /// <returns></returns>
        public static BaseResponse CreateCompany(CompanyAndMail companyAndMail, HttpContext filesInfo)
        {
            try
            {
                return CLVSSUPER.DAO.PostData.CreateCompany(companyAndMail, filesInfo);
            }
            catch (Exception exc)
            {
                return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
            }
        }

        /// <summary>
        /// metodo para actualizar una compannia y el correo en la aplicacion
        /// recibe como parametro el modelo de compannia con el correo
        /// </summary>
        /// <param name="companyAndMail"></param>
        /// <param name="filesInfo"></param>
        /// <returns></returns>
        public static BaseResponse UpdateCompany(CompanyAndMail companyAndMail, HttpContext filesInfo)
        {
            try
            {
                return CLVSSUPER.DAO.PostData.UpdateCompany(companyAndMail, filesInfo);
            }
            catch (Exception exc)
            {
                return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
            }
        }

        /// <summary>
        /// Metodo para obtener una lista de la informacion de los usuarios para configuracion 
        ///no recibe parametros
        /// </summary>
        /// <returns></returns>
        public static UserListModel GetUserUserAssignList()
        {
            try
            {
                var userId = GetUserId();
                var company = GetData.GetCompanyByUserId(userId);
                return CLVSSUPER.DAO.GetData.GetUserUserAssignList(company);
            }
            catch (Exception exc)
            {
                return (UserListModel)LogManager.HandleExceptionWithReturn(exc, "UserListModel");
            }
        }

        /// <summary>
        /// metodo para obtener las conexiones de SAP de la DB Local
        /// no recibe parametros 
        /// </summary>
        /// <returns></returns>
        public static SapConnectionResponse GetSapConnection()
        {
            try
            {
                return CLVSSUPER.DAO.GetData.GetSapConnection();
            }
            catch (Exception exc)
            {
                return (SapConnectionResponse)LogManager.HandleExceptionWithReturn(exc, "SapConnectionResponse");
            }
        }

        /// <summary>
        /// metodo para obtener los almacenes con los que cuenta una compania
        /// Recibe el id de la compania dese el front para buscar por compañia seleccionada... 
        /// no logeada.
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        public static StoreListModel GetStoresByCompany(int company)
        {
            try
            {
                //var userId = GetUserId();
                //var company = GetData.GetCompanyByUserId(userId);
                return CLVSSUPER.DAO.GetData.GetStoresByCompany(company);

            }
            catch (Exception exc)
            {
                return (StoreListModel)LogManager.HandleExceptionWithReturn(exc, "StoreListModel");
            }
        }

        /// <summary>
        /// metodo para obtener los almasenes con los que cuenta una compañias
        /// el id de la compania dese el front para buscar por compañia seleccionada
        /// no logeada.
        /// </summary>
        /// <returns></returns>
        public static StoreListModel GetAllStores()
        {
            try
            {
                return CLVSSUPER.DAO.GetData.GetAllStores();
            }
            catch (Exception exc)
            {
                return (StoreListModel)LogManager.HandleExceptionWithReturn(exc, "StoreListModel");
            }
        }

        /// <summary>
        /// funcion para crear un nuevo usuario
        /// recibe como parametro el modelo de usuario para agregar un nuevo registro userAssign
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static BaseResponse CreateNewUser(UserAsingModel user)
        {
            try
            {
                return CLVSSUPER.DAO.PostData.CreateNewUser(user);

            }
            catch (Exception exc)
            {
                return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
            }
        }

        /// <summary>
        /// Funcion para editar un usuario
        /// recibe como parametro el modelo se usuario para editar un nuevo registro userAssign
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static BaseResponse UpdateUser(UserAsingModel user)
        {
            try
            {
                return CLVSSUPER.DAO.PostData.UpdateUser(user);

            }
            catch (Exception exc)
            {
                return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
            }
        }

        /// <summary>
        /// metodo para obtener la informacion de un item de la compannia
        /// recibe como parametro el codigo del item a consultar
        /// </summary>
        /// <param name="cardCode"></param>
        /// <param name="sede"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        public static InvoicesListResp GetPayInvoices(string cardCode, string sede, string currency)
        {
            try
            {
                var userId = GetUserId();
                var company = GetData.GetCompanyByUserId(userId);
                return CLVSSUPER.SAPDAO.GetSapData.GetPayInvoices(cardCode, sede, currency, company);
            }
            catch (Exception exc)
            {
                return (InvoicesListResp)LogManager.HandleExceptionWithReturn(exc, "InvoicesListResp");
            }
        }

        /// <summary>
        /// Metodo para obtener las series de numeracion con los que cuenta una compañias
        /// no recibe parametros 
        /// </summary>
        /// <returns></returns>
        public static NumberingSeriesModelResponse GetSeries()
        {
            try
            {
                var userId = GetUserId();
                var company = GetData.GetCompanyByUserId(userId);
                return CLVSSUPER.DAO.GetData.GetSeries(company);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Obtiene la lista de las enumeraciones con los tipos de series que hay 
        /// ejemplo, facturacion - cotizacion - pagos
        /// no recibe parametros
        /// </summary>
        /// <returns></returns>
        public static enumsResponse GetSeriesType()
        {
            try
            {
                return CLVSSUPER.DAO.GetData.GetSeriesType();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// Obtiene los tipos de serie de numeracion que hay  ejemplo, manual - automatico
        /// no recibe parametros
        /// </summary>
        /// <returns></returns>
        public static enumsResponse GetSeriesTypeNumber()
        {
            try
            {
                return CLVSSUPER.DAO.GetData.GetSeriesTypeNumber();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Funcion que actualiza los cambios cuando se modifica una serie
        /// recibe como parametro el modelo de la serie
        /// </summary>
        /// <param name="serie"></param>
        /// <returns></returns>
        public static BaseResponse UpdateSerie(NumberingSeriesModel serie)
        {
            try
            {
                return CLVSSUPER.DAO.PostData.UpdateSerie(serie);
            }
            catch (Exception exc)
            {
                return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
            }
        }

        /// <summary>
        /// Funcion que crea una nueva serie en la bd
        /// recibe como parametro el modelo de la serie
        /// </summary>
        /// <param name="serie"></param>
        /// <returns></returns>
        public static BaseResponse CreateNewSerie(NumberingSeriesModel serie)
        {
            try
            {
                return CLVSSUPER.DAO.PostData.CreateNewSerie(serie);
            }
            catch (Exception exc)
            {
                return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
            }
        }

        /// <summary>
        /// Funcion que trae las listas de las cuentas
        /// no recibe parametros
        /// </summary>
        /// <returns></returns>
        public static AccountResponse GetAccounts()
        {
            try
            {
                var userId = GetUserId();
                var company = GetData.GetCompanyByUserId(userId);
                return CLVSSUPER.SAPDAO.GetSapData.GetAccounts(company);
            }
            catch (Exception exc)
            {
                return (AccountResponse)LogManager.HandleExceptionWithReturn(exc, "AccountResponse");
            }
        }

        /// <summary>
        /// Funcion que encripta datos en SHA256
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static string GenerateSHA256String(string inputString)
        {
            SHA256 sha256 = SHA256Managed.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hash = sha256.ComputeHash(bytes);
            return GetStringFromHash(hash);
        }

        /// <summary>
        /// Retorna un string a partir del hash recibido
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        private static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }

        /// <summary>
        /// trae las listas de las tarjetas
        /// no recibe parametros
        /// </summary>
        /// <returns></returns>
        public static CardsResponse GetCards()
        {
            try
            {
                var userId = GetUserId();
                var company = GetData.GetCompanyByUserId(userId);
                return CLVSSUPER.SAPDAO.GetSapData.GetCards(company);
            }
            catch (Exception exc)
            {
                return (CardsResponse)LogManager.HandleExceptionWithReturn(exc, "CardsResponse");
            }
        }

        /// <summary>
        /// obtiene la cuenta cuentas de los bancos
        /// no recibe parametros
        /// </summary>
        /// <returns></returns>
        public static BankResponse GetAccountsBank()
        {
            try
            {
                var userId = GetUserId();
                var company = GetData.GetCompanyByUserId(userId);
                return CLVSSUPER.SAPDAO.GetSapData.GetAccountsBank(company);
            }
            catch (Exception exc)
            {
                return (BankResponse)LogManager.HandleExceptionWithReturn(exc, "BankResponse");
            }
        }

        /// <summary>
        /// obtiene la cuenta cuentas de los vendedores
        /// no recibe parametros
        /// </summary>
        /// <returns></returns>
        public static SalesManResponse GetSalesMan()
        {
            try
            {
                var userId = GetUserId();
                var company = GetData.GetCompanyByUserId(userId);
                return CLVSSUPER.SAPDAO.GetSapData.GetSalesMan(company);
            }
            catch (Exception exc)
            {
                return (SalesManResponse)LogManager.HandleExceptionWithReturn(exc, "SalesManResponse");
            }
        }


        /// <summary>
        /// metodo para realizar una cancelacion de un pago en SAP
        /// recibe como parametro el modelo de cancelacion de pago
        /// </summary>
        /// <param name="canPay"></param>
        /// <returns></returns>
        public static BaseResponse CancelPayment(CancelPayModel canPay)
        {
            try
            {
                var userId = GetUserId();
                var company = GetData.GetCompanyByUserId(userId);
                return CLVSSUPER.SAPDAO.PostSapData.CancelPayment(canPay, company);
            }
            catch (Exception exc)
            {
                return (PaymentSapResponse)LogManager.HandleExceptionWithReturn(exc, "PaymentSapResponse");
            }
        }

        /// <summary>
        /// metodos que jala las listas de facturas a las cuales se les va a cancelar el pago
        /// parametros el modelo de datos de la informacion para buscar la lista
        /// </summary>
        /// <param name="canPay"></param>
        /// <returns></returns>
        public static CancelpaymentResponce GetPaymentList(paymentSearchModel canPay)
        {
            try
            {
                var userId = GetUserId();
                var company = GetData.GetCompanyByUserId(userId);
                return CLVSSUPER.SAPDAO.GetSapData.GetPaymentList(company, canPay);
            }
            catch (Exception exc)
            {
                return (CancelpaymentResponce)LogManager.HandleExceptionWithReturn(exc, "CancelpaymentResponce");
            }
        }

        /// <summary>
        /// metodo para registrar un usuario desde la app
        ///recibe como parametro un RegisterUser, que contiene un UserDB
        /// </summary>
        /// <param name="registerUser"></param>
        /// <returns></returns>
        public static BaseResponse RegisterUser(User registerUser)
        {
            try
            {
                bool exist = CLVSSUPER.DAO.GetData.VerifyUserExist(registerUser.Email);
                if (!exist)
                {
                    //return NDESMO.DAO.PostData.RegisterUser(RegisterUser);

                    BaseResponse br = CLVSSUPER.DAO.PostData.RegisterUser(registerUser);
                    if (br.result)
                    {
                        string token = GetToken(registerUser.Email, registerUser.Password);
                        string UrlVerificationAccount = System.Configuration.ConfigurationManager.AppSettings["UrlVerificationAccount"].ToString();
                        string linkMsg = "Completar Registro";
                        string body = string.Format("Estimado Usuario<BR/>Para completar el registro por favor ingrese al siguiente link: <a href=\"{0}\" title=\"User Email Confirm\">{1}</a>", UrlVerificationAccount + token + "/1", linkMsg);
                        SendEmail(registerUser.Email, registerUser.FullName, body);
                    }
                    return br;
                }
                return new BaseResponse
                {
                    result = false,
                    errorInfo = new ErrorInfo
                    {
                        Code = -1,
                        Message = "El usuario: " + registerUser.Email + " ya esta registrado en la aplicacion"
                    }
                };
            }
            catch (Exception exc)
            {
                return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
            }
        }

        /// <summary>
        /// Metodo para recuperar la contrasenna del usuario, enviando un correo para la recuperacion
        /// recibe como parametro un StringModel, que contiene un string con el correo
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public static BaseResponse SendRecoverPswdEmail(StringModel userEmail)
        {
            try
            {
                bool exist = CLVSSUPER.DAO.GetData.VerifyUserExist(userEmail.word);
                if (exist)
                {
                    string UrlRecoverPswd = System.Configuration.ConfigurationManager.AppSettings["UrlRecoverPswd"].ToString();
                    string linkMsg = "Recuperacion de contraseña";
                    string Body = string.Format("Estimado Usuario<BR/>Intento de recuperacion de contraseña, para completar el registro por favor ingrese al siguiente link: <a href=\"{1}\" title=\"User Email Confirm\">{2}</a>", userEmail.word, UrlRecoverPswd + userEmail.word, linkMsg);
                    SendEmail(userEmail.word, linkMsg, Body);
                    return new BaseResponse
                    {
                        result = true
                    };
                }
                return new BaseResponse
                {
                    result = false,
                    errorInfo = new ErrorInfo
                    {
                        Code = -1,
                        Message = "El usuario: " + userEmail.word + " no esta registrado en la aplicacion"
                    }
                };
            }
            catch (Exception exc)
            {
                return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
            }
        }

        /// <summary>
        /// Metodo para obtener el token al registrar un usuario nuevo
        /// Recibe como parametro el correo y la contrasenna
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string GetToken(string email, string password)
        {
            string FeAppToken = System.Configuration.ConfigurationManager.AppSettings["AppToken"].ToString();
            var client = new RestClient(FeAppToken);
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("application/x-www-form-urlencoded", "grant_type=password&UserName=" + email + "&Password=" + password, RestSharp.ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            var content = response.Content; // raw content as string
            var token = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<TokenModel>(content);
            return token.access_token;
        }

        /// <summary>
        /// Metodo para enviar un correo de confirmacion de la cuenta
        /// Recibe como parametro el correo y la contrasena
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public static void SendEmail(string to, string subject, string body)
        {
            var message = new MailMessage();
            message.To.Add(new MailAddress(to));
            message.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["Email"].ToString());
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;
            message.BodyEncoding = UTF8Encoding.UTF8;
            message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = System.Configuration.ConfigurationManager.AppSettings["EmailUser"].ToString(),
                    Password = System.Configuration.ConfigurationManager.AppSettings["EmailPassword"].ToString()
                };
                smtp.Port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Port"]);
                smtp.Host = System.Configuration.ConfigurationManager.AppSettings["Host"].ToString();
                smtp.EnableSsl = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["EnableSsl"]);
                smtp.Timeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = credential;
                smtp.Send(message);
            }
        }

        /// <summary>
        /// metodo para el envio de la informcion para el cambio de la contrasenna de la cuenta del usuario
        /// recibe como parametro un recoverPswd, que contiene un recoverPswd
        /// </summary>
        /// <param name="recoverPswd"></param>
        /// <returns></returns>
        public static BaseResponse RecoverPswd(User recoverPswd)
        {
            try
            {
                bool exist = CLVSSUPER.DAO.GetData.VerifyUserExist(recoverPswd.Email);
                if (exist)
                {
                    return CLVSSUPER.DAO.PostData.RecoverPswd(recoverPswd);
                }
                return new BaseResponse
                {
                    result = false,
                    errorInfo = new ErrorInfo
                    {
                        Code = -1,
                        Message = "El usuario: " + recoverPswd.Email + " no esta registrado en la aplicacion"
                    }
                };
            }
            catch (Exception exc)
            {
                return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
            }
        }

        /// <summary>
        /// metodos que jala las listas de facturas para mostrar en la vista de reimprecion
        /// parametros el modelo de datos de la informacion para buscar la lista
        /// </summary>
        /// <param name="inv"></param>
        /// <returns></returns>
        public static InvListPrintResponde GetInvPrintList(invPrintSearch inv)
        {
            try
            {
                var userId = GetUserId();
                var company = GetData.GetCompanyByUserId(userId);
                return CLVSSUPER.SAPDAO.GetSapData.GetInvPrintList(company, inv);
            }
            catch (Exception exc)
            {
                return (InvListPrintResponde)LogManager.HandleExceptionWithReturn(exc, "InvListPrintResponde");
            }
        }


        /// <summary>
        /// actualiza los cambios cuando se un parametro asociado a una vista
        /// recibe como parametro una lisata de un modelo de parametros para una vista
        /// </summary>
        /// <param name="Params"></param>
        /// <returns></returns>
        public static BaseResponse UpdateParamsViewState(List<ParamsModel> Params)
        {
            try
            {
                return CLVSSUPER.DAO.PostData.UpdateParamsViewState(Params);

            }
            catch (Exception exc)
            {
                return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
            }
        }

        /// <summary>
        /// Trae la informacion de las listas de precios 
        /// no recibe parametros
        /// </summary>
        /// <returns></returns>
        public static PriceListResponse GetPriceList()
        {
            try
            {
                var userId = GetUserId();
                var company = GetData.GetCompanyByUserId(userId);
                return CLVSSUPER.SAPDAO.GetSapData.GetPriceList(company);

            }
            catch (Exception exc)
            {
                return (PriceListResponse)LogManager.HandleExceptionWithReturn(exc, "PriceListResponse");
            }
        }

        /// <summary>
        /// Trae la informacion de los terminos de pagos
        /// no recibe parametros
        /// </summary>
        /// <returns></returns>
        public static PayTermsResponse GetPayTermsList()
        {
            try
            {
                var userId = GetUserId();
                var company = GetData.GetCompanyByUserId(userId);
                return CLVSSUPER.SAPDAO.GetSapData.GetPayTermsList(company);

            }
            catch (Exception exc)
            {
                return (PayTermsResponse)LogManager.HandleExceptionWithReturn(exc, "PayTermsResponse");
            }
        }

        /// <summary>
        /// Funcion para obtener el logo de la compannia desde DBLocal, obteniendo la compannia a la cual se esta logueado
        /// no recibe parametros
        /// </summary>
        /// <returns></returns>
        public static BaseResponse GetCompanyLogo()
        {
            try
            {
                var userId = GetUserId();
                var company = GetData.GetCompanyByUserId(userId);
                return CLVSSUPER.DAO.GetData.GetCompanyLogo(company);
            }
            catch (Exception exc)
            {
                return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
            }
        }

        /// <summary>
        /// metodo para realizar una factura con el pago pago en SAP
        /// recibe como parametro el modelo de la factura con el pago
        /// </summary>
        /// <param name="createInvoice"></param>
        /// <returns></returns>
        public static InvoiceSapResponse CreateInvoice(CreateInvoice createInvoice)
        {
            try
            {
                var userId = GetUserId();
                var company = GetData.GetCompanyByUserId(userId);
                return CLVSSUPER.SAPDAO.PostSapData.CreateInvoice(createInvoice, company, userId);
            }
            catch (Exception exc)
            {
                return (InvoiceSapResponse)LogManager.HandleExceptionWithReturn(exc, "InvoiceSapResponse");
            }
        }

        /// <summary>
        /// Trae la informacion de los terminos de pagos
        /// no recibe parametros
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public static WHPlaceResponce GetStoresList(int companyId)
        {
            try
            {
                //var userId = GetUserId();

                var company = GetData.GetCompanyBycompanyId(companyId);
                return CLVSSUPER.SAPDAO.GetSapData.GetStoresList(company);

            }
            catch (Exception exc)
            {
                return (WHPlaceResponce)LogManager.HandleExceptionWithReturn(exc, "WHPlaceResponce");
            }
        }

        /// <summary>
        /// crea un nuevo almecen para una compañia
        /// de parametro obtiene un modelo de tipo Store
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        public static BaseResponse CreateStore(StoresModel store)
        {
            try
            {
                return CLVSSUPER.DAO.PostData.CreateStore(store);

            }
            catch (Exception exc)
            {
                return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
            }
        }

        /// <summary>
        /// Funcion que actualiza un almacen en especifico
        /// Como parametro recibe un modelo tipo Store
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        public static BaseResponse UpdateStore(StoresModel store)
        {
            try
            {
                return CLVSSUPER.DAO.PostData.UpdateStore(store);

            }
            catch (Exception exc)
            {
                return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
            }
        }

        public static StoreModelResult GetStorebyId(int store)
        {
            try
            {
                //var userId = GetUserId();
                //var company = GetData.GetCompanyBycompanyId(companyId);
                return CLVSSUPER.DAO.GetData.GetStorebyId(store);

            }
            catch (Exception exc)
            {
                return (StoreModelResult)LogManager.HandleExceptionWithReturn(exc, "StoreModelResult");
            }
        }

        /// <summary>
        /// Metodo para verificar la cuenta del propietario 
        /// </summary>
        /// <returns></returns>
        public static BaseResponse ConfirmEmail()
        {
            try
            {
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                var userId = identity.Claims.Where(c => c.Type == "userId").Single().Value;
                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    try
                    {
                        using (DbContextTransaction transactionDao = dbDao.Database.BeginTransaction())
                        {
                            var user = dbDao.Users.Find(userId);

                            if (user != null)
                            {
                                user.EmailConfirmed = true;
                                dbDao.Entry(user).State = EntityState.Modified;
                                dbDao.SaveChanges();
                                transactionDao.Commit();
                            }
                            else
                            {
                                return new BaseResponse
                                {
                                    result = false,
                                    errorInfo = new ErrorInfo
                                    {
                                        Code = -1,
                                        Message = "No se encontro el usuario"
                                    }
                                };
                            }
                        }
                        return new BaseResponse
                        {
                            result = true
                        };
                    }
                    catch (Exception exc)
                    {
                        return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
                    }
                }
            }
            catch (Exception exc)
            {
                return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
            }
        }

        public static PermsModelResponse GetPermsByUserMenu()
        {
            try
            {
                var userId = GetUserId();
                return CLVSSUPER.DAO.GetData.GetPermsByUser(userId);

            }
            catch (Exception exc)
            {
                return (PermsModelResponse)LogManager.HandleExceptionWithReturn(exc, "PermsModelResponse");
            }
        }

        public static discountResponse GetDiscount()
        {
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            var userId = identity.Claims.Where(c => c.Type == "userId").Single().Value;
            try
            {
                decimal disc = CLVSSUPER.DAO.GetData.getDiscount(userId);
                return new discountResponse
                {
                    discount = disc,
                    result = true
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static BalanceByUserResponse GetBalanceInvoices_UsrOrTime(GetBalanceModel_UsrOrDate BalanceModel)
        {
            try
            {
                var userId = GetUserId();
                var company = GetData.GetCompanyByUserId(userId);
                BalanceModel.DBCode = company.DBCode.ToString();
                return  SAPDAO.GetSapData.GetBalanceInvoices_UsrOrTime(BalanceModel);
            }
            catch(Exception exc) {
                return (BalanceByUserResponse)LogManager.HandleExceptionWithReturn(exc, "BalanceByUserResponse");
            }
            
        }
    }
}
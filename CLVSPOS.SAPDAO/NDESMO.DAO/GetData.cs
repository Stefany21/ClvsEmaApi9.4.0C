using CLVSSUPER.COMMON;
using CLVSSUPER.MODELS;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static CLVSSUPER.COMMON.Constants;
using System.Security.Claims;
using System.Threading;

namespace CLVSSUPER.DAO
{
    public class GetData
    {
        private static Exception ParseSourceException(Exception exc) {
            return new Exception(exc.InnerException != null ? exc.InnerException.InnerException != null ? exc.InnerException.InnerException.Message : exc.InnerException.Message : exc.Message);           
        }

        /// <summary>
        /// Metodo que obtiene las compannias de la DBLocal
        /// Recibe el codigo de la compañia como parametro
        /// </summary>
        /// <param name="dbCode"></param>
        /// <returns></returns>
        public static Companys GetCompanyByBDCode(string dbCode)
        {
            try
            {
                Companys co = new Companys();

                using (SuperV2_Entities db = new SuperV2_Entities())
                {
                    co = db.Companys.Where(x => x.DBCode == dbCode).Include(b => b.SAPConnection).FirstOrDefault();

                    if (co.Id != 0) { return co; }
                    else { throw new Exception("DAO/GetData/GetCompanyByBDCode/ - No existen Compañías con ese código de Base de Datos."); }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Companys GetCompanyBycompanyId(int Id)
        {
            try
            {
                Companys co = new Companys();
                using (SuperV2_Entities db = new SuperV2_Entities())
                {
                    co = db.Companys.Where(x => x.Id == Id).Include(b => b.SAPConnection).FirstOrDefault();

                    if (co.Id != 0) { return co; }
                    else { throw new Exception("DAO/GetData/GetCompanyByBDCode/ - No existen Compañías con ese código de Base de Datos."); }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Obtiene el mapa del ususario a partir de la compañia
        /// Recibe al Id de usuario como parametro
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static int GetUserMappIdByUserID(string user)
        {
            try
            {
                List<UserAssign> userAsing = new List<UserAssign>();

                using (SuperV2_Entities db = new SuperV2_Entities())
                {
                    userAsing = db.UserAssign.ToList();

                    foreach (UserAssign x in userAsing)
                    {
                        if (x.UserId == user)
                        {
                            return x.Id;
                        }
                    }

                    throw new Exception("El usuario seleccionado no esta asignado a una compañía");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("DAO/GetData/GetUserMappIdByUserID/ - Error al intentar cargar la informacion de usuario - {0}", ex.Message));

            }
        }

        /// <summary>
        /// Obtiene la lista de permisos de un usuario por compañia
        /// Recibe el id de usuario como parametro
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static PermsModelResponse GetPermsByUser(string user)
        {
            //REVC NO SE SI DESPUES SE PUEDA USAR LA COMPAÑIA PARA EL ID DEL USER            
            try
            {
                using (SuperV2_Entities db = new SuperV2_Entities())
                {
                    List<Permissions> perm = db.Permissions.ToList();
                    List<PermsByUser> pbu = new List<PermsByUser>();

                    if (user != "0")
                    {
                        int userMapId = GetUserMappIdByUserID(user.ToString());
                        pbu = db.PermsByUser.Where(x => x.UserMappId == userMapId).ToList();
                    }

                    List<PermissionsModel> _prms = new List<PermissionsModel>();
                        foreach (Permissions p in perm)
                        {
                            _prms.Add(new PermissionsModel
                            {
                                Id = p.Id,
                                Name = p.Name,
                                Description = p.Description,
                                Active = user != "0" ? pbu.Where(x => x.PermId == p.Id).Count() > 0 ? 
                                                                     pbu.Where(x => x.PermId == p.Id).Select(x => x.BoolValue).FirstOrDefault() 
                                                                     : false 
                                                     : p.Active
                            });
                        }
                    return new PermsModelResponse
                    {
                        result = true,
                        perms = _prms
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// validar usuario
        /// recibe como parametro el password del cliente y el username
        /// </summary>
        /// <param name="clientPass"></param>
        /// <param name="userName"></param>
        /// <param name="isLogin"></param>
        /// <returns></returns>
        public bool ValidateAccess(string clientPass, string userName, bool isLogin)
        {
            try
            {
                using (SuperV2_Entities db = new SuperV2_Entities())
                {
                    string encryptClienPass = Common.Encrypt(clientPass);

                    List<Users> result = new List<Users>();

                    if (isLogin)
                    {
                        result = db.Users.Where(x => x.UserName == userName && x.PasswordHash == encryptClienPass && x.EmailConfirmed && x.Active).ToList();
                    }
                    else {
                        result = db.Users.Where(x => x.UserName == userName && x.PasswordHash == encryptClienPass).ToList();
                    }

                    if (result != null && result.Count > 1)
                    {
                        throw new Exception("Disculpas, parece que existe un error con usuarios, por favor contacte a Clavisco!");
                    }

                    else if (result != null && result.Count == 1)
                    {
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ParseSourceException(ex);
            }
        }

        /// <summary>
        /// Funcion que devuelve el userId del user logueado
        /// No recibe parametro
        /// </summary>
        /// <param name="password"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static string GetUserId(string password, string userName)
        {
            try
            {
                var userId = string.Empty;
                string encryptUserId = Common.Encrypt(password);

                using (SuperV2_Entities db = new SuperV2_Entities())
                {
                    userId = db.Users.Where(x => x.UserName == userName && x.PasswordHash == encryptUserId).FirstOrDefault().Id;
                }

                return userId;
            }
            catch (Exception ex)
            {
                throw ParseSourceException(ex); 
            }
        }

        /// <summary>
        /// retorna la lista de los usuarios para ser usada en la vista de configuracion de usuarios;
        /// </summary>
        /// <returns></returns>
        public static UserModelResponse GetUserList()
        {
            try
            {
            //    var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            //    var userId = identity.Claims.Where(c => c.Type == "userId").Single().Value;
            //    var company = GetData.GetCompanyByUserId(userId);
                List<Users> ListaUsuarios = new List<Users>();
                List<UserModel> Userlist = new List<UserModel>();

                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    ListaUsuarios = (from User in dbDao.Users
                                //join cbu in dbDao.CompanyByUsers on User.Id equals cbu.UserId
                                //where cbu.CompanyId == company.Id
                                select User).ToList();        
                }

                foreach (Users x in ListaUsuarios)
                {
                    Userlist.Add(new UserModel
                    {
                        Id = x.Id,
                        FullName = x.FullName,
                        Email = x.Email,
                        EmailConfirmed = x.EmailConfirmed,
                        UserName = x.UserName,
                        PasswordHash = x.PasswordHash,
                        CreateDate = x.CreateDate,
                        Active = x.Active
                    });
                }

                return new UserModelResponse
                {
                    result = true,
                    users = Userlist,
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Metodo que obtiene la compannias de la DBLocal del usuario logueado
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static Companys GetCompanyByUserId(string userId)
        {
            try
            {
                Companys company = new Companys();
                using (SuperV2_Entities db = new SuperV2_Entities())
                {
                    //                    int userid = Convert.ToInt32(userId);
                    var userAssign = db.UserAssign.Where(x => x.UserId == userId).Select(x => x).FirstOrDefault();
                    if (userAssign != null)
                    {
                        //                        company = db.Companys.Find(userAssign.CompanyId);
                        company = db.Companys.Where(x => x.Id == userAssign.CompanyId).Include(b => b.SAPConnection).FirstOrDefault();
                        if (company != null)
                        {
                            return company;
                        }
                        else
                        {
                            throw new Exception("DAO/GetData/GetCompanyByUserId/ - No existen Compañías para el usuario logueado.");
                        }
                    }
                    else
                    {
                        throw new Exception("DAO/GetData/GetCompanyByUserId/ - No existen la asignacion de usuario para el usuario logueado.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ParseSourceException(ex);
            }
        }

        /// <summary>
        /// Metodo que obtiene la asignacion del usuario logueado de la DBLocal del usuario logundeeado
        /// Recibe como parametro el id del usuario
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static UserAssign GetUserMappId(string userId)
        {
            try
            {
                Companys company = new Companys();
                using (SuperV2_Entities db = new SuperV2_Entities())
                {
                    //int userid = Convert.ToInt32(userId);
                    return db.UserAssign.Where(x => x.UserId == userId).FirstOrDefault();                
                }
            }
            catch (Exception ex)
            {
                throw ParseSourceException(ex);
            }
        }

        /// <summary>
        /// Metodo para obtener las compannias registradas en la aplicacion
        /// </summary>
        /// <returns></returns>
        public static CompanyListResponse GetCompanies()
        {
            try
            {
                List<CompanysSapConnModel> companyList = new List<CompanysSapConnModel>();

                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    companyList = dbDao.Companys.Select(x => new CompanysSapConnModel
                    {
                        Id = x.Id,
                        Server = x.SAPConnection.Server,
                        DBName = x.DBName,
                        DBCode = x.DBCode,
                        Active = x.Active
                    }).ToList();
                }

                return new CompanyListResponse
                {
                    result = true,
                    companiesList = companyList,
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// metodo para obtener las paremitraciones de una vista especific
        /// como parametro recive a la vista que pertenesen las paramtizaciones.
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public static ParamsViewResponse GetViewParam(int view)
        {
            List<ParamsModel> paramsModel = new List<ParamsModel>();
            try
            {
                using (SuperV2_Entities db = new SuperV2_Entities())
                {
                    //Params = db.ParamsViewCompany.Where(x => x.ViewsId == view).Select(x => new ParamsModel
                    //{
                    //    CompanyId = x.CompanyId,
                    //    Descrip = x.Descrip,
                    //    Display = x.Display,
                    //    Id = x.Id,
                    //    Order = x.Order,
                    //    ParamsId = x.ParamsId,
                    //    Text = x.Text,
                    //    ViewsId = x.ViewsId,
                    //    Visibility = x.Visibility,
                    //    Name = x.V_ViewParams.Name,
                    //    type = x.V_ViewParams.Type
                    //}).ToList();

                    //Params = Params.OrderBy(o => o.Order).ToList();

                    paramsModel = db.ParamsViewCompany.Where(x => x.ViewsId == view).Select(x => new ParamsModel
                    {
                        CompanyId = x.CompanyId,
                        Descrip = x.Descrip,
                        Display = x.Display,
                        Id = x.Id,
                        Order = x.Order,
                        ParamsId = x.ParamsId,
                        Text = x.Text,
                        ViewsId = x.ViewsId,
                        Visibility = x.Visibility,
                        Name = x.V_ViewParams.Name,
                        type = x.V_ViewParams.Type
                    }).OrderBy(o => o.Order).ToList();

                    //Params = Params.OrderBy(o => o.Order).ToList();



                    return new ParamsViewResponse
                    {
                        result = true,
                        Params = paramsModel
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Metodo para obtener la informacion de una compannia por el id de la misma
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public static CompanyResponse GetCompanyById(int companyId)
        {
            try
            {
                List<CompanysSapConnModel> companyList = new List<CompanysSapConnModel>();

                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    CompanyAndMail cm = new CompanyAndMail();
                    Companys company = dbDao.Companys.Find(companyId);

                    cm.company = new CompanysModel
                    {
                        Id = company.Id,
                        DBName = company.DBName,
                        DBCode = company.DBCode,
                        SAPConnectionId = company.SAPConnectionId,
                        ExchangeRate = company.ExchangeRate,
                        ExchangeRateValue = company.ExchangeRateValue,
                        HandleItem = company.HandleItem,
                        BillItem = company.BillItem,
                        Active = company.Active,
                        LogoPath = company.LogoPath,
                        SP_ItemInfo = company.SP_ItemInfo,
                        SP_CancelPayment = company.SP_CancelPayment,
                        SP_InvoiceInfoPrint = company.SP_InvoiceInfoPrint,
                        SP_PayDocuments = company.SP_PayDocuments,
                        SP_SeriesByItem = company.SP_SeriesByItem,
                        SP_WHAvailableItem = company.SP_WHAvailableItem,
                        V_BPS = company.V_BPS,
                        V_Items = company.V_Items,
                        V_ExRate = company.V_ExRate,
                        V_Taxes = company.V_Taxes,
                        V_GetAccounts = company.V_GetAccounts,
                        V_GetCards = company.V_GetCards,
                        V_GetBanks = company.V_GetBanks,
                        V_GetSalesMan = company.V_GetSalesMan
                    };

                    cm.mail = null;

                    if (company.V_MailData != null)
                    {
                        cm.mail = new MailDataModel
                        {
                            Id = company.V_MailData.Id,
                            subject = company.V_MailData.subject,
                            from = company.V_MailData.from,
                            user = company.V_MailData.user,
                            pass = company.V_MailData.pass,
                            port = company.V_MailData.port,
                            Host = company.V_MailData.Host,
                            SSL = company.V_MailData.SSL
                        };
                    }

                    if (System.IO.File.Exists(company.LogoPath))
                    {
                        byte[] bytesImagen = System.IO.File.ReadAllBytes(company.LogoPath);
                        string imagenBase64 = Convert.ToBase64String(bytesImagen);
                        string tipoContenido = GetFileTypeContent(Path.GetExtension(company.LogoPath).ToLowerInvariant());                        

                        string b64 = string.Format("data:{0};base64,{1}", tipoContenido, imagenBase64);
                    }
                    return new CompanyResponse
                    {
                        result = true,
                        companyAndMail = cm,
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// metodo para obtener las compannias registradas en la aplicacion
        /// </summary>
        /// <returns></returns>
        public static SapConnectionResponse GetSapConnection()
        {
            try
            {
                List<SAPConnectionModel> SAPConnectionList = new List<SAPConnectionModel>();

                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    SAPConnectionList = dbDao.SAPConnection.Select(x => new SAPConnectionModel
                    {
                        Id = x.Id,
                        BoSuppLangs = x.BoSuppLangs,
                        DBEngine = x.DBEngine,
                        DBPass = x.DBPass,
                        DBUser = x.DBUser,
                        DST = x.DST,
                        LicenseServer = x.LicenseServer,
                        ODBCType = x.ODBCType,
                        Server = x.Server,
                        ServerType = x.ServerType,
                        UseTrusted = x.UseTrusted
                    }).ToList();

                    return new SapConnectionResponse
                    {
                        result = true,
                        SAPConnections = SAPConnectionList,
                    };
                }
            }
            catch (Exception)
            {           
                throw;
            }
        }

        /// <summary>
        /// Genera una lista de usuarios para ser mostrados en la vista de configuracion de usuarios
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        public static UserListModel GetUserUserAssignList( Companys company)
        {
            List<UserAsingModel> user = new List<UserAsingModel>();

            try
            {
                using (SuperV2_Entities db = new SuperV2_Entities())
                {
                    user = db.UserAssign.Where(x => x.CompanyId == company.Id).Select(x => new UserAsingModel
                    {
                        Id = x.Id,
                        UserId = x.UserId,
                        Active = x.Active,
                        CenterCost = x.CenterCost,
                        CompanyId = x.CompanyId,
                        minDiscount = x.minDiscount,
                        PriceListDef = x.PriceListDef,
                        SAPPass = x.SAPPass,
                        SAPUser = x.SAPUser,
                        SlpCode = x.SlpCode,
                        StoreId = x.StoreId,
                        SuperUser = x.SuperUser,
                        UserName = db.Users.Where(y => y.Id == x.UserId).Select(y => y.FullName).FirstOrDefault(),
                        CompanyName = db.Companys.Where(y => y.Id == x.CompanyId).Select(y => y.DBCode).FirstOrDefault(),
                        StoreName = db.Store.Where(y => y.Id == x.StoreId).Select(y => y.Name).FirstOrDefault(),
                        Series = x.V_SeriesByUser.Where(y => y.UsrMappId == x.Id).Select(y => new SeriesByUserModel
                        {
                            Id = y.Id,
                            SerieId = y.SerieId,
                            UsrMappId = y.UsrMappId,
                            Name = y.V_Series.Name,
                            type = y.V_Series.DocType
                        }).ToList()
                    }).ToList();
                }

                return new UserListModel
                {
                    result = true,
                    Users = user
                };
            }
            catch (Exception ex)
            {
                throw ParseSourceException(ex);
            }
        }

        /// <summary>
        /// Genera una lista con todas los almasenes de la Compañia 
        /// el id de la compañia dese el front para buscar por compañia seleccionada... no logeada.
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        public static StoreListModel GetStoresByCompany(int company)
        {
            List<StoresModel> Stores = new List<StoresModel>();

            try
            {
                using (SuperV2_Entities db = new SuperV2_Entities())
                {
                    Stores = db.Store.Where(x => x.CompanyId == company).Select(x => new StoresModel
                    {
                        Id = x.Id,
                        StoreName = x.Name
                    }).ToList();
                }

                return new StoreListModel
                {
                    result = true,
                    Stores = Stores
                };
            }
            catch (Exception ex)
            {
                throw ParseSourceException(ex);
            }

        }

        /// <summary>
        /// genera una lista con todas los almasenes de la empresa 
        /// el id de la compañia dese el front para buscar por compañia seleccionada... no logeada.
        /// </summary>
        /// <returns></returns>
        public static StoreListModel GetAllStores()
        {
            List<StoresModel> Stores = new List<StoresModel>();

            try
            {
                using (SuperV2_Entities db = new SuperV2_Entities())
                {
                    Stores = db.Store.Select(x => new StoresModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        StoreCode = x.WhCode,
                        StoreName = x.WhName,
                        StoreStatus = x.Active,
                        companyName = db.Companys.Where(y => y.Id == x.CompanyId).Select(y => y.DBName).FirstOrDefault().ToString()

                    }).ToList();
                }

                return new StoreListModel
                {
                    result = true,
                    Stores = Stores
                };
            }
            catch (Exception ex)
            {
                throw ParseSourceException(ex);
            }
        }

        /// <summary>
        /// genera una lista con todas series de numeracion de la empresa.  
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        public static NumberingSeriesModelResponse GetSeries(Companys company)
        {
            try
            {
                using (SuperV2_Entities db = new SuperV2_Entities())
                {
                    var arrValues = Enum.GetValues(typeof(Constants.SerialType));
                    var arrNames = Enum.GetNames(typeof(Constants.SerialType));

                    List<EnumsList> enums = arrNames.Select((nombre, index) => new EnumsList
                    {
                        Value = Convert.ToInt32(arrValues.GetValue(index)),
                        Name = nombre
                    }).ToList<EnumsList>();

                    List<NumberingSeriesModel> Series = db.Series.Where(x => x.CompanyId == company.Id).Select(x => new NumberingSeriesModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Serie = x.Serie,
                        Numbering = x.Numbering,
                        CompanyId = x.CompanyId,
                        DocType = x.DocType,
                        Active = x.Active,
                        CompanyName = company.DBName
                    }).ToList();

                    foreach (NumberingSeriesModel n in Series)
                    {
                        foreach (EnumsList e in enums)
                        {
                            if (n.DocType == e.Value) { n.typeName = e.Name.ToString(); }
                        }
                    }

                    return new NumberingSeriesModelResponse
                    {
                        result = true,
                        Series = Series
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// obtiene la lista de las enumeraciones con los tipos de series que hay ejemplo, facturacion - cotizacion - pagos
        /// </summary>
        /// <returns></returns>
        public static enumsResponse GetSeriesType()
        {
            try
            {
                var arrValues = Enum.GetValues(typeof(Constants.SerialType));
                var arrNames = Enum.GetNames(typeof(Constants.SerialType));

                List<EnumsList> enums = arrNames.Select((nombre, index) => new EnumsList
                {
                    Value = Convert.ToInt32(arrValues.GetValue(index)),
                    Name = nombre
                }).ToList<EnumsList>();

                return new enumsResponse
                {
                    result = true,
                    Enums = enums
                };
            }
            catch (Exception){
                throw;
            }
        }

        /// <summary>
        /// obtiene los tipos de serie de numeracion que hay  
        /// ejemplo, manual - automatico
        /// </summary>
        /// <returns></returns>
        public static enumsResponse GetSeriesTypeNumber()
        {
            try
            {
                var arrValues = Enum.GetValues(typeof(SerialNumberTypes));
                var arrNames = Enum.GetNames(typeof(SerialNumberTypes));

                List<EnumsList> enums = arrNames.Select((nombre, index) => new EnumsList
                {
                    Value = Convert.ToInt32(arrValues.GetValue(index)),
                    Name = nombre
                }).ToList<EnumsList>();

                return new enumsResponse
                {
                    result = true,
                    Enums = enums
                };
            }
            catch (Exception) {
                throw;
            }
        }

        /// <summary>
        /// Metodo que obtiene  si existe ya un correo registrado en la DB
        ///  Recibe como parametro el correo del usuario
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool VerifyUserExist(string email)
        {
            try
            {
                using (SuperV2_Entities db = new SuperV2_Entities())
                {
                    return db.Users.Any(x => x.Email == email);
                }
            }
            catch (Exception ex)
            {
                throw ParseSourceException(ex);
            }
        }

        private static string GetFileTypeContent(string fileExtension) {
            try
            {
                return fileExtension.Replace(".", "image/");
            }
            catch (Exception) {
                return null;
            }                        
        }

        /// <summary>
        /// Funcion para obtener el logo de la compannia desde la DBLocal
        /// obteniendo la compania a la cual se esta logueado
        /// Recibe un objeto compania como parametro
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        public static LogoCompanyResponse GetCompanyLogo(Companys company)
        {
            try
            {
                byte[] bytesImagen = System.IO.File.ReadAllBytes(company.LogoPath);
                string imagenBase64 = Convert.ToBase64String(bytesImagen);
                string tipoContenido = GetFileTypeContent(Path.GetExtension(company.LogoPath).ToLowerInvariant());
                
                return new LogoCompanyResponse
                {
                    result = true,
                    LogoB64 = string.Format("data:{0};base64,{1}", tipoContenido, imagenBase64)
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Metodo que obtiene las monedas de una Vista SQL [CurrencyType]
        /// </summary>
        /// <returns></returns>
        public static List<CurrencyModel> GetCurrencyType()
        {
            try
            {
                List<CurrencyModel> DocCurreny = new List<CurrencyModel>();
                using (SuperV2_Entities db = new SuperV2_Entities())
                {                    
                  return DocCurreny = db.Database.SqlQuery<CurrencyModel>("SELECT * from CLVS_POS_GETCURRENCYTYPE").ToList<CurrencyModel>();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Retorna un almacen por el ID
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        public static StoreModelResult GetStorebyId(int store)
        {
            StoresModel Stores = new StoresModel();
            try
            {
                using (SuperV2_Entities db = new SuperV2_Entities())
                {
                    Stores = db.Store.Where(x=> x.Id == store).Select(x => new StoresModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        StoreCode = x.WhCode,
                        StoreName = x.WhName,
                        StoreStatus = x.Active,
                        companyName = db.Companys.Where(y => y.Id == x.CompanyId).Select(y => y.Id).FirstOrDefault().ToString()

                    }).FirstOrDefault();
                }

                return new StoreModelResult
                {
                    result = true,
                    Store = Stores
                };
            }
            catch (Exception ex)
            {
                throw ParseSourceException(ex);
            }

        }

        /// <summary>
        /// Retorna el descuento del user asing
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static decimal getDiscount(string userId)
        {
            try
            {
                using (SuperV2_Entities db = new SuperV2_Entities())
                {
                   return Convert.ToDecimal( db.UserAssign.Where(x => x.UserId == userId).Select(x => x.minDiscount).FirstOrDefault());
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
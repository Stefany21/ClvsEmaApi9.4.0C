using CLVSPOS.COMMON;
using CLVSPOS.MODELS;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using static CLVSPOS.COMMON.Constants;
using System.Security.Claims;
using System.Threading;
using CLVSSUPER.MODELS;
using Newtonsoft.Json;
using System.Data;

namespace CLVSPOS.DAO
{
    public class GetData
    {
        private static Exception ParseSourceException(Exception exc)
        {
            return new Exception(exc.InnerException != null ? exc.InnerException.InnerException != null ? exc.InnerException.InnerException.Message : exc.InnerException.Message : exc.Message);
        }

		public static PPTerminalsByUserResponse GetPPTerminalsByUser(string _userId)
		{
            try
            {
                List<PPTerminalByUser> PPTerminalsByUser = null;

                using (SuperV2_Entities db = new SuperV2_Entities())
                {
                    PPTerminalsByUser = db.PPTerminalByUser.Where(x => x.UserId.Equals(_userId)).ToList();
                }

                bool result = PPTerminalsByUser != null && PPTerminalsByUser.Count > 0;

                return new PPTerminalsByUserResponse
                {
                    Result = result,
                    PPTerminalsByUser = PPTerminalsByUser,
                    Error = new ErrorInfo()
					{
                        Code = -1,
                        Message = result ?"":"No cuenta con terminales asignados"
					}
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

		/// <summary>
		/// Devuelve un registro del tipo de objeto en funcion
		/// a la llave primario del mismo objeto
		/// </summary>
		/// <typeparam name="T">Representa el tipo de objeto que se va a manipular</typeparam>
		/// <typeparam name="I">Indica el tipo de indice porque se va a filtrar en el sp</typeparam>
		/// <param name="_objectName">Indica el nombre del objeto de la base de datos a consumir</param>
		/// <param name="_object">Representa el tipo de objeto que se va a trabajar ejemplo, usuarios, companias</param>
		/// <param name="_index">Es el valor que se va a usar en el objeto de la bd para filtrar</param>
		/// <returns>Devuelve un objeto de tipo T</returns>
		public static ApiResponse<T> GetRecordByKey<T, I>(string _objectName, I _index) where T : new()
        {
            SuperV2_Entities db = new SuperV2_Entities();

            try
            {

                string DB_OBJECT = GetNameObject(_objectName);// Revisar
                string PARSED_FILTER = typeof(I).Name.Equals("String") ? String.Concat("'", _index, "'") : String.Concat("", _index);
                string QUERY = $"EXEC dbo.{DB_OBJECT} {PARSED_FILTER}";
                DataTable oDataTable = Common.GetDataTable(db.Database.Connection, QUERY);

                if (oDataTable.Rows.Count == 0)
                {
                    return new ApiResponse<T>
                    {
                        Result = false,
                        Error = new ErrorInfo()
                        {

                            Code = -401,
                            Message = "No se han encontrado registros"
                        }
                    };
                }

                DataRow oDataRow = oDataTable.Rows[0];

                T oT = Common.FillObject<T>(System.Reflection.MethodBase.GetCurrentMethod().Name, oDataRow, oDataTable.Columns);

                return new ApiResponse<T>
                {
                    Result = true,
                    Data = oT
                };
            }
            catch
            {
                throw;
            }
            finally
            {
                db?.Dispose();
            }
        }

		public static PPTerminalsResponse GetPPTerminals()
		{
            try
            {
                List<PPTerminal> oPPTerminals = null;

                using (SuperV2_Entities db = new SuperV2_Entities())
                {
                    oPPTerminals = db.PPTerminal.ToList();
                }

                return new PPTerminalsResponse
                {
                    Result = oPPTerminals != null && oPPTerminals.Count > 0,
                    PPTerminals = oPPTerminals
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Recibe como parametro el ID del terminal (Id de la tabla, primary key)
		public static PPTerminalResponse GetPPTerminal(int _id)
		{
            try
            {
                PPTerminal oPPTerminal = null;

                using (SuperV2_Entities db = new SuperV2_Entities())
                {
                    oPPTerminal = db.PPTerminal.Find(_id);
                }

                return new PPTerminalResponse
                {
                    Result = oPPTerminal != null,
                    PPTerminal = oPPTerminal
                };
            }
            catch (Exception)
            {
                throw;
            }
        }


        // Recibe como parametro el ID propio del terminal
        public static PPTerminalResponse GetPPTerminal(string _terminalID)
        {
            try
            {
                PPTerminal oPPTerminal = null;

                using (SuperV2_Entities db = new SuperV2_Entities())
                {
                    oPPTerminal = db.PPTerminal.Where(terminal=> terminal.TerminalId.Equals(_terminalID)).FirstOrDefault();
                }

                return new PPTerminalResponse
                {
                    Result = oPPTerminal != null,
                    PPTerminal = oPPTerminal
                };
            }
            catch (Exception)
            {
                throw;
            }
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

        /// <summary>
        /// Obtiene la lista de permisos por  usuario por compañia para sincronizar localmente
        /// </summary>
        /// <returns></returns>
        public static SyncResponse SyncGetPermsByUser()
        {
            //REVC NO SE SI DESPUES SE PUEDA USAR LA COMPAÑIA PARA EL ID DEL USER            
            try
            {
                using (SuperV2_Entities db = new SuperV2_Entities())
                {
                    List<SyncPermsByUser> permsList = new List<SyncPermsByUser>();


                    permsList = db.PermsByUser.Select(p => new SyncPermsByUser
                    {
                        Id = p.Id,
                        UserMappId = p.UserMappId,
                        PermId = p.PermId,
                        BoolValue = p.BoolValue,
                        TextValue = p.TextValue,
                        IntValue = p.IntValue,
                        DoubleValue = p.DoubleValue,
                        DecimalValue = p.DecimalValue

                    }).ToList();

                    return new SyncResponse
                    {
                        result = true,
                        rowsToSync = permsList.Cast<object>().ToList()
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Obtiene la lista de permisos para sincronizar localmente
        /// </summary>
        /// <returns></returns>
        public static SyncResponse SyncGetPermissions()
        {
            //REVC NO SE SI DESPUES SE PUEDA USAR LA COMPAÑIA PARA EL ID DEL USER            
            try
            {
                using (SuperV2_Entities db = new SuperV2_Entities())
                {
                    List<Permissions> pbu = db.Permissions.ToList();

                    return new SyncResponse
                    {
                        result = true,
                        rowsToSync = pbu.Cast<object>().ToList()
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
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

		public static PPTransactionsResponse GetTransactionsByACQ(int _acqNumber, int _terminalId, string _acqType, string _spGetPPBalanceByACQ)
		{
            try
            {
                List<PPTransaction> oPPTransactions = new List<PPTransaction>();

                using (SuperV2_Entities db = new SuperV2_Entities())
                {

                    object[] param = new object[] {
                        new SqlParameter("@AcqNumber", _acqNumber),
                        new SqlParameter("@TerminalId", _terminalId),
                        new SqlParameter("@AcqType", _acqType)
                    };

                    oPPTransactions = db.Database.SqlQuery<PPTransaction>($"EXEC dbo." + _spGetPPBalanceByACQ + " @AcqNumber, @TerminalId, @AcqType", param).ToList();
                }

                return new PPTransactionsResponse
                {
                    Result = true,
                    PPTransactions = oPPTransactions
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

		public static ApiResponse<List<CommittedTransaction>> GetCommitedTransactions(PPBalanceRequest _pPBalaceRequest, string _dbObject)
		{
            SuperV2_Entities db = null;
            SqlConnection sqlConn = null;
            try
            {
                db = new SuperV2_Entities();
                sqlConn = (SqlConnection)db.Database.Connection;

                string From = _pPBalaceRequest.From.ToString("yyyy/MM/dd");
                string To = _pPBalaceRequest.To.ToString("yyyy/MM/dd");

                List<CommittedTransaction> CommittedTransactions = new List<CommittedTransaction>();

				List<SqlParameter> oParameters = new List<SqlParameter>
				{
					new SqlParameter { ParameterName = "@From", Value = From },
					new SqlParameter { ParameterName = "@To", Value = To },
					new SqlParameter { ParameterName = "@TransactionType", Value = _pPBalaceRequest.DocumentType },
					new SqlParameter { ParameterName = "@TerminalId", Value = _pPBalaceRequest.TerminalId }
				};

				DataTable dt =Common.ExecuteStoredProcedure(sqlConn.ConnectionString, "dbo." + _dbObject, oParameters);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        CommittedTransactions.Add(new CommittedTransaction()
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            DocEntry = Convert.ToInt32(row["DocEntry"]),
                            InvoiceNumber = Convert.ToString(row["InvoiceNumber"]),
                            ReferenceNumber = Convert.ToString(row["ReferenceNumber"]),
                            AuthorizationNumber = Convert.ToString(row["AuthorizationNumber"]),
                            SalesAmount = Convert.ToString(row["SalesAmount"]),
                            ACQ = Convert.ToInt32(row["ACQ"]),
                            CreationDate = Convert.ToDateTime(row["CreationDate"]),
                            TerminalCode = Convert.ToString(row["TerminalCode"]),
                            TransactionType = Convert.ToString(row["TransactionType"]),
                            HostDate = Convert.ToString(row["HostDate"])
                        });
                    }
                }

                return new ApiResponse<List<CommittedTransaction>>
                {
                    Data = CommittedTransactions,
                    Result = dt.Rows.Count > 0
                };
            }
            catch
            {
                throw;
                //return (PPCommitedTransactions)Logger.LogManager.HandleExceptionWithReturn(exc, "PPCommitedTransactions");
            }
            finally
            {
                sqlConn.Close();
                sqlConn.Dispose();

                db.Dispose();
                db = null;
            }
        }

		public static ApiResponse<string> GetTransactionsPinpadTotal(int _terminalId, string _spPinpadTransactionsTotal)
		{
            try
            {
                string Total = string.Empty;
                double TotalDouble = 0;
                object[] param = new object[] {
                        new SqlParameter("@TerminalId", _terminalId)
                };

                using (SuperV2_Entities db = new SuperV2_Entities())
                {
                    TotalDouble = db.Database.SqlQuery<double>($"EXEC[dbo]." + _spPinpadTransactionsTotal + " @TerminalId", param).FirstOrDefault();
                }
                if (TotalDouble == -1)
                {
                    throw new Exception("No hay cierres de tarjeta pinpad realizados en el sistema");
                }
                else
                {
                    Total = Convert.ToString(TotalDouble);
                    return new ApiResponse<string>
                    {
                        Result = true,
                        Data = Total,
                        Error = null
                    };
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
                        Result = true,
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
                        //result = db.Users.Where(x => x.UserName == userName && x.PasswordHash == encryptClienPass && x.EmailConfirmed && x.Active).ToList();
                        result = db.Users.Where(x => x.UserName == userName && x.PasswordHash == encryptClienPass && x.Active).ToList();
                    }
                    else
                    {
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
                    Result = true,
                    users = Userlist,
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

		public static ApiResponse<List<PPTransaction>> GetTransactionsByDocEntry(int _docEntry)
		{
            SuperV2_Entities oSuperV2_Entities = new SuperV2_Entities();

            try
            {


                List<PPTransaction> oPPTransactions = oSuperV2_Entities.PPTransaction.Where(x => x.DocEntry == _docEntry).ToList();

                return new ApiResponse<List<PPTransaction>>
                {
                    Data = oPPTransactions,
                    Result = true
                };
            }
            catch
            {
                throw;
            }
            finally
            {
                oSuperV2_Entities?.Dispose();
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
            catch 
            {
                throw;
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
                    return db.UserAssign.Where(x => x.UserId == userId).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ParseSourceException(ex);
            }
        }

        public static UserAssign GetUserMapCustom(int MappId)
        {
            try
            {
                Companys company = new Companys();

                using (SuperV2_Entities db = new SuperV2_Entities())
                {
                    //                    int userid = Convert.ToInt32(userId);
                    UserAssign oUserAssign = db.UserAssign.Where(x => x.Id == MappId).Select(x => x).FirstOrDefault();
                    string password = oUserAssign.SAPPass;
                    string user = oUserAssign.SAPUser;
                    if (oUserAssign != null)
                    {
                        //                        company = db.Companys.Find(userAssign.CompanyId);
                        company = db.Companys.Where(x => x.Id == oUserAssign.CompanyId).Include(b => b.SAPConnection).FirstOrDefault();
                        if (company != null)
                        {
                            company.SAPUser = oUserAssign.SAPUser;
                            company.SAPPass = oUserAssign.SAPPass;
                            oUserAssign.Companys = company;
                            return oUserAssign;
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

		public static PPTransactionsResponse GetPPTransactionCenceledStatus(PPTransactionsCanceledPrintSearch _doc, string _spGetPPTransactiosCanceled)
		{
            try
            {
                List<PPTransaction> oPPTransactions = null;

                using (SuperV2_Entities db = new SuperV2_Entities())
                {
                    object[] param = new object[] {
                        new SqlParameter("@UserPrefix", _doc.UserPrefix),
                        new SqlParameter("@FechaIni", _doc.FechaIni),
                        new SqlParameter("@FechaFin", _doc.FechaFin)

                    };
                    oPPTransactions = new List<PPTransaction>(db.Database.SqlQuery<PPTransaction>($"EXEC dbo." + _spGetPPTransactiosCanceled + " @UserPrefix, @FechaIni, @FechaFin", param).ToList<PPTransaction>());
                    //oPPTransactions = db.PPTransaction.Where(x => x.DocEntry.ToString().Equals(_invoiceNumber)
                    //).ToList();
                }

                return new PPTransactionsResponse
                {
                    Result = oPPTransactions != null && oPPTransactions.Count > 0,
                    PPTransactions = oPPTransactions
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Obtiene una serie de registros en funcion a llave
        /// foreanea
        /// </summary>
        /// <typeparam name="T">Representa el tipo de objeto que se va a manipular</typeparam>
        /// <typeparam name="I">Indica el tipo de indice porque se va a filtrar en el sp</typeparam>
        /// <param name="_dbObjectName">Indica el nombre del objeto de la base de datos a consumir</param>
        /// <param name="_object">Representa el tipo de objeto que se va a trabajar ejemplo, usuarios, companias</param>
        /// <param name="_index">Es el valor que se va a usar en el objeto de la bd para filtrar</param>
        /// <returns>Devuelve una lista de objetos definidos por T</returns>
        public static ApiResponse<List<T>> GetRecordsByKey<T, I>(string _dbObjectName, I _index) where T : new()
        {
            SuperV2_Entities oGGSERVONEContext = new SuperV2_Entities();
            List<T> tList = null;

            try
            {
                
                string PARSED_FILTER = "SYSTEM.STRING".Contains(typeof(I).Name.ToUpper()) ? String.Concat("'", _index, "'") : String.Concat("", _index);
                string QUERY = $"EXEC dbo.{_dbObjectName} {PARSED_FILTER}";

                DataTable oDataTable = COMMON.Common.GetDataTable(oGGSERVONEContext.Database.Connection, QUERY);

                tList = new List<T>();

                int RECORDS = oDataTable.Rows.Count;

                for (int c = 0; c < RECORDS; c++)
                {
                    DataRow oDataRow = oDataTable.Rows[c];

                    T oT = COMMON.Common.FillObject<T>(System.Reflection.MethodBase.GetCurrentMethod().Name, oDataRow, oDataTable.Columns);

                    tList.Add(oT);
                }

                return new ApiResponse<List<T>>
                {
                    Result = true,
                    Data = tList.Select(x => x).ToList()
                };
            }
            catch
            {
                throw;
            }
            finally
            {
                tList?.Clear();
                oGGSERVONEContext?.Dispose();
            }
        }

        public static ApiResponse<List<CompanyMargins>> GetCompanyMargins()
        {
            try
            {
                var result = new List<CompanyMargins>();

                using(SuperV2_Entities db = new SuperV2_Entities())
                {
                    result = db.Database.SqlQuery<CompanyMargins>($"SELECT * FROM [dbo].[{GetNameObject("viewGETACCEPTEDMARGINS")}]").ToList();
                }

                return new ApiResponse<List<CompanyMargins>>()
                {
                    Result = true,
                    Data = result,
                    Error = null
                };
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Metodo para obtener las compannias registradas en la aplicacion
        /// </summary>
        /// <returns></returns>
        public static SyncResponse SyncGetCompanies()
        {
            try
            {
                List<CompanysModel> companiesList = new List<CompanysModel>();

                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    companiesList = dbDao.Companys.Select(x => new CompanysModel
                    {
                        Id = x.Id,
                        DBName = x.DBName,
                        DBCode = x.DBCode,
                        Active = x.Active,
                        SAPConnectionId = x.SAPConnectionId,
                        LogoPath = x.LogoPath,
                        MailDataId = x.MailDataId,
                        ExchangeRate = x.ExchangeRate,
                        ExchangeRateValue = x.ExchangeRateValue,
                        HandleItem = x.HandleItem,
                        BillItem = x.BillItem,
                        SP_ItemInfo = x.SP_ItemInfo,
                        SP_InvoiceInfoPrint = x.SP_InvoiceInfoPrint,
                        V_BPS = x.V_BPS,
                        V_Items = x.V_Items,
                        V_ExRate = x.V_ExRate,
                        V_Taxes = x.V_Taxes,
                        SP_WHAvailableItem = x.SP_WHAvailableItem,
                        SP_SeriesByItem = x.SP_SeriesByItem,
                        SP_PayDocuments = x.SP_PayDocuments,
                        V_GetAccounts = x.V_GetAccounts,
                        V_GetCards = x.V_GetCards,
                        V_GetBanks = x.V_GetBanks,
                        V_GetSalesMan = x.V_GetSalesMan,
                        SP_CancelPayment = x.SP_CancelPayment,
                        SAPUser = x.SAPUser,
                        SAPPass = x.SAPPass,
                        ReportPath = x.ReportPath,
                        ReportPathInventory = x.ReportPathInventory,
                        ReportPathCopy = x.ReportPathCopy,
                        ReportPathQuotation = x.ReportPathQuotation,
                        ReportPathSO = x.ReportPathSO,
                        ReportBalance = x.ReportBalance,
                        FEIdNumber = x.FEIdNumber,
                        FEIdType = x.FEIdType,               
                        ScaleMaxWeightToTreatAsZero = x.ScaleMaxWeightToTreatAsZero,
                        ScaleWeightToSubstract = x.ScaleWeightToSubstract,
                        IsLinePriceEditable = x.IsLinePriceEditable,
                        HasOfflineMode = x.HasOfflineMode,
                        DecimalAmountPrice = x.DecimalAmountPrice,
                        DecimalAmountTotalLine = x.DecimalAmountTotalLine,
                        DecimalAmountTotalDocument = x.DecimalAmountTotalDocument,
                        PrinterConfiguration = x.PrinterConfiguration,
                        HasZeroBilling = x.HasZeroBilling,
                        LineMode = x.LineMode
                    }).ToList();
                }

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = companiesList.Cast<object>().ToList(),
                };
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static SyncResponse SyncGetMailData()
        {
            try
            {
                List<MailDataModel> mailDataList = new List<MailDataModel>();

                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    mailDataList = dbDao.MailData.Select(x => new MailDataModel
                    {
                        Id = x.Id,
                        from = x.from,
                        Host = x.Host,
                        pass = x.pass,
                        port = x.port,
                        SSL = x.SSL,
                        subject = x.subject,
                        user = x.user

                    }).ToList();
                }

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = mailDataList.Cast<object>().ToList(),
                };
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static SyncResponse SyncGetCompaniesByUser()
        {
            try
            {
                List<SyncCompanyByUsers> companiesByUserList = new List<SyncCompanyByUsers>();

                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    companiesByUserList = dbDao.CompanyByUsers.Select(x => new SyncCompanyByUsers
                    {
                        Id = x.Id,
                        UserId = x.UserId,
                        CompanyId = x.CompanyId,
                        Status = x.Status,
                        CreationDate = x.CreationDate

                    }).ToList();
                }

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = companiesByUserList.Cast<object>().ToList(),
                };
            }
            catch (Exception)
            {
                throw;
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
                        Active = x.Active,
                        IsLinePriceEditable = x.IsLinePriceEditable,
                        ScaleWeightToSubstract = x.ScaleWeightToSubstract,
                        ScaleMaxWeightToTreatAsZero = x.ScaleMaxWeightToTreatAsZero,
                        LineMode = x.LineMode
                    }).ToList();
                }

                return new CompanyListResponse
                {
                    Result = true,
                    companiesList = companyList,
                };
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static SalesManResponse GetSalesMan(string dbObject_ViewGetSalesManApp)
        {
            try
            {
                List<SalesManModel> salesMan = new List<SalesManModel>();

                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {

                    salesMan = dbDao.Database.SqlQuery<SalesManModel>("SELECT * FROM [dbo]."+ dbObject_ViewGetSalesManApp).ToList();

                }

                return new SalesManResponse
                {
                    Result = true,
                    salesManList = salesMan
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
                        Result = true,
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
        /// retorna la lista de los usuarios para ser usada en sincronizacion local;
        /// </summary>
        /// <returns></returns>
        public static SyncResponse SyncGetUsers()
        {
            try
            {
                List<Users> users = new List<Users>();

                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    users = dbDao.Users.ToList();
                }

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = users.Cast<object>().ToList(),
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// retorna la lista de los usuarios para ser usada en sincronizacion local;
        /// </summary>
        /// <returns></returns>
        public static SyncResponse SyncGetUserAssigns()
        {
            try
            {
                List<SyncUserAssign> usersAssings = new List<SyncUserAssign>();

                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    usersAssings = dbDao.UserAssign.Select(x => new SyncUserAssign
                    {
                        Id = x.Id,
                        UserId = x.UserId,
                        SuperUser = x.SuperUser,
                        SAPUser = x.SAPUser,
                        SAPPass = x.SAPPass,
                        SlpCode = x.SlpCode,
                        StoreId = x.StoreId,
                        minDiscount = x.minDiscount,
                        CenterCost = x.CenterCost,
                        Active = x.Active,
                        PriceListDef = x.PriceListDef,
                        CompanyId = x.CompanyId
                    }).ToList();
                }

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = usersAssings.Cast<object>().ToList(),
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static SyncResponse SyncGetParamsViewCompanies()
        {
            List<ParamsModel> paramsList = new List<ParamsModel>();
            try
            {
                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    paramsList = dbDao.ParamsViewCompany.Select(x => new ParamsModel
                    {
                        Id = x.Id,
                        CompanyId = x.CompanyId,
                        ViewsId = x.ViewsId,
                        ParamsId = x.ParamsId,
                        Descrip = x.Descrip,
                        Order = x.Order,
                        Display = x.Display,
                        Visibility = x.Visibility,
                        Text = x.Text,
                    }).ToList();
                }

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = paramsList.Cast<object>().ToList(),
                };

            }
            catch (Exception)
            {
                throw;
            }
        }


        public static SyncResponse SyncGetViewParams()
        {
            List<ViewParams> paramsList = new List<ViewParams>();
            try
            {
                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    paramsList = dbDao.ViewParams.ToList();
                }

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = paramsList.Cast<object>().ToList(),
                };

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
                        //SP_ItemInfo = company.SP_ItemInfo,
                        //SP_CancelPayment = company.SP_CancelPayment,
                        //SP_InvoiceInfoPrint = company.SP_InvoiceInfoPrint,
                        //SP_PayDocuments = company.SP_PayDocuments,
                        //SP_SeriesByItem = company.SP_SeriesByItem,
                        //SP_WHAvailableItem = company.SP_WHAvailableItem,
                        //V_BPS = company.V_BPS,
                        //V_Items = company.V_Items,
                        //V_ExRate = company.V_ExRate,
                        //V_Taxes = company.V_Taxes,
                        //V_GetAccounts = company.V_GetAccounts,
                        //V_GetCards = company.V_GetCards,
                        //V_GetBanks = company.V_GetBanks,
                        //V_GetSalesMan = company.V_GetSalesMan,
                        ScaleMaxWeightToTreatAsZero = company.ScaleMaxWeightToTreatAsZero,
                        ScaleWeightToSubstract = company.ScaleWeightToSubstract,
                        IsLinePriceEditable = company.IsLinePriceEditable,
                        HasOfflineMode = company.HasOfflineMode,
                        DecimalAmountPrice = company.DecimalAmountPrice,
                        DecimalAmountTotalLine = company.DecimalAmountTotalLine,
                        DecimalAmountTotalDocument = company.DecimalAmountTotalDocument,
                        PrinterConfiguration = company.PrinterConfiguration,
                        HasZeroBilling = company.HasZeroBilling,
                        //LineMode = company.LineMode,
                        AcceptedMargins = company.AcceptedMargins

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
                        Result = true,
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
                        Result = true,
                        SAPConnections = SAPConnectionList,
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static SyncResponse SyncGetSapConnections()
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

                    return new SyncResponse
                    {
                        result = true,
                        rowsToSync = SAPConnectionList.Cast<object>().ToList()
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
        /// <param name="dbObject_SpGetUserAssingListApp"></param>
        /// <param name="dbObject_SpGetSeriesByUsersAppp"></param>
        /// <returns></returns>
        public static UserListModel GetUserUserAssignList(Companys company, string _dbObjectSpGetUserAssingListApp, string _dbObjectSpGetSeriesByUsersAppp)
        {
           

            try
            {
                List<UserAsingModel> user = new List<UserAsingModel>();
               

                using (SuperV2_Entities oSuperV2_Entities = new SuperV2_Entities())
                {
                    object[] param = new object[]
                       {
                        new SqlParameter("@CompanyId", company.Id),
                       };
                    user = oSuperV2_Entities.Database.SqlQuery<UserAsingModel>($"EXEC dbo."+ _dbObjectSpGetUserAssingListApp + " @CompanyId", param).ToList();

                    foreach (UserAsingModel x in user)
                    {
                        object[] param2 = new object[]
                        {
                        new SqlParameter("@UsrMappId", x.Id),
                        };
                        x.Series = oSuperV2_Entities.Database.SqlQuery<SeriesByUserModel>($"EXEC dbo."+ _dbObjectSpGetSeriesByUsersAppp + " @UsrMappId", param2).ToList();
                    }
                }

                return new UserListModel
                {
                    Result = true,
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
                    Result = true,
                    Stores = Stores
                };
            }
            catch (Exception ex)
            {
                throw ParseSourceException(ex);
            }

        }


        public static SyncResponse SyncGetStores()
        {
            List<SyncStore> stores = new List<SyncStore>();

            try
            {
                using (SuperV2_Entities db = new SuperV2_Entities())
                {
                    stores = db.Store.Select(x => new SyncStore
                    {
                        Id = x.Id,
                        Name = x.Name,
                        WhCode = x.WhCode,
                        WhName = x.WhName,
                        CompanyId = x.CompanyId,
                        Active = x.Active
                    }).ToList();
                }

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = stores.Cast<object>().ToList()
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
                    Result = true,
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
                        CompanyName = company.DBName,
                        Type = x.Type

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
                        Result = true,
                        Series = Series
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static SyncResponse SyncGetSeries()
        {
            try
            {
                using (SuperV2_Entities db = new SuperV2_Entities())
                {
                    List<SyncSeriesModel> series = db.Series.Select(x => new SyncSeriesModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Serie = x.Serie,
                        Numbering = x.Numbering,
                        CompanyId = x.CompanyId,
                        DocType = x.DocType,
                        Active = x.Active,
                    }).ToList();

                    return new SyncResponse
                    {
                        result = true,
                        rowsToSync = series.Cast<object>().ToList()
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static SyncResponse SyncGetSeriesByUsers()
        {
            try
            {
                using (SuperV2_Entities db = new SuperV2_Entities())
                {
                    List<SyncSeriesByUser> seriesByUser = db.SeriesByUser.Select(x => new SyncSeriesByUser
                    {
                        Id = x.Id,
                        SerieId = x.SerieId,
                        UsrMappId = x.UsrMappId
                    }).ToList();

                    return new SyncResponse
                    {
                        result = true,
                        rowsToSync = seriesByUser.Cast<object>().ToList()
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
                    Result = true,
                    Enums = enums
                };
            }
            catch (Exception)
            {
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
                    Result = true,
                    Enums = enums
                };
            }
            catch (Exception)
            {
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

        private static string GetFileTypeContent(string fileExtension)
        {
            try
            {
                return fileExtension.Replace(".", "image/");
            }
            catch (Exception)
            {
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
                    Result = true,
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
        public static ApiResponse<List<CurrencyModel>> GetCurrencyType()
        {
            try
            {
                List<CurrencyModel> DocCurreny = new List<CurrencyModel>();
                using (SuperV2_Entities db = new SuperV2_Entities())
                {
                     DocCurreny = db.Database.SqlQuery<CurrencyModel>("SELECT * from "+ GetNameObject("viewGetCurrencyTypeApp")).ToList<CurrencyModel>();
                    return new ApiResponse<List<CurrencyModel>>
                    {
                        Result = true,
                        Data = DocCurreny,
                        Error = null
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Metodo para obtener la compania por defecto
        /// </summary>
        /// <returns></returns>
        public static CompanySapResponse GetDefaultCompany()
        {
            try
            {
                CompanysSapConnModel defaultCompany = new CompanysSapConnModel();

                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    defaultCompany = dbDao.Companys.Select(x => new CompanysSapConnModel
                    {
                        Id = x.Id,
                        Server = x.SAPConnection.Server,
                        DBName = x.DBName,
                        DBCode = x.DBCode,
                        Active = x.Active,
                        IsLinePriceEditable = x.IsLinePriceEditable,
                        ScaleWeightToSubstract = x.ScaleWeightToSubstract,
                        ScaleMaxWeightToTreatAsZero = x.ScaleMaxWeightToTreatAsZero
                    }).FirstOrDefault();
                }

                return new CompanySapResponse
                {
                    Result = true,
                    Company = defaultCompany,
                };
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
                    Stores = db.Store.Where(x => x.Id == store).Select(x => new StoresModel
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
                    Result = true,
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
                    return Convert.ToDecimal(db.UserAssign.Where(x => x.UserId == userId).Select(x => x.minDiscount).FirstOrDefault());
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        public static ViewLineAgrupationResponse GetViewGroupList()
        {
            try
            {
                List<GroupLine> ViewGroupLineList = new List<GroupLine>();

                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    ViewGroupLineList = dbDao.ViewLineAgrupation.Select(x => new GroupLine
                    {
                        Id = x.Id,
                        CodNum = x.CodNum,
                        NameView = x.NameView,
                        isGroup = x.isGroup,
                        LineMode = x.LineMode,

                    }).ToList();
                }

                return new ViewLineAgrupationResponse
                {
                    Result = true,
                    ViewGroupList = ViewGroupLineList,
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Cambios Jorge EMA Aromas
        public static LoggedUser SelectLoggedUser(string email, string password)
        {
            try {
                LoggedUser user;
                string encryptedPassword = Common.Encrypt(password);
                object[] param = new object[]
                {
                new SqlParameter("@Email", email),
                new SqlParameter("@Password", encryptedPassword)
                };

                using (SuperV2_Entities db = new SuperV2_Entities())
                {
                    user = db.Database.SqlQuery<LoggedUser>($"EXEC dbo.{GetNameObject("spGetLoggedUser")} @Email, @Password", param).FirstOrDefault();
                }
                if (user == null)
                {
                    throw new Exception("Usuario sin configuraciones asignadas");
                }
                encryptedPassword = null;
                param = null;
                return user;
            
            } catch (Exception ex)
            {
                
                throw;
            }
}

		public static bool CheckIfTransactionExists(string _transactionId)
		{
            PPTransaction oPPTransaction = null;
            try
            {
                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    using (DbContextTransaction transaction = dbDao.Database.BeginTransaction())
                    {
                        oPPTransaction = dbDao.PPTransaction.Where(x => x.TransactionId.Equals(_transactionId)).FirstOrDefault();
                    }
                }

                return oPPTransaction != null;
            }
            catch
            {
                throw;
            }
        }
		#endregion

		public static UserAppResponse GetUserApp(string _id)
        {
            UserModel oUserModel = null;
            string message = String.Empty;
            try
            {
                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    using (DbContextTransaction transaction = dbDao.Database.BeginTransaction())
                    {
                        Users oUsers = dbDao.Users.Where(x => x.Id.Equals(_id)).FirstOrDefault();
                        if (oUsers != null)
                        {
                            oUserModel = new UserModel()
                            {
                                Active = oUsers.Active,
                                Email = oUsers.Email,
                                EmailConfirmed = oUsers.EmailConfirmed,
                                FullName = oUsers.FullName,
                                UserName = oUsers.UserName,
                                Id = oUsers.Id
                            };
                        }
                        else
                        {
                            message = "No existe el usuario solicitado";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                throw;
            }

            return new UserAppResponse()
            {
                Result = oUserModel != null,
                Error = new ErrorInfo()
                {
                    Message = message
                },
                User = oUserModel
            };
        }
        public static UsersAppResponse GetUsersApp()
        {
            List<UserModel> oUsersModel = new List<UserModel>();
            string message = String.Empty;
            try
            {
                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    using (DbContextTransaction transaction = dbDao.Database.BeginTransaction())
                    {
                        var oUsers = dbDao.Users.ToList();
                        if (oUsers.Count > 0)
                        {
                            oUsers.ForEach(oUser =>
                            {
                                oUsersModel.Add(new UserModel()
                                {
                                    Active = oUser.Active,
                                    Email = oUser.Email,
                                    EmailConfirmed = oUser.EmailConfirmed,
                                    FullName = oUser.FullName,
                                    UserName = oUser.UserName,
                                    Id = oUser.Id
                                });
                            });
                        }
                        else
                        {
                            message = "No hay usuarios registrados";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                throw;
            }

            return new UsersAppResponse()
            {
                Result = oUsersModel.Count > 0,
                Error = new ErrorInfo()
                {
                    Message = message
                },
                Users = oUsersModel
            };
        }

        #region Reports
        public static ReportsResponse GetReports(string _dbObjectViewGetReport)
        {
            List<KeyValue> reports = new List<KeyValue>();
            using (SuperV2_Entities db = new SuperV2_Entities())
            {
                reports = db.Database.SqlQuery<KeyValue>($"SELECT * FROM dbo.[{_dbObjectViewGetReport}]").ToList();
            }

            if (reports.Count > 0) return new ReportsResponse { Result = true, Reports = reports };
            else throw new Exception("No se obtuvieron reportes");
        }

        public static string GetReportPath(int reportKey, string _dbObjectViewGetReportPath)
        {
            string path = string.Empty;
            object[] param = new object[]
            {
                new SqlParameter("@ReportKey", reportKey)
            };

            using (SuperV2_Entities db = new SuperV2_Entities())
            {
                path = db.Database.SqlQuery<string>($"EXEC dbo.[{_dbObjectViewGetReportPath}] @ReportKey", param).FirstOrDefault();
            }

            if (!string.IsNullOrEmpty(path)) return path;
            else throw new Exception($"No se obtuvo la ruta del reporte {reportKey}");
        }
        #endregion

        #region Udfs
        public static UdfsResponse GetConfiguredUdfs(string _category)
        {
            try
            {
                SuperV2_Entities oSuperV2_Entities = new SuperV2_Entities();

                CompanyUdfs oCompanyUdfs = oSuperV2_Entities.CompanyUdfs.Where(x => x.TableId.Equals(_category)).FirstOrDefault();

                List<Udf> udfs = new List<Udf>();

                if (oCompanyUdfs != null)
                {
                    udfs = JsonConvert.DeserializeObject<List<Udf>>(oCompanyUdfs.Udfs);
                }

                oSuperV2_Entities.Dispose();
                oSuperV2_Entities = null;

                return new UdfsResponse
                {
                    Result = true,
                    Udfs = udfs
                };
            }
            catch
            {
                throw;
            }
        }
        #endregion
        public static SyncResponse SyncGetConfiguredUdfs()
        {
            try
            {
                SuperV2_Entities oSuperV2_Entities = new SuperV2_Entities();
                List<CompanyUdfs> udfs = new List<CompanyUdfs>(oSuperV2_Entities.CompanyUdfs.ToList<CompanyUdfs>());            

                oSuperV2_Entities.Dispose();
                oSuperV2_Entities = null;

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = udfs.Cast<object>().ToList()
                };
            }
            catch
            {
                throw;
            }        
        }

        #region Settings
        public static ApiResponse<List<Settings>> GetViewSettings()
        {
            try
            {
                using (SuperV2_Entities oSuperV2_Entities = new SuperV2_Entities())
                {
                    List<Settings> settings = oSuperV2_Entities.Settings.ToList();              


                  
                    if (settings.Count > 0)
                    {

                        return new ApiResponse<List<Settings>>
                        {
                            Result = true,
                            Data = settings,
                            Error = null
                        };
                    }
                    else
                    {
                        throw new Exception("No se obtuvieron configuraciones de la compañía");
                    }

                }

            }
            catch
            {
                throw;
            }
        }
        public static ApiResponse<Settings> GetViewSettingbyId(int Code)
        {
            try
            {
                using (SuperV2_Entities oSuperV2_Entities = new SuperV2_Entities())
                {
                    Settings settings = oSuperV2_Entities.Settings.Where(x => x.Codigo == Code).FirstOrDefault();

                    if (settings != null) {

                        return new ApiResponse<Settings>
                        {
                            Result = true,
                            Data = settings,
                            Error = null
                        };
                    }
                    else
                    {
                        throw new Exception("No se obtuvieron configuraciones determinadas en compañía para esta vista");
                    }                   
                }
            }
            catch
            {
                throw;
            }
        }

        #endregion
        #region DBObjectNames
        /// <summary>
        /// Obtine nombre del objeto(sp, view, function) guardado en la db aplicacion 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetNameObject(string _dbObject)
        {
            try
            {
                string DbObjectName;
                using (SuperV2_Entities oSuperV2_Entities = new SuperV2_Entities())
                {                
                    object[] param = new object[]
                    {
                        new SqlParameter("@Name", _dbObject),
                    };
                    DbObjectName = oSuperV2_Entities.Database.SqlQuery<string>($"EXEC dbo.{Common.GetDBObjectByKey(System.Reflection.MethodBase.GetCurrentMethod(), "spGetNameDbObjectApp")} @Name", param).FirstOrDefault();

                    if (String.IsNullOrEmpty(DbObjectName))
                    {                      
                        throw new Exception($"No ha definido nombre objeto para el key {_dbObject} en la app, agreguelo por favor");
                    }
                    return DbObjectName;
                }
            }
            catch
            {
                throw;
            }
        }

        public static ApiResponse<List<DBObjectName>> GetDbObjectNames()
        {
            try
            {
                List<DBObjectName> DbObjectNames = new List<DBObjectName>();

                using (SuperV2_Entities oSuperV2_Entities = new SuperV2_Entities())
                {

                    DbObjectNames = oSuperV2_Entities.Database.SqlQuery<DBObjectName>($"EXEC dbo.{GetNameObject("spGetDBObjectNamesList")}").ToList();
                    
                    return new ApiResponse<List<DBObjectName>>
                    {
                        Result = true,
                        Data = DbObjectNames,
                        Error = null
                    };
                }
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region Paydesk balance
        public static PaydeskBalance GetPaydeskBalance(string userId, string createDate)
        {
            PaydeskBalance paydesk;
            object[] param = new object[]
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@CreationDate", createDate),
            };

            using (SuperV2_Entities db = new SuperV2_Entities())
            {
                paydesk = db.Database.SqlQuery<PaydeskBalance>($"EXEC[dbo].{GetNameObject("spPayDeskBalance")} @UserId, @CreationDate", param).FirstOrDefault();
            }

            if (paydesk != null) return paydesk;
            else throw new Exception("No se obtuvo cierre de caja para el usuario");
        }
        #endregion
    }

}

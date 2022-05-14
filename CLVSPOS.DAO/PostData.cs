using CLVSPOS.COMMON;
using CLVSPOS.LOGGER;
using CLVSPOS.MODELS;
using CLVSSUPER.MODELS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CLVSPOS.DAO
{
    public class PostData
    {

        /// <summary>
        /// funcion que se encrga de editar los permisos por usuario
        /// se trae la lista de los permisos que van a se modificados y el usuario al que se le va
        /// a asignar, tambien se especifica la compania.
        /// </summary>
        /// <param name="permsUserEdit"></param>
        /// <returns></returns>
        public static BaseResponse EditPermsByUser(PermsUserEdit permsUserEdit)
        {
            try
            {
                using (SuperV2_Entities db = new SuperV2_Entities())
                {
                    int userMapId = DAO.GetData.GetUserMappIdByUserID(permsUserEdit.UserId);
                    List<Permissions> perms = db.PermsByUser.Where(x => x.UserMappId == userMapId).Select(x => x.V_Permissions).ToList();
                    foreach (Permissions perm in perms)
                    {
                        PermsByUser prmByUser = db.PermsByUser.Where(x => x.UserMappId == userMapId && x.V_Permissions.Id == perm.Id).SingleOrDefault();
                        db.PermsByUser.Remove(prmByUser);
                    }
                    UserAssign userAssign = db.UserAssign.Find(userMapId);
                    List<PermsByUser> permByUser = new List<PermsByUser>();
                    if (permsUserEdit.UserPerms != null)
                    {
                        foreach (Permissions perm in permsUserEdit.UserPerms)
                        {
                            permByUser.Add(new PermsByUser
                            {
                                PermId = perm.Id,
                                BoolValue = perm.Active
                            });
                        }
                    }
                    userAssign.V_PermsByUser = permByUser;
                    db.Entry(userAssign).State = EntityState.Modified;
                    db.SaveChanges();
                    return new BaseResponse
                    {
                        Result = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponse
                {
                    Result = false,
                    Error = new ErrorInfo
                    {
                        Message = ex.Message,
                        Code = ex.HResult
                    }
                };
            }

        }

		public static BaseResponse UpdatePPTerminalsByUser(PPTerminalsByUser _pPTerminalsByUser)
		{
            try
            {
                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    using (DbContextTransaction transaction = dbDao.Database.BeginTransaction())
                    {
                        string userId = _pPTerminalsByUser.UserId;

                        //delete all records

                        List<PPTerminalByUser> recordsToDelete = dbDao.PPTerminalByUser.Where(x => x.UserId.Equals(userId)).ToList();

                        recordsToDelete.ForEach(x =>
                        {
                            dbDao.Entry(x).State = EntityState.Deleted;
                            dbDao.SaveChanges();
                        });

                        _pPTerminalsByUser.TerminalsByUser.ForEach(x =>
                        {
                            dbDao.PPTerminalByUser.Add(x);
                            dbDao.SaveChanges();
                        });

                        transaction.Commit();
                    }

                    return new BaseResponse
                    {
                        Result = true
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

		public static PPBalanceResponse CreatePPBalance(PPBalance _pPBalance, PPBalanceRequest pPBalanceRequest)
		{
            try
            {
                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    using (DbContextTransaction transaction = dbDao.Database.BeginTransaction())
                    {
                        _pPBalance.CreationDate = DateTime.Now;
                        _pPBalance.ModificationDate = DateTime.Now;
                        dbDao.PPBalance.Add(_pPBalance);
                        dbDao.SaveChanges();
                        transaction.Commit();
                    }

                    return new PPBalanceResponse
                    {
                        Result = true,
                        PPBalance = _pPBalance
                    };
                }
            }
            catch (Exception e)
            {
                String message = e.Message;
                throw;
            }
        }

		
		public static BaseResponse UpdatePPTransactionMassive(DateTime _from, DateTime _to, int _terminalId,string _dbObject)
		{
            try
            {
                SuperV2_Entities db = new SuperV2_Entities();
                SqlConnection sqlConn = (SqlConnection)db.Database.Connection;

                string sql = string.Format("dbo." + _dbObject);

                List<SqlParameter> pars = new List<SqlParameter>();

                string From = _from.ToString("yyyy/MM/dd");
                string To = _to.ToString("yyyy/MM/dd");

                pars.Add(new SqlParameter { ParameterName = "@From", Value = From });
                pars.Add(new SqlParameter { ParameterName = "@To", Value = To });
                pars.Add(new SqlParameter { ParameterName = "@TerminalId", Value = _terminalId });

                List<InvPrintModel> invList = new List<InvPrintModel>();
                DataTable dt = Common.ExecuteStoredProcedure(sqlConn.ConnectionString, sql, pars);

                return new BaseResponse
                {
                    Result = true
                };
            }
            catch (Exception ex)
            {
                LogManager.LogMessage("Ocurrio un error al insertar Log: " + ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message, (int)Constants.LogTypes.General);
                throw ex;
            }
        }

		/// <summary>
		/// funcion para la creacion de la compannia y el correo de la misma
		/// recibe como parametro la compania y el correo
		/// </summary>
		/// <param name="companyAndMail"></param>
		/// <param name="filesInfo"></param>
		/// <returns></returns>
		public static BaseResponse CreateCompany(CompanyAndMail companyAndMail, HttpContext filesInfo)
        {
            string path = System.Configuration.ConfigurationManager.AppSettings["FilesPath"].ToString();
            try
            {
                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    using (DbContextTransaction transaction = dbDao.Database.BeginTransaction())
                    {
                        var httpPostedFile = filesInfo.Request;
                        var file = httpPostedFile.Files["file"];
                        var fileInv = httpPostedFile.Files["fileInv"];
                        var fileQuo = httpPostedFile.Files["fileQuo"];
                        var fileInven = httpPostedFile.Files["fileInven"];
                        var fileCopy = httpPostedFile.Files["fileOinvCopy"];
                        var fileSO = httpPostedFile.Files["fileSO"];
                        var fileOinvPP = httpPostedFile.Files["fileOinvPP"];
                        string company = companyAndMail.company.DBCode;

                        MailData mail = new MailData
                        {
                            subject = companyAndMail.mail.subject,
                            from = companyAndMail.mail.from,
                            user = companyAndMail.mail.user,
                            pass = companyAndMail.mail.pass,
                            port = companyAndMail.mail.port,
                            Host = companyAndMail.mail.Host,
                            SSL = companyAndMail.mail.SSL,
                        };

                        Companys comp = new Companys
                        {
                            DBCode = companyAndMail.company.DBCode,
                            DBName = companyAndMail.company.DBName,
                            SAPConnectionId = companyAndMail.company.SAPConnectionId,
                            Active = companyAndMail.company.Active,
                            ExchangeRate = companyAndMail.company.ExchangeRate,
                            ExchangeRateValue = companyAndMail.company.ExchangeRateValue,
                            HandleItem = companyAndMail.company.HandleItem,
                            BillItem = companyAndMail.company.BillItem,
                            V_MailData = mail,
                            HasOfflineMode = companyAndMail.company.HasOfflineMode,
                            DecimalAmountPrice = companyAndMail.company.DecimalAmountPrice,
                            DecimalAmountTotalDocument = companyAndMail.company.DecimalAmountPrice,
                            DecimalAmountTotalLine = companyAndMail.company.DecimalAmountTotalLine,
                            PrinterConfiguration = companyAndMail.company.PrinterConfiguration,
                            HasZeroBilling = companyAndMail.company.HasZeroBilling,

                            //SP_ItemInfo = companyAndMail.company.SP_ItemInfo,
                            //SP_InvoiceInfoPrint = companyAndMail.company.SP_InvoiceInfoPrint,
                            //SP_WHAvailableItem = companyAndMail.company.SP_WHAvailableItem,
                            //SP_SeriesByItem = companyAndMail.company.SP_SeriesByItem,
                            //SP_PayDocuments = companyAndMail.company.SP_PayDocuments,
                            //SP_CancelPayment = companyAndMail.company.SP_CancelPayment,

                            //V_BPS = companyAndMail.company.V_BPS,
                            //V_Items = companyAndMail.company.V_Items,
                            //V_ExRate = companyAndMail.company.V_ExRate,
                            //V_Taxes = companyAndMail.company.V_Taxes,
                            //V_GetAccounts = companyAndMail.company.V_GetAccounts,
                            //V_GetCards = companyAndMail.company.V_GetCards,
                            //V_GetBanks = companyAndMail.company.V_GetBanks,
                            //V_GetSalesMan = companyAndMail.company.V_GetSalesMan,

                            LogoPath = file != null ? path + company + '\u0022' + file.FileName : "",
                            ReportPath = fileInv != null ? path + company + '\u0022' + fileInv.FileName : "",
                            ReportPathCopy = fileCopy != null ? path + company + '\u0022' + fileCopy.FileName : "",
                            ReportPathInventory = fileInven != null ? path + company + '\u0022' + fileInven.FileName : "",
                            ReportPathQuotation = fileQuo != null ? path + company + '\u0022' + fileQuo.FileName : "",
                            ReportPathSO = fileSO != null ? path + company + '\u0022' + fileSO.FileName : "",
                            ReportPathPP = fileOinvPP != null ? path + company + '\u0022' + fileOinvPP.FileName : ""
                        };

                        if (!savePrintFiles(file, company) || !savePrintFiles(fileInv, company) || !savePrintFiles(fileQuo, company) ||
                               !savePrintFiles(fileInven, company) || !savePrintFiles(fileCopy, company) || !savePrintFiles(fileSO, company) || !savePrintFiles(fileOinvPP, company))
                        {
                            transaction.Rollback();
                            return new BaseResponse
                            {
                                Result = false,
                                Error = new ErrorInfo
                                {
                                    Message = "El error al intentar crear Archivo o directorio"
                                }
                            };
                        }

                        dbDao.Entry(comp).State = EntityState.Added;
                        dbDao.SaveChanges();
                        transaction.Commit();
                    }
                    return new BaseResponse
                    {
                        Result = true
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
       

        public static BaseResponse UpdatePPTransactionACQ(int _terminalId, int _acqDocument, string type, int _acqFilter,string _dbObject)
		{
            try
            {
                SuperV2_Entities db = new SuperV2_Entities();
                SqlConnection sqlConn = (SqlConnection)db.Database.Connection;

                string sql = string.Format("dbo." + _dbObject);

				List<SqlParameter> pars = new List<SqlParameter>
				{
					new SqlParameter { ParameterName = "@TerminalId", Value = _terminalId },
					new SqlParameter { ParameterName = "@AcqDocument", Value = _acqDocument },
					new SqlParameter { ParameterName = "@AcqType", Value = type },
					new SqlParameter { ParameterName = "@AcqFilter", Value = _acqFilter }
				};

				List<InvPrintModel> invList = new List<InvPrintModel>();
                DataTable dt =Common.ExecuteStoredProcedure(sqlConn.ConnectionString, sql, pars);

                return new BaseResponse
                {
                    Result = true
                };
            }
            catch (Exception ex)
            {
                LogManager.LogMessage("Ocurrio un error al insertar Log: " + ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message, (int)Constants.LogTypes.General);
                throw ex;
            }
        }

		

		public static PPBalanceResponse CreatePPBalance(PPBalance _pPBalance)
		{
            try
            {
                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    using (DbContextTransaction transaction = dbDao.Database.BeginTransaction())
                    {
                        _pPBalance.CreationDate = DateTime.Now;
                        _pPBalance.ModificationDate = DateTime.Now;
                        dbDao.PPBalance.Add(_pPBalance);
                        dbDao.SaveChanges();
                        transaction.Commit();
                    }

                    return new PPBalanceResponse
                    {
                        Result = true,
                        PPBalance = _pPBalance
                    };
                }
            }
            catch (Exception e)
            {
                String message = e.Message;
                throw;
            }
        }

		public static BaseResponse UpdateBACTerminal(PPTerminal _pPTerminal)
		{
            try
            {
                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    using (DbContextTransaction transaction = dbDao.Database.BeginTransaction())
                    {
                        PPTerminal oPPTerminal = dbDao.PPTerminal.Find(_pPTerminal.Id);

                        oPPTerminal.Password = _pPTerminal.Password;
                        oPPTerminal.TerminalId = _pPTerminal.TerminalId;
                        oPPTerminal.UserName = _pPTerminal.UserName;
                        oPPTerminal.Status = _pPTerminal.Status;
                        oPPTerminal.Currency = _pPTerminal.Currency;
                        oPPTerminal.QuickPayAmount = _pPTerminal.QuickPayAmount;
                        dbDao.Entry(oPPTerminal).State = EntityState.Modified;
                        dbDao.SaveChanges();
                        transaction.Commit();
                    }

                    return new BaseResponse
                    {
                        Result = true
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

		public static BaseResponse CreateBacTerminal(PPTerminal _bacTerminal)
		{
            try
            {
                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    using (DbContextTransaction transaction = dbDao.Database.BeginTransaction())
                    {
                        dbDao.PPTerminal.Add(_bacTerminal);
                        dbDao.SaveChanges();
                        transaction.Commit();
                    }

                    return new BaseResponse
                    {
                        Result = true
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

		/// <summary>
		/// funcion para la modificacion de la compannia y el correo de la misma,
		/// recibe como parametro la compannia y el correo
		/// </summary>
		/// <param name="companyAndMail"></param>
		/// <param name="filesInfo"></param>
		/// <returns></returns>
		public static BaseResponse UpdateCompany(CompanyAndMail companyAndMail, HttpContext filesInfo)
        {
            string path = System.Configuration.ConfigurationManager.AppSettings["FilesPath"].ToString();
            LogManager.LogMessage("Path: " + path, (int)Constants.LogTypes.General);
            try
            {
                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    using (DbContextTransaction transaction = dbDao.Database.BeginTransaction())
                    {
                        try
                        {
                            // aqui la informacion de los archivos de los cuales salen los reportes y el logo
                            var httpPostedFile = filesInfo.Request;
                            var file = httpPostedFile.Files["file"];
                            var fileInv = httpPostedFile.Files["fileInv"];
                            var fileQuo = httpPostedFile.Files["fileQuo"];
                            var fileInven = httpPostedFile.Files["fileInven"];
                            //var fileCopy = httpPostedFile.Files["fileCopy"];
                            var fileSO = httpPostedFile.Files["fileSO"];
                            var fileBalance = httpPostedFile.Files["fileBalance"];
                            var fileOinvPP = httpPostedFile.Files["fileOinvPP"];
                            var fileOinvCopy = httpPostedFile.Files["fileOinvCopy"];
                            string companyCode = companyAndMail.company.DBCode;

                            // actualizacion compannia
                            Companys company = dbDao.Companys.Find(companyAndMail.company.Id);

                            company.LogoPath = file != null ? path + file.FileName : company.LogoPath;
                            company.ReportPath = fileInv != null ? path + fileInv.FileName : company.ReportPath;
                            //company.ReportPathCopy = fileCopy != null ? path + fileCopy.FileName : company.ReportPathCopy;
                            company.ReportPathInventory = fileInven != null ? path + fileInven.FileName : company.ReportPathInventory;
                            company.ReportPathQuotation = fileQuo != null ? path + fileQuo.FileName : company.ReportPathQuotation;
                            company.ReportPathSO = fileSO != null ? path + fileSO.FileName : company.ReportPathSO;
                            company.ReportBalance = fileBalance != null ? path + fileBalance.FileName : company.ReportBalance;
                            company.ReportPathPP = fileOinvPP != null ? path + fileOinvPP.FileName : company.ReportPathPP;
                            company.ReportPathCopy = fileOinvCopy != null ? path + fileOinvCopy.FileName : company.ReportPathCopy;

                            company.LineMode = companyAndMail.company.LineMode;
                            company.DBCode = companyAndMail.company.DBCode;
                            company.DBName = companyAndMail.company.DBName;
                            company.SAPConnectionId = companyAndMail.company.SAPConnectionId;
                            company.Active = companyAndMail.company.Active;
                            company.ExchangeRate = companyAndMail.company.ExchangeRate;
                            company.ExchangeRateValue = companyAndMail.company.ExchangeRateValue;
                            company.HandleItem = companyAndMail.company.HandleItem;
                            company.BillItem = companyAndMail.company.BillItem;

                            //company.SP_ItemInfo = companyAndMail.company.SP_ItemInfo;
                            //company.SP_InvoiceInfoPrint = companyAndMail.company.SP_InvoiceInfoPrint;
                            //company.SP_WHAvailableItem = companyAndMail.company.SP_WHAvailableItem;
                            //company.SP_SeriesByItem = companyAndMail.company.SP_SeriesByItem;
                            //company.SP_PayDocuments = companyAndMail.company.SP_PayDocuments;
                            //company.SP_CancelPayment = companyAndMail.company.SP_CancelPayment;

                            //company.V_BPS = companyAndMail.company.V_BPS;
                            //company.V_Items = companyAndMail.company.V_Items;
                            //company.V_ExRate = companyAndMail.company.V_ExRate;
                            //company.V_Taxes = companyAndMail.company.V_Taxes;
                            //company.V_GetAccounts = companyAndMail.company.V_GetAccounts;
                            //company.V_GetCards = companyAndMail.company.V_GetCards;
                            //company.V_GetBanks = companyAndMail.company.V_GetBanks;
                            //company.V_GetSalesMan = companyAndMail.company.V_GetSalesMan;
                            company.IsLinePriceEditable = companyAndMail.company.IsLinePriceEditable;
                            company.ScaleMaxWeightToTreatAsZero = companyAndMail.company.ScaleMaxWeightToTreatAsZero;
                            company.ScaleWeightToSubstract = companyAndMail.company.ScaleWeightToSubstract;
                            company.HasOfflineMode = companyAndMail.company.HasOfflineMode;
                            company.DecimalAmountPrice = companyAndMail.company.DecimalAmountPrice;
                            company.DecimalAmountTotalLine = companyAndMail.company.DecimalAmountTotalLine;
                            company.DecimalAmountTotalDocument = companyAndMail.company.DecimalAmountTotalDocument;
                            company.PrinterConfiguration = companyAndMail.company.PrinterConfiguration;
                            company.HasZeroBilling = companyAndMail.company.HasZeroBilling;

                            // actualizacion correo
                            if (company.V_MailData != null)
                            {
                                company.V_MailData.subject = companyAndMail.mail.subject;
                                company.V_MailData.from = companyAndMail.mail.from;
                                company.V_MailData.user = companyAndMail.mail.user;
                                company.V_MailData.pass = companyAndMail.mail.pass;
                                company.V_MailData.port = companyAndMail.mail.port;
                                company.V_MailData.Host = companyAndMail.mail.Host;
                                company.V_MailData.SSL = companyAndMail.mail.SSL;
                            }
                            else
                            {
                                MailData mail = new MailData
                                {
                                    subject = companyAndMail.mail.subject,
                                    from = companyAndMail.mail.from,
                                    user = companyAndMail.mail.user,
                                    pass = companyAndMail.mail.pass,
                                    port = companyAndMail.mail.port,
                                    Host = companyAndMail.mail.Host,
                                    SSL = companyAndMail.mail.SSL,
                                };
                                company.V_MailData = mail;
                            }

                            if (!savePrintFiles(file, companyCode) || !savePrintFiles(fileInv, companyCode) || !savePrintFiles(fileQuo, companyCode) ||
                               !savePrintFiles(fileInven, companyCode) || !savePrintFiles(fileOinvCopy, companyCode) || !savePrintFiles(fileSO, companyCode) ||
                               !savePrintFiles(fileBalance, companyCode)  || !savePrintFiles(fileOinvPP, companyCode))
                            {
                                transaction.Rollback();
                                return new BaseResponse
                                {
                                    Result = false,
                                    Error = new ErrorInfo
                                    {
                                        Message = "El error al intentar crear Archivo o directorio"
                                    }
                                };
                            }

                            dbDao.Entry(company).State = EntityState.Modified;
                            dbDao.SaveChanges();
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }

                    return new BaseResponse
                    {
                        Result = true
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

		public static BaseResponse UpdatePPTransactionCancel(PPTransaction _oPPTransaction)
		{

            try
            {
                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    using (DbContextTransaction transaction = dbDao.Database.BeginTransaction())
                    {
                        PPTransaction oPPTransaction = dbDao.PPTransaction.Where(x => x.InvoiceNumber.Equals(_oPPTransaction.InvoiceNumber)).FirstOrDefault();

                        oPPTransaction.LastUpDate = _oPPTransaction.LastUpDate;
                        oPPTransaction.CanceledResponse = _oPPTransaction.CanceledResponse;
                        oPPTransaction.CanceledStatus = _oPPTransaction.CanceledStatus;
                        oPPTransaction.CharchedStatus = null;
                        oPPTransaction.ChargedResponse = null;
                        dbDao.Entry(oPPTransaction).State = EntityState.Modified;
                        dbDao.SaveChanges();
                        transaction.Commit();

                        return new BaseResponse()
                        {
                            Result = true
                        };

                    }
                }
            }
            catch
            {
                throw;
            }
        }

		public static BaseResponse createPPTransaction(PPTransaction _bacTransasction)
		{
            try
            {
                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    using (DbContextTransaction transaction = dbDao.Database.BeginTransaction())
                    {
                        dbDao.PPTransaction.Add(_bacTransasction);
                        dbDao.SaveChanges();
                        transaction.Commit();
                    }

                    return new BaseResponse
                    {
                        Result = true
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

		public static ApiResponse<bool> UpdateCompanyMargins(int idCompany, string marginsViews)
        {
            try
            {

                using (SuperV2_Entities db = new SuperV2_Entities())
                {
                    var company = db.Companys.Find(idCompany);

                    company.AcceptedMargins = marginsViews;

                    db.Entry(company).State = EntityState.Modified;

                    db.SaveChanges();
                }

                return new ApiResponse<bool>()
                {
                    Result = true
                };
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// funcion para crear un nuevo usuario
        /// recibe como parametro el modelo se usuario para agregar un nuevo registro userAssign
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static BaseResponse CreateNewUser(UserAsingModel user)
        {
            try
            {
                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    using (DbContextTransaction transaction = dbDao.Database.BeginTransaction())
                    {
                        List<UserAssign> listaUsuarios = dbDao.UserAssign.Where(x => x.UserId == user.UserId && x.CompanyId == user.CompanyId).ToList();

                        if (listaUsuarios.Count > 0)
                        {
                            throw new Exception("Usuario ya esta asignado en la Base de Datos.");
                        }

                        List<SeriesByUser> seriesList = new List<SeriesByUser>();
                        user.Series = user.Series.Where(x => x.SerieId != -1).ToList();
                        foreach (var x in user.Series)
                        {
                            seriesList.Add(new SeriesByUser
                            {
                                SerieId = x.SerieId,
                                UsrMappId = x.UsrMappId
                            });
                        }

                        UserAssign newUser = new UserAssign();
                        CompanyByUsers cpbu = new CompanyByUsers();

                        Users usuario = dbDao.Users.Where(x => x.Id == user.UserId).FirstOrDefault();

                        newUser.SAPUser = user.SAPUser;
                        newUser.UserId = user.UserId;
                        newUser.SAPPass = user.SAPPass;
                        newUser.minDiscount = user.minDiscount;
                        newUser.PriceListDef = user.PriceListDef;
                        newUser.SlpCode = user.SlpCode;
                        newUser.StoreId = user.StoreId;
                        newUser.SuperUser = user.SuperUser;
                        newUser.CompanyId = user.CompanyId;
                        newUser.CenterCost = user.CenterCost;
                        newUser.Active = user.Active;
                        newUser.V_SeriesByUser = seriesList;

                        usuario.Active = user.Active;

                        dbDao.UserAssign.Add(newUser);
                        dbDao.Entry(usuario).State = EntityState.Modified;
                        dbDao.SaveChanges();
                        transaction.Commit();
                    }
                    return new BaseResponse
                    {
                        Result = true
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// funcion para editar usuario
        /// recibe como parametro el modelo se usuario para editar un nuevo registro userAssign
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static BaseResponse UpdateUser(UserAsingModel user)
        {
            try
            {
                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    using (DbContextTransaction transaction = dbDao.Database.BeginTransaction())
                    {
                        List<SeriesByUser> series = dbDao.SeriesByUser.Where(x => x.UsrMappId == user.Id).ToList();
                        foreach (SeriesByUser s in series)
                        {
                            dbDao.SeriesByUser.Remove(s);
                            dbDao.SaveChanges();
                        }
                        user.Series = user.Series.Where(x => x.SerieId != -1).ToList();

                        List<SeriesByUser> seriesList = new List<SeriesByUser>();
                        foreach (var x in user.Series)
                        {
                            seriesList.Add(new SeriesByUser
                            {
                                SerieId = x.SerieId,
                                UsrMappId = x.UsrMappId
                            });
                        }

                        UserAssign newUser = dbDao.UserAssign.Where(x => x.Id == user.Id).FirstOrDefault();
                        Users usuario = dbDao.Users.Where(x => x.Id == user.UserId).FirstOrDefault();
                        newUser.SAPUser = user.SAPUser;
                        newUser.SAPPass = user.SAPPass;
                        newUser.minDiscount = user.minDiscount;
                        newUser.PriceListDef = user.PriceListDef;
                        newUser.SlpCode = user.SlpCode;
                        newUser.StoreId = user.StoreId;
                        newUser.SuperUser = user.SuperUser;
                        newUser.CenterCost = user.CenterCost;
                        newUser.Active = user.Active;
                        newUser.V_SeriesByUser = seriesList;
                        usuario.Active = user.Active;

                        dbDao.Entry(usuario).State = EntityState.Modified;
                        dbDao.Entry(newUser).State = EntityState.Modified;
                        dbDao.SaveChanges();
                        transaction.Commit();
                    }
                    return new BaseResponse
                    {
                        Result = true
                    };
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public static BaseResponse UpdateUserApp(UserModel _user)
        {
            try
            {
                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    using (DbContextTransaction transaction = dbDao.Database.BeginTransaction())
                    {

                        if (dbDao.Users.Where(x => x.Email.Equals(_user.Email) && x.Id != _user.Id).FirstOrDefault() != null)
                        {
                            throw new Exception("El correo ya se encuentra registrado");

                        }
                        Users oUsers = dbDao.Users.Find(_user.Id);
                        oUsers.Email = _user.Email;
                        //oUsers.PasswordHash = Common.Encrypt(_user.PasswordHash);
                        oUsers.UserName = _user.Email;
                        oUsers.EmailConfirmed = false;
                        oUsers.Active = _user.Active;
                        oUsers.FullName = _user.FullName;
                        if (!String.IsNullOrEmpty(_user.PasswordHash))
                        {
                            oUsers.PasswordHash = COMMON.Common.Encrypt(_user.PasswordHash);
                        }


                        dbDao.Entry(oUsers).State = EntityState.Modified;
                        dbDao.SaveChanges();
                        transaction.Commit();
                    }
                    return new BaseResponse
                    {
                        Result = true
                    };
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static BaseResponse CreateUserApp(UserModel _user)
        {
            try
            {
                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    using (DbContextTransaction transaction = dbDao.Database.BeginTransaction())
                    {

                        if (dbDao.Users.Where(x => x.Email.Equals(_user.Email)).FirstOrDefault() != null)
                        {
                            throw new Exception("El correo ya se encuentra registrado");

                        }

                        Users oUsers = new Users();
                        oUsers.Id = Common.Encrypt(_user.Email);
                        oUsers.Email = _user.Email;
                        oUsers.PasswordHash = Common.Encrypt(_user.PasswordHash);
                        oUsers.UserName = _user.Email;
                        oUsers.EmailConfirmed = false;
                        oUsers.Active = _user.Active;
                        oUsers.CreateDate = DateTime.Now;
                        oUsers.FullName = _user.FullName;
                        dbDao.Users.Add(oUsers);
                        dbDao.SaveChanges();
                        transaction.Commit();
                    }
                    return new BaseResponse
                    {
                        Result = true
                    };
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        /// <summary>
        /// Crea una nueva serie en la bd
        /// Recibe como parametro el modelo de la serie
        /// </summary>
        /// <param name="serie"></param>
        /// <returns></returns>
        public static BaseResponse CreateNewSerie(NumberingSeriesModel serie)
        {
            try
            {
                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    using (DbContextTransaction transaction = dbDao.Database.BeginTransaction())
                    {
                        Series newSerie = new Series();
                        newSerie.Name = serie.Name;
                        newSerie.DocType = serie.DocType;
                        newSerie.Active = serie.Active;
                        newSerie.CompanyId = serie.CompanyId;
                        newSerie.Numbering = serie.Numbering;
                        newSerie.Serie = serie.Serie;
                        newSerie.Type = serie.Type;
                        dbDao.Series.Add(newSerie);
                        dbDao.SaveChanges();
                        transaction.Commit();
                    }

                    return new BaseResponse
                    {
                        Result = true
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// Actualiza los cambios cuando se modifica una serie
        /// Recibe como parametro el modelo de la serie
        /// </summary>
        /// <param name="serie"></param>
        /// <returns></returns>
        public static BaseResponse UpdateSerie(NumberingSeriesModel serie)
        {
            try
            {
                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    using (DbContextTransaction transaction = dbDao.Database.BeginTransaction())
                    {
                        Series newSerie = dbDao.Series.Find(serie.Id);
                        newSerie.Name = serie.Name;
                        newSerie.DocType = serie.DocType;
                        newSerie.Active = serie.Active;
                        newSerie.CompanyId = serie.CompanyId;
                        newSerie.Numbering = serie.Numbering;
                        newSerie.Serie = serie.Serie;
                        newSerie.Type = serie.Type;
                        dbDao.Entry(newSerie).State = EntityState.Modified;
                        dbDao.SaveChanges();
                        transaction.Commit();
                    }

                }
                return new BaseResponse
                {
                    Result = true
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// Funcion para registrar un nuevo usuario
        /// Recibe como parametro el modelo de usuario para agregarlo a la aplicacion
        /// </summary>
        /// <param name="registerUser"></param>
        /// <returns></returns>
        public static BaseResponse RegisterUser(User registerUser)
        {
            try
            {
                DateTime date = DateTime.Now;
                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    using (DbContextTransaction transaction = dbDao.Database.BeginTransaction())
                    {
                        // SE ENCRIPTA EL CORREO Y PASSWORD PARA EL USERID
                        string encryptUser = Common.Encrypt(registerUser.Email);
                        // SE ENCRIPTA EL CORREO Y PASSWORD PARA EL USERID
                        string encryptClienPass = Common.Encrypt(registerUser.Password);

                        // SE CREA EL OBJETO USUARIO
                        Users user = new Users
                        {
                            Id = encryptUser,
                            FullName = registerUser.FullName,
                            Email = registerUser.Email,
                            EmailConfirmed = false,
                            UserName = registerUser.Email,
                            PasswordHash = encryptClienPass,
                            Active = true,
                            CreateDate = date
                        };

                        dbDao.Entry(user).State = EntityState.Added;
                        dbDao.SaveChanges();
                        transaction.Commit();

                        var userAssign = dbDao.UserAssign.FirstOrDefault();
                        var newUserAssign = new UserAssign();


                        if (userAssign != null)
                        {
                            newUserAssign.Active = true;
                            newUserAssign.CenterCost = userAssign.CenterCost;
                            newUserAssign.CompanyId = userAssign.CompanyId;
                            newUserAssign.SAPUser = userAssign.SAPUser;
                            newUserAssign.SAPPass = userAssign.SAPPass;
                            newUserAssign.SlpCode = userAssign.SlpCode;
                            newUserAssign.StoreId = userAssign.StoreId;
                            newUserAssign.minDiscount = userAssign.minDiscount;
                            newUserAssign.PriceListDef = userAssign.PriceListDef;
                            newUserAssign.UserId = user.Id;
                        }

                        dbDao.UserAssign.Add(newUserAssign);
                        dbDao.SaveChanges();
                    }

                    return new BaseResponse
                    {
                        Result = true
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Metodo para el envio de la informcion para el cambio de la contrasena de la cuenta del usuario
        /// recibe como parametro el modelo de usuario para agregarlo a la actualizacion de la contrasena de la cuenta
        /// </summary>
        /// <param name="recoverPswd"></param>
        /// <returns></returns>
        public static BaseResponse RecoverPswd(User recoverPswd)
        {
            try
            {
                DateTime date = DateTime.Now;
                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    using (DbContextTransaction transaction = dbDao.Database.BeginTransaction())
                    {
                        // SE ENCRIPTA EL CORREO Y PASSWORD PARA EL USERID
                        string encryptClienPass = Common.Encrypt(recoverPswd.Password);

                        Users user = dbDao.Users.Where(x => x.Email == recoverPswd.Email).FirstOrDefault();
                        user.PasswordHash = encryptClienPass;
                        dbDao.Entry(user).State = EntityState.Modified;
                        dbDao.SaveChanges();
                        transaction.Commit();
                    }
                    return new BaseResponse
                    {
                        Result = true
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// actualiza los cambios cuando se un parametro asociado a una vista
        /// recibe como parametro una lista de un modelo de parametros para una vista
        /// </summary>
        /// <param name="Params"></param>
        /// <returns></returns>
        public static BaseResponse UpdateParamsViewState(List<ParamsModel> Params)
        {
            try
            {
                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    using (DbContextTransaction transaction = dbDao.Database.BeginTransaction())
                    {
                        foreach (var x in Params)
                        {
                            ParamsViewCompany pbc = dbDao.ParamsViewCompany.Find(x.Id);
                            pbc.Order = x.Order;
                            pbc.Visibility = x.Visibility;
                            pbc.Text = x.Text;
                            dbDao.Entry(pbc).State = EntityState.Modified;
                        }

                        dbDao.SaveChanges();
                        transaction.Commit();
                    }
                    return new BaseResponse
                    {
                        Result = true
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Funcion que verifica si la direccion del archivo y el archivo existe
        /// Recibe un archivo y el nombre de una compania como parametros
        /// Retorna false si no se envio un archivo valido
        /// Retorna true si el archivo no es vacio
        /// si el archivo ya existia lo sobreescribe, si no lo guarda
        /// </summary>
        /// <param name="file"></param>
        /// <param name="company"></param>
        /// <returns></returns>
        public static bool savePrintFiles(HttpPostedFile file, string company)
        {
            string path = System.Configuration.ConfigurationManager.AppSettings["FilesPath"].ToString();
            LogManager.LogMessage("Path on savePrintFiles: " + path, (int)Constants.LogTypes.General);
            try
            {
                if (file != null)
                {
                    if (System.IO.Directory.Exists(path))
                    {
                        if (System.IO.File.Exists(path + file.FileName))
                        {
                            System.IO.File.Delete(path + file.FileName);
                            file.SaveAs(path + file.FileName);
                        }
                        else
                        {
                            file.SaveAs(path + file.FileName);
                        }
                    }
                    else
                    {
                        System.IO.Directory.CreateDirectory(path);
                        file.SaveAs(path + file.FileName);

                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LogManager.LogMessage("Error " + ex.Message, (int)Constants.LogTypes.General);
                return false;
            }
        }

        /// <summary>
        /// Crea una tienda asociada a una compania
        /// Recibe un modelo que contiene  los datos de la tienda a crear
        /// recibe un modelo de la tienda a crear
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        public static BaseResponse CreateStore(StoresModel store)
        {
            try
            {
                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    if (dbDao.Store != null && dbDao.Store.Any(x => x.WhCode == store.StoreCode))
                        return new BaseResponse
                        {
                            Result = false,
                            Error = new ErrorInfo { Code = -1, Message = "Existe un almacén con el mismo codigo de SAP" }
                        };

                    using (DbContextTransaction transaction = dbDao.Database.BeginTransaction())
                    {
                        Store tienda = new Store();
                        tienda.Active = store.StoreStatus;
                        tienda.CompanyId = Convert.ToInt32(store.companyName);
                        tienda.Name = store.Name;
                        tienda.WhName = store.StoreName;
                        tienda.WhCode = store.StoreCode;
                        dbDao.Store.Add(tienda);
                        dbDao.SaveChanges();
                        transaction.Commit();
                    }

                    return new BaseResponse
                    {
                        Result = true
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Actualiza los datos de una tienda 
        /// Recibe los datos de la tienda en un modelo de la tienda
        /// </summary>
        /// <param name="store"></param>
        /// <returns></returns>
        public static BaseResponse UpdateStore(StoresModel store)
        {
            try
            {
                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    if (dbDao.Store.Any(x => x.WhCode == store.StoreCode && x.Id != store.Id))
                        return new BaseResponse
                        {
                            Result = false,
                            Error = new ErrorInfo { Code = -1, Message = "Existe un almacén con el mismo codigo de SAP" }
                        };

                    using (DbContextTransaction transaction = dbDao.Database.BeginTransaction())
                    {
                        Store tienda = dbDao.Store.Where(x => x.Id == store.Id).FirstOrDefault();
                        tienda.Active = store.StoreStatus;
                        tienda.CompanyId = Convert.ToInt32(store.companyName);
                        tienda.Name = store.Name;
                        tienda.WhName = store.StoreName;
                        tienda.WhCode = store.StoreCode;
                        dbDao.Entry(tienda).State = EntityState.Modified;
                        dbDao.SaveChanges();
                        transaction.Commit();
                    }
                    return new BaseResponse
                    {
                        Result = true
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static BaseResponse UpdateLog(LogModel _logModel, string _typeDocument, string _document, DateTime? _startTimeSapDocument,
            DateTime? _endTimeSapDocument, string _elapsedTimeSapDocument, DateTime? _startTimeDocument, DateTime? _endTimeDocument, DateTime? _startTimeCompany,
            DateTime? _endTimeCompany, string _errorDetail)
        {
            try
            {
                string ElapsedTimeCompany = _startTimeCompany != null && _endTimeCompany != null ? Convert.ToString(_endTimeCompany - _startTimeCompany) : "";
                string ElapsedTimeCreateDocument = _startTimeDocument != null && _endTimeDocument != null ? Convert.ToString(_endTimeDocument - _startTimeDocument) : "";
                string connString = System.Configuration.ConfigurationManager.AppSettings["SuperV2_Entities"];
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    SqlParameter[] parameters = new SqlParameter[]
                       {
                           new SqlParameter("LogId", _logModel.Id),
                            new SqlParameter("TypeDocument", _typeDocument),
                            new SqlParameter("Document", _document),
                            new SqlParameter("StartTimeDocument", (Object)_startTimeDocument ?? DBNull.Value),
                            new SqlParameter("EndTimeDocument", (Object)_endTimeDocument ?? DBNull.Value),
                            new SqlParameter("ElapsedTimeCreateDocument", ElapsedTimeCreateDocument),
                            new SqlParameter("StartTimeCompany", (Object)_startTimeCompany ?? DBNull.Value),
                            new SqlParameter("EndTimeCompany", (Object)_endTimeCompany ?? DBNull.Value),
                            new SqlParameter("ElapsedTimeCompany", ElapsedTimeCompany),
                            new SqlParameter("StartTimeSapDocument", (Object)_startTimeSapDocument ?? DBNull.Value),
                            new SqlParameter("EndTimeSapDocument", (Object)_endTimeSapDocument ?? DBNull.Value),
                            new SqlParameter("ElapsedTimeSapDocument",(Object)_elapsedTimeSapDocument ?? ""),
                            new SqlParameter("ErrorDetail", _errorDetail),
                       };

                    string sqlQuery = "EXEC " + DAO.GetData.GetNameObject("spUpdateLog");
                    for (int param = 0; param < parameters.Count(); param++)
                    {
                        sqlQuery += " @" + parameters[param].ToString();
                        sqlQuery += (param != (parameters.Count() - 1)) ? "," : string.Empty;
                    }
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                    {
                        cmd.Parameters.AddRange(parameters);
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
                return new BaseResponse
                {
                    Result = true
                };
            }
            catch (Exception ex)
            {
                LogManager.LogMessage("Ocurrio un error al insertar Log: " + ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message, (int)Constants.LogTypes.General);
                throw ex;
            }
        }

        public static LogModelResponse CreateLog(int _typeDocument, string _document, DateTime _startTimeDocument)
        {
            try
            {

                string POLICY = Convert.ToString(_startTimeDocument - _startTimeDocument);
                string connString = System.Configuration.ConfigurationManager.AppSettings["SuperV2_Entities"];
                int LogId = -1;
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("TypeDocument", _typeDocument ),
                        new SqlParameter("Document", _document ),
                        new SqlParameter("StartTimeDocument", _startTimeDocument ),
                        //new SqlParameter("EndTimeDocument",_startTimeDocument ),
                        //new SqlParameter("ElapsedTimeCreateDocument", _startTimeDocument ),
                        //new SqlParameter("StartTimeCompany", _startTimeDocument ),
                        //new SqlParameter("EndTimeCompany", _startTimeDocument ),
                        //new SqlParameter("ElapsedTimeCompany", _startTimeDocument ),
                        //new SqlParameter("StartTimeSapDocument", _startTimeDocument ),
                        //new SqlParameter("EndTimeSapDocument", _startTimeDocument ),
                        //new SqlParameter("ElapsedTimeSapDocument", _startTimeDocument ),
                    };

                    string sqlQuery = "EXEC " + DAO.GetData.GetNameObject("spInsertLog");
                    for (int param = 0; param < parameters.Count(); param++)
                    {
                        sqlQuery += " @" + parameters[param].ToString();
                        sqlQuery += (param != (parameters.Count() - 1)) ? "," : string.Empty;
                    }

                    DataTable dt = new DataTable();

                    using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                    {
                        cmd.Parameters.AddRange(parameters);
                        //cmd.ExecuteNonQuery();
                        SqlDataAdapter oSqlDataAdapter = new SqlDataAdapter();
                        oSqlDataAdapter.SelectCommand = cmd;
                        oSqlDataAdapter.Fill(dt);
                    }

                    if (dt.Rows.Count > 0)
                    {
                        LogId = Convert.ToInt32(dt.Rows[0]["LogId"]);
                    }

                    conn.Close();
                }
                return new LogModelResponse
                {
                    LogModel = new LogModel
                    {
                        Id = LogId
                    },
                    Result = true
                };
            }
            catch (Exception ex)
            {
                LogManager.LogMessage("Ocurrio un error al insertar Log: " + ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message, (int)Constants.LogTypes.General);
                throw ex;
            }
        }

        public static BaseResponse UpdateViewLineAgrupation(ViewLinesAgrupationList ViewGroupList)
        {
            try
            {
                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    using (DbContextTransaction transaction = dbDao.Database.BeginTransaction())
                    {
                        try
                        {
                            foreach (var view in ViewGroupList.viewLineAgrupationList)
                            {
                                // actualizacion agrupaciones                            
                                ViewLineAgrupation viewLineAgrupation = dbDao.ViewLineAgrupation.Find(view.Id);
                                viewLineAgrupation.CodNum = view.CodNum;
                                viewLineAgrupation.NameView = view.NameView;
                                viewLineAgrupation.isGroup = view.isGroup;
                                viewLineAgrupation.LineMode = view.LineMode;

                                dbDao.Entry(viewLineAgrupation).State = EntityState.Modified;


                            }
                            dbDao.SaveChanges();
                            transaction.Commit();


                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }

                    return new BaseResponse
                    {
                        Result = true
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        // Guarda las configuraciones de udfs seleccionadas por el usuario de aplicacion
        public static BaseResponse SaveUdf(List<Udf> _udfs, string _tableId)
        {
            try
            {
                SuperV2_Entities oSuperV2_Entities = new SuperV2_Entities();

                string serializedUdfs = JsonConvert.SerializeObject(_udfs); // Todos los udfs por tabla se guardar serializados

                CompanyUdfs oCompanyUdfs = oSuperV2_Entities.CompanyUdfs.Where(y => y.TableId.Equals(_tableId)).FirstOrDefault();
                // El objetivo es si lo encuentra lo actualiza sino lo crea
                if (oCompanyUdfs == null)
                {
                    oCompanyUdfs = new CompanyUdfs();

                    oCompanyUdfs.TableId = _tableId;
                    oCompanyUdfs.Udfs = serializedUdfs;

                    oSuperV2_Entities.CompanyUdfs.Add(oCompanyUdfs);
                    oSuperV2_Entities.SaveChanges();
                }
                else
                {
                    oSuperV2_Entities.Entry(oCompanyUdfs).State = EntityState.Deleted;
                    oSuperV2_Entities.SaveChanges();

                    oCompanyUdfs.TableId = oCompanyUdfs.TableId;
                    oCompanyUdfs.Udfs = serializedUdfs;

                    oSuperV2_Entities.CompanyUdfs.Add(oCompanyUdfs);
                    oSuperV2_Entities.SaveChanges();
                }

                oSuperV2_Entities.Dispose();
                oSuperV2_Entities = null;
                return new BaseResponse
                {
                    Result = true
                };

            }
            catch (Exception)
            {
                throw;
            }
        }
        public static BaseResponse SaveSettings(Settings _settings)
        {
            try
            {

                SuperV2_Entities oSuperV2_Entities = new SuperV2_Entities();
                Settings setting = oSuperV2_Entities.Settings.Where(x => x.Codigo == _settings.Codigo).FirstOrDefault();
                if (setting == null)
                {
                    setting = new Settings();
                    setting.Codigo = _settings.Codigo;
                    setting.Vista = _settings.Vista;
                    setting.Json = _settings.Json;

                    oSuperV2_Entities.Settings.Add(setting);
                }
                else
                {
                    setting.Json = _settings.Json;
                    oSuperV2_Entities.Entry(setting).State = EntityState.Modified;

                }
                oSuperV2_Entities.SaveChanges();
                oSuperV2_Entities.Dispose();
                oSuperV2_Entities = null;
                return new BaseResponse()
                {
                    Result = true
                };
            }
            catch
            {
                throw;
            }
        }

        #region DbObjectNames
        public static BaseResponse UpdateDbObjectNames(List<DBObjectName> DBObjectNameList)
        {
            try
            {
                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    using (DbContextTransaction transaction = dbDao.Database.BeginTransaction())
                    {
                        try
                        {
                            foreach (var obj in DBObjectNameList)
                            {
                                DBObjectName nameObject = dbDao.DBObjectName.Find(obj.Id);
                                nameObject.DbObject = obj.DbObject;

                                dbDao.Entry(nameObject).State = EntityState.Modified;
                            }
                            dbDao.SaveChanges();
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                    }

                    return new BaseResponse
                    {
                        Result = true
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Paydesk balance
        public static BaseResponse PostPaydeskBalance(PaydeskBalance paydeskBalance, string userId)
        {
            object[] param = new object[]
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@UserSignature", paydeskBalance.UserSignature),
                new SqlParameter("@Cash", paydeskBalance.Cash),
                new SqlParameter("@Cards", paydeskBalance.Cards),
                new SqlParameter("@CardsPinpad", paydeskBalance.CardsPinpad),
                new SqlParameter("@Transfer", paydeskBalance.Transfer),
                new SqlParameter("@CashflowIncomme", paydeskBalance.CashflowIncomme),
                new SqlParameter("@CashflowEgress", paydeskBalance.CashflowEgress)
            };

            using (SuperV2_Entities db = new SuperV2_Entities())
            {
                db.Database.ExecuteSqlCommand("EXEC [dbo]. " + DAO.GetData.GetNameObject("spInsertPayDeskBalance") + " @UserId, @UserSignature, @Cash, @Cards,@CardsPinpad, @Transfer, @CashflowIncomme, @CashflowEgress", param);
            }

            return new BaseResponse { Result = true };
        }

		public static void UpdateLastTransaction(PPTransaction _pPTransaction, DateTime _lastUpdate)
		{
            try
            {
                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    using (DbContextTransaction transaction = dbDao.Database.BeginTransaction())
                    {
                        PPTransaction oPPTransaction = dbDao.PPTransaction.Where(x => x.TransactionId == _pPTransaction.TransactionId).FirstOrDefault();

                        oPPTransaction.LastUpDate = _lastUpdate;
                        oPPTransaction.UpdatesCounter = oPPTransaction.UpdatesCounter + 1;
                        dbDao.Entry(oPPTransaction).State = EntityState.Modified;
                        dbDao.SaveChanges();
                        transaction.Commit();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

		public static void UpdatePPTransactionReferences(PPTransaction _pPTransaction, DateTime _lastUpdate, int _docNum, int _docEntry)
		{
            try
            {
                using (SuperV2_Entities dbDao = new SuperV2_Entities())
                {
                    using (DbContextTransaction transaction = dbDao.Database.BeginTransaction())
                    {
                        PPTransaction oPPTransaction = dbDao.PPTransaction.Where(x => x.TransactionId == _pPTransaction.TransactionId).FirstOrDefault();

                        oPPTransaction.LastUpDate = _lastUpdate;
                        oPPTransaction.DocEntry = _docEntry;
                        oPPTransaction.DocNum = _docNum;
                        dbDao.Entry(oPPTransaction).State = EntityState.Modified;
                        dbDao.SaveChanges();
                        transaction.Commit();
                    }
                }
            }
            catch
            {
                throw;
            }
        }
		#endregion
	}
}
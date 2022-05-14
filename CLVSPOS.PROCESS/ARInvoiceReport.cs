using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CLVSPOS.DAO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using CLVSPOS.COMMON;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using CLVSSUPER.MODELS;
using CLVSPOS.MODELS;
using static CLVSPOS.COMMON.Constants;

namespace CLVSPOS.PROCESS
{
    public class ApplyCRLogin
    {
        public void apply_info(ref ReportDocument obj_report, string server, string db)
        {
            Database obj_db = obj_report.Database;
            Tables obj_tables = obj_db.Tables;
            TableLogOnInfo obj_table_logon_info = new TableLogOnInfo();
            ConnectionInfo obj_cr_connection_info = new ConnectionInfo();
            TableLogOnInfo obj_connection_info = new TableLogOnInfo();


            obj_connection_info.ConnectionInfo.ServerName = server;
            obj_connection_info.ConnectionInfo.DatabaseName = db;
            obj_connection_info.ConnectionInfo.UserID = "CrystalSAPConSP";
            obj_connection_info.ConnectionInfo.Password = "CrystalR18+";

            Sections sections = obj_report.ReportDefinition.Sections;
            foreach (Section section in sections)
            {
                ReportObjects reportObjects = section.ReportObjects;
                foreach (ReportObject reportObject in reportObjects)
                {
                    if (reportObject.Kind == ReportObjectKind.SubreportObject)
                    {
                        SubreportObject subreportObject = (SubreportObject)reportObject;
                        ReportDocument subReportDocument = subreportObject.OpenSubreport(subreportObject.SubreportName);
                        foreach (Table table in subReportDocument.Database.Tables)
                        {
                            obj_table_logon_info = table.LogOnInfo;
                            obj_table_logon_info.ConnectionInfo = obj_cr_connection_info;
                            table.ApplyLogOnInfo(obj_connection_info);
                        }
                    }
                }
            }


            foreach (Table cr_table in obj_tables)
            {
                obj_table_logon_info = cr_table.LogOnInfo;
                obj_table_logon_info.ConnectionInfo = obj_cr_connection_info;
                cr_table.ApplyLogOnInfo(obj_connection_info);
            }

        }
    }

    public class ARInvoiceReport
    {

        /// <summary>
        /// Funcion que retorna el id del usuario logueado
        /// </summary>
        /// <returns></returns>
        public static string GetUserId()
        {
            // se obtiene el userId, localizado en los Claims
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            return identity.Claims.Where(c => c.Type == "userId").Single().Value;
        }
        
        public static ApiResponse<string> PrintReport(int DocEntry, int ReportType)
        {
            try
            {
                var userId = GetUserId();
                var company = GetData.GetCompanyByUserId(userId);
                string path = "";
                byte[] _contentBytes;
                ReportDocument reportDocument = new ReportDocument();

                switch (ReportType)
                {
                    case (int)Constants.ReportTypes.SaleOrder:
                        path = company.ReportPathSO;
                        reportDocument.Load(path);
                        break;
                    case (int)Constants.ReportTypes.Inventory:
                        path = company.ReportPathInventory;
                        reportDocument.Load(path);
                        break;
                    case (int)Constants.ReportTypes.Quotation:
                        path = company.ReportPathQuotation;
                        reportDocument.Load(path);
                        break;
                    case (int)Constants.ReportTypes.ArInvoice:
                        path = company.ReportPath;
                        reportDocument.Load(path);
                        break;
                    case (int)Constants.ReportTypes.RecivedPaid:
                        path = company.ReportRecivedPaid;
                        reportDocument.Load(path);
                        break;
                }
                
                CLVSPOS.LOGGER.LogManager.LogMessage("Loaded Report From path: " + path, 1);

                reportDocument.SetDatabaseLogon("CrystalSAPConSP", "CrystalR18+");
                reportDocument.SetParameterValue("@DocEntry", DocEntry);


                CLVSPOS.LOGGER.LogManager.LogMessage("Applied Login and Parameter DOcEntry: " + DocEntry, 1);

                _contentBytes = StreamToBytes(reportDocument.ExportToStream(ExportFormatType.PortableDocFormat));
                string b64 = Convert.ToBase64String(_contentBytes); 
                reportDocument.Close();
                reportDocument.Dispose();

                return new ApiResponse<string>
                {
                    Result = true,
                    Error = null,
                    Data = b64
                };

            }
            catch (Exception exc) {
                throw;
            }            
        }

        #region PP_REPORTS
        public static PPReportResponse PrintReportPP(TransactionPrint _transactionPrint)
        {
            try
            {
                var userId = GetUserId();
                var company = GetData.GetCompanyByUserId(userId);
                string path = "";
                string unSignedReport = String.Empty;
                string signedReport = String.Empty;
                byte[] _contentBytes;
                ReportDocument reportDocument = new ReportDocument();

                path = company.ReportPathPP;
                reportDocument.Load(path);
                
                if (_transactionPrint.IsSigned)
                {
                    CLVSPOS.LOGGER.LogManager.LogMessage("Loaded Report From path: " + path, 1);

                    reportDocument.SetDatabaseLogon("CrystalSAPConSP", "CrystalR18+");
                    reportDocument.SetParameterValue("@DocEntry", _transactionPrint.DocEntry);
                    reportDocument.SetParameterValue("@Terminal", _transactionPrint.TerminalCode);
                    reportDocument.SetParameterValue("@MaskedCard", _transactionPrint.MaskedNumberCard);
                    reportDocument.SetParameterValue("PrintTags", _transactionPrint.PrintTags);
                    reportDocument.SetParameterValue("IsSigned", (int)VOUCHER_SIGN.SIGNED_WITH_COPY);

                    CLVSPOS.LOGGER.LogManager.LogMessage("Applied Login and Parameter DOcEntry: " + _transactionPrint.DocEntry, (int)Constants.LogTypes.General);

                    _contentBytes = StreamToBytes(reportDocument.ExportToStream(ExportFormatType.PortableDocFormat));
                    unSignedReport = Convert.ToBase64String(_contentBytes);

                    reportDocument.SetParameterValue("@DocEntry", _transactionPrint.DocEntry);
                    reportDocument.SetParameterValue("@Terminal", _transactionPrint.TerminalCode);
                    reportDocument.SetParameterValue("@MaskedCard", _transactionPrint.MaskedNumberCard);
                    reportDocument.SetParameterValue("PrintTags", _transactionPrint.PrintTags);
                    reportDocument.SetParameterValue("IsSigned", (int)VOUCHER_SIGN.SIGNED);

                    CLVSPOS.LOGGER.LogManager.LogMessage("Applied Login and Parameter DOcEntry signed: " + _transactionPrint.DocEntry, (int)Constants.LogTypes.General);

                    _contentBytes = StreamToBytes(reportDocument.ExportToStream(ExportFormatType.PortableDocFormat));
                    signedReport = Convert.ToBase64String(_contentBytes);
                }
                else
                {
                    CLVSPOS.LOGGER.LogManager.LogMessage("Loaded Report From path: " + path, 1);

                    reportDocument.SetDatabaseLogon("CrystalSAPConSP", "CrystalR18+");
                    reportDocument.SetParameterValue("@DocEntry", _transactionPrint.DocEntry);
                    reportDocument.SetParameterValue("@Terminal", _transactionPrint.TerminalCode);
                    reportDocument.SetParameterValue("@MaskedCard", _transactionPrint.MaskedNumberCard);
                    reportDocument.SetParameterValue("PrintTags", _transactionPrint.PrintTags);
                    reportDocument.SetParameterValue("IsSigned", (int) VOUCHER_SIGN.NO_SIGNED);

                    CLVSPOS.LOGGER.LogManager.LogMessage("Applied Login and Parameter DOcEntry: " + _transactionPrint.DocEntry, (int)Constants.LogTypes.General);

                    _contentBytes = StreamToBytes(reportDocument.ExportToStream(ExportFormatType.PortableDocFormat));
                    unSignedReport = Convert.ToBase64String(_contentBytes);
                }

                reportDocument.Close();
                reportDocument.Dispose();

                return new PPReportResponse()
                {
                    UnsignedReport = unSignedReport,
                    SignedReport = signedReport,
                    Result = true
                };
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        public static ApiResponse<string> PrintVoucher(PPTransaction _pPTransaction)
        {
            try
            {
                var userId = GetUserId();
                var company = GetData.GetCompanyByUserId(userId);
                string path = "";
                byte[] _contentBytes;
                ReportDocument reportDocument = new ReportDocument();

                path = Common.GetDBObjectByKey(System.Reflection.MethodBase.GetCurrentMethod(), "VoucherPath"); //System.Configuration.ConfigurationManager.AppSettings["VoucherPath"];
               
                reportDocument.Load(path);

                CLVSPOS.LOGGER.LogManager.LogMessage("Loaded Report From path: " + path, 1);
                
                
                reportDocument.SetDatabaseLogon("CrystalSAPConSP", "CrystalR18+");
                //Authorizatio
                //CreationDate
                //ReferenceNum
                //SystemTrace
                //TransactionI
                reportDocument.SetParameterValue("@DocEntry", 0);
                reportDocument.SetParameterValue("@Autho", _pPTransaction.AuthorizationNumber);
                reportDocument.SetParameterValue("@Date", _pPTransaction.CreationDate);
                reportDocument.SetParameterValue("@RefNum", _pPTransaction.ReferenceNumber);
                reportDocument.SetParameterValue("@SysTrac", _pPTransaction.SystemTrace);
                reportDocument.SetParameterValue("@TransId", _pPTransaction.TransactionId);
                reportDocument.SetParameterValue("@SaleAmount", _pPTransaction.SaleAmount);

                CLVSPOS.LOGGER.LogManager.LogMessage("Applied Login and Parameter DOcEntry: " + _pPTransaction.ToString(), 1);

                _contentBytes = StreamToBytes(reportDocument.ExportToStream(ExportFormatType.PortableDocFormat));
                string b64 = Convert.ToBase64String(_contentBytes);
                reportDocument.Close();
                reportDocument.Dispose();

                return new ApiResponse<string>
                {
                    Result = true,
                    Error = null,
                    Data = b64
                };
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        #endregion
        private static byte[] StreamToBytes(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
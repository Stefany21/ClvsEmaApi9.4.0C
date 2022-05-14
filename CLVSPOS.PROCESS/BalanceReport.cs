using CLVSPOS.DAO;
using CLVSPOS.MODELS;
using CLVSSUPER.MODELS;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;

namespace CLVSPOS.PROCESS
{
    public class BalanceReport
    {
        /// <summary>
        /// Funcion para obtener el id del usuario logueado
        /// </summary>
        /// <returns></returns>
        public static string GetUserId()
        {
            // se obtiene el userId, localizado en los Claims
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            return identity.Claims.Where(c => c.Type == "userId").Single().Value;

        }

        private static string FormatDateForSql(DateTime dateToFormat)
        {
            return string.Format("{0}/{1}/{2} {3}:{4}:{5}", dateToFormat.Month.ToString().PadLeft(2, '0'),
                                                            dateToFormat.Day.ToString().PadLeft(2, '0'),
                                                            dateToFormat.Year.ToString(),
                                                            dateToFormat.Hour.ToString().PadLeft(2, '0'),
                                                            dateToFormat.Minute.ToString().PadLeft(2, '0'),
                                                            dateToFormat.Second.ToString().PadLeft(2, '0'));
        }


        public static string GetBalanceReport(MODELS.GetBalanceModel_UsrOrDate BalanceModel)
        {
            try
            {
                var userId = GetUserId();
                var company = GetData.GetCompanyByUserId(userId);
                byte[] _contentBytes;
                ReportDocument reportDocument = new ReportDocument();


                var appSettings = System.Configuration.ConfigurationManager.AppSettings;

                CLVSPOS.LOGGER.LogManager.LogMessage("Loading Report from " + company.ReportBalance, 1);

                reportDocument.Load(company.ReportBalance);
                reportDocument.SetDatabaseLogon("CrystalSAP", "CrystalR18+");

                CLVSPOS.LOGGER.LogManager.LogMessage("Report Loaded", 1);

                string fechaInicial = FormatDateForSql(BalanceModel.FIni);
                string fechaFinal = FormatDateForSql(BalanceModel.FFin);


                reportDocument.SetParameterValue("Desde", fechaInicial);
                reportDocument.SetParameterValue("Hasta", fechaFinal);
                reportDocument.SetParameterValue("Usuario", BalanceModel.User.Replace("CLAVISCO\\", ""));

                CLVSPOS.LOGGER.LogManager.LogMessage("Parameters/Login Applied", 1);

                _contentBytes = StreamToBytes(reportDocument.ExportToStream(ExportFormatType.PortableDocFormat));

                CLVSPOS.LOGGER.LogManager.LogMessage("Report Exported", 1);

                var b64 = Convert.ToBase64String(_contentBytes);

                return b64;
            }
            catch (Exception exc)
            {
                throw;
            }

        }


        /// <summary>
        /// Convierte el archivo de tipo stream en BIT para pasarlo al frente
        /// El reporrte viene en formato de tipo stream
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
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

        //    public static string GetBalanceReportC(ListMailModel MailDataModel)
        //    {
        //        try
        //        {
        //            var userId = GetUserId();
        //            var company = GetData.GetCompanyByUserId(userId);
        //            byte[] _contentBytes;
        //            ReportDocument reportDocument = new ReportDocument();


        //            var appSettings = System.Configuration.ConfigurationManager.AppSettings;

        //            CLVSPOS.LOGGER.LogManager.LogMessage("Loading Report from " + company.ReportBalance, 1);

        //            reportDocument.Load(company.ReportBalance);
        //            reportDocument.SetDatabaseLogon("CrystalSAPConSP", "CrystalR18+");

        //            CLVSPOS.LOGGER.LogManager.LogMessage("Report Loaded", 1);

        //            string fechaInicial = FormatDateForSql(MailDataModel.BalanceModel.);
        //            string fechaFinal = FormatDateForSql(MailDataModel.BalanceModel.FFin);


        //            reportDocument.SetParameterValue("@FIni", fechaInicial);
        //            reportDocument.SetParameterValue("@FFin", fechaFinal);
        //            reportDocument.SetParameterValue("@SlpCode", MailDataModel.BalanceModel.User);

        //            CLVSPOS.LOGGER.LogManager.LogMessage("Parameters/Login Applied", 1);

        //            _contentBytes = StreamToBytes(reportDocument.ExportToStream(ExportFormatType.PortableDocFormat));

        //            CLVSPOS.LOGGER.LogManager.LogMessage("Report Exported", 1);

        //            var b64 = Convert.ToBase64String(_contentBytes);

        //            return b64;
        //        }
        //        catch (Exception exc)
        //        {
        //            CLVSPOS.LOGGER.LogManager.HandleException(exc, "Loading Report", 1);
        //            throw;
        //        }

        //    }
        //}

        public static string BalanceReport2(PaydeskBalance paydeskBalance, string reporthPath)
        {
            ReportDocument reportDocument = new ReportDocument();
            reportDocument.Load(reporthPath);
            reportDocument.SetDatabaseLogon("CrystalSAP", "CrystalR18+");
            reportDocument.SetParameterValue("Fecha", paydeskBalance.CreationDate);
            reportDocument.SetParameterValue("Cash", paydeskBalance.Cash);
            reportDocument.SetParameterValue("Cards", paydeskBalance.Cards);
            reportDocument.SetParameterValue("CardsPinpad", paydeskBalance.CardsPinpad);
            reportDocument.SetParameterValue("Transfer", paydeskBalance.Transfer);
            reportDocument.SetParameterValue("INTERNAL_K", paydeskBalance.UserSignature);

            byte[] reportAsByteArray = StreamToBytes(reportDocument.ExportToStream(ExportFormatType.PortableDocFormat));

            reportDocument.Close();
            reportDocument.Dispose();

            Convert.ToBase64String(reportAsByteArray);

            return Convert.ToBase64String(reportAsByteArray);
        }
    }
}
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
using CLVSPOS.COMMON;
using CLVSPOS.MODELS;

namespace CLVSPOS.PROCESS
{
    public class ARInvCopyReport
    {

        /// <summary>
        /// Funcion que retrna el id del usuario logueado
        /// </summary>
        /// <returns></returns>
        public static string GetUserId()
        {
            // se obtiene el userId, localizado en los Claims
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            return identity.Claims.Where(c => c.Type == "userId").Single().Value;
        }

        public static ApiResponse<string> GetARInvCopyReport(int DocEntry, int ReportType)
        {
            try
            {
                CLVSPOS.LOGGER.LogManager.LogMessage("Starting GetARInvCopyReport. DocEntry: " + DocEntry.ToString() + " ReportType: " + ReportType.ToString(), 1);

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
                    case (int)Constants.ReportTypes.Quotation:
                        path = company.ReportPathQuotation;
                        reportDocument.Load(path);
                        break;
                    case (int)Constants.ReportTypes.ArInvoice:
                        path = company.ReportPathCopy;
                        reportDocument.Load(path);
                        break;

                }

                CLVSPOS.LOGGER.LogManager.LogMessage("Loaded ARCopy from path: " + path, 1);

               
                reportDocument.SetDatabaseLogon("CrystalSAPConSP", "CrystalR18+");
                

                reportDocument.SetParameterValue("@DocEntry", DocEntry);

                CLVSPOS.LOGGER.LogManager.LogMessage("Loding / Parameter applied ", 1);

                _contentBytes = StreamToBytes(reportDocument.ExportToStream(ExportFormatType.PortableDocFormat));
            

                var b64 = Convert.ToBase64String(_contentBytes);

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
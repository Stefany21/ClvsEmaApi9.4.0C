using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CLVSSUPER.DAO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using CLVSSUPER.COMMON;

namespace CLVSSUPER.PROCESS
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

        public static string GetARInvCopyReport(int DocEntry, int ReportType)
        {
            var userId = GetUserId();
            var company = GetData.GetCompanyByUserId(userId);
            string path = "";
            byte[] _contentBytes;
            ReportDocument reportDocument = new ReportDocument();
            switch (ReportType)
            {
                case (int)Constants.ReportTypes.SaleOrder:
                    path = company.ReportPath;
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
            reportDocument.SetParameterValue("@DocEntry", DocEntry);
            _contentBytes = StreamToBytes(reportDocument.ExportToStream(ExportFormatType.PortableDocFormat));
            var b64 = Convert.ToBase64String(_contentBytes);
            return b64;
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
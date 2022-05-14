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
using System.Web.Mvc;
using CLVSSUPER.COMMON;

namespace CLVSSUPER.PROCESS
{
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

        //private readonly byte[] _contentBytes;

        public static string PrintReport(int DocEntry, int ReportType)
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
                

            }
            
            reportDocument.SetParameterValue("@DocEntry", DocEntry);
            _contentBytes = StreamToBytes(reportDocument.ExportToStream(ExportFormatType.PortableDocFormat));
            var b64 = Convert.ToBase64String(_contentBytes);
            return b64;
        }

        //public override void ExecuteResult(ControllerContext context)
        //{

        //    var response = context.HttpContext.ApplicationInstance.Response;
        //    response.Clear();
        //    response.Buffer = false;
        //    response.ClearContent();
        //    response.ClearHeaders();
        //    response.Cache.SetCacheability(HttpCacheability.Public);
        //    response.ContentType = "application/pdf";

        //    using (var stream = new MemoryStream(_contentBytes))
        //    {
        //        stream.WriteTo(response.OutputStream);
        //        stream.Flush();
        //    }
        //}

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
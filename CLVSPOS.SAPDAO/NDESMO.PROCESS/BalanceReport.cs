using CLVSSUPER.DAO;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;

namespace CLVSSUPER.PROCESS
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


        public static string GetBalanceReport()
        {
            var userId = GetUserId();
            var company = GetData.GetCompanyByUserId(userId);
            byte[] _contentBytes;
            ReportDocument reportDocument = new ReportDocument();
            reportDocument.Load(System.Configuration.ConfigurationManager.AppSettings["ReportPath"].ToString());
            //reportDocument.Load(company.ReportPathInventory);
            //reportDocument.SetParameterValue("@Articulo", string.IsNullOrEmpty(Articulo) ? "" : Articulo);
            _contentBytes = StreamToBytes(reportDocument.ExportToStream(ExportFormatType.PortableDocFormat));
            var b64 = Convert.ToBase64String(_contentBytes);
            return b64;
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
    }
}
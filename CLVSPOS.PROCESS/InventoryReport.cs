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
using CLVSPOS.MODELS;

namespace CLVSPOS.PROCESS
{
    public class InventoryReport
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

        /// <summary>
        /// Funcion que se encarga de llamar al reporte y enviar los parametros con los que el reporte 
        /// va a solicitar al SP en SAP que le retorne la informacion
        /// Recibe como parametros el nombre del articulo, la marca, el grupo y el subgrupo
        /// estos pueden ir vacios
        /// </summary>
        /// <param name="Articulo"></param>
        /// <param name="Marca"></param>
        /// <param name="Grupo"></param>
        /// <param name="subGrupo"></param>
        /// <returns></returns>
        public static ApiResponse<string> InventoryReports(string Articulo, string Marca, string Grupo, string subGrupo)
        {
            var userId = GetUserId();
            var company = GetData.GetCompanyByUserId(userId);
            byte[] _contentBytes;
            ReportDocument reportDocument = new ReportDocument();
            //reportDocument.Load(System.Configuration.ConfigurationManager.AppSettings["ReportPathInventory"].ToString());
            reportDocument.Load(company.ReportPathInventory);


            ApplyCRLogin obj_apply_login = new ApplyCRLogin();
            obj_apply_login.apply_info(ref reportDocument, company.SAPConnection.Server, company.DBCode);


            reportDocument.SetParameterValue("@Articulo", string.IsNullOrEmpty(Articulo) ? "" : Articulo);
            reportDocument.SetParameterValue("@Marca", string.IsNullOrEmpty(Marca) ? "" : Marca);
            reportDocument.SetParameterValue("@Grupo", string.IsNullOrEmpty(Grupo) ? "" : Grupo);
            reportDocument.SetParameterValue("@subGrupo", string.IsNullOrEmpty(subGrupo) ? "" : subGrupo);
            _contentBytes = StreamToBytes(reportDocument.ExportToStream(ExportFormatType.PortableDocFormat));
            
            string b64 = Convert.ToBase64String(_contentBytes);

            obj_apply_login = null;
            return new ApiResponse<string>
            {
                Result = true,
                Error = null,
                Data = b64
            };
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
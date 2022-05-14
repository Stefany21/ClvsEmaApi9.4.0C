using CLVSPOS.COMMON;
using CLVSPOS.DAO;
using CLVSPOS.LOGGER;
using CLVSSUPER.MODELS;
using SAPbobsCOM;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;

namespace CLVSPOS.DIAPI
{
    public class DIAPICommon
    {
        public static string GetUserId()
        {
            try
            {
                // se obtiene el userId, localizado en los Claims
                var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
                return identity.Claims.Where(c => c.Type == "userId").Single().Value;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }





        //---------------------------- CreateCompany with credential holder

        public static Company CreateCompanyObject(CredentialHolder _UserCredentials)
        {
            var startTime = DateTime.Now;
            LogManager.LogMessage(string.Format("                           PostSAPData>CreateCompanyObject. Start Time: {0}", startTime), 2);

            Company oCompany = null;

            oCompany = new Company
            {
                Server = _UserCredentials.Server,
                UserName = _UserCredentials.SAPUser,
                Password = _UserCredentials.SAPPass,
                CompanyDB = _UserCredentials.DBCode,
                DbServerType = getDST(_UserCredentials.DST)
            };


            var result = oCompany.Connect();
            int lErrCode;
            string temp_string;
            oCompany.GetLastError(out  lErrCode, out  temp_string);

            if (lErrCode == 0)
            {

                LogManager.LogMessage(string.Format("                           PostSAPData>CreateCompanyObject. End Time: {0}| Total Time Time: {1}", DateTime.Now, DateTime.Now - startTime), 2);

                return oCompany;
            }
            else
            {
                string sapErrMessage = "Ocurrio un error cuando intentaba establecer la conexion con SAP," + lErrCode + temp_string;
                throw new Exception(sapErrMessage);
            }
        }






        //-----------------------------------------------------------------------




















        /// <summary>
        /// Crea una conexion con el dll de sap para abrir la operacion que se requiera en el posdiapi
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        public static Company CreateCompanyObject(Companys company)
        {
            var startTime = DateTime.Now;
            LogManager.LogMessage(string.Format("                           PostSAPData>CreateCompanyObject. Start Time: {0}", startTime), 2);

            var userId = GetUserId();
            var userAsing = DAO.GetData.GetUserMappId(userId);
            Company oCompany = null;
            oCompany = new Company();
            oCompany.Server = company.SAPConnection.Server;
            if (!string.IsNullOrEmpty(company.SAPConnection.LicenseServer))
            {
                oCompany.LicenseServer = company.SAPConnection.LicenseServer;
            }
            if (!string.IsNullOrEmpty(company.SAPConnection.BoSuppLangs))
            {
                oCompany.language = getBoSuppLangs(company.SAPConnection.BoSuppLangs);
            }
            oCompany.UserName = userAsing.SAPUser;
            oCompany.Password = userAsing.SAPPass;
            oCompany.CompanyDB = company.DBCode;
            oCompany.DbServerType = getDST(company.SAPConnection.DST);
            oCompany.UseTrusted = company.SAPConnection.UseTrusted;
            if (!company.SAPConnection.UseTrusted)
            {
                oCompany.DbUserName = company.SAPUser;
                oCompany.DbPassword = company.SAPPass;
            }
            int lErrCode;
            string temp_string;
            var result = oCompany.Connect();
            oCompany.GetLastError(out lErrCode, out temp_string);

            if (lErrCode == 0)
            {

                LogManager.LogMessage(string.Format("                           PostSAPData>CreateCompanyObject. End Time: {0}| Total Time Time: {1}", DateTime.Now, DateTime.Now - startTime), 2);

                return oCompany;
            }
            else
            {
                string sapErrMessage = "Ocurrio un error cuando intentaba establecer la conexion con SAP," + lErrCode + temp_string;
                throw new Exception(sapErrMessage);
            }
        }

        /// <summary>
        /// Crea una conexion con el dll de sap para abrir la operacion que se requiera en el posdiapi
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        public static Company CreateCompanyObject(Companys company, string userId)
        {
            var startTime = DateTime.Now;
            LogManager.LogMessage(string.Format("                           PostSAPData>CreateCompanyObject. Start Time: {0}", startTime), 2);

            //var userId = GetUserId();
            var userAsing = DAO.GetData.GetUserMappId(userId);

            Company oCompany = null;
            oCompany = new Company();

            oCompany.Server = company.SAPConnection.Server;

            if (!string.IsNullOrEmpty(company.SAPConnection.LicenseServer))
            {
                oCompany.LicenseServer = company.SAPConnection.LicenseServer;
            }
            if (!string.IsNullOrEmpty(company.SAPConnection.BoSuppLangs))
            {
                oCompany.language = getBoSuppLangs(company.SAPConnection.BoSuppLangs);
            }

            oCompany.UserName = userAsing.SAPUser;
            oCompany.Password = userAsing.SAPPass;
            oCompany.CompanyDB = company.DBCode;
            oCompany.DbServerType = getDST(company.SAPConnection.DST);
            oCompany.UseTrusted = company.SAPConnection.UseTrusted;

            if (!company.SAPConnection.UseTrusted)
            {
                oCompany.DbUserName = company.SAPUser;
                oCompany.DbPassword = company.SAPPass;
            }

            int lErrCode;
            string temp_string;
            var result = oCompany.Connect();
            oCompany.GetLastError(out lErrCode, out temp_string);

            if (lErrCode == 0)
            {

                LogManager.LogMessage(string.Format("                           PostSAPData>CreateCompanyObject. End Time: {0}| Total Time Time: {1}", DateTime.Now, DateTime.Now - startTime), 2);

                return oCompany;
            }
            else
            {
                string sapErrMessage = "Ocurrio un error cuando intentaba establecer la conexion con SAP," + lErrCode + temp_string;
                throw new Exception(sapErrMessage);
            }
        }

        private static BoSuppLangs getBoSuppLangs(string Lang)
        {
            switch (Lang)
            {
                case "ln_English":
                    return BoSuppLangs.ln_English;
                case "ln_Spanish":
                    return BoSuppLangs.ln_Spanish;
                default:
                    return BoSuppLangs.ln_Spanish_La;
            }
        }
        private static BoDataServerTypes getDST(string DST)
        {
            switch (DST)
            {
                case "dst_MSSQL2014":
                    return BoDataServerTypes.dst_MSSQL2014;
                case "dst_MSSQL2012":
                    return BoDataServerTypes.dst_MSSQL2012;
                case "dst_MSSQL2008":
                    return BoDataServerTypes.dst_MSSQL2008;
                case "dst_HANADB":
                    return BoDataServerTypes.dst_HANADB;
                case "dst_MSSQL2016":
                    return BoDataServerTypes.dst_MSSQL2016;
                default:
                    return BoDataServerTypes.dst_MSSQL2008;
            }
        }

        //Metodo para mantener conexiones con la compania y evitar que el SDK se inactive
        //lo que debe mejorar el tiempo de conexion
        //no es la solucion definitiva es solo para probar
        public static void ConnectCompany(UserAssign UserAssign)
        {
            var startTime = DateTime.Now;
            try
            {
                LogManager.LogMessage(string.Format("                           DIAPICommon>ConnectCompany. Start Time: {0}", startTime), (int)Constants.LogTypes.CompanyActive);

                Company oCompany = null;
                oCompany = new Company();

                oCompany.Server = UserAssign.Companys.SAPConnection.Server;
                oCompany.UserName = UserAssign.Companys.SAPUser;
                oCompany.Password = UserAssign.Companys.SAPPass;
                oCompany.CompanyDB = UserAssign.Companys.DBCode;
                oCompany.DbServerType = getDST(UserAssign.Companys.SAPConnection.DST);

                int lErrCode;
                string temp_string;
                var result = oCompany.Connect();
                oCompany.GetLastError(out lErrCode, out temp_string);

                if (lErrCode == 0)
                {

                    LogManager.LogMessage(string.Format("                           DIAPICommon>ConnectCompany successfull. End Time: {0}| Total Time Time: {1}", DateTime.Now, DateTime.Now - startTime), (int)Constants.LogTypes.CompanyActive);
                    oCompany.Disconnect();
                    oCompany = null;
                }
                else
                {

                    string sapErrMessage = "Ocurrio un error cuando intentaba establecer la conexion con SAP," + lErrCode + temp_string;
                    LogManager.LogMessage(string.Format("                           DIAPICommon>ConnectCompany error:{0},. End Time: {1}| Total Time Time: {2}", sapErrMessage, DateTime.Now, DateTime.Now - startTime), (int)Constants.LogTypes.CompanyActive);
                    oCompany = null;
                }
            }
            catch (Exception ex)
            {
                string ErrMessage = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;
                LogManager.LogMessage(string.Format("                           DIAPICommon>ConnectCompany error:{0},. End Time: {1}| Total Time Time: {2}", ErrMessage, DateTime.Now, DateTime.Now - startTime), (int)Constants.LogTypes.CompanyActive);

            }
        }
    }
}
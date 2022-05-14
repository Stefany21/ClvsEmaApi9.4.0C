
using CLVSSUPER.MODELS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace CLVSSUPER.COMMON
{
    public class Common
    {
        private const string cryptoKey = "clavisconsultoresfe";
        private static MD5 md5 = new MD5CryptoServiceProvider();

        //ORIGINAL NAME: replaceConectODBC
        //ReplaceConnectorODBC
        public static string ReplaceConnectorODBC(CompanysSAPModel company)
        {
            string connection = string.Empty;
            string odbctype = string.Empty;
                        
            string server = company.Server;
            string user = company.User;
            string pass = company.Pass;

            odbctype = company.odbctype;

            connection = Constants.ConnectionString[company.ServerType.ToUpper()];

            connection = connection.Replace("#Server#", server)
                                   .Replace("#ODBCType#", odbctype);

            if (connection != Constants.ConnectionString[Constants.SQLSERVER_TRUSTED]) {
                connection = connection.Replace("#UserId#", user)
                                       .Replace("#Password#", pass);
            }

            

            //switch (company.ServerType.ToUpper())
            //{
            //    case "SQLSERVERT":
            //        Connection = Constants.SQLTODBCConFormat.Replace("#ODBCType#", odbctype).Replace("#Server#", Server);
            //        break;
            //    case "SQLSERVER":
            //        Connection = Constants.SQLODBCConFormat.Replace("#ODBCType#", odbctype).Replace("#Server#", Server).Replace("#UserId#", User).Replace("#Password#", Pass);
            //        break;
            //    case "HANASERVERT":
            //        Connection = Constants.HANATODBCConFormat.Replace("#ODBCType#", odbctype).Replace("#Server#", Server).Replace("#UserId#", User).Replace("#Password#", Pass);
            //        break;
            //    case "HANASERVER":
            //        Connection = Constants.HANAODBCConFormat.Replace("#ODBCType#", odbctype).Replace("#Server#", Server).Replace("#UserId#", User).Replace("#Password#", Pass);
            //        break;
            //}
            return connection;
        }

        /// <summary>
        /// metodo para encriptar una clave
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Encrypt(string s)
        {
            byte[] bytes = Encoding.ASCII.GetBytes("qualityi");
            if (s == null || s.Length == 0) return string.Empty;
            string result = string.Empty;
            try
            {
                byte[] buffer = Encoding.ASCII.GetBytes(s);
                TripleDESCryptoServiceProvider des =
                    new TripleDESCryptoServiceProvider();
                des.Key = md5.ComputeHash(Encoding.Unicode.GetBytes(cryptoKey));
                des.IV = bytes;
                result = Convert.ToBase64String(
                    des.CreateEncryptor().TransformFinalBlock(
                        buffer, 0, buffer.Length));
            }
            catch
            {
                throw;
            }
            return result;
        }
    }
}
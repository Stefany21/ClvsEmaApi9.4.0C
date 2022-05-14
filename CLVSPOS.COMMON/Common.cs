
using CLVSPOS.MODELS;
using CLVSSUPER.MODELS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;

namespace CLVSPOS.COMMON
{
    public class Common
    {
        private const string cryptoKey = "clavisconsultoresfe";

        private static MD5 md5 = new MD5CryptoServiceProvider();     

        //ORIGINAL NAME: replaceConectODBC
        //ReplaceConnectorODBC
        public static string ReplaceConnectorODBC(string dbn)
        {
            string connection = string.Empty;
            string odbctype = GetDBObjectByKey(System.Reflection.MethodBase.GetCurrentMethod(), "ODBCType");
            string server = GetDBObjectByKey(System.Reflection.MethodBase.GetCurrentMethod(), "Server");
            string user = GetDBObjectByKey(System.Reflection.MethodBase.GetCurrentMethod(), "UserId");
            string pass = GetDBObjectByKey(System.Reflection.MethodBase.GetCurrentMethod(), "Password");
            string companyName = GetDBObjectByKey(System.Reflection.MethodBase.GetCurrentMethod(), "CompanyName");
            string ServerType = GetDBObjectByKey(System.Reflection.MethodBase.GetCurrentMethod(), "ServerType");
            //string server = company.Server;
            //string user = company.User;
            //string pass = company.Pass;
            //odbctype = company.odbctype;


            //connection = Constants.ConnectionString[GetServerType().ToUpper()] + $"Application Name={companyName};";

            //connection = connection.Replace("#Server#", server).Replace("#ODBCType#", odbctype);

            //if (connection != Constants.ConnectionString[Constants.SQLSERVER_TRUSTED]) {
            //    connection = connection.Replace("#UserId#", user).Replace("#Password#", pass);
            //}

            // connection = connection.Replace("#companyName#", companyName);

            switch (ServerType.ToUpper())   
            {
                case "SQLSERVERT":  
                    connection = Constants.ConnectionString[ServerType.ToUpper()].Replace("#Server#", server).Replace("#ODBCType#", odbctype); // ServerType.ToUpper().Replace("#Server#", server).Replace("#ODBCType#", odbctype);
                    break;
                case "SQLSERVER":
                    connection = Constants.ConnectionString[ServerType.ToUpper()].Replace("#ODBCType#", odbctype).Replace("#Server#", server).Replace("#UserId#", user).Replace("#Password#", pass);
                    break;
                case "HANASERVERT":
                    connection = Constants.ConnectionString[ServerType.ToUpper()].Replace("#ODBCType#", odbctype).Replace("#Server#", server).Replace("#UserId#", user).Replace("#Password#", pass);
                    break;
                case "HANASERVER":
                    connection = Constants.ConnectionString[ServerType.ToUpper()].Replace("#ODBCType#", odbctype).Replace("#Server#", server).Replace("#UserId#", user).Replace("#Password#", pass);
                    break;
            }
            connection += $"Application Name={companyName};";
            return connection;
        }

        /// <summary>
        /// Este metodo es usado para setear objetos dinamicamente
        /// </summary>
        /// <param name="_invoker">El metodo que invoca esta funcion</param>
        /// <param name="_object">El objeto que va a actualizar</param>
        /// <param name="_oDataRow">Los registros que seran leidos</param>
        /// <param name="_columns">Todas las columnas que devuelve la consulta</param>
        public static T FillObject<T>(string _invoker, DataRow _oDataRow, DataColumnCollection _columns) where T : new()
        {
            try
            {
                T _object = new T();
                foreach (PropertyInfo prop in _object.GetType().GetProperties())
                {
                    try
                    {
                        Type type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                        if (_columns.Contains(prop.Name))
                        {
                            if (!_oDataRow.IsNull(prop.Name) && !System.DBNull.Value.Equals(_oDataRow[prop.Name]) && _oDataRow[prop.Name] != null)
                            {
                                prop.SetValue(_object, _oDataRow[prop.Name]);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Invoker={_invoker}* Checkout <{prop.Name}> ? <{_oDataRow[prop.Name]}> {ex.Message}");
                    }
                }
                return _object;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Usado para obtener el resultado de varias consultas
        /// </summary>
        /// <param name="_connection">Datos de la conexion</param>
        /// <param name="_query">Query que va a ser ejecutado</param>
        /// <returns>DataTable con los resultados del query</returns>
        public static DataTable GetDataTable(DbConnection _connection, string _query)
        {
            DataTable oDataTable = null;
            DbCommand ODbCommand = null;
            DbDataAdapter oDbDataAdapter = null;

            try
            {
                oDataTable = new DataTable();
                oDataTable.Clear();

                ODbCommand = _connection.CreateCommand();

                oDbDataAdapter = new SqlDataAdapter();


                ODbCommand.CommandText = _query;
                ODbCommand.Connection = _connection;

                oDbDataAdapter.SelectCommand = ODbCommand;
                oDbDataAdapter.Fill(oDataTable);

                return oDataTable;
            }
            catch
            {
                throw;
            }
            finally
            {
                ODbCommand?.Dispose();
                oDbDataAdapter?.Dispose();
            }
        }



        public static DataTable ExecuteStoredProcedure(string connectionString, string sql, List<SqlParameter> pars)
        {
            try
            {
				DataTable dt = new DataTable();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (pars.Count > 0)
                        {
                            foreach (SqlParameter p in pars)
                            {
                                cmd.Parameters.Add(p);
                            }
                        }

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            dt.Clear();
                            da.SelectCommand.CommandTimeout = 0;
                            da.Fill(dt);
                        }
                    }

                    conn.Close();
                }

                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }




        public static DataTable QueryToTable(string connectionString, string _query)
        {
            try
            {

             

                DataSet oDataSet = new DataSet();

                using (OdbcConnection conn = new OdbcConnection(connectionString))
                {
                    OdbcDataAdapter da = new OdbcDataAdapter(_query, conn);



                    conn.Open();
                    da.Fill(oDataSet);
                    conn.Close();
                }

                if (oDataSet.Tables.Count == 0) throw new Exception("No table returned from query");

                return oDataSet.Tables[0];
            }
            catch
            {
                throw;
            }
        }


        public static string ReplaceConectODBC(CredentialHolder credentialHolder)
        {
            string companyName = GetDBObjectByKey(MethodBase.GetCurrentMethod(), "CompanyName");
            //ODBC
            string Connection = string.Empty;
            string odbctype = credentialHolder.ODBCType;
            string Server = credentialHolder.Server;
            string User = credentialHolder.DBUser;
            string Pass = credentialHolder.DBPass;
            switch (credentialHolder.ServerType.ToUpper())
            {
                case "SQLSERVERT":
                    Connection = Constants.SQLTODBCConFormat.Replace("#ODBCType#", odbctype).Replace("#Server#", Server);
                    break;
                case "SQLSERVER":
                    Connection = Constants.SQLODBCConFormat.Replace("#ODBCType#", odbctype).Replace("#Server#", Server).Replace("#UserId#", User).Replace("#Password#", Pass);
                    break;
                case "HANASERVERT":
                    Connection = Constants.HANATODBCConFormat.Replace("#ODBCType#", odbctype).Replace("#Server#", Server).Replace("#UserId#", User).Replace("#Password#", Pass);
                    break;
                case "HANASERVER":
                    Connection = Constants.HANAODBCConFormat.Replace("#ODBCType#", odbctype).Replace("#Server#", Server).Replace("#UserId#", User).Replace("#Password#", Pass);
                    break;
            }
            Connection += $"Application Name={companyName};";
            return Connection;
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


        public static void SetObjectProperty<T>(string propertyName, string value, ref T obj)
        {
            PropertyInfo propertyInfo = obj.GetType().GetProperty(propertyName);
            // make sure object has the property we are after
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(obj, value, null);
            }
        }



        public static string GetKeyValue(string _keyName)
        {
            try
            {
                return System.Configuration.ConfigurationManager.AppSettings["InvoiceType"].ToString();
            }
            catch
            {
                throw new Exception($"La llave {_keyName} no existe en webconfig");
            }
        }

        /// <summary>
        ///  Devuelve el nombre del objeto de la bd local
        /// </summary>
        /// <param name="_invoker"></param>
        /// <param name="_objectKey"></param>
        /// <returns></returns>
        public static string GetDBObjectByKey(MethodBase _invoker, string _objectKey)
        {
            try
            {

                string OBJECT_NAME = ConfigurationManager.AppSettings[_objectKey];


                if (String.IsNullOrEmpty(OBJECT_NAME))
                {
                    string invokerPath = $"{_invoker.DeclaringType}.{_invoker.Name}";
                    throw new Exception($"Invoker={invokerPath}* No ha definido {_objectKey} en el webconfig, agreguelo por favor");
                }
                return OBJECT_NAME;
            }
            catch
            {
                throw;
            }
        }

    }
}
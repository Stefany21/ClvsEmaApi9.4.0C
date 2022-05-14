using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using CLVSPOS.MODELS;
using CLVSPOS.LOGGER;
using CLVSPOS.DAO;
using System.Linq;
using CLVSSUPER.MODELS;
using System.Data.SqlClient;
using CLVSPOS.COMMON;
using System.Reflection;

namespace CLVSPOS.SAPDAO
{
    public class GetSapData
    {
        public static string QueryType = System.Configuration.ConfigurationManager.AppSettings["Database"].ToUpper();
        // public static string QueryType = Common.GetDBObjectByKey(System.Reflection.MethodBase.GetCurrentMethod(), "Database");// System.Configuration.ConfigurationManager.AppSettings["Database"].ToUpper();
        private static DataTable ExecuteQuery(string connectionString, string sql)
        {
            try
            {
                DataTable dt = new DataTable();

                using (OdbcConnection conn = new OdbcConnection(connectionString))
                {

                    using (OdbcCommand cmd = new OdbcCommand(sql, conn))
                    {
                        using (OdbcDataAdapter da = new OdbcDataAdapter(cmd))
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
        /// <summary>
        /// Metodo para retornar difertes tablas desde un sp
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        private static DataSet ExecuteQueryGetMultipleTables(string connectionString, string sql)
        {
            try
            {
                //    int size = 0;
                //    int limit = 0;

                //    bool locked = false;

                //    foreach (char c in connectionString)
                //    {
                //        size++;
                //        if (!locked)
                //        {
                //            limit++;
                //        }
                //        if (c == ';')
                //        {
                //            locked = true;
                //        }

                //    }


                //    connectionString = connectionString.Substring(limit, size - limit);

                DataSet oDataSet = new DataSet();

                //using (SqlConnection con = new SqlConnection(connectionString))
                //{
                //    using (SqlCommand cmd = new SqlCommand(sql))
                //    {
                //        using (SqlDataAdapter sda = new SqlDataAdapter())
                //        {
                //            cmd.Connection = con;
                //            sda.SelectCommand = cmd;
                //            using (DataSet ds = new DataSet())
                //            {
                //                sda.Fill(ds);
                //                oDataSet = ds;
                //            }
                //        }
                //    }
                //}
                using (OdbcConnection conn = new OdbcConnection(connectionString))
                {
                    OdbcDataAdapter da = new OdbcDataAdapter(sql, conn);

                    conn.Open();
                    da.Fill(oDataSet);
                    conn.Close();
                }

                return oDataSet;
            }
            catch (Exception e)
            {
                string message = e.Message;
                throw;
            }
        }

        public static bool IsLocalInvoiceIdValid(CredentialHolder _userCredentials, string _idInvoice)
        {
            try
            {
                string sql = string.Format("SELECT 1 FROM {0}.dbo.OINV (NOLOCK) WHERE ISNULL(U_CLVS_POS_UniqueInvId,'') = '{1}'", _userCredentials.DBCode, _idInvoice);


                // string SqlQuery = QueryType == "SQL" ? $"SELECT 1 FROM {company.DBCode}.dbo.OINV (NOLOCK) WHERE ISNULL(U_CLVS_POS_UniqueInvId,'') = '{_idInvoice}'" : $"SELECT 1 FROM {company.DBCode}.OINV (NOLOCK) WHERE ISNULL(U_CLVS_PORS_UniqueInvId,'')='{_idInvoice}'";



                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, sql);

                return dt.Rows.Count == 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        ///  Realiza una conexion Hana/SQL para traer la lista de todos los socios de negocios 
        /// </summary>
        /// <param name="company"></param>
        /// <param name="_dbObjectViewGetBPCustomers"></param>
        /// <returns></returns>
        public static BPSResponseModel GetBusinessPartners(CredentialHolder _UserCredentials, string _dbObjectViewGetBPCustomers)
        {
            try
            {
                string query = QueryType == "SQL" ? string.Format("SELECT * FROM {0}.dbo.{1}", _UserCredentials.DBCode, _dbObjectViewGetBPCustomers) : String.Format("SELECT * FROM {0}.{1}", _UserCredentials.DBCode, _dbObjectViewGetBPCustomers);


                BPSResponseModel businessPartner = new BPSResponseModel
                {
                    Result = false
                };

                List<BusinessPartnerModel> bpList = new List<BusinessPartnerModel>();


                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_UserCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);




                if (dt.Rows != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        BusinessPartnerModel bp = new BusinessPartnerModel
                        {
                            CardCode = Convert.ToString(row["CardCode"]),
                            CardName = Convert.ToString(row["CardName"]),
                            Address = Convert.ToString(row["Address"]),
                            Phone1 = Convert.ToString(row["Phone1"]),
                            Balance = Convert.ToString(row["Balance"]),
                            GroupNum = Convert.ToString(row["GroupNum"]),
                            Discount = Convert.ToString(row["Discount"]),
                            ListNum = Convert.ToString(row["ListNum"]),
                            Currency = Convert.ToString(row["Currency"]),
                            E_mail = Convert.ToString(row["E_mail"]),
                            QryGroup1 = Convert.ToString(row["QryGroup1"]),
                            IdType = string.IsNullOrEmpty(Convert.ToString(row["U_TipoIdentificacion"])) ? "00" : Convert.ToString(row["U_TipoIdentificacion"]),
                            Provincia = Convert.ToString(row["U_provincia"]),
                            Canton = Convert.ToString(row["U_canton"]),
                            Distrito = Convert.ToString(row["U_distrito"]),
                            Barrio = Convert.ToString(row["U_barrio"]),
                            Direccion = Convert.ToString(row["U_direccion"]),
                            Cedula = Convert.ToString(row["Cedula"]),
                            ClienteContado = Convert.ToBoolean(row["ClienteContado"]),
                            

                        };
                        bpList.Add(bp);
                    }
                }
                else
                {
                    businessPartner.Result = false;
                    businessPartner.Error = new ErrorInfo
                    {
                        Message = "No se encontraron Clientes..."
                    };
                }
                
                businessPartner.Result = true;
                businessPartner.BPS = bpList;

                return businessPartner;
            }
            catch (Exception exc)
            {
                return (BPSResponseModel)LogManager.HandleExceptionWithReturn(exc, "BPSResponseModel");
            }
        }

        /// <summary>
        ///Traer la lista de todos los socios de negocios 
        /// </summary>
        /// <param name="company"></param>
        /// <param name="_dbObjectViewGetSuppliers"></param>
        /// <returns></returns>
        public static BPSResponseModel GetSuppliers(CredentialHolder _UserCredentials, string _dbObjectViewGetSuppliers)
        {
            try
            {

                string query = QueryType == "SQL" ? string.Format("SELECT * FROM {0}.dbo.{1}", _UserCredentials.DBCode, _dbObjectViewGetSuppliers) : String.Format("SELECT * FROM {0}.{1}", _UserCredentials.DBCode, _dbObjectViewGetSuppliers);


                BPSResponseModel businessPartner = new BPSResponseModel
                {
                    Result = false
                };

                List<BusinessPartnerModel> bpList = new List<BusinessPartnerModel>();




                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_UserCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);


                if (dt.Rows != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        BusinessPartnerModel bp = new BusinessPartnerModel
                        {
                            CardCode = Convert.ToString(row["CardCode"]),
                            CardName = Convert.ToString(row["CardName"]),
                            Currency = Convert.ToString(row["Currency"]),
                            Address = Convert.ToString(row["Address"]),
                            Phone1 = Convert.ToString(row["Phone1"]),
                            Balance = Convert.ToString(row["Balance"]),
                            Discount = Convert.ToString(row["Discount"]),
                            ListNum = Convert.ToString(row["ListNum"]),
                            E_mail = Convert.ToString(row["E_mail"]),
                            Cedula = Convert.ToString(row["Cedula"]),
                            GroupNum = Convert.ToString(row["GroupNum"])

                        };
                        bpList.Add(bp);
                    }
                }
                else
                {
                    businessPartner.Result = false;
                    businessPartner.Error = new ErrorInfo
                    {
                        Message = "No se encontraron Clientes..."
                    };
                }

                businessPartner.Result = true;
                businessPartner.BPS = bpList;

                return businessPartner;
            }
            catch (Exception exc)
            {
                return (BPSResponseModel)LogManager.HandleExceptionWithReturn(exc, "BPSResponseModel");
            }
        }

        public static BPFEInfoResponseModel GetBusinessPartnerFEInfo(CredentialHolder _UserCredentials, string cardCode, string _dbObjectViewGetBPCustomers)
        {
            try
            {
                string sql = string.Format("SELECT ISNULL(U_TipoIdentificacion,'') U_TipoIdentificacion, ISNULL(Cedula,'') Cedula, ISNULL(E_Mail,'') E_Mail" + Environment.NewLine +
                                           //"       ISNULL(U_provincia,'') U_provincia, ISNULL(U_canton,'') U_canton, ISNULL(U_distrito,'') U_distrito, " + Environment.NewLine +
                                           //"       ISNULL(U_barrio,'') U_barrio, ISNULL(U_direccion,'') U_direccion" + Environment.NewLine +                                        
                                           "FROM {0}.dbo.{1}" + Environment.NewLine +
                                           "WHERE  RTRIM(LTRIM(CardCode)) = '{2}'", _UserCredentials.DBCode, _dbObjectViewGetBPCustomers, cardCode);

                BPFEInfoResponseModel feInfo = new BPFEInfoResponseModel
                {
                    Result = true
                };

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_UserCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, sql);

                if (dt.Rows != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    InvoiceFEInfo bpFEInfo = new InvoiceFEInfo
                    {
                        IdType = string.IsNullOrEmpty(Convert.ToString(row["U_TipoIdentificacion"])) ? "00" : Convert.ToString(row["U_TipoIdentificacion"]),
                        Identification = Convert.ToString(row["Cedula"]),
                        //Provincia = Convert.ToString(row["U_provincia"]),
                        //Canton = Convert.ToString(row["U_canton"]),
                        //Distrito = Convert.ToString(row["U_distrito"]),
                        //Barrio = Convert.ToString(row["U_barrio"]),
                        //Direccion = Convert.ToString(row["U_direccion"]),                        
                        Email = Convert.ToString(row["E_Mail"])
                    };

                    feInfo.FEInfo = bpFEInfo;
                }
                else
                {
                    feInfo.Result = false;
                    feInfo.Error = new ErrorInfo
                    {
                        Message = "No se encontró Cliente..."
                    };
                }

                return feInfo;
            }
            catch (Exception exc)
            {
                return (BPFEInfoResponseModel)LogManager.HandleExceptionWithReturn(exc, "BPFEInfoResponseModel");
            }
        }

        //#001 07/09/2021
        public static ApiResponse<double> GetItemAVGPrice(string itemCode, CredentialHolder _userCredentials, string _dbObjectSpGetItemAvgPrice)
        {
            try
            {
                string query = QueryType == "SQL" ? string.Format("EXEC {0}.dbo." + _dbObjectSpGetItemAvgPrice + " {1}", _userCredentials.DBCode, itemCode) : $"CAL {_userCredentials.DBCode}.CLVS_EMA)SLT_AVGPRICE('{itemCode}')";
                double avgPrice;

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                if (dt.Rows != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    avgPrice = Convert.ToDouble(row["AvgPrice"]);
                }
                else
                {
                    throw new Exception("No se pudo obtener AVGPrice del item: " + itemCode);
                }



                return new ApiResponse<double>()
                {
                    Result = true,
                    Error = null,
                    Data = avgPrice
                };
            }
            catch
            {
                throw;
            }
        }


        public static ApiResponse<double> GetItemLastPurchagePrice(string itemCode, CredentialHolder _userCredentials, string _dbObjectSpGetItemLastPrice)
        {
            try
            {
                string query = QueryType == "SQL" ?
                    string.Format("EXEC {0}.dbo." + _dbObjectSpGetItemLastPrice + " {1}", _userCredentials.DBCode, itemCode) : $"CALL {_userCredentials.DBCode}.{_dbObjectSpGetItemLastPrice}('{itemCode}')";

                double lastPrice;


                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                if (dt.Rows != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    lastPrice = Convert.ToDouble(row["LastPrice"]);
                }
                else
                {
                    throw new Exception("No se pudo obtener ultimo precio del item: " + itemCode);
                }



                return new ApiResponse<double>()
                {
                    Result = true,
                    Error = null,
                    Data = lastPrice
                };

            }
            catch
            {
                throw;
            }
        }



        public static BPFEInfoResponseModel GetBusinessPartnerFEInfo(CredentialHolder _userCredentials, string idType, string idNumber, string _dbObjectSpGetFeInfo)
        {
            try
            {
                string sql = string.Format("EXEC {0}.dbo.{1} '{2}','{3}'", _userCredentials.DBCode, _dbObjectSpGetFeInfo, idType, idNumber);

                BPFEInfoResponseModel feInfo = new BPFEInfoResponseModel
                {
                    Result = true
                };

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, sql);

                if (dt.Rows != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    InvoiceFEInfo bpFEInfo = new InvoiceFEInfo
                    {
                        CardName = Convert.ToString(row["CardName"]),
                        IdType = string.IsNullOrEmpty(Convert.ToString(row["U_TipoIdentificacion"])) ? "00" : Convert.ToString(row["U_TipoIdentificacion"]),
                        Identification = Convert.ToString(row["U_NumIdentFE"]),
                        //Provincia = Convert.ToString(row["U_provincia"]),
                        //Canton = Convert.ToString(row["U_canton"]),
                        //Distrito = Convert.ToString(row["U_distrito"]),
                        //Barrio = Convert.ToString(row["U_barrio"]),
                        //Direccion = Convert.ToString(row["U_direccion"]),
                        Email = Convert.ToString(row["U_CorreoFE"])
                    };

                    feInfo.FEInfo = bpFEInfo;
                }
                else
                {
                    feInfo.Result = false;
                    feInfo.Error = new ErrorInfo
                    {
                        Message = "No se encontró Información..."
                    };
                }

                return feInfo;
            }
            catch (Exception exc)
            {
                return (BPFEInfoResponseModel)LogManager.HandleExceptionWithReturn(exc, "BPFEInfoResponseModel");
            }
        }

        public static ApiResponse<List<ItemDataForInvoiceGoodReceipt>> GetItemDataForGoodReceiptInvoice(List<string> itemCodes, CredentialHolder _userCredentials, string WhsCode, string _dbObjectSpGetItemAvgPrice, string _dbObjectSpGetItemLastPrice, string _dbObjectSpGetValidateDeviation)
        {
            try
            {
                List<ItemDataForInvoiceGoodReceipt> values = new List<ItemDataForInvoiceGoodReceipt>();

                string query = string.Empty;


                foreach (var itemCode in itemCodes)
                {
                    var ItemData = new ItemDataForInvoiceGoodReceipt()
                    {
                        ItemCode = itemCode,
                        AVGPrice = 0,
                        LastPrice = 0,
                        DeviationStatus = 0
                    };
                    query = QueryType == "SQL" ?
                    string.Format("EXEC {0}.dbo." + _dbObjectSpGetItemAvgPrice + " {1}", _userCredentials.DBCode, itemCode, WhsCode)
                   : $"CALL {_userCredentials.DBCode}.{_dbObjectSpGetItemAvgPrice}('{itemCode}', '{WhsCode}')";


                    string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                    DataTable dataTable = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                    if (dataTable != null && dataTable.Rows.Count > 0)
                    {
                        DataRow row = dataTable.Rows[0];
                        ItemData.AVGPrice = Convert.ToDouble(row["AvgPrice"]);
                    }
                    query = QueryType == "SQL" ?
                    string.Format("EXEC {0}.dbo." + _dbObjectSpGetItemLastPrice + " {1}", _userCredentials.DBCode, itemCode)
                   : $"CALL {_userCredentials.DBCode}.{_dbObjectSpGetItemLastPrice}('{itemCode}')";


                    dataTable = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                    if (dataTable.Rows != null && dataTable.Rows.Count > 0)
                    {
                        DataRow row = dataTable.Rows[0];

                        ItemData.LastPrice = Convert.ToDouble(row["LastPrice"]);
                    }

                    query = QueryType == "SQL" ?
                    string.Format("EXEC {0}.dbo." + _dbObjectSpGetValidateDeviation + " {1},{2},{3}", _userCredentials.DBCode, itemCode, ItemData.AVGPrice, ItemData.LastPrice)
                  : $"CALL {_userCredentials.DBCode}.{_dbObjectSpGetValidateDeviation}('{itemCode}', {ItemData.AVGPrice}, {ItemData.LastPrice})";

                    dataTable = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                    if (dataTable.Rows != null && dataTable.Rows.Count > 0)
                    {
                        DataRow row = dataTable.Rows[0];

                        ItemData.DeviationStatus = Convert.ToInt32(row["DeviationStatus"]);
                        ItemData.Message = Convert.ToString(row["Message"]);
                    }


                    values.Add(ItemData);

                }

                return new ApiResponse<List<ItemDataForInvoiceGoodReceipt>>()
                {
                    Result = true,
                    Error = null,
                    Data = values
                };
            }
            catch
            {
                throw;
            }
        }

        public static SyncResponse SyncGetBusinessPartners(CredentialHolder _userCredentials, string _dbObjectViewGetBPCustomers)
        {
            try
            {
                string sql = string.Format("SELECT * FROM {0}.dbo.{1}", _userCredentials.DBCode, _dbObjectViewGetBPCustomers);

                List<Models.CLVS_POS_BPS> bpList = new List<Models.CLVS_POS_BPS>();

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, sql);

                if (dt.Rows != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        Models.CLVS_POS_BPS bp = new Models.CLVS_POS_BPS
                        {
                            CardCode = Convert.ToString(row["CardCode"]),
                            CardName = Convert.ToString(row["CardName"]),
                            Currency = Convert.ToString(row["Currency"]),
                            TaxCode = Convert.ToString(row["TaxCode"]),
                            Available = Convert.ToDouble(row["Available"]),
                            CreditLine = Convert.ToDouble(row["CreditLine"]),
                            Balance = Convert.ToDouble(row["Balance"]),
                            Phone1 = Convert.ToString(row["Phone1"]),
                            Phone2 = Convert.ToString(row["Phone2"]),
                            Fax = Convert.ToString(row["Fax"]),
                            Cellular = Convert.ToString(row["Cellular"]),
                            E_Mail = Convert.ToString(row["E_Mail"]),
                            MailAddres = Convert.ToString(row["MailAddres"]),
                            Discount = Convert.ToDouble(row["Discount"]),
                            ListNum = Convert.ToInt32(row["ListNum"]),
                            GroupNum = Convert.ToInt32(row["GroupNum"]),
                            Address = Convert.ToString(row["Address"]),
                            Lat = Convert.ToString(row["Lat"]),
                            Lng = Convert.ToString(row["Lng"]),
                            QryGroup1 = Convert.ToString(row["QryGroup1"]),
                            EditPrice = Convert.ToString(row["EditPrice"]),
                            Cedula = Convert.ToString(row["Cedula"]),
                            ContactPerson = Convert.ToString(row["ContactPerson"]),
                            U_TipoIdentificacion = Convert.ToString(row["U_TipoIdentificacion"]),
                            U_provincia = Convert.ToString(row["U_provincia"]),
                            U_canton = Convert.ToString(row["U_canton"]),
                            U_distrito = Convert.ToString(row["U_distrito"]),
                            U_barrio = Convert.ToString(row["U_barrio"]),
                            U_direccion = Convert.ToString(row["U_direccion"])
                        };
                        bpList.Add(bp);
                    };
                    return new SyncResponse
                    {
                        result = true,
                        rowsToSync = bpList.Cast<object>().ToList()
                    };
                }
                else
                {
                    return new SyncResponse
                    {
                        result = false
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Funcion que obtiene un modelo con 2 listas una que es el codigo de todos los ITEMS y la otra obtiene el Nombre de todos los Items
        /// </summary>
        /// <param name="company"></param>
        /// <param name="_dbObjectViewGetItems"></param>
        /// <param name="_dbObjectViewGetItemGroupList"></param>
        /// <param name="_dbObjectViewGetFirmsList"></param>
        /// <returns></returns>
        public static ItemNamesResponse GetItemNames(CredentialHolder _userCredentials, string _dbObjectViewGetItems, string _dbObjectViewGetItemGroupList, string _dbObjectViewGetFirmsList)
        {
            try
            {
                ItemNamesModel itemNameList = new ItemNamesModel();

                List<string> itemList = new List<string>();
                List<string> itemCodes = new List<string>();
                List<string> itembarCode = new List<string>();
                List<string> ItemCompleteName = new List<string>();

                List<ItemGroupModel> ItemGroupName = new List<ItemGroupModel>();
                List<ItemFirmModel> ItemFirmName = new List<ItemFirmModel>();

                string query = QueryType == "SQL" ?
                string.Format("SELECT * FROM {0}.dbo.{1}", _userCredentials.DBCode, _dbObjectViewGetItems)
                   : $"SELECT * FROM {_userCredentials.DBCode}.{_dbObjectViewGetItems}";

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                foreach (DataRow row in dt.Rows)
                {
                    itemList.Add(Convert.ToString(row["ItemName"]));
                    itemCodes.Add(Convert.ToString(row["ItemCode"]));
                    itembarCode.Add(Convert.ToString(row["CodeBars"]));
                    ItemCompleteName.Add(string.Format("{0} COD. {1} COD. {2}", Convert.ToString(row["ItemCode"]), Convert.ToString(row["CodeBars"]), Convert.ToString(row["ItemName"])));
                }

                itemNameList.ItemName = itemList;
                itemNameList.ItemCode = itemCodes;
                itemNameList.ItemCompleteName = ItemCompleteName;
                itemNameList.ItemGroupName = GetGroupNums(_userCredentials, _dbObjectViewGetItemGroupList);
                itemNameList.ItemFirmName = GetFirms(_userCredentials, _dbObjectViewGetFirmsList);

                return new ItemNamesResponse
                {
                    Result = true,
                    ItemList = itemNameList
                };

            }
            catch (Exception exc)
            {
                return (ItemNamesResponse)LogManager.HandleExceptionWithReturn(exc, "ItemNamesResponse");
            }
        }

        /// <summary>
        /// Funcion que obtiene el detalle de un item de acuerdo a los parametros especificados
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="discount"></param>
        /// <param name="company"></param>
        /// <param name="priceList"></param>
        /// <param name="_cardCode"></param>
        /// <param name="whCode"></param>
        /// <param name="_dbObjectSpGetItemInfo"></param>
        /// <returns></returns>
        public static ItemsResponse GetInfoItem(string itemCode, decimal discount, CredentialHolder _userCredentials, int priceList, string _cardCode, string whCode, string _documentType, string _dbObjectSpGetItemInfo)
        {
            try
            {
                ItemsModel item = new ItemsModel();
                string query = QueryType == "SQL" ?
                string.Format("EXEC {0}.dbo.{1} '{2}','{3}','{4}', '{5}', '{6}', '{7}'", _userCredentials.DBCode, _dbObjectSpGetItemInfo, itemCode, discount, priceList, _cardCode, whCode, _documentType)
                 : $"CALL {_userCredentials.DBCode}.{_dbObjectSpGetItemInfo}('{itemCode}', '{discount}', '{priceList}', '{_cardCode}', '{whCode}', '{_documentType}')";

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                if (dt.Rows.Count > 0)
                {
                    item = new ItemsModel
                    {
                        ItemCode = Convert.ToString(dt.Rows[0]["ItemCode"]),
                        OnHand = Convert.ToInt32(dt.Rows[0]["OnHand"]),
                        ItemName = Convert.ToString(dt.Rows[0]["ItemName"]),
                        InvntItem = Convert.ToString(dt.Rows[0]["InvntItem"]),
                        Discount = dt.Rows[0]["U_Discount"] != null && dt.Rows[0]["U_Discount"].ToString() != "" ? Convert.ToDecimal(dt.Rows[0]["U_Discount"]) : 0,
                        TaxCode = string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["TaxCode"])) ? "" : Convert.ToString(dt.Rows[0]["TaxCode"]),
                        TaxRate = string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["TaxRate"])) ? 0 : Convert.ToDouble(dt.Rows[0]["TaxRate"]),
                        UnitPrice = dt.Rows[0]["UnitPrice"] != null && dt.Rows[0]["UnitPrice"].ToString() != "" ? Convert.ToDecimal(dt.Rows[0]["UnitPrice"]) : 0,
                        FirmName = string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["PreferredVendor"])) ? "" : Convert.ToString(dt.Rows[0]["PreferredVendor"]),
                        ForeingName = string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["ForeingName"])) ? "" : Convert.ToString(dt.Rows[0]["ForeingName"]),
                        LastPurchaseDate = string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["LastPurDat"])) ? null : (DateTime?)dt.Rows[0]["LastPurDat"],
                        LastPurchasePrice = dt.Rows[0]["LastPurPrc"] != null && dt.Rows[0]["LastPurPrc"].ToString() != "" ? Convert.ToDouble(dt.Rows[0]["LastPurPrc"]) : 0.0,
                        HasInconsistency = Convert.ToBoolean(dt.Rows[0]["HasInconsistency"]),
                        InconsistencyMessage = Convert.ToString(dt.Rows[0]["InconsistencyMessage"]),
                        ItemClass = Convert.ToString(dt.Rows[0]["ItemClass"]),
                        ShouldValidateStock = Convert.ToBoolean(dt.Rows[0]["ShouldValidateStock"])
                    };

                    return new ItemsResponse
                    {
                        Result = true,
                        Item = item
                    };
                }
                else
                {
                    throw new Exception("No existe informacion del item en SAP.");
                }
            }
            catch (Exception exc)
            {
                return (ItemsResponse)LogManager.HandleExceptionWithReturn(exc, "ItemsResponse");
            }
        }

        /// <summary>
        /// Funcion que se utiliza y retorna el tipo de cambio del dia 
        /// </summary>
        /// <param name="company"></param>
        /// <param name="_dbObjectViewGetExRate"></param>
        /// <returns></returns>
        public static ExchangeRateResponse GetExchangeRate(CredentialHolder _userCredentials, string _dbObjectViewGetExRate)
        {
            try
            {
                double Rate = 0;
                string query = QueryType == "SQL" ?
                string.Format("SELECT * FROM {0}.[dbo].[{1}]", _userCredentials.DBCode, _dbObjectViewGetExRate)
                   : $"SELECT * FROM {_userCredentials.DBCode}.{_dbObjectViewGetExRate}";


                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                if (dt.Rows.Count > 0)
                {
                    if (Convert.ToDouble(dt.Rows[0]["Rate"]) > 0)
                    {
                        Rate = Convert.ToDouble(dt.Rows[0]["Rate"]);

                        return new ExchangeRateResponse
                        {
                            Result = true,
                            exRate = Rate
                        };
                    }
                    else
                    {
                        throw new Exception("El tipo de cambio es igual a cero");
                    }
                }
                else
                {
                    throw new Exception("No se ha definido el tipo de cambio.");
                }
            }
            catch (Exception exc)
            {
                return (ExchangeRateResponse)LogManager.HandleExceptionWithReturn(exc, "ExchangeRateResponse");
            }
        }


        /// <summary>
        /// Metodo para obtener detalles de un articulo y un numero de entradas 
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="company"></param>
        /// <param name="NumeroEntradas"></param>
        /// <param name="forView"></param>
        /// <param name="_dbObjectSpGetItemPurchaseDetail"></param>
        /// <returns></returns>
        public static ItemDetailResponse GetItemDetails(string itemCode, CredentialHolder _userCredentials, int NumeroEntradas, int forView, string _dbObjectSpGetItemPurchaseDetail)
        {
            try
            {
                string TableCode = string.Empty;

                switch (forView)
                {
                    case 1:
                        TableCode = "OPDN";
                        break;
                    default:
                        TableCode = "OPCH";
                        break;
                }


                ItemDetail item = new ItemDetail();
                string query = QueryType == "SQL" ?
                  string.Format("EXEC {0}.dbo." + _dbObjectSpGetItemPurchaseDetail + " '{1}','{2}','{3}'", _userCredentials.DBCode, itemCode, NumeroEntradas, TableCode)
                   : $"CALL {_userCredentials.DBCode}.{_dbObjectSpGetItemPurchaseDetail}('{itemCode}','{NumeroEntradas}','{TableCode}')";

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                if (dt.Rows.Count > 0)
                {
                    item = new ItemDetail
                    {
                        ItemCode = Convert.ToString(dt.Rows[0]["ItemCode"]),
                        ItemName = Convert.ToString(dt.Rows[0]["ItemName"]),
                        OnHand = Convert.ToInt32(dt.Rows[0]["Stock"]),
                        Available = Convert.ToInt32(dt.Rows[0]["Available"]),
                        TaxRate = Convert.ToString(dt.Rows[0]["U_IVA"]),
                        LastPurPrc = dt.Rows[0]["LastPurPrc"] != null && dt.Rows[0]["LastPurPrc"].ToString() != "" ? Convert.ToDouble(dt.Rows[0]["LastPurPrc"]) : 0.0
                    };
                    item.GoodsRecipts = new List<GoodReceipt>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        item.GoodsRecipts.Add(new GoodReceipt()
                        {
                            BissnesPartner = new BusinessPartnerModel()
                            {
                                CardName = Convert.ToString(dt.Rows[i]["CardName"]),
                                CardCode = Convert.ToString(dt.Rows[i]["CardCode"])
                            },
                            Store = new StoresModel()
                            {
                                StoreCode = Convert.ToString(dt.Rows[i]["WhsCode"]),
                                StoreName = Convert.ToString(dt.Rows[i]["WhsName"])

                            },
                            DocDate = Convert.ToDateTime(dt.Rows[i]["DocDate"]),
                            Comment = Convert.ToString(dt.Rows[i]["Comments"]),
                            DocEntry = Convert.ToInt32(dt.Rows[i]["DocEntry"]),
                            DocNum = Convert.ToInt32(dt.Rows[i]["DocNum"]),
                            DocTotal = Convert.ToInt32(dt.Rows[i]["DocTotal"]),
                            Price = Convert.ToDouble(dt.Rows[i]["Price"]),
                            Quantity = Convert.ToInt32(dt.Rows[i]["Quantity"]),
                            TaxCode = Convert.ToString(dt.Rows[i]["TaxCode"])

                        });
                    }






                    return new ItemDetailResponse
                    {
                        Result = true,
                        Item = item
                    };
                }
                else
                {
                    throw new Exception("No existen entradas del item en SAP.");
                }
            }
            catch (Exception exc)
            {
                return (ItemDetailResponse)LogManager.HandleExceptionWithReturn(exc, "ItemDetailResponse");
            }
        }

        /// <summary>
        /// Funcion que se utiliza y retorna el tipo de cambio del dia 
        /// </summary>
        /// <param name="company"></param>
        /// <param name="userId"></param>
        /// <param name="_dbObjectViewGetExrate"></param>
        /// <returns></returns>
        public static SyncResponse SyncGetExchangeRate(CredentialHolder _userCredentials, string userId, string _dbObjectViewGetExrate)
        {
            try
            {
                string sql = string.Format("SELECT * FROM {0}.dbo.{1}", _userCredentials.DBCode, _dbObjectViewGetExrate);

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, sql);
                Double rate = 0;
                DateTime rateDate;
                int index = 1;

                if (dt.Rows.Count > 0)
                {
                    if (Convert.ToDouble(dt.Rows[0]["Rate"]) > 0)
                    {
                        rate = Convert.ToDouble(dt.Rows[0]["Rate"]);
                        rateDate = Convert.ToDateTime(dt.Rows[0]["RateDate"]);
                    }
                    else
                    {
                        throw new Exception("El tipo de cambio es igual a cero");
                    }
                }
                else
                {
                    throw new Exception("No se ha definido el tipo de cambio.");
                }


                DAO.SuperV2_Entities db = new SuperV2_Entities();
                List<Models.CLVS_POS_EXRATE> rateUsersList = new List<Models.CLVS_POS_EXRATE>();


                db.Users.ToList().ForEach(u =>
                {
                    rateUsersList.Add(new Models.CLVS_POS_EXRATE
                    {
                        Rate = rate,
                        RateDate = rateDate,
                        UserId = u.Id
                    });
                });

                db.Dispose();

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = rateUsersList.Cast<object>().ToList()
                };


            }
            catch (Exception exc)
            {
                return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncGetExchangeRate");
            }
        }

        public static SyncResponse SyncGetItems(CredentialHolder _userCredentials, string _dbObjectViewGetItems)
        {
            try
            {
                List<Models.CLVS_POS_ITEMS> itemList = new List<Models.CLVS_POS_ITEMS>();

                string sql = string.Format("SELECT * FROM {0}.dbo.{1}", _userCredentials.DBCode, _dbObjectViewGetItems);

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, sql);

                foreach (DataRow row in dt.Rows)
                {
                    Models.CLVS_POS_ITEMS item = new Models.CLVS_POS_ITEMS()
                    {
                        ItemName = Convert.ToString(row["ItemName"]),
                        ItemCode = Convert.ToString(row["ItemCode"]),
                        CodeBars = Convert.ToString(row["CodeBars"]),
                        Available = Convert.ToDouble(row["Available"])
                    };

                    itemList.Add(item);
                }

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = itemList.Cast<object>().ToList()
                };

            }
            catch (Exception exc)
            {
                return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse");
            }
        }

        public static SyncResponse SyncGetFirms(CredentialHolder _userCredentials, string _dbObjectViewGetFirmsList)
        {
            List<Models.CLVS_POS_GETFIRMSLIST> firmsList = new List<Models.CLVS_POS_GETFIRMSLIST>();
            try
            {
                string sql = string.Format("SELECT * FROM {0}.dbo." + _dbObjectViewGetFirmsList, _userCredentials.DBCode);

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, sql);

                foreach (DataRow row in dt.Rows)
                {
                    firmsList.Add(new Models.CLVS_POS_GETFIRMSLIST
                    {
                        FirmCode = Convert.ToInt32(row["FirmCode"]),
                        FirmName = Convert.ToString(row["FirmName"])
                    });
                }

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = firmsList.Cast<object>().ToList()
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static SyncResponse SyncGetOCRD(CredentialHolder _userCredentials)
        {
            List<Models.OCRD> ocdrList = new List<Models.OCRD>();
            try
            {
                string sql = string.Format("SELECT CardCode, CardName, FatherCard, U_TipoIdentificacion, U_provincia, U_canton, U_distrito, U_barrio, U_direccion, GroupNum FROM {0}.dbo.OCRD", _userCredentials.DBCode);

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, sql);

                foreach (DataRow row in dt.Rows)
                {
                    ocdrList.Add(new Models.OCRD
                    {
                        CardCode = Convert.ToString(row["CardCode"]),
                        CardName = Convert.ToString(row["CardName"]),
                        FatherCard = Convert.ToString(row["FatherCard"]),
                        U_TipoIdentificacion = Convert.ToString(row["U_TipoIdentificacion"]),
                        U_provincia = Convert.ToString(row["U_provincia"]),
                        U_canton = Convert.ToString(row["U_canton"]),
                        U_distrito = Convert.ToString(row["U_distrito"]),
                        U_barrio = Convert.ToString(row["U_barrio"]),
                        U_direccion = Convert.ToString(row["U_direccion"]),
                        GroupNum = Convert.ToInt16(row["GroupNum"])
                    });
                }

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = ocdrList.Cast<object>().ToList()
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static SyncResponse SyncGetOTCX(CredentialHolder _userCredentials, string _dbObjectViewGetOTCX)
        {
            List<Models.OTCX> otcxList = new List<Models.OTCX>();
            try
            {
                string sql = string.Format("SELECT LnTaxCode, StrVal1 FROM {0}.dbo.{1}", _userCredentials.DBCode, _dbObjectViewGetOTCX);

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, sql);

                foreach (DataRow row in dt.Rows)
                {
                    otcxList.Add(new Models.OTCX
                    {
                        LnTaxCode = Convert.ToString(row["LnTaxCode"]),
                        StrVal1 = Convert.ToString(row["StrVal1"])
                    });
                }

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = otcxList.Cast<object>().ToList()
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static SyncResponse SyncGetOITM(CredentialHolder _userCredentials)
        {
            List<Models.OITM> oitmList = new List<Models.OITM>();
            try
            {
                // string sql = string.Format("EXEC {0}.dbo.[CLVS_POS_GETNUMFEONLINE_SPR] {1}", dbn, docEntry);
                string sql = string.Format("SELECT ItemCode, ItemName, OnHand, CardCode,ItmsGrpCod, frozenFor, InvntItem,CodeBars,U_IVA FROM {0}.dbo.OITM", _userCredentials.DBCode);

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, sql);

                foreach (DataRow row in dt.Rows)
                {
                    oitmList.Add(new Models.OITM
                    {
                        ItemCode = Convert.ToString(row["ItemCode"]),
                        ItemName = Convert.ToString(row["ItemName"]),
                        OnHand = Convert.ToDouble(row["OnHand"]),
                        CardCode = Convert.ToString(row["CardCode"]),
                        ItmsGrpCod = Convert.ToInt32(row["ItmsGrpCod"]),
                        frozenFor = Convert.ToString(row["frozenFor"]),
                        InvntItem = Convert.ToString(row["InvntItem"]),
                        CodeBars = Convert.ToString(row["CodeBars"]),
                        U_IVA = Convert.ToString(row["U_IVA"])
                    });
                }

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = oitmList.Cast<object>().ToList()
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static SyncResponse SyncGetOSTA(CredentialHolder _userCredentials)
        {
            List<Models.OSTA> ostaList = new List<Models.OSTA>();
            try
            {
                string sql = string.Format("SELECT Code, Name, Rate FROM {0}.dbo.OSTA", _userCredentials.DBCode);

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, sql);

                foreach (DataRow row in dt.Rows)
                {
                    ostaList.Add(new Models.OSTA
                    {
                        Code = Convert.ToString(row["Code"]),
                        Name = Convert.ToString(row["Name"]),
                        Rate = Convert.ToDouble(row["Rate"])
                    });
                }

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = ostaList.Cast<object>().ToList()
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static SyncResponse SyncGetITM1(CredentialHolder _userCredentials)
        {
            List<Models.ITM1> itm1List = new List<Models.ITM1>();
            try
            {
                string sql = string.Format("SELECT T1.PriceList,T1.ItemCode, COALESCE(T1.Price,0) AS Price, T0.LastPurPrc FROM {0}.dbo.ITM1 T1  LEFT JOIN {0}.dbo.OITM T0 ON T1.ItemCode = T0.ItemCode", _userCredentials.DBCode);

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, sql);

                foreach (DataRow row in dt.Rows)
                {
                    itm1List.Add(new Models.ITM1
                    {
                        PriceList = Convert.ToInt32(row["PriceList"]),
                        ItemCode = Convert.ToString(row["ItemCode"]),
                        Price = Convert.ToDouble(row["Price"]),
                        LastPurchasePrice = Convert.ToDouble(row["LastPurPrc"])

                    });
                }

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = itm1List.Cast<object>().ToList()
                };
            }
            catch (Exception)
            {
                throw;
            }
        }



        public static SyncResponse SyncGetOITW(CredentialHolder _userCredentials)
        {
            List<Models.OITW> oitwList = new List<Models.OITW>();
            try
            {
                string sql = string.Format("SELECT WhsCode, ItemCode, OnHand, IsCommited, OnOrder, AvgPrice FROM {0}.dbo.OITW", _userCredentials.DBCode);

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, sql);

                foreach (DataRow row in dt.Rows)
                {
                    oitwList.Add(new Models.OITW
                    {
                        WhsCode = Convert.ToString(row["WhsCode"]),
                        ItemCode = Convert.ToString(row["ItemCode"]),
                        OnHand = Convert.ToDouble(row["OnHand"]),
                        IsCommited = Convert.ToDouble(row["IsCommited"]),
                        OnOrder = Convert.ToDouble(row["OnOrder"]),
                        AvgPrice = Convert.ToDouble(row["AvgPrice"]),
                    });
                }

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = oitwList.Cast<object>().ToList()
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static SyncResponse SyncGetOWHS(CredentialHolder _userCredentials)
        {
            List<Models.OWHS> oitwList = new List<Models.OWHS>();
            try
            {
                string sql = string.Format("SELECT WhsCode,  WhsName FROM {0}.dbo.OWHS", _userCredentials.DBCode);

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, sql);

                foreach (DataRow row in dt.Rows)
                {
                    oitwList.Add(new Models.OWHS
                    {
                        WhsCode = Convert.ToString(row["WhsCode"]),
                        WhsName = Convert.ToString(row["WhsName"])
                    });
                }

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = oitwList.Cast<object>().ToList()
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static SyncResponse SyncGetINV6(Companys company)
        {
            List<Models.INV6> inv6List = new List<Models.INV6>();
            try
            {
                string sql = string.Format("SELECT ISNULL(InsTotalFC,0) InsTotalFC, ISNULL(PaidFC,0) PaidFC, DocEntry, InstlmntID, PaidToDate FROM {0}.dbo.INV6", company.DBCode);

                DataTable dt = ExecuteQuery(Common.ReplaceConnectorODBC(company.DBCode), sql);

                foreach (DataRow row in dt.Rows)
                {
                    inv6List.Add(new Models.INV6
                    {
                        DocEntry = Convert.ToInt32(row["DocEntry"]),
                        InsTotalFC = Convert.ToDouble(row["InsTotalFC"]),
                        PaidFC = Convert.ToDouble(row["PaidFC"]),
                        InstlmntID = Convert.ToInt32(row["InstlmntID"]),
                        PaidToDate = Convert.ToDouble(row["PaidToDate"]),
                    });
                }

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = inv6List.Cast<object>().ToList()
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static SyncResponse SyncGetDPI6(Companys company)
        {
            List<Models.DPI6> dpi6List = new List<Models.DPI6>();
            try
            {
                string sql = string.Format("SELECT ISNULL(InsTotalFC,0) InsTotalFC, ISNULL(PaidFC,0) PaidFC, DocEntry, ISNULL(InsTotal,0) InsTotal, InstlmntID, PaidToDate FROM {0}.dbo.DPI6", company.DBCode);

                DataTable dt = ExecuteQuery(Common.ReplaceConnectorODBC(company.DBCode), sql);

                foreach (DataRow row in dt.Rows)
                {
                    dpi6List.Add(new Models.DPI6
                    {
                        DocEntry = Convert.ToInt32(row["DocEntry"]),
                        InsTotalFC = Convert.ToDouble(row["InsTotalFC"]),
                        PaidFC = Convert.ToDouble(row["PaidFC"]),
                        InsTotal = Convert.ToDouble(row["InsTotal"]),
                        InstlmntID = Convert.ToInt32(row["InstlmntID"]),
                        PaidToDate = Convert.ToDouble(row["PaidToDate"]),
                    });
                }

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = dpi6List.Cast<object>().ToList()
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static SyncResponse SyncGetODPI(Companys company)
        {
            List<Models.ODPI> odpiList = new List<Models.ODPI>();
            try
            {
                string sql = string.Format("SELECT DocEntry, DocNum, DocType, DocDate, DocDueDate, CardCode, CardName, NumAtCard, DocCur, DocStatus FROM {0}.dbo.ODPI", company.DBCode);

                DataTable dt = ExecuteQuery(Common.ReplaceConnectorODBC(company.DBCode), sql);

                foreach (DataRow row in dt.Rows)
                {
                    odpiList.Add(new Models.ODPI
                    {
                        DocEntry = Convert.ToInt32(row["DocEntry"]),
                        DocNum = Convert.ToInt32(row["DocNum"]),
                        DocType = Convert.ToString(row["DocType"]),
                        DocDate = Convert.ToDateTime(row["DocDate"]),
                        DocDueDate = Convert.ToDateTime(row["DocDueDate"]),
                        CardCode = Convert.ToString(row["CardCode"]),
                        CardName = Convert.ToString(row["CardName"]),
                        NumAtCard = Convert.ToString(row["NumAtCard"]),
                        DocCur = Convert.ToString(row["DocCur"]),
                        DocStatus = Convert.ToString(row["DocStatus"])
                    });
                }

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = odpiList.Cast<object>().ToList()
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static SyncResponse SyncGetOINV(Companys company, string filter)
        {
            List<Models.OINV> oinvList = new List<Models.OINV>();
            try
            {
                string sql = string.Format("SELECT DocEntry, DocNum,CardCode, CardName, NumAtCard, DocCur, DocDate, DocDueDate, DocStatus, Series," + Environment.NewLine +
                                           "       ISNULL(U_TipoIdentificacion,'') U_TipoIdentificacion, ISNULL(U_NumIdentFE,'') U_NumIdentFE, ISNULL(U_CorreoFE,'') U_CorreoFE," + Environment.NewLine +
                                           "       ISNULL(U_provincia,'') U_provincia, ISNULL(U_canton,'') U_canton, ISNULL(U_distrito,'') U_distrito, ISNULL(U_barrio,'') U_barrio, ISNULL(U_direccion,'') U_direccion," + Environment.NewLine +
                                           "       Canceled,SlpCode,DocRate,DocTime,DocTotal,DocTotalFC,PaidToDate,PaidFC,UserSign,ISNULL(Comments,'') Comments, ISNULL(U_ClaveFE,'') U_ClaveFE, ISNULL(U_NumFE,'') U_NumFE " + Environment.NewLine +
                                           "FROM {0}.dbo.OINV (NOLOCK) {1}", company.DBCode, filter.Length > 0 ? string.Format("WHERE {0}", filter) : string.Empty);

                DataTable dt = ExecuteQuery(Common.ReplaceConnectorODBC(company.DBCode), sql);

                foreach (DataRow row in dt.Rows)
                {
                    oinvList.Add(new Models.OINV
                    {
                        DocEntry = Convert.ToInt32(row["DocEntry"]),
                        DocNum = Convert.ToInt32(row["DocNum"]),
                        CardCode = Convert.ToString(row["CardCode"]),
                        CardName = Convert.ToString(row["CardName"]),
                        NumAtCard = Convert.ToString(row["NumAtCard"]),
                        DocCur = Convert.ToString(row["DocCur"]),
                        DocDate = Convert.ToDateTime(row["DocDate"]),
                        DocDueDate = Convert.ToDateTime(row["DocDueDate"]),
                        DocStatus = Convert.ToString(row["DocStatus"]),
                        Series = Convert.ToString(row["Series"]),
                        U_barrio = Convert.ToString(row["U_barrio"]),
                        U_canton = Convert.ToString(row["U_canton"]),
                        U_CorreoFE = Convert.ToString(row["U_CorreoFE"]),
                        U_direccion = Convert.ToString(row["U_direccion"]),
                        U_distrito = Convert.ToString(row["U_distrito"]),
                        U_NumIdentFE = Convert.ToString(row["U_NumIdentFE"]),
                        U_provincia = Convert.ToString(row["U_provincia"]),
                        U_TipoIdentificacion = Convert.ToString(row["U_TipoIdentificacion"]),
                        U_ClaveFE = Convert.ToString(row["U_ClaveFE"]),
                        U_NumFE = Convert.ToString(row["U_NumFE"]),
                        Canceled = Convert.ToString(row["Canceled"]),
                        DocRate = Convert.ToDecimal(row["DocRate"]),
                        DocTotal = Convert.ToDecimal(row["DocTotal"]),
                        PaidFC = Convert.ToDecimal(row["PaidFC"]),
                        PaidToDate = Convert.ToDecimal(row["PaidToDate"]),
                        DocTotalFC = Convert.ToDecimal(row["DocTotalFC"]),
                        DocTime = Convert.ToInt16(row["DocTime"]),
                        UserSign = Convert.ToInt16(row["UserSign"]),
                        SlpCode = Convert.ToInt16(row["SlpCode"])
                    });
                }

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = oinvList.Cast<object>().ToList()
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        internal static DocInfo GetGoodReceiptDocNum(CredentialHolder userCredentials, string _dbObjectSpGetGoodReceiptDocNum, int _docEntry)
        {
            DocInfo docInfo = null;
            try
            {

                string query = QueryType == "SQL" ?
                 string.Format("EXEC {0}.dbo.[{2}] '{1}'", userCredentials.DBCode, _docEntry, _dbObjectSpGetGoodReceiptDocNum)
                    : $"CALL {userCredentials.DBCode}.{_dbObjectSpGetGoodReceiptDocNum}('{_docEntry}')";

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                foreach (DataRow row in dt.Rows)
                {
                    docInfo = new DocInfo
                    {
                        DocNum = Convert.ToInt32(row["DocNum"]),

                    };
                }

                return docInfo;
            }
            catch (Exception e)
            {
                return docInfo;
            }
        }

        public static SyncResponse SyncGetORCT(Companys company)
        {
            List<Models.ORCT> orctList = new List<Models.ORCT>();
            try
            {
                string sql = string.Format("SELECT DocTime,DocDate,DocNum,DocEntry,DocCurr,CashSum,CreditSum,CheckSum,TrsfrSum,CashSumFC,CredSumFC,CheckSumFC,TrsfrSumFC,DocTotal,DocTotalFC,CardName" + Environment.NewLine +
                                           "FROM {0}.dbo.ORCT (NOLOCK)", company.DBCode);

                DataTable dt = ExecuteQuery(Common.ReplaceConnectorODBC(company.DBCode), sql);

                foreach (DataRow row in dt.Rows)
                {
                    orctList.Add(new Models.ORCT
                    {
                        CashSum = Convert.ToDecimal(row["CashSum"]),
                        CashSumFC = Convert.ToDecimal(row["CashSumFC"]),
                        CheckSum = Convert.ToDecimal(row["CheckSum"]),
                        CheckSumFC = Convert.ToDecimal(row["CheckSumFC"]),
                        CreditSum = Convert.ToDecimal(row["CreditSum"]),
                        CredSumFC = Convert.ToDecimal(row["CredSumFC"]),
                        DocTotal = Convert.ToDecimal(row["DocTotal"]),
                        DocTotalFC = Convert.ToDecimal(row["DocTotalFC"]),
                        TrsfrSum = Convert.ToDecimal(row["TrsfrSum"]),
                        TrsfrSumFC = Convert.ToDecimal(row["TrsfrSumFC"]),
                        DocCurr = Convert.ToString(row["DocCurr"]),
                        DocDate = Convert.ToDateTime(row["DocDate"]),
                        DocEntry = Convert.ToInt32(row["DocEntry"]),
                        DocNum = Convert.ToInt32(row["DocEntry"]),
                        DocTime = Convert.ToInt16(row["DocEntry"])
                    });
                }

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = orctList.Cast<object>().ToList()
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static SyncResponse SyncGetRCT2(Companys company)
        {
            List<Models.RCT2> rct2List = new List<Models.RCT2>();
            try
            {
                string sql = string.Format("SELECT InvType,DocEntry,DocNum" + Environment.NewLine +
                                           "FROM   {0}.dbo.RCT2 (NOLOCK)", company.DBCode);

                DataTable dt = ExecuteQuery(Common.ReplaceConnectorODBC(company.DBCode), sql);

                foreach (DataRow row in dt.Rows)
                {
                    rct2List.Add(new Models.RCT2
                    {
                        InvType = Convert.ToString(row["InvType"]),
                        DocEntry = Convert.ToInt32(row["DocEntry"]),
                        DocNum = Convert.ToInt32(row["DocNum"]),
                    });
                }

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = rct2List.Cast<object>().ToList()
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        internal static DocInfo GetGoodReceipReturnDocNum(CredentialHolder _userCredentials, int _docEntry, string _dbObjectSpGetGoodReceipReturnDocNum)
        {
            DocInfo docInfo = null;
            try
            {
                string query = QueryType == "SQL" ?
                  string.Format("EXEC {0}.dbo.[{2}] '{1}'", _userCredentials.DBCode, _docEntry, _dbObjectSpGetGoodReceipReturnDocNum)
                    : $"CALL {_userCredentials.DBCode}.{_dbObjectSpGetGoodReceipReturnDocNum}('{_docEntry}')";


                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                foreach (DataRow row in dt.Rows)
                {
                    docInfo = new DocInfo
                    {
                        DocNum = Convert.ToInt32(row["DocNum"]),

                    };
                }

                return docInfo;
            }
            catch (Exception e)
            {
                return docInfo;
            }
        }

        public static SyncResponse SyncGetINV1(Companys company)
        {
            List<Models.INV1> inv1List = new List<Models.INV1>();
            try
            {
                string sql = string.Format("SELECT DocEntry,Currency,TaxCode,Dscription,DiscPrcnt,Price,Quantity,ItemCode,LineTotal,VatPrcnt,TotalSumSy" + Environment.NewLine +
                                           "FROM   {0}.dbo.INV1 (NOLOCK)", company.DBCode);

                DataTable dt = ExecuteQuery(Common.ReplaceConnectorODBC(company.DBCode), sql);

                foreach (DataRow row in dt.Rows)
                {
                    inv1List.Add(new Models.INV1
                    {
                        DocEntry = Convert.ToInt32(row["DocEntry"]),
                        Currency = Convert.ToString(row["Currency"]),
                        TaxCode = Convert.ToString(row["TaxCode"]),
                        Dscription = Convert.ToString(row["Dscription"]),
                        DiscPrcnt = Convert.ToDouble(row["DiscPrcnt"]),
                        Price = Convert.ToDouble(row["Price"]),
                        Quantity = Convert.ToInt32(row["Quantity"]),
                        ItemCode = Convert.ToString(row["ItemCode"]),
                        LineTotal = Convert.ToDouble(row["LineTotal"]),
                        TotalSumSy = Convert.ToDouble(row["TotalSumSy"]),
                        VatPrcnt = Convert.ToDouble(row["VatPrcnt"]),
                    });
                }

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = inv1List.Cast<object>().ToList()
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static SyncResponse SyncGetOUSR(Companys company)
        {
            List<Models.OUSR> ousrList = new List<Models.OUSR>();
            try
            {
                string sql = string.Format("SELECT INTERNAL_K" + Environment.NewLine +
                                           "FROM   {0}.dbo.OUSR (NOLOCK)", company.DBCode);

                DataTable dt = ExecuteQuery(Common.ReplaceConnectorODBC(company.DBCode), sql);

                foreach (DataRow row in dt.Rows)
                {
                    ousrList.Add(new Models.OUSR
                    {
                        INTERNAL_K = Convert.ToInt16(row["INTERNAL_K"])
                    });
                }

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = ousrList.Cast<object>().ToList()
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static SyncResponse SyncGetORDR(Companys company)
        {
            List<Models.ORDR> ocdrList = new List<Models.ORDR>();
            try
            {
                string sql = string.Format("SELECT DocEntry,DocDate,CardName,DocStatus,SlpCode" + Environment.NewLine +
                                           "FROM   {0}.dbo.ORDR (NOLOCK)", company.DBCode);

                DataTable dt = ExecuteQuery(Common.ReplaceConnectorODBC(company.DBCode), sql);

                foreach (DataRow row in dt.Rows)
                {
                    ocdrList.Add(new Models.ORDR
                    {
                        DocEntry = Convert.ToInt32(row["DocEntry"]),
                        DocDate = Convert.ToDateTime(row["DocDate"]),
                        CardName = Convert.ToString(row["CardName"]),
                        DocStatus = Convert.ToString(row["DocStatus"]),
                        SlpCode = Convert.ToInt32(row["SlpCode"])
                    });
                }

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = ocdrList.Cast<object>().ToList()
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static SyncResponse SyncGetOQUT(Companys company)
        {
            List<Models.OQUT> ocdrList = new List<Models.OQUT>();
            try
            {
                string sql = string.Format("SELECT DocEntry,DocDate,CardName,DocStatus,SlpCode" + Environment.NewLine +
                                           "FROM   {0}.dbo.OQUT (NOLOCK)", company.DBCode);

                DataTable dt = ExecuteQuery(Common.ReplaceConnectorODBC(company.DBCode), sql);

                foreach (DataRow row in dt.Rows)
                {
                    ocdrList.Add(new Models.OQUT
                    {
                        DocEntry = Convert.ToInt32(row["DocEntry"]),
                        DocDate = Convert.ToDateTime(row["DocDate"]),
                        CardName = Convert.ToString(row["CardName"]),
                        DocStatus = Convert.ToString(row["DocStatus"]),
                        SlpCode = Convert.ToInt32(row["SlpCode"])
                    });
                }

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = ocdrList.Cast<object>().ToList()
                };
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static SyncResponse SyncGetPriceGroups(Companys company, string _dbObjectViewGetItemGroupList)
        {
            List<Models.CLVS_POS_GETGROUPLIST> groupList = new List<Models.CLVS_POS_GETGROUPLIST>();
            try
            {
                string sql = string.Format("SELECT * FROM {0}.dbo." + _dbObjectViewGetItemGroupList, company.DBCode);

                DataTable dt = ExecuteQuery(Common.ReplaceConnectorODBC(company.DBCode), sql);

                foreach (DataRow row in dt.Rows)
                {
                    groupList.Add(new Models.CLVS_POS_GETGROUPLIST
                    {
                        ItmsGrpCod = Convert.ToInt32(row["ItmsGrpCod"]),
                        ItmsGrpNam = Convert.ToString(row["ItmsGrpNam"])
                    });
                }

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = groupList.Cast<object>().ToList()
                };


            }
            catch (Exception)
            {
                throw;
            }
        }

        public static SyncResponse SyncGetAccounts(Companys company, string _dbObjectViewGetAccounts)
        {
            try
            {
                List<Models.CLVS_POS_ACCOUNTS> accountList = new List<Models.CLVS_POS_ACCOUNTS>();

                string sql = string.Format("SELECT * FROM {0}.dbo.{1}", company.DBCode, _dbObjectViewGetAccounts);

                DataTable dt = ExecuteQuery(Common.ReplaceConnectorODBC(company.DBCode), sql);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        accountList.Add(new Models.CLVS_POS_ACCOUNTS
                        {
                            Account = Convert.ToString(row["Account"]),
                            AccountName = Convert.ToString(row["AccountName"])
                        });
                    }
                }
                else
                {
                    throw new Exception("No se encontraron datos.");
                }

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = accountList.Cast<object>().ToList()
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static SyncResponse SyncGetAccountsBank(Companys company, string _dbObjectViewGetBanks)
        {
            try
            {
                List<Models.CLVS_POS_GETBANKS> bankList = new List<Models.CLVS_POS_GETBANKS>();
                string sql = string.Format("SELECT * FROM {0}.dbo.{1}", company.DBCode, _dbObjectViewGetBanks);

                DataTable dt = ExecuteQuery(Common.ReplaceConnectorODBC(company.DBCode), sql);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        bankList.Add(new Models.CLVS_POS_GETBANKS
                        {
                            BankCode = Convert.ToString(row["BankCode"]),
                            BankName = Convert.ToString(row["BankName"])
                        });
                    }
                }
                else
                {
                    throw new Exception("No se encontraron datos.");
                }

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = bankList.Cast<object>().ToList()
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static SyncResponse SyncGetCards(Companys company, string _dbObjectViewGetCreditCards)
        {
            try
            {
                List<Models.CLVS_POS_CREDITCARDS> cardsList = new List<Models.CLVS_POS_CREDITCARDS>();

                string sql = string.Format("SELECT * FROM {0}.dbo.{1}", company.DBCode, _dbObjectViewGetCreditCards);

                DataTable dt = ExecuteQuery(Common.ReplaceConnectorODBC(company.DBCode), sql);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        cardsList.Add(new Models.CLVS_POS_CREDITCARDS
                        {
                            CardName = Convert.ToString(row["CardName"]),
                            AcctCode = Convert.ToString(row["AcctCode"])
                        });
                    }
                }
                else
                {
                    throw new Exception("No se encontraron datos.");
                }

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = cardsList.Cast<object>().ToList()
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static SyncResponse SyncGetPriceList(Companys company, string _dbObjectViewPriceList)
        {
            try
            {
                List<Models.CLVS_POS_GETPRICELIST> priceList = new List<Models.CLVS_POS_GETPRICELIST>();

                string sql = string.Format("SELECT * FROM {0}.dbo.{1}", company.DBCode, _dbObjectViewPriceList);

                DataTable dt = ExecuteQuery(Common.ReplaceConnectorODBC(company.DBCode), sql);

                foreach (DataRow row in dt.Rows)
                {
                    priceList.Add(new Models.CLVS_POS_GETPRICELIST
                    {
                        ListNum = Convert.ToInt32(row["ListNum"]),
                        ListName = Convert.ToString(row["ListName"])
                    });
                };

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = priceList.Cast<object>().ToList()
                };
            }
            catch (Exception exc)
            {
                return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse");
            }
        }

        public static SyncResponse SyncGetPayTermsList(Companys company, string _dbObjecViewGetPayTerms)
        {
            try
            {
                List<Models.CLVS_POS_PAYTERMS> payTermsList = new List<Models.CLVS_POS_PAYTERMS>();

                string sql = string.Format("SELECT * FROM {0}.dbo." + _dbObjecViewGetPayTerms, company.DBCode);

                DataTable dt = ExecuteQuery(Common.ReplaceConnectorODBC(company.DBCode), sql);

                foreach (DataRow row in dt.Rows)
                {
                    payTermsList.Add(new Models.CLVS_POS_PAYTERMS
                    {
                        GroupNum = Convert.ToInt32(row["GroupNum"]),
                        PymntGroup = Convert.ToString(row["PymntGroup"]),
                        Type = Convert.ToInt32(row["Type"])
                    });
                };

                return new SyncResponse { result = true, rowsToSync = payTermsList.Cast<object>().ToList() };
            }
            catch (Exception exc)
            {
                return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "PayTermsResponse");
            }
        }

        public static SyncResponse SyncGetSalesMan(Companys company, string _dbObjectViewGetSalesMan)
        {
            try
            {
                List<CLVS_POS_GETSALESMAN> salesManList = new List<CLVS_POS_GETSALESMAN>();
                string sql = string.Format("SELECT * FROM {0}.dbo.{1}", company.DBCode, _dbObjectViewGetSalesMan);

                DataTable dt = ExecuteQuery(Common.ReplaceConnectorODBC(company.DBCode), sql);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        salesManList.Add(new CLVS_POS_GETSALESMAN
                        {
                            SlpCode = Convert.ToInt32(row["SlpCode"]),
                            SlpName = Convert.ToString(row["SlpName"])
                        });
                    }
                }
                else
                {
                    throw new Exception("No se encontraron datos.");
                }

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = salesManList.Cast<object>().ToList()
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static SyncResponse SyncGetTaxes(Companys company, string _dbObjectViewGetTaxes)
        {
            try
            {
                List<Models.CLVS_POS_GETTAXES> taxes = new List<Models.CLVS_POS_GETTAXES>();
                string sql = string.Format("SELECT * FROM {0}.dbo.{1}", company.DBCode, _dbObjectViewGetTaxes);

                DataTable dt = ExecuteQuery(Common.ReplaceConnectorODBC(company.DBCode), sql);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        taxes.Add(new Models.CLVS_POS_GETTAXES
                        {
                            Code = Convert.ToString(row["Code"]),
                            Rate = Convert.ToString(row["Rate"])
                        });
                    }

                    return new SyncResponse
                    {
                        result = true,
                        rowsToSync = taxes.Cast<object>().ToList()
                    };
                }
                else
                {
                    throw new Exception("No se encontraron datos.");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        internal static DocInfo GetDocNumByTableAndDocEntry(string _table, int _docEntry, CredentialHolder _userCredentials, string _dbObject)
        {
            DocInfo docInfo = null;
            try
            {

                string query = QueryType == "SQL" ?
                string.Format("EXEC {0}.dbo.[{2}] '{1}', '{3}'", _userCredentials.DBCode, _docEntry, _dbObject, _table)
                                  : $"CALL {_userCredentials.DBCode}.{_dbObject}('{_docEntry}', '{_table}')";

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                foreach (DataRow row in dt.Rows)
                {
                    docInfo = new DocInfo
                    {
                        DocNum = Convert.ToInt32(row["DocNum"]),

                    };
                }

                return docInfo;
            }
            catch (Exception e)
            {
                return docInfo;
            }
        }

        /// <summary>
        ///  Metodo que obtiene los pagos realizados a un documento (factura)
        /// </summary>
        /// <param name="_docEntry"></param>
        /// <param name="_dbObjectSpCheckForPayments"></param>
        /// <param name="credentials"></param>
        /// <returns></returns>
        public static InvoiceSapResponse HasPayment(int _docEntry, string _dbObjectSpCheckForPayments, CredentialHolder _credentials)
        {
            try
            {
                InvoiceSapResponse oDocumentModel = new InvoiceSapResponse();

                List<InvoicesListModel> invoicesList = new List<InvoicesListModel>();

                string query = QueryType == "SQL" ?
                 string.Format("EXEC {0}.dbo.{1} '{2}'", _credentials.DBCode, _dbObjectSpCheckForPayments, _docEntry)
                : $"CALL {_credentials.DBCode}.{_dbObjectSpCheckForPayments}('{_docEntry}')";


                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_credentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                oDocumentModel.Result = dt.Rows.Count > 0;
                if (oDocumentModel.Result)
                {
                    oDocumentModel.DocEntry = int.Parse(dt.Rows[0]["DocEntry"].ToString());
                }
                return oDocumentModel;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Funcion que trae la informacion de los tipos de impuestos por compania
        /// </summary>
        /// <param name="company"></param>
        /// <param name="_dbObjectViewGetTaxes"></param>
        /// <returns></returns>
        public static TaxesResponse GetTaxes(CredentialHolder _userCredentials, string _dbObjectViewGetTaxes)
        {
            try
            {
                List<TaxModel> Taxes = new List<TaxModel>();

                string query = QueryType == "SQL" ?
                string.Format("SELECT * FROM {0}.dbo.{1}", _userCredentials.DBCode, _dbObjectViewGetTaxes)
                   : $"SELECT * FROM {_userCredentials.DBCode}.{_dbObjectViewGetTaxes}";
                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        Taxes.Add(new TaxModel
                        {
                            TaxCode = Convert.ToString(row["Code"]),
                            TaxRate = Convert.ToString(row["Rate"])
                        });
                    }

                    return new TaxesResponse
                    {
                        Result = true,
                        Taxes = Taxes
                    };
                }
                else
                {
                    throw new Exception("No se encontraron datos.");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        ///  Funcion que obtiene el docnum a partir del docentry generado por el diapi
        /// </summary>
        /// <param name="dbn"></param>
        /// <param name="docEntry"></param>
        /// <param name="table"></param>
        /// <param name="_dbObjectSpGetNumFEOnline"></param>
        /// <returns></returns>
        public static DocInfo GetDocNumByDocEntry(CredentialHolder _userCredentials, int docEntry, string table, string _dbObjectSpGetNumFEOnline)
        {
            DocInfo docInfo = null;
            try
            {

                string query = QueryType == "SQL" ?
                   string.Format("EXEC {0}.dbo.[{2}] '{1}'", _userCredentials.DBCode, docEntry, _dbObjectSpGetNumFEOnline)
                    : $"CALL {_userCredentials.DBCode}.{_dbObjectSpGetNumFEOnline}('{docEntry}')";


                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                foreach (DataRow row in dt.Rows)
                {
                    docInfo = new DocInfo
                    {
                        DocNum = Convert.ToInt32(row["DocNum"]),
                        NumFE = (Convert.ToString(row["NumFe"]) != null && Convert.ToString(row["NumFe"]) != "") ? Convert.ToString(row["NumFe"]) : "0",
                        ClaveFE = (Convert.ToString(row["ClaveFE"]) != null && Convert.ToString(row["ClaveFE"]) != "") ? Convert.ToString(row["ClaveFE"]) : "0"
                    };
                }

                return docInfo;
            }
            catch (Exception e)
            {
                return docInfo;
            }
        }

        public static DocInfo GetDocNumByDocEntry(CredentialHolder credentials, int docEntry, string _dbObjectSpGetNumFEOnline)
        {
            DocInfo docInfo = null;
            try
            {
                string query = QueryType == "SQL" ?
                  string.Format("EXEC {0}.dbo.[{2}] '{1}'", credentials.DBCode, docEntry, _dbObjectSpGetNumFEOnline)
                   : $"CALL {credentials.DBCode}.{_dbObjectSpGetNumFEOnline}('{docEntry}')";

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(credentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                foreach (DataRow row in dt.Rows)
                {
                    docInfo = new DocInfo
                    {
                        DocNum = Convert.ToInt32(row["DocNum"]),
                        NumFE = (Convert.ToString(row["NumFe"]) != null && Convert.ToString(row["NumFe"]) != "") ? Convert.ToString(row["NumFe"]) : "0",
                        ClaveFE = (Convert.ToString(row["ClaveFE"]) != null && Convert.ToString(row["ClaveFE"]) != "") ? Convert.ToString(row["ClaveFE"]) : "0"
                    };
                }

                return docInfo;
            }
            catch (Exception e)
            {
                return docInfo;
            }
        }




        /// <summary>
        /// Funcion que obtiene el docnum a partir del docentry generado por el diapi
        /// </summary>
        /// <param name="docEntry"></param>
        /// <param name="_dbObjectSpGetDocNumOQUT"></param>
        /// <returns></returns>
        public static DocInfo GetDocNumByDocEntryOQUT(int docEntry, string _dbObjectSpGetDocNumOQUT, CredentialHolder _credentials)
        {
            DocInfo docInfo = null;
            try
            {
                string query = QueryType == "SQL" ?
                   string.Format("EXEC {0}.dbo.[{1}] {2}", _credentials.DBCode, _dbObjectSpGetDocNumOQUT, docEntry)
                    : $"CALL {_credentials.DBCode}.{_dbObjectSpGetDocNumOQUT}('{docEntry}')";

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_credentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                foreach (DataRow row in dt.Rows)
                {
                    docInfo = new DocInfo
                    {
                        DocNum = Convert.ToInt32(row["DocNum"]),
                        NumFE = null,
                        ClaveFE = null
                    };
                }

                return docInfo;
            }
            catch (Exception)
            {
                return docInfo;
            }
        }

        /// <summary>
        /// Funcion que obtiene el docnum a partir del docentry generado por el diapi
        /// </summary>
        /// <param name="dbn"></param>
        /// <param name="docEntry"></param>
        /// <param name="_dbObjectSpGetDocNumORDR"></param>
        /// <returns></returns>
        public static DocInfo GetDocNumByDocEntryORDR(CredentialHolder _userCredentials, int docEntry, string _dbObjectSpGetDocNumORDR)
        {
            DocInfo docInfo = null;
            try
            {
                string query = QueryType == "SQL" ?
                 string.Format("EXEC {0}.dbo.[{1}] {2}", _userCredentials.DBCode, _dbObjectSpGetDocNumORDR, docEntry)
                   : $"CALL {_userCredentials.DBCode}.{_dbObjectSpGetDocNumORDR}('{docEntry}')";

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                foreach (DataRow row in dt.Rows)
                {
                    docInfo = new DocInfo
                    {
                        DocNum = Convert.ToInt32(row["DocNum"]),
                        NumFE = null,
                        ClaveFE = null
                    };
                }

                return docInfo;
            }
            catch (Exception)
            {
                return docInfo;
            }
        }

        /// <summary>
        ///  Funcion que obtiene el docnum a partir del docentry generado por el diapi
        /// </summary>
        /// <param name="dbn"></param>
        /// <param name="docEntry"></param>
        /// <param name="_dbObjectSpGetDocNumORCT"></param>
        /// <returns></returns>
        public static DocInfo GetDocNumByDocEntryORCT(CredentialHolder _userCredentials, int docEntry, string _dbObjectSpGetDocNumORCT)
        {
            DocInfo docInfo = null;
            try
            {
                string query = QueryType == "SQL" ?
                 string.Format("EXEC {0}.dbo." + _dbObjectSpGetDocNumORCT + " {1}", _userCredentials.DBCode, docEntry)
                 : $"CALL {_userCredentials.DBCode}.{_dbObjectSpGetDocNumORCT}('{docEntry}')";

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                foreach (DataRow row in dt.Rows)
                {
                    docInfo = new DocInfo
                    {
                        DocNum = Convert.ToInt32(row["DocNum"]),
                        NumFE = null,
                        ClaveFE = null
                    };
                }

                return docInfo;
            }
            catch (Exception)
            {
                return docInfo;
            }
        }

        /// <summary>
        /// Funcion que llama la coneccion con los datos para conectar a la base de datos de SAP 
        /// y prosesar las ordenes que solicitan eta coneccion
        /// </summary>
        /// <param name = "dbn" ></ param >
        /// < returns ></ returns >
        //public static string Connection(string dbn)
        //{
        //    Companys company = DAO.GetData.GetCompanyByBDCode(dbn);
        //    CompanysSAPModel companyConecct = new CompanysSAPModel();

        //    companyConecct.odbctype = company.SAPConnection.ODBCType;
        //    companyConecct.Server = company.SAPConnection.Server;
        //    companyConecct.User = company.SAPConnection.DBUser;
        //    companyConecct.Pass = company.SAPConnection.DBPass;
        //    companyConecct.ServerType = company.SAPConnection.ServerType;

        //    return COMMON.Common.ReplaceConnectorODBC(dbn);

        //}

        /// <summary>
        ///  Funcion para obtener la cantidad de items disponibles en los almacenes de la compannia
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="company"></param>
        /// <param name="_dbObjectSpGetAvailableItems"></param>
        /// <returns></returns>
        public static WHInfoResponse GetWHAvailableItem(string itemCode, CredentialHolder _userCredentials, string _dbObjectSpGetAvailableItems)
        {
            try
            {
                List<WHInfoModel> items = new List<WHInfoModel>();
                string query = QueryType == "SQL" ?
                  string.Format("EXEC {0}.dbo.{1} '{2}'", _userCredentials.DBCode, _dbObjectSpGetAvailableItems, itemCode)
                    : $"CALL {_userCredentials.DBCode}.{_dbObjectSpGetAvailableItems}('{itemCode}')";


                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                foreach (DataRow row in dt.Rows)
                {
                    WHInfoModel item = new WHInfoModel
                    {
                        WhsCode = Convert.ToString(row["WhsCode"]),
                        WhsName = Convert.ToString(row["WhsName"]),
                        OnHand = Convert.ToDecimal(row["OnHand"]),
                        IsCommited = Convert.ToDecimal(row["IsCommited"]),
                        OnOrder = Convert.ToDecimal(row["OnOrder"]),
                        Disponible = Convert.ToDecimal(row["Disponible"]),
                        Price = Convert.ToDecimal(row["AvgPrice"]),
                        InvntItem = Convert.ToString(row["InvntItem"]),
                    };
                    items.Add(item);
                }

                return new WHInfoResponse
                {
                    Result = true,
                    whInfo = items
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Funcion para obtener las series por almacen de un item de la compannia
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="whsCode"></param>
        /// <param name="company"></param>
        /// <param name="_dbObjectSpSeriesByItem"></param>
        /// <returns></returns>
        public static SeriesResponse GetSeriesByItem(string itemCode, string whsCode, CredentialHolder _userCredentials, string _dbObjectSpSeriesByItem)
        {
            // Este endpoint no se sabe si se usa, el SP no existe en la BD de SAP
            try
            {
                List<SeriesModel> seriesList = new List<SeriesModel>();
                string query = QueryType == "SQL" ?
               string.Format("EXEC {0}.dbo.{1} '{2}','{3}'", _userCredentials.DBCode, _dbObjectSpSeriesByItem, itemCode, whsCode)
                  : $"CALL {_userCredentials.DBCode}.{_dbObjectSpSeriesByItem}('{itemCode}', '{whsCode}')";


                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                foreach (DataRow row in dt.Rows)
                {
                    SeriesModel serie = new SeriesModel
                    {
                        Motor = string.IsNullOrEmpty(Convert.ToString(row["Motor"])) ? "" : Convert.ToString(row["Motor"]),
                        PlacaChasis = string.IsNullOrEmpty(Convert.ToString(row["Placa / Chasis"])) ? "" : Convert.ToString(row["Placa / Chasis"]),
                        Quantity = row["Quantity"] is DBNull ? 0 : Convert.ToDecimal(row["Quantity"]),
                        Comprometido = row["Comprometido"] is DBNull ? 0 : Convert.ToDecimal(row["Comprometido"]),
                        Disponible = row["Disponible"] is DBNull ? 0 : Convert.ToDecimal(row["Disponible"]),
                        Color = string.IsNullOrEmpty(Convert.ToString(row["Color"])) ? "" : Convert.ToString(row["Color"]),
                        Annio = row["Año"] is DBNull ? 0 : Convert.ToInt32(row["Año"]),
                        Ubicacion = string.IsNullOrEmpty(Convert.ToString(row["Ubicacion"])) ? "" : Convert.ToString(row["Ubicacion"]),
                        InDate = Convert.ToDateTime(row["InDate"]),
                        Precio = row["Precio"] is DBNull ? 0 : Convert.ToDecimal(row["Precio"]),
                        Almacen = string.IsNullOrEmpty(Convert.ToString(row["Almacen"])) ? "" : Convert.ToString(row["Almacen"]),
                        SysNumber = string.IsNullOrEmpty(Convert.ToString(row["SysNumber"])) ? "" : Convert.ToString(row["SysNumber"]),
                    };
                    seriesList.Add(serie);
                }

                return new SeriesResponse
                {
                    Result = true,
                    series = seriesList
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// trae las listas de todos los pagos que se deben hacer sobre las facturas
        /// recibe como parametro el cardcode del cliente y la sede
        /// </summary>
        /// <param name="cardCode"></param>
        /// <param name="sede"></param>
        /// <param name="currency"></param>
        /// <param name="company"></param>
        /// <param name="_dbObjectSpGetPayDocuments"></param>
        /// <returns></returns>
        public static InvoicesListResp GetPayInvoices(string cardCode, string sede, string currency, string _dbObjectSpGetPayDocuments, CredentialHolder _credentials)
        {
            try
            {
                List<InvoicesListModel> invoicesList = new List<InvoicesListModel>();
                string query = QueryType == "SQL" ?
                  string.Format("EXEC {0}.dbo.{1} '{2}','{3}','{4}'", _credentials.DBCode, _dbObjectSpGetPayDocuments, cardCode, sede, currency)
                    : $"CALL {_credentials.DBCode}.{_dbObjectSpGetPayDocuments}('{cardCode}', '{sede}', '{currency}')";


                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_credentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        decimal total = 0;
                        decimal balance = 0;

                        if (Convert.ToString(row["DocCur"]) == "COL")
                        {
                            total = Convert.ToDecimal(row["Total"]);
                            balance = Convert.ToDecimal(row["Saldo"]);
                        }
                        else
                        {
                            total = Convert.ToDecimal(row["TotalFC"]);
                            balance = Convert.ToDecimal(row["SaldoFC"]);
                        }
                        invoicesList.Add(new InvoicesListModel
                        {
                            DocNum = Convert.ToInt32(row["DocNum"]),
                            DocEntry = Convert.ToInt32(row["DocEntry"]),
                            Date = Convert.ToDateTime(row["DocDate"]),
                            DocCur = Convert.ToString(row["DocCur"]),
                            type = Convert.ToString(row["Tipo"]),
                            DocDueDate = Convert.ToDateTime(row["DocDueDate"]),
                            DocTotal = total,
                            DocBalance = balance,
                            InstlmntID = Convert.ToInt32(row["InstlmntID"])
                        });
                    }
                }
                else
                {
                    throw new Exception("No se encontraron datos.");
                }

                return new InvoicesListResp
                {
                    Result = true,
                    InvoicesList = invoicesList
                };
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Obtiene el detalle de pago de una factura
        /// </summary>
        /// <param name="company"></param>
        /// <param name="_docEntry"></param>
        /// <param name="_dbObjectSpGetInvoDetail"></param>
        /// <returns></returns>
        public static InvoicePaymentDetailResponse GetInvoicePaymentDetail(CredentialHolder _credentials, int _docEntry, string _dbObjectSpGetInvoDetail)
        {
            try
            {
                List<InvoicesListModel> invoicesList = new List<InvoicesListModel>();

                string query = QueryType == "SQL" ?
                string.Format("EXEC {0}.dbo.{1} '{2}'", _credentials.DBCode, _dbObjectSpGetInvoDetail, _docEntry)
                    : $"CALL {_credentials.DBCode}.{_dbObjectSpGetInvoDetail}('{_docEntry}')";

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_credentials);



                DataSet oDataSet = ExecuteQueryGetMultipleTables(REPLACED_CONNECTION_STRING, query);





                InvoicePaymentDetail oInvoicePaymentDetail = null;
                List<CardModel> oCardModels = null;

                if (oDataSet.Tables.Count > 0)
                {

                    DataTable oDataTable = oDataSet.Tables[0];
                    DataRow oDataRow = oDataTable.Rows[0];
                    oInvoicePaymentDetail = new InvoicePaymentDetail();

                    oInvoicePaymentDetail.CashSum = Convert.ToDouble(oDataRow["CashSum"]);
                    oInvoicePaymentDetail.CashSumFC = Convert.ToDouble(oDataRow["CashSumFC"]);
                    oInvoicePaymentDetail.TrsfrSum = Convert.ToDouble(oDataRow["TrsfrSum"]);
                    oInvoicePaymentDetail.TrsfrSumFC = Convert.ToDouble(oDataRow["TrsfrSumFC"]);
                    if (oDataSet.Tables.Count > 1)
                    {
                        oDataTable = oDataSet.Tables[1];
                        oCardModels = new List<CardModel>();
                        foreach (DataRow row in oDataTable.Rows)
                        {
                            oCardModels.Add(new CardModel
                            {
                                CardValid = Convert.ToDateTime(row["CardValid"]),
                                CreditAcct = Convert.ToString(row["CreditAcct"]),
                                CreditCard = Convert.ToString(row["CreditCard"]),
                                CreditSum = Convert.ToDouble(row["CreditSum"]),
                                FirstDue = Convert.ToDateTime(row["FirstDue"]),
                                LineID = Convert.ToInt32(row["LineID"]),
                                VoucherNum = Convert.ToString(row["VoucherNum"]),
                                IsManulEntry = Convert.ToString(row["IsManualEntry"]).Equals("1")
                            });
                        }
                        oInvoicePaymentDetail.Cards = oCardModels;
                    }
                }
                else
                {
                    throw new Exception("No se encontraron datos.");
                }

                return new InvoicePaymentDetailResponse
                {
                    Result = true,
                    InvoicePaymentDetail = oInvoicePaymentDetail

                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Funcion que trae las listas de las cuentas
        /// </summary>
        /// <param name="company"></param>
        /// <param name="_dbObjectViewGetAccounts"></param>
        /// <returns></returns>
        public static ApiResponse<ContableAccounts> GetAccounts(CredentialHolder _userCredentials, string _dbObjectViewGetAccounts)
        {
            try
            {
                ContableAccounts accounts = new ContableAccounts()
                {
                    CashAccounts = new List<AccountModel>(),
                    TransferAccounts = new List<AccountModel>(),
                    CheckAccounts = new List<AccountModel>()
                };

                string query = string.Empty;
                //List<AccountModel> accountList = new List<AccountModel>();
                query = QueryType == "SQL" ?
                string.Format("EXEC  {0}.dbo.{1} '{2}'", _userCredentials.DBCode, _dbObjectViewGetAccounts, "Cash") //Revisar
                : $"CALL {_userCredentials.DBCode}.{_dbObjectViewGetAccounts}('Cash')";


                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        accounts.CashAccounts.Add(new AccountModel
                        {
                            Account = Convert.ToString(row["Account"]),
                            AccountName = Convert.ToString(row["AccountName"])
                        });
                    }
                }
                else
                {
                    throw new Exception("No existen cuentas contables en efectivo.");
                }

                query = QueryType == "SQL" ?
                string.Format("EXEC  {0}.dbo.{1} '{2}'", _userCredentials.DBCode, _dbObjectViewGetAccounts, "Transfer")
                : $"CALL {_userCredentials.DBCode}.{_dbObjectViewGetAccounts}('Transfer')";

                dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        accounts.TransferAccounts.Add(new AccountModel
                        {
                            Account = Convert.ToString(row["Account"]),
                            AccountName = Convert.ToString(row["AccountName"])
                        });
                    }
                }
                else
                {
                    throw new Exception("No existen cuentas contables para transferencias");
                }

                query = QueryType == "SQL" ?
                string.Format("EXEC  {0}.dbo.{1} '{2}'", _userCredentials.DBCode, _dbObjectViewGetAccounts, "Check")
                : $"CALL {_userCredentials.DBCode}.{_dbObjectViewGetAccounts}('Check')";

                dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        accounts.CheckAccounts.Add(new AccountModel
                        {
                            Account = Convert.ToString(row["Account"]),
                            AccountName = Convert.ToString(row["AccountName"])
                        });
                    }
                }
                else
                {
                    throw new Exception("No existen cuentas contables para chekes.");
                }


                return new ApiResponse<ContableAccounts>
                {
                    Result = true,
                    Data = accounts
                };
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// Trae la informacion de las tarjetas
        /// </summary>
        /// <param name="company"></param>
        /// <param name="_dbObjectViewGetCreditCards"></param>
        /// <returns></returns>
        public static CardsResponse GetCards(CredentialHolder _userCredentials, string _dbObjectViewGetCreditCards)
        {
            try
            {
                List<CardsModel> cardsList = new List<CardsModel>();
                string query = QueryType == "SQL" ?
                  string.Format("SELECT * FROM {0}.dbo.{1}", _userCredentials.DBCode, _dbObjectViewGetCreditCards)
                    : $"SELECT * FROM {_userCredentials.DBCode}.{_dbObjectViewGetCreditCards}";


                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        cardsList.Add(new CardsModel
                        {
                            CardName = Convert.ToString(row["CardName"]),
                            CreditCard = Convert.ToString(row["AcctCode"])
                        });
                    }
                }
                else
                {
                    throw new Exception("No se encontraron datos.");
                }

                return new CardsResponse
                {
                    Result = true,
                    cardsList = cardsList
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Funcion que trae la informacion de las cuentas de los bancos por compannia
        /// </summary>
        /// <param name="company"></param>
        /// <param name="_dbObjectViewGetBanks"></param>
        /// <returns></returns>
        public static BankResponse GetAccountsBank(CredentialHolder _userCredentials, string _dbObjectViewGetBanks)
        {
            try
            {
                List<BankModel> BankList = new List<BankModel>();
                string query = QueryType == "SQL" ?
                   string.Format("SELECT * FROM {0}.dbo.{1}", _userCredentials.DBCode, _dbObjectViewGetBanks)
                    : $"SELECT * FROM {_userCredentials.DBCode}.{_dbObjectViewGetBanks}";

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        BankList.Add(new BankModel
                        {
                            BankCode = Convert.ToString(row["BankCode"]),
                            BankName = Convert.ToString(row["BankName"])
                        });
                    }
                }
                else
                {
                    throw new Exception("No se encontraron datos.");
                }

                return new BankResponse
                {
                    Result = true,
                    banksList = BankList
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        ///  Funcion que trae la informacion de los vendedores por compannia
        /// </summary>
        /// <param name="company"></param>
        /// <param name="_dbObjectViewGetSalesMan"></param>
        /// <returns></returns>
        public static SalesManResponse GetSalesMan(CredentialHolder _userCredentials, string _dbObjectViewGetSalesMan)
        {
            try
            {
                List<SalesManModel> salesManList = new List<SalesManModel>();

                string query = QueryType == "SQL" ?
                string.Format("SELECT * FROM {0}.dbo.{1}", _userCredentials.DBCode, _dbObjectViewGetSalesMan)
                   : $"SELECT * FROM {_userCredentials.DBCode}.{_dbObjectViewGetSalesMan}";

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        salesManList.Add(new SalesManModel
                        {
                            SlpCode = Convert.ToString(row["SlpCode"]),
                            SlpName = Convert.ToString(row["SlpName"])
                        });
                    }
                }
                else
                {
                    throw new Exception("No se encontraron datos.");
                }

                return new SalesManResponse
                {
                    Result = true,
                    salesManList = salesManList
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Obtiene el nombre del consolidador si existe
        /// </summary>
        /// <param name="company"></param>
        /// <param name="cardCode"></param>
        /// <param name="_dbObjectSpGetFatherCard"></param>
        /// <returns></returns>
       

        // Este se deberia mantener apartir de ahora
        public static string GetFatherCard(CredentialHolder _credential, string cardCode, string _dbObjectSpGetFatherCard)
        {
            string fatherCard = string.Empty;
            try
            {
                string query = QueryType == "SQL" ?
                string.Format("EXEC {0}.dbo.{1} '{2}'", _credential.DBCode, _dbObjectSpGetFatherCard, cardCode)
                    : $"SELECT \"FatherCard\" FROM {_credential.DBCode}.OCRD WHERE \"CardCode\" = '{cardCode}'";


                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_credential);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                if (dt.Rows.Count > 0)
                {
                    fatherCard = Convert.ToString(dt.Rows[0]["FatherCard"]);
                }
                return fatherCard;
            }
            catch (Exception)
            {
                return fatherCard;
            }
        }

        /// <summary>
        /// Funcion que obtiene la lista de facturas a las que se le va a canselar el pago desde SAP
        /// </summary>
        /// <param name="company"></param>
        /// <param name="payment"></param>
        /// <param name="_dbObjectSpGetCancelPayment"></param>
        /// <returns></returns>
        public static CancelpaymentResponce GetPaymentList(CredentialHolder _credential, paymentSearchModel payment, string _dbObjectSpGetCancelPayment)
        {
            string fatherCard = string.Empty;
            try
            {
                string query = QueryType == "SQL" ?
                 string.Format("EXEC {0}.dbo.{1} '{2}','{3}','{4}'", _credential.DBCode, _dbObjectSpGetCancelPayment, payment.CardCode, payment.FIni, payment.FFin)
                  : $"CALL {_credential.DBCode}.{_dbObjectSpGetCancelPayment} ('{payment.CardCode}', '{payment.FIni}', '{payment.FFin}')";

                List<CancelPaymentModel> invList = new List<CancelPaymentModel>();
                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_credential);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                foreach (DataRow row in dt.Rows)
                {
                    invList.Add(new CancelPaymentModel
                    {
                        DocNum = Convert.ToInt32(row["DocNum"]),
                        DocEntry = Convert.ToInt32(row["DocEntry"]),
                        DocNumPago = Convert.ToInt32(row["DocNumPago"]),
                        DocDate = Convert.ToString(row["DocDate"]),
                        DocTotal = Convert.ToDouble(row["DocTotal"]),
                        DocTotalFC = Convert.ToDouble(row["DocTotalFC"]),
                        DocCurr = Convert.ToString(row["DocCur"]),
                        CardCode = Convert.ToString(row["CardCode"]),
                        CardName = Convert.ToString(row["CardName"]),
                        Status = Convert.ToString(row["DocStatus"]),
                        Selected = Convert.ToBoolean(row["Selected"]),
                        InvoDocEntry = Convert.ToInt32(row["InvoDocEntry"]),
                        U_CLVS_POS_UniqueInvId = Convert.ToString(row["U_CLVS_POS_UniqueInvId"])
                    });
                }

                return new CancelpaymentResponce
                {
                    Result = true,
                    paymentList = invList
                };
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Funcion que obtiene la lista de facturas para la vista de reimpresion
        /// parametros, un objeto company para la coneccion y un objeto de busqueda de facturas, nombre consulta.
        /// </summary>
        /// <param name="company"></param>
        /// <param name="inv"></param>
        /// <param name="_dbObjectSpGetInvPrintList"></param>
        /// <returns></returns>
        public static InvListPrintResponde GetInvPrintList(CredentialHolder _userCredentials, invPrintSearch inv, string _dbObjectSpGetInvPrintList)
        {
            try
            {
                string query = QueryType == "SQL" ?
                string.Format("EXEC {0}.[dbo].[{6}] '{1}','{2}','{3}','{4}',{5}", _userCredentials.DBCode, inv.slpCode, inv.DocEntry, inv.FechaIni, inv.FechaFin, inv.InvType, _dbObjectSpGetInvPrintList)
                  : $"CALL {_userCredentials.DBCode}.{_dbObjectSpGetInvPrintList}('{inv.slpCode}', '{inv.DocEntry}', '{inv.FechaIni}', '{inv.FechaFin}', '{inv.InvType}')";




                List<InvPrintModel> invList = new List<InvPrintModel>();


                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        invList.Add(new InvPrintModel
                        {
                            DocEntry = Convert.ToInt32(row["DocEntry"]),
                            DocNum = Convert.ToInt32(row["DocNum"]),
                            DocDate = Convert.ToString(row["DocDate"]),
                            CardName = Convert.ToString(row["CardName"]),
                            DocStatus = Convert.ToString(row["DocStatus"]),
                            IsManualEntry = Convert.ToBoolean(row["IsManualEntry"].ToString().Equals("1") ? "true" : "false"),
                            InvoiceNumber = Convert.ToString(row["InvoiceNumber"].ToString()),
                            DocCur = Convert.ToString(row["DocCur"]),
                            DocTotal = Convert.ToDouble(row["DocTotal"]),

                        });
                    }
                }

                return new InvListPrintResponde
                {
                    Result = true,
                    invList = invList
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Trae lista de fabricantes
        /// </summary>
        /// <param name="company"></param>
        /// <param name="_dbObjectViewGetFirmsList"></param>
        /// <returns></returns>
        public static List<ItemFirmModel> GetFirms(CredentialHolder _userCredentials, string _dbObjectViewGetFirmsList)
        {
            List<ItemFirmModel> marcas = new List<ItemFirmModel>();
            try
            {
                string query = QueryType == "SQL" ?
                string.Format("SELECT * FROM {0}.dbo." + _dbObjectViewGetFirmsList, _userCredentials.DBCode)
                  : $"SELECT * FROM {_userCredentials.DBCode}.{_dbObjectViewGetFirmsList}";

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                foreach (DataRow row in dt.Rows)
                {
                    marcas.Add(new ItemFirmModel
                    {
                        FirmCode = Convert.ToString(row["FirmCode"]),
                        FirmName = Convert.ToString(row["FirmName"])
                    });
                }

                return marcas;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        ///  Funcion que obtiene el group num de los items
        /// </summary>
        /// <param name="company"></param>
        /// <param name="_dbObjectViewGetItemGroupList"></param>
        /// <returns></returns>
        public static List<ItemGroupModel> GetGroupNums(CredentialHolder _userCredentials, string _dbObjectViewGetItemGroupList)
        {
            List<ItemGroupModel> grupos = new List<ItemGroupModel>();
            try
            {
                string query = QueryType == "SQL" ?
                string.Format("SELECT * FROM {0}.dbo." + _dbObjectViewGetItemGroupList, _userCredentials.DBCode)
                  : $"SELECT * FROM {_userCredentials.DBCode}.{_dbObjectViewGetItemGroupList}";


                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                foreach (DataRow row in dt.Rows)
                {
                    grupos.Add(new ItemGroupModel
                    {
                        GroupCode = Convert.ToString(row["ItmsGrpCod"]),
                        GroupName = Convert.ToString(row["ItmsGrpNam"])
                    });
                }

                return grupos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Funcion que retorna una lista con todos las listas de precios
        /// </summary>
        /// <param name="company"></param>
        /// <param name="_dbObjectViewPriceList"></param>
        /// <returns></returns>
        public static PriceListResponse GetPriceList(CredentialHolder _userCredentials, string _dbObjectViewPriceList)
        {
            try
            {
                List<PriceListModel> PriceList = new List<PriceListModel>();

                string query = QueryType == "SQL" ?
                string.Format("SELECT * FROM {0}.[dbo].[{1}]", _userCredentials.DBCode, _dbObjectViewPriceList)
                   : $"SELECT * FROM {_userCredentials.DBCode}.{_dbObjectViewPriceList}";

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                foreach (DataRow row in dt.Rows)
                {
                    PriceList.Add(new PriceListModel
                    {
                        ListNum = Convert.ToInt32(row["ListNum"]),
                        ListName = Convert.ToString(row["ListName"]),
                        PrimCurr = Convert.ToString(row["PrimCurr"]),
                        AddCurr1 = Convert.ToString(row["AddCurr1"]),
                        AddCurr2 = Convert.ToString(row["AddCurr2"])
                    });
                };

                return new PriceListResponse { Result = true, priceList = PriceList };
            }
            catch (Exception exc)
            {
                return (PriceListResponse)LogManager.HandleExceptionWithReturn(exc, "PriceListResponse");
            }
        }

        public static PriceListSelfResponse GetDefaultPriceList(CredentialHolder _userCredentials, string _cardCode, string _dbObjectSpGetDefaultPriceList)
        {
            try
            {
                List<PriceListModel> PriceList = new List<PriceListModel>();

                string query = QueryType == "SQL" ?
                    string.Format("EXEC {0}.dbo." + _dbObjectSpGetDefaultPriceList + " '{1}'", _userCredentials.DBCode, _cardCode)
                    : $"CALL {_userCredentials.DBCode}.{_dbObjectSpGetDefaultPriceList}('{_cardCode}')";

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                PriceListSelfResponse oPriceListSelfResponse = new PriceListSelfResponse
                {
                    Result = dt.Rows.Count > 0
                };

                if (oPriceListSelfResponse.Result)
                {
                    DataRow oDataRow = dt.Rows[0];
                    oPriceListSelfResponse.PriceList = new PriceListModel
                    {
                        ListNum = Convert.ToInt32(oDataRow["ListNum"]),
                        ListName = Convert.ToString(oDataRow["ListName"])
                    };
                }

                return oPriceListSelfResponse;
            }
            catch (Exception exc)
            {
                return (PriceListSelfResponse)LogManager.HandleExceptionWithReturn(exc, "PriceListSelfResponse");
            }
        }

        /// <summary>
        /// Funcion que retorna una lista con todos los terminos de pago
        /// </summary>
        /// <param name="company"></param>
        /// <param name="_dbObjectViewGetPayTerms"></param>
        /// <returns></returns>
        public static PayTermsResponse GetPayTermsList(CredentialHolder _userCredentials, string _dbObjectViewGetPayTerms)
        {
            try
            {
                List<PayTermsModel> PayTermsList = new List<PayTermsModel>();

                string query = QueryType == "SQL" ?
                  string.Format("SELECT * FROM {0}.dbo." + _dbObjectViewGetPayTerms, _userCredentials.DBCode)
                    : $"SELECT * FROM {_userCredentials.DBCode}.{_dbObjectViewGetPayTerms}";

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                foreach (DataRow row in dt.Rows)
                {
                    PayTermsList.Add(new PayTermsModel
                    {
                        GroupNum = Convert.ToInt32(row["GroupNum"]),
                        PymntGroup = Convert.ToString(row["PymntGroup"]),
                        Type = Convert.ToInt32(row["Type"])
                    });
                };

                return new PayTermsResponse { Result = true, payTermsList = PayTermsList };
            }
            catch (Exception exc)
            {
                return (PayTermsResponse)LogManager.HandleExceptionWithReturn(exc, "PayTermsResponse");
            }
        }

        /// <summary>
        /// Funcion que retorna una lista con todos los almacenes
        /// </summary>
        /// <param name="company"></param>
        /// <param name="_bdbObjectViewWarehouses"></param>
        /// <returns></returns>
        public static WHPlaceResponce GetStoresList(CredentialHolder _userCredentials, string _bdbObjectViewWarehouses)
        {
            try
            {
                List<WHplaceModel> StoresList = new List<WHplaceModel>();
                string query = QueryType == "SQL" ?
                string.Format("SELECT * FROM {0}.dbo.[{1}]", _userCredentials.DBCode, _bdbObjectViewWarehouses)
                   : $"SELECT * FROM {_userCredentials.DBCode}.{_bdbObjectViewWarehouses}";


                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                foreach (DataRow row in dt.Rows)
                {
                    StoresList.Add(new WHplaceModel
                    {
                        WhsName = Convert.ToString(row["WhsName"]),
                        WhsCode = Convert.ToString(row["WhsCode"])
                    });
                };

                return new WHPlaceResponce { Result = true, WHPlaceList = StoresList };
            }
            catch (Exception exc)
            {
                return (WHPlaceResponce)LogManager.HandleExceptionWithReturn(exc, "WHPlaceResponce");
            }
        }

        //cierre de caja por usuario o por hora
        public static BalanceByUserResponse GetBalanceInvoices_UsrOrTime(GetBalanceModel_UsrOrDate BalanceModel, string _dbObjectSpUsrBalanceCreditNotes, string _dbObjectSpUsrBalance, CredentialHolder _userCredentials)
        {
            try
            {
                string StartHr = string.Empty;
                string EndHr = string.Empty;
                List<BalanceByUserDetails> Balance = new List<BalanceByUserDetails>();
                StartHr = BalanceModel.FIni.ToString("yyyy/MM/dd HH:mm:ss");
                EndHr = BalanceModel.FFin.ToString("yyyy/MM/dd HH:mm:ss");

                // obtiene las notas de credito
                List<BalanceByUserDetailsCN> CreditNotesList = new List<BalanceByUserDetailsCN>();

                string query = QueryType == "SQL" ?
               string.Format("EXEC {0}.dbo.{3} '{1}','{2}'", _userCredentials.DBCode, StartHr, EndHr, _dbObjectSpUsrBalanceCreditNotes)
                   : $"CALL {_userCredentials.DBCode}.{_dbObjectSpUsrBalanceCreditNotes}('{StartHr}', '{EndHr}')";

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);
                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        CreditNotesList.Add(new BalanceByUserDetailsCN
                        {
                            DocDate = Convert.ToString(row["DocDate"]),
                            DocNumP = Convert.ToString(row["DocNumP"]),
                            DocNumF = Convert.ToString(row["DocNumF"]),
                            DocEntry = Convert.ToInt32(row["DocEntF"]),
                            DocCur = Convert.ToString(row["DocCur"]),
                            CardName = Convert.ToString(row["CardName"]),
                            Type = Convert.ToString(row["PayType"]),
                            Balance = Convert.ToDecimal(row["Balance"]),
                            PayTotal = Convert.ToDecimal(row["PayTotal"]),
                            CashSum = Convert.ToDecimal(row["CashSum"]),
                            CreditSum = Convert.ToDecimal(row["CreditSum"]),
                            CheckSum = Convert.ToDecimal(row["CheckSum"]),
                            TrsfrSum = Convert.ToDecimal(row["TrsfrSum"]),
                            CashSumFC = Convert.ToDecimal(row["CashSumFC"]),
                            CredSumFC = Convert.ToDecimal(row["CredSumFC"]),
                            CheckSumFC = Convert.ToDecimal(row["CheckSumFC"]),
                            TrsfrSumFC = Convert.ToDecimal(row["TrsfrSumFC"]),
                            TotalDoc = Convert.ToDouble(row["TotalDoc"])
                        });
                    }
                }
                //28-12-2021
                query = QueryType == "SQL" ?
                  string.Format("EXEC {0}.dbo.{4} '{1}','{2}',{3}", _userCredentials.DBCode, StartHr, EndHr, BalanceModel.User, _dbObjectSpUsrBalance)
                    : $"CALL {_userCredentials.DBCode}.{_dbObjectSpUsrBalance}('{StartHr}', '{EndHr}', '{BalanceModel.User}')";

                dt.Rows.Clear();
                dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        Balance.Add(new BalanceByUserDetails
                        {
                            DocDate = Convert.ToString(row["DocDate"]),
                            DocNumP = Convert.ToString(row["DocNumP"]),
                            DocNumF = Convert.ToString(row["DocNumF"]),
                            DocEntry = Convert.ToInt32(row["DocEntF"]),
                            DocCur = Convert.ToString(row["DocCur"]),
                            CardName = Convert.ToString(row["CardName"]),
                            Type = Convert.ToString(row["PayType"]),
                            Balance = Convert.ToDecimal(row["Balance"]),
                            PayTotal = Convert.ToDecimal(row["PayTotal"]),
                            CashSum = Convert.ToDecimal(row["CashSum"]),
                            CreditSum = Convert.ToDecimal(row["CreditSum"]),
                            CheckSum = Convert.ToDecimal(row["CheckSum"]),
                            TrsfrSum = Convert.ToDecimal(row["TrsfrSum"]),
                            CashSumFC = Convert.ToDecimal(row["CashSumFC"]),
                            CredSumFC = Convert.ToDecimal(row["CredSumFC"]),
                            CheckSumFC = Convert.ToDecimal(row["CheckSumFC"]),
                            TrsfrSumFC = Convert.ToDecimal(row["TrsfrSumFC"]),
                            TotalDoc = Convert.ToDouble(row["TotalDoc"])
                        });
                    }
                }
                else { throw new Exception("No se encontraron datos."); }
                return new BalanceByUserResponse
                {
                    Result = true,
                    UsrBalance = Balance,
                    CreditNotes = CreditNotesList.Count > 0 ? CreditNotesList : null
                };
            }
            catch (Exception exc)
            {
                return (BalanceByUserResponse)LogManager.HandleExceptionWithReturn(exc, "BalanceByUserResponse");
            }
        }

        // consulta si el codigo unico de una factura ya esta creado en SAP
        public static bool CheckUniqueInvId(string UniqueInvId, CredentialHolder _userCredentials, string _dbObjectSpGetCheckUniqueInvId)
        {
            try
            {
                List<InvoicesListModel> invoicesList = new List<InvoicesListModel>();

                string query = QueryType == "SQL" ?
              string.Format("EXEC {0}.dbo." + _dbObjectSpGetCheckUniqueInvId + " '{1}'", _userCredentials.DBCode, UniqueInvId)
                    : $"CALL {_userCredentials.DBCode}.{_dbObjectSpGetCheckUniqueInvId}('{UniqueInvId}')";

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                if (dt.Rows.Count > 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }



        // Verifica si el ID unico existe segun la tabla que se envia 
        public static DocInfo CheckUniqueDocumentID(string _table, string _uniqueID, CredentialHolder _userCredentials, string _dbObject)
        {
            DocInfo docInfo = null;
            try
            {

             string query = QueryType == "SQL" ?
             string.Format("EXEC {0}.dbo." + _dbObject + " '{1}', '{2}'", _userCredentials.DBCode, _uniqueID, _table)   
             : $"CALL {_userCredentials.DBCode}.{_dbObject}('{_uniqueID}','{_table}')";


                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);


                if (dt != null && dt.Rows.Count > 0)
                {
                    docInfo = new DocInfo
                    {
                        DocEntry = Convert.ToInt32(dt.Rows[0]["DocEntry"]),
                        DocNum = Convert.ToInt32(dt.Rows[0]["DocNum"])
                    };
                }

                return docInfo;
            }
            catch (Exception e)
            {
                return docInfo;
            }
        }
















        //onsulta si el codigo unico de una factura ya existe en SAP y retorna info del documento
        public static InvoiceSapResponse CheckUniqueInv(string UniqueInvId, string _dbObjectSpGetCheckUniqueInvIdReturnInfo, CredentialHolder credentials)
        {
            try
            {

                InvoiceSapResponse oDocumentModel = new InvoiceSapResponse();

                List<InvoicesListModel> invoicesList = new List<InvoicesListModel>();

               string query = QueryType == "SQL" ?
               string.Format("EXEC {0}.dbo." + _dbObjectSpGetCheckUniqueInvIdReturnInfo + " '{1}'", credentials.DBCode, UniqueInvId)
                   : $"CALL {credentials.DBCode}.{_dbObjectSpGetCheckUniqueInvIdReturnInfo}('{UniqueInvId}')";

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(credentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                oDocumentModel.Result = dt.Rows.Count > 0;
                if (oDocumentModel.Result)
                {
                    oDocumentModel.DocEntry = Convert.ToInt32(dt.Rows[0]["DocEntry"]);
                    oDocumentModel.DocNum = Convert.ToInt32(dt.Rows[0]["DocNum"]);
                }

                return oDocumentModel;
            }
            catch
            {
                throw;
            }
        }






        /// <summary>
        /// Obtiene las listas de precios asociadas a un item, ademas de la informacion propia de cada lista
        /// </summary>
        /// <param name="_itemCode"></param>
        /// <param name="company"></param>
        /// <param name="_dbObjectSpGetItemList"></param>
        /// <returns></returns>
        public static ItemsResponse GetItemPriceList(string _itemCode, CredentialHolder _userCredentials, string _dbObjectSpGetItemList)
        {
            try
            {
                ItemsModel item = new ItemsModel();
                string query = QueryType == "SQL" ?
                  string.Format("EXEC {0}.dbo.{1} '{2}'", _userCredentials.DBCode, _dbObjectSpGetItemList, _itemCode)
                    : $"CALL {_userCredentials.DBCode}.{_dbObjectSpGetItemList}('{_itemCode}')";


                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                if (dt.Rows.Count > 0)
                {
                    var plm = new List<PriceListModel>(); // plm = PriceListModel
                    foreach (DataRow row in dt.Rows)
                    {
                        plm.Add(new PriceListModel
                        {
                            ListName = row["ListName"].ToString(),
                            ListNum = int.Parse(string.IsNullOrEmpty(Convert.ToString(row["ListNum"])) ? "0" : Convert.ToString(row["ListNum"])),
                            Price = double.Parse(string.IsNullOrEmpty(Convert.ToString(row["Price"])) ? "0" : Convert.ToString(row["Price"]))
                        });
                    }

                    item = new ItemsModel
                    {
                        PriceList = plm
                    };

                    return new ItemsResponse
                    {
                        Result = true,
                        Item = item
                    };
                }
                else
                {
                    throw new Exception("No existe informacion del item en SAP.");
                }
            }
            catch (Exception exc)
            {
                throw exc;
                //return (ItemsResponse)LogManager.HandleExceptionWithReturn(exc, "ItemsResponse");
            }
        }

        /// <summary>
        /// Obtiene todos los codigos de barras que pertenecen a un item
        /// </summary>
        /// <param name="_itemCode"></param>
        /// <param name="company"></param>
        /// <param name="_dbObjecSpGetBarCodeByItem"></param>
        /// <returns></returns>
        public static ItemsResponse GetBarcodesByItem(string _itemCode, CredentialHolder _userCredentials, string _dbObjecSpGetBarCodeByItem)
        {
            try
            {
                ItemsModel item = new ItemsModel();
                string query = QueryType == "SQL" ?
                string.Format("EXEC {0}.dbo.{1} '{2}'", _userCredentials.DBCode, _dbObjecSpGetBarCodeByItem, _itemCode)
               : $"CALL {_userCredentials.DBCode}.{_dbObjecSpGetBarCodeByItem}('{_itemCode}')";
            

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                if (dt.Rows.Count > 0)
                {
                    var plm = new List<ItemsBarcodeModel>(); // plm = PriceListModel
                    foreach (DataRow row in dt.Rows)
                    {
                        plm.Add(new ItemsBarcodeModel
                        {
                            BcdEntry = int.Parse(row["BcdEntry"].ToString()),
                            BcdCode = row["BcdCode"].ToString(),
                            BcdName = row["BcdName"].ToString(),
                            UomEntry = int.Parse(row["UomEntry"].ToString())
                        });
                    }

                    item = new ItemsModel
                    {
                        Barcodes = plm
                    };

                    return new ItemsResponse
                    {
                        Result = true,
                        Item = item
                    };
                }
                else
                {
                    throw new Exception("No existe informacion del item en SAP.");
                }
            }
            catch (Exception exc)
            {
                return (ItemsResponse)LogManager.HandleExceptionWithReturn(exc, "ItemsResponse");
            }
        }

        /// <summary>
        /// Obtiene la informacion del item basado en su codigo de barras
        /// </summary>
        /// <param name="_itemBarcode"></param>
        /// <param name="company"></param>
        /// <param name="_dbObjectSpGetItemByCodeBar"></param>
        /// <returns></returns>
        public static ItemsResponse GetItemByBarcode(string _itemBarcode, string _dbObjectSpGetItemByCodeBar, CredentialHolder _UserCredentials)
        {
            try
            {
                ItemsModel item = new ItemsModel();            
                string query = QueryType == "SQL" ?
                string.Format("EXEC {0}.dbo.{1} '{2}'", _UserCredentials.DBCode, _dbObjectSpGetItemByCodeBar, _itemBarcode)
                : $"CALL {_UserCredentials.DBCode}.{_dbObjectSpGetItemByCodeBar}('{_itemBarcode}')";

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_UserCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);


                if (dt.Rows.Count > 0)
                {
                    var plm = new List<ItemsBarcodeModel>(); // plm = PriceListModel
                    foreach (DataRow row in dt.Rows)
                    {
                        plm.Add(new ItemsBarcodeModel
                        {
                            BcdEntry = int.Parse(row["BcdEntry"].ToString()),
                            BcdCode = row["BcdCode"].ToString(),
                            BcdName = row["ItemName"].ToString(),
                            UomEntry = int.Parse(row["UomEntry"].ToString())
                        });

                        item = new ItemsModel
                        {
                            ItemCode = row["ItemCode"].ToString(),
                            ItemName = row["ItemName"].ToString(),
                            Barcodes = plm
                        };

                        return new ItemsResponse
                        {
                            Result = true,
                            Item = item
                        };
                    }

                }
                return new ItemsResponse
                {
                    Result = false,
                    Item = null
                };
            }
            catch (Exception exc)
            {
                return (ItemsResponse)LogManager.HandleExceptionWithReturn(exc, "ItemsResponse");
            }
        }

        public static CustomerResponseModel GetCustomer(CredentialHolder _userCredentials, string _dbObjectSpGetBP)
        {
            try
            {              
                   string query = QueryType == "SQL" ?
                  string.Format("EXEC {0}.dbo.{1}", _userCredentials.DBCode, _dbObjectSpGetBP)
                    : $"CALL {_userCredentials.DBCode}.{_dbObjectSpGetBP}()";
                CustomerResponseModel responseCustomer = new CustomerResponseModel
                {
                    Result = false
                };
                List<GetCustomerModel> bpList = new List<GetCustomerModel>();

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                if (dt.Rows != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        GetCustomerModel bp = new GetCustomerModel
                        {
                            CardCode = Convert.ToString(row["CardCode"]),
                            CardName = Convert.ToString(row["CardName"]),
                            Phone1 = Convert.ToString(row["Phone1"]),
                            LicTradNum = Convert.ToString(row["LicTradNum"]),
                            CardType = Convert.ToString(row["CardType"]),
                            E_Mail = Convert.ToString(row["E_Mail"]),
                            U_TipoIdentificacion = Convert.ToString(row["U_TipoIdentificacion"]),
                            U_provincia = Convert.ToString(row["U_provincia"]),
                            U_canton = Convert.ToString(row["U_canton"]),
                            U_distrito = Convert.ToString(row["U_distrito"]),
                            U_barrio = Convert.ToString(row["U_barrio"]),
                            U_direccion = Convert.ToString(row["U_direccion"])
                        };
                        bpList.Add(bp);
                    }
                }
                else
                {
                    responseCustomer.Result = false;
                    responseCustomer.Error = new ErrorInfo
                    {
                        Message = "No se encontraron Clientes..."
                    };
                }
                responseCustomer.Result = true;
                responseCustomer.Customer = bpList;
                return responseCustomer;
            }
            catch (Exception exc)
            {
                return (CustomerResponseModel)LogManager.HandleExceptionWithReturn(exc, "CustomerResponseModel");
            }
        }
        /// <summary>
        /// Obtener de Sap los Clientes
        /// </summary>
        /// <param name="company"></param>
        /// <param name="CardCode"></param>
        /// <param name="_dbObjectSpGetBPByCode"></param>
        /// <returns></returns>
        public static CustomerResponseModel GetCustomerbyCode(CredentialHolder _userCredentials, string CardCode, string _dbObjectSpGetBPByCode)
        {
            try
            {
                string query = QueryType == "SQL" ?
                  string.Format("EXEC {0}.dbo.{1} '{2}'", _userCredentials.DBCode, _dbObjectSpGetBPByCode, CardCode)
                    : $"CALL {_userCredentials.DBCode}.{_dbObjectSpGetBPByCode}('{CardCode}')";
               
                CustomerResponseModel responseCustomer = new CustomerResponseModel
                {
                    Result = false
                };
                List<GetCustomerModel> bpList = new List<GetCustomerModel>();

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);
                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                if (dt.Rows != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        GetCustomerModel bp = new GetCustomerModel
                        {
                            CardCode = CardCode,
                            CardName = Convert.ToString(row["CardName"]),
                            Phone1 = Convert.ToString(row["Phone1"]),
                            LicTradNum = Convert.ToString(row["LicTradNum"]),
                            CardType = Convert.ToString(row["CardType"]),
                            E_Mail = Convert.ToString(row["E_Mail"]),
                            U_TipoIdentificacion = Convert.ToString(row["U_TipoIdentificacion"]),
                            U_provincia = Convert.ToString(row["U_provincia"]),
                            U_canton = Convert.ToString(row["U_canton"]),
                            U_distrito = Convert.ToString(row["U_distrito"]),
                            U_barrio = Convert.ToString(row["U_barrio"]),
                            U_direccion = Convert.ToString(row["U_direccion"])
                        };
                        bpList.Add(bp);
                    }
                }
                else
                {
                    responseCustomer.Result = false;
                    responseCustomer.Error = new ErrorInfo
                    {
                        Message = "No se encontraron Clientes..."
                    };
                }
                responseCustomer.Result = true;
                responseCustomer.Customer = bpList;
                return responseCustomer;
            }
            catch (Exception exc)
            {
                return (CustomerResponseModel)LogManager.HandleExceptionWithReturn(exc, "CustomerResponseModel");
            }
        }
        //Consulta si el codigo unico de una factura ya esta creado en Sap
        public static bool CheckUniqueInvIdSupplier(string UniqueInvId, CredentialHolder _userCredentials, string _dbObjectSpGetCheckUniqueInvIdSup)
        {

            try
            {
                List<InvoicesListModel> invoicesList = new List<InvoicesListModel>();
                string query = QueryType == "SQL" ?
                 string.Format("EXEC {0}.dbo." + _dbObjectSpGetCheckUniqueInvIdSup + " '{1}'", _userCredentials.DBCode, UniqueInvId)
                    : $"CALL {_userCredentials.DBCode}.{_dbObjectSpGetCheckUniqueInvIdSup}('{UniqueInvId}')";
            

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);
                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                if (dt.Rows.Count > 0)
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Funcion que obtiene el docnum a partir del docentry generado por el diapi
        /// </summary>
        /// <param name="dbn"></param>
        /// <param name="docEntry"></param>
        /// <param name="table"></param>
        /// <param name="_dbObjectSpNumApInv"></param>
        /// <returns></returns>
        public static DocInfo GetDocNumByDocEntryApInvoice(CredentialHolder _userCredentials, int docEntry, string table, string _dbObjectSpNumApInv)
        {
            DocInfo docInfo = null;
            try
            {
                //string sql = string.Format("SELECT DocNum, U_NumFE, U_ClaveFE FROM {0}.dbo.{1} WHERE DocEntry = '{2}'", dbn, table, docEntry);
                //string sql = string.Format("EXE dbo.CLVS_POS_GETNUMFEONLINE_SPR {0}", docEntry);
                //SqlString = string.Format("EXEC " + company.DBCode + ".dbo." + company.GetIncPaymentsSP + " '" + ds + "'");
               
                string query = QueryType == "SQL" ?
                 string.Format("EXEC {0}.dbo." + _dbObjectSpNumApInv + " '{1}'", _userCredentials.DBCode, docEntry)
                  : $"CALL {_userCredentials.DBCode}.{_dbObjectSpNumApInv}('{docEntry}')";

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);
                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                foreach (DataRow row in dt.Rows)
                {
                    docInfo = new DocInfo
                    {
                        DocNum = Convert.ToInt32(row["DocNum"])
                    };
                }

                return docInfo;
            }
            catch (Exception)
            {
                return docInfo;
            }
        }

        /// <summary>
        /// Trae lista facturas proveedores pendientes de pago
        /// </summary>
        /// <param name="cardCode"></param>
        /// <param name="sede"></param>
        /// <param name="currency"></param>
        /// <param name="company"></param>
        /// <param name="_dbObjectSpGetPayDocumentsSupplier"></param>
        /// <returns></returns>
        public static InvoicesListResp GetPayApInvoices(string cardCode, string sede, string currency, CredentialHolder _credentials, string _dbObjectSpGetPayDocumentsSupplier)
        {
            try
            {
                List<InvoicesListModel> invoicesList = new List<InvoicesListModel>();
              
                string query = QueryType == "SQL" ?
                string.Format("EXEC {0}.dbo." + _dbObjectSpGetPayDocumentsSupplier + " '{1}','{2}','{3}'", _credentials.DBCode, cardCode, sede, currency)
                  : $"CALL {_credentials.DBCode}.{_dbObjectSpGetPayDocumentsSupplier}('{cardCode}', '{sede}', '{currency}')";

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_credentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        decimal total = 0;
                        decimal balance = 0;

                        if (Convert.ToString(row["DocCur"]) == "COL")
                        {
                            total = Convert.ToDecimal(row["Total"]);
                            balance = Convert.ToDecimal(row["Saldo"]);
                        }
                        else
                        {
                            total = Convert.ToDecimal(row["TotalFC"]);
                            balance = Convert.ToDecimal(row["SaldoFC"]);
                        }
                        invoicesList.Add(new InvoicesListModel
                        {
                            DocNum = Convert.ToInt32(row["DocNum"]),
                            DocEntry = Convert.ToInt32(row["DocEntry"]),
                            Date = Convert.ToDateTime(row["DocDate"]),
                            DocCur = Convert.ToString(row["DocCur"]),
                            type = Convert.ToString(row["Tipo"]),
                            DocDueDate = Convert.ToDateTime(row["DocDueDate"]),
                            DocTotal = total,
                            DocBalance = balance,
                            Customer = Convert.ToString(row["CardName"])
                            // NumAtCard = Convert.ToString(row["NumAtCard"]),
                            // InstlmntID = Convert.ToInt32(row["InstlmntID"])
                        });
                    }
                }
                else
                {
                    throw new Exception("No se encontraron datos.");
                }

                return new InvoicesListResp
                {
                    Result = true,
                    InvoicesList = invoicesList
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Funcion que retorna una lista con todos las listas de precios
        /// </summary>
        /// <param name="company"></param>
        /// <param name="_dbObjetSpGetAllPriceList"></param>
        /// <returns></returns>
        public static PriceListResponse GetAllPriceList(CredentialHolder _userCredentials, string _dbObjetSpGetAllPriceList)
        {
            try
            {
                List<PriceListModel> PriceList = new List<PriceListModel>();
                string query = QueryType == "SQL" ?
                 string.Format("SELECT * FROM {0}.dbo." + _dbObjetSpGetAllPriceList, _userCredentials.DBCode)
                  : $"SELECT * FROM {_userCredentials.DBCode}.{_dbObjetSpGetAllPriceList}";
                

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);
                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                foreach (DataRow row in dt.Rows)
                {
                    PriceList.Add(new PriceListModel
                    {
                        ListNum = Convert.ToInt32(row["ListNum"]),
                        ListName = Convert.ToString(row["ListName"])
                    });
                };

                return new PriceListResponse { Result = true, priceList = PriceList };
            }
            catch (Exception exc)
            {
                return (PriceListResponse)LogManager.HandleExceptionWithReturn(exc, "PriceListResponse");
            }
        }

        /// <summary>
        /// Obtener precios de los item de acuerdo a la lista de precio
        /// </summary>
        /// <param name="itemModel"></param>
        /// <param name="discount"></param>
        /// <param name="company"></param>
        /// <param name="_dbObjectSpGetItemInfo"></param>
        /// <returns></returns>
        public static ItemsResponse GetItemChangePrice(ItemsChangePriceModel itemModel, decimal discount, CredentialHolder _userCredentials, string _dbObjectSpGetItemInfo)
        {
            try
            {
                string sql;
                ItemsChangeResponse itemChange = new ItemsChangeResponse
                {
                    Result = false
                };
                List<ItemsModel> ItemList = new List<ItemsModel>();

                foreach (var line in itemModel.ItemsList)
                {
                    
                    string query = QueryType == "SQL" ?
                    string.Format("EXEC {0}.dbo." + _dbObjectSpGetItemInfo + " '{1}','{2}','{3}'", _userCredentials.DBCode, line.ItemCode, discount, itemModel.priceList)
                   : $"CALL {_userCredentials.DBCode}.{_dbObjectSpGetItemInfo}('{line.ItemCode}', '{discount}', '{itemModel.priceList}')";
                    // ItemsModel item = new ItemsModel();
                    //string sql = string.Format("EXEC {0}.dbo.[CLVS_POS_GETITEM_MD_SPR] '{1}','{2}','{3}'", company.DBCode, itemModel.ItemsList, discount, itemModel.priceList);

                    string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);
                    DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                    if (dt.Rows != null && dt.Rows.Count > 0)
                    {
                        // foreach (DataRow row in dt.Rows)
                        //  { 

                        ItemsModel bp = new ItemsModel
                        {
                            ItemCode = Convert.ToString(dt.Rows[0]["ItemCode"]),
                            //OnHand = Convert.ToInt32(dt.Rows[0]["OnHand"]),
                            ItemName = Convert.ToString(dt.Rows[0]["ItemName"]),
                            InvntItem = Convert.ToString(dt.Rows[0]["InvntItem"]),
                            Discount = dt.Rows[0]["U_Discount"] != null && dt.Rows[0]["U_Discount"].ToString() != "" ? Convert.ToDecimal(dt.Rows[0]["U_Discount"]) : 0,
                            TaxCode = string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["TaxCode"])) ? "" : Convert.ToString(dt.Rows[0]["TaxCode"]),
                            TaxRate = string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["TaxRate"])) ? 0 : Convert.ToDouble(dt.Rows[0]["TaxRate"]),
                            UnitPrice = dt.Rows[0]["UnitPrice"] != null && dt.Rows[0]["UnitPrice"].ToString() != "" ? Convert.ToDecimal(dt.Rows[0]["UnitPrice"]) : 0,
                            FirmName = string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["PreferredVendor"])) ? "" : Convert.ToString(dt.Rows[0]["PreferredVendor"])

                        };
                        ItemList.Add(bp);
                    }
                    else
                    {
                        throw new Exception("No existe informacion del item en SAP.");
                    }

                };

                return new ItemsResponse
                {
                    Result = true,
                    ItemList = ItemList
                };
            }
            catch (Exception exc)
            {
                return (ItemsResponse)LogManager.HandleExceptionWithReturn(exc, "ItemsResponse");
            }
        }



        #region PurchaseOrder
        public static PurchaserOrdersResponse GetPurchaseOrderList(CredentialHolder _userCredentials, PurchaseOrderSearchModel purchaseorder, string _dbObjectSpGetPurchaseOrderList)
        {
            string fatherCard = string.Empty;
            try
            {
                string sql = string.Format("EXEC {0}.dbo.[{1}]'{2}','{3}','{4}'", _userCredentials.DBCode, _dbObjectSpGetPurchaseOrderList, purchaseorder.CardCode, purchaseorder.FIni, purchaseorder.FFin);

                List<PurchaseOrderModel> orderList = new List<PurchaseOrderModel>();

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);
                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, sql);

                LogManager.LogMessage("Obteniendo ordenes de compra", 1);

                foreach (DataRow row in dt.Rows)
                {
                    orderList.Add(new PurchaseOrderModel
                    {
                        BusinessPartner = new BusinessPartnerModel
                        {
                            CardName = Convert.ToString(row["CardName"])
                        },
                        DocNum = Convert.ToInt32(row["DocNum"]),
                        DocDate = Convert.ToDateTime(row["DocDate"]),
                        DocTotal = Convert.ToDouble(row["DocTotal"]),

                    });
                }

                LogManager.LogMessage("Ordenes de compra obtenidas", 1);

                return new PurchaserOrdersResponse
                {
                    Result = true,
                    PurchaseOrders = orderList
                };
            }
            catch (Exception e)
            {
                LogManager.LogMessage("Error al obtener las ordenes de compra " + e.Message, 1);
                throw;
            }
        }
        public static PurchaserOrderResponse GetPurchaseOrder(CredentialHolder _userCredentials, int _DocNum, string _dbObjectSpGetPurchaseOrderByCode, string _dbObjectSpBPPurchaseOrder)
        {
            string fatherCard = string.Empty;
            try
            {
                string sql = string.Format("EXEC {0}.dbo.[{2}]'{1}'", _userCredentials.DBCode, _DocNum, _dbObjectSpGetPurchaseOrderByCode);
                List<EntryLineModel> itemList = new List<EntryLineModel>();

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);
                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, sql);

                foreach (DataRow row in dt.Rows)
                {
                    itemList.Add(new EntryLineModel
                    {
                        ItemCode = Convert.ToString(row["ItemCode"]),
                        WareHouse = Convert.ToString(row["WhsCode"]),
                        Quantity = Convert.ToInt32(row["Quantity"]),
                        UnitPrice = Convert.ToDouble(row["UnitPrice"] != null && row["UnitPrice"].ToString() != "" ? Convert.ToDecimal(row["UnitPrice"]) : 0),
                        ItemName = Convert.ToString(row["ItemName"]),
                        Discount = Convert.ToDouble(row["DiscPrcnt"]),
                        //TaxRate = Convert.ToInt32(row["TaxRate"]),
                        TaxRate = string.IsNullOrEmpty(Convert.ToString(row["TaxRate"])) ? 0 : Convert.ToDouble(row["TaxRate"]),
                        TaxCode = Convert.ToString(row["TaxCode"]),
                        LineTotal = Convert.ToDouble(row["LineTotal"]),
                        TaxOnly = Convert.ToString(row["TaxOnly"]).Equals("Y"),
                    });
                }


                string sql1 = string.Format("EXEC {0}.dbo.[{2}]'{1}'", _userCredentials.DBCode, _DocNum, _dbObjectSpBPPurchaseOrder);
                BusinessPartnerModel businessPartnerModel = new BusinessPartnerModel();
                DataTable dt1 = Common.QueryToTable(REPLACED_CONNECTION_STRING, sql1);
                if (dt1.Rows.Count > 0)
                {
                    businessPartnerModel = new BusinessPartnerModel
                    {
                        CardCode = Convert.ToString(dt1.Rows[0]["CardCode"]),
                        CardName = Convert.ToString(dt1.Rows[0]["CardName"]),
                    };
                }
                string Comment = String.Empty;

                if (dt1.Rows.Count > 0)
                {
                    Comment = Convert.ToString(dt1.Rows[0]["Comments"]);
                }


                return new PurchaserOrderResponse
                {
                    Result = true,
                    PurchaseOrder = new PurchaseOrderModel
                    {
                        Lines = itemList,
                        Comments = Comment,
                        BusinessPartner = businessPartnerModel

                    }
                };
            }
            catch (Exception)
            {
                throw;
            }

        }

        #endregion

        #region Quotation
        // obtiene Ofertas de Venta
        public static QuotationResponse GetQuotations(quotationSearch quotationSearch, string dbObject, CredentialHolder credential)
        {
            try
            {
                string Fini = quotationSearch.Fini.ToString("yyyy-MM-dd");
                string Ffin = quotationSearch.Ffin.ToString("yyyy-MM-dd");

                //string sql = $"EXEC {company.DBCode}.dbo." + dbObject + " @SlpCode, @DocStatus, @DocNum, @CardCode, @Fini, @Ffin";

                string query = QueryType == "SQL" ?
               string.Format("EXEC {0}.dbo.[{1}] {2}, {3}, '{4}', '{5}', '{6}', '{7}'", credential.DBCode, dbObject, quotationSearch.SlpCode, quotationSearch.DocStatus, quotationSearch.DocNum, quotationSearch.CardCode, Fini, Ffin)
                 : $"CALL {credential.DBCode}.{dbObject}('{quotationSearch.SlpCode}', '{quotationSearch.DocStatus}','{(quotationSearch.DocNum > 0 ? quotationSearch.DocNum : 0)}', '{(!string.IsNullOrEmpty(quotationSearch.CardCode) ? quotationSearch.CardCode : "")}', '{Fini}', '{Ffin}')";


                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(credential);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);


                if (dt == null || dt.Rows.Count == 0)
                {
                    return new QuotationResponse()
                    {
                        Quotations = null,
                        Error = new ErrorInfo()
                        {
                            Code = -1,
                            Message = "No se encontraron documentos."
                        },
                        Result = false
                    };
                }



                List<QuotationsModel> quotation = new List<QuotationsModel>();

                foreach (DataRow row in dt.Rows)
                {
                    quotation.Add(new QuotationsModel()
                    {
                        DocEntry = Convert.ToInt32(row["DocEntry"]),
                        DocNum = Convert.ToInt32(row["DocNum"]),
                        DocDate = Convert.ToDateTime(row["DocDate"]),
                        CardCode = Convert.ToString(row["CardCode"]),
                        CardName = Convert.ToString(row["CardName"]),
                        DocTotal = Convert.ToInt32(row["DocTotal"]),
                        DocTotalFC = Convert.ToInt32(row["DocTotalFC"]),
                        DocStatus = Convert.ToString(row["DocStatus"]),
                        SalesPersonCode = Convert.ToInt32(row["SlpCode"]),
                        SalesMan = Convert.ToString(row["SalesMan"]),
                        Comments = Convert.ToString(row["Comment"]),
                        DocCurrency = Convert.ToString(row["Currency"])

                    });
                }

                return new QuotationResponse()
                {
                    Quotations = quotation,
                    Error = null,
                    Result = true
                };

                //List<QuotationsModel> quotation;
                //string Fini = quotationSearch.Fini.ToString("yyyy-MM-dd");
                //string Ffin = quotationSearch.Ffin.ToString("yyyy-MM-dd");
                //using (SuperV2_Entities db = new SuperV2_Entities())
                //{
                //    object[] param = new object[] {
                //        new SqlParameter("@SlpCode", quotationSearch.SlpCode),
                //        new SqlParameter("@DocStatus", quotationSearch.DocStatus),
                //        new SqlParameter("@DocNum", quotationSearch.DocNum > 0 ? quotationSearch.DocNum : 0),
                //        new SqlParameter("@CardCode", !string.IsNullOrEmpty(quotationSearch.CardCode) ? quotationSearch.CardCode : ""),
                //        new SqlParameter("@Fini", Fini),
                //        new SqlParameter("@Ffin", Ffin)
                //        //new SqlParameter("@U_Almacen", quotationSearch.U_Almacen)
                //    };
                //    quotation = db.Database.SqlQuery<QuotationsModel>($"EXEC {company.DBCode}.dbo." + dbObject + " @SlpCode, @DocStatus, @DocNum, @CardCode, @Fini, @Ffin", param).ToList<QuotationsModel>();
                //    quotation.Reverse();
                //    db.Dispose();
                //    if (quotation != null)
                //    {
                //        return new QuotationResponse
                //        {
                //            result = true,
                //            errorInfo = null,
                //            Quotations = quotation
                //        };
                //    }
                //    else
                //    {
                //        return null;
                //    }
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // editar Oferta de venta
        public static ApiResponse<IDocument> GetQuotationEdit(int DocEntry, bool _allLines, string dbObjectEditHeader, string dbObjectEditLines, CredentialHolder credentials)
        {
            try
            {
               

                string query = QueryType == "SQL" ?
                 $"EXEC {credentials.DBCode}.dbo.[{dbObjectEditHeader}] {DocEntry}"
                   : $"CALL {credentials.DBCode}.{dbObjectEditHeader}('{DocEntry}')";

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(credentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);


                if (dt == null || dt.Rows.Count == 0)
                {
                    throw new Exception("Cotización no encontrada.");
                }
                IDocument quotationEdit =  new IDocument();
                FillObject(System.Reflection.MethodBase.GetCurrentMethod().Name, quotationEdit, dt.Rows[0], dt.Columns);


                //foreach (DataRow row in dt.Rows)
                //{
                //    quotationEdit = new IDocument()
                //    {
                //        DocEntry = Convert.ToInt32(row["DocEntry"]),
                //        DocNum = Convert.ToInt32(row["DocNum"]),
                //        DocDate = Convert.ToDateTime(row["DocDate"]),
                //        CardCode = Convert.ToString(row["CardCode"]),
                //        CardName = Convert.ToString(row["CardName"]),
                //        DocTotal = Convert.ToDecimal(row["DocTotal"]),
                //        DocTotalFC = Convert.ToDecimal(row["DocTotalFC"]),
                //        DocStatus = Convert.ToString(row["DocStatus"]),
                //        Comments = Convert.ToString(row["Comment"]),
                //        DocCurrency = Convert.ToString(row["Currency"]),
                //        SalesPersonCode = Convert.ToInt32(row["SlpCode"]),
                //        SalesMan = Convert.ToString(row["SalesMan"]),
                //        U_TipoIdentificacion = Convert.ToString(row["IdType"]),
                //        U_NumIdentFE = Convert.ToString(row["Identification"]),
                //        U_CorreoFE = Convert.ToString(row["Email"]),
                //        U_ObservacionFE = Convert.ToString(row["U_ObservacionFE"]),
                //        U_TipoDocE = Convert.ToString(row["DocumentType"]),
                //        PaymentGroupCode = Convert.ToString(row["PayTerms"]),
                //        NumAtCard = Convert.ToString(row["NumAtCard"]),
                //        U_ListNum = Convert.ToInt32(row["U_ListNum"]),
                //        //Currency = Convert.ToString(row["Currency"])
                //    };
                //}

                //dt = null;


                //  sql = $"EXEC {credentials.DBCode}.dbo." + dbObjectEditLines + " {DocEntry}, {CardCode}, {AllLines}";
                query = QueryType == "SQL" ?
                string.Format("EXEC {0}.dbo.{1} {2}, {3}, {4}", credentials.DBCode, dbObjectEditLines, DocEntry, quotationEdit.CardCode, _allLines ? 0 : 1)
              : $"CALL {credentials.DBCode}.{dbObjectEditLines}('{DocEntry}', '{quotationEdit.CardCode}', '{(_allLines ? 1 : 0)}')";
          
                dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);


                if (dt == null || dt.Rows.Count == 0)
                {
                    throw new Exception("Lineas no encontrdas.");
                }

                List<DocumentLines> documentLines = new List<DocumentLines>();

                for (int c = 0; c < dt.Rows.Count; c++)
                {
                    DocumentLines OQdocumentLines = new DocumentLines();

                    DataRow oDataRow = dt.Rows[c];

                    FillObject(System.Reflection.MethodBase.GetCurrentMethod().Name, OQdocumentLines, oDataRow, dt.Columns);

                    documentLines.Add(OQdocumentLines);
                }
                quotationEdit.DocumentLines = documentLines;

                //foreach (DataRow row in dt.Rows)
                //{
                //    quotationEdit.DocumentLines.Add(new DocumentLines()
                //    {
                //        ItemCode = Convert.ToString(row["Itemcode"]),
                //        ItemName = Convert.ToString(row["ItemName"]),
                //        Quantity = Convert.ToDouble(row["Quantity"]),
                //        UnitPrice = Convert.ToDouble(row["UnitPrice"]),
                //        DiscountPercent = row["Discount"] is DBNull ? 0 : Convert.ToDouble(row["Discount"]),
                //        TaxCode = Convert.ToString(row["TaxCode"]),
                //        TaxRate = Convert.ToDouble(row["TaxRate"]),
                //        WarehouseCode = Convert.ToString(row["WhsCode"]),
                //        WhsName = Convert.ToString(row["WhsName"]),
                //        LineNum = Convert.ToInt32(row["LineNum"]),
                //        BaseLine = Convert.ToInt32(row["BaseLine"]),
                //        LastPurchasePrice = Convert.ToDouble(row["LastPurchasePrice"]),
                //        LineStatus = (row["LineStatus"] is DBNull) ? "" : Convert.ToString(row["LineStatus"]),
                //    });
                //}
             
                    return new ApiResponse<IDocument>
                    {
                        Result = true,
                        Error = null,
                        Data = quotationEdit
                    };
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Sale_Order
        // obtiene ordenes de venta
        public static SaleOrderResponse GetSaleOrders(saleOrderSearch saleOrderSearch, CredentialHolder _credentials, string dbObject)
        {
            try
            {

                string Fini = saleOrderSearch.Fini.ToString("yyyy-MM-dd");
                string Ffin = saleOrderSearch.Ffin.ToString("yyyy-MM-dd");

                int docNumb = saleOrderSearch.DocNum > 0 ? saleOrderSearch.DocNum : 0;
                string cardCode = !string.IsNullOrEmpty(saleOrderSearch.CardCode) ? saleOrderSearch.CardCode : "";

                string query = QueryType == "SQL" ?
                string.Format("EXEC {0}.dbo.[{1}] {2}, {3}, '{4}', '{5}', '{6}', '{7}'", _credentials.DBCode, dbObject, saleOrderSearch.SlpCode, saleOrderSearch.DocStatus, docNumb, cardCode, Fini, Ffin)
                 : $"CALL {_credentials.DBCode}.{dbObject}('{ saleOrderSearch.SlpCode}', '{ saleOrderSearch.DocStatus}','{(saleOrderSearch.DocNum > 0 ? saleOrderSearch.DocNum : 0)}', '{(!string.IsNullOrEmpty(saleOrderSearch.CardCode) ? saleOrderSearch.CardCode : "")}', '{Fini}', '{Ffin}')";

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_credentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                if (dt == null || dt.Rows.Count == 0)
                {
                    throw new Exception("No se encontraron documentos.");
                }          

                List<SalesOrderModel> saleOrders = new List<SalesOrderModel>();

                for (int c = 0; c < dt.Rows.Count; c++)
                {
                    SalesOrderModel oSalesOrderModel = new SalesOrderModel();

                    DataRow oDataRow = dt.Rows[c];

                    FillObject(System.Reflection.MethodBase.GetCurrentMethod().Name, oSalesOrderModel, oDataRow, dt.Columns);

                    saleOrders.Add(oSalesOrderModel);
                }

                //foreach (DataRow row in dt.Rows)
                //{
                //    quotation.Add(new SalesOrderModel()
                //    {
                //        DocEntry = Convert.ToInt32(row["DocEntry"]),
                //        DocNum = Convert.ToInt32(row["DocNum"]),
                //        DocDate = Convert.ToDateTime(row["DocDate"]),
                //        CardCode = Convert.ToString(row["CardCode"]),
                //        CardName = Convert.ToString(row["CardName"]),
                //        DocTotal = Convert.ToInt32(row["DocTotal"]),
                //        DocTotalFC = Convert.ToInt32(row["DocTotalFC"]),
                //        DocStatus = Convert.ToString(row["DocStatus"]),
                //        SalesPersonCode = Convert.ToInt32(row["SlpCode"]),
                //        SalesMan = Convert.ToString(row["SalesMan"]),
                //        Comments = Convert.ToString(row["Comments"]),
                //        DocCurrency = Convert.ToString(row["Currency"])

                //    });
                //}

                return new SaleOrderResponse()
                {
                    SaleOrders = saleOrders,
                    Error = null,
                    Result = true
                };


                //using (SuperV2_Entities db = new SuperV2_Entities())
                //{
                //    object[] param = new object[] {
                //        new SqlParameter("@SlpCode", -1),
                //        new SqlParameter("@DocStatus", saleOrderSearch.DocStatus),
                //        new SqlParameter("@DocNum", saleOrderSearch.DocNum > 0 ? saleOrderSearch.DocNum : 0),
                //        new SqlParameter("@CardCode", "C0001"),
                //        new SqlParameter("@Fini", Fini),
                //        new SqlParameter("@Ffin", Ffin)
                //        new SqlParameter("@U_Almacen", saleOrderSearch.U_Almacen)
                //    };

                //    saleOrders = db.Database.SqlQuery<SalesOrderModel>($"EXEC TST_CL_SUPERLT.dbo." + dbObject + "  @SlpCode, @DocStatus, @DocNum, @CardCode, @Fini, @Ffin", param).ToList<SalesOrderModel>();
                //    saleOrders.Reverse();
                //    db.Dispose();
                //    if (saleOrders != null)
                //    {
                //        return new SaleOrderResponse
                //        {
                //            result = true,
                //            errorInfo = null,
                //            SaleOrders = saleOrders
                //        };
                //    }
                //    else
                //    {
                //        return null;
                //    }
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        // Obtiene una orden de venta
        public static ApiResponse<IDocument> GetSaleOrder(int DocEntry, string dbObjectEditHeader, string dbObjectEditLines, CredentialHolder _credentials)
        {
            try
            {
                string query = QueryType == "SQL" ?
                  string.Format("EXEC {0}.dbo.{1} {2}", _credentials.DBCode, dbObjectEditHeader, DocEntry)
                   : $"CALL {_credentials.DBCode}.{dbObjectEditHeader}('{DocEntry}')";
              
                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_credentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                if (dt == null || dt.Rows.Count == 0)
                {
                    throw new Exception("Orden de venta no encontrada.");
                }

                IDocument saleOrder = new IDocument();

                FillObject(System.Reflection.MethodBase.GetCurrentMethod().Name, saleOrder, dt.Rows[0], dt.Columns);

                //DataRow row = dt.Rows[0];

                query = QueryType == "SQL" ?
                string.Format("EXEC {0}.dbo.{1} {2}, {3}", _credentials.DBCode, dbObjectEditLines, DocEntry, saleOrder.CardCode)
                : $"CALL {_credentials.DBCode}.{dbObjectEditLines}('{DocEntry}', '{saleOrder.CardCode}')";

                //saleOrder = new IDocument()
                //{
                //    DocEntry = Convert.ToInt32(row["DocEntry"]),
                //    PaymentGroupCode = Convert.ToString(row["PayTerms"]),
                //    DocNum = Convert.ToInt32(row["DocNum"]),
                //    DocDate = Convert.ToDateTime(row["DocDate"]),
                //    CardCode = Convert.ToString(row["CardCode"]),
                //    CardName = Convert.ToString(row["CardName"]),
                //    DocTotal = Convert.ToDecimal(row["DocTotal"]),
                //    DocTotalFC = Convert.ToDecimal(row["DocTotalFC"]),
                //    DocStatus = Convert.ToString(row["DocStatus"]),
                //    Comments = Convert.ToString(row["Comment"]),
                //    DocCurrency = Convert.ToString(row["Currency"]),
                //    SalesPersonCode = Convert.ToInt32(row["SlpCode"]),
                //    SalesMan = Convert.ToString(row["SalesMan"]),
                //    U_TipoIdentificacion = Convert.ToString(row["IdType"]),
                //    U_NumIdentFE = Convert.ToString(row["Identification"]),
                //    U_CorreoFE = Convert.ToString(row["Email"]),
                //    U_ObservacionFE = Convert.ToString(row["U_ObservacionFE"]),
                //    NumAtCard = Convert.ToString(row["NumAtCard"]),
                //    U_TipoDocE = Convert.ToString(row["DocumentType"]),
                //    U_ListNum = Convert.ToInt32(row["U_ListNum"]),
                //    //Currency = Convert.ToString(row["Currency"])

                //};

                //dt = null;

                //sql = string.Format("EXEC {0}.dbo.{1} {2}, {3}", _credentials.DBCode, dbObjectEditLines, DocEntry, saleOrder.CardCode);


                  
                dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                if (dt == null || dt.Rows.Count == 0)
                {
                    throw new Exception("Lineas del documento no encontradas.");
                }

                List<DocumentLines> documentLines = new List<DocumentLines>();

                for (int c = 0; c < dt.Rows.Count; c++)
                {
                    DocumentLines oSaleOrderLines = new DocumentLines();

                    DataRow oDataRow = dt.Rows[c];

                    FillObject(System.Reflection.MethodBase.GetCurrentMethod().Name, oSaleOrderLines, oDataRow, dt.Columns);

                    documentLines.Add(oSaleOrderLines);
                }

                saleOrder.DocumentLines = documentLines;

                //saleOrder.DocumentLines = new List<DocumentLines>();

                //foreach (DataRow rowline in dt.Rows)
                //{
                //    saleOrder.DocumentLines.Add(new DocumentLines()
                //    {
                //        ItemCode = Convert.ToString(rowline["ItemCode"]),
                //        ItemName = Convert.ToString(rowline["ItemName"]),
                //        Quantity = Convert.ToDouble(rowline["Quantity"]),
                //        UnitPrice = Convert.ToDouble(rowline["UnitPrice"]),
                //        DiscountPercent = rowline["Discount"] is DBNull ? 0 : Convert.ToDouble(rowline["Discount"]),
                //        TaxCode = Convert.ToString(rowline["TaxCode"]),
                //        TaxRate = Convert.ToDouble(rowline["TaxRate"]),
                //        WarehouseCode = Convert.ToString(rowline["WhsCode"]),
                //        WhsName = Convert.ToString(rowline["WhsName"]),
                //        BaseLine = (rowline["BaseLine"] is DBNull) ? -1 : Convert.ToInt32(rowline["BaseLine"]),
                //        LineNum = Convert.ToInt32(rowline["LineNum"]),
                //        BaseEntry = rowline["BaseEntry"] is DBNull ? 0 : Convert.ToInt32(rowline["BaseEntry"]),
                //        BaseType = Convert.ToInt32(rowline["BaseType"]),
                //        LastPurchasePrice = Convert.ToDouble(rowline["LastPurchasePrice"]),
                //        LineStatus = (rowline["LineStatus"] is DBNull) ? "" : Convert.ToString(rowline["LineStatus"]),
                //    });

                //}


                return new ApiResponse<IDocument>()
                {
                    Result = true,
                    Data = saleOrder
                };

                //SalesOrderModel saleoOrder;
                //using (SuperV2_Entities db = new SuperV2_Entities())
                //{
                //    object[] param = new object[] {
                //        new SqlParameter("@DocEntry", DocEntry)
                //    };

                //saleoOrder = db.Database.SqlQuery<SalesOrderModel>($"EXEC {company.DBCode}.dbo." + dbObjectEditHeader + " @DocEntry", param).FirstOrDefault<SalesOrderModel>();
                //    if (saleoOrder == null)
                //    {
                //        db.Dispose();
                //        throw new Exception("Orden de venta no encontrada.");
                //    }
                //    object[] param2 = new object[] {
                //        new SqlParameter("@DocEntry", DocEntry),
                //        new SqlParameter("@CardCode", saleoOrder.CardCode)
                //    };
                //    saleoOrder.SaleOrderLinesList = new List<InvoiceLinesModelBase>(db.Database.SqlQuery<InvoiceLinesModelBase>($"EXEC {company.DBCode}.dbo." + dbObjectEditLines + " @DocEntry, @CardCode", param2).ToList<InvoiceLinesModelBase>());
                //    db.Dispose();
                //    if (saleoOrder != null)
                //    {
                //        return new GetSaleOrderResponse
                //        {
                //            result = true,
                //            errorInfo = null,
                //            SaleOrder = saleoOrder
                //        };
                //    }
                //    else
                //    {
                //        return null;
                //    }
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Cards
        public static CardResponse GetPaymentCardByDocEntry(int _docEntry)
        {
            try
            {
                CardsModel oCard = new CardsModel();

                using (SuperV2_Entities db = new SuperV2_Entities())
                {

                    object[] param = new object[] {
                        new SqlParameter("@DocEntry", _docEntry)
                    };

                    string query = QueryType == "SQL" ?
                   $"EXEC dbo.{Common.GetDBObjectByKey(System.Reflection.MethodBase.GetCurrentMethod(), "spGetVoucherDetail")} @DocEntry"
                   : $"CALL {Common.GetDBObjectByKey(System.Reflection.MethodBase.GetCurrentMethod(), "spGetVoucherDetail")}('@DocEntry')";

                    oCard = db.Database.SqlQuery<CardsModel>(query, param).FirstOrDefault<CardsModel>();

                    //oCard = db.Database.SqlQuery<CardsModel>($"EXEC dbo.{Common.GetDBObjectByKey(System.Reflection.MethodBase.GetCurrentMethod(), "spGetVoucherDetail")} @DocEntry", param).FirstOrDefault<CardsModel>();

                }

                return new CardResponse
                {
                    Result = true,
                    Card = oCard
                };
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Invoice
        public static InvoiceTypesResponse GetInvoiceTypes(CredentialHolder _credentials, string _dbObjectViewGetInvoiceType)
        {
            try
            {
                string query = QueryType == "SQL" ?
                   $"SELECT * FROM {_credentials.DBCode}.dbo.{_dbObjectViewGetInvoiceType}"
                   : $"SELECT * FROM {_credentials.DBCode}.{_dbObjectViewGetInvoiceType}";

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_credentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                List<InvoiceType> invoiceTypes = new List<InvoiceType>();

                for (int c = 0; c < dt.Rows.Count; c++)
                {
                    InvoiceType oInvoiceType = new InvoiceType();

                    DataRow oDataRow = dt.Rows[c];

                    FillObject(System.Reflection.MethodBase.GetCurrentMethod().Name, oInvoiceType, oDataRow, dt.Columns);

                    invoiceTypes.Add(oInvoiceType);
                }

                //SuperV2_Entities oSuperV2_Entities = new SuperV2_Entities();

                //List<InvoiceType> invoiceTypes = new List<InvoiceType>(oSuperV2_Entities.Database.SqlQuery<InvoiceType>($"SELECT * FROM {_credentials.DBCode}.dbo.{_dbObjectViewGetInvoiceType}").ToList<InvoiceType>());

                //oSuperV2_Entities.Dispose();
                //oSuperV2_Entities = null;

                return new InvoiceTypesResponse
                {
                    Result = true,
                    InvoiceTypes = invoiceTypes
                };
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region Udf
        // Obtiene los udfs de sap que seran renderizados segun categoria solicita
        public static UdfsResponse GetUdfs(Companys _company, string _category, string _dbObjectSpGetUdfs, CredentialHolder _userCredentials)
        {
            try
            {
                //    SuperV2_Entities oSuperV2_Entities = new SuperV2_Entities();

                //    List<Udf> udfs = new List<Udf>(oSuperV2_Entities.Database.SqlQuery<Udf>($"EXEC {_company.DBCode}.dbo.[{dbObject}] '{_category}','{page}','{pageSize}'").ToList<Udf>());

                //    oSuperV2_Entities.Dispose();
                //    oSuperV2_Entities = null;


                string query = QueryType == "SQL" ?
                    $"EXEC {_userCredentials.DBCode}.dbo.[{_dbObjectSpGetUdfs}] '{_category}'"
                   : $"CALL {_company.DBCode}.{_dbObjectSpGetUdfs}('{_category}')";

                List<Udf> udfs = new List<Udf>();

                DataSet dt = ExecuteQueryGetMultipleTables(Common.ReplaceConectODBC(_userCredentials), query);

                //int Size = 0;

                if (dt.Tables[0].Rows != null && dt.Tables[0].Rows.Count > 0)
                {
                    //DataRow auxRow = dt.Tables[1].Rows[0];

                    //Size = Convert.ToInt32(auxRow["Size"]);
                    foreach (DataRow row in dt.Tables[0].Rows)
                    {
                        udfs.Add(new Udf()
                        {
                            TableId = Convert.ToString(row["TableID"]),
                            Name = Convert.ToString(row["Name"]),
                            Description = Convert.ToString(row["Description"]),
                            FieldType = Convert.ToString(row["FieldType"]),
                            Values = row["Values"] is DBNull ? string.Empty : Convert.ToString(row["Values"]),
                        });
                    }
                }
                else
                {
                    throw new Exception($"No se encontraron registros para la categoría {_category}");
                }









                return new UdfsResponse
                {
                    Result = true,
                    Udfs = udfs,
                    //FullSize = Size
                };
            }
            catch
            {
                throw;
            }
        }

        public static UdfCategoriesResponse GetUdfCategories(CredentialHolder _userCredentials, string _dbObjectViewGetUdfCategories)
        {
            try
            {
                //SuperV2_Entities oSuperV2_Entities = new SuperV2_Entities();
                string query = QueryType == "SQL" ?
                   $"SELECT * FROM {_userCredentials.DBCode}.dbo.{_dbObjectViewGetUdfCategories}"
                   : $"SELECT * FROM {_userCredentials.DBCode}.{_dbObjectViewGetUdfCategories}";

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                List<UdfCategory> udfCategories = new List<UdfCategory>();

                for (int c = 0; c < dt.Rows.Count; c++)
                {
                    UdfCategory oUdfCategory = new UdfCategory();

                    DataRow oDataRow = dt.Rows[c];

                    FillObject(System.Reflection.MethodBase.GetCurrentMethod().Name, oUdfCategory, oDataRow, dt.Columns);

                    udfCategories.Add(oUdfCategory);
                }

                //List<UdfCategory> udfCategories = new List<UdfCategory>(oSuperV2_Entities.Database.SqlQuery<UdfCategory>($"SELECT * FROM {_company.DBCode}.dbo.[{_dbObjectViewGetUdfCategories}]").ToList<UdfCategory>());

                //oSuperV2_Entities.Dispose();
                //oSuperV2_Entities = null;

                return new UdfCategoriesResponse
                {
                    Result = true,
                    UdfCategories = udfCategories
                };
            }
            catch
            {
                throw;
            }
        }

        public static UdfCategoriesResponse GetUdfDevelopment(CredentialHolder _userCredentials, string _dbObjectViewGetUdfDevelopment)
        {
            try
            {
                string query = QueryType == "SQL" ?
                    $"SELECT * FROM {_userCredentials.DBCode}.dbo.{_dbObjectViewGetUdfDevelopment} WHERE Name NOT IN('BASE_UDF')"
                    : $"SELECT * FROM {_userCredentials.DBCode}.{_dbObjectViewGetUdfDevelopment} WHERE \"Name\" NOT IN('BASE_UDF');";

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                List<UdfCategory> udfCategories = new List<UdfCategory>();

                for (int c = 0; c < dt.Rows.Count; c++)
                {
                    UdfCategory oUdfCategory = new UdfCategory();

                    DataRow oDataRow = dt.Rows[c];

                    FillObject(System.Reflection.MethodBase.GetCurrentMethod().Name, oUdfCategory, oDataRow, dt.Columns);

                    udfCategories.Add(oUdfCategory);
                }

                //SuperV2_Entities oSuperV2_Entities = new SuperV2_Entities();

                //List<UdfCategory> udfCategories = new List<UdfCategory>(oSuperV2_Entities.Database.SqlQuery<UdfCategory>($"SELECT * FROM {_company.DBCode}.dbo.[{_dbObjectViewGetUdfDevelopment}] WHERE Name NOT IN('BASE_UDF')").ToList<UdfCategory>());

                //oSuperV2_Entities.Dispose();
                //oSuperV2_Entities = null;

                return new UdfCategoriesResponse
                {
                    Result = true,
                    UdfCategories = udfCategories
                };
            }
            catch
            {
                throw;
            }
        }

        public static UdfsTargetResponse GetUdfsData(CredentialHolder _userCredentials, UdfSource _udfSource)
        {
            try
            {
                string query = QueryType == "SQL" ?
                 $"SELECT * FROM {_userCredentials.DBCode}.dbo.{_udfSource.TableId} WHERE {_udfSource.Key}={_udfSource.Value}"
                 : $"SELECT * FROM {_userCredentials.DBCode}.{_udfSource.TableId} WHERE \"{_udfSource.Key}\"={_udfSource.Value};";

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                //SuperV2_Entities oSuperV2_Entities = new SuperV2_Entities();
                string parameters = String.Empty;
                List<UdfTarget> udfsTarget = new List<UdfTarget>();
                //string query = $"SELECT * FROM {_company.DBCode}.dbo.{_udfSource.TableId} WHERE {_udfSource.Key}={_udfSource.Value}";

                //DataTable dt = ExecuteQuery(Common.ReplaceConnectorODBC(_company.DBCode), query);

                if (dt.Rows != null && dt.Rows.Count > 0)
                {
                    _udfSource.UdfsTarget.ForEach(x =>
                    {
                        UdfTarget oUdfTarget = new UdfTarget
                        {
                            Value = Convert.ToString(dt.Rows[0][x.Name]),
                            Name = x.Name,
                            FieldType = x.FieldType
                        };

                        udfsTarget.Add(oUdfTarget);
                    });
                }
                else
                {
                    throw new Exception($"No se encontraron registros para {_udfSource.TableId} con llave {_udfSource.Value}");
                }

                //oSuperV2_Entities.Dispose();
                //oSuperV2_Entities = null;

                return new UdfsTargetResponse
                {
                    Result = true,
                    UdfsTarget = udfsTarget
                };
            }
            catch
            {
                throw;
            }
        }
        #endregion
        public static SyncResponse SyncGetInvoiceTypes(Companys company, string _dbObjectViewGetInvoiceType)
        {

            try
            {
                SuperV2_Entities oSuperV2_Entities = new SuperV2_Entities();

                List<InvoiceType> invoiceTypes = new List<InvoiceType>(oSuperV2_Entities.Database.SqlQuery<InvoiceType>($"SELECT * FROM {company.DBCode}.dbo.{_dbObjectViewGetInvoiceType}").ToList<InvoiceType>());

                oSuperV2_Entities.Dispose();
                oSuperV2_Entities = null;

                return new SyncResponse
                {
                    result = true,
                    rowsToSync = invoiceTypes.Cast<object>().ToList()
                };

            }
            catch (Exception)
            {
                throw;
            }
        }
        #region Paydesk
        public static ApiResponse<List<CashflowReasonModel>> GetCashflowReasons(CredentialHolder _userCredentials, string _dbObjectSpCashflowReason)
        {
            try
            {
                List<CashflowReasonModel> reasons = new List<CashflowReasonModel>();
                string query = QueryType == "SQL" ?
                      $"SELECT * FROM {_userCredentials.DBCode}.dbo.{_dbObjectSpCashflowReason}"
                      : $"SELECT * FROM {_userCredentials.DBCode}.{_dbObjectSpCashflowReason}";

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                if (dt.Rows != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        reasons.Add(new CashflowReasonModel
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            Name = Convert.ToString(row["Name"]),
                            Type = Convert.ToInt32(row["Type"]),

                        });   
                    }                                          
                }
                else
                {
                    throw new Exception("No se obtuvieron motivos por defecto para movimientos de caja");
                }

                return new ApiResponse<List<CashflowReasonModel>>
                {
                    Result = true,
                    Data = reasons,
                    Error = null
                };
            }
            catch
            {
                throw;
            }
        }
        public static List<double> SelectCashflowTotals(int userSignature, CredentialHolder _userCredentials, string _dbObjectSpGetChasFlowTotal)
        {
            List<double> cashflowTotals = new List<double>();

            string query = QueryType == "SQL" ?
                    $"EXEC {_userCredentials.DBCode}.dbo.{_dbObjectSpGetChasFlowTotal} '{userSignature}'"
                    : $"CALL {_userCredentials.DBCode}.{_dbObjectSpGetChasFlowTotal}('{userSignature}')";

            string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

            DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

            foreach (DataRow row in dt.Rows)
            {
                cashflowTotals.Add(Convert.ToDouble(row["Total"]));
            }

            return cashflowTotals;
        }
        #endregion
        /// <summary>
        /// Obtiene código, nombre de item si encuentra coincidencia con parametro itemNameXml descripcion item del xml
        /// Obtiene TaxCode de acuerdo al porcentaje de impuesto del item
        /// </summary>
        /// <param name="_userCredentials"></param>
        /// <param name="_dbObjectSpItemByNameXml"></param>
        /// <param name="itemNameXml"></param>
        /// <param name="_dbObjectSpGetTaxCodeByTaxRate"></param>
        /// <param name="_impuesto"></param>
        /// <returns></returns>
        public static ApiResponse<EntryLineModel> GetItembyItemNameXml(CredentialHolder _userCredentials, string _dbObjectSpItemByNameXml, string itemNameXml, string _dbObjectSpGetTaxCodeByTaxRate, double _impuesto)
        {
            try
            {
                EntryLineModel infoItem = new EntryLineModel();
                string query = $"EXEC {_userCredentials.DBCode}.dbo.[{_dbObjectSpItemByNameXml}] '{itemNameXml}'";


                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                if (dt.Rows.Count > 0)
                {
                    infoItem.ItemCode = Convert.ToString(dt.Rows[0]["ItemCode"]);
                    infoItem.ItemName = Convert.ToString(dt.Rows[0]["ItemName"]);
                   
                }

                query = $"EXEC {_userCredentials.DBCode}.dbo.[{_dbObjectSpGetTaxCodeByTaxRate}] '{_impuesto}'";

                dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                if (dt.Rows.Count > 0)
                {
                    infoItem.TaxCode = Convert.ToString(dt.Rows[0]["TaxCode"]);
                }

                return new ApiResponse<EntryLineModel>
                {
                    Result = true,
                    Data = infoItem,
                    Error = null
                };

            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Busqueda  de nombre de proveedor de xml en BP de sap
        /// </summary>
        /// <param name="_userCredentials"></param>
        /// <param name="_dbObjectSpBPByNameXml"></param>
        /// <param name="_identificationXml"></param>
        /// <returns></returns>
        public static ApiResponse<BusinessPartnerModel> GetBusinessPartnerbyNameXml(CredentialHolder _userCredentials, string _dbObjectSpBPByNameXml, string _identificationXml)
        {
            try
            {
                BusinessPartnerModel infoBP = new BusinessPartnerModel();
                string query = $"EXEC {_userCredentials.DBCode}.dbo.[{_dbObjectSpBPByNameXml}] '{_identificationXml}'";

                string REPLACED_CONNECTION_STRING = Common.ReplaceConectODBC(_userCredentials);

                DataTable dt = Common.QueryToTable(REPLACED_CONNECTION_STRING, query);

                if (dt.Rows.Count > 0)
                {
                    infoBP.CardCode = Convert.ToString(dt.Rows[0]["CardCode"]);
                    infoBP.CardName = Convert.ToString(dt.Rows[0]["CardName"]);
                    infoBP.Cedula = Convert.ToString(dt.Rows[0]["LicTradNum"]);

                }                

                return new ApiResponse<BusinessPartnerModel>
                {
                    Result = true,
                    Data = infoBP,
                    Error = null
                };

            }
            catch
            {
                throw;
            }
        }
        private static void FillObject(string _invoker, Object _object, DataRow _oDataRow, DataColumnCollection _columns)
        {
            try
            {
                foreach (PropertyInfo prop in _object.GetType().GetProperties())
                {
                    try
                    {
                        Type type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                        if (_columns.Contains(prop.Name))
                        {
                            if (!_oDataRow.IsNull(prop.Name) && !System.DBNull.Value.Equals(_oDataRow[prop.Name]) && _oDataRow[prop.Name] != null)
                            {
                                switch (_oDataRow[prop.Name]?.GetType().Name)
                                {
                                    case "Byte":
                                        prop.SetValue(_object, Convert.ToBoolean(_oDataRow[prop.Name]));
                                        break;
                                    default:
                                        prop.SetValue(_object, _oDataRow[prop.Name]);
                                        break;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Invoker={_invoker}* Checkout <{prop.Name}> ? <{_oDataRow[prop.Name]}> {ex.Message}");
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
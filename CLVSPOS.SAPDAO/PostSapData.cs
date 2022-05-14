using CLVSPOS.COMMON;
using CLVSPOS.DAO;
using CLVSPOS.DIAPI;
using CLVSPOS.LOGGER;
using CLVSPOS.MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using SAPbobsCOM;
using CLVSSUPER.MODELS;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using CLVSSLCONN.Interface;
using CLVSSLCONN.Models;
using System.Reflection;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CLVSPOS.SAPDAO
{
	public class PostSapData
	{
		/// <summary>
		/// Funcion que crea un ARInvoice en SAP
		/// </summary>
		/// <param name="SaleOrder"></param>
		/// <param name="company"></param>
		/// <returns></returns>
		public static async Task<SalesOrderToSAPResponse> CreateSaleOrder(ISaleDocument SaleOrder, string dbObject, CredentialHolder credential, string userId)
		{


			try
			{

				LogManager.LogMessage("DIAPI/PostSapData/CreateSaleOrder-- Ingresando..." + " CardCode: " + SaleOrder.CardCode, (int)Constants.LogTypes.SAP);
				var SaleOrderToString = new JavaScriptSerializer().Serialize(SaleOrder);
				LogManager.LogMessage("DIAPI/PostSapData/CreateSaleOrder-- Objeto recibido SaleOrder: " + SaleOrderToString + ", company: " + credential.DBCode, (int)Constants.LogTypes.SAP);



				if (SaleOrder != null && SaleOrder.DocumentLines != null && SaleOrder.DocumentLines.Count > 0)
				{
					SalesOrderToSAPResponse SaleOrderResposeFromSap = new SalesOrderToSAPResponse();


					

					Resources connection = new Resources("https://clhna31.clavisco.com:50000/", "/b1s/v1/Login");

					Login loggin = new CLVSSLCONN.Models.Login()
					{
						CompanyDB = credential.DBCode,
						UserName = credential.SAPUser,
						Password = credential.SAPPass
					};





					DAO.SuperV2_Entities db = new SuperV2_Entities();
					int SerieType = (int)COMMON.Constants.SerialType.SOrder;

					UserAssign userA = db.UserAssign.Where(x => x.UserId == userId).FirstOrDefault();

					DiapiSerie diapiSeries = userA.V_SeriesByUser.Where(x => x.V_Series.DocType == SerieType).Select(x => new DiapiSerie
					{
						NumType = x.V_Series.Numbering,
						Serie = x.V_Series.Serie
					}).FirstOrDefault();


					if (diapiSeries == null) throw new Exception("No se ha definido una serie para crear Orden de ventas");


					SaleOrder.Series = diapiSeries.Serie;




					JObject jObjectSaleOrder = (JObject)JsonConvert.DeserializeObject(new JavaScriptSerializer().Serialize(SaleOrder));

					if (SaleOrder.UdfTarget != null)
					{
						SaleOrder.UdfTarget.Where(x => x != null && x.Value != null).ToList().ForEach(x =>
						{
							if (x.Value.GetType().FullName.Contains("String") && !String.IsNullOrEmpty(x.Value.ToString()))
							{
								jObjectSaleOrder[x.Name] = x.Value.ToString();
							}
							else
							{
								Object oObject = Convert.ChangeType(x.Value, Type.GetType($"System.{x.Value.GetType().Name}"));
								jObjectSaleOrder[x.Name] = JToken.FromObject(oObject);
							}
						});
					}


					JArray JLines = (JArray)JsonConvert.DeserializeObject(new JavaScriptSerializer().Serialize(SaleOrder.DocumentLines));

					List<string> unnecessariesProperties = new List<string>()
				{
					"DocEntry",
					"DocNum",
					"NumAtCard"
				};

					foreach (string propertie in unnecessariesProperties)
					{
						jObjectSaleOrder.Property(propertie).Remove();
					}

					foreach (JObject line in JLines)
					{
						line.Property("LineNum").Remove();
						line.Property("Serie").Remove();
					}
					jObjectSaleOrder.Property("DocumentLines").Remove();
					jObjectSaleOrder.Add("DocumentLines", JLines);

					string SalesOrderString = jObjectSaleOrder.ToString();



					HttpResponseMessage responsSalesOrder = await connection.SLRequest(loggin, "https://clhna31.clavisco.com:50000/b1s/v1/Orders", "POST", new List<KeyValuePair<string, string>>(), SalesOrderString);








					if (responsSalesOrder.IsSuccessStatusCode)
					{
						DocumentModel oSalesOrder = new JavaScriptSerializer().Deserialize<DocumentModel>(responsSalesOrder.Content.ReadAsStringAsync().Result);
						return new SalesOrderToSAPResponse()
						{
							DocEntry = oSalesOrder.DocEntry,
							DocNum = oSalesOrder.DocNum,
							Result = true
						};
					}
					else
					{
						SLResponse badResponse = new JavaScriptSerializer().Deserialize<SLResponse>(responsSalesOrder.Content.ReadAsStringAsync().Result);
						throw new Exception("La orden no ha sido creada debido a: " + badResponse.error.message.value);
					}

				}
				else
				{
					throw new Exception("El documento debe contener al menos una linea");
				}
			}
			catch
			{
				throw;
			}
		}

		public static async Task<DocumentUpdateResponse> UpdateSaleOrder(ISaleDocument saleOrder, CredentialHolder _credentials, string dbObject)
		{

			try
			{
				if (saleOrder != null && saleOrder.DocumentLines != null && saleOrder.DocumentLines.Count > 0)
				{


					Resources connection = new Resources("https://clhna31.clavisco.com:50000/", "/b1s/v1/Login");

					Login loggin = new CLVSSLCONN.Models.Login()
					{
						CompanyDB = _credentials.DBCode,
						UserName = _credentials.SAPUser,
						Password = _credentials.SAPPass
					};


					
					JObject jObjectSaleOrder = (JObject)JToken.FromObject(saleOrder);

					if (saleOrder.UdfTarget != null)
					{
						saleOrder.UdfTarget.Where(x => x != null && x.Value != null).ToList().ForEach(x =>
						{
							if (x.Value.GetType().FullName.Contains("String") && !String.IsNullOrEmpty(x.Value.ToString()))
							{
								jObjectSaleOrder[x.Name] = x.Value.ToString();
							}
							else
							{
								Object oObject = Convert.ChangeType(x.Value, Type.GetType($"System.{x.Value.GetType().Name}"));
								jObjectSaleOrder[x.Name] = JToken.FromObject(oObject);
							}
						});
					}

					JArray JLines = (JArray)JsonConvert.DeserializeObject(new JavaScriptSerializer().Serialize(saleOrder.DocumentLines));



					List<string> unnecessariesProperties = new List<string>()
				{
					"DocNum",
					"DocEntry",
					"DocDate",
					"DocDueDate",
					"BaseEntry",
					"DocType",
					"Series",
					"CardCode",
					"CardName",
					"DocTotal",
					"DocTotalFC",
					"U_CLVS_POS_UniqueInvId",
					"UdfTarget"
				};


					List<string> notRequiredDataForLines = new List<string>()
				{
					"LineNum",
					"BaseEntry",
					"BaseLine",
					"BaseType",
					"Serie",
					"TaxRate",
					"TaxOnly"
				};


					List<KeyValuePair<string, string>> headers = new List<KeyValuePair<string, string>>() {

					new KeyValuePair<string, string>("B1S-ReplaceCollectionsOnPatch","true")

					};


					// Se limpia el objeto de valores null
					var aux = jObjectSaleOrder.DeepClone();

					foreach (JProperty p in aux)
					{
						if (p.Value.Type == JTokenType.Null)
							jObjectSaleOrder.Property(p.Name).Remove();

					}


					//se limpia el objeto de las propiedades que no deben irse al SL
					foreach (string propertie in unnecessariesProperties)
					{
						if (jObjectSaleOrder.Property(propertie).Type != JTokenType.Null)
							jObjectSaleOrder.Property(propertie).Remove();
					}

					//Se limpia las lineas de las propiedades que no se ocupan
					foreach (JObject line in JLines)
					{


						foreach (string property in notRequiredDataForLines)
						{
							if (line.Property(property).Type != JTokenType.Null)
								line.Property(property).Remove();

						}
					}

					jObjectSaleOrder.Property("DocumentLines").Remove();
					jObjectSaleOrder.Add("DocumentLines", JLines);




					string SaleOrderString = jObjectSaleOrder.ToString();




					HttpResponseMessage responseSalesOrder = await connection.SLRequest(
						loggin,
						$"https://clhna31.clavisco.com:50000/b1s/v1/Orders({saleOrder.DocEntry})",
						"PATCH",
						new List<KeyValuePair<string, string>>() {
							new KeyValuePair<string, string>("B1S-ReplaceCollectionsOnPatch","true")
						},
						SaleOrderString);



					if (responseSalesOrder.StatusCode == System.Net.HttpStatusCode.NoContent)
					{

						DocInfo docInfo = GetSapData.GetDocNumByDocEntryORDR(_credentials, saleOrder.DocEntry, dbObject);

						return new DocumentUpdateResponse()
						{
							DocEntry = saleOrder.DocEntry,
							DocNum = docInfo != null ? docInfo.DocNum : 0,
							Result = true
						};

					}
					else
					{
						SLResponse badResponse = new JavaScriptSerializer().Deserialize<SLResponse>(responseSalesOrder.Content.ReadAsStringAsync().Result);
						throw new Exception("La orden de venta no ha sido actualizada debido a: " + badResponse.error.message.value);
					}
				}
				else
				{
					throw new Exception("El documento debe contener al menos una linea");
				}
			}
			catch
			{
				throw;

			}
		}

		/// <summary>
		///  Funcion que crea una cotizacion en SAP
		/// </summary>
		/// <param name="Quotations"></param>
		/// <param name="company"></param>
		/// <param name="userId"></param>
		/// <param name="_dbObjectSpGetDocNumOQUT"></param>
		/// <returns></returns>
		public static async Task<QuotationsToSAPResponse> CreateQuotations(IQuotDocument Quotations, string dbObject, CredentialHolder credential)
		{




			LogManager.LogMessage("DIAPI/PostSapData/CreateQuotations-- Ingresando..." + " CardCode: " + Quotations.CardCode, (int)Constants.LogTypes.SAP);
			var QuotationForLog = new JavaScriptSerializer().Serialize(Quotations);
			LogManager.LogMessage("DIAPI/PostSapData/CreateQuotations-- Objeto recibido Quotation: " + QuotationForLog + ", company: " + credential.DBCode, (int)Constants.LogTypes.SAP);


			try
			{
				if (Quotations.DocumentLines == null || Quotations.DocumentLines.Count == 0)
				{
					throw new Exception("El documento debe contener al menos una linea.");
				}

				JObject jObjectQuotations = (JObject)JsonConvert.DeserializeObject(new JavaScriptSerializer().Serialize(Quotations));

				if (Quotations.UdfTarget != null)
				{
					Quotations.UdfTarget.Where(x => x != null && x.Value != null).ToList().ForEach(x =>
					{
						if (x.Value.GetType().FullName.Contains("String") && !String.IsNullOrEmpty(x.Value.ToString()))
						{
							jObjectQuotations[x.Name] = x.Value.ToString();
						}
						else
						{
							Object oObject = Convert.ChangeType(x.Value, Type.GetType($"System.{x.Value.GetType().Name}"));
							jObjectQuotations[x.Name] = JToken.FromObject(oObject);
						}
					});
				}

				Resources connection = new Resources("https://clhna31.clavisco.com:50000/", "/b1s/v1/Login");

				Login loggin = new CLVSSLCONN.Models.Login()
				{
					CompanyDB = credential.DBCode,
					UserName = credential.SAPUser,
					Password = credential.SAPPass
				};



				

				JArray JLines = (JArray)JsonConvert.DeserializeObject(new JavaScriptSerializer().Serialize(Quotations.DocumentLines));

				List<string> unnescesariHeaderAtributes = new List<string>()
				{

					"DocEntry",
					"DocNum",
					"BaseEntry",
					"Series",
					"NumAtCard"
				};

				foreach (string propertie in unnescesariHeaderAtributes)
				{
					jObjectQuotations.Property(propertie).Remove();
				}

				foreach (JObject line in JLines)
				{
					line.Property("LineNum").Remove();
					line.Property("Serie").Remove();
				}
				jObjectQuotations.Property("DocumentLines").Remove();
				jObjectQuotations.Add("DocumentLines", JLines);





				string QuotationString = jObjectQuotations.ToString();



				HttpResponseMessage responseQuotation = await connection.SLRequest(loggin, "https://clhna31.clavisco.com:50000/b1s/v1/Quotations", "POST", new List<KeyValuePair<string, string>>(), QuotationString);




				DocumentModel oQuotation = new JavaScriptSerializer().Deserialize<DocumentModel>(responseQuotation.Content.ReadAsStringAsync().Result);


				if (responseQuotation.IsSuccessStatusCode)
				{
					return new QuotationsToSAPResponse()
					{
						DocEntry = oQuotation.DocEntry,
						DocNum = oQuotation.DocNum,
						Result = true
					};
				}
				else
				{
					SLResponse badResponse = new JavaScriptSerializer().Deserialize<SLResponse>(responseQuotation.Content.ReadAsStringAsync().Result);
					throw new Exception("La cotizacion no ha sido creada debido a: " + badResponse.error.message.value);
				}
			}
			catch
			{
				throw;
			}
		}

		#region Cotizacion
		/// <summary>
		/// Funcion que actualiza una Orden de Venta en SAP
		/// </summary>
		/// <param name="quotationEdit"></param>
		/// <param name="_credentials"></param>
		/// <returns></returns>
		public static async Task<UpdateQuotationsResponse> UpdateQuotation(IQuotDocument quotationEdit, CredentialHolder _credentials, string dbObject)
		{

			try
			{

				if (quotationEdit.DocumentLines == null || quotationEdit.DocumentLines.Count == 0)
				{
					throw new Exception("El documento debe contener al menos una lina.");
				}

				Resources connection = new Resources("https://clhna31.clavisco.com:50000/", "/b1s/v1/Login");

				Login loggin = new CLVSSLCONN.Models.Login()
				{
					CompanyDB = _credentials.DBCode,
					UserName = _credentials.SAPUser,
					Password = _credentials.SAPPass
				};

				//var slCompany = await connection.SLAuthentication(loggin, null);

				JObject jObjectQuotations = (JObject)JToken.FromObject(quotationEdit); //(JObject)JsonConvert.DeserializeObject(new JavaScriptSerializer().Serialize(quotationEdit));

				if (quotationEdit.UdfTarget != null)
				{
					quotationEdit.UdfTarget.Where(x => x != null && x.Value != null).ToList().ForEach(x =>
					{
						if (x.Value.GetType().FullName.Contains("String") && !String.IsNullOrEmpty(x.Value.ToString()))
						{
							jObjectQuotations[x.Name] = x.Value.ToString();
						}
						else
						{
							Object oObject = Convert.ChangeType(x.Value, Type.GetType($"System.{x.Value.GetType().Name}"));
							jObjectQuotations[x.Name] = JToken.FromObject(oObject);
						}
					});
				}


			

				JArray JLines = (JArray)JsonConvert.DeserializeObject(new JavaScriptSerializer().Serialize(quotationEdit.DocumentLines));

				List<string> unnecessariesProperties = new List<string>()
				{
					"DocNum",
					"DocEntry",
					"DocDate",
					"DocDueDate",
					
					"DocType",
					"Series",
					"CardCode",
					"CardName",
					"DocTotal",
					"DocTotalFC",
					"U_CLVS_POS_UniqueInvId",
					"UdfTarget"
				};


				List<string> notRequiredDataForLines = new List<string>()
				{
					"LineNum",
					"BaseEntry",
					"BaseLine",
					"BaseType",
					"Serie",
					"TaxRate"

				};



				// Se limpia el objeto de valores null
				var aux = jObjectQuotations.DeepClone();

				foreach (JProperty p in aux)
				{
					if (p.Value.Type == JTokenType.Null)
						jObjectQuotations.Property(p.Name).Remove();

				}

				// Se limpia el objeto de propiedades que no se requieren en el SL
				foreach (string propertie in unnecessariesProperties)
				{
					if (jObjectQuotations.Property(propertie).Type != JTokenType.Null)
						jObjectQuotations.Property(propertie).Remove();
				}

				foreach (JObject line in JLines)
				{
					foreach (string property in notRequiredDataForLines)
					{
						if (line.Property(property).Type != JTokenType.Null)
							line.Property(property).Remove();

					}
				}
				jObjectQuotations.Property("DocumentLines").Remove();
				jObjectQuotations.Add("DocumentLines", JLines);






				string QuotationString = jObjectQuotations.ToString();

				HttpResponseMessage responseQuotation = await connection.SLRequest(loggin, $"https://clhna31.clavisco.com:50000/b1s/v1/Quotations({quotationEdit.DocEntry})", "PATCH", new List<KeyValuePair<string, string>>() {

							 new KeyValuePair<string, string>("B1S-ReplaceCollectionsOnPatch","true")

						 }, QuotationString);

				if (responseQuotation.StatusCode == System.Net.HttpStatusCode.NoContent)
				{

					DocInfo docInfo = GetSapData.GetDocNumByDocEntryOQUT(quotationEdit.DocEntry, dbObject, _credentials);

					return new UpdateQuotationsResponse()
					{
						DocEntry = quotationEdit.DocEntry,
						DocNum = docInfo != null ? docInfo.DocNum : 0,
						Result = true
					};


				}
				else
				{
					SLResponse badResponse = new JavaScriptSerializer().Deserialize<SLResponse>(responseQuotation.Content.ReadAsStringAsync().Result);
					throw new Exception("La cotizacion no ha sido actualizada debido a: " + badResponse.error.message.value);
				}

			}
			catch
			{
				throw;
			}
		}
		#endregion

		/// <summary>
		/// Crea el pago de los documentos en SAP
		/// Recibe como parametro el modelo de pago, el modelo de company, el id del usuario, nombr de objetos para obtener fathercard, docnum
		/// </summary>
		/// <param name="_payment"></param>
		/// <param name="company"></param>
		/// <param name="user"></param>
		/// <param name="dbObjectSpGetFatherCard"></param>
		/// <param name="dbObjectSpGetDocNumORCT"></param>
		/// <returns></returns>
		public static async Task<PaymentSapResponse> CreatePaymentRecived(CreateSLRecivedPaymentModel _payment, string user, string _dbObjectSpGetFatherCard, CredentialHolder _credential)
		{

			try
			{

				string FatherCard = GetSapData.GetFatherCard(_credential, _payment.CardCode, _dbObjectSpGetFatherCard);

				if (!string.IsNullOrEmpty(FatherCard)) _payment.CardCode = FatherCard;

				// DocEntry = 0 quiere decir que ya las lineas del pago vinen con DocEntry
				return await CreatePaymentForInvoice(_payment, 0, user, _credential);

				//if (_payment != null)
				//{
				//    PaymentSapResponse PayResposeFromSap = new PaymentSapResponse();

				//    Company oCompany = null;
				//    oCompany = new Company();
				//    oCompany = DIAPICommon.CreateCompanyObject(company);

				//   PayResposeFromSap = pso.CreatePaymentRecived(_payment, company.DBName, user, FatherCard, ref oCompany);


				//    if (PayResposeFromSap.result)
				//    {
				//        DocInfo docInfo = GetSapData.GetDocNumByDocEntryORCT(company.DBCode, PayResposeFromSap.DocEntry, _dbObjectSpGetDocNumORCT);
				//        if (docInfo != null)
				//        {
				//            PayResposeFromSap.DocNum = docInfo.DocNum;
				//        }
				//        oCompany.Disconnect();
				//        oCompany = null;

				//        return PayResposeFromSap;
				//    }
				//    else
				//    {
				//        throw new Exception(PayResposeFromSap.errorInfo.Message);
				//    }
				//}
				//else
				//{
				//    throw new Exception("El documento debe contener al menos una linea");
				//}
			}
			catch (Exception ex)
			{
				int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
				string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;
				string ErrMsj = "-" + Convert.ToString("SAPDAO/PostSapData/CreatePaymentRecived Catch-- Code: " + code + " Message: " + message);
				LogManager.LogMessage(ErrMsj, (int)Constants.LogTypes.SAP);
				return new PaymentSapResponse
				{
					Result = false,
					Error = new ErrorInfo
					{
						Code = code,
						Message = message
					}
				};
			}
		}

		/// <summary>
		/// Metodo para realizar una cancelacion de un pago en SAP
		/// Recibe como parametro el modelo de cancelacion de pago, el modelo de Company
		/// </summary>
		/// <param name="canPay"></param>
		/// <param name="company"></param>
		/// <returns></returns>
		public static BaseResponse CancelPayment(CancelPayModel canPay, CredentialHolder _userCredentials)
		{
			try
			{
				PostDIAPIData pso = new PostDIAPIData();
				return pso.CancelPayment(canPay, _userCredentials);
			}
			catch (Exception ex)
			{
				int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
				string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;
				string ErrMsj = "-" + Convert.ToString("SAPDAO/PostSapData/CancelPayment Catch-- Code: " + code + " Message: " + message);
				LogManager.LogMessage(ErrMsj, (int)Constants.LogTypes.SAP);
				return new BaseResponse
				{
					Result = false,
					Error = new ErrorInfo
					{
						Code = code,
						Message = message
					}
				};
			}
		}

		/// <summary>
		/// Crea un ARInvoice en SAP
		/// </summary>
		/// <param name="CreateInvoice"></param>
		/// <param name="company"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		/// 


		public static async Task<InvoiceSapResponse> CreateInvoice(CreateSlInvoice CreateInvoice, string userId, string _dbObjectSpGetFatherCard, string _dbObjetSpGetNumFEOnline, CredentialHolder credentials)
		{
			var startTime = DateTime.Now;
			LogManager.LogMessage(string.Format("               PostSAPData>CreateInvoice. Start Time: {0}", startTime), 2);

			int DocEntry = 0;

			string oInvoiceSerilized = new JavaScriptSerializer().Serialize(CreateInvoice);

			LogModel oLogModel = PostData.CreateLog(13, oInvoiceSerilized, startTime).LogModel;

			SLResponse badResponse = new SLResponse();

			try
			{



				if (CreateInvoice != null && CreateInvoice.Invoice.DocumentLines != null && CreateInvoice.Invoice.DocumentLines.Count > 0)
				{
					string FatherCard = GetSapData.GetFatherCard(credentials, CreateInvoice.Invoice.CardCode, _dbObjectSpGetFatherCard);

					SuperV2_Entities db = new SuperV2_Entities();

					int SerieType = (int)Constants.SerialType.Invoice;

					UserAssign userA = db.UserAssign.Where(x => x.UserId == userId).FirstOrDefault();

					DiapiSerie diapiSeries = userA.V_SeriesByUser.Where(x => x.V_Series.DocType == SerieType && x.V_Series.Type == (int)COMMON.Constants.SerieNumberingType.Online).Select(x => new DiapiSerie
					{
						NumType = x.V_Series.Numbering,
						Serie = x.V_Series.Serie

					}).FirstOrDefault();

					if (diapiSeries == null)
					{
						throw new Exception(string.Format("No se ha definido una serie para crear facturas Online"));
					}


					DocumentModel invoiceToSend = CreateInvoice.Invoice;


					invoiceToSend.Series = diapiSeries.Serie;




					Resources connection = new Resources("https://clhna31.clavisco.com:50000/", "/b1s/v1/Login");



					CLVSSLCONN.Models.Login loggin = new CLVSSLCONN.Models.Login()
					{
						CompanyDB = credentials.DBCode,
						UserName = credentials.SAPUser,//"CLAVISCO\\cl.clavis.desarrollo",
						Password = credentials.SAPPass//"D3s4rr0ll0+"
					};




					JObject jObjectInvoice = (JObject)JsonConvert.DeserializeObject(new JavaScriptSerializer().Serialize(invoiceToSend));

					if (CreateInvoice.Invoice.UdfTarget != null)
					{
						CreateInvoice.Invoice.UdfTarget.Where(x => x != null && x.Value != null).ToList().ForEach(x =>
						{
							if (x.Value.GetType().FullName.Contains("String") && !String.IsNullOrEmpty(x.Value.ToString()))
							{
								jObjectInvoice[x.Name] = x.Value.ToString();
							}
							else
							{
								Object oObject = Convert.ChangeType(x.Value, Type.GetType($"System.{x.Value.GetType().Name}"));
								jObjectInvoice[x.Name] = JToken.FromObject(oObject);
							}
						});
					}


					JArray JLines = (JArray)JsonConvert.DeserializeObject(new JavaScriptSerializer().Serialize(invoiceToSend.DocumentLines));

					List<string> unnecessariesProperties = new List<string>()
					{
						"DocEntry",
						"DocNum",

						"NumAtCard",
						"DocDueDate"
					};


					foreach (string propertie in unnecessariesProperties)
					{
						jObjectInvoice.Property(propertie).Remove();
					}


					foreach (JObject line in JLines)
					{
						line.Property("LineNum").Remove();
						line.Property("Serie").Remove();
					}

					jObjectInvoice.Property("DocumentLines").Remove();
					jObjectInvoice.Add("DocumentLines", JLines);




					//Factura que se envia a SL
					string JSONInvoice = jObjectInvoice.ToString();




					var _startTimeSapDocument = DateTime.Now;
					PostData.UpdateLog(oLogModel, "", "", _startTimeSapDocument, null, "", null, null, null, null, "");

					HttpResponseMessage createInvoiceRequestSL = await connection.SLRequest(loggin, "https://clhna31.clavisco.com:50000/b1s/v1/Invoices", "POST", new List<KeyValuePair<string, string>>(), JSONInvoice);

					var endTimeSapDocument = DateTime.Now;
					PostData.UpdateLog(oLogModel, "", "", _startTimeSapDocument, endTimeSapDocument, (_startTimeSapDocument - endTimeSapDocument).ToString(), null, null, null, null, "");



					DocumentModel invoiceResponseDataSL = new JavaScriptSerializer().Deserialize<DocumentModel>(createInvoiceRequestSL.Content.ReadAsStringAsync().Result);



					if (createInvoiceRequestSL.IsSuccessStatusCode)
					{
						DocEntry = invoiceResponseDataSL.DocEntry;

						if (CreateInvoice.Payment != null)
						{
							if (CreateInvoice.Payment.PaymentInvoices != null)
							{
								foreach (var pay in CreateInvoice.Payment.PaymentInvoices)
								{
									pay.DocEntry = DocEntry;

								};


								diapiSeries = null;

								SerieType = (int)Constants.SerialType.Payment;

								diapiSeries = userA.V_SeriesByUser.Where(x => x.V_Series.DocType == SerieType).Select(x => new DiapiSerie
								{
									NumType = x.V_Series.Numbering,
									Serie = x.V_Series.Serie
								}).FirstOrDefault();

								if (diapiSeries == null)
								{
									throw new Exception(string.Format("No se ha definido una serie para crear pagos"));
								}


								BasePayment Payment = CreateInvoice.Payment;



								Payment.Series = diapiSeries.Serie;


								if (Payment.PaymentCreditCards != null && Payment.PaymentCreditCards.Count > 0)
								{
									foreach (var cc in CreateInvoice.Payment.PaymentCreditCards)
									{

										string fecha = cc.CardValid;
										string[] array = fecha.Split('/');
										var lastDayOfMonth = DateTime.DaysInMonth(Convert.ToInt32("20" + array[1]), Convert.ToInt32(array[0]));
										var day = Convert.ToInt32(lastDayOfMonth);
										DateTime nuevafecha = new DateTime(Convert.ToInt32("20" + array[1]), Convert.ToInt32(array[0]), day);
										cc.CardValidUntil = nuevafecha.ToString("yyyy-MM-dd");
									}
								}

								JObject jObjectPayment = (JObject)JsonConvert.DeserializeObject(new JavaScriptSerializer().Serialize(Payment));

								if (Payment.UDFs != null)
								{
									Payment.UDFs.Where(x => x != null && x.Value != null).ToList().ForEach(x =>
									{
										if (x.Value.GetType().FullName.Contains("String") && !String.IsNullOrEmpty(x.Value.ToString()))
										{
											jObjectPayment[x.Name] = x.Value.ToString();
										}
										else
										{
											Object oObject = Convert.ChangeType(x.Value, Type.GetType($"System.{x.Value.GetType().Name}"));
											jObjectPayment[x.Name] = JToken.FromObject(oObject);
										}
									});
								}


								string JSONPayment = jObjectPayment.ToString();//new JavaScriptSerializer().Serialize(Payment);													


								HttpResponseMessage createPaymentRequestSL = await connection.SLRequest(loggin, "https://clhna31.clavisco.com:50000/b1s/v1/IncomingPayments", "POST", new List<KeyValuePair<string, string>>(), JSONPayment);


								// var debbug = createPaymentRequestSL.Content.ReadAsStringAsync().Result;


								BasePayment paymentResponseDataSL = new JavaScriptSerializer().Deserialize<BasePayment>(createPaymentRequestSL.Content.ReadAsStringAsync().Result);

								if (!createPaymentRequestSL.IsSuccessStatusCode)
								{
									badResponse = new JavaScriptSerializer().Deserialize<SLResponse>(createPaymentRequestSL.Content.ReadAsStringAsync().Result);
									throw new Exception("Factura registrada, pero no se le pudo hacer el pago debido a: " + badResponse.error.message.value);
								}
							}
						}
					}
					else
					{
						badResponse = new JavaScriptSerializer().Deserialize<SLResponse>(createInvoiceRequestSL.Content.ReadAsStringAsync().Result);
						throw new Exception("Error creando la factura, causa: " + badResponse.error.message.value);
					}

					InvoiceSapResponse InvoiceSapResponse = new InvoiceSapResponse();

					DocInfo docInfo = GetSapData.GetDocNumByDocEntry(credentials, invoiceResponseDataSL.DocEntry, _dbObjetSpGetNumFEOnline);

					if (docInfo != null)
					{
						InvoiceSapResponse.DocEntry = invoiceResponseDataSL.DocEntry;
						InvoiceSapResponse.DocNum = docInfo.DocNum;
						InvoiceSapResponse.NumDocFe = docInfo.NumFE;
						InvoiceSapResponse.Consecutivo = docInfo.ClaveFE;
					}

					var EndTime = DateTime.Now;

					LogManager.LogMessage(string.Format("               PostSAPData>CreateInvoice. End Time: {0}| Total Time Time: {1}", DateTime.Now, DateTime.Now - startTime), (int)Constants.LogTypes.API);

					DAO.PostData.UpdateLog(oLogModel, "", "", null, null, null, startTime, EndTime, null, null, "");
					InvoiceSapResponse.Result = true;

					return InvoiceSapResponse;
				}
				else
				{
					throw new Exception("El documento debe contener al menos una linea");
				}
			}
			catch (Exception ex)
			{
				int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
				string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;
				string ErrMsj = "-" + Convert.ToString("SAPDAO/PostSapData/CreateInvoice Catch-- Code: " + code + " Message: " + message);
				LogManager.LogMessage(ErrMsj, (int)Constants.LogTypes.SAP);
				DAO.PostData.UpdateLog(oLogModel, "", "", null, null, null, startTime, null, null, null, ex.Message);
				throw;

			}
		}

		public static DocumentUpdateResponse UpdateSaleOrderDIAPI(ISaleDocument saleOrder, CredentialHolder _UserCredentials, string dbObject)
		{
			PostDIAPIData pso = new PostDIAPIData();
			DocumentUpdateResponse DocumentUpdateResponse = new DocumentUpdateResponse();
			try
			{
				if (saleOrder != null && saleOrder.DocumentLines != null && saleOrder.DocumentLines.Count > 0)
				{
					DocumentUpdateResponse QuotationsResposeFromSap = new DocumentUpdateResponse();

					QuotationsResposeFromSap = pso.UpdateSaleOrder(saleOrder, _UserCredentials);
					if (QuotationsResposeFromSap.Result)
					{
						DocInfo docInfo = GetSapData.GetDocNumByDocEntryORDR(_UserCredentials, QuotationsResposeFromSap.DocEntry, dbObject);
						if (docInfo != null)
						{
							QuotationsResposeFromSap.DocNum = docInfo.DocNum;
						}
						return QuotationsResposeFromSap;
					}
					else
					{
						throw new Exception(QuotationsResposeFromSap.Error.Message);
					}
				}
				else
				{
					throw new Exception("El documento debe contener al menos una linea");
				}
			}
			catch
			{
				throw;

			}
		}

		public static object UpdateQuotationDIAPI(IQuotDocument quotationEdit, CredentialHolder _UserCredentials, string dbObject, CredentialHolder _credentialHolder)
		{
			// PostDIAPIData pso = new PostDIAPIData();
			//Company oCompany = null;
			// oCompany = new Company();
			PostDIAPIData pso = new PostDIAPIData();
			try
			{
				if (quotationEdit != null && quotationEdit.DocumentLines != null && quotationEdit.DocumentLines.Count > 0)
				{
					UpdateQuotationsResponse QuotationsResposeFromSap = new UpdateQuotationsResponse();

					QuotationsResposeFromSap = pso.UpdateQuotation(quotationEdit, _UserCredentials);
					if (QuotationsResposeFromSap.Result)
					{
						DocInfo docInfo = GetSapData.GetDocNumByDocEntryOQUT(QuotationsResposeFromSap.DocEntry, dbObject, _credentialHolder);
						if (docInfo != null)
						{
							QuotationsResposeFromSap.DocNum = docInfo.DocNum;
						}
						return QuotationsResposeFromSap;
					}
					else
					{
						throw new Exception(QuotationsResposeFromSap.Error.Message);
					}
				}
				else
				{
					throw new Exception("El documento debe contener al menos una linea");
				}
			}
			catch
			{
				throw;
			}
		}



		/// <summary>
		/// #001 Metodo para generar los pagos de una factura que ya fue creada y tuvo problemas al crear los pagos previamente.
		/// </summary>
		/// <param name="CreateInvoice"></param>
		/// <param name="company"></param>
		/// <param name="userId"></param>
		/// <param name="_dbObjectSpFatherCard"></param>
		/// <returns></returns>
		public static PaymentSapResponse CreatePaymentForInvoice(CreateInvoice CreateInvoice, CredentialHolder company, string userId, string _dbObjectSpFatherCard)
		{
			string FatherCard = GetSapData.GetFatherCard(company, CreateInvoice.Payment.CardCode, _dbObjectSpFatherCard);

			Company oCompany = DIAPICommon.CreateCompanyObject(company);
			try
			{
				if (CreateInvoice.Payment != null)
				{


					if (CreateInvoice.Payment.V_PaymentLines != null)
					{
						foreach (var pay in CreateInvoice.Payment.V_PaymentLines)
						{
							pay.DocEntry = CreateInvoice.Invoice.DocEntry;

							if (CreateInvoice.Payment.V_CreditCards != null && CreateInvoice.Payment.V_CreditCards.Count > 0)
							{
								foreach (MODELS.CreditCards card in CreateInvoice.Payment.V_CreditCards)
								{//cambiar por docnum
									card.VoucherNum = card.VoucherNum; //Convert.ToString(DocNum);
									card.OwnerIdNum = card.OwnerIdNum;
									card.CreditCardNumber = card.CreditCardNumber;
									//card.IsManualEntry = card.IsManualEntry;
								}
							}
						};

						PostDIAPIData pso = new PostDIAPIData();

						PaymentSapResponse response = pso.CreatePayment(CreateInvoice.Payment, company.DBCode, userId, FatherCard, ref oCompany);



						return response;

					}

				}
				return new PaymentSapResponse()
				{
					Result = false
				};
			}
			catch
			{
				throw;
			}
			finally
			{
				if (oCompany != null && oCompany.Connected)
				{
					oCompany.Disconnect();
					oCompany = null;
				}
			}


		}


		public static async Task<PaymentSapResponse> CreatePaymentForInvoice(BasePayment _Payment, int _DocEntry, string userId, CredentialHolder credentials)
		{
			//string FatherCard = GetSapData.GetFatherCard(company, CreateInvoice.Payment.CardCode, dbObjectFatherCard);


			try
			{
				int DocNum = 0;
				int DocEntry = 0;
				if (_Payment == null)
				{
					throw new Exception("No se pudo realizar el pago");
				}

				if (_Payment.PaymentInvoices != null)
				{

					if (_DocEntry != 0)
						foreach (var pay in _Payment.PaymentInvoices)
						{
							pay.DocEntry = _DocEntry;

						};




					SuperV2_Entities db = new SuperV2_Entities();

					int SerieType = (int)Constants.SerialType.Payment;

					UserAssign userA = db.UserAssign.Where(x => x.UserId == userId).FirstOrDefault();

					DiapiSerie diapiSeries = userA.V_SeriesByUser.Where(x => x.V_Series.DocType == SerieType).Select(x => new DiapiSerie
					{
						NumType = x.V_Series.Numbering,
						Serie = x.V_Series.Serie
					}).FirstOrDefault();

					if (diapiSeries == null)
					{
						throw new Exception(string.Format("No se ha definido una serie para crear pagos"));
					}






					if (_Payment.UDFs != null)
					{
						_Payment.UDFs.ForEach(x =>
						{
							Common.SetObjectProperty<BasePayment>(x.Name, x.Value, ref _Payment);
						});
					}




					_Payment.Series = diapiSeries.Serie;


					if (_Payment.PaymentCreditCards != null && _Payment.PaymentCreditCards.Count > 0)
					{
						foreach (var cc in _Payment.PaymentCreditCards)
						{

							string fecha = cc.CardValid;
							string[] array = fecha.Split('/');

							var lastDayOfMonth = DateTime.DaysInMonth(Convert.ToInt32("20" + array[1]), Convert.ToInt32(array[0]));

							var day = Convert.ToInt32(lastDayOfMonth);

							DateTime nuevafecha = new DateTime(Convert.ToInt32("20" + array[1]), Convert.ToInt32(array[0]), day);

							var tst = nuevafecha.ToString("yyyy-MM-dd");


							cc.CardValidUntil = nuevafecha.ToString("yyyy-MM-dd");
						}
					}
					JObject jObjectPayment = (JObject)JsonConvert.DeserializeObject(new JavaScriptSerializer().Serialize(_Payment));

					if (_Payment.UDFs != null)
					{
						_Payment.UDFs.Where(x => x != null && x.Value != null).ToList().ForEach(x =>
						{
							if (x.Value.GetType().FullName.Contains("String") && !String.IsNullOrEmpty(x.Value.ToString()))
							{
								jObjectPayment[x.Name] = x.Value.ToString();
							}
							else
							{
								Object oObject = Convert.ChangeType(x.Value, Type.GetType($"System.{x.Value.GetType().Name}"));
								jObjectPayment[x.Name] = JToken.FromObject(oObject);
							}
						});
					}


					var connection = new Resources("https://clhna31.clavisco.com:50000/", "/b1s/v1/Login");

					var userAsing = DAO.GetData.GetUserMappId(userId);

					var loggin = new CLVSSLCONN.Models.Login()
					{
						CompanyDB = credentials.DBCode,
						UserName = credentials.SAPUser,// "CLAVISCO\\cl.clavis.desarrollo",
						Password = credentials.SAPPass// "D3s4rr0ll0+"
					};
					string JSONPayment = jObjectPayment.ToString();//new JavaScriptSerializer().Serialize(_Payment);





					var createPaymentRequestSL = await connection.SLRequest(loggin, "https://clhna31.clavisco.com:50000/b1s/v1/IncomingPayments", "POST", new List<KeyValuePair<string, string>>(), JSONPayment);

					// Esto es MERAMENTE DE PRUEBAS NO HACE NADA EN EL FLUJO solo se utiliza para conocer como se mapean los datos.
					//var t = await connection.SLRequest(loggin, "https://clhna31.clavisco.com:50000/b1s/v1/IncomingPayments(35606)", "GET", new List<KeyValuePair<string, string>>(), "");
					//var debbug = t.Content.ReadAsStringAsync().Result;


					var paymentResponseDataSL = new JavaScriptSerializer().Deserialize<BasePayment>(createPaymentRequestSL.Content.ReadAsStringAsync().Result);

					if (!createPaymentRequestSL.IsSuccessStatusCode)
					{
						SLResponse badResponse = new JavaScriptSerializer().Deserialize<SLResponse>(createPaymentRequestSL.Content.ReadAsStringAsync().Result);
						throw new Exception("Factura registrada, pero no se le pudo hacer el pago debido a: " + badResponse.error.message.value);
					}

					DocEntry = paymentResponseDataSL.DocEntry;
					DocNum = paymentResponseDataSL.DocNum;
				}

				return new PaymentSapResponse()
				{
					Result = true,
					DocEntry = DocEntry,
					DocNum = DocNum

				};
			}
			catch
			{
				throw;
			}
		}



		public static InvoiceSapResponse SyncCreateInvoice(OFF_CreateInvoice createInvoice, Companys company, string userId, string _dbObjectSpGetNumFEOnliney, CredentialHolder _userCredentials)
		{
			PostDIAPIData pso = new PostDIAPIData();

			InvoiceSapResponse InvoiceSapResponse = new InvoiceSapResponse();

			try
			{

				Company oCompany = null;
				oCompany = new Company();

				oCompany = DIAPICommon.CreateCompanyObject(_userCredentials);

				InvoiceSapResponse = pso.SyncCreateInvoice(createInvoice.Invoice, company, userId, ref oCompany);

				if (InvoiceSapResponse.Result)
				{
					DocInfo docInfo = GetSapData.GetDocNumByDocEntry(_userCredentials, InvoiceSapResponse.DocEntry, "OINV", _dbObjectSpGetNumFEOnliney);
					if (docInfo != null)
					{
						InvoiceSapResponse.DocNum = docInfo.DocNum;
						InvoiceSapResponse.NumDocFe = docInfo.NumFE;
						InvoiceSapResponse.Consecutivo = docInfo.ClaveFE;
					}

					//Create Payment
					if (createInvoice.Payments != null && createInvoice.Payments.Count() > 0)
					{
						foreach (var pay in createInvoice.Payments)
						{
							pay.DocEntry = InvoiceSapResponse.DocEntry;
						};

						InvoiceSapResponse.PaymentResponse = pso.SyncCreatePayment(createInvoice.Payments[0], company, userId, createInvoice.Invoice.FatherCard, ref oCompany, createInvoice.Invoice.DocCur);
					}

					if (oCompany.Connected)
					{
						oCompany.Disconnect();
					}

					oCompany = null;

					return InvoiceSapResponse;
				}
				else
				{
					throw new Exception(InvoiceSapResponse.Error.Message);
				}

				// return InvoiceSapResponse;

				//if (CreateInvoice != null && CreateInvoice.Invoice.InvoiceLinesList != null && CreateInvoice.Invoice.InvoiceLinesList.Count > 0)
				//{
				//    InvoiceSapResponse InvoiceSapResponse = new InvoiceSapResponse();
				//    string FatherCard = GetSapData.GetFatherCard(company, CreateInvoice.Payment.CardCode);

				//    InvoiceSapResponse = pso.CreateInvoice(CreateInvoice, company, userId, FatherCard);
				//    if (InvoiceSapResponse.result)
				//    {
				//        InvoiceSapResponse.DocNum = GetSapData.GetDocNumByDocEntry(company.DBCode, InvoiceSapResponse.DocEntry, "OINV");

				//        //Create Payment
				//        if (CreateInvoice.Payment != null)
				//        {
				//            foreach (var pay in CreateInvoice.Payment.V_PaymentLines)
				//            {

				//                pay.DocEntry = InvoiceSapResponse.DocEntry;
				//                if (CreateInvoice.Payment.V_CreditCards != null && CreateInvoice.Payment.V_CreditCards.Count > 0)
				//                {
				//                    foreach (MODELS.CreditCards card in CreateInvoice.Payment.V_CreditCards)
				//                    {//cambiar por docnum
				//                        card.VoucherNum = "Entry" + Convert.ToString(InvoiceSapResponse.DocNum);
				//                    }
				//                }
				//            };
				//            InvoiceSapResponse.PaymentResponse = pso.CreatePayment(CreateInvoice.Payment, company, userId, FatherCard);
				//        }

				//        return InvoiceSapResponse;
				//    }
				//    else
				//    {
				//        throw new Exception(InvoiceSapResponse.errorInfo.Message);
				//    }
				//}
				//else
				//{
				//    throw new Exception("El documento debe contener al menos una linea");
				//}
			}
			catch (Exception ex)
			{
				int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
				string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;
				string ErrMsj = "-" + Convert.ToString("SAPDAO/PostSapData/SyncCreateInvoice Catch-- Code: " + code + " Message: " + message);
				LogManager.LogMessage(ErrMsj, (int)Constants.LogTypes.SAP);
				return new InvoiceSapResponse
				{
					Result = false,
					Error = new ErrorInfo
					{
						Code = code,
						Message = message
					}
				};
			}
		}

		/// <summary>
		/// Crea el item, ademas que genera las instancias necesarias para poder conectar con sap
		/// </summary>
		/// <param name="_itemModel"></param>
		/// <param name="_UserCredentials"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		public static ItemsResponse CreateItem(ItemsModel _itemModel, CredentialHolder _UserCredentials)
		{
			var startTime = DateTime.Now;
			LogManager.LogMessage(string.Format("PostSAPData>CreateItem. Start Time: {0}", startTime), 2);

			PostDIAPIData oPostDIAPIData = new PostDIAPIData();

			Company oCompany = null;
			oCompany = new Company();

			try
			{
				oCompany = DIAPICommon.CreateCompanyObject(_UserCredentials);
				return oPostDIAPIData.CreateItem(_itemModel, ref oCompany);
			}
			catch (Exception ex)
			{
				int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
				string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;
				string ErrMsj = "-" + Convert.ToString("SAPDAO/PostSapData/CreateItem Catch-- Code: " + code + " Message: " + message);
				LogManager.LogMessage(ErrMsj, (int)Constants.LogTypes.SAP);

				if (oCompany.Connected)
				{
					oCompany.Disconnect();
					oCompany = null;
				}

				return new ItemsResponse
				{
					Result = false,
					Error = new ErrorInfo
					{
						Code = code,
						Message = message
					}
				};
			}
		}

		/// <summary>
		/// Actualiza el item con los nuevos valores, ademas de agregar o editar los codigos barras y las listas de precios
		/// </summary>
		/// <param name="_itemModel"></param>
		/// <param name="company"></param>
		/// <param name="userId"></param>
		/// <param name="_barcodes"></param>
		/// <returns></returns>
		public static ItemsResponse UpdateItem(ItemsModel _itemModel, CredentialHolder _UserCredentials, List<ItemsBarcodeModel> _barcodes)
		{
			var startTime = DateTime.Now;
			LogManager.LogMessage(string.Format("PostSAPData>UpdateItem. Start Time: {0}", startTime), 2);

			PostDIAPIData oPostDIAPIData = new PostDIAPIData();

			Company oCompany = null;
			oCompany = new Company();

			try
			{
				oCompany = DIAPICommon.CreateCompanyObject(_UserCredentials);
				return oPostDIAPIData.UpdateItem(_itemModel, ref oCompany, _barcodes);
			}
			catch (Exception ex)
			{
				int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
				string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;
				string ErrMsj = "-" + Convert.ToString("SAPDAO/PostSapData/UpdateItem Catch-- Code: " + code + " Message: " + message);
				LogManager.LogMessage(ErrMsj, (int)Constants.LogTypes.SAP);

				if (oCompany.Connected)
				{
					oCompany.Disconnect();
					oCompany = null;
				}


				return new ItemsResponse
				{
					Result = false,
					Error = new ErrorInfo
					{
						Code = code,
						Message = message
					}
				};
			}
		}

		public static ItemsResponse CreateGoodsReceipt(GoodsReceipt _goodsReceipt, CredentialHolder _UserCredentials, string _dbObjectSpGetGoodReceiptDocNum)
		{

			var startTime = DateTime.Now;
			LogManager.LogMessage(string.Format("PostSAPData>CreateGoodsRecipt. Start Time: {0}", startTime), 2);

			PostDIAPIData oPostDIAPIData = new PostDIAPIData();

			Company oCompany = null;
			oCompany = new Company();

			try
			{








				oCompany = DIAPICommon.CreateCompanyObject(_UserCredentials);


				ItemsResponse responseDIAPI = oPostDIAPIData.CreateGoodsReceipt(_goodsReceipt, ref oCompany);

				DocInfo docInfo = GetSapData.GetGoodReceiptDocNum(_UserCredentials, _dbObjectSpGetGoodReceiptDocNum, responseDIAPI.DocEntry);

				if (docInfo != null)
				{
					responseDIAPI.DocNum = docInfo.DocNum;
				}


				return responseDIAPI;
			}
			catch
			{
				if (oCompany.Connected)
				{
					oCompany.Disconnect();
					oCompany = null;
				}
				throw;
			}

		}

		public static InvoiceSapResponse CreateInvoiceNc(CreateInvoice CreateInvoice, CredentialHolder _UserCredentials, string userId, string _dbObjectSpGetFatherCard)
		{
			var startTime = DateTime.Now;
			LogManager.LogMessage(string.Format("PostSAPData>CreateInvoiceNc. Start Time: {0}", startTime), 2);

			PostDIAPIData pso = new PostDIAPIData();
			//int DocEntry = 0;

			InvoiceSapResponse InvoiceSapResponse = new InvoiceSapResponse();
			Company oCompany = null;
			oCompany = new Company();

			try
			{
				if (CreateInvoice != null && CreateInvoice.Invoice.InvoiceLinesList != null && CreateInvoice.Invoice.InvoiceLinesList.Count > 0) // CreateInvoice.Payment != null Check for Payment Lines
				{
					string FatherCard = GetSapData.GetFatherCard(_UserCredentials, CreateInvoice.Payment.CardCode, _dbObjectSpGetFatherCard);

					oCompany = DIAPICommon.CreateCompanyObject(_UserCredentials);

					InvoiceSapResponse = pso.CreateInvoiceNc(CreateInvoice, _UserCredentials.DBCode, userId, FatherCard, ref oCompany);
					int DocNumNc = InvoiceSapResponse.DocNum;
					if (InvoiceSapResponse.Result)
					{
						//DocEntry = InvoiceSapResponse.DocEntry; //Get DocEntry ASAP, store it in memory for fast access

						//DocInfo docInfo = GetSapData.GetDocNumByDocEntry(company.DBCode, InvoiceSapResponse.DocEntry, "ORIN");
						//if (docInfo != null)
						//{
						//    InvoiceSapResponse.DocNum = docInfo.DocNum;
						//    InvoiceSapResponse.NumDocFe = docInfo.NumFE;
						//    InvoiceSapResponse.Consecutivo = docInfo.ClaveFE;
						//}
						//if (CreateInvoice.Payment != null && InvoiceSapResponse.DocNum > 0)
						//{
						//    if (CreateInvoice.Payment.V_PaymentLines != null)
						//    {
						//        foreach (var pay in CreateInvoice.Payment.V_PaymentLines)
						//        {f
						//            pay.DocEntry = DocEntry;

						//            if (CreateInvoice.Payment.V_CreditCards != null && CreateInvoice.Payment.V_CreditCards.Count > 0)
						//            {
						//                foreach (MODELS.CreditCards card in CreateInvoice.Payment.V_CreditCards)
						//                {//cambiar por docnum
						//                    card.VoucherNum = "Entry" + card.OwnerIdNum; //Convert.ToString(DocNum);
						//                }
						//            }
						//        };

						//        InvoiceSapResponse.PaymentResponse = pso.CreatePayment(CreateInvoice.Payment, company.DBName, userId, FatherCard, ref oCompany);
						//        InvoiceSapResponse.DocNum = DocNumNc;
						//    }
						//}
					}
					else
					{
						throw new Exception(InvoiceSapResponse.Error.Message);
					}

					oCompany.Disconnect();
					oCompany = null;

					LogManager.LogMessage(string.Format("               PostSAPData>CreateInvoiceNc. End Time: {0}| Total Time Time: {1}", DateTime.Now, DateTime.Now - startTime), (int)Constants.LogTypes.API);

					return InvoiceSapResponse;
				}
				else
				{
					throw new Exception("El documento debe contener al menos una linea");
				}
			}
			catch (Exception ex)
			{
				int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
				string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;
				string ErrMsj = "-" + Convert.ToString("SAPDAO/PostSapData/CreateInvoiceNc Catch-- Code: " + code + " Message: " + message);
				LogManager.LogMessage(ErrMsj, (int)Constants.LogTypes.SAP);

				if (oCompany.Connected)
				{
					oCompany.Disconnect();
					oCompany = null;
				}

				return new InvoiceSapResponse
				{
					Result = false,
					Error = new ErrorInfo
					{
						Code = code,
						Message = message
					}
				};
			}
		}

		public static ItemsResponse CreateGoodsReceiptReturn(GoodsReceipt _goodsRecipt, CredentialHolder _UserCredentials, string _dbObjectSpGetGoodReceipReturnDocNum)
		{
			var startTime = DateTime.Now;
			LogManager.LogMessage(string.Format("                PostSAPData>CreateGoodsReceiptReturn. Start Time: {0}", startTime), 2);

			PostDIAPIData oPostDIAPIData = new PostDIAPIData();

			Company oCompany = null;
			oCompany = new Company();

			try
			{
				oCompany = DIAPICommon.CreateCompanyObject(_UserCredentials);

				ItemsResponse responseDIAPI = oPostDIAPIData.CreateGoodsReceiptReturn(_goodsRecipt, ref oCompany);

				DocInfo docInfo = GetSapData.GetGoodReceipReturnDocNum(_UserCredentials, responseDIAPI.DocEntry, _dbObjectSpGetGoodReceipReturnDocNum);

				if (docInfo != null)
					responseDIAPI.DocNum = docInfo.DocNum;


				return responseDIAPI;
			}
			catch
			{

				if (oCompany.Connected)
				{
					oCompany.Disconnect();
					oCompany = null;
				}
				throw;
			}

		}


		public static async Task<PaymentSapResponse> CreatePayApInvoices(BasePayment _Payment, string userId, string _dbObjectSpFatherCard, CredentialHolder _credentials)
		{
			// PostDIAPIData pso = new PostDIAPIData();
			try
			{
				string FatherCard = GetSapData.GetFatherCard(_credentials, _Payment.CardCode, _dbObjectSpFatherCard);
				if (_Payment != null)
				{


					if (!string.IsNullOrEmpty(FatherCard)) _Payment.CardCode = FatherCard;

					int DocNum = 0;
					int DocEntry = 0;
					if (_Payment == null)
					{
						throw new Exception("No se pudo realizar el pago");
					}



					SuperV2_Entities db = new SuperV2_Entities();

					int SerieType = (int)Constants.SerialType.Payment;

					UserAssign userA = db.UserAssign.Where(x => x.UserId == userId).FirstOrDefault();

					DiapiSerie diapiSeries = userA.V_SeriesByUser.Where(x => x.V_Series.DocType == SerieType).Select(x => new DiapiSerie
					{
						NumType = x.V_Series.Numbering,
						Serie = x.V_Series.Serie
					}).FirstOrDefault();

					if (diapiSeries == null)
					{
						throw new Exception(string.Format("No se ha definido una serie para crear pagos"));
					}
					if (_Payment.UDFs != null)
					{
						_Payment.UDFs.ForEach(x =>
						{
							Common.SetObjectProperty<BasePayment>(x.Name, x.Value, ref _Payment);
						});
					}





					_Payment.Series = diapiSeries.Serie;


					if (_Payment.PaymentCreditCards != null && _Payment.PaymentCreditCards.Count > 0)
					{
						foreach (var cc in _Payment.PaymentCreditCards)
						{

							string fecha = cc.CardValid;
							string[] array = fecha.Split('/');

							var lastDayOfMonth = DateTime.DaysInMonth(Convert.ToInt32("20" + array[1]), Convert.ToInt32(array[0]));

							var day = Convert.ToInt32(lastDayOfMonth);

							DateTime nuevafecha = new DateTime(Convert.ToInt32("20" + array[1]), Convert.ToInt32(array[0]), day);

							var tst = nuevafecha.ToString("yyyy-MM-dd");


							cc.CardValidUntil = nuevafecha.ToString("yyyy-MM-dd");
						}
					}

					JObject jObjectPayment = (JObject)JsonConvert.DeserializeObject(new JavaScriptSerializer().Serialize(_Payment));

					if (_Payment.UDFs != null)
					{
						_Payment.UDFs.Where(x => x != null && x.Value != null).ToList().ForEach(x =>
						{
							if (x.Value.GetType().FullName.Contains("String") && !String.IsNullOrEmpty(x.Value.ToString()))
							{
								jObjectPayment[x.Name] = x.Value.ToString();
							}
							else
							{
								Object oObject = Convert.ChangeType(x.Value, Type.GetType($"System.{x.Value.GetType().Name}"));
								jObjectPayment[x.Name] = JToken.FromObject(oObject);
							}
						});
					}




					var connection = new Resources("https://clhna31.clavisco.com:50000/", "/b1s/v1/Login");

					var userAsing = DAO.GetData.GetUserMappId(userId);

					var loggin = new CLVSSLCONN.Models.Login()
					{
						CompanyDB = _credentials.DBCode,
						UserName = _credentials.SAPUser,// "CLAVISCO\\cl.clavis.desarrollo",
						Password = _credentials.SAPPass// "D3s4rr0ll0+"
					};
					string JSONPayment = jObjectPayment.ToString();//new JavaScriptSerializer().Serialize(_Payment);

					// var slCompany = await connection.SLAuthentication(loggin, null);

					var createPaymentRequestSL = await connection.SLRequest(loggin, "https://clhna31.clavisco.com:50000/b1s/v1/VendorPayments", "POST", new List<KeyValuePair<string, string>>(), JSONPayment);

					// var createPaymentRequestSL = await connection.SLRequest("https://clhna31.clavisco.com:50000/b1s/v1/VendorPayments", "GET", slCompany.Cookie.SessionId, "");

					var debbug = createPaymentRequestSL.Content.ReadAsStringAsync().Result;


					var paymentResponseDataSL = new JavaScriptSerializer().Deserialize<BasePayment>(createPaymentRequestSL.Content.ReadAsStringAsync().Result);

					if (!createPaymentRequestSL.IsSuccessStatusCode)
					{
						SLResponse badResponse = new JavaScriptSerializer().Deserialize<SLResponse>(createPaymentRequestSL.Content.ReadAsStringAsync().Result);
						throw new Exception("No se pudo efectuar el pago debido a: " + badResponse.error.message.value);
					}

					DocEntry = paymentResponseDataSL.DocEntry;
					DocNum = paymentResponseDataSL.DocNum;


					return new PaymentSapResponse()
					{
						Result = true,
						DocEntry = DocEntry,
						DocNum = DocNum

					};





					//PaymentSapResponse PayResposeFromSap = new PaymentSapResponse();

					//Company oCompany = null;
					//oCompany = new Company();
					//oCompany = DIAPICommon.CreateCompanyObject(company);

					////Create Payment
					//if (payment.V_PaymentLines != null)
					//{
					//    if (payment.V_CreditCards != null && payment.V_CreditCards.Count > 0)
					//    {
					//        foreach (MODELS.CreditCards card in payment.V_CreditCards)
					//        {//cambiar por docnum
					//            card.VoucherNum = "Entry" + card.OwnerIdNum; //Convert.ToString(DocNum);
					//        }
					//    }
					//}

					//PayResposeFromSap = pso.CreateApPayment(payment, company.DBName, user, FatherCard, ref oCompany);

					//if (PayResposeFromSap.result)
					//{
					//    DocInfo docInfo = GetSapData.GetDocNumByDocEntryApInvoice(company.DBCode, PayResposeFromSap.DocEntry, "OPCH", _dbObjectSpNumApInv);
					//    if (docInfo != null)
					//    {
					//        PayResposeFromSap.DocNum = docInfo.DocNum;
					//    }
					//    oCompany.Disconnect();
					//    oCompany = null;

					//    return PayResposeFromSap;
					//}
					//else
					//{
					//    throw new Exception(PayResposeFromSap.errorInfo.Message);
					//}
				}
				else
				{
					throw new Exception("El documento debe contener al menos una linea");
				}
			}
			catch (Exception ex)
			{
				int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
				string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;
				string ErrMsj = "-" + Convert.ToString("SAPDAO/PostSapData/CreatePayApInvoices Catch-- Code: " + code + " Message: " + message);
				LogManager.LogMessage(ErrMsj, (int)Constants.LogTypes.SAP);
				return new PaymentSapResponse
				{
					Result = false,
					Error = new ErrorInfo
					{
						Code = code,
						Message = message
					}
				};
			}
		}

		/// <summary>
		/// Crea Factura y pago en Sap
		/// </summary>
		/// <param name="createapInvoice"></param>
		/// <param name="company"></param>
		/// <param name="userId"></param>
		/// <param name="_dbObjectSpFatherCard"></param>
		/// <param name="_dbObjectSpNumApInv"></param>
		/// <returns></returns>
		public static InvoiceSapResponse CreateapInvoice(CreateInvoice createapInvoice, Companys company, string userId, string _dbObjectSpFatherCard, string _dbObjectSpNumApInv, CredentialHolder _UserCredentials)
		{
			var startTime = DateTime.Now;
			LogManager.LogMessage(string.Format("PostSAPData>CreateapInvoice. Start Time: {0}", startTime), 2);
			PostDIAPIData pso = new PostDIAPIData();
			int DocEntry = 0;
			InvoiceSapResponse InvoiceSapResponse = new InvoiceSapResponse();
			Company oCompany = null;
			oCompany = new Company();

			try
			{
				if (createapInvoice != null &&
					createapInvoice.Invoice.InvoiceLinesList != null &&
					createapInvoice.Invoice.InvoiceLinesList.Count > 0
					) // CreateInvoice.Payment != null Check for Payment Lines
				{
					//if (CreateInvoice.Payment.V_PaymentLines.Count > 0) {

					string FatherCard = GetSapData.GetFatherCard(_UserCredentials, createapInvoice.Payment.CardCode, _dbObjectSpFatherCard);

					oCompany = DIAPICommon.CreateCompanyObject(_UserCredentials);

					InvoiceSapResponse = pso.CreateapInvoice(createapInvoice, company.DBName, userId, FatherCard, ref oCompany);

					if (InvoiceSapResponse.Result)
					{
						DocEntry = InvoiceSapResponse.DocEntry; //Get DocEntry ASAP, store it in memory for fast access

						DocInfo docInfo = GetSapData.GetDocNumByDocEntryApInvoice(_UserCredentials, InvoiceSapResponse.DocEntry, "APINV", _dbObjectSpNumApInv);
						if (docInfo != null)
						{
							InvoiceSapResponse.DocNum = docInfo.DocNum;
							//InvoiceSapResponse.NumDocFe = docInfo.NumFE;
							//InvoiceSapResponse.Consecutivo = docInfo.ClaveFE;
						}
						//Create Payment
						if (createapInvoice.Payment != null && InvoiceSapResponse.DocNum > 0)
						{
							if (createapInvoice.Payment.PaymentInvoices != null)
							{
								foreach (var pay in createapInvoice.Payment.PaymentInvoices)
								{
									pay.DocEntry = DocEntry;

									//if (createapInvoice.Payment.PaymentCreditCards != null && createapInvoice.Payment.PaymentCreditCards.Count > 0)
									//{
									//    foreach (MODELS.CreditCards card in createapInvoice.Payment.PaymentCreditCards)
									//    {//cambiar por docnum
									//        card.VoucherNum = "Entry" + card.OwnerIdNum; //Convert.ToString(DocNum);
									//    }
									//}
								};

								InvoiceSapResponse.PaymentResponse = pso.CreateApPayment(createapInvoice.Payment, company.DBName, userId, FatherCard, ref oCompany);

								oCompany.Disconnect();
								oCompany = null;
							}
						}
					}
					else
					{
						throw new Exception(InvoiceSapResponse.Error.Message);
					}
					//}

					LogManager.LogMessage(string.Format("               PostSAPData>CreateapInvoice. End Time: {0}| Total Time Time: {1}", DateTime.Now, DateTime.Now - startTime), (int)Constants.LogTypes.API);


					return InvoiceSapResponse;
				}
				else
				{
					throw new Exception("El documento debe contener al menos una linea");
				}
			}
			catch (Exception ex)
			{
				int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
				string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;
				string ErrMsj = "-" + Convert.ToString("SAPDAO/PostSapData/CreateapInvoice Catch-- Code: " + code + " Message: " + message);
				LogManager.LogMessage(ErrMsj, (int)Constants.LogTypes.SAP);

				if (oCompany.Connected)
				{
					oCompany.Disconnect();
					oCompany = null;
				}


				return new InvoiceSapResponse
				{
					Result = false,
					Error = new ErrorInfo
					{
						Code = code,
						Message = message
					}
				};
			}
		}

		public static ItemsResponse CreateGoodsReceiptStock(GoodsReceipt _goodsReceipt, CredentialHolder _UserCredentials, string _dbObject)
		{
			var startTime = DateTime.Now;
			LogManager.LogMessage(string.Format("PostSAPData>CreateGoodsReceiptStock. Start Time: {0}", startTime), 2);

			PostDIAPIData oPostDIAPIData = new PostDIAPIData();

			Company oCompany = null;
			oCompany = new Company();

			try
			{
				oCompany = DIAPICommon.CreateCompanyObject(_UserCredentials);

				ItemsResponse response = oPostDIAPIData.CreateGoodsReceiptStock(_goodsReceipt, ref oCompany);

				DocInfo docInfo = SAPDAO.GetSapData.GetDocNumByTableAndDocEntry("OIGN", response.DocEntry, _UserCredentials, _dbObject);


				if (docInfo != null)
					response.DocNum = docInfo.DocNum;



				return response;
			}
			catch
			{
				if (oCompany.Connected)
				{
					oCompany.Disconnect();
					oCompany = null;
				}
				throw;
			}
		}

		public static ItemsResponse CreateGoodsIssueStock(GoodsReceipt goodsIssue, CredentialHolder _UserCredentials, string _dbObject)
		{
			var startTime = DateTime.Now;
			LogManager.LogMessage(string.Format("                PostSAPData>CreateGoodsIssueStock. Start Time: {0}", startTime), 2);

			PostDIAPIData oPostDIAPIData = new PostDIAPIData();

			Company oCompany = null;
			oCompany = new Company();

			try
			{
				oCompany = DIAPICommon.CreateCompanyObject(_UserCredentials);

				ItemsResponse resultFromSAP = oPostDIAPIData.CreateGoodsIssueStock(goodsIssue, ref oCompany);


				DocInfo info = SAPDAO.GetSapData.GetDocNumByTableAndDocEntry("OIGE", resultFromSAP.DocEntry, _UserCredentials, _dbObject);

				if (info != null)
				{
					resultFromSAP.DocNum = info.DocNum;
				}



				return resultFromSAP;
			}
			catch
			{
				if (oCompany.Connected)
				{
					oCompany.Disconnect();
					oCompany = null;
				}
				throw;
			}
		}

		public static PurchaserOrderResponse CreatePurchaseOrder(PurchaseOrderModel _purchaseOrderModel, CredentialHolder _UserCredentials, string _DBObject)
		{
			var startTime = DateTime.Now;
			LogManager.LogMessage(string.Format("PostSAPData>CreatePurchaseOrder. Start Time: {0}", startTime), 2);

			PostDIAPIData oPostDIAPIData = new PostDIAPIData();

			Company oCompany = null;
			oCompany = new Company();

			try
			{
				oCompany = DIAPICommon.CreateCompanyObject(_UserCredentials);

				PurchaserOrderResponse temp = oPostDIAPIData.CreatePurchaseOrder(_purchaseOrderModel, ref oCompany);


				DocInfo docInfoDocNum = GetSapData.GetDocNumByTableAndDocEntry("OPOR", temp.PurchaseOrder.DocEntry, _UserCredentials, _DBObject);

				if (docInfoDocNum != null)
				{
					temp.PurchaseOrder.DocNum = docInfoDocNum.DocNum;
				}



				return temp;
			}
			catch (Exception ex)
			{
				int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
				string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;
				string ErrMsj = "-" + Convert.ToString("SAPDAO/PostSapData/CreatePurchaseOrder Catch-- Code: " + code + " Message: " + message);
				LogManager.LogMessage(ErrMsj, (int)Constants.LogTypes.SAP);

				if (oCompany.Connected)
				{
					oCompany.Disconnect();
					oCompany = null;
				}


				throw;
			}
		}


		public static object UpdatePurchaseOrder(PurchaseOrderModel _purchaseOrderModel, CredentialHolder _UserCredentials)
		{
			var startTime = DateTime.Now;
			LogManager.LogMessage(string.Format("PostSAPData>UpdatePurchaseOrder. Start Time: {0}", startTime), 2);

			PostDIAPIData oPostDIAPIData = new PostDIAPIData();

			Company oCompany = null;
			oCompany = new Company();

			try
			{
				oCompany = DIAPICommon.CreateCompanyObject(_UserCredentials);
				return oPostDIAPIData.UpdatePurchaseOrder(_purchaseOrderModel, ref oCompany);
			}
			catch (Exception ex)
			{
				int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
				string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;
				string ErrMsj = "-" + Convert.ToString("SAPDAO/PostSapData/UpdatePurchaseOrder Catch-- Code: " + code + " Message: " + message);
				LogManager.LogMessage(ErrMsj, (int)Constants.LogTypes.SAP);

				if (oCompany.Connected)
				{
					oCompany.Disconnect();
					oCompany = null;
				}

				throw;
			}
		}
		public static BaseResponse CreateCashflow(CashflowModel cashflow, CredentialHolder _UserCredentials)
		{
			try
			{
				return PostDIAPIData.CreateCashflow(cashflow, _UserCredentials);
			}
			catch (Exception)
			{
				throw;
			}
		}

		#region PayDesk
		public static int SelectUserSignature(CredentialHolder company)
		{
			Company oCompany = null;
			try
			{
				oCompany = DIAPICommon.CreateCompanyObject(company);
				int userSign = oCompany.UserSignature;

				return userSign;
			}
			catch (Exception)
			{
				throw;
			}
			finally
			{
				if (oCompany != null)
				{
					if (oCompany.Connected)
						oCompany.Disconnect();

					oCompany = null;
				}
			}
		}
		#endregion

	}
}
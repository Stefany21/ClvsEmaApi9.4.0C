using CLVSPOS.COMMON;
using CLVSPOS.DAO;
using CLVSPOS.DIAPI;
using CLVSPOS.LOGGER;
using CLVSPOS.MODELS;
using CLVSSUPER.MODELS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace CLVSPOS.PROCESS
{
	public class Process
	{

		#region Credentials


		private static CredentialHolder GetCurrentUserCredentials()
		{
			try
			{
				return GetData.GetRecordByKey<CredentialHolder, string>("spGetSapCredentials", GetUserId()).Data;

				//var UserCredentials = GetData.GetRecordByKey<CredentialHolder, string>("spGetSapCredentials", GetUserId()).Data;
				//UserCredentials.DBCode = "TST_CL_SUPERLT10";
				//UserCredentials.Server = "CLVSSQL10b\\SU10";
				//UserCredentials.DBUser = "CLAVISCO\\cl.clavis.desarrollo";
				//UserCredentials.DBPass = "D3s4rr0ll0+";

				//return UserCredentials;
			}
			catch
			{
				throw;
			}
		}

		public static PPTerminalsResponse GetTerminals()
		{
			try
			{
				return DAO.GetData.GetPPTerminals();
			}
			catch (Exception exc)
			{
				return (PPTerminalsResponse)LogManager.HandleExceptionWithReturn(exc, "PPTerminalsResponse");
			}
		}

		public static PPTerminalsByUserResponse GetTerminalsByUser(string _userId)
		{
			try
			{
				return DAO.GetData.GetPPTerminalsByUser(_userId);
			}
			catch (Exception exc)
			{
				return (PPTerminalsByUserResponse)LogManager.HandleExceptionWithReturn(exc, "PPTerminalsByUserResponse");
			}
		}


		#endregion

		/// <summary>
		/// Funcion para obtener el id del usuario logueado
		/// </summary>
		/// <returns></returns>
		public static string GetUserId()
		{
			try
			{
				// se obtiene el userId, localizado en los Claims
				var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
				return identity.Claims.Where(c
					=> c.Type == "userId").Single().Value;

			}
			catch (Exception ex)
			{

				throw ex;
			}
		}




		public static ApiResponse<List<CommittedTransaction>> SavePreBalance(ACQTransaction _aCQTransaction)
		{
			string result = string.Empty;

			DateTime CURRENT_DATE = DateTime.Now;			

			try
			{
				PPBalanceResponse oPPBalanceResponse = PostData.CreatePPBalance(_aCQTransaction.OverACQ);

				PostData.UpdatePPTransactionACQ(_aCQTransaction.Terminal.Id, oPPBalanceResponse.PPBalance.Id, Constants.PPRequestType.PBalance, 0,GetNameObject("spUpdatePPTransactionACQ"));

				PPTransactionsResponse oPPTransactionsResponse = GetData.GetTransactionsByACQ(oPPBalanceResponse.PPBalance.Id, _aCQTransaction.Terminal.Id, Constants.PPRequestType.PBalance,GetNameObject("spGetPPBalanceByACQ"));


				_aCQTransaction.BalanceRequest.DocumentType = Enum.GetName(typeof(Constants.DocumentTypeTransaction), Constants.DocumentTypeTransaction.PRE_BALANCE);// Constants.PPRequestType.PBalance; 
				return new ApiResponse<List<CommittedTransaction>>
				{
					Data = GetData.GetCommitedTransactions(_aCQTransaction.BalanceRequest,GetNameObject("spGetPPCommitedTransactios"))?.Data,
					Result = true,
				};
			}
			catch
			{
				throw;

			}
		}



		public static BaseResponse UpdateTerminalsByUser(PPTerminalsByUser _pPTerminalsByUser)
		{
			try
			{
				return DAO.PostData.UpdatePPTerminalsByUser(_pPTerminalsByUser);
			}
			catch (Exception exc)
			{
				return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, "BaseResponse");
			}
		}

		public static bool IsLocalInvoiceIdValid(string _idInvoice)
		{
			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();

				return SAPDAO.GetSapData.IsLocalInvoiceIdValid(userCredentials, _idInvoice);
			}
			catch (Exception)
			{
				throw;
			}
		}

		public static PPTerminalResponse GetPPTerminal(int _terminalId)
		{
			try
			{
				return DAO.GetData.GetPPTerminal(_terminalId);
			}
			catch (Exception exc)
			{
				return (PPTerminalResponse)LogManager.HandleExceptionWithReturn(exc, "PPTerminalResponse");
			}
		}

		/// <summary>
		/// Metodo para obtener los impuestos de la compania
		/// no recibe parametros
		/// </summary>
		/// <returns></returns>
		public static TaxesResponse GetTaxes()
		{
			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return CLVSPOS.SAPDAO.GetSapData.GetTaxes(userCredentials, GetNameObject("viewGETTAXES"));
			}
			catch (Exception exc)
			{
				return (TaxesResponse)LogManager.HandleExceptionWithReturn(exc, "TaxesResponse");
			}
		}

		public static ApiResponse<List<CommittedTransaction>> SaveBalance(ACQTransaction _aCQTransaction)
		{
			string result = string.Empty;

			PPTerminal oPPTerminal = _aCQTransaction.Terminal;
			DateTime CURRENT_DATE = DateTime.Now;
			try
			{
				string messageDetail = string.Empty;

				PPBalanceRequest _pPBalanceRequest = new PPBalanceRequest()
				{
					TerminalId = oPPTerminal.Id,
					From = DateTime.Now,
					To = DateTime.Now
				};

				String value = ((Constants.DocumentTypeTransaction)(int)Constants.DocumentTypeTransaction.BALANCE).ToString();


				PPBalance oPPBalance = _aCQTransaction.OverACQ;
				oPPBalance.TransactionType = value;

				PostData.UpdatePPTransactionMassive(DateTime.Now, DateTime.Now, _aCQTransaction.Terminal.Id,GetNameObject("spUpdatePPBalance"));

				PPBalanceResponse oPPBalanceResponse = PostData.CreatePPBalance(oPPBalance);

				PostData.UpdatePPTransactionACQ(oPPTerminal.Id, oPPBalanceResponse.PPBalance.Id, Constants.PPRequestType.Balance, 0,GetNameObject("spUpdatePPTransactionACQ"));

				_pPBalanceRequest.DocumentType = value;

				return GetData.GetCommitedTransactions(_pPBalanceRequest,GetNameObject("spGetPPCommitedTransactios"));
			}
			catch
			{
				throw;
			}
		}

		public static ApiResponse<string> GetTransactionsPinpadTotal(int _terminalId)
		{
			try
			{
				return DAO.GetData.GetTransactionsPinpadTotal(_terminalId, GetNameObject("spPinpadTransactionsTotal"));
			}
			catch
			{
				throw;
			}
		}

		public static ApiResponse<List<CommittedTransaction>> PreBalanceOnRegisters(PPBalanceRequest _pPBalanceRequest)
		{
			ApiResponse<List<CommittedTransaction>> oResponse = new ApiResponse<List<CommittedTransaction>>();

			try
			{

				oResponse = GetData.GetCommitedTransactions(_pPBalanceRequest, GetNameObject("spGetPPCommitedTransactios"));
			}
			catch (Exception exc)
			{
				oResponse.Result = false;
				oResponse.Error = new ErrorInfo()
				{
					Message = "Error al consultar el balance " + exc.Message + exc.InnerException?.Message
				};
			}
			return oResponse;
		}

		public static BaseResponse CreateTerminal(PPTerminal _bacTerminal)
		{
			try
			{
				return DAO.PostData.CreateBacTerminal(_bacTerminal);
			}
			catch (Exception exc)
			{
				return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, "BACTerminalResponse");
			}
		}

		public static SyncResponse SyncGetTaxes(string userId)
		{
			try
			{
				var company = GetData.GetCompanyByUserId(userId);
				return CLVSPOS.SAPDAO.GetSapData.SyncGetTaxes(company, GetNameObject("viewGETTAXES"));
			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse");
			}
		}

		public static BaseResponse UpdateTerminal(PPTerminal _bacTerminal)
		{
			try
			{
				return DAO.PostData.UpdateBACTerminal(_bacTerminal);
			}
			catch (Exception exc)
			{
				return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, "BACTerminalResponse");
			}
		}

		/// <summary>
		/// Metodo para obtener los clientes de la compania
		/// No recibe parametros
		/// </summary>
		/// <returns></returns>
		public static BPSResponseModel GetBusinessPartners()
		{
			try
			{


				CredentialHolder UserCredentials = GetCurrentUserCredentials();



				return CLVSPOS.SAPDAO.GetSapData.GetBusinessPartners(UserCredentials, GetNameObject("viewGetBPCustomers"));

			}
			catch (Exception exc)
			{
				return (BPSResponseModel)LogManager.HandleExceptionWithReturn(exc, "BPSResponseModel");
			}
		}
		/// <summary>
		/// Metodo para obtener los clientes de la compania
		/// No recibe parametros
		/// </summary>
		/// <returns></returns>
		public static BPSResponseModel GetSuppliers()
		{
			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return CLVSPOS.SAPDAO.GetSapData.GetSuppliers(userCredentials, GetNameObject("viewGetSuppliers"));

			}
			catch (Exception exc)
			{
				return (BPSResponseModel)LogManager.HandleExceptionWithReturn(exc, "BPSResponseModel");
			}
		}

		public static BPFEInfoResponseModel GetBusinessPartnerFEInfo(string cardCode)
		{
			try
			{
				CredentialHolder UserCredentials = GetCurrentUserCredentials();

				return CLVSPOS.SAPDAO.GetSapData.GetBusinessPartnerFEInfo(UserCredentials, cardCode, GetNameObject("viewGetBPCustomers"));

			}
			catch (Exception exc)
			{
				return (BPFEInfoResponseModel)LogManager.HandleExceptionWithReturn(exc, "BPFEInfoResponseModel");
			}
		}

		public static BPFEInfoResponseModel GetBusinessPartnerFEInfo(string idType, string idNumber)
		{
			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return CLVSPOS.SAPDAO.GetSapData.GetBusinessPartnerFEInfo(userCredentials, idType, idNumber, GetNameObject("spGetFeInfo"));

			}
			catch (Exception exc)
			{
				return (BPFEInfoResponseModel)LogManager.HandleExceptionWithReturn(exc, "BPFEInfoResponseModel");
			}
		}

		public static async Task<BPInfoPadronResponseModel> GetBusinessPartnerPadronInfo(string idNumber, string tokenPadron)
		{
			try
			{
				BPInfoPadronResponseModel response = new BPInfoPadronResponseModel();
				HttpResponseMessage padronResponse = await COMMON.Padron.GetBpInfoPadron(idNumber, tokenPadron);
				if (padronResponse.IsSuccessStatusCode)
				{
					string responseData = padronResponse.Content.ReadAsStringAsync().Result;
					BPDataModel obj = new BPDataModel();
					obj = JsonConvert.DeserializeObject<BPDataModel>(responseData);
					if (obj.Result)
					{
						response.Result = true;
						response.Error = null;
						response.CardName = string.Format("{0} {1} {2}", obj.BPDataModelList[0].Nombre, obj.BPDataModelList[0].Apellido1, obj.BPDataModelList[0].Apellido2);
					}
					else
					{
						response.Result = false;
						response.Error = new ErrorInfo
						{
							Code = obj.Error.Code,
							Message = obj.Error.Message
						};
					}
				}
				else
				{
					response.Result = false;
					JavaScriptSerializer js = new JavaScriptSerializer();
					var headers = padronResponse.Headers.ToList();
					string responseMessage = js.Serialize(headers);
					response.Error = new ErrorInfo
					{
						Code = 0,
						Message = responseMessage
					};
					//LogManager.LogMessage("Respuesta incorrecta SyncCompanyReport detalles: " + responseMessage, (int)Constants.LogType.WService);
				}
				return response;
			}
			catch (Exception exc)
			{
				return (BPInfoPadronResponseModel)LogManager.HandleExceptionWithReturn(exc, "BPInfoPadronResponseModel");
			}
		}

		public static SyncResponse SyncGetBusinessPartners(string userId)
		{
			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return CLVSPOS.SAPDAO.GetSapData.SyncGetBusinessPartners(userCredentials, GetNameObject("viewGetBPCustomers"));

			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncGetBusinessPartners");
			}
		}

		/// <summary>
		/// Metodo para obtener los items de la compania
		/// No recibe parametros
		/// </summary>
		/// <returns></returns>
		public static ItemNamesResponse GetItemNames()
		{
			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return CLVSPOS.SAPDAO.GetSapData.GetItemNames(userCredentials, GetNameObject("viewGetItems"), GetNameObject("viewGetItemGroupList"), GetNameObject("viewGetFirmsList"));

			}
			catch (Exception exc)
			{
				return (ItemNamesResponse)LogManager.HandleExceptionWithReturn(exc, "ItemNamesResponse");
			}
		}

		public static SyncResponse SyncGetItems(string userId)
		{
			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return CLVSPOS.SAPDAO.GetSapData.SyncGetItems(userCredentials, GetNameObject("viewGetItems"));

			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse");
			}
		}

		public static SyncResponse SyncGetPriceGroupList(string userId)
		{
			try
			{
				var company = GetData.GetCompanyByUserId(userId);
				return CLVSPOS.SAPDAO.GetSapData.SyncGetPriceGroups(company, GetNameObject("viewGetItemGroupList"));

			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse");
			}
		}

		public static SyncResponse SyncGetFirms(string userId)
		{
			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return CLVSPOS.SAPDAO.GetSapData.SyncGetFirms(userCredentials, GetNameObject("viewGetFirmsList"));

			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse");
			}
		}

		public static SyncResponse SyncGetOCRD(string userId)
		{
			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return CLVSPOS.SAPDAO.GetSapData.SyncGetOCRD(userCredentials);

			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse");
			}
		}

		public static SyncResponse SyncGetOSTA(string userId)
		{
			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return CLVSPOS.SAPDAO.GetSapData.SyncGetOSTA(userCredentials);

			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse");
			}
		}

		public static SyncResponse SyncGetOTCX(string userId)
		{
			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return CLVSPOS.SAPDAO.GetSapData.SyncGetOTCX(userCredentials, GetNameObject("viewGetOTCX"));

			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse");
			}
		}

		public static ApiResponse<double> GetItemAVGPrice(string itemCode)
		{
			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return SAPDAO.GetSapData.GetItemAVGPrice(itemCode, userCredentials, GetNameObject("spGetItemAvgPrice"));
			}
			catch
			{
				throw;
			}
		}

		public static SyncResponse SyncGetOITM(string userId)
		{
			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return CLVSPOS.SAPDAO.GetSapData.SyncGetOITM(userCredentials);

			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse");
			}
		}

		public static SyncResponse SyncGetITM1(string userId)
		{
			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return CLVSPOS.SAPDAO.GetSapData.SyncGetITM1(userCredentials);

			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse");
			}
		}

		public static SyncResponse SyncGetOWHS(string userId)
		{
			try
			{
				// var company = GetData.GetCompanyByUserId(userId);
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return CLVSPOS.SAPDAO.GetSapData.SyncGetOWHS(userCredentials);

			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse");
			}
		}

		public static ApiResponse<List<PPTransaction>> GetTransactionsByDocEntryOpened(int _docEntry)
		{
			try
			{
				ApiResponse<List<PPTransaction>> oDynamicReponse = DAO.GetData.GetTransactionsByDocEntry(_docEntry);

				if (oDynamicReponse.Result)
				{
					if (oDynamicReponse.Data.Any(x => x.IsOnBalance))
					{
						throw new Exception("Ya se ha hecho el cierre de tarjetas, la factura no puede ser anulada.");
					}
				}

				return oDynamicReponse;
			}
			catch
			{
				throw;
			}
		}

		public static SyncResponse SyncGetOITW(string userId)
		{
			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return CLVSPOS.SAPDAO.GetSapData.SyncGetOITW(userCredentials);

			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse");
			}
		}

		public static ApiResponse<double> GetItemLastPurchagePrice(string itemCode)
		{
			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return SAPDAO.GetSapData.GetItemLastPurchagePrice(itemCode, userCredentials, GetNameObject("spGetItemLastPrice"));
			}
			catch
			{
				throw;
			}
		}

		public static SyncResponse SyncGetINV6(string userId)
		{
			try
			{
				var company = GetData.GetCompanyByUserId(userId);
				return CLVSPOS.SAPDAO.GetSapData.SyncGetINV6(company);

			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse");
			}
		}

		public static SyncResponse SyncGetDPI6(string userId)
		{
			try
			{
				var company = GetData.GetCompanyByUserId(userId);
				return CLVSPOS.SAPDAO.GetSapData.SyncGetDPI6(company);

			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse");
			}
		}

		public static ApiResponse<PPTransaction> CommitCanceledCard(SelfPPTransaction _selfTransaction)
		{
			try
			{
				if (_selfTransaction == null || _selfTransaction.Transaction == null)
				{
					throw new Exception("Target anulada. No se pudo respaldar la transacción en base de datos, consulte logs por favor.");
				}

				PPTransaction oPPTransaction = _selfTransaction.Transaction;
				oPPTransaction.UserPrefix = _selfTransaction.UserPrefix;

				oPPTransaction.CanceledStatus = Enum.GetName(typeof(Constants.TransactionStatus), Constants.TransactionStatus.FINISHED);
				oPPTransaction.CanceledResponse = _selfTransaction.RawData;
				DateTime currentDate = DateTime.Now;
				oPPTransaction.CreationDate = currentDate;
				oPPTransaction.LastUpDate = currentDate;

				if (CheckIfTransactionExistForCancel(oPPTransaction.InvoiceNumber))
				{
					PostData.UpdatePPTransactionCancel(oPPTransaction);
				}
				else
				{
					PostData.createPPTransaction(oPPTransaction);
				}

				return new ApiResponse<PPTransaction>
				{
					Data = oPPTransaction,
					Result = true
				};
			}
			catch
			{
				throw;
			}
		}

		private static bool CheckIfTransactionExistForCancel(string _invoiceNumber)
		{
			PPTransaction oPPTransaction = null;
			try
			{
				if (_invoiceNumber == null) return false;

				using (SuperV2_Entities dbDao = new SuperV2_Entities())
				{
					using (DbContextTransaction transaction = dbDao.Database.BeginTransaction())
					{
						oPPTransaction = dbDao.PPTransaction.Where(x => x.InvoiceNumber.Equals(_invoiceNumber)).FirstOrDefault();
					}
				}

				return oPPTransaction != null;
			}
			catch
			{
				throw;
			}
		}

		public static SyncResponse SyncGetODPI(string userId)
		{
			try
			{
				var company = GetData.GetCompanyByUserId(userId);
				return CLVSPOS.SAPDAO.GetSapData.SyncGetODPI(company);

			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse");
			}
		}

		public static PPTransactionsResponse GetPPTransactionCenceledStatus(PPTransactionsCanceledPrintSearch _doc)
		{
			try
			{
				return DAO.GetData.GetPPTransactionCenceledStatus(_doc,GetNameObject("spGetPPTransactiosCanceled"));
			}
			catch
			{
				throw;
			}
		}

		public static SyncResponse SyncGetOINV(string userId, string filter)
		{
			try
			{
				var company = GetData.GetCompanyByUserId(userId);
				return CLVSPOS.SAPDAO.GetSapData.SyncGetOINV(company, filter);

			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse");
			}
		}

		public static ApiResponse<List<ItemDataForInvoiceGoodReceipt>> GetItemDataForGoodReceiptInvoice(List<string> itemCodes, string WhsCode)
		{
			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return SAPDAO.GetSapData.GetItemDataForGoodReceiptInvoice(itemCodes, userCredentials, WhsCode, GetNameObject("spGetItemAvgPrice"), GetNameObject("spGetItemLastPrice"), GetNameObject("spGetValidateDeviation"));
			}
			catch
			{
				throw;
			}
		}

		public static SyncResponse SyncGetORCT(string userId)
		{
			try
			{
				var company = GetData.GetCompanyByUserId(userId);
				return CLVSPOS.SAPDAO.GetSapData.SyncGetORCT(company);

			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse");
			}
		}

		public static ApiResponse<List<PPTransaction>> GetPPTransactionsByDocumentKey(string _documentKey)
		{
			try
			{

				ApiResponse<List<PPTransaction>> oResponse = DAO.GetData.GetRecordsByKey<PPTransaction, string>(GetNameObject("spGetPPTransactionsByDocumentKey"), _documentKey);

				if (oResponse.Data?.Count == 0)
				{
					oResponse.Result = false;
					oResponse.Error = new ErrorInfo
					{
						Code = -5, // Le pongo este valor porque asi lo defini en el ui en su momento by eaguilar
						Message = "No se encontraron transacciones asociadas a la factura"
					};
				}

				return oResponse;
			}
			catch
			{
				LogManager.LogMessage(string.Format("api/Payment/dammit"), (int)Constants.LogTypes.API);
				throw;
			}
		}

		public static SyncResponse SyncGetRCT2(string userId)
		{
			try
			{
				var company = GetData.GetCompanyByUserId(userId);
				return CLVSPOS.SAPDAO.GetSapData.SyncGetRCT2(company);

			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse");
			}
		}
		//001
		public static ApiResponse<bool> UpdateCompanyViewsMargins(int idCompany, string marginsViews)
		{
			try
			{
				return PostData.UpdateCompanyMargins(idCompany, marginsViews);
			}
			catch
			{
				throw;
			}
		}

		public static SyncResponse SyncGetINV1(string userId)
		{
			try
			{
				var company = GetData.GetCompanyByUserId(userId);
				return CLVSPOS.SAPDAO.GetSapData.SyncGetINV1(company);

			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse");
			}
		}

		public static SyncResponse SyncGetOUSR(string userId)
		{
			try
			{
				var company = GetData.GetCompanyByUserId(userId);
				return CLVSPOS.SAPDAO.GetSapData.SyncGetOUSR(company);

			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse");
			}
		}
		//001
		// Obtiene la lista de las vistas a las que se usara margenes
		public static ApiResponse<List<CompanyMargins>> GetViewMargins()
		{
			try
			{
				return GetData.GetCompanyMargins();
			}
			catch
			{
				throw;
			}
		}

		public static SyncResponse SyncGetORDR(string userId)
		{
			try
			{
				var company = GetData.GetCompanyByUserId(userId);
				return CLVSPOS.SAPDAO.GetSapData.SyncGetORDR(company);

			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse");
			}
		}

		public static ApiResponse<PPTransaction> GetPPTransactionByInvoiceNumber(string _documentKey)
		{
			try
			{

				ApiResponse<PPTransaction> oResponse = GetData.GetRecordByKey<PPTransaction, string>("spGetPPTransactionsByInvoiceNumber", _documentKey);

				if (!oResponse.Result)
				{
					oResponse.Result = false;
					oResponse.Error = new ErrorInfo
					{
						Code = -5,
						Message = "No se encontraron transacciones asociadas a la factura"
					};
				}

				return oResponse;
			}
			catch
			{
				LogManager.LogMessage(string.Format("api/Payment/GetPPTransactionByInvoiceNumber"), (int)Constants.LogTypes.API);
				throw;
			}
		}

		public static SyncResponse SyncGetOQUT(string userId)
		{
			try
			{
				var company = GetData.GetCompanyByUserId(userId);
				return CLVSPOS.SAPDAO.GetSapData.SyncGetOQUT(company);

			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse");
			}
		}

		/// <summary>
		/// Metodo para obtener la informacion de un item de la compania
		/// Recibe como parametro el codigo, lista de precio, cardcode, alamacen del item a consultar
		/// </summary>
		/// <param name="itemCode"></param>
		/// <param name="priceList"></param>
		/// <param name="_cardCode"></param>
		/// <param name="whCode"></param>
		/// <returns></returns>
		public static ItemsResponse GetInfoItem(string itemCode, int priceList, string _cardCode, string whCode, string _documentType)
		{
			try
			{
				var userId = GetUserId();
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				var userAssign = GetData.GetUserMappId(userId);
				return SAPDAO.GetSapData.GetInfoItem(itemCode, Convert.ToDecimal(userAssign.minDiscount), userCredentials, priceList, _cardCode, whCode, _documentType, GetNameObject("spGetItemInfo"));

			}
			catch (Exception exc)
			{
				return (ItemsResponse)LogManager.HandleExceptionWithReturn(exc, "ItemsResponse");
			}
		}


		/// <summary>
		/// Metodo para obtener la informacion de un item de la compania
		/// No recibe parametros
		/// </summary>
		/// <returns></returns>
		public static ExchangeRateResponse GetExchangeRate()
		{
			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return CLVSPOS.SAPDAO.GetSapData.GetExchangeRate(userCredentials, GetNameObject("viewGETEXRATE"));
			}
			catch (Exception exc)
			{
				return (ExchangeRateResponse)LogManager.HandleExceptionWithReturn(exc, "ExchangeRateResponse");
			}
		}

		/// <summary>
		/// Metodo para obtener la informacion del tipo de cambio para ser sincronizado localmente
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public static SyncResponse SyncGetExchangeRate(string userId)
		{
			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();

				return SAPDAO.GetSapData.SyncGetExchangeRate(userCredentials, userId, GetNameObject("viewGETEXRATE"));
			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncGetExchangeRate");
			}
		}

		/// <summary>
		/// Metodo para enviar una sale order a SAP
		/// </summary>
		/// <param name="saleOrder"></param>
		/// <returns></returns>
		public static async Task<SalesOrderToSAPResponse> CreateSaleOrder(ISaleDocument saleOrder)
		{
			try
			{
				CredentialHolder UserCredentials = GetCurrentUserCredentials();


				var userId = GetUserId();


				DocInfo isAlreadyCreated = SAPDAO.GetSapData.CheckUniqueDocumentID("ORDR", saleOrder.U_CLVS_POS_UniqueInvId, UserCredentials, GetNameObject("spCheckUniqueDocumentID"));


				if (isAlreadyCreated != null)
				{
					return new SalesOrderToSAPResponse()
					{
						Result = true,
						DocEntry = isAlreadyCreated.DocEntry,
						DocNum = isAlreadyCreated.DocNum
					};
				}

				return await CLVSPOS.SAPDAO.PostSapData.CreateSaleOrder(saleOrder, GetNameObject("spGetDocNumORDR"), UserCredentials, userId);
			}
			catch
			{
				throw;
			}
		}


		/// <summary>
		/// Metodo para enviar una cotizacion a SAP
		/// Los parametros que recibe son el modelo de Cotizacion que va a guardar en SAP
		/// </summary>
		/// <param name="quotations"></param>
		/// <returns></returns>
		public static async Task<QuotationsToSAPResponse> CreateQuotation(IQuotDocument quotations)
		{
			try
			{

				CredentialHolder UserCredentials = GetCurrentUserCredentials();

				DocInfo docInfo = SAPDAO.GetSapData.CheckUniqueDocumentID("OQUT", quotations.U_CLVS_POS_UniqueInvId, UserCredentials, GetNameObject("spCheckUniqueDocumentID"));

				if (docInfo != null)
				{
					return new QuotationsToSAPResponse()
					{
						DocEntry = docInfo.DocEntry,
						DocNum = docInfo.DocNum,
						Result = true
					};
				}


				return await SAPDAO.PostSapData.CreateQuotations(quotations, GetNameObject("spGetDocNumOQUT"), UserCredentials);
			}
			catch
			{
				throw;

			}
		}

		public static object UpdateQuotationDIAPI(IQuotDocument quotationEdit)
		{
			try
			{
				CredentialHolder UserCredentials = GetCurrentUserCredentials();
				return SAPDAO.PostSapData.UpdateQuotationDIAPI(quotationEdit, UserCredentials, GetNameObject("spGetDocNumOQUT"), UserCredentials);
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Metodo para realizar un pago en SAP
		/// Recibe como parametros el modelo de pago
		/// </summary>
		/// <param name="_payment"></param>
		/// <returns></returns>
		public static async Task<PaymentSapResponse> CreatePaymentRecived(CreateSLRecivedPaymentModel _payment)
		{
			try
			{
				CredentialHolder UserCredentials = GetCurrentUserCredentials();

				var userId = GetUserId();


				DocInfo IsAlreadyCreated = SAPDAO.GetSapData.CheckUniqueDocumentID("ORCT", _payment.U_CLVS_POS_UniqueInvId, UserCredentials, GetNameObject("spCheckUniqueDocumentID"));

				if (IsAlreadyCreated != null)
				{
					return new PaymentSapResponse()
					{
						DocEntry = IsAlreadyCreated.DocEntry,
						DocNum = IsAlreadyCreated.DocNum,
						Result = true

					};
				}


				return await SAPDAO.PostSapData.CreatePaymentRecived(_payment, userId, GetNameObject("spGetFatherCard"), UserCredentials);
			}
			catch (Exception exc)
			{
				return (PaymentSapResponse)LogManager.HandleExceptionWithReturn(exc, "PaymentSapResponse");
			}
		}

		public static object UpdateSaleOrderDIAPI(ISaleDocument saleOrder)
		{
			try
			{

				CredentialHolder UserCredentials = GetCurrentUserCredentials();

				return CLVSPOS.SAPDAO.PostSapData.UpdateSaleOrderDIAPI(saleOrder, UserCredentials, GetNameObject("spGetDocNumORDR"));
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Metodo para obtener la informacion de un item de la compania
		/// Recibe como parametro el codigo del item a consultar
		/// </summary>
		/// <param name="itemCode"></param>
		/// <returns></returns>
		public static WHInfoResponse GetWHAvailableItem(string itemCode)
		{
			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return CLVSPOS.SAPDAO.GetSapData.GetWHAvailableItem(itemCode, userCredentials, GetNameObject("spGetAvailableItems"));

			}
			catch (Exception exc)
			{
				return (WHInfoResponse)LogManager.HandleExceptionWithReturn(exc, "WHInfoResponse");
			}
		}

		/// <summary>
		/// metodo para obtener la de los parametros que lleva una determinada vista
		/// recibe como parametro el el valor de la vista de la cual va a traer los parametros
		/// </summary>
		/// <param name="view"></param>
		/// <returns></returns>
		public static ParamsViewResponse GetViewParam(int view)
		{
			try
			{
				return CLVSPOS.DAO.GetData.GetViewParam(view);
			}
			catch (Exception exc)
			{
				return (ParamsViewResponse)LogManager.HandleExceptionWithReturn(exc, "ParamsViewResponse");
			}
		}

		public static SyncResponse SyncGetParamsViewCompanies()
		{
			try
			{
				return CLVSPOS.DAO.GetData.SyncGetParamsViewCompanies();
			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncGetParamsViewCompanies");
			}
		}

		public static SyncResponse SyncGetViewParams()
		{
			try
			{
				return CLVSPOS.DAO.GetData.SyncGetViewParams();
			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncGetViewParams");
			}
		}

		/// <summary>
		/// metodo para obtener la informacion de de las seies por almacen
		/// recibe como parametro el codigo del item y el codigo del almacen a consultar
		/// </summary>
		/// <param name="itemCode"></param>
		/// <param name="whsCode"></param>
		/// <returns></returns>
		public static SeriesResponse GetSeriesByItem(string itemCode, string whsCode)
		{
			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return SAPDAO.GetSapData.GetSeriesByItem(itemCode, whsCode, userCredentials, GetNameObject("sp_SeriesByItem"));

			}
			catch (Exception exc)
			{
				return (SeriesResponse)LogManager.HandleExceptionWithReturn(exc, "SeriesResponse");
			}
		}

		public static ItemDetailResponse GetItemDetails(string itemCode, int NumeroEntradas, int forView)
		{

			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return SAPDAO.GetSapData.GetItemDetails(itemCode, userCredentials, NumeroEntradas, forView, GetNameObject("spGetItemPurchaseDetail"));

			}
			catch (Exception exc)
			{
				return (ItemDetailResponse)LogManager.HandleExceptionWithReturn(exc, "ItemDetailResponse");
			}
		}

		/// <summary>
		/// metodo para obtener las compannias registradas en la aplicacion
		/// no recibe parametros 
		/// </summary>
		/// <returns></returns>
		public static SyncResponse SyncGetCompanies()
		{
			try
			{
				return CLVSPOS.DAO.GetData.SyncGetCompanies();
			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
			}
		}

		public static SyncResponse SyncGetMailData()
		{
			try
			{
				return CLVSPOS.DAO.GetData.SyncGetMailData();
			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
			}
		}

		public static SyncResponse SyncGetCompaniesByUser()
		{
			try
			{
				return CLVSPOS.DAO.GetData.SyncGetCompaniesByUser();
			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
			}
		}

		/// <summary>
		/// metodo para obtener las compannias registradas en la aplicacion
		/// no recibe parametros 
		/// </summary>
		/// <returns></returns>
		public static BaseResponse GetCompanies()
		{
			try
			{
				return CLVSPOS.DAO.GetData.GetCompanies();
			}
			catch (Exception exc)
			{
				return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
			}
		}

		/// <summary>
		/// metodo para obtener las compannias registradas en la aplicacion
		/// no recibe parametros 
		/// </summary>
		/// <returns></returns>
		public static BaseResponse GetDefaultCompany()
		{
			try
			{
				return CLVSPOS.DAO.GetData.GetDefaultCompany();
			}
			catch (Exception exc)
			{
				return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
			}
		}

		/// <summary>
		/// metodo para obtener la informacion de una compannia por el id de la misma
		/// recibe como parametro el id de la compannia
		/// </summary>
		/// <param name="companyId"></param>
		/// <returns></returns>
		public static CompanyResponse GetCompanyById(int companyId)
		{
			try
			{
				return CLVSPOS.DAO.GetData.GetCompanyById(companyId);
			}
			catch (Exception)
			{
				throw new Exception(string.Format("Information for Company {0} Not Found", companyId));
			}
		}

		/// <summary>
		/// metodo para crear una compannia en la aplicacion
		/// recibe como parametro el modelo de compannia
		/// </summary>
		/// <param name="companyAndMail"></param>
		/// <param name="filesInfo"></param>
		/// <returns></returns>
		public static BaseResponse CreateCompany(CompanyAndMail companyAndMail, HttpContext filesInfo)
		{
			try
			{
				return CLVSPOS.DAO.PostData.CreateCompany(companyAndMail, filesInfo);
			}
			catch (Exception exc)
			{
				return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
			}
		}

		/// <summary>
		/// metodo para actualizar una compannia y el correo en la aplicacion
		/// recibe como parametro el modelo de compannia con el correo
		/// </summary>
		/// <param name="companyAndMail"></param>
		/// <param name="filesInfo"></param>
		/// <returns></returns>
		public static BaseResponse UpdateCompany(CompanyAndMail companyAndMail, HttpContext filesInfo)
		{
			try
			{
				return CLVSPOS.DAO.PostData.UpdateCompany(companyAndMail, filesInfo);
			}
			catch (Exception exc)
			{
				return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
			}
		}

		/// <summary>
		/// Metodo para obtener una lista de la informacion de los usuarios para configuracion 
		///no recibe parametros
		/// </summary>
		/// <returns></returns>
		public static SyncResponse SyncGetUserUserAssign()
		{
			try
			{
				return CLVSPOS.DAO.GetData.SyncGetUserAssigns();
			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "UserListModel");
			}
		}

		/// <summary>
		/// Metodo para obtener una lista de la informacion de los usuarios para configuracion 
		///no recibe parametros
		/// </summary>
		/// <returns></returns>
		public static UserListModel GetUserUserAssignList()
		{
			try
			{
				var userId = GetUserId();
				var company = GetData.GetCompanyByUserId(userId);
				return CLVSPOS.DAO.GetData.GetUserUserAssignList(company, GetNameObject("spGetUserAssingListApp"), GetNameObject("spGetSeriesByUsersApp"));
			}
			catch (Exception exc)
			{
				return (UserListModel)LogManager.HandleExceptionWithReturn(exc, "UserListModel");
			}
		}



		/// <summary>
		/// Metodo para obtener una lista de la informacion de los usuarios para configuracion 
		///no recibe parametros
		/// </summary>
		/// <returns></returns>
		public static SyncResponse SyncGetUsers()
		{
			try
			{
				return CLVSPOS.DAO.GetData.SyncGetUsers();
			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "UserListModel");
			}
		}

		/// <summary>
		/// metodo para obtener las conexiones de SAP de la DB Local
		/// no recibe parametros 
		/// </summary>
		/// <returns></returns>
		public static SapConnectionResponse GetSapConnection()
		{
			try
			{
				return CLVSPOS.DAO.GetData.GetSapConnection();
			}
			catch (Exception exc)
			{
				return (SapConnectionResponse)LogManager.HandleExceptionWithReturn(exc, "SapConnectionResponse");
			}
		}

		public static SyncResponse SyncGetSapConnections()
		{
			try
			{
				return CLVSPOS.DAO.GetData.SyncGetSapConnections();
			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncGetSapConnections");
			}
		}

		/// <summary>
		/// metodo para obtener los almacenes con los que cuenta una compania
		/// Recibe el id de la compania dese el front para buscar por compañia seleccionada... 
		/// no logeada.
		/// </summary>
		/// <param name="company"></param>
		/// <returns></returns>
		public static StoreListModel GetStoresByCompany(int company)
		{
			try
			{
				//var userId = GetUserId();
				//var company = GetData.GetCompanyByUserId(userId);
				return CLVSPOS.DAO.GetData.GetStoresByCompany(company);

			}
			catch (Exception exc)
			{
				return (StoreListModel)LogManager.HandleExceptionWithReturn(exc, "StoreListModel");
			}
		}

		/// <summary>
		/// metodo para obtener los almasenes con los que cuenta una compañias
		/// el id de la compania dese el front para buscar por compañia seleccionada
		/// no logeada.
		/// </summary>
		/// <returns></returns>
		public static StoreListModel GetAllStores()
		{
			try
			{
				return CLVSPOS.DAO.GetData.GetAllStores();
			}
			catch (Exception exc)
			{
				return (StoreListModel)LogManager.HandleExceptionWithReturn(exc, "StoreListModel");
			}
		}

		/// <summary>
		/// metodo para obtener los almasenes con los que cuenta una compañias
		/// el id de la compania dese el front para buscar por compañia seleccionada
		/// no logeada.
		/// </summary>
		/// <returns></returns>
		public static SyncResponse SyncGetStores()
		{
			try
			{
				return CLVSPOS.DAO.GetData.SyncGetStores();
			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncGetStores");
			}
		}

		/// <summary>
		/// funcion para crear un nuevo usuario
		/// recibe como parametro el modelo de usuario para agregar un nuevo registro userAssign
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public static BaseResponse CreateNewUser(UserAsingModel user)
		{
			try
			{
				return CLVSPOS.DAO.PostData.CreateNewUser(user);

			}
			catch (Exception exc)
			{
				throw;
			}
		}

		/// <summary>
		/// Funcion para editar un usuario
		/// recibe como parametro el modelo se usuario para editar un nuevo registro userAssign
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public static BaseResponse UpdateUser(UserAsingModel user)
		{
			try
			{
				return CLVSPOS.DAO.PostData.UpdateUser(user);

			}
			catch (Exception exc)
			{
				throw;
			}
		}
		public static BaseResponse UpdateUserApp(UserModel user)
		{
			try
			{
				return CLVSPOS.DAO.PostData.UpdateUserApp(user);

			}
			catch (Exception exc)
			{
				throw;
			}
		}
		public static BaseResponse CreateUserApp(UserModel user)
		{
			try
			{
				return CLVSPOS.DAO.PostData.CreateUserApp(user);

			}
			catch (Exception exc)
			{
				throw;
			}
		}

		public static UserAppResponse GetUserApp(string _id)
		{
			try
			{
				return CLVSPOS.DAO.GetData.GetUserApp(_id);

			}
			catch (Exception exc)
			{
				return (UserAppResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
			}
		}

		public static UsersAppResponse GetUsersApp()
		{
			try
			{
				return CLVSPOS.DAO.GetData.GetUsersApp();

			}
			catch (Exception exc)
			{
				return (UsersAppResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
			}
		}

		/// <summary>
		/// trae las listas de todos los pagos que se deben hacer sobre las facturas
		/// recibe como parametro el cardcode del cliente y la sede
		/// </summary>
		/// <param name="cardCode"></param>
		/// <param name="sede"></param>
		/// <param name="currency"></param>
		/// <returns></returns>
		public static InvoicesListResp GetPayInvoices(string cardCode, string sede, string currency)
		{
			try
			{
				CredentialHolder UserCredentials = GetCurrentUserCredentials();

				return CLVSPOS.SAPDAO.GetSapData.GetPayInvoices(cardCode, sede, currency, GetNameObject("spGetPayDocuments"), UserCredentials);
			}
			catch (Exception exc)
			{
				return (InvoicesListResp)LogManager.HandleExceptionWithReturn(exc, "InvoicesListResp");
			}
		}
		/// <summary>
		/// Obtiene el detalle de pago de una factura
		/// </summary>
		/// <param name="_docEntry"></param>
		/// <returns></returns>
		public static InvoicePaymentDetailResponse GetInvoicePaymentDetail(int _docEntry)
		{
			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return CLVSPOS.SAPDAO.GetSapData.GetInvoicePaymentDetail(userCredentials, _docEntry, GetNameObject("spGetInvoDetail"));
			}
			catch (Exception exc)
			{
				return (InvoicePaymentDetailResponse)LogManager.HandleExceptionWithReturn(exc, "InvoicePaymentDetailResponse");
			}
		}

		/// <summary>
		/// Metodo para obtener las series de numeracion con los que cuenta una compañias
		/// no recibe parametros 
		/// </summary>
		/// <returns></returns>
		public static NumberingSeriesModelResponse GetSeries()
		{
			try
			{
				var userId = GetUserId();
				var company = GetData.GetCompanyByUserId(userId);
				return CLVSPOS.DAO.GetData.GetSeries(company);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public static SyncResponse SyncGetSeries()
		{
			try
			{
				return CLVSPOS.DAO.GetData.SyncGetSeries();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public static SyncResponse SyncGetSeriesByUsers()
		{
			try
			{
				return CLVSPOS.DAO.GetData.SyncGetSeriesByUsers();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		/// <summary>
		/// Obtiene la lista de las enumeraciones con los tipos de series que hay 
		/// ejemplo, facturacion - cotizacion - pagos
		/// no recibe parametros
		/// </summary>
		/// <returns></returns>
		public static enumsResponse GetSeriesType()
		{
			try
			{
				return CLVSPOS.DAO.GetData.GetSeriesType();
			}
			catch (Exception ex)
			{

				throw ex;
			}
		}

		/// <summary>
		/// Obtiene los tipos de serie de numeracion que hay  ejemplo, manual - automatico
		/// no recibe parametros
		/// </summary>
		/// <returns></returns>
		public static enumsResponse GetSeriesTypeNumber()
		{
			try
			{
				return CLVSPOS.DAO.GetData.GetSeriesTypeNumber();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		/// <summary>
		/// Funcion que actualiza los cambios cuando se modifica una serie
		/// recibe como parametro el modelo de la serie
		/// </summary>
		/// <param name="serie"></param>
		/// <returns></returns>
		public static BaseResponse UpdateSerie(NumberingSeriesModel serie)
		{
			try
			{
				return CLVSPOS.DAO.PostData.UpdateSerie(serie);
			}
			catch (Exception exc)
			{
				return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
			}
		}

		/// <summary>
		/// Funcion que crea una nueva serie en la bd
		/// recibe como parametro el modelo de la serie
		/// </summary>
		/// <param name="serie"></param>
		/// <returns></returns>
		public static BaseResponse CreateNewSerie(NumberingSeriesModel serie)
		{
			try
			{
				return CLVSPOS.DAO.PostData.CreateNewSerie(serie);
			}
			catch (Exception exc)
			{
				return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
			}
		}

		/// <summary>
		/// Funcion que trae las listas de las cuentas
		/// no recibe parametros
		/// </summary>
		/// <returns></returns>
		public static ApiResponse<ContableAccounts> GetAccounts()
		{
			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();

				return CLVSPOS.SAPDAO.GetSapData.GetAccounts(userCredentials, GetNameObject("spGetContableAccounts"));
			}
			catch
			{
				throw;
			}
		}

		public static SyncResponse SyncGetAccounts(string userId)
		{
			try
			{
				var company = GetData.GetCompanyByUserId(userId);
				return CLVSPOS.SAPDAO.GetSapData.SyncGetAccounts(company, GetNameObject("viewGETACCOUNTS"));
			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse");
			}
		}

		/// <summary>
		/// Funcion que encripta datos en SHA256
		/// </summary>
		/// <param name="inputString"></param>
		/// <returns></returns>
		public static string GenerateSHA256String(string inputString)
		{
			SHA256 sha256 = SHA256Managed.Create();
			byte[] bytes = Encoding.UTF8.GetBytes(inputString);
			byte[] hash = sha256.ComputeHash(bytes);
			return GetStringFromHash(hash);
		}

		/// <summary>
		/// Retorna un string a partir del hash recibido
		/// </summary>
		/// <param name="hash"></param>
		/// <returns></returns>
		private static string GetStringFromHash(byte[] hash)
		{
			StringBuilder result = new StringBuilder();
			for (int i = 0; i < hash.Length; i++)
			{
				result.Append(hash[i].ToString("X2"));
			}
			return result.ToString();
		}

		/// <summary>
		/// trae las listas de las tarjetas
		/// no recibe parametros
		/// </summary>
		/// <returns></returns>
		public static CardsResponse GetCards()
		{
			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return CLVSPOS.SAPDAO.GetSapData.GetCards(userCredentials, GetNameObject("viewGETCREDITCARDS"));
			}
			catch (Exception exc)
			{
				return (CardsResponse)LogManager.HandleExceptionWithReturn(exc, "CardsResponse");
			}
		}

		public static SyncResponse SyncGetCards(string userId)
		{
			try
			{
				var company = GetData.GetCompanyByUserId(userId);
				return CLVSPOS.SAPDAO.GetSapData.SyncGetCards(company, GetNameObject("viewGETCREDITCARDS"));
			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "CardsResponse");
			}
		}

		/// <summary>
		/// obtiene la cuenta cuentas de los bancos
		/// no recibe parametros
		/// </summary>
		/// <returns></returns>
		public static BankResponse GetAccountsBank()
		{
			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return CLVSPOS.SAPDAO.GetSapData.GetAccountsBank(userCredentials, GetNameObject("viewGetBanks"));
			}
			catch (Exception exc)
			{
				return (BankResponse)LogManager.HandleExceptionWithReturn(exc, "BankResponse");
			}
		}

		public static SyncResponse SyncGetAccountsBank(string userId)
		{
			try
			{
				var company = GetData.GetCompanyByUserId(userId);
				return CLVSPOS.SAPDAO.GetSapData.SyncGetAccountsBank(company, GetNameObject("viewGetBanks"));
			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "BankResponse");
			}
		}

		/// <summary>
		/// obtiene la cuenta cuentas de los vendedores
		/// no recibe parametros
		/// </summary>
		/// <returns></returns>
		public static SalesManResponse GetSalesMan()
		{
			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return CLVSPOS.SAPDAO.GetSapData.GetSalesMan(userCredentials, GetNameObject("viewGetSalesMan"));
			}
			catch (Exception exc)
			{
				return (SalesManResponse)LogManager.HandleExceptionWithReturn(exc, "SalesManResponse");
			}
		}
		public static SalesManResponse GetSalesManBalance()
		{
			try
			{
				return CLVSPOS.DAO.GetData.GetSalesMan(GetNameObject("viewGetSalesManApp"));
			}
			catch (Exception exc)
			{
				return (SalesManResponse)LogManager.HandleExceptionWithReturn(exc, "SalesManResponse");
			}
		}
		public static SyncResponse SyncGetSalesMan(string userId)
		{
			try
			{
				var company = GetData.GetCompanyByUserId(userId);
				return CLVSPOS.SAPDAO.GetSapData.SyncGetSalesMan(company, GetNameObject("viewGetSalesMan"));
			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SalesManResponse");
			}
		}


		/// <summary>
		/// metodo para realizar una cancelacion de un pago en SAP
		/// recibe como parametro el modelo de cancelacion de pago
		/// </summary>
		/// <param name="canPay"></param>
		/// <returns></returns>
		public static BaseResponse CancelPayment(CancelPayModel canPay)
		{
			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return CLVSPOS.SAPDAO.PostSapData.CancelPayment(canPay, userCredentials);
			}
			catch (Exception exc)
			{
				return (PaymentSapResponse)LogManager.HandleExceptionWithReturn(exc, "PaymentSapResponse");
			}
		}

		/// <summary>
		/// metodos que jala las listas de facturas a las cuales se les va a cancelar el pago
		/// parametros el modelo de datos de la informacion para buscar la lista
		/// </summary>
		/// <param name="canPay"></param>
		/// <returns></returns>
		public static CancelpaymentResponce GetPaymentList(paymentSearchModel canPay)
		{
			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return CLVSPOS.SAPDAO.GetSapData.GetPaymentList(userCredentials, canPay, GetNameObject("spGetCancelPayment"));
			}
			catch (Exception exc)
			{
				return (CancelpaymentResponce)LogManager.HandleExceptionWithReturn(exc, "CancelpaymentResponce");
			}
		}

		/// <summary>
		/// metodo para registrar un usuario desde la app
		///recibe como parametro un RegisterUser, que contiene un UserDB
		/// </summary>
		/// <param name="registerUser"></param>
		/// <returns></returns>
		public static BaseResponse RegisterUser(User registerUser)
		{
			try
			{
				bool exist = CLVSPOS.DAO.GetData.VerifyUserExist(registerUser.Email);
				if (!exist)
				{
					//return NDESMO.DAO.PostData.RegisterUser(RegisterUser);

					BaseResponse br = CLVSPOS.DAO.PostData.RegisterUser(registerUser);
					if (br.Result)
					{

						// token = GetToken(registerUser.Email, registerUser.Password);
						//string UrlVerificationAccount = System.Configuration.ConfigurationManager.AppSettings["UrlVerificationAccount"].ToString();
						//string linkMsg = "Completar Registro";
						//string body = string.Format("Estimado Usuario<BR/>Para completar el registro por favor ingrese al siguiente link: <a href=\"{0}\" title=\"User Email Confirm\">{1}</a>", UrlVerificationAccount + token + "/1", linkMsg);
						//SendEmail(registerUser.Email, registerUser.FullName, body);
					}
					return br;
				}
				return new BaseResponse
				{
					Result = false,
					Error = new ErrorInfo
					{
						Code = -1,
						Message = "El usuario: " + registerUser.Email + " ya esta registrado en la aplicacion"
					}
				};
			}
			catch (Exception exc)
			{
				return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
			}
		}

		/// <summary>
		/// Metodo para recuperar la contrasenna del usuario, enviando un correo para la recuperacion
		/// recibe como parametro un StringModel, que contiene un string con el correo
		/// </summary>
		/// <param name="userEmail"></param>
		/// <returns></returns>
		public static BaseResponse SendRecoverPswdEmail(StringModel userEmail)
		{
			try
			{
				bool exist = CLVSPOS.DAO.GetData.VerifyUserExist(userEmail.word);
				if (exist)
				{
					string UrlRecoverPswd = System.Configuration.ConfigurationManager.AppSettings["UrlRecoverPswd"].ToString();
					string linkMsg = "Recuperacion de contraseña";
					string Body = string.Format("Estimado Usuario<BR/>Intento de recuperacion de contraseña, para completar el registro por favor ingrese al siguiente link: <a href=\"{1}\" title=\"User Email Confirm\">{2}</a>", userEmail.word, UrlRecoverPswd + userEmail.word, linkMsg);
					SendEmail(userEmail.word, linkMsg, Body);
					return new BaseResponse
					{
						Result = true
					};
				}
				return new BaseResponse
				{
					Result = false,
					Error = new ErrorInfo
					{
						Code = -1,
						Message = "El usuario: " + userEmail.word + " no esta registrado en la aplicacion"
					}
				};
			}
			catch (Exception exc)
			{
				return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
			}
		}


		private static HttpWebRequest CreateWebRequest(string url, string method, string contentType)
		{
			try
			{
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
				IWebProxy theProxy = request.Proxy;

				if (theProxy != null)
				{
					theProxy.Credentials = CredentialCache.DefaultCredentials;
				}

				CookieContainer cookies = new CookieContainer();
				request.UseDefaultCredentials = true;
				request.CookieContainer = cookies;

				request.ContentType = contentType;
				request.Method = method;

				return request;
			}
			catch (Exception)
			{
				throw;
			}
		}

		public class AuthorizationToken
		{
			public string access_token { get; set; } = string.Empty;
			public string userId { get; set; } = string.Empty;
		}

		/// <summary>
		/// Metodo para obtener el token al registrar un usuario nuevo
		/// Recibe como parametro el correo y la contrasenna
		/// </summary>
		/// <param name="email"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public static string GetToken(string email, string password)
		{
			string FeAppToken = System.Configuration.ConfigurationManager.AppSettings["AppToken"].ToString();

			HttpWebRequest request = CreateWebRequest(FeAppToken, "POST", "application/x-www-form-urlencoded");

			string body = string.Format("&Username={0}&Password={1}&grant_type=password", email, password);

			byte[] byteArray = Encoding.UTF8.GetBytes(body);

			var data = Encoding.ASCII.GetBytes(body); // or UTF8

			request.ContentLength = data.Length;


			System.IO.Stream dataStream = request.GetRequestStream();
			dataStream.Write(byteArray, 0, byteArray.Length);
			dataStream.Close();

			using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
			{
				System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream());
				string responseJSON = reader.ReadToEnd();

				var settings = new JsonSerializerSettings
				{
					PreserveReferencesHandling = PreserveReferencesHandling.All
				};

				var root = JsonConvert.DeserializeObject<AuthorizationToken>(responseJSON, settings);

				if (root != null)
				{
					return root.access_token;
				}
				else
				{
					return string.Empty;
				}
			}

			//string FeAppToken = System.Configuration.ConfigurationManager.AppSettings["AppToken"].ToString();
			//var client = new RestClient(FeAppToken);
			//var request = new RestRequest(Method.POST);
			//request.AddHeader("cache-control", "no-cache");
			//request.AddHeader("content-type", "application/x-www-form-urlencoded");
			//request.AddParameter("application/x-www-form-urlencoded", "grant_type=password&UserName=" + email + "&Password=" + password, RestSharp.ParameterType.RequestBody);
			//IRestResponse response = client.Execute(request);

			//var content = response.Content; // raw content as string
			//var token = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<TokenModel>(content);
			//return token.access_token;
		}

		/// <summary>
		/// Metodo para enviar un correo de confirmacion de la cuenta
		/// Recibe como parametro el correo y la contrasena
		/// </summary>
		/// <param name="to"></param>
		/// <param name="subject"></param>
		/// <param name="body"></param>
		public static void SendEmail(string to, string subject, string body)
		{
			var message = new MailMessage();
			message.To.Add(new MailAddress(to));
			message.From = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["Email"].ToString());
			message.Subject = subject;
			message.Body = body;
			message.IsBodyHtml = true;
			message.BodyEncoding = UTF8Encoding.UTF8;
			message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
			using (var smtp = new SmtpClient())
			{
				var credential = new NetworkCredential
				{
					UserName = System.Configuration.ConfigurationManager.AppSettings["EmailUser"].ToString(),
					Password = System.Configuration.ConfigurationManager.AppSettings["EmailPassword"].ToString()
				};
				smtp.Port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Port"]);
				smtp.Host = System.Configuration.ConfigurationManager.AppSettings["Host"].ToString();
				smtp.EnableSsl = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["EnableSsl"]);
				smtp.Timeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Timeout"]);
				smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
				smtp.UseDefaultCredentials = false;
				smtp.Credentials = credential;
				smtp.Send(message);
			}
		}

		/// <summary>
		/// metodo para el envio de la informcion para el cambio de la contrasenna de la cuenta del usuario
		/// recibe como parametro un recoverPswd, que contiene un recoverPswd
		/// </summary>
		/// <param name="recoverPswd"></param>
		/// <returns></returns>
		public static BaseResponse RecoverPswd(User recoverPswd)
		{
			try
			{
				bool exist = CLVSPOS.DAO.GetData.VerifyUserExist(recoverPswd.Email);
				if (exist)
				{
					return CLVSPOS.DAO.PostData.RecoverPswd(recoverPswd);
				}
				return new BaseResponse
				{
					Result = false,
					Error = new ErrorInfo
					{
						Code = -1,
						Message = "El usuario: " + recoverPswd.Email + " no esta registrado en la aplicacion"
					}
				};
			}
			catch (Exception exc)
			{
				return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
			}
		}

		/// <summary>
		/// metodos que jala las listas de facturas para mostrar en la vista de reimprecion
		/// parametros el modelo de datos de la informacion para buscar la lista
		/// </summary>
		/// <param name="inv"></param>
		/// <returns></returns>
		public static InvListPrintResponde GetInvPrintList(invPrintSearch inv)
		{
			try
			{
				return CLVSPOS.SAPDAO.GetSapData.GetInvPrintList(GetCurrentUserCredentials(), inv, GetNameObject("spGETINVPRINTLIST_SPR"));
			}
			catch (Exception exc)
			{
				return (InvListPrintResponde)LogManager.HandleExceptionWithReturn(exc, "InvListPrintResponde");
			}
		}


		/// <summary>
		/// actualiza los cambios cuando se un parametro asociado a una vista
		/// recibe como parametro una lisata de un modelo de parametros para una vista
		/// </summary>
		/// <param name="Params"></param>
		/// <returns></returns>
		public static BaseResponse UpdateParamsViewState(List<ParamsModel> Params)
		{
			try
			{
				return CLVSPOS.DAO.PostData.UpdateParamsViewState(Params);

			}
			catch (Exception exc)
			{
				return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
			}
		}

		/// <summary>
		/// Trae la informacion de las listas de precios 
		/// no recibe parametros
		/// </summary>
		/// <returns></returns>
		public static PriceListResponse GetPriceList()
		{
			try
			{

				return CLVSPOS.SAPDAO.GetSapData.GetPriceList(GetCurrentUserCredentials(), GetNameObject("viewPRICELIST"));

			}
			catch (Exception exc)
			{
				return (PriceListResponse)LogManager.HandleExceptionWithReturn(exc, "PriceListResponse");
			}
		}
		public static PriceListSelfResponse GetDefaultPriceList(string _cardCode)
		{
			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return CLVSPOS.SAPDAO.GetSapData.GetDefaultPriceList(userCredentials, _cardCode, GetNameObject("spGetDefaultPriceList"));

			}
			catch (Exception exc)
			{
				return (PriceListSelfResponse)LogManager.HandleExceptionWithReturn(exc, "PriceListSelfResponse");
			}
		}

		public static SyncResponse SyncGetPriceList(string userId)
		{
			try
			{
				var company = GetData.GetCompanyByUserId(userId);
				return CLVSPOS.SAPDAO.GetSapData.SyncGetPriceList(company, GetNameObject("viewPRICELIST"));

			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "SyncResponse");
			}
		}

		/// <summary>
		/// Trae la informacion de los terminos de pagos
		/// no recibe parametros
		/// </summary>
		/// <returns></returns>
		public static PayTermsResponse GetPayTermsList()
		{
			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return CLVSPOS.SAPDAO.GetSapData.GetPayTermsList(userCredentials, GetNameObject("viewGetPayTerms"));

			}
			catch (Exception exc)
			{
				return (PayTermsResponse)LogManager.HandleExceptionWithReturn(exc, "PayTermsResponse");
			}
		}

		public static SyncResponse SyncGetPayTermsList(string userId)
		{
			try
			{
				var company = GetData.GetCompanyByUserId(userId);
				return CLVSPOS.SAPDAO.GetSapData.SyncGetPayTermsList(company, GetNameObject("viewGetPayTerms"));
			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "PayTermsResponse");
			}
		}

		/// <summary>
		/// Funcion para obtener el logo de la compannia desde DBLocal, obteniendo la compannia a la cual se esta logueado
		/// no recibe parametros
		/// </summary>
		/// <returns></returns>
		public static BaseResponse GetCompanyLogo()
		{
			try
			{
				var userId = GetUserId();
				var company = GetData.GetCompanyByUserId(userId);
				return CLVSPOS.DAO.GetData.GetCompanyLogo(company);
			}
			catch (Exception exc)
			{
				return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, "BaseResponse");
			}
		}



		/// <summary>
		/// #001
		/// Metodo para crear una factura, modificado por Alejandro para realizar el pago si la factura ya existe y dio error creando los pagos
		/// El metodo original se llama: CreateInvoiceBACKUP.
		/// 01/09/2021
		/// </summary>
		/// <param name="createInvoice"></param>
		/// <returns></returns>
		/// <Pendiente> * </Pendiente>


		public static async Task<InvoiceSapResponse> CreateInvoice(CreateSlInvoice createInvoice)
		{
			try
			{

				CredentialHolder UserCredentials = GetCurrentUserCredentials();



				string userId = GetUserId();

				if (createInvoice.Payment != null) {

					if (createInvoice.Invoice.DocCurrency.Equals(createInvoice.Payment.DocCurrency))
					{
						createInvoice.Payment.DocRate = 0;

					}
				}


				// Se consulta si la factura ya existe en sap mediante el UniqueInvID
				InvoiceSapResponse InvExist = SAPDAO.GetSapData.CheckUniqueInv(createInvoice.Invoice.U_CLVS_POS_UniqueInvId, GetNameObject("spGetCheckUniqueInvIdReturnInfo"), UserCredentials);
				//Si Existe en sap esta factura
				if (InvExist.Result)
				{
					// Se consulta si la factura que ya existe en sap tiene algun pago hecho.
					InvoiceSapResponse oInvoiceHasPayments = SAPDAO.GetSapData.HasPayment(InvExist.DocEntry, GetNameObject("spCheckForPayments"), UserCredentials);

					// Si ya cuenta con algun pago hecho.
					if (oInvoiceHasPayments.Result)
					{
						// Variable de respuesta.
						InvoiceSapResponse oInvoiceAlreadypaid = new InvoiceSapResponse();
						// Consultamos el documento para obtener los datos que vamos a devolver a UI
						DocInfo oDocInfo = SAPDAO.GetSapData.GetDocNumByDocEntry(UserCredentials, InvExist.DocEntry, GetNameObject("spGetNumFEOnline"));

						// Llenamos la variable de respuesta con la data cosultada
						oInvoiceAlreadypaid.DocNum = oDocInfo.DocNum;
						oInvoiceAlreadypaid.DocEntry = InvExist.DocEntry;
						oInvoiceAlreadypaid.Consecutivo = oDocInfo.ClaveFE;
						oInvoiceAlreadypaid.NumDocFe = oDocInfo.NumFE;
						oInvoiceAlreadypaid.PaymentResponse = new PaymentSapResponse
						{
							DocEntry = oInvoiceHasPayments.DocEntry,
							Result = true
						};
						oInvoiceAlreadypaid.Result = true;

						return oInvoiceAlreadypaid;
					}
					else
					{
						// Si no cuenta con algun pago
						createInvoice.Invoice.DocEntry = InvExist.DocEntry;

						CreateInvoice temp = new CreateInvoice();//Esto no va solo es por mientras
						PaymentSapResponse payment = await SAPDAO.PostSapData.CreatePaymentForInvoice(createInvoice.Payment, InvExist.DocEntry, userId, UserCredentials);

						if (payment.Result)
						{

							// Variable de respuesta.
							InvoiceSapResponse oInvoiceAlreadypaid = new InvoiceSapResponse();
							// Consultamos el documento para obtener los datos que vamos a devolver a UI
							DocInfo oDocInfo = SAPDAO.GetSapData.GetDocNumByDocEntry(UserCredentials, InvExist.DocEntry, GetNameObject("spGetNumFEOnline"));

							// Llenamos la variable de respuesta con la data cosultada
							oInvoiceAlreadypaid.DocNum = oDocInfo.DocNum;
							oInvoiceAlreadypaid.DocEntry = InvExist.DocEntry;
							oInvoiceAlreadypaid.Consecutivo = oDocInfo.ClaveFE;
							oInvoiceAlreadypaid.NumDocFe = oDocInfo.NumFE;
							oInvoiceAlreadypaid.PaymentResponse = payment;
							oInvoiceAlreadypaid.Result = true;

							return oInvoiceAlreadypaid;
						}
						else
						{
							string message = string.Format("No se crearon los pagos para la factura con UniqueInvId {0}.", createInvoice.Invoice.U_CLVS_POS_UniqueInvId);
							LogManager.LogMessage(message, (int)Constants.LogTypes.API);
							throw new Exception(message);
						}
					}
				}
				else
				{
					// Como no existe se sigue el flujo tal y como estaba sin ninguna modificacion.
					//return await SAPDAO.PostSapData.CreateInvoice(createInvoice, userId, GetNameObject("spGetFatherCard"), GetNameObject("spGetNumFEOnline"), UserCredentials);


					// Como no existe se sigue el flujo tal y como estaba sin ninguna modificacion.

					if (createInvoice.PPTransaction != null)
					{ // Esto implica que se uso pin pad para cubrir todo o parte del monto de la factura
						if (String.IsNullOrEmpty(createInvoice.PPTransaction.TransactionId))
						{
							throw new Exception("No se pudo obtener el identificador de la transacción de la targeta y es un campo requerido");
						}

						if (GetData.CheckIfTransactionExists(createInvoice.PPTransaction.TransactionId))
						{
							DateTime lastUpdate = DateTime.Now;

							PostData.UpdateLastTransaction(createInvoice.PPTransaction, lastUpdate);

							// write some logs with the getted date
						}
						else
						{
							PostData.createPPTransaction(createInvoice.PPTransaction);
						}
					}
					//
					InvoiceSapResponse oInvoiceSapResponse = await SAPDAO.PostSapData.CreateInvoice(createInvoice, userId, GetNameObject("spGetFatherCard"), GetNameObject("spGetNumFEOnline"), UserCredentials);

					if (createInvoice.PPTransaction != null && oInvoiceSapResponse.Result)
					{
						DateTime lastUpdate = DateTime.Now;
						PostData.UpdatePPTransactionReferences(createInvoice.PPTransaction, lastUpdate, oInvoiceSapResponse.DocEntry, oInvoiceSapResponse.DocNum);
						// write some logs
					}

					return oInvoiceSapResponse;


				}
			}
			catch
			{
				throw;
			}
		}

		public static InvoiceSapResponse CreateInvoiceNc(CreateInvoice createInvoice)
		{
			try
			{
				var userId = GetUserId();


				CredentialHolder UserCredentials = GetCurrentUserCredentials();



				bool InvExist = SAPDAO.GetSapData.CheckUniqueInvId(createInvoice.Invoice.CLVS_POS_UniqueInvId, UserCredentials, GetNameObject("spGetCheckUniqueInvId"));
				if (InvExist)
				{
					throw new Exception(string.Format("La nota de credito con UniqueInvId {0} ya está registrada en SAP.", createInvoice.Invoice.CLVS_POS_UniqueInvId));
				}
				return SAPDAO.PostSapData.CreateInvoiceNc(createInvoice, UserCredentials, userId, GetNameObject("spGetFatherCard"));
			}
			catch (Exception exc)
			{
				return (InvoiceSapResponse)LogManager.HandleExceptionWithReturn(exc, "InvoiceNcSapResponse");
			}
		}

		/// <summary>
		/// Trae la informacion de los terminos de pagos
		/// </summary>
		/// <param name="companyId"></param>
		/// <returns></returns>
		public static WHPlaceResponce GetStoresList(int companyId)
		{
			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return CLVSPOS.SAPDAO.GetSapData.GetStoresList(userCredentials, GetNameObject("viewETWHAREHOUSES"));

			}
			catch (Exception exc)
			{
				return (WHPlaceResponce)LogManager.HandleExceptionWithReturn(exc, "WHPlaceResponce");
			}
		}

		/// <summary>
		/// crea un nuevo almecen para una compañia
		/// de parametro obtiene un modelo de tipo Store
		/// </summary>
		/// <param name="store"></param>
		/// <returns></returns>
		public static BaseResponse CreateStore(StoresModel store)
		{
			try
			{
				return CLVSPOS.DAO.PostData.CreateStore(store);

			}
			catch (Exception exc)
			{
				return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
			}
		}

		/// <summary>
		/// Funcion que actualiza un almacen en especifico
		/// Como parametro recibe un modelo tipo Store
		/// </summary>
		/// <param name="store"></param>
		/// <returns></returns>
		public static BaseResponse UpdateStore(StoresModel store)
		{
			try
			{
				return CLVSPOS.DAO.PostData.UpdateStore(store);

			}
			catch (Exception exc)
			{
				return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
			}
		}

		public static StoreModelResult GetStorebyId(int store)
		{
			try
			{
				//var userId = GetUserId();
				//var company = GetData.GetCompanyBycompanyId(companyId);
				return CLVSPOS.DAO.GetData.GetStorebyId(store);

			}
			catch (Exception exc)
			{
				return (StoreModelResult)LogManager.HandleExceptionWithReturn(exc, "StoreModelResult");
			}
		}

		/// <summary>
		/// Metodo para verificar la cuenta del propietario 
		/// </summary>
		/// <returns></returns>
		public static BaseResponse ConfirmEmail()
		{
			try
			{
				var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
				var userId = identity.Claims.Where(c => c.Type == "userId").Single().Value;
				using (SuperV2_Entities dbDao = new SuperV2_Entities())
				{
					try
					{
						using (DbContextTransaction transactionDao = dbDao.Database.BeginTransaction())
						{
							var user = dbDao.Users.Find(userId);

							if (user != null)
							{
								user.EmailConfirmed = true;
								dbDao.Entry(user).State = EntityState.Modified;
								dbDao.SaveChanges();
								transactionDao.Commit();
							}
							else
							{
								return new BaseResponse
								{
									Result = false,
									Error = new ErrorInfo
									{
										Code = -1,
										Message = "No se encontro el usuario"
									}
								};
							}
						}
						return new BaseResponse
						{
							Result = true
						};
					}
					catch (Exception exc)
					{
						return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
					}
				}
			}
			catch (Exception exc)
			{
				return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
			}
		}

		public static PermsModelResponse GetPermsByUserMenu()
		{
			try
			{
				var userId = GetUserId();
				return CLVSPOS.DAO.GetData.GetPermsByUser(userId);

			}
			catch (Exception exc)
			{
				return (PermsModelResponse)LogManager.HandleExceptionWithReturn(exc, "PermsModelResponse");
			}
		}

		public static discountResponse GetDiscount()
		{
			var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
			var userId = identity.Claims.Where(c => c.Type == "userId").Single().Value;
			try
			{
				decimal disc = CLVSPOS.DAO.GetData.getDiscount(userId);
				return new discountResponse
				{
					discount = disc,
					Result = true
				};
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public static BalanceByUserResponse GetBalanceInvoices_UsrOrTime(GetBalanceModel_UsrOrDate BalanceModel)
		{
			try
			{
				var userId = GetUserId();
				var company = GetData.GetCompanyByUserId(userId);
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				BalanceModel.DbCode = company.DBCode.ToString();
				return SAPDAO.GetSapData.GetBalanceInvoices_UsrOrTime(BalanceModel, GetNameObject("spGETUSRBALANCE_CREDITNOTES"), GetNameObject("spGETUSRBALANCE"), userCredentials);
			}
			catch (Exception exc)
			{
				return (BalanceByUserResponse)LogManager.HandleExceptionWithReturn(exc, "BalanceByUserResponse");
			}

		}


		public static InvoiceSapResponse SyncCreateInvoice(OFF_CreateInvoice createInvoice)
		{
			try
			{
				var userId = GetUserId();
				var company = GetData.GetCompanyByUserId(userId);
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return CLVSPOS.SAPDAO.PostSapData.SyncCreateInvoice(createInvoice, company, userId, GetNameObject("spGetNumFEOnline"), userCredentials);
			}
			catch (Exception exc)
			{
				return (InvoiceSapResponse)LogManager.HandleExceptionWithReturn(exc, "SyncCreateInvoice");
			}
		}

		/// <summary>
		/// Crea un item basado en su modelo, ademas de que comprueba la disponibilidad de los codigos de barra que el item tendra
		/// </summary>
		/// <param name="_itemModel"></param>
		/// <returns></returns>
		public static ItemsResponse CreateItem(ItemsModel _itemModel)
		{
			try
			{
				var userId = GetUserId();
				var company = GetData.GetCompanyByUserId(userId);

				CredentialHolder UserCredentials = GetCurrentUserCredentials();


				// Verifica que los codigos de barras que se pretenden ingresar esten disponibles para asociarlos al item
				foreach (var barcode in _itemModel.Barcodes)
				{
					ItemsResponse itemResponse = CLVSPOS.SAPDAO.GetSapData.GetItemByBarcode(barcode.BcdCode, GetNameObject("spGetItemByCodeBar"), UserCredentials);
					if (itemResponse.Result)
					{
						return new ItemsResponse
						{
							Result = false,
							Error = new ErrorInfo
							{
								Code = 505,
								Message = $"El código de barras {barcode.BcdCode}, se encuentra asociado a {itemResponse.Item.ItemName}."
							}
						};
					}
				}
				return CLVSPOS.SAPDAO.PostSapData.CreateItem(_itemModel, UserCredentials);
			}
			catch (Exception exc)
			{
				return (ItemsResponse)LogManager.HandleExceptionWithReturn(exc, "SyncCreateInvoice");
			}
		}

		/// <summary>
		/// Actualiza los valores del item y verifica que si en caso de que el usuario haya agreado nuevos codigos estos esten disponibles para el item
		/// </summary>
		/// <param name="_itemModel"></param>
		/// <returns></returns>
		public static ItemsResponse UpdateItem(ItemsModel _itemModel)
		{
			try
			{
				var userId = GetUserId();
				var company = GetData.GetCompanyByUserId(userId);

				CredentialHolder userCredentials = GetCurrentUserCredentials();

				var barcodes_sp = new List<ItemsBarcodeModel>();

				CredentialHolder UserCredentials = GetCurrentUserCredentials();

				// Consulta los items que ya tiene asociados el producto
				var result = CLVSPOS.SAPDAO.GetSapData.GetBarcodesByItem(_itemModel.ItemCode, userCredentials, GetNameObject("spGetBarCodeByItem")).Item;
				if (result != null)
				{
					barcodes_sp = result.Barcodes;
					foreach (var barcode_ui in _itemModel.Barcodes)
					{

						ItemsResponse bcd = checkBarcode(barcode_ui.BcdCode, UserCredentials);

						if (bcd.Result) // Comprueba si el codigo esta libre
						{
							if (bcd.Item.ItemCode != _itemModel.ItemCode)
							{
								return new ItemsResponse
								{
									Result = false,
									Error = new ErrorInfo
									{
										Code = 505,
										Message = $"El código de barras {barcode_ui.BcdCode}, ya se encuentra asociado a {bcd.Item.ItemName}"
									}
								};
							}
						}
					}
				}
				return CLVSPOS.SAPDAO.PostSapData.UpdateItem(_itemModel, UserCredentials, barcodes_sp);
			}
			catch (Exception exc)
			{
				return (ItemsResponse)LogManager.HandleExceptionWithReturn(exc, "Update Item");
			}
		}

		/// <summary>
		/// Es usado a modo de comprobacion de la disponibilidad de un codigo de barras
		/// </summary>
		/// <param name="_barcode">Recibe el codigo de barras a buscar</param>
		/// <param name="company"></param>
		/// <returns></returns>
		private static ItemsResponse checkBarcode(string _barcode, CredentialHolder _UserCredentials)
		{
			return CLVSPOS.SAPDAO.GetSapData.GetItemByBarcode(_barcode, GetNameObject("spGetItemByCodeBar"), _UserCredentials);
		}

		public static ItemsResponse GetItemPriceList(string _itemCode)
		{
			try
			{

				return CLVSPOS.SAPDAO.GetSapData.GetItemPriceList(_itemCode, GetCurrentUserCredentials(), GetNameObject("spGetItemList"));
			}
			catch (Exception exc)
			{
				throw exc;
				//return (ItemsResponse)LogManager.HandleExceptionWithReturn(exc, "ItemsResponse");
			}
		}

		public static ItemsResponse GetBarcodesByItem(string _itemCode)
		{
			try
			{

				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return CLVSPOS.SAPDAO.GetSapData.GetBarcodesByItem(_itemCode, userCredentials, GetNameObject("spGetBarCodeByItem"));
			}
			catch (Exception exc)
			{
				return (ItemsResponse)LogManager.HandleExceptionWithReturn(exc, "ItemsResponse");
			}
		}

		public static ItemsResponse CreateGoodsReceipt(GoodsReceipt _goodsReceipt)
		{
			try
			{
				CredentialHolder UserCredemtials = GetCurrentUserCredentials();


				DocInfo docInfo = SAPDAO.GetSapData.CheckUniqueDocumentID("OPDN", _goodsReceipt.U_CLVS_POS_UniqueInvId, UserCredemtials, GetNameObject("spCheckUniqueDocumentID"));

				if (docInfo != null)
				{
					return new ItemsResponse()
					{
						Result = true,
						DocEntry = docInfo.DocEntry,
						DocNum = docInfo.DocNum
					};
				}






				return SAPDAO.PostSapData.CreateGoodsReceipt(_goodsReceipt, UserCredemtials, GetNameObject("spGetGoodReceiptDocNum"));
			}
			catch
			{
				throw;
			}
		}
		public static GoodsReciptXmlResponse CreateGoodsReciptXml(HttpRequest _httpRequest)
		{
			try
			{
				string WhsCode = _httpRequest.Form["WhsCode"];
				string userId = _httpRequest.Form["UserId"];
				List<HttpPostedFile> postedFiles = new List<HttpPostedFile>
				{
					_httpRequest.Files[$"file"]
				};

                if (postedFiles.Any(x => ((x.ContentType != "text/xml"))))
                {
                    throw new Exception("Solo se pueden subir archivos xml");
                }

                byte[] _contentBytes;
				_contentBytes = StreamToBytes(postedFiles[0].InputStream);

				CLVSPOS.MODELS.File file = new CLVSPOS.MODELS.File
				{
					Content = postedFiles[0].InputStream,
					Name = postedFiles[0].FileName,
					Type = postedFiles[0].ContentType,
					Base64 = Convert.ToBase64String(_contentBytes)
				};

				return ParseXML(file, WhsCode, userId);

			}
			catch (Exception)
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

		private static GoodsReciptXmlResponse ParseXML(CLVSPOS.MODELS.File file, string WhsCode, string userId)
		{
			try
			{
				XmlWriterSettings oXmlWriterSettings = new XmlWriterSettings();

				XmlSerializer oXmlSerializer = new XmlSerializer(typeof(GoodsReceipt), null, null, new XmlRootAttribute("GoodsReceipt"), null);

				string decodedXML = DecodeStringFromBase64(file.Base64);

				//string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());

				//if (decodedXML.StartsWith(_byteOrderMarkUtf8))
				//{
				//    decodedXML = decodedXML.Remove(0, _byteOrderMarkUtf8.Length);
				//}

				string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());

				if (decodedXML.StartsWith(_byteOrderMarkUtf8, StringComparison.Ordinal))
				{
					decodedXML = decodedXML.Remove(0, _byteOrderMarkUtf8.Length);
				}


				XmlDocument doc = new XmlDocument();
				doc.LoadXml(decodedXML);

				XmlElement myElement = doc.DocumentElement;

				XmlAttributeCollection attrColl = myElement.Attributes;

				GoodsReceipt _goodsReceipt = new GoodsReceipt();

				List<EntryLineModel> oLines = new List<EntryLineModel>();

				List<LineaDetalle> oLinesXml = new List<LineaDetalle>();

				Emisor oEmisorXml = new Emisor();

				for (int j = 0; j < myElement.ChildNodes.Count; j++)// DETALLESERVICIO
				{
					Dictionary<string, string> pairedData = new Dictionary<string, string>();				

					for (int c = 0; c < myElement.ChildNodes[j].ChildNodes.Count; c++)//LineaDetalle
					{
						XmlNode xmlElement = myElement.ChildNodes[j].ChildNodes[c];

						if (myElement.ChildNodes[j].ChildNodes[c].Name.Equals("LineaDetalle"))
						{
                            Dictionary<string, string> linesData = new Dictionary<string, string>();

                            for (int k = 0; k < myElement.ChildNodes[j].ChildNodes[c].ChildNodes.Count; k++)
							{
								if (k > 0)
								{
									if (myElement.ChildNodes[j].ChildNodes[c].ChildNodes[k].Name.Equals(myElement.ChildNodes[j].ChildNodes[c].ChildNodes[k - 1].Name))
									{

									}
									else
									{
										if (myElement.ChildNodes[j].ChildNodes[c].ChildNodes[k].Name.Equals("Impuesto"))
										{

											for (int l = 0; l < myElement.ChildNodes[j].ChildNodes[c].ChildNodes[k].ChildNodes.Count; l++) //Listaimpuesto 
											{
												if (myElement.ChildNodes[j].ChildNodes[c].ChildNodes[k].ChildNodes[l].Name.Equals("Tarifa"))
												{
													linesData.Add(myElement.ChildNodes[j].ChildNodes[c].ChildNodes[k].ChildNodes[l].Name, xmlElement.ChildNodes[k].ChildNodes[l].FirstChild?.InnerText);
												}
												if (myElement.ChildNodes[j].ChildNodes[c].ChildNodes[k].ChildNodes[l].Name.Equals("Exoneracion"))
												{
													for (int n = 0; n < myElement.ChildNodes[j].ChildNodes[c].ChildNodes[k].ChildNodes[l].ChildNodes.Count; n++) //ListaExoneracion
													{
														if (myElement.ChildNodes[j].ChildNodes[c].ChildNodes[k].ChildNodes[l].ChildNodes[n].Name.Equals("PorcentajeExoneracion"))
														{
															linesData.Add(myElement.ChildNodes[j].ChildNodes[c].ChildNodes[k].ChildNodes[l].ChildNodes[n].Name, xmlElement.ChildNodes[k].ChildNodes[l].ChildNodes[n].FirstChild?.InnerText);
														}															

													}
												}
											}

										}
										else
											if (myElement.ChildNodes[j].ChildNodes[c].ChildNodes[k].Name.Equals("Descuento"))
											{

												for (int l = 0; l < myElement.ChildNodes[j].ChildNodes[c].ChildNodes[k].ChildNodes.Count; l++) // 
												{
													if (myElement.ChildNodes[j].ChildNodes[c].ChildNodes[k].ChildNodes[l].Name.Equals("MontoDescuento"))
													{
														linesData.Add(myElement.ChildNodes[j].ChildNodes[c].ChildNodes[k].ChildNodes[l].Name, xmlElement.ChildNodes[k].ChildNodes[l].FirstChild?.InnerText);
													}
												}

										}
										else
											{
												linesData.Add(myElement.ChildNodes[j].ChildNodes[c].ChildNodes[k].Name, xmlElement.ChildNodes[k].FirstChild?.InnerText);
										}

									}
								}
								else
								{
									linesData.Add(myElement.ChildNodes[j].ChildNodes[c].ChildNodes[k].Name, xmlElement.ChildNodes[k].FirstChild?.InnerText);
								}
							}

							LineaDetalle oLineDetalleXML = new LineaDetalle();
							Type o_Type = oLineDetalleXML.GetType();
							Boolean isFound = false; // Solo mapea campos que estan en el modelo de LineaDetalle

							foreach (var item in linesData)
							{
								isFound = false;
								foreach (var property in oLineDetalleXML.GetType().GetProperties())
								{
									if (property.Name.Equals(item.Key))
									{
										isFound = true;
									}
								}  

								if (isFound)
								{															
									o_Type.GetProperty(item.Key).SetValue(oLineDetalleXML, item.Value, null);
								}

							}
							oLinesXml.Add(oLineDetalleXML);

						}
						
                        if (myElement.ChildNodes[j].Name.Equals("Emisor"))
                        {
							if (myElement.ChildNodes[j].ChildNodes[c].Name.Equals("Identificacion"))
							{
								for (int k = 0; k < myElement.ChildNodes[j].ChildNodes[c].ChildNodes.Count; k++)
								{
									if (myElement.ChildNodes[j].ChildNodes[c].ChildNodes[k].Name.Equals("Numero"))
									{
										pairedData.Add(myElement.ChildNodes[j].ChildNodes[c].ChildNodes[k].Name, xmlElement.ChildNodes[k].FirstChild?.InnerText);
									}						
								}
							}

						}
                       
					}

					Emisor oEmisor = new Emisor();
					Type o_Type2 = oEmisor.GetType();
					Boolean isFound2 = false; // Solo mapea campos que estan en el modelo de LineaDetalle
					foreach (var item in pairedData)
					{
						//oType.GetProperty(item.Key).SetValue(oEmisor, item.Value, null);
						isFound2 = false;
						foreach (var property in oEmisor.GetType().GetProperties())
						{
							if (property.Name.Equals(item.Key))
							{
								isFound2 = true;
							}
						}

						if (isFound2)
						{
							o_Type2.GetProperty(item.Key).SetValue(oEmisor, item.Value, null);
						}
					}

                    if (!string.IsNullOrEmpty(oEmisor.Numero))
                    {
						oEmisorXml = oEmisor;
					}
					


				}
				if (oLinesXml.Count > 0)
				{

					ApiResponse<EntryLineModel> infoitem = new ApiResponse<EntryLineModel>();
					ApiResponse<BusinessPartnerModel> infoBP = new ApiResponse<BusinessPartnerModel>();

					Companys company = GetData.GetCompanyByUserId(userId);

					CredentialHolder userCredentials = GetCurrentUserCredentials();

					

                    infoBP = CLVSPOS.SAPDAO.GetSapData.GetBusinessPartnerbyNameXml(userCredentials, GetNameObject("spGetBPByItemXml"), oEmisorXml.Numero);
					_goodsReceipt.BusinessPartner = infoBP.Data;
			

					foreach (var linexml in oLinesXml)
					{
						infoitem = CLVSPOS.SAPDAO.GetSapData.GetItembyItemNameXml(userCredentials, GetNameObject("spGetInfoByItemXml"), linexml.Detalle.Replace("'", "''"), GetNameObject("spGetTaxCodeByTaxRate"), string.IsNullOrEmpty(linexml.PorcentajeExoneracion) ? Convert.ToDouble(linexml.Tarifa) : Convert.ToDouble(linexml.PorcentajeExoneracion));  
						EntryLineModel line = new EntryLineModel
						{

							ItemNameXml = linexml.Detalle != null ? linexml.Detalle : string.Empty,
							ItemCode = infoitem.Data.ItemCode != null ? infoitem.Data.ItemCode : string.Empty,
							ItemName = infoitem.Data.ItemName != null ? infoitem.Data.ItemName : string.Empty,
							UnitPrice = Convert.ToDouble(linexml.PrecioUnitario),
							LineTotal = Convert.ToDouble(linexml.SubTotal),
							TaxRate = string.IsNullOrEmpty(linexml.PorcentajeExoneracion) ? Convert.ToDouble(linexml.Tarifa) : Convert.ToDouble(linexml.PorcentajeExoneracion),
							TaxCode = infoitem.Data.TaxCode != null ? infoitem.Data.TaxCode : string.Empty,
							Quantity = Convert.ToDouble(linexml.Cantidad),
							WareHouse = WhsCode,
							Discount = GetDiscoountPercentForXMLEntry(linexml.PrecioUnitario, linexml.MontoDescuento, linexml.Cantidad)
						};

						oLines.Add(line);
					}
					_goodsReceipt.Lines = oLines;

					return new GoodsReciptXmlResponse
					{
						Result = true,
						GoodsReceipt = _goodsReceipt
					};
				}
				else
				{
					throw new Exception("No se obtuvieron líneas del xml");

				}

			}
			catch
			{
				throw;
			}
		}


		private static double GetDiscoountPercentForXMLEntry(string _unitPrice, string _lineTotalDiscount, string _cantidad)
		{
			double discountPercent = 0;
			try
			{
				if (string.IsNullOrEmpty(_lineTotalDiscount))
					return discountPercent;

				if (string.IsNullOrEmpty(_unitPrice))
					return discountPercent;

				if (string.IsNullOrEmpty(_cantidad))
					return discountPercent;

				

				double total = Convert.ToDouble(_unitPrice);
				double discount = Convert.ToDouble(_lineTotalDiscount);
				double cantidad = Convert.ToDouble(_cantidad);

				return (double)((discount / (total * cantidad)) * 100);
			}
			catch (Exception e)
			{
				LogManager.LogMessage($"Error GetDiscoountPercentForXMLEntry causado por:{e.Message} montos recibidos, total = {_unitPrice} descuento = {_lineTotalDiscount}, cantidad = {_cantidad} ", (int)Constants.LogTypes.API);
				return discountPercent;
			}


		}


		private static string DecodeStringFromBase64(string base64EncodedText)
		{
			if (String.IsNullOrEmpty(base64EncodedText))
			{
				return base64EncodedText;
			}

			byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedText);
			return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
		}


		public static ItemsResponse CreateGoodsReceiptReturn(GoodsReceipt _goodsRecipt)
		{

			try
			{
				CredentialHolder UserCredentials = GetCurrentUserCredentials();





				return CLVSPOS.SAPDAO.PostSapData.CreateGoodsReceiptReturn(_goodsRecipt, UserCredentials, GetNameObject("spGetGoodReceipReturnDocNum"));
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Funcion para obtener todos los Clientes
		/// </summary>
		/// <returns></returns>
		public static CustomerResponseModel GetCustomer()
		{
			CustomerResponseModel compsResponse = new CustomerResponseModel();
			try
			{
				CredentialHolder usreCredentials = GetCurrentUserCredentials();
				return compsResponse = SAPDAO.GetSapData.GetCustomer(usreCredentials, GetNameObject("spGetBP"));
			}
			catch (Exception ex)
			{
				return (CustomerResponseModel)LogManager.HandleExceptionWithReturn(ex, "CustomerResponseModel");

			}
		}

		/// <summary>
		/// Actualizar Cliente
		/// </summary>
		/// <param name="Customer"></param>
		/// <returns></returns>
		public static BaseResponse UpdateCustomer(GetCustomerModel Customer)
		{
			try
			{
				CredentialHolder UserCredentials = GetCurrentUserCredentials();

				return PostDIAPIData.UpdateCustomer(Customer, UserCredentials);
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		///  Funcion para obtener Cliente
		/// </summary>
		/// <param name="CardCode"></param>
		/// <returns></returns>
		public static CustomerResponseModel GetCustomerbyCode(string CardCode)
		{
			CustomerResponseModel compsResponse = new CustomerResponseModel();

			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return compsResponse = SAPDAO.GetSapData.GetCustomerbyCode(userCredentials, CardCode, GetNameObject("spGetBPByCode"));
			}
			catch (Exception ex)
			{
				return (CustomerResponseModel)LogManager.HandleExceptionWithReturn(ex, "CustomerResponseModel");
			}
		}

		/// <summary>
		/// Crear nuevo Cliente
		/// </summary>
		/// <param name="Customer"></param>
		/// <returns></returns>
		public static BaseResponse CreateCustomer(GetCustomerModel Customer)
		{
			try
			{

				var userId = GetUserId();
				var company = GetData.GetCompanyByUserId(userId);
				CredentialHolder UserCredentials = GetCurrentUserCredentials();

				return PostDIAPIData.CreateCustomer(Customer, UserCredentials, userId);
			}
			catch
			{
				throw;
			}
		}
		/// <summary>
		/// Funcion para Crear Una Factura de Proveedor en Sap
		/// </summary>
		/// <param name="createapInvoice"></param>
		/// <returns></returns>
		public static InvoiceSapResponse CreateapInvoice(CreateInvoice createapInvoice)
		{
			try
			{
				var userId = GetUserId();
				var company = GetData.GetCompanyByUserId(userId);

				CredentialHolder userCredentials = GetCurrentUserCredentials();


				bool InvExist = SAPDAO.GetSapData.CheckUniqueInvIdSupplier(createapInvoice.Invoice.CLVS_POS_UniqueInvId, userCredentials, GetNameObject("spGetCheckUniqueInvIdSup"));
				if (InvExist)
				{
					throw new Exception(string.Format("La factura con UniqueInvId {0} ya está registrada en SAP.", createapInvoice.Invoice.CLVS_POS_UniqueInvId));
				}
				var response = SAPDAO.PostSapData.CreateapInvoice(createapInvoice, company, userId, GetNameObject("spGetFatherCard"), GetNameObject("spGetNumApInv"), GetCurrentUserCredentials());
				return response;
			}
			catch (Exception exc)
			{
				return (InvoiceSapResponse)LogManager.HandleExceptionWithReturn(exc, "InvoiceSapResponse");
			}
		}
		/// <summary>
		/// Obtener list de Facturas
		/// </summary>
		/// <param name="cardCode"></param>
		/// <param name="sede"></param>
		/// <param name="currency"></param>
		/// <returns></returns>
		public static InvoicesListResp GetPayApInvoices(string cardCode, string sede, string currency)
		{
			try
			{

				return CLVSPOS.SAPDAO.GetSapData.GetPayApInvoices(cardCode, sede, currency, GetCurrentUserCredentials(), GetNameObject("spGetPayDocumentsSupplier"));
			}
			catch (Exception exc)
			{
				return (InvoicesListResp)LogManager.HandleExceptionWithReturn(exc, "InvoicesListResp");
			}
		}
		/// <summary>
		/// Metodo para realizar un pago a proveedor en SAP
		/// Recibe como parametros el modelo de pago
		/// </summary>
		/// <param name="payment"></param>
		/// <returns></returns>
		public static async Task<PaymentSapResponse> CreatePayApInvoices(CreateSLRecivedPaymentModel payment)
		{
			try
			{
				var userId = GetUserId();

				CredentialHolder userCredentials = GetCurrentUserCredentials();

				DocInfo IsAlreadyCreated = SAPDAO.GetSapData.CheckUniqueDocumentID("OVPM", payment.U_CLVS_POS_UniqueInvId, userCredentials, GetNameObject("spCheckUniqueDocumentID"));

				if (IsAlreadyCreated != null)
				{
					return new PaymentSapResponse()
					{
						Result = true,
						DocEntry = IsAlreadyCreated.DocEntry,
						DocNum = IsAlreadyCreated.DocNum
					};
				}

				return await CLVSPOS.SAPDAO.PostSapData.CreatePayApInvoices(payment, userId, GetNameObject("spGetFatherCard"), userCredentials);
			}
			catch (Exception exc)
			{
				return (PaymentSapResponse)LogManager.HandleExceptionWithReturn(exc, "PaymentSapResponse");
			}
		}

		/// <summary>
		/// Metodo para generar entrada de mercaderia
		/// </summary>
		/// <param name="_goodsReceipt"></param>
		/// <returns></returns>
		public static ItemsResponse CreateGoodsReceiptStock(GoodsReceipt _goodsReceipt)
		{
			try
			{
				CredentialHolder UserCredentials = GetCurrentUserCredentials();

				return SAPDAO.PostSapData.CreateGoodsReceiptStock(_goodsReceipt, UserCredentials, GetNameObject("spGetDocNumByTable"));
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Trae la informacion de las listas de precios 
		/// no recibe parametros
		/// </summary>
		/// <returns></returns>
		public static PriceListResponse GetAllPriceList()
		{
			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return CLVSPOS.SAPDAO.GetSapData.GetAllPriceList(userCredentials, GetNameObject("spGetAllPriceList"));

			}
			catch (Exception exc)
			{
				return (PriceListResponse)LogManager.HandleExceptionWithReturn(exc, "PriceListResponse");
			}
		}

		public static ItemsResponse CreateGoodsIssueStock(GoodsReceipt goodsIssue)
		{
			try
			{
				CredentialHolder UserCredentials = GetCurrentUserCredentials();


				DocInfo IsAlreadyCreated = SAPDAO.GetSapData.CheckUniqueDocumentID("OIGE", goodsIssue.U_CLVS_POS_UniqueInvId, UserCredentials, GetNameObject("spCheckUniqueDocumentID"));

				if (IsAlreadyCreated != null)
				{
					return new ItemsResponse()
					{
						Result = true,
						DocEntry = IsAlreadyCreated.DocEntry,
						DocNum = IsAlreadyCreated.DocNum
					};
				}

				return CLVSPOS.SAPDAO.PostSapData.CreateGoodsIssueStock(goodsIssue, UserCredentials, GetNameObject("spGetDocNumByTable"));
			}
			catch
			{
				throw;
			}
		}

		public static ItemsResponse GetItemChangePrice(ItemsChangePriceModel itemModel)
		{
			try
			{
				var userId = GetUserId();
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				var userAssign = GetData.GetUserMappId(userId);
				return SAPDAO.GetSapData.GetItemChangePrice(itemModel, Convert.ToDecimal(userAssign.minDiscount), userCredentials, GetNameObject("spGetItemInfo"));

			}
			catch (Exception exc)
			{
				return (ItemsResponse)LogManager.HandleExceptionWithReturn(exc, "ItemsResponse");
			}
		}

		public static void ConnectCompany(int MappId)
		{
			try
			{
				UserAssign UserAssign = GetData.GetUserMapCustom(MappId);
				DIAPICommon.ConnectCompany(UserAssign);
			}
			catch (Exception exc)
			{
				LogManager.LogMessage(string.Format("                           Proccess>ConnectCompany error:{0}", exc.Message), (int)Constants.LogTypes.Auto);
			}
		}


		public static BaseResponse CreatePDFToSendMail(GetBalanceModel_UsrOrDate MailDataModel)
		{
			try
			{
				string userId = GetUserId();
				Companys company = GetData.GetCompanyByUserId(userId);
				PaydeskBalance paydeskBalance = GetData.GetPaydeskBalance(userId, MailDataModel.FIni.ToString("yyyy-MM-dd"));
				string b64 = BalanceReport.BalanceReport2(paydeskBalance, company.ReportBalance);
				SyncResponse mailDataLits = GetData.SyncGetMailData();
				var rowsToSync = mailDataLits.rowsToSync.Cast<MailDataModel>().ToList();

				return CreatePDFToSendMail(MailDataModel, company, b64, rowsToSync);
			}
			catch (Exception exc)
			{
				throw;
			}
		}
		public static BaseResponse CreatePDFToSendMail(GetBalanceModel_UsrOrDate _MailDataModel, Companys company, string b64, List<MailDataModel> emailConfig)//MailDataModel
		{

			List<string> mailsToList = new List<string>();

			string receptorMail = string.Empty;
			try
			{
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

				string pdfBase64 = b64;// CreatePDFToSendMail(Company, BpMailToSend, "");
				string mailSubject = string.Format("{0}", _MailDataModel.SendMailModel.subject);
				using (MailMessage message = new MailMessage())
				{
					using (SmtpClient SmtpServer = new SmtpClient(emailConfig[0].Host))
					{
						message.From = new MailAddress(emailConfig[0].from, "", System.Text.Encoding.UTF8);
						message.Sender = new MailAddress(_MailDataModel.SendMailModel.emailsend, "", System.Text.Encoding.UTF8);
						SmtpServer.Port = emailConfig[0].port;
						SmtpServer.Credentials = new System.Net.NetworkCredential(emailConfig[0].from, emailConfig[0].pass);
						SmtpServer.EnableSsl = emailConfig[0].SSL;

						message.To.Add(_MailDataModel.SendMailModel.emailsend);

						if (!string.IsNullOrEmpty(_MailDataModel.SendMailModel.EmailCC))
						{
							string[] receptorCCMails = _MailDataModel.SendMailModel.EmailCC.Split(';');
							foreach (string receptorCC in receptorCCMails)
							{
								message.CC.Add(receptorCC);
							}
						}



						message.Subject = mailSubject;
						message.IsBodyHtml = true;
						message.BodyEncoding = UTF8Encoding.UTF8;
						message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
						message.Body = _MailDataModel.SendMailModel.message;


						//Adjunta al correo el PDF en memoria
						byte[] stemp = Convert.FromBase64String(pdfBase64);
						using (Stream stream = new MemoryStream(stemp))
						{
							string pdfName = string.Empty;


							DateTime aDate = DateTime.Now;
							string Titulo = "Reporte Cierre de Caja";
							string IDate = string.Empty;
							IDate = _MailDataModel.FIni.ToString("dd MMM yyyy");
							string FDate = string.Empty;
							FDate = _MailDataModel.FFin.ToString("dd MMM yyyy");

							pdfName = string.Format("{0} {1}.pdf", Titulo, IDate);

							message.Attachments.Add(new System.Net.Mail.Attachment(stream, pdfName, "application/pdf"));
							LogManager.LogMessage("Enviando------: " + _MailDataModel.SendMailModel.emailsend, 1);//, (int)Constants.LogType.API);
							SmtpServer.Send(message);
						}

						LogManager.LogMessage("Correo enviado a la bandeja: " + _MailDataModel.SendMailModel.emailsend, 1);//, (int)Constants.LogType.Email);
																														   //GC.Collect();
						return new BaseResponse
						{
							Result = true,

						};
					}
				}

			}
			catch (Exception ex)
			{
				throw;
			}

		}

		public static ReportResponse GetBalanceReport(GetBalanceModel_UsrOrDate BalanceModel)
		{
			try
			{
				string file = BalanceReport.GetBalanceReport(BalanceModel);
				return new ReportResponse
				{
					File = file,
					Result = true
				};
			}
			catch (Exception exc)
			{
				throw;
			}
		}
		public static BaseResponse GetViewGroupList()
		{
			try
			{
				return CLVSPOS.DAO.GetData.GetViewGroupList();
			}
			catch (Exception exc)
			{
				return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
			}
		}

		public static BaseResponse UpdateViewLineAgrupation(ViewLinesAgrupationList ViewGroupList)
		{
			try
			{
				return CLVSPOS.DAO.PostData.UpdateViewLineAgrupation(ViewGroupList);
			}
			catch (Exception exc)
			{
				return (BaseResponse)LogManager.HandleExceptionWithReturn(exc, string.Empty);
			}
		}

		/// <summary>
		/// Metodo para crear un orden de compra
		/// </summary>
		/// <param name="_purchaseOrderModel"></param>
		/// <returns></returns>
		public static PurchaserOrderResponse CreatePurchaseOrder(PurchaseOrderModel _purchaseOrderModel)
		{
			try
			{
				CredentialHolder UserCredentials = GetCurrentUserCredentials();


				DocInfo IsDocumentAlreadyCreated = SAPDAO.GetSapData.CheckUniqueDocumentID("OPOR", _purchaseOrderModel.U_CLVS_POS_UniqueInvId, UserCredentials, GetNameObject("spCheckUniqueDocumentID"));



				if (IsDocumentAlreadyCreated != null)
				{
					return new PurchaserOrderResponse()
					{
						Result = true,
						Error = null,
						PurchaseOrder = new PurchaseOrderModel()
						{
							DocNum = IsDocumentAlreadyCreated.DocNum
						}

					};
				}


				return CLVSPOS.SAPDAO.PostSapData.CreatePurchaseOrder(_purchaseOrderModel, UserCredentials, GetNameObject("spGetDocNumByTable"));
			}
			catch (Exception exc)
			{
				throw;
			}
		}
		/// <summary>
		/// Obtiene lista de ordenes de compra
		/// </summary>
		/// <param name="_purchaseOrderModel"></param>
		/// <returns></returns>
		public static PurchaserOrdersResponse GetPurchaseOrderList(PurchaseOrderSearchModel _purchaseOrderModel)
		{
			try
			{
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return CLVSPOS.SAPDAO.GetSapData.GetPurchaseOrderList(userCredentials, _purchaseOrderModel, GetNameObject("spGETPURCHASEORDERLIST"));
			}
			catch (Exception exc)
			{
				return (PurchaserOrdersResponse)LogManager.HandleExceptionWithReturn(exc, "GetPurchaseOrderList");
			}
		}

		/// <summary>
		/// Metodo para editar un orden de compra
		/// </summary>
		/// <param name="_purchaseOrderModel"></param>
		/// <returns></returns>
		public static object UpdatePurchaseOrder(PurchaseOrderModel _purchaseOrderModel)
		{
			try
			{
				CredentialHolder UserCredentials = GetCurrentUserCredentials();
				return CLVSPOS.SAPDAO.PostSapData.UpdatePurchaseOrder(_purchaseOrderModel, UserCredentials);
			}
			catch (Exception exc)
			{
				throw;
			}
		}
		/// <summary>
		/// Metodo para obtener Item de orden de compra
		/// </summary>
		/// <param name="_DocNum"></param>
		/// <returns></returns>
		public static PurchaserOrderResponse GetPurchaseOrder(int _DocNum)
		{
			try
			{

				return CLVSPOS.SAPDAO.GetSapData.GetPurchaseOrder(GetCurrentUserCredentials(), _DocNum, GetNameObject("spGETPURCHASEORDERBYCODE"), GetNameObject("spGETBPPURCHASEORDER"));
			}
			catch (Exception exc)
			{
				return (PurchaserOrderResponse)LogManager.HandleExceptionWithReturn(exc, "CreatePurchaseOrder");
			}
		}
		// Get session claim
		public static string GetClaimsItem(string itemName)
		{
			var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
			return identity.Claims.Single(c => c.Type == itemName).Value;
		}

		#region Cotizacion
		/// <summary>
		/// Metodo para obtener cotizaciones de acuerdo a los criterios de busqueda
		/// </summary>
		/// <param name="quotationSearch"></param>
		/// <returns></returns>
		public static QuotationResponse GetQuotations(quotationSearch quotationSearch)
		{
			try
			{
				CredentialHolder UserCredentials = GetCurrentUserCredentials();


				return SAPDAO.GetSapData.GetQuotations(quotationSearch, GetNameObject("spGetQuotations"), UserCredentials);
			}
			catch (Exception exc)
			{
				return (QuotationResponse)LogManager.HandleExceptionWithReturn(exc, "QuotationResponse");
			}
		}

		/// <summary>
		/// Obtiene informacion de cotizacion para editar
		/// </summary>
		/// <param name="DocEntry"></param>
		/// <param name="_allLines"></param>
		/// <returns></returns>
		public static ApiResponse<IDocument> GetQuotationEdit(int DocEntry, bool _allLines)
		{
			try
			{

				CredentialHolder UserCredentials = GetCurrentUserCredentials();





				var userId = GetUserId();
				// var company = GetData.GetCompanyByUserId(userId);
				return SAPDAO.GetSapData.GetQuotationEdit(DocEntry, _allLines, GetNameObject("spGetQuotationEditHeader"), GetNameObject("spGetQuotationEditLines"), UserCredentials);
			}
			catch (Exception exc)
			{
				return (ApiResponse<IDocument>)LogManager.HandleExceptionWithReturn(exc, "ApiResponse<IDocument>");
			}
		}

		/// <summary>
		/// Metodo para actualizar una Oferta de Venta a SAP
		/// </summary>
		/// <param name="quotationEdit"></param>
		/// <returns></returns>
		public static async Task<UpdateQuotationsResponse> UpdateQuotation(IQuotDocument quotationEdit)
		{
			try
			{
				CredentialHolder UserCredentials = GetCurrentUserCredentials();


				var userId = GetUserId();

				//quotationEdit.RSDBId = GetData.GetRSDBIdByUserId(userId);
				return await CLVSPOS.SAPDAO.PostSapData.UpdateQuotation(quotationEdit, UserCredentials, GetNameObject("spGetDocNumOQUT"));
			}
			catch
			{
				throw;
			}
		}
		#endregion

		#region Orden de venta
		/// <summary>
		/// Metodo para obtener ordenes de venta
		/// </summary>
		/// <param name="saleOrderSearch"></param>
		/// <returns></returns>
		public static SaleOrderResponse GetSaleOrders(saleOrderSearch saleOrderSearch)
		{
			try
			{
				CredentialHolder UserCredentials = GetCurrentUserCredentials();






				return CLVSPOS.SAPDAO.GetSapData.GetSaleOrders(saleOrderSearch, UserCredentials, GetNameObject("spGetSaleOrders"));
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Metodo para obtener ordenes de venta
		/// </summary>
		/// <param name="DocEntry"></param>
		/// <returns></returns>
		public static ApiResponse<IDocument> GetSaleOrder(int DocEntry)
		{
			try
			{

				CredentialHolder UserCredentials = GetCurrentUserCredentials();

				return CLVSPOS.SAPDAO.GetSapData.GetSaleOrder(DocEntry, GetNameObject("spGetSaleOrderEditHeader"), GetNameObject("spGetSaleOrderEditLines"), UserCredentials);
			}
			catch (Exception exc)
			{
				return (ApiResponse<IDocument>)LogManager.HandleExceptionWithReturn(exc, "ApiResponse<IDocument>");
			}
		}

		public static async Task<DocumentUpdateResponse> UpdateSaleOrder(ISaleDocument saleOrder)
		{
			try
			{
				CredentialHolder UserCredentials = GetCurrentUserCredentials();



				var userId = GetUserId();

				return await CLVSPOS.SAPDAO.PostSapData.UpdateSaleOrder(saleOrder, UserCredentials, GetNameObject("spGetDocNumORDR"));
			}
			catch
			{
				throw;
			}
		}
		#endregion

		#region Reports
		public static ReportsResponse GetReports()
		{
			try
			{
				return GetData.GetReports(GetNameObject("viewGetReports"));
			}
			catch (Exception)
			{
				throw;
			}
		}

		public static FileResponse DownloadReportFile(int reportKey)
		{
			try
			{
				string filePath = GetData.GetReportPath(reportKey, GetNameObject("viewGetReportPath"));
				string base64File = DownloadFile(filePath);

				return new FileResponse { Result = true, File = base64File };
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public static string DownloadFile(string filePath)
		{
			byte[] contentBytes;

			using (FileStream fileStream = System.IO.File.OpenRead(filePath))
			{
				using (var memoryStream = new MemoryStream())
				{
					fileStream.CopyTo(memoryStream);
					contentBytes = memoryStream.ToArray();
					memoryStream.Close();
				}
				fileStream.Close();
			}
			string base64report = Convert.ToBase64String(contentBytes);
			contentBytes = null;

			return base64report;
		}
		#endregion

		#region Invoice
		public static InvoiceTypesResponse GetInvoiceTypes()
		{
			try
			{
				CredentialHolder UserCredentials = GetCurrentUserCredentials();


				return CLVSPOS.SAPDAO.GetSapData.GetInvoiceTypes(UserCredentials, GetNameObject("viewGetInvoiceType"));
			}
			catch (Exception exc)
			{
				return (InvoiceTypesResponse)LogManager.HandleExceptionWithReturn(exc, "InvoiceTypesResponse");
			}
		}

		/// <summary>
		/// Usado para renderizar los udfs de manera dinamica a nivel de ui
		/// </summary>
		/// <param name="_category"></param>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		public static UdfsResponse GetUdfs(string _category)
		{
			try
			{
				var userId = GetUserId();
				var company = GetData.GetCompanyByUserId(userId);
				CredentialHolder userCredentials = GetCurrentUserCredentials();
				return CLVSPOS.SAPDAO.GetSapData.GetUdfs(company, _category, GetNameObject("spGETUDFS"), userCredentials);
			}
			catch
			{
				throw;
			}
		}
		/// <summary>
		/// Devuelve todos los udfs que seran visibles para el usuario final
		/// </summary>
		/// <param name="_category"></param>
		/// <returns></returns>
		public static UdfsResponse GetConfiguredUdfs(string _category)
		{
			try
			{
				return CLVSPOS.DAO.GetData.GetConfiguredUdfs(_category);
			}
			catch
			{
				throw;
			}
		}
		/// <summary>
		/// Retorna los documentos que tienen udfs para configurar
		/// </summary>
		/// <returns></returns>
		public static UdfCategoriesResponse GetUdfCategories()
		{
			try
			{
				CredentialHolder UserCredentials = GetCurrentUserCredentials();
				return CLVSPOS.SAPDAO.GetSapData.GetUdfCategories(UserCredentials, GetNameObject("viewGetUdfCategories"));
			}
			catch
			{
				throw;
			}
		}

		public static UdfCategoriesResponse GetUdfDevelopment()
		{
			try
			{
				CredentialHolder UserCredentials = GetCurrentUserCredentials();
				return CLVSPOS.SAPDAO.GetSapData.GetUdfDevelopment(UserCredentials, GetNameObject("viewGetUdfDevelopment"));
			}
			catch
			{
				throw;
			}
		}

		public static BaseResponse SaveUdfs(List<Udf> _udfs, string _category)
		{
			try
			{
				return CLVSPOS.DAO.PostData.SaveUdf(_udfs, _category);
			}
			catch
			{
				throw;
			}
		}
		/// <summary>
		/// Retorna la data de la lista de udfs para ser procesados a nivel de ui
		/// </summary>
		/// <param name="_udfSource"></param>
		/// <returns></returns>
		public static UdfsTargetResponse GetUdfsData(UdfSource _udfSource)
		{
			try
			{
				// Para proteger inyeccion sql
				string[] charsToRemove = new string[] { "@", ",", ".", ";", "'", "-" };

				_udfSource.UdfsTarget.ForEach(x =>
				{
					foreach (var c in charsToRemove)
					{
						x.Name = x.Name.Replace(c, string.Empty);
					}
				});

				foreach (var c in charsToRemove)
				{
					_udfSource.TableId = _udfSource.TableId.Replace(c, string.Empty);
				}

				UdfCategory oUdfCategory = GetUdfCategories().UdfCategories.Where(x => x.Name.Equals(_udfSource.TableId)).FirstOrDefault();
				// Es requerido para encontrar el criterio de filtro del objeto en el query
				if (oUdfCategory == null) throw new Exception($"No existe la categoria solicitada {_udfSource.TableId}");
				_udfSource.Key = oUdfCategory.Key;

				CredentialHolder UserCredentials = GetCurrentUserCredentials();

				return CLVSPOS.SAPDAO.GetSapData.GetUdfsData(UserCredentials, _udfSource);

			}
			catch
			{
				throw;
			}
		}
		#endregion

		public static SyncResponse SyncGetConfiguredUdfs()
		{
			try
			{

				return CLVSPOS.DAO.GetData.SyncGetConfiguredUdfs();
			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "UdfsResponse");
			}
		}
		public static SyncResponse SyncGetInvoiceTypes()
		{
			try
			{
				var userId = GetUserId();
				var company = GetData.GetCompanyByUserId(userId);
				return CLVSPOS.SAPDAO.GetSapData.SyncGetInvoiceTypes(company, GetNameObject("viewGetInvoiceType"));
			}
			catch (Exception exc)
			{
				return (SyncResponse)LogManager.HandleExceptionWithReturn(exc, "InvoiceTypesResponse");
			}
		}

		#region Settings

		public static ApiResponse<List<Settings>> GetViewSettings()
		{
			try
			{
				return GetData.GetViewSettings();
			}
			catch
			{
				throw;
			}
		}

		public static BaseResponse SaveSettings(Settings settings)
		{
			try
			{
				return PostData.SaveSettings(settings);
			}
			catch
			{
				throw;
			}
		}
		public static ApiResponse<Settings> GetViewSettingbyId(int Code)
		{
			try
			{
				return GetData.GetViewSettingbyId(Code);
			}
			catch
			{
				throw;
			}
		}

		#endregion
		#region DBOBJECTNAMES
		public static string GetNameObject(string _dbObject)
		{
			try
			{
				return GetData.GetNameObject(_dbObject);
			}
			catch
			{
				throw;

			}
		}
		public static ApiResponse<List<DBObjectName>> GetDbObjectNames()
		{
			try
			{
				return GetData.GetDbObjectNames();
			}
			catch
			{
				throw;
			}
		}
		public static BaseResponse UpdateDbObjectNames(List<DBObjectName> DBObjectNameList)
		{
			try
			{
				return CLVSPOS.DAO.PostData.UpdateDbObjectNames(DBObjectNameList);
			}
			catch
			{
				throw;
			}
		}
		#endregion
		#region Paydesk balance

		public static FileResponse GetPaydeskBalance(string creationDate)
		{
			try
			{
				string userId = GetUserId();
				Companys company = GetData.GetCompanyByUserId(userId);
				PaydeskBalance paydeskBalance = GetData.GetPaydeskBalance(userId, creationDate);

				string report = BalanceReport.BalanceReport2(paydeskBalance, company.ReportBalance);

				return new FileResponse { File = report, Result = true };
			}
			catch (Exception)
			{
				throw;
			}
		}

		public static FileResponse PostPaydeskBalance(PaydeskBalance paydeskBalance)
		{
			try
			{
				string userId = GetUserId();
				Companys company = GetData.GetCompanyByUserId(userId);



				int userSignature = SAPDAO.PostSapData.SelectUserSignature(GetCurrentUserCredentials());


				List<double> cashFlowTotals = SAPDAO.GetSapData.SelectCashflowTotals(userSignature, GetCurrentUserCredentials(), GetNameObject("spGetCashflowTotal"));

				paydeskBalance.CashflowEgress = Convert.ToDouble(cashFlowTotals[0]);
				paydeskBalance.CashflowIncomme = Convert.ToDouble(cashFlowTotals[1]);
				paydeskBalance.UserSignature = userSignature;
				paydeskBalance.CreationDate = DateTime.Now;

				PostData.PostPaydeskBalance(paydeskBalance, userId);

				string report = BalanceReport.BalanceReport2(paydeskBalance, company.ReportBalance);

				return new FileResponse { File = report, Result = true };
			}
			catch (Exception)
			{
				throw;
			}
		}
		public static ApiResponse<List<CashflowReasonModel>> GetCashflowReasons()

		{
			try
			{


				return CLVSPOS.SAPDAO.GetSapData.GetCashflowReasons(GetCurrentUserCredentials(), GetNameObject("viewGetCashflowReason"));
			}
			catch (Exception)
			{
				throw;
			}
		}
		public static BaseResponse CreateCashflow(CashflowModel cashflow)
		{
			try
			{
				//string userId = GetUserId();
				//Companys company = GetData.GetCompanyByUserId(userId);

				CredentialHolder UserCredentials = GetCurrentUserCredentials();

				return CLVSPOS.SAPDAO.PostSapData.CreateCashflow(cashflow, UserCredentials);
			}
			catch (Exception)
			{
				throw;
			}
		}
		#endregion

	}
}
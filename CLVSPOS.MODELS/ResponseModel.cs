using CLVSSUPER.MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static CLVSPOS.MODELS.PayInvoiceModel;
namespace CLVSPOS.MODELS
{
    public class ResponseModel
    {
    }
    /// <summary>
    /// Modelo generico para devolver respuestas bases
    /// </summary>
    public class BaseResponse
    {
        public bool Result { get; set; }
        public ErrorInfo Error { get; set; }
    }

    public class ApiResponse <T> : BaseResponse
    {
        public T Data { get; set; }
        
    }


    public class PPBalanceResponse : BaseResponse
    {
        public PPBalance PPBalance { get; set; }
    }



    public class PPTerminalsResponse : BaseResponse
    {
        public List<PPTerminal> PPTerminals { get; set; }
    }
    public class PPTerminalsByUserResponse : BaseResponse
    {
        public List<PPTerminalByUser> PPTerminalsByUser { get; set; }
    }

    public class PPTerminalResponse : BaseResponse
    {
        public PPTerminal PPTerminal { get; set; }
    }
    public class SyncResponse
    {
        public bool result { get; set; }
        public ErrorInfo errorInfo { get; set; }
        public List<Object> rowsToSync { get; set; }
    }

    public class discountResponse : BaseResponse
    {
        public decimal discount { get; set; }
    }

    public class PPTransactionsResponse : BaseResponse
    {
        public List<PPTransaction> PPTransactions { get; set; }
    }



    /// <summary>
    /// Modelo de respuesta de un error generado por una excepcion
    /// </summary>
    public class ErrorInfo
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }


    /// <summary>
    /// Modelo de respuesta para una lista de BPs
    /// </summary>
    public class BPSResponseModel : BaseResponse
    {
        public List<BusinessPartnerModel> BPS { get; set; }
    }

    public class BPFEInfoResponseModel : BaseResponse
    {
        public InvoiceFEInfo FEInfo { get; set; }
    }

    public class BPInfoPadronResponseModel : BaseResponse
    {
        public string CardName { get; set; }
    }

    /// <summary>
    /// Modelo de respuesta para solo un item
    /// </summary>
    public class ItemsResponse : BaseResponse
    {
        public ItemsModel Item { get; set; }
        public List<ItemsModel> ItemList { get; set; }
        public int DocNum { get; set; }
        public int DocEntry { get; set; }
    }

    public class ItemsChangeResponse : BaseResponse
    {
        public List<ItemsChangePriceModel> Item { get; set; }

    }

    /// <summary>
    /// Modelo de respuesta para una lista de items
    /// </summary>
    public class ItemNamesResponse : BaseResponse
    {
        public ItemNamesModel ItemList { get; set; }
    }

    /// <summary>
    /// Modelo de dato de respuesta para obtener el tipo de cambio
    /// </summary>
    public class ExchangeRateResponse : BaseResponse
    {
        public double exRate { get; set; }
    }

    /// <summary>
    /// Modelo de respuesta para los impuestos
    /// </summary>
    public class TaxesResponse : BaseResponse
    {
        public List<TaxModel> Taxes { get; set; }
    }

    /// <summary>
    /// Modelo para manejar la respuesta de la creacion de la factura en SAP
    /// </summary>
    public class SalesOrderToSAPResponse : BaseResponse
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
    }
    public class SalesDocumentToSAPResponse : BaseResponse
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
    }

    /// <summary>
    /// Modelo para manejar la respuesta de la creacion de las cotizaciones en SAP
    /// </summary>
    public class QuotationsToSAPResponse : BaseResponse
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
    }

    /// <summary>
    /// Modelo de respuesta para las listas de permisos
    /// </summary>
    public class PermsModelResponse : BaseResponse
    {
        public List<PermissionsModel> perms { get; set; }
    }

    /// <summary>
    /// Modelo de respuesta para las listas de usuario
    /// </summary>
    public class UserModelResponse : BaseResponse
    {
        public List<UserModel> users { get; set; }

    }

    /// <summary>
    /// Respuesta al solicitar informacion del inventario
    /// </summary>
    public class WHInfoResponse : BaseResponse
    {
        public List<WHInfoModel> whInfo { get; set; }
    }

    /// <summary>
    /// Respuesta al solicitar informacion de las series por almacen
    /// </summary>
    public class SeriesResponse : BaseResponse
    {
        public List<SeriesModel> series { get; set; }
    }


    /// <summary>
    /// Respuesta al solicitar las companias en ld DB local
    /// </summary>
    public class CompanyListResponse : BaseResponse
    {
        public List<CompanysSapConnModel> companiesList { get; set; }
    }

    /// <summary>
    /// Respuesta al solicitar la compania por defecto de sap
    /// </summary>
    public class CompanySapResponse : BaseResponse
    {
        public CompanysSapConnModel Company { get; set; }
    }

    /// <summary>
    /// Respuesta al solicitar la informacion de una compania en la DB local
    /// </summary>
    public class CompanyResponse : BaseResponse
    {
        public CompanyAndMail companyAndMail { get; set; }
    }

    /// <summary>
    /// Modelo de respuesta para las listas de las companias
    /// </summary>
    public class companyListResponse : BaseResponse
    {
        public List<CompanysModel> company { get; set; }
    }

    /// <summary>
    /// Respuesta al solicitar las conexiones con SAP de la DB local
    /// </summary>
    public class SapConnectionResponse : BaseResponse
    {
        public List<SAPConnectionModel> SAPConnections { get; set; }
    }

    /// <summary>
    /// Respuesta para los parametros que se envian a las vistas
    /// </summary>
    public class ParamsViewResponse : BaseResponse
    {
        public List<ParamsModel> Params { get; set; }
    }

    /// <summary>
    /// Respuesta para la lista de usuarios para configuracion de usuarios
    /// </summary>
    public class UserListModel : BaseResponse
    {
        public List<UserAsingModel> Users { get; set; }
    }

    /// <summary>
    /// Modelo de respuesta para los almacenes
    /// </summary>
    public class StoreListModel : BaseResponse
    {
        public List<StoresModel> Stores { get; set; }
    }

    public class StoreModelResult : BaseResponse
    {
        public StoresModel Store { get; set; }
    }

    /// <summary>
    /// Modelo de respuesta para las listas de series de numeracion
    /// </summary>
    public class NumberingSeriesModelResponse : BaseResponse
    {
        public List<NumberingSeriesModel> Series { get; set; }
    }

    /// <summary>
    /// Respuesta con la lista de facturas
    /// </summary>
    public class InvoicesListResp : BaseResponse
    {
        public List<InvoicesListModel> InvoicesList { get; set; }
    }

    /// <summary>
    /// Respuesta con la lista de cuentas
    /// </summary>
    public class AccountResponse : BaseResponse
    {
        public List<AccountModel> accountList { get; set; }
    }

    /// <summary>
    /// Modelo de respuesta en caso de listar alguna enumeracion
    /// </summary>
    public class enumsResponse : BaseResponse
    {
        public List<EnumsList> Enums { get; set; }
    }

    /// <summary>
    /// Respuesta con la lista de tarjetas
    /// </summary>
    public class CardsResponse : BaseResponse
    {
        public List<CardsModel> cardsList { get; set; }
    }
    public class CardResponse : BaseResponse
    {
        public CardsModel Card { get; set; }
    }
    /// <summary>
    /// Respuesta con la lista de bancos
    /// </summary>
    public class BankResponse : BaseResponse
    {
        public List<BankModel> banksList { get; set; }
    }

    /// <summary>
    /// Modelo de respuesta para despues de transaccionar el pago
    /// </summary>
    public class PaymentSapResponse : BaseResponse
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
    }

    /// <summary>
    /// Respuesta con la lista de vendedores
    /// </summary>
    public class SalesManResponse : BaseResponse
    {
        public List<SalesManModel> salesManList { get; set; }
    }

    public class CancelpaymentResponce : BaseResponse
    {
        public List<CancelPaymentModel> paymentList { get; set; }
    }

    public class InvListPrintResponde : BaseResponse
    {
        public List<InvPrintModel> invList { set; get; }
    }

    /// <summary>
    /// Respuesta con el logo en base64 de la compania
    /// </summary>
    public class LogoCompanyResponse : BaseResponse
    {
        public string LogoB64 { get; set; }
    }

    public class InvoiceSapResponse : BaseResponse
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string NumDocFe { get; set; }
        public string Consecutivo { get; set; }
        public PaymentSapResponse PaymentResponse { get; set; }
        public DateTime StartTimeDocument { get; set; }
        public DateTime EndTimeDocument { get; set; }
        public string Document { get; set; }
        public string TypeDocument { get; set; }
        public string ElapsedTimeDocument { get; set; }

        //SL

        public DocumentModel invoice { get; set; }

    }

    public class PriceListResponse : BaseResponse
    {
        public List<PriceListModel> priceList { set; get; }
    }
    public class PriceListSelfResponse : BaseResponse
    {
        public PriceListModel PriceList { get; set; }
    }
    public class PayTermsResponse : BaseResponse
    {
        public List<PayTermsModel> payTermsList { set; get; }
    }

    /// <summary>
    /// Modelo de respuesta para la info de los almacenes para la vista de almacenes
    /// </summary>
    public class WHPlaceResponce : BaseResponse
    {
        public List<WHplaceModel> WHPlaceList { set; get; }
    }

    //modelo de respuesta para los balances por usuario. 
    public class BalanceByUserResponse : BaseResponse
    {
        public List<BalanceByUserDetails> UsrBalance { get; set; }
        public List<BalanceByUserDetailsCN> CreditNotes { get; set; }

    }

    public class CustomerResponseModel : BaseResponse
    {
        public List<GetCustomerModel> Customer { set; get; }
    }

    public class SendMailResponse : BaseResponse
    {
        public GetBalanceModel_UsrOrDate mail { get; set; }
        public List<GetBalanceModel_UsrOrDate> MailList { get; set; }
    }
    public class SendWhatsappResponse : BaseResponse
    {
    }
    public class ReportResponse : BaseResponse
    {
        public string File { get; set; }

    }
    /// <summary>
    /// Modelo de respuesta para una lista de items
    /// </summary>
    public class ViewLineAgrupationResponse : BaseResponse
    {
        public List<GroupLine> ViewGroupList { get; set; }
    }
    /// <summary>
    /// Model para retornar una orden de compra
    /// </summary>
    public class PurchaserOrderResponse : BaseResponse
    {
        public PurchaseOrderModel PurchaseOrder { get; set; }
    }
    /// <summary>
    /// Modelo para retorar la lista de ordenes de compra
    /// </summary>
    public class PurchaserOrdersResponse : BaseResponse
    {
        public List<PurchaseOrderModel> PurchaseOrders { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class LogModelResponse : BaseResponse
    {
        public LogModel LogModel { get; set; }
    }
    /// <summary>
    /// Model de respueta para el detalle de pago en facturacion
    /// </summary>
    public class InvoicePaymentDetailResponse : BaseResponse
    {
        public InvoicePaymentDetail InvoicePaymentDetail { get; set; }
    }

    public class UserAppResponse : BaseResponse
    {
        public UserModel User { get; set; }
    }
    public class UsersAppResponse : BaseResponse
    {
        public List<UserModel> Users { get; set; }
    }

    public class ReportsResponse : BaseResponse
    {
        public List<KeyValue> Reports { get; set; }
    }

    public class FileResponse : BaseResponse
    {
        public string File { get; set; }
    }


    public class ItemDetailResponse : BaseResponse
    {
        public ItemDetail Item { get; set; }
    }
    /// <summary>
    /// Model para retornar lineas documento xml
    /// </summary>
    public class GoodsReciptXmlResponse : BaseResponse
    {
        public GoodsReceipt GoodsReceipt { get; set; }
    }
  

    #region Report Region
    public class PPReportResponse : BaseResponse
    {
        public string SignedReport { get; set; }
        public string UnsignedReport { get; set; }
    }
    #endregion

    #region Quotation
    public class QuotationResponse : BaseResponse
    {
        public List<QuotationsModel> Quotations { get; set; }
    }
    public class QuotationEditResponse : BaseResponse
    {
        public QuotationsModel QuotationEdit { get; set; }
    }
    public class UpdateQuotationsResponse : BaseResponse
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
    }

    public class DocumentUpdateResponse : BaseResponse
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
    }
    #endregion

    #region Orden de venta
    public class SaleOrderResponse : BaseResponse
    {
        public List<SalesOrderModel> SaleOrders { get; set; }
    }
    public class GetSaleOrderResponse : BaseResponse
    {
        public SalesOrderModel SaleOrder { get; set; }
    }
    #endregion

    #region Invoice
    public class InvoiceTypesResponse : BaseResponse
    {
        public List<InvoiceType> InvoiceTypes { get; set; }
    }
    #endregion

    #region Udf
    public class UdfsResponse : BaseResponse
    {
        public List<Udf> Udfs { get; set; }
        public int FullSize { get; set; }
    }

    public class UdfCategoriesResponse: BaseResponse
    {
        public List<UdfCategory> UdfCategories;
    }

    public class UdfsTargetResponse : BaseResponse
    {
        public List<UdfTarget> UdfsTarget { get; set; }
    }
    #endregion



    #region SL error
    public class SLMessage
    {
        public string lang { get; set; }
        public string value { get; set; }
    }

    public class SLError
    {
        public int code { get; set; }
        public SLMessage message { get; set; }
    }

    public class SLResponse
    {
        public SLError error { get; set; }
    }



    #endregion


  
}

using CLVSPOS.COMMON;
using CLVSPOS.DAO;
using CLVSPOS.LOGGER;
using CLVSPOS.MODELS;
using CLVSSUPER.MODELS;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace CLVSPOS.DIAPI
{
    public class PostDIAPIData
    {
        /// <summary>
        /// Recibe como parametro un objeto de tipo factura y el objeto company para poder conectarse, userId para obtener configuraciones asignadas al usuario
        /// </summary>
        /// <param name="SaleOrder"></param>
        /// <param name="company"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public SalesOrderToSAPResponse CreateSaleOrder(SalesOrderModel SaleOrder, CredentialHolder _userCredentials, string userId)
        {
            LogManager.LogMessage("DIAPI/PostDiapiData/CreateSaleOrder-- Ingresando..." + " CardCode: " + SaleOrder.CardCode, (int)Constants.LogTypes.SAP);
            var SaleOrderToString = new JavaScriptSerializer().Serialize(SaleOrder);
            LogManager.LogMessage("DIAPI/PostDiapiData/CreateSaleOrder-- Objeto recibido SaleOrder: " + SaleOrderToString + ", company: " + _userCredentials.DBCode, (int)Constants.LogTypes.SAP);

            Company oCompany = null;
            oCompany = new Company();
            int DocEntry = 0;
            try
            {
                DateTime DocDate = DateTime.Today;
                oCompany = DIAPICommon.CreateCompanyObject(_userCredentials);
                Documents oOrd = null;

                oOrd = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oOrders);

                DAO.SuperV2_Entities db = new SuperV2_Entities();
                int SerieType = (int)COMMON.Constants.SerialType.SOrder;

                UserAssign userA = db.UserAssign.Where(x => x.UserId == userId).FirstOrDefault();

                DiapiSerie diapiSeries = userA.V_SeriesByUser.Where(x => x.V_Series.DocType == SerieType).Select(x => new DiapiSerie
                {
                    NumType = x.V_Series.Numbering,
                    Serie = x.V_Series.Serie
                }).FirstOrDefault();


                if (diapiSeries == null) throw new Exception("No se ha definido una serie para crear Orden de ventas");

                oOrd.Series = diapiSeries.Serie;
                oOrd.CardCode = SaleOrder.CardCode;
                oOrd.CardName = SaleOrder.CardName;
                oOrd.DocDate = DocDate;
                oOrd.DocDueDate = DocDate;
                oOrd.SalesPersonCode = SaleOrder.SalesPersonCode;
                oOrd.DocType = BoDocumentTypes.dDocument_Items;//es de articulos, consultar si se van a enviar de servicio
                oOrd.DocCurrency = SaleOrder.DocCurrency;
                oOrd.PaymentGroupCode = Convert.ToInt32(SaleOrder.PaymentGroupCode);
                oOrd.Comments = SaleOrder.Comments;

                if (!string.IsNullOrEmpty(SaleOrder.U_TipoIdentificacion) && (SaleOrder.U_TipoIdentificacion != "99" && SaleOrder.U_TipoIdentificacion != "00"))
                {
                    oOrd.UserFields.Fields.Item("U_TipoIdentificacion").Value = SaleOrder.U_TipoIdentificacion;
                }
                if (!string.IsNullOrEmpty(SaleOrder.U_NumIdentFE))
                {
                    oOrd.UserFields.Fields.Item("U_NumIdentFE").Value = SaleOrder.U_NumIdentFE;
                }
                if (!string.IsNullOrEmpty(SaleOrder.U_TipoDocE))
                {
                    oOrd.UserFields.Fields.Item("U_TipoDocE").Value = SaleOrder.U_TipoDocE;
                }
                if (!string.IsNullOrEmpty(SaleOrder.U_Online))
                {
                    oOrd.UserFields.Fields.Item("U_Online").Value = SaleOrder.U_Online;
                }
                if (!string.IsNullOrEmpty(SaleOrder.U_CorreoFE))
                {
                    oOrd.UserFields.Fields.Item("U_CorreoFE").Value = SaleOrder.U_CorreoFE;
                }
                oOrd.UserFields.Fields.Item("U_ListNum").Value = SaleOrder.U_ListNum;

                if (SaleOrder.UdfTarget != null)
                {
                    UserFields oUserFields = oOrd.UserFields;

                    SaleOrder.UdfTarget.ForEach(x =>
                    {
                        SetUdfValue(x.Name, x.FieldType, x.Value, ref oUserFields);
                    });
                }

                foreach (var line in SaleOrder.SaleOrderLinesList)
                {
                    oOrd.Lines.ItemCode = line.ItemCode;
                    oOrd.Lines.Quantity = line.Quantity;
                    oOrd.Lines.TaxPercentagePerRow = 0;
                    if (!line.TaxCode.Equals("EXE"))
                    {
                        oOrd.Lines.TaxCode = line.TaxCode;
                        oOrd.Lines.TaxPercentagePerRow = line.TaxRate;
                    }
                    oOrd.Lines.UnitPrice = line.UnitPrice;
                    oOrd.Lines.WarehouseCode = line.WarehouseCode;
                    oOrd.Lines.DiscountPercent = line.DiscountPercent;

                    //// OJO activar cuendo se cree el udf
                    ////oOrd.Lines.UserFields.Fields.Item("U_SugPrice").Value = Convert.ToDouble(line.U_SugPrice);
                    //if (!string.IsNullOrEmpty(line.Serie))
                    //{
                    //    oOrd.Lines.SerialNumbers.SetCurrentLine(count);
                    //    oOrd.Lines.SerialNumbers.InternalSerialNumber = line.Serie;
                    //    oOrd.Lines.SerialNumbers.SystemSerialNumber = line.SysNumber;
                    //    oOrd.Lines.SerialNumbers.Add();
                    //}

                    if (SaleOrder.BaseLines != null && SaleOrder.BaseLines.Count > 0)
                    {
                        int size = SaleOrder.BaseLines.Count;
                        for (int c = 0; c < size; c++)
                        {
                            if (SaleOrder.BaseLines[c].ItemCode == line.ItemCode)
                            {
                                oOrd.Lines.BaseType = 23;//(int)CLVSPOS.COMMON.Constants.BaseTypeLine.SALE_QUOTATION;
                                oOrd.Lines.BaseLine = SaleOrder.BaseLines[c].BaseLine.Value;
                                oOrd.Lines.BaseEntry = SaleOrder.BaseEntry.Value;
                                SaleOrder.BaseLines.RemoveAt(c);
                                break;
                            }
                        }
                    }
                    oOrd.Lines.Add();
                }
                if (oOrd.Add() != 0)
                {
                    string sapErrMessage = oCompany.GetLastErrorCode() + " - " + oCompany.GetLastErrorDescription();
                    string ModelObject = "Referencia CardCode #" + SaleOrder.CardCode;
                    LogManager.LogMessage("DIAPI/PostDiapiData/CreateSaleOrder-- Error al crear orden de venta, sapErrMessage: " + sapErrMessage + ", ModelObject" + ModelObject, (int)Constants.LogTypes.General);
                    throw new Exception(sapErrMessage + ModelObject);
                }
                DocEntry = Convert.ToInt32(oCompany.GetNewObjectKey());
                if (oCompany.Connected)
                {
                    oCompany.Disconnect();
                }
                return new SalesOrderToSAPResponse
                {
                    Result = true,
                    DocEntry = DocEntry,
                    Error = null
                };
            }
            catch (Exception ex)
            {
                if (oCompany != null)
                {
                    if (oCompany.Connected)
                    {
                        oCompany.Disconnect();
                    }
                } 
                int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
                string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;
                string ErrMsj = "-" + Convert.ToString("DIAPI/PostDiapiData/CreateSaleOrder Catch-- Code: " + code + " Message: " + message);
                LogManager.LogMessage(ErrMsj, (int)Constants.LogTypes.SAP);
                throw;
              
            }
        }

        public DocumentUpdateResponse UpdateSaleOrder(ISaleDocument saleOrder, CredentialHolder _UserCredentials)
        {
            Company oCompany = null;
            oCompany = new Company();
            string returnMsg = string.Empty;
            int lines = 0;
            // objecto documento de SAP
            Documents document = null;
            // objecto lineas del documento de SAP
            Document_Lines documentLines = null;
            // objeto udf para SO
            UserFields udfdocument = null;
            // objeto udf para las lineas de SO
            UserFields udfdocumentLines = null;
            try
            {
                DateTime DocDate = DateTime.Today;
                oCompany = DIAPICommon.CreateCompanyObject(_UserCredentials);
                document = (SAPbobsCOM.Documents)oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);

                if (!document.GetByKey(saleOrder.DocEntry)) throw new Exception("El número de la orden de venta es incorrecto");
                documentLines = document.Lines;
                udfdocument = document.UserFields;
                udfdocumentLines = documentLines.UserFields;

                lines = document.Lines.Count;
                document.DocType = BoDocumentTypes.dDocument_Items;
                document.DocCurrency = saleOrder.DocCurrency;
                document.PaymentGroupCode = Convert.ToInt32(saleOrder.PaymentGroupCode);
                document.DocDate = DocDate;
                document.DocDueDate = DocDate;
                document.Comments = saleOrder.Comments;
                document.SalesPersonCode = saleOrder.SalesPersonCode;
                if (!String.IsNullOrEmpty(saleOrder.NumAtCard) && !saleOrder.NumAtCard.Equals("0"))
                {
                    document.NumAtCard = saleOrder.NumAtCard;
                }

                for (int linea = 0; linea < lines; linea++)
                {
                    documentLines.Delete();
                }

                foreach (var line in saleOrder.DocumentLines)
                {
                    documentLines.ItemCode = line.ItemCode;
                    documentLines.Quantity = Convert.ToDouble(line.Quantity);
                    documentLines.UnitPrice = Convert.ToDouble(line.UnitPrice);
                    documentLines.DiscountPercent = Convert.ToDouble(line.DiscountPercent);
                    documentLines.TaxCode = line.TaxCode;
                    documentLines.WarehouseCode = line.WarehouseCode;

                    documentLines.Add();
                }
                if (!string.IsNullOrEmpty(saleOrder.U_TipoIdentificacion) && (saleOrder.U_TipoIdentificacion != "99" && saleOrder.U_TipoIdentificacion != "00"))
                {
                    udfdocument.Fields.Item("U_TipoIdentificacion").Value = saleOrder.U_TipoIdentificacion;
                }
                if (!string.IsNullOrEmpty(saleOrder.U_NumIdentFE))
                {
                    udfdocument.Fields.Item("U_NumIdentFE").Value = saleOrder.U_NumIdentFE;
                }
                if (!string.IsNullOrEmpty(saleOrder.U_CorreoFE))
                {
                    udfdocument.Fields.Item("U_CorreoFE").Value = saleOrder.U_CorreoFE;
                }
                if (!string.IsNullOrEmpty(saleOrder.U_ObservacionFE))
                {
                    udfdocument.Fields.Item("U_ObservacionFE").Value = saleOrder.U_ObservacionFE;
                }
                if (!string.IsNullOrEmpty(saleOrder.U_TipoDocE))
                {
                    udfdocument.Fields.Item("U_TipoDocE").Value = saleOrder.U_TipoDocE;
                }
                if (!string.IsNullOrEmpty(saleOrder.U_Online))
                {
                    udfdocument.Fields.Item("U_Online").Value = saleOrder.U_Online;
                }

                udfdocument.Fields.Item("U_ListNum").Value = saleOrder.U_ListNum;
                

                if (saleOrder.UdfTarget != null) // Caso poco problable que pase, pero por aquello
                {
                    saleOrder.UdfTarget.ForEach(x =>
                    {
                        SetUdfValue(x.Name, x.FieldType, x.Value, ref udfdocument);
                    });
                }


                //if (!string.IsNullOrEmpty(saleOrder.U_FacturaVencida))
                //{
                //    udfdocument.Fields.Item("U_FacturaVencida").Value = saleOrder.U_FacturaVencida;
                //}
                //if (!string.IsNullOrEmpty(saleOrder.U_NVT_Medio_Pago))
                //{
                //    udfdocument.Fields.Item("U_NVT_Medio_Pago").Value = saleOrder.U_NVT_Medio_Pago;
                //}
                //if (!string.IsNullOrEmpty(saleOrder.U_Facturacion))
                //{
                //    udfdocument.Fields.Item("U_Facturacion").Value = saleOrder.U_Facturacion;
                //}
                //if (udfdocument != null)
                //{
                //    System.Runtime.InteropServices.Marshal.ReleaseComObject(udfdocument);
                //}
                if (document.Update() != 0) throw new Exception(oCompany.GetLastErrorCode() + oCompany.GetLastErrorDescription());
                //if (udfdocumentLines != null)
                //{
                //    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(udfdocumentLines);
                //}
                //if (documentLines != null)
                //{
                //    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(documentLines);
                //}
                //if (udfdocument != null)
                //{
                //    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(udfdocument);
                //}
                //if (document != null)
                //{
                //    System.Runtime.InteropServices.Marshal.ReleaseComObject(document);
                //}
                //if (document != null)
                //{
                //    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(document);
                //}
                return new DocumentUpdateResponse
                {
                    Result = true,
                    DocEntry = saleOrder.DocEntry,
                    DocNum = document.DocNum
                };
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (udfdocumentLines != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(udfdocumentLines);
                }
                if (documentLines != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(documentLines);
                }
                if (udfdocument != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(udfdocument);
                }
                if (udfdocumentLines != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(udfdocumentLines);
                }
                if (documentLines != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(documentLines);
                }
                if (udfdocument != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(udfdocument);
                }
                if (document != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(document);
                }
                if (document != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(document);
                }

                if (oCompany != null && oCompany.Connected)
                {
                    oCompany.Disconnect();
                    oCompany = null;
                }
            }
        }

        //Crea la cotizacion en el objeto de SAP
        //recive como parametro un Objeto de tipo cotizacion y el objeto company para poder conectarce.
        public QuotationsToSAPResponse CreateQuotations(QuotationsModel Quotations, CredentialHolder _userCredentials, string userId)
        {
            LogManager.LogMessage("DIAPI/PostDiapiData/CreateQuotations-- Ingresando..." + " CardCode: " + Quotations.CardCode, (int)Constants.LogTypes.SAP);
            var QuotationsToString = new JavaScriptSerializer().Serialize(Quotations);
            LogManager.LogMessage("DIAPI/PostDiapiData/CreateQuotations-- Objeto recibido Quotations: " + QuotationsToString + ", company: " + _userCredentials.DBCode, (int)Constants.LogTypes.SAP);

            Company oCompany = null;
            oCompany = new Company();
            int DocEntry = 0;
            try
            {
                DAO.SuperV2_Entities db = new SuperV2_Entities();
                DateTime DocDate = DateTime.Today;
                oCompany = DIAPICommon.CreateCompanyObject(_userCredentials);
                Documents oQut = null;

                oQut = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oQuotations);

                int SerieType = (int)COMMON.Constants.SerialType.Quote;
                UserAssign userA = db.UserAssign.Where(x => x.UserId == userId).FirstOrDefault();
                DiapiSerie diapiSeries = userA.V_SeriesByUser.Where(x => x.V_Series.DocType == SerieType).Select(x => new DiapiSerie
                {
                    NumType = x.V_Series.Numbering,
                    Serie = x.V_Series.Serie
                }).FirstOrDefault();

                if (diapiSeries == null) throw new Exception("No se ha definido una serie para crear Ofertas de ventas");

                oQut.Series = diapiSeries.Serie;
                oQut.CardCode = Quotations.CardCode;
                if (!string.IsNullOrEmpty(Quotations.CardName)) oQut.CardName = Quotations.CardName;
                oQut.DocDate = DocDate;
                oQut.DocType = BoDocumentTypes.dDocument_Items;//es de articulos, consultar si se van a enviar de servicio
                oQut.DocCurrency = Quotations.DocCurrency;
                oQut.PaymentGroupCode = Convert.ToInt32(Quotations.PaymentGroupCode);
                oQut.Comments = Quotations.Comments;
                oQut.SalesPersonCode = Quotations.SalesPersonCode;

                if (!String.IsNullOrEmpty(Quotations.U_TipoIdentificacion) && (Quotations.U_TipoIdentificacion != "99" && Quotations.U_TipoIdentificacion != "00"))
                {
                    oQut.UserFields.Fields.Item("U_TipoIdentificacion").Value = Quotations.U_TipoIdentificacion;
                }
                if (!string.IsNullOrEmpty(Quotations.U_NumIdentFE))
                {
                    oQut.UserFields.Fields.Item("U_NumIdentFE").Value = Quotations.U_NumIdentFE;
                }
                if (!string.IsNullOrEmpty(Quotations.U_CorreoFE))
                {
                    oQut.UserFields.Fields.Item("U_CorreoFE").Value = Quotations.U_CorreoFE;
                }
                if (!string.IsNullOrEmpty(Quotations.U_TipoDocE))
                {
                    oQut.UserFields.Fields.Item("U_TipoDocE").Value = Quotations.U_TipoDocE;
                }
                if (!string.IsNullOrEmpty(Quotations.U_Online))
                {
                    oQut.UserFields.Fields.Item("U_Online").Value = Quotations.U_Online;
                }

                if (Quotations.UdfTarget != null) // Caso poco problable que pase, pero por aquello
                {
                    UserFields oUserFields = oQut.UserFields;
                    Quotations.UdfTarget.ForEach(x =>
                    {
                        SetUdfValue(x.Name, x.FieldType, x.Value, ref oUserFields);
                    });
                }

                foreach (var line in Quotations.QuotationsLinesList)
                {
                    oQut.Lines.ItemCode = line.ItemCode;
                    oQut.Lines.Quantity = line.Quantity;
                    oQut.Lines.TaxCode = line.TaxCode;
                    oQut.Lines.UnitPrice = line.UnitPrice;
                    oQut.Lines.DiscountPercent = line.DiscountPercent;
                    oQut.Lines.WarehouseCode = line.WarehouseCode;
                    oQut.Lines.Add();
                }

                if (oQut.Add() != 0)
                {
                    string sapErrMessage = oCompany.GetLastErrorCode() + " - " + oCompany.GetLastErrorDescription();
                    string ModelObject = "Referencia CardCode #" + Quotations.CardCode;
                    LogManager.LogMessage("DIAPI/PostDiapiData/CreateQuotations-- Error al actualizar Quotations, sapErrMessage: " + sapErrMessage + ", ModelObject" + ModelObject, (int)Constants.LogTypes.General);
                    throw new Exception(sapErrMessage + ModelObject);
                }
                DocEntry = Convert.ToInt32(oCompany.GetNewObjectKey());
                if (oCompany.Connected)
                {
                    oCompany.Disconnect();
                }

                return new QuotationsToSAPResponse
                {
                    Result = true,
                    DocEntry = DocEntry,
                    Error = null
                };
            }
            catch (Exception ex)
            {
                if (oCompany != null)
                {
                    if (oCompany.Connected)
                    {
                        oCompany.Disconnect();
                    }
                }
                int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
                string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;
                string ErrMsj = "-" + Convert.ToString("DIAPI/PostDiapiData/CreateQuotations Catch-- Code: " + code + " Message: " + message);
                LogManager.LogMessage(ErrMsj, (int)Constants.LogTypes.SAP);

                throw;
            }
        }

        public UpdateQuotationsResponse UpdateQuotation(IQuotDocument docModel, CredentialHolder _UserCredentials)
        {
            string returnMsg = string.Empty;
            int lines = 0;
            Company oCompany = null;
            oCompany = new Company();
            Documents oQuot = null;
            Document_Lines oQuotLines = null;
            try
            {
                oCompany = DIAPICommon.CreateCompanyObject(_UserCredentials);
                DateTime DocDate = DateTime.Today;
                oQuot = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oQuotations);
                oQuotLines = oQuot.Lines;

                if (!oQuot.GetByKey(docModel.DocEntry))
                {
                    throw new Exception("El número de proforma es incorrecto");
                }

                lines = oQuot.Lines.Count;

                oQuot.DocType = BoDocumentTypes.dDocument_Items;
                oQuot.DocDate = DocDate;
                oQuot.DocDueDate = DocDate;
                oQuot.PaymentGroupCode = Convert.ToInt32(docModel.PaymentGroupCode);
                oQuot.Comments = docModel.Comments;
                oQuot.SalesPersonCode = docModel.SalesPersonCode;
                oQuot.DocCurrency = docModel.DocCurrency;
                oQuot.PaymentGroupCode = Convert.ToInt32(docModel.PaymentGroupCode);


                if (!String.IsNullOrEmpty(docModel.U_TipoIdentificacion) && (docModel.U_TipoIdentificacion != "99" && docModel.U_TipoIdentificacion != "00"))
                {
                    oQuot.UserFields.Fields.Item("U_TipoIdentificacion").Value = docModel.U_TipoIdentificacion;
                }
                if (!string.IsNullOrEmpty(docModel.U_NumIdentFE))
                {
                    oQuot.UserFields.Fields.Item("U_NumIdentFE").Value = docModel.U_NumIdentFE;
                }
                if (!string.IsNullOrEmpty(docModel.U_CorreoFE))
                {
                    oQuot.UserFields.Fields.Item("U_CorreoFE").Value = docModel.U_CorreoFE;
                }
                if (!string.IsNullOrEmpty(docModel.U_TipoDocE))
                {
                    oQuot.UserFields.Fields.Item("U_TipoDocE").Value = docModel.U_TipoDocE;
                }
                if (!string.IsNullOrEmpty(docModel.U_Online))
                {
                    oQuot.UserFields.Fields.Item("U_Online").Value = docModel.U_Online;
                }

                oQuot.UserFields.Fields.Item("U_ListNum").Value = docModel.U_ListNum;

                if (docModel.UdfTarget != null) // Caso poco problable que pase, pero por aquello
                {
                    UserFields oUserFields = oQuot.UserFields;

                    docModel.UdfTarget.ForEach(x =>
                    {
                        SetUdfValue(x.Name, x.FieldType, x.Value, ref oUserFields);
                    });
                }

                for (var linea = 0; linea < lines; linea++)
                {
                    oQuot.Lines.Delete();
                }
                foreach (var line in docModel.DocumentLines)
                {
                    oQuot.Lines.ItemCode = line.ItemCode;
                    oQuot.Lines.Quantity = Convert.ToDouble(line.Quantity);
                    oQuot.Lines.UnitPrice = Convert.ToDouble(line.UnitPrice);
                    oQuot.Lines.DiscountPercent = Convert.ToDouble(line.DiscountPercent);
                    oQuot.Lines.TaxCode = line.TaxCode;
                    oQuot.Lines.WarehouseCode = line.WarehouseCode;
                    oQuot.Lines.Add();
                }
                if (oQuotLines != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oQuotLines);
                }
                if (oQuot.Update() != 0)
                {
                    throw new Exception(oCompany.GetLastErrorCode() + oCompany.GetLastErrorDescription());
                }
                if (oQuotLines != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oQuotLines);
                }
                if (oQuot != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oQuot);
                }
                if (oQuot != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oQuot);
                }

                int DocEntry = Convert.ToInt32(oCompany.GetNewObjectKey());

                if (oCompany.Connected)
                {
                    oCompany.Disconnect();
                    oCompany = null;
                }
                return new UpdateQuotationsResponse
                {
                    Result = true,
                    Error = null,
                    DocEntry = DocEntry,
                    DocNum = 0
                };
            }
            catch (Exception ex)
            {
                if (oQuotLines != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oQuotLines);
                }
                if (oQuotLines != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oQuotLines);
                }
                if (oQuot != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oQuot);
                }
                if (oQuot != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oQuot);
                }

                if (oCompany != null)
                {
                    if (oCompany.Connected)
                    {
                        oCompany.Disconnect();
                    }
                }
                int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
                string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;
                string ErrMsj = "-" + Convert.ToString("DIAPI/PostDiapiData/UpdateQuotation Catch-- Code: " + code + " Message: " + message);
                LogManager.LogMessage(ErrMsj, (int)Constants.LogTypes.SAP);
                throw;
            }
        }
        //Crea se encarga de crear el pago de la facture an sap
        //recive como parametro el docentry de la factura de SAP y el objeto company para poder conectarce, el modelo del pago el Id de usuario y en el caso de necesitar el Father Card
        // en caso de clientes que cuenten con una cuenta padre.
        public PaymentSapResponse CreatePayment(CreatePaymentModel payment, string DBName, string user, string FatherCard, ref Company oCompany)
        {

            var startTime = DateTime.Now;
            LogManager.LogMessage(string.Format("                           PostDIAPIData>CreatePayment. Start Time: {0}", startTime), 2);

            var paymentToString = new JavaScriptSerializer().Serialize(payment);
            LogManager.LogMessage("DIAPI/PostDiapiData/CreatePayment-- Objeto recibido payment: " + paymentToString + ", company: " + DBName + ", user: " + user + ", FatherCard: " + FatherCard, (int)Constants.LogTypes.SAP);

            int DocEntry = 0;
            int DocNum = 0;
            double ccSum = 0;
            double checkSum = 0;
            try
            {
                DAO.SuperV2_Entities db = new SuperV2_Entities();
                DateTime DocDate = DateTime.Today;
                SAPbobsCOM.Payments oPay = null;

                oPay = (SAPbobsCOM.Payments)oCompany.GetBusinessObject(BoObjectTypes.oIncomingPayments);

                int SerieType = (int)COMMON.Constants.SerialType.Payment;

                UserAssign userA = db.UserAssign.Where(x => x.UserId == user).FirstOrDefault();

                DiapiSerie diapiSeries = userA.V_SeriesByUser.Where(x => x.V_Series.DocType == SerieType).Select(x => new DiapiSerie
                {
                    NumType = x.V_Series.Numbering,
                    Serie = x.V_Series.Serie
                }).FirstOrDefault();

                if (diapiSeries == null)
                {
                    throw new Exception(string.Format("No se ha definido una serie para crear pagos"));
                }

                oPay.Series = diapiSeries.Serie;

                if (oPay.Invoices != null && oPay.Invoices.Count > 0)
                {
                    oPay.DocType = BoRcptTypes.rCustomer;
                }
                else
                {
                    oPay.DocType = BoRcptTypes.rAccount;
                }

                oPay.DocCurrency = payment.DocCurrency;
                oPay.CardCode = !string.IsNullOrEmpty(FatherCard) ? FatherCard : payment.CardCode;
                oPay.DocDate = DateTime.Now;
                oPay.DueDate = DateTime.Now;
                oPay.CounterReference = payment.CounterReference;
                oPay.Remarks = payment.Remarks;


                if (payment.CashSum != 0)
                {
                    decimal change = 0;

                    if (payment.V_PaymentLines.Count > 0)
                    {
                        change = payment.V_PaymentLines[0].Change <= 0 ? 0 : payment.V_PaymentLines[0].Change;
                    }

                    oPay.CashSum = Convert.ToDouble(payment.CashSum) - Convert.ToDouble(change);
                    oPay.CashAccount = payment.CashAccount;
                }

                if (payment.V_Checks != null && payment.V_Checks.Count > 0)
                {
                    foreach (var checks in payment.V_Checks)
                    {
                        oPay.CheckAccount = checks.CheckAccount;
                        oPay.Checks.AccounttNum = checks.AcctNum;
                        oPay.Checks.BankCode = checks.BankCode;
                        oPay.Checks.CountryCode = checks.CountryCode;
                        oPay.Checks.CheckNumber = Convert.ToInt32(checks.CheckNumber);
                        oPay.Checks.CheckSum = Convert.ToDouble(checks.CheckSum);
                        oPay.Checks.DueDate = Convert.ToDateTime(checks.DueDate);
                        oPay.DocCurrency = checks.Curr;
                        oPay.Checks.Add();

                        checkSum += Convert.ToDouble(checks.CheckSum);
                    }

                }

                if (payment.V_CreditCards != null && payment.V_CreditCards.Count > 0)
                {
                    foreach (var cc in payment.V_CreditCards)
                    {
                        oPay.CreditCards.CreditSum = Convert.ToDouble(cc.CreditSum);
                        string[] cardnumarray = cc.FormatCode.Split(' ');
                        oPay.CreditCards.CreditCard = Convert.ToInt32(cardnumarray[0]);
                        oPay.CreditCards.CreditCardNumber = Convert.ToString(cc.CreditCardNumber);

                        string fecha = cc.CardValid;
                        string[] array = fecha.Split('/');
                        var lastDayOfMonth = DateTime.DaysInMonth(Convert.ToInt32("20" + array[1]), Convert.ToInt32(array[0]));
                        var day = Convert.ToInt32(lastDayOfMonth);
                        DateTime nuevafecha = new DateTime(Convert.ToInt32("20" + array[1]), Convert.ToInt32(array[0]), day);

                        oPay.CreditCards.CardValidUntil = Convert.ToDateTime(nuevafecha);
                        oPay.CreditCards.OwnerIdNum = cc.OwnerIdNum;
                        oPay.CreditCards.VoucherNum = cc.VoucherNum;

                        UserFields oUserFields = oPay.CreditCards.UserFields;
                       // SetUdfValue("U_ManualEntry", "String", (cc.IsManualEntry ? 1 : 0).ToString(), ref oUserFields);
                        oPay.CreditCards.Add();
                        ccSum += Convert.ToDouble(cc.CreditSum);

                    }
                }

                if (payment.TransferSum > 0)
                {
                    oPay.TransferSum = Convert.ToDouble(payment.TransferSum);
                    oPay.TransferAccount = Convert.ToString(payment.TransferAccount);
                    oPay.TransferDate = Convert.ToDateTime(payment.TransferDate);
                    oPay.TransferReference = Convert.ToString(payment.TransferReference);
                }


                //if (!payment.accountPayment)
                //{
                //    foreach (var lines in payment.V_PaymentLines)
                //    {
                //        oPay.Invoices.DocEntry = lines.DocEntry;
                //        double totalPay = (Convert.ToDouble(payment.CashSum) + Convert.ToDouble(ccSum) + Convert.ToDouble(checkSum) + Convert.ToDouble(payment.TransferSum)) - Convert.ToDouble((lines.Change <= 0 ? 0 : lines.Change));

                //        //if (lines.InvoiceType == (int)COMMON.Constants.DocumentTypes.AnticipatedPayment) { oPay.Invoices.InvoiceType = SAPbobsCOM.BoRcptInvTypes.it_DownPayment; }


                //        oPay.Invoices.AppliedFC = totalPay;
                //        oPay.Invoices.SumApplied = lines.SumApplied;                      

                //      //  oPay.UserFields.Fields.Item("U_MontoRecibido").Value = lines.U_MontoRecibido.ToString();

                //        oPay.Invoices.Add();
                //    }
                //}

                if (oPay.Add() != 0)
                {
                    string sapErrMessage = oCompany.GetLastErrorCode() + " - " + oCompany.GetLastErrorDescription();
                    string ModelObject = "Referencia CardCode #" + payment.CardCode;
                    LogManager.LogMessage("DIAPI/PostDiapiData/CreatePayment-- Error al actualizar Pago, sapErrMessage: " + sapErrMessage + ", ModelObject" + ModelObject, (int)Constants.LogTypes.General);
                    throw new Exception("No se pudo realizar el pago, razon: " + oCompany.GetLastErrorCode() + " " + oCompany.GetLastErrorDescription());
                }

                DocEntry = Convert.ToInt32(oCompany.GetNewObjectKey());

                LogManager.LogMessage(string.Format("                           PostDIAPIData>CreatePayment. End Time: {0}| Total Time Time: {1}", DateTime.Now, DateTime.Now - startTime), (int)Constants.LogTypes.API);

                return new PaymentSapResponse
                {
                    Result = true,
                    DocEntry = DocEntry,
                    DocNum = DocNum,
                    Error = null
                };
            }
            catch (Exception ex)
            {
                int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
                string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;
                string ErrMsj = "-" + Convert.ToString("DIAPI/PostDiapiData/CreatePayment Catch-- Code: " + code + " Message: " + message);
                LogManager.LogMessage(ErrMsj, (int)Constants.LogTypes.SAP);

                string sapErrMessage = oCompany.GetLastErrorCode() + oCompany.GetLastErrorDescription();
                
                throw;                
            }
            finally
            {
                if (oCompany != null)
                {
                    if (oCompany.Connected)
                    {
                        oCompany.Disconnect();
                    }
                }
            }
        }

        //Crea se encarga de crear el pago de la facture an sap
        //recive como parametro el docentry de la factura de SAP y el objeto company para poder conectarce, el modelo del pago el Id de usuario y en el caso de necesitar el Father Card
        // en caso de clientes que cuenten con una cuenta padre.
        public PaymentSapResponse CreatePaymentRecived(CreateRecivedPaymentModel payment, string DBName, string user, string FatherCard, ref Company oCompany)
        {
            var startTime = DateTime.Now;
            LogManager.LogMessage(string.Format("                           PostDIAPIData>CreatePaymentRecived. Start Time: {0}", startTime), 2);

            var paymentToString = new JavaScriptSerializer().Serialize(payment);
            LogManager.LogMessage("DIAPI/PostDiapiData/CreatePaymentRecived-- Objeto recibido payment: " + paymentToString + ", company: " + DBName + ", user: " + user + ", FatherCard: " + FatherCard, (int)Constants.LogTypes.SAP);

            int DocEntry = 0;
            int DocNum = 0;
            double ccSum = 0;
            double checkSum = 0;
            double recivedAmount = 0;
            try
            {
                DAO.SuperV2_Entities db = new SuperV2_Entities();
                DateTime DocDate = DateTime.Today;
                SAPbobsCOM.Payments oPay = null;

                oPay = (SAPbobsCOM.Payments)oCompany.GetBusinessObject(BoObjectTypes.oIncomingPayments);

                int SerieType = (int)COMMON.Constants.SerialType.Payment;

                UserAssign userA = db.UserAssign.Where(x => x.UserId == user).FirstOrDefault();

                DiapiSerie diapiSeries = userA.V_SeriesByUser.Where(x => x.V_Series.DocType == SerieType).Select(x => new DiapiSerie
                {
                    NumType = x.V_Series.Numbering,
                    Serie = x.V_Series.Serie
                }).FirstOrDefault();

                oPay.Series = diapiSeries.Serie;
                oPay.DocType = BoRcptTypes.rCustomer;

                oPay.DocCurrency = payment.DocCurrency;
                oPay.CardCode = !string.IsNullOrEmpty(FatherCard) ? FatherCard : payment.CardCode;
                oPay.DocDate = DateTime.Now;
                oPay.DueDate = DateTime.Now;
                oPay.CounterReference = payment.CounterReference;
                oPay.Remarks = payment.Remarks;

                decimal change = 0;

                if (payment.CashSum != 0)
                {

                    if (payment.V_PaymentLines.Count > 0)
                    {
                        change = payment.V_PaymentLines[0].Change <= 0 ? 0 : payment.V_PaymentLines[0].Change;
                    }
                    else
                    {
                       // change = Convert.ToDecimal( payment.CashSum )- Convert.ToDecimal( payment.Total);
                    }
                    recivedAmount += payment.CashSum;
                    oPay.CashSum = Convert.ToDouble(payment.CashSum) - Convert.ToDouble(change);
                    oPay.CashAccount = payment.CashAccount;
                }

                if (payment.V_Checks != null && payment.V_Checks.Count > 0)
                {
                    foreach (var checks in payment.V_Checks)
                    {
                        oPay.CheckAccount = checks.CheckAccount;
                        oPay.Checks.AccounttNum = checks.AcctNum;
                        oPay.Checks.BankCode = checks.BankCode;
                        oPay.Checks.CountryCode = checks.CountryCode;
                        oPay.Checks.CheckNumber = Convert.ToInt32(checks.CheckNumber);
                        oPay.Checks.CheckSum = Convert.ToDouble(checks.CheckSum);
                        recivedAmount += Convert.ToDouble(checks.CheckSum);
                        oPay.Checks.DueDate = Convert.ToDateTime(checks.DueDate);
                        oPay.DocCurrency = checks.Curr;
                        oPay.Checks.Add();

                        checkSum += Convert.ToDouble(checks.CheckSum);
                    }
                }

                if (payment.V_CreditCards != null && payment.V_CreditCards.Count > 0)
                {
                    foreach (var cc in payment.V_CreditCards)
                    {
                        oPay.CreditCards.CreditSum = Convert.ToDouble(cc.CreditSum);
                        string[] cardnumarray = cc.FormatCode.Split(' ');
                        oPay.CreditCards.CreditCard = Convert.ToInt32(cardnumarray[0]);
                        oPay.CreditCards.CreditCardNumber = Convert.ToString(cc.CreditCardNumber);

                        string fecha = cc.CardValid;
                        string[] array = fecha.Split('/');
                        var lastDayOfMonth = DateTime.DaysInMonth(Convert.ToInt32("20" + array[1]), Convert.ToInt32(array[0]));
                        var day = Convert.ToInt32(lastDayOfMonth);
                        DateTime nuevafecha = new DateTime(Convert.ToInt32("20" + array[1]), Convert.ToInt32(array[0]), day);

                        oPay.CreditCards.CardValidUntil = Convert.ToDateTime(nuevafecha);
                        oPay.CreditCards.OwnerIdNum = cc.OwnerIdNum;
                        oPay.CreditCards.VoucherNum = cc.VoucherNum;
                        oPay.CreditCards.Add();
                        recivedAmount += Convert.ToDouble(cc.CreditSum);
                        ccSum += Convert.ToDouble(cc.CreditSum);
                    }
                }

                if (payment.TransferSum > 0)
                {
                    oPay.TransferSum = Convert.ToDouble(payment.TransferSum);
                    oPay.TransferAccount = Convert.ToString(payment.TransferAccount);
                    oPay.TransferDate = Convert.ToDateTime(payment.TransferDate);
                    oPay.TransferReference = Convert.ToString(payment.TransferReference);
                    recivedAmount += Convert.ToDouble(payment.TransferSum);
                }

                if (!payment.isPayAccount)
                {
                    foreach (var lines in payment.V_PaymentLines)
                    {
                        oPay.Invoices.DocEntry = lines.DocEntry;
                        double totalPay = (Convert.ToDouble(payment.CashSum) + Convert.ToDouble(ccSum) + Convert.ToDouble(checkSum) + Convert.ToDouble(payment.TransferSum)) - Convert.ToDouble((lines.Change <= 0 ? 0 : lines.Change));

                        //if (lines.InvoiceType == (int)COMMON.Constants.DocumentTypes.AnticipatedPayment)
                        //{
                        //    oPay.Invoices.InvoiceType = SAPbobsCOM.BoRcptInvTypes.it_DownPayment;
                        //}

                      //  oPay.Invoices.SumApplied = Convert.ToDouble(lines.PayTotal);
                        oPay.Invoices.Add();
                    }
                }

                oPay.UserFields.Fields.Item("U_MontoRecibido").Value = recivedAmount.ToString();

                if (payment.UdfTarget != null)
                {
                    UserFields oUserFields = oPay.UserFields;

                    payment.UdfTarget.ForEach(x =>
                    {
                        SetUdfValue(x.Name, x.FieldType, x.Value, ref oUserFields);
                    });
                }

                if (oPay.Add() != 0)
                {
                    string sapErrMessage = oCompany.GetLastErrorCode() + " - " + oCompany.GetLastErrorDescription();
                    string ModelObject = "Referencia CardCode #" + payment.CardCode;
                    LogManager.LogMessage("DIAPI/PostDiapiData/CreatePaymentRecived-- Error al actualizar Pago, sapErrMessage: " + sapErrMessage + ", ModelObject" + ModelObject, (int)Constants.LogTypes.General);
                    throw new Exception(oCompany.GetLastErrorCode() + oCompany.GetLastErrorDescription() + "  cliente:" + payment.CardCode);
                }

                DocEntry = Convert.ToInt32(oCompany.GetNewObjectKey());

                LogManager.LogMessage(string.Format("                           PostDIAPIData>CreatePaymentRecived. End Time: {0}| Total Time Time: {1}", DateTime.Now, DateTime.Now - startTime), (int)Constants.LogTypes.API);

                return new PaymentSapResponse
                {
                    Result = true,
                    DocEntry = DocEntry,
                    DocNum = DocNum,
                    Error = null
                };
            }
            catch (Exception ex)
            {
                int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
                string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;
                string ErrMsj = "-" + Convert.ToString("DIAPI/PostDiapiData/CreatePaymentRecived Catch-- Code: " + code + " Message: " + message);
                LogManager.LogMessage(ErrMsj, (int)Constants.LogTypes.SAP);

                string sapErrMessage = oCompany.GetLastErrorCode() + oCompany.GetLastErrorDescription();

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


        // metodo para realizar una cancelacion de un pago en SAP
        // recibe como parametro el modelo de cancelacion de pago, el modelo de company
        public BaseResponse CancelPayment(CancelPayModel canPay, CredentialHolder _userCredentials)
        {
            LogManager.LogMessage("DIAPI/PostDiapiData/CancelPayment-- Cancelando..." + " DocEntry: " + canPay.DocEntry, (int)Constants.LogTypes.SAP);
            var canPayToString = new JavaScriptSerializer().Serialize(canPay);
            // var companyToString = new JavaScriptSerializer().Serialize(company);
            LogManager.LogMessage("DIAPI/PostDiapiData/CancelPayment-- Objeto recibido canPay: " + canPayToString + ", company: " + _userCredentials.DBCode, (int)Constants.LogTypes.SAP);

            Company oCompany = null;
            oCompany = new Company();
            try
            {
                DAO.SuperV2_Entities db = new SuperV2_Entities();
                DateTime DocDate = DateTime.Today;
                oCompany = DIAPICommon.CreateCompanyObject(_userCredentials);
                SAPbobsCOM.Payments oPayments = null;
                oPayments = (SAPbobsCOM.Payments)oCompany.GetBusinessObject(BoObjectTypes.oIncomingPayments);

                if (!oPayments.GetByKey(canPay.DocEntry))
                {
                    if (oCompany != null && oCompany.Connected)
                    {
                        oCompany.Disconnect();
                    }
                    throw new Exception("No existe el documento solicitado");
                }
                else
                {
                    oPayments.Cancel();
                }

                // #REVISAR# EL METODO DE CANCELAR EL PAGO NO ACTUALIZA EN SAP EL MISMO
                //if (oPayments.Update() != 0)
                //{
                //    throw new Exception(oCompany.GetLastErrorCode() + oCompany.GetLastErrorDescription()+"111");
                //}

                if (oCompany != null && oCompany.Connected)
                {
                    oCompany.Disconnect();
                }
                // GC.Collect();
                return new BaseResponse
                {
                    Result = true,
                    Error = null
                };
            }
            catch (Exception ex)
            {
                int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
                string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;
                string ErrMsj = "-" + Convert.ToString("DIAPI/PostDiapiData/CancelPayment Catch-- Code: " + code + " Message: " + message + " DocEntry: " + canPay.DocEntry);
                LogManager.LogMessage(ErrMsj, (int)Constants.LogTypes.SAP);

                string sapErrMessage = oCompany.GetLastErrorCode() + oCompany.GetLastErrorDescription();

                if (oCompany != null)
                {
                    if (oCompany.Connected)
                    {
                        oCompany.Disconnect();
                    }
                }

                GC.Collect();
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

        public void AssignUserDefinedField(string value, string property, ref Documents invoice)
        {
            try
            {
                if (!string.IsNullOrEmpty(value))
                {
                    invoice.UserFields.Fields.Item(property).Value = value;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void SetUdfValue(string _udfName, string _udfDataType, string _udfValue, ref UserFields _udfs)
        {
            try
            {
                if (!String.IsNullOrEmpty(_udfValue))
                {
                    Object oObject = Convert.ChangeType(_udfValue, Type.GetType($"System.{_udfDataType}"));
                    _udfs.Fields.Item(_udfName).Value = oObject;
                }
            }
            catch (Exception e)
            {
                throw new Exception($"El udf {_udfName} no existe o tiene un valor incorrecto: ${e.Message + e.InnerException?.Message}");
            }
        }

        //Metodo para crear una factura en SAP
        //recive como parametro un Objeto de tipo Factura y el objeto company para poder conectarce.
        public InvoiceSapResponse CreateInvoice(CreateInvoice CreateInvoice, string DBName, string userId, string FatherCard, ref Company oCompany, ref LogModel _logModel)
        {
            var startTime = DateTime.Now;
            LogManager.LogMessage(string.Format("                           PostDIAPIData>CreateInvoice. Start Time: {0}", startTime), 2);

            string CreateInvoiceToString = new JavaScriptSerializer().Serialize(CreateInvoice);
            LogManager.LogMessage("DIAPI/PostDiapiData/CreateInvoice-- Objeto recibido canPay: " + CreateInvoiceToString + ", company: " + DBName + ", userId: " + userId + ", FatherCard: " + FatherCard, (int)Constants.LogTypes.SAP);
            DAO.PostData.UpdateLog(_logModel, "", "", startTime, null, "", null, null, null, null, "");
            int DocEntry = 0;

            PaymentSapResponse paymentResponse = new PaymentSapResponse();

            try
            {
                SuperV2_Entities db = new SuperV2_Entities();
                DateTime DocDate = DateTime.Now;
                Documents oInv = null;
                oInv = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oInvoices);
                int SerieType = (int)COMMON.Constants.SerialType.Invoice;

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

                oInv.Series = diapiSeries.Serie;
                oInv.PaymentGroupCode = Convert.ToInt32(CreateInvoice.Invoice.PaymentGroupCode);
                oInv.CardCode = CreateInvoice.Invoice.CardCode;
                oInv.CardName = CreateInvoice.Invoice.CardName;
                oInv.DocDate = DateTime.Now;
                oInv.Comments = CreateInvoice.Invoice.Comments;
                oInv.DocType = BoDocumentTypes.dDocument_Items;//es de articulos, consultar si se van a enviar de servicio
                oInv.DocCurrency = CreateInvoice.Invoice.DocCurrency;
                oInv.SalesPersonCode = CreateInvoice.Invoice.SalesPersonCode;
                if (!string.IsNullOrEmpty(CreateInvoice.Invoice.U_ClaveFe))
                {
                    oInv.UserFields.Fields.Item("U_ClaveFE").Value = CreateInvoice.Invoice.U_ClaveFe;
                }
                if (!string.IsNullOrEmpty(CreateInvoice.Invoice.U_NumFE))
                {
                    oInv.UserFields.Fields.Item("U_NumFE").Value = CreateInvoice.Invoice.U_NumFE;

                }
                if (!string.IsNullOrEmpty(CreateInvoice.Invoice.CLVS_POS_UniqueInvId))
                {
                    oInv.UserFields.Fields.Item("U_CLVS_POS_UniqueInvId").Value = CreateInvoice.Invoice.CLVS_POS_UniqueInvId;
                }
                if (!string.IsNullOrEmpty(CreateInvoice.Invoice.U_TipoDocE))
                {
                    oInv.UserFields.Fields.Item("U_TipoDocE").Value = CreateInvoice.Invoice.U_TipoDocE;
                }
                if (!String.IsNullOrEmpty(CreateInvoice.Invoice.U_Online))
                {
                    oInv.UserFields.Fields.Item("U_Online").Value = CreateInvoice.Invoice.U_Online;
                }
                if (CreateInvoice.Invoice.FEInfo != null)
                {
                    var FEInfo = CreateInvoice.Invoice.FEInfo;

                    if (FEInfo.IdType != string.Empty)
                    {
                        if (int.Parse(FEInfo.IdType) != 0 && int.Parse(FEInfo.IdType) != 99)
                        {
                            AssignUserDefinedField(FEInfo.IdType, "U_TipoIdentificacion", ref oInv);
                            AssignUserDefinedField(FEInfo.Identification, "U_NumIdentFE", ref oInv);
                            AssignUserDefinedField(FEInfo.Email, "U_CorreoFE", ref oInv);
                        }
                    }
                }

                if (CreateInvoice.Invoice.UdfTarget != null)
                {
                    UserFields oUserFields = oInv.UserFields;

                    CreateInvoice.Invoice.UdfTarget.ForEach(x =>
                    {
                        SetUdfValue(x.Name, x.FieldType, x.Value, ref oUserFields);
                    });
                }

                foreach (var line in CreateInvoice.Invoice.InvoiceLinesList)
                {
                    oInv.Lines.ItemCode = line.ItemCode;
                    oInv.Lines.Quantity = line.Quantity;
                    oInv.Lines.UnitPrice = line.UnitPrice;
                    oInv.Lines.DiscountPercent = line.DiscountPercent;
                    oInv.Lines.TaxCode = line.TaxCode;
                    oInv.Lines.WarehouseCode = line.WarehouseCode;
                    oInv.Lines.TaxOnly = line.TaxOnly ? BoYesNoEnum.tYES : BoYesNoEnum.tNO;
                    if (CreateInvoice.Invoice.BaseLines != null && CreateInvoice.Invoice.BaseLines.Count > 0)
                    {
                        int size = CreateInvoice.Invoice.BaseLines.Count;
                        for (int c = 0; c < size; c++)
                        {
                            if (CreateInvoice.Invoice.BaseLines[c].ItemCode == line.ItemCode)
                            {
                                oInv.Lines.BaseType = 17;//(int)CLVSPOS.COMMON.Constants.BaseTypeLine.SALE_QUOTATION;
                                oInv.Lines.BaseLine = CreateInvoice.Invoice.BaseLines[c].BaseLine.Value;
                                oInv.Lines.BaseEntry = CreateInvoice.Invoice.BaseEntry.Value;
                                CreateInvoice.Invoice.BaseLines.RemoveAt(c);
                                break;
                            }
                        }
                    }

                    oInv.Lines.Add();
                }

                if (oInv.Add() != 0)
                {
                    string sapErrMessage = oCompany.GetLastErrorCode() + " - " + oCompany.GetLastErrorDescription();
                    string ModelObject = "Referencia CardCode #" + CreateInvoice.Invoice.CardCode;
                    LogManager.LogMessage("DIAPI/PostDiapiData/CreateInvoice-- Error al actualizar ARInvoice, sapErrMessage: " + sapErrMessage + ", ModelObject" + ModelObject, (int)Constants.LogTypes.SAP);
                    throw new Exception("Error creando la factura, causado por: " + sapErrMessage);
                }

                DocEntry = Convert.ToInt32(oCompany.GetNewObjectKey());

                if (DocEntry == 0)
                {
                    throw new Exception("DocEntry == 0 | " + oCompany.GetLastErrorCode() + " - " + oCompany.GetLastErrorDescription());
                }
                var endTime = DateTime.Now;

                LogManager.LogMessage(string.Format("                           PostDIAPIData>CreateInvoice. End Time: {0}| Total Time Time: {1}", DateTime.Now, DateTime.Now - startTime), (int)Constants.LogTypes.API);

                DAO.PostData.UpdateLog(_logModel, "", "", startTime, endTime, (endTime - startTime).ToString(), null, null, null, null, "");

                return new InvoiceSapResponse
                {
                    Result = true,
                    DocEntry = DocEntry,
                    TypeDocument = Convert.ToString((int)Constants.ObjectSapCode.Invoice),
                    Document = new JavaScriptSerializer().Serialize(CreateInvoice.Invoice),
                    StartTimeDocument = startTime,
                    EndTimeDocument = endTime,
                    ElapsedTimeDocument = Convert.ToString(endTime - startTime),
                    PaymentResponse = paymentResponse

                };
            }
            catch (Exception ex)
            {
                int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
                string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;
                string ErrMsj = "-" + Convert.ToString("DIAPI/PostDiapiData/CreateInvoice Catch-- Code: " + code + " Message: " + message);
                LogManager.LogMessage(ErrMsj, (int)Constants.LogTypes.SAP);
                if (oCompany != null)
                {
                    if (oCompany.Connected)
                    {
                        oCompany.Disconnect();
                    }
                }
                throw;               
            }

        }



        public ItemsResponse CreateGoodsReceiptReturn(GoodsReceipt _goodsRecipt, ref Company _company)
        {
            Document_Lines lines = null;
            SAPbobsCOM.Documents oPurchaseReturns = null;
            LogManager.LogMessage("                        DIAPI/PostDiapiData/CreateGoodsReceiptReturn-- Iniciando devolucion de mercaderia, ", (int)Constants.LogTypes.General);
            try
            {
                DateTime DocDate = DateTime.Now;
                oPurchaseReturns = (SAPbobsCOM.Documents)_company.GetBusinessObject(BoObjectTypes.oPurchaseReturns);

                oPurchaseReturns.CardCode = _goodsRecipt.BusinessPartner.CardCode;
                oPurchaseReturns.DocDate = DocDate;
                if (!String.IsNullOrEmpty(_goodsRecipt.Comments))
                {
                    oPurchaseReturns.Comments = _goodsRecipt.Comments;
                }
                oPurchaseReturns.NumAtCard = _goodsRecipt.NumAtCard;

                if (_goodsRecipt.UdfTarget != null)
                {
                    UserFields oUserFields = oPurchaseReturns.UserFields;

                    _goodsRecipt.UdfTarget.ForEach(x =>
                    {
                        SetUdfValue(x.Name, x.FieldType, x.Value, ref oUserFields);
                    });
                }
                lines = oPurchaseReturns.Lines;

                foreach (var line in _goodsRecipt.Lines)
                {
                    lines.ItemCode = line.ItemCode;
                    lines.UnitPrice = line.UnitPrice;
                    lines.Quantity = line.Quantity;
                    lines.WarehouseCode = line.WareHouse;
                    lines.TaxCode = line.TaxCode;
                    lines.DiscountPercent = line.Discount;
                    lines.Add();
                }


                if (!string.IsNullOrEmpty(_goodsRecipt.U_CLVS_POS_UniqueInvId))
                    oPurchaseReturns.UserFields.Fields.Item("U_CLVS_POS_UniqueInvId").Value = _goodsRecipt.U_CLVS_POS_UniqueInvId;


                if (lines != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(lines);
                }

                if (oPurchaseReturns.Add() != 0)
                {
                    string sapErrMessage = _company.GetLastErrorCode() + " - " + _company.GetLastErrorDescription();
                    LogManager.LogMessage("                        DIAPI/PostDiapiData/CreateGoodReceiptReturn-- Error al crear la devolucion de inventario , sapErrMessage: " + sapErrMessage, (int)Constants.LogTypes.General);
                    throw new Exception(_company.GetLastErrorCode() + _company.GetLastErrorDescription());
                }

                int docEntry = Convert.ToInt32(_company.GetNewObjectKey());

                if (lines != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(lines);
                }

                if (oPurchaseReturns != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oPurchaseReturns);
                }

                if (oPurchaseReturns != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oPurchaseReturns);
                }

                return new ItemsResponse
                {
                    Result = true,
                    DocNum = docEntry
                };

            }
            catch
            {
                if (lines != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(lines);
                }

                if (lines != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(lines);
                }

                if (oPurchaseReturns != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oPurchaseReturns);
                }

                if (oPurchaseReturns != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oPurchaseReturns);
                }

                throw;
            }
        }

        //Metodo para crear una nc en SAP
        //recive como parametro un Objeto de tipo Factura y el objeto company para poder conectarce.
        public InvoiceSapResponse CreateInvoiceNc(CreateInvoice CreateInvoice, string DBName, string userId, string FatherCard, ref Company oCompany)
        {
            DateTime startTime = DateTime.Now;
            LogManager.LogMessage(string.Format("PostDIAPIData>CreateInvoiceNc. Start Time: {0}", startTime), 2);

            string CreateInvoiceToString = new JavaScriptSerializer().Serialize(CreateInvoice);
            LogManager.LogMessage("                 DIAPI/PostDiapiData/CreateInvoiceNc-- Objeto : " + CreateInvoiceToString + ", company: " + DBName + ", userId: " + userId + ", FatherCard: " + FatherCard, (int)Constants.LogTypes.SAP);

            int DocEntry = 0;

            PaymentSapResponse paymentResponse = new PaymentSapResponse();
            Documents oInv = null;

            try
            {
                DAO.SuperV2_Entities db = new SuperV2_Entities();
                DateTime DocDate = DateTime.Now;

                oInv = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oCreditNotes);

                int SerieType = (int)COMMON.Constants.SerialType.CreditMemo;

                UserAssign userA = db.UserAssign.Where(x => x.UserId == userId).FirstOrDefault();

                DiapiSerie diapiSeries = userA.V_SeriesByUser.Where(x => x.V_Series.DocType == SerieType && x.V_Series.Type == (int)COMMON.Constants.SerieNumberingType.Online).Select(x => new DiapiSerie
                {
                    NumType = x.V_Series.Numbering,
                    Serie = x.V_Series.Serie
                }).FirstOrDefault();

                if (diapiSeries == null)
                {
                    throw new Exception(string.Format("No se ha definido una serie para crear notas de crédito Online"));
                }

                oInv.Series = diapiSeries.Serie;
                oInv.DocType = BoDocumentTypes.dDocument_Items;//es de articulos, consultar si se van a enviar de servicio
                oInv.PaymentGroupCode = Convert.ToInt32(CreateInvoice.Invoice.PaymentGroupCode);
                oInv.CardCode = CreateInvoice.Invoice.CardCode;
                oInv.CardName = CreateInvoice.Invoice.CardName;
                oInv.DocDate = DateTime.Now;
                oInv.Comments = CreateInvoice.Invoice.Comments;
                oInv.DocCurrency = CreateInvoice.Invoice.DocCurrency;
                oInv.SalesPersonCode = CreateInvoice.Invoice.SalesPersonCode;

                if (!String.IsNullOrEmpty(CreateInvoice.Invoice.NumAtCard)) oInv.NumAtCard = CreateInvoice.Invoice.NumAtCard;


                if (!string.IsNullOrEmpty(CreateInvoice.Invoice.U_ClaveFe))
                {
                    oInv.UserFields.Fields.Item("U_ClaveFE").Value = CreateInvoice.Invoice.U_ClaveFe;
                }
                if (!string.IsNullOrEmpty(CreateInvoice.Invoice.U_NumFE))
                {
                    oInv.UserFields.Fields.Item("U_NumFE").Value = CreateInvoice.Invoice.U_NumFE;

                }
                if (!string.IsNullOrEmpty(CreateInvoice.Invoice.CLVS_POS_UniqueInvId))
                {
                    oInv.UserFields.Fields.Item("U_CLVS_POS_UniqueInvId").Value = CreateInvoice.Invoice.CLVS_POS_UniqueInvId;

                }
                if (CreateInvoice.Invoice.FEInfo != null)
                {
                    var FEInfo = CreateInvoice.Invoice.FEInfo;

                    if (FEInfo.IdType != string.Empty)
                    {
                        if (int.Parse(FEInfo.IdType) != 0 && int.Parse(FEInfo.IdType) != 99)
                        {
                            AssignUserDefinedField(FEInfo.IdType, "U_TipoIdentificacion", ref oInv);
                            AssignUserDefinedField(FEInfo.Identification, "U_NumIdentFE", ref oInv);
                            AssignUserDefinedField(FEInfo.Email, "U_CorreoFE", ref oInv);
                            //AssignUserDefinedField(FEInfo.Provincia, "U_Provincia", ref oInv);
                            //AssignUserDefinedField(FEInfo.Canton, "U_Canton", ref oInv);
                            //AssignUserDefinedField(FEInfo.Distrito, "U_Distrito", ref oInv);
                            //AssignUserDefinedField(FEInfo.Barrio, "U_Barrio", ref oInv);
                            //AssignUserDefinedField(FEInfo.Direccion, "U_Direccion", ref oInv);
                        }
                    }
                }
                if (CreateInvoice.Invoice.UdfTarget != null)
                {
                    UserFields oUserFields = oInv.UserFields;

                    CreateInvoice.Invoice.UdfTarget.ForEach(x =>
                    {
                        SetUdfValue(x.Name, x.FieldType, x.Value, ref oUserFields);
                    });
                }

                foreach (LinesInvoiceModel line in CreateInvoice.Invoice.InvoiceLinesList)
                {
                    oInv.Lines.ItemCode = line.ItemCode;
                    oInv.Lines.Quantity = line.Quantity;
                    oInv.Lines.UnitPrice = line.UnitPrice;
                    oInv.Lines.DiscountPercent = line.DiscountPercent;
                    oInv.Lines.TaxCode = line.TaxCode;
                    oInv.Lines.WarehouseCode = line.WarehouseCode;
                    oInv.Lines.Add();
                }

                if (oInv.Add() != 0)
                {
                    string sapErrMessage = oCompany.GetLastErrorCode() + " - " + oCompany.GetLastErrorDescription();
                    string ModelObject = "Referencia CardCode #" + CreateInvoice.Invoice.CardCode;
                    LogManager.LogMessage("DIAPI/PostDiapiData/CreateInvoiceNc-- Error al crear la nota crédito, sapErrMessage: " + sapErrMessage + ", ModelObject" + ModelObject, (int)Constants.LogTypes.SAP);
                    throw new Exception(sapErrMessage + ModelObject);
                }

                DocEntry = Convert.ToInt32(oCompany.GetNewObjectKey());

                if (DocEntry == 0)
                {
                    throw new Exception("DocEntry == 0 | " + oCompany.GetLastErrorCode() + " - " + oCompany.GetLastErrorDescription());
                }

                LogManager.LogMessage(string.Format("                           PostDIAPIData>CreateInvoiceNc. End Time: {0}| Total Time Time: {1}", DateTime.Now, DateTime.Now - startTime), (int)Constants.LogTypes.API);

                if (oInv != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oInv);
                }

                if (oInv != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oInv);
                }

                Documents oNc = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oCreditNotes);
                if (!oNc.GetByKey(DocEntry))
                {
                    return new InvoiceSapResponse
                    {
                        Result = true,
                        DocNum = -1,
                        DocEntry = DocEntry,
                        PaymentResponse = paymentResponse
                    };
                }
                int DocNum = oNc.DocNum;
                return new InvoiceSapResponse
                {
                    Result = true,
                    DocNum = DocNum,
                    DocEntry = DocEntry,
                    PaymentResponse = paymentResponse
                };
            }
            catch (Exception ex)
            {
                int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
                string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;
                string ErrMsj = "-" + Convert.ToString("DIAPI/PostDiapiData/CreateInvoiceNc Catch-- Code: " + code + " Message: " + message);
                LogManager.LogMessage(ErrMsj, (int)Constants.LogTypes.SAP);
                if (oCompany != null)
                {
                    if (oCompany.Connected)
                    {
                        oCompany.Disconnect();
                    }
                }
                return new InvoiceSapResponse
                {
                    Result = false,
                    DocEntry = DocEntry,
                    Error = new ErrorInfo
                    {
                        Code = code,
                        Message = message
                    }
                };
            }
        }

        public InvoiceSapResponse SyncCreateInvoice(OFF_OINV createInvoice, Companys company, string userId, ref Company oCompany)
        {
            var CreateInvoiceToString = new JavaScriptSerializer().Serialize(createInvoice);
            LogManager.LogMessage("DIAPI/PostDiapiData/SyncCreateInvoice-- Objeto recibido canPay: " + CreateInvoiceToString + ", company: " + company.DBName + ", userId: " + userId + ", FatherCard: " + createInvoice.FatherCard, (int)Constants.LogTypes.SAP);

            int DocEntry = 0;

            PaymentSapResponse paymentResponse = new PaymentSapResponse();
            try
            {
                DAO.SuperV2_Entities db = new SuperV2_Entities();
                DateTime DocDate = DateTime.Today;
                Documents oInv = null;

                oInv = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oInvoices);

                oInv.Series = int.Parse(createInvoice.Series);
                oInv.PaymentGroupCode = int.Parse(createInvoice.PaymentGroupCode);
                oInv.CardCode = createInvoice.CardCode;
                oInv.CardName = createInvoice.CardName;

                oInv.DocDate = DateTime.Now;
                //oInv.DocDueDate = createInvoice.DocDate ?? DateTime.Now;
                oInv.Comments = createInvoice.Comments;
                oInv.DocType = (SAPbobsCOM.BoDocumentTypes)int.Parse(createInvoice.DocType);
                oInv.DocCurrency = createInvoice.DocCur;
                oInv.SalesPersonCode = int.Parse(createInvoice.SalesPersonCode);

                oInv.UserFields.Fields.Item("U_DocDateOffline").Value = createInvoice.DocDate;
                oInv.UserFields.Fields.Item("U_DocTimeOffline").Value = createInvoice.DocTime;
                oInv.UserFields.Fields.Item("U_CLVS_POS_UniqueInvId").Value = createInvoice.U_CLVS_POS_UniqueInvId;

                oInv.UserFields.Fields.Item("U_ClaveFE").Value = createInvoice.Clave ?? "";
                oInv.UserFields.Fields.Item("U_NumFE").Value = createInvoice.NumeroConsecutivo ?? "";

                var idType = createInvoice.U_TipoIdentificacion;

                if (!string.IsNullOrEmpty(idType))
                {
                    if (int.Parse(idType) != 0 && int.Parse(idType) != 99)
                    {
                        AssignUserDefinedField(createInvoice.IdType, "U_TipoIdentificacion", ref oInv);
                        AssignUserDefinedField(createInvoice.U_NumIdenFE, "U_NumIdentFE", ref oInv);
                        AssignUserDefinedField(createInvoice.U_CorreoFE, "U_CorreoFE", ref oInv);
                        AssignUserDefinedField(createInvoice.U_Provincia, "U_Provincia", ref oInv);
                        AssignUserDefinedField(createInvoice.U_Canton, "U_Canton", ref oInv);
                        AssignUserDefinedField(createInvoice.U_Distrito, "U_Distrito", ref oInv);
                        AssignUserDefinedField(createInvoice.U_Barrio, "U_Barrio", ref oInv);
                        AssignUserDefinedField(createInvoice.U_Direccion, "U_Direccion", ref oInv);
                    }
                }

                foreach (var line in createInvoice.OFF_INV1)
                {
                    oInv.Lines.ItemCode = line.ItemCode;
                    oInv.Lines.Quantity = line.Quantity;
                    oInv.Lines.UnitPrice = line.UnitPrice;
                    oInv.Lines.DiscountPercent = line.DiscountPercent;
                    oInv.Lines.TaxCode = line.TaxCode;
                    oInv.Lines.WarehouseCode = line.WarehouseCode;

                    oInv.Lines.Add();
                }

                if (oInv.Add() != 0)
                {
                    string sapErrMessage = oCompany.GetLastErrorCode() + " - " + oCompany.GetLastErrorDescription();
                    string ModelObject = "Referencia CardCode #" + createInvoice.CardCode;
                    LogManager.LogMessage("DIAPI/PostDiapiData/SyncCreateInvoice-- Error al actualizar ARInvoice, sapErrMessage: " + sapErrMessage + ", ModelObject" + ModelObject, (int)Constants.LogTypes.SAP);
                    throw new Exception(sapErrMessage + ModelObject);
                }

                DocEntry = Convert.ToInt32(oCompany.GetNewObjectKey());

                return new InvoiceSapResponse
                {
                    Result = true,
                    DocEntry = DocEntry,
                    PaymentResponse = paymentResponse
                };
            }
            catch (Exception ex)
            {
                int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
                string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;
                string ErrMsj = "-" + Convert.ToString("DIAPI/PostDiapiData/SyncCreateInvoice Catch-- Code: " + code + " Message: " + message);
                LogManager.LogMessage(ErrMsj, (int)Constants.LogTypes.SAP);
             
                return new InvoiceSapResponse
                {
                    Result = false,
                    DocEntry = DocEntry,
                    Error = new ErrorInfo
                    {
                        Code = code,
                        Message = message
                    }
                };
            }
        }

        public PaymentSapResponse SyncCreatePayment(OFF_Payment payment, Companys company, string user, string FatherCard, ref Company oCompany, string invoiceCurrency)
        {
            LogManager.LogMessage("DIAPI/PostDiapiData/SyncCreatePayment-- Ingresando..." + " CardCode: " + payment.CardCode, (int)Constants.LogTypes.SAP);
            var paymentToString = new JavaScriptSerializer().Serialize(payment);
            //var companyToString = new JavaScriptSerializer().Serialize(company);
            LogManager.LogMessage("DIAPI/PostDiapiData/SyncCreatePayment-- Objeto recibido payment: " + paymentToString + ", company: " + company.DBName + ", user: " + user + ", FatherCard: " + FatherCard, (int)Constants.LogTypes.SAP);

            int DocEntry = 0;
            int DocNum = 0;
            double ccSum = 0;
            double checkSum = 0;
            try
            {
                DAO.SuperV2_Entities db = new SuperV2_Entities();
                DateTime DocDate = DateTime.Today;

                Payments oPay = null;
                oPay = (Payments)oCompany.GetBusinessObject(BoObjectTypes.oIncomingPayments);
                oPay.Series = payment.Series;

                if (oPay.Invoices != null && oPay.Invoices.Count > 0)
                {
                    oPay.DocType = BoRcptTypes.rCustomer;
                }
                else
                {
                    oPay.DocType = BoRcptTypes.rAccount;
                }

                oPay.DocCurrency = payment.DocCurrency;
                oPay.CardCode = !string.IsNullOrEmpty(FatherCard) ? FatherCard : payment.CardCode;
                oPay.CardCode = payment.CardCode;
                oPay.DocDate = DateTime.Now;
                oPay.DueDate = DateTime.Now;
                oPay.CounterReference = payment.CounterReference;
                oPay.Remarks = payment.Remarks;

                if (payment.CashSum != 0)
                {
                    oPay.CashSum = payment.CashSum - payment.Change;
                    oPay.CashAccount = payment.CashAccount;
                }

                if (payment.Checks != null && payment.Checks.Count > 0)
                {
                    DateTime testDate;
                    string parsedDate = string.Empty;

                    foreach (var checks in payment.Checks)
                    {
                        oPay.CheckAccount = checks.CheckAccount;
                        oPay.Checks.AccounttNum = checks.AcctNum;
                        oPay.Checks.BankCode = checks.BankCode;
                        oPay.Checks.CountryCode = checks.CountryCode;
                        oPay.Checks.CheckNumber = Convert.ToInt32(checks.CheckNumber);
                        oPay.Checks.CheckSum = Convert.ToDouble(checks.CheckSum);

                        if (DateTime.TryParse(checks.DueDate, out testDate))
                        {
                            oPay.Checks.DueDate = testDate;
                        }
                        else
                        {
                            parsedDate = string.Format("{0}/{1}/{2}", checks.DueDate.Substring(3, 2), checks.DueDate.Substring(0, 2), checks.DueDate.Substring(6, 4));
                            oPay.Checks.DueDate = DateTime.Parse(parsedDate);
                        }

                        oPay.DocCurrency = checks.Curr;
                        oPay.Checks.Add();

                        checkSum += Convert.ToDouble(checks.CheckSum);
                    }

                }

                if (payment.CreditCards != null && payment.CreditCards.Count > 0)
                {
                    foreach (var cc in payment.CreditCards)
                    {
                        oPay.CreditCards.CreditSum = Convert.ToDouble(cc.CreditSum);
                        oPay.CreditCards.CreditCard = Convert.ToInt32(cc.CreditCard);
                        oPay.CreditCards.CreditCardNumber = Convert.ToString(cc.CreditCardNumber);

                        DateTime testDate;
                        string parsedDate = string.Empty;

                        if (DateTime.TryParse(cc.CardValid, out testDate))
                        {
                            oPay.CreditCards.CardValidUntil = testDate;
                        }
                        else
                        {
                            parsedDate = string.Format("{0}/{1}/{2}", cc.CardValid.Substring(3, 2), cc.CardValid.Substring(0, 2), cc.CardValid.Substring(6, 4));
                            oPay.CreditCards.CardValidUntil = DateTime.Parse(parsedDate);
                        }

                        oPay.CreditCards.OwnerIdNum = cc.OwnerIdNum;
                        oPay.CreditCards.VoucherNum = cc.VoucherNum;
                        oPay.CreditCards.Add();
                        ccSum += Convert.ToDouble(cc.CreditSum);

                    }
                }

                if (payment.TransferSum > 0)
                {
                    string parsedDate = string.Empty;

                    oPay.TransferSum = Convert.ToDouble(payment.TransferSum);
                    oPay.TransferAccount = Convert.ToString(payment.TransferAccount);

                    parsedDate = string.Format("{0}/{1}/{2}", payment.TransferDate.Value.Month, payment.TransferDate.Value.Day, payment.TransferDate.Value.Year);
                    oPay.TransferDate = DateTime.Parse(parsedDate);

                    oPay.TransferReference = Convert.ToString(payment.TransferReference);
                }


                if (payment.DocType == (int)COMMON.Constants.DocumentTypes.AnticipatedPayment) { oPay.Invoices.InvoiceType = SAPbobsCOM.BoRcptInvTypes.it_DownPayment; }

                oPay.Invoices.DocEntry = payment.DocEntry;

                oPay.Invoices.AppliedFC = payment.AppliedFC;
                oPay.Invoices.SumApplied = payment.SumApplied;
                oPay.UserFields.Fields.Item("U_MontoRecibido").Value = payment.U_MontoRecibido;


                //double totalPay = Convert.ToDouble(payment.CashSum) + Convert.ToDouble(ccSum) + Convert.ToDouble(checkSum) + Convert.ToDouble(payment.TransferSum) - Convert.ToDouble((payment.Change <= 0 ? 0 : payment.Change));

                //if (invoiceCurrency == "COL")
                //{
                //    oPay.Invoices.AppliedFC = totalPay;
                //    oPay.Invoices.SumApplied = totalPay * Convert.ToDouble(payment.DocRate);
                //}
                //else
                //{
                //    oPay.Invoices.AppliedFC = totalPay;
                //    oPay.Invoices.SumApplied = totalPay / Convert.ToDouble(payment.DocRate);
                //}

                oPay.Invoices.Add();

                if (oPay.Add() != 0)
                {
                    string sapErrMessage = oCompany.GetLastErrorCode() + " - " + oCompany.GetLastErrorDescription();
                    string ModelObject = "Referencia CardCode #" + payment.CardCode;
                    LogManager.LogMessage("DIAPI/PostDiapiData/SyncCreatePayment-- Error al actualizar Pago, sapErrMessage: " + sapErrMessage + ", ModelObject" + ModelObject, (int)Constants.LogTypes.General);
                    throw new Exception(oCompany.GetLastErrorCode() + oCompany.GetLastErrorDescription() + "  cliente:" + payment.CardCode);
                }

                DocEntry = Convert.ToInt32(oCompany.GetNewObjectKey());
                DocNum = 0;

                return new PaymentSapResponse
                {
                    Result = true,
                    DocEntry = DocEntry,
                    DocNum = DocNum,
                    Error = null
                };
            }
            catch (Exception ex)
            {
                int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
                string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;
                string ErrMsj = "-" + Convert.ToString("DIAPI/PostDiapiData/SyncCreatePayment Catch-- Code: " + code + " Message: " + message);
                LogManager.LogMessage(ErrMsj, (int)Constants.LogTypes.SAP);

                string sapErrMessage = oCompany.GetLastErrorCode() + oCompany.GetLastErrorDescription();

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
        /// Crea un item basado en el model ItemModel
        /// </summary>
        /// <param name="_itemModel"></param>
        /// <param name="_company"></param>
        /// <returns></returns>
        public ItemsResponse CreateItem(ItemsModel _itemModel, ref Company _company)
        {
            var startTime = DateTime.Now;
            try
            {
                DateTime DocDate = DateTime.Now;

                Items oItem = null;
                var oPriceList = (PriceLists)_company.GetBusinessObject(BoObjectTypes.oPriceLists);
                oItem = (Items)_company.GetBusinessObject(BoObjectTypes.oItems);

                if (oItem.GetByKey(_itemModel.ItemCode))
                {
                    return new ItemsResponse
                    {
                        Result = false,
                        Error = new ErrorInfo
                        {
                            Code = 500,
                            Message = "El código del producto ya se encuentra registrado"
                        }
                    };
                }

                oItem.Series = 74; // harcoded #VALORPRUEBA#

                oItem.ItemName = _itemModel.ItemName;
                oItem.ForeignName = _itemModel.ForeingName;
                oItem.ApTaxCode = _itemModel.TaxCode;
                if (_itemModel.Barcodes != null && _itemModel.Barcodes.Count > 0) oItem.BarCode = _itemModel.Barcodes[0].BcdCode;
                //oItem.ForeignName = _itemModel.FirmName;
                UserFields userFields = oItem.UserFields;

                userFields.Fields.Item("U_IVA").Value = _itemModel.TaxCode;

                if (_itemModel.UdfTarget != null)
                {
                    _itemModel.UdfTarget.ForEach(x =>   
                    {
                        SetUdfValue(x.Name, x.FieldType, x.Value, ref userFields);
                    });
                }

                if (oItem.Add() != 0)
                {
                    string sapErrMessage = _company.GetLastErrorCode() + " - " + _company.GetLastErrorDescription();
                    LogManager.LogMessage("DIAPI/PostDiapiData/CreateItem-- Error al crear el item, sapErrMessage: " + sapErrMessage, (int)Constants.LogTypes.General);
                    throw new Exception(_company.GetLastErrorCode() + _company.GetLastErrorDescription());
                }

                // Se pasa por referencia tmp para obtener el itemcode recien creado y poder llamar el metodo de actualizar
                string itemCode = "";
                _company.GetNewObjectCode(out itemCode);

                if (itemCode == "")
                {
                    string sapErrMessage = "No se pudo agregar el codigo de barras, edite el producto y agreguelo";
                    LogManager.LogMessage("DIAPI/PostDiapiData/CreateItem-- Error al crear el item, : " + sapErrMessage, (int)Constants.LogTypes.General);
                    throw new Exception(_company.GetLastErrorCode() + _company.GetLastErrorDescription());
                }

                _itemModel.ItemCode = itemCode;
                List<ItemsBarcodeModel> barCodesToUpdate = new List<ItemsBarcodeModel>();
                if (_itemModel.Barcodes != null && _itemModel.Barcodes.Count > 0)
                {
                    barCodesToUpdate.Add(_itemModel.Barcodes[0]);
                }
                // Registra los posibles codigos de barras asociados
                if (!UpdateBarcodes(ref _company, _itemModel, barCodesToUpdate).Result)
                {
                    string sapErrMessage = _company.GetLastErrorCode() + " - " + _company.GetLastErrorDescription();
                    LogManager.LogMessage("DIAPI/PostDiapiData/UpdatePriceList-- Error al agregar codigos de barras al producto, sapErrMessage: " + sapErrMessage, (int)Constants.LogTypes.General);
                    throw new Exception(_company.GetLastErrorCode() + _company.GetLastErrorDescription());
                }
                // Registra las posibles listas de precios asociadas a un producto
                if (!UpdatePriceList(ref _company, ref _itemModel).Result)
                {
                    string sapErrMessage = _company.GetLastErrorCode() + " - " + _company.GetLastErrorDescription();
                    LogManager.LogMessage("DIAPI/PostDiapiData/UpdatePriceList-- Error al agregar el precio al item, sapErrMessage: " + sapErrMessage, (int)Constants.LogTypes.General);
                    throw new Exception(_company.GetLastErrorCode() + _company.GetLastErrorDescription());
                }

                return new ItemsResponse
                {
                    Result = true
                };

            }
            catch (Exception ex)
            {
                throw;
                
            }
        }

        /// <summary>
        /// Actualiza los valores del item, ademas de agrega o edita tanto la lista de codigos de barra, como la lista de precios
        /// </summary>
        /// <param name="_itemModel"></param>
        /// <param name="_company"></param>
        /// <param name="_barcodes"></param>
        /// <returns></returns>
        public ItemsResponse UpdateItem(ItemsModel _itemModel, ref Company _company, List<ItemsBarcodeModel> _barcodes)
        {
            var startTime = DateTime.Now;
            Boolean isFound = false;
            UserFields userFields = null;
            SAPbobsCOM.BarCode oBarCode = null;
            SAPbobsCOM.BarCodeParams oBarCodeParams = null;
            SAPbobsCOM.BarCodesService oBarCodesService = null;
            SAPbobsCOM.Items_Prices oItemPrice = null;
            SAPbobsCOM.ICompanyService oCompanyService = null;
            try
            {
                DateTime DocDate = DateTime.Now;

                Items oItem = null;
                oItem = (Items)_company.GetBusinessObject(BoObjectTypes.oItems);
                userFields = oItem.UserFields;
                if (oItem.GetByKey(_itemModel.ItemCode))
                {
                    oItem.ItemName = _itemModel.ItemName;
                    oItem.ApTaxCode = _itemModel.TaxCode;
                    oItem.ForeignName = _itemModel.FirmName;
                    oItem.Valid = !_itemModel.Frozen ? BoYesNoEnum.tYES : BoYesNoEnum.tNO;
                    oItem.Frozen = _itemModel.Frozen ? BoYesNoEnum.tYES : BoYesNoEnum.tNO;
                    oItem.ForeignName = _itemModel.ForeingName;
                    userFields.Fields.Item("U_IVA").Value = _itemModel.TaxCode;

                    if (_itemModel.UdfTarget != null)
                    {
                        _itemModel.UdfTarget.ForEach(x =>
                        {
                            SetUdfValue(x.Name, x.FieldType, x.Value, ref userFields);
                        });
                    }

                    if (_barcodes != null && _barcodes.Count > 0) oItem.BarCode = _barcodes[0].BcdCode;

                    if (userFields != null)
                    {
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(userFields);
                    }

                    oItemPrice = oItem.PriceList;
                    oCompanyService = (SAPbobsCOM.ICompanyService)_company.GetCompanyService();
                    oBarCodesService = (SAPbobsCOM.BarCodesService)oCompanyService.GetBusinessService(ServiceTypes.BarCodesService);// .ServiceTypes.BarCodesService;

                    if (oItem.Update() != 0)
                    {
                        string sapErrMessage = _company.GetLastErrorCode() + " - " + _company.GetLastErrorDescription();
                        LogManager.LogMessage("DIAPI/PostDiapiData/UpdateItem-- Error al editar el item, sapErrMessage: " + sapErrMessage, (int)Constants.LogTypes.General);
                        throw new Exception(_company.GetLastErrorCode() + _company.GetLastErrorDescription());
                    }
                
                    UpdatePriceList(ref _company, ref _itemModel);

                    if (userFields != null)
                    {
                        System.Runtime.InteropServices.Marshal.FinalReleaseComObject(userFields);
                    }

                    UpdateBarcodes(ref _company, _itemModel, _barcodes);

                    #region MARSHAL_SECTION

                    if (oBarCodesService != null)
                    {
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(oBarCodesService);
                    }
                    if (oBarCodesService != null)
                    {
                        System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oBarCodesService);
                    }
                    if (oBarCode != null)
                    {
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(oBarCode);
                    }
                    if (oBarCode != null)
                    {
                        System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oBarCode);
                    }
                    if (oBarCodeParams != null)
                    {
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(oBarCode);
                    }
                    if (oBarCodeParams != null)
                    {
                        System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oBarCode);
                    }
                    if (oItemPrice != null)
                    {
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(oItemPrice);
                    }
                    if (oItemPrice != null)
                    {
                        System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oItemPrice);
                    }
                    if (oCompanyService != null)
                    {
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(oItemPrice);
                    }
                    if (oCompanyService != null)
                    {
                        System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oCompanyService);
                    }

                    #endregion

                    return new ItemsResponse
                    {
                        Result = true
                    };
                }

                return new ItemsResponse
                {
                    Result = false,
                    Error = new ErrorInfo
                    {
                        Code = 500,
                        Message = "El producto no existe"
                    }
                };
            }
            catch (Exception ex)
            {
                #region MARSHAL_SECTION
                if (oBarCodesService != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oBarCodesService);
                }
                if (oBarCodesService != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oBarCodesService);
                }
                if (oBarCode != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oBarCode);
                }
                if (oBarCode != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oBarCode);
                }
                if (oBarCodeParams != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oBarCode);
                }
                if (oBarCodeParams != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oBarCode);
                }
                if (oItemPrice != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oItemPrice);
                }
                if (oItemPrice != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oItemPrice);
                }
                if (oCompanyService != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oItemPrice);
                }
                if (oCompanyService != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oCompanyService);
                }
                #endregion
                int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
                string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;
                string ErrMsj = "-" + Convert.ToString("DIAPI/PostDiapiData/UpdateItem Catch-- Code: " + code + " Message: " + message);
                LogManager.LogMessage(ErrMsj, (int)Constants.LogTypes.SAP);

                string sapErrMessage = _company.GetLastErrorCode() + _company.GetLastErrorDescription();

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

        private ItemsResponse UpdateBarcodes(ref Company _company, ItemsModel _itemModel, List<ItemsBarcodeModel> _barcodes)
        {
            SAPbobsCOM.Items_Prices oItemPrice = null;
            Items oItem = null;
            SAPbobsCOM.BarCode oBarCode = null;
            SAPbobsCOM.BarCodeParams oBarCodeParams = null;
            SAPbobsCOM.BarCodesService oBarCodesService = null;
            SAPbobsCOM.ICompanyService oCompanyService = null;
            oItem = (Items)_company.GetBusinessObject(BoObjectTypes.oItems);
            Boolean result = false;
            Boolean isFound = false;
            try
            {
                if (oItem.GetByKey(_itemModel.ItemCode))
                {
                    oCompanyService = (SAPbobsCOM.ICompanyService)_company.GetCompanyService();
                    oBarCodesService = (SAPbobsCOM.BarCodesService)oCompanyService.GetBusinessService(ServiceTypes.BarCodesService);// .ServiceTypes.BarCodesService;

                    foreach (var barcode in _itemModel.Barcodes)
                    {
                        oBarCode = (SAPbobsCOM.BarCode)oBarCodesService.GetDataInterface(BarCodesServiceDataInterfaces.bsBarCode);
                        isFound = false;
                        foreach (var spBarcodee in _barcodes)
                        {
                            if (barcode.BcdCode.Equals(spBarcodee.BcdCode)) isFound = true;
                        }

                        // Si lo encuentra lo edita, si no lo crea( Se verifica en el Process.cs que le codigo esta libre)
                        if (!isFound)
                        {
                            oBarCode.ItemNo = _itemModel.ItemCode;
                            oBarCode.BarCode = barcode.BcdCode;
                            oBarCode.FreeText = barcode.BcdName;
                            oBarCode.UoMEntry = -1; // TODO -> Hay que reemplazar cuando se vean las unidadesd de medida
                            oBarCodesService.Add(oBarCode);
                        }
                        else
                        {  
                            if (barcode.BcdEntry != -1)
                            {// Comprobacion para evitar cualquier acceso ilegal a una actualizacion de codigo de barras
                                oBarCodeParams = (SAPbobsCOM.BarCodeParams)oBarCodesService.GetDataInterface(BarCodesServiceDataInterfaces.bsBarCodeParams);
                                oBarCodeParams.AbsEntry = barcode.BcdEntry;
                                oBarCode = oBarCodesService.Get(oBarCodeParams);
                                oBarCode.BarCode = barcode.BcdCode;
                                oBarCode.FreeText = barcode.BcdName;
                                oBarCode.UoMEntry = -1; // TODO -> Hay que reemplazar cuando se vean las unidadesd de medida
                                oBarCodesService.Update(oBarCode);
                            }
                        }
                    }
                    result = true;
                }

                return new ItemsResponse
                {
                    Result = result
                };
            }
            catch (Exception ex)
            {
                if (oItemPrice != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oItemPrice);
                }
                if (oItemPrice != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oItemPrice);
                }
                int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
                string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;
                string ErrMsj = "-" + Convert.ToString("DIAPI/PostDiapiData/UpdateBarcodes Catch-- Code: " + code + " Message: " + message);
                LogManager.LogMessage(ErrMsj, (int)Constants.LogTypes.SAP);

                string sapErrMessage = _company.GetLastErrorCode() + _company.GetLastErrorDescription();

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

        private ItemsResponse UpdatePriceList(ref Company _company, ref ItemsModel _itemModel)
        {
            SAPbobsCOM.Items_Prices oItemPrice = null;
            Items oItem = null;
            oItem = (Items)_company.GetBusinessObject(BoObjectTypes.oItems);
            Boolean result = false;
            try
            {
                if (oItem.GetByKey(_itemModel.ItemCode))
                {
                    oItemPrice = oItem.PriceList;
                    // Itera sobre la lista de precios del objecto y le setea los valores indicados
                    foreach (var item in _itemModel.PriceList.Select((value, i) => new { i, value }))
                    {
                        oItemPrice.SetCurrentLine(item.i);
                        oItemPrice.Price = item.value.Price;

                    }

                    if (oItem.Update() != 0)
                    {
                        string sapErrMessage = _company.GetLastErrorCode() + " - " + _company.GetLastErrorDescription();
                        LogManager.LogMessage("DIAPI/PostDiapiData/UpdateItem-- Error al editar el item, sapErrMessage: " + sapErrMessage, (int)Constants.LogTypes.General);
                        throw new Exception(_company.GetLastErrorCode() + _company.GetLastErrorDescription());
                    }
                    result = true;
                }

                return new ItemsResponse
                {
                    Result = result
                };
            }
            catch (Exception ex)
            {
                if (oItemPrice != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oItemPrice);
                }
                if (oItemPrice != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oItemPrice);
                }
                int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
                string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;
                string ErrMsj = "-" + Convert.ToString("DIAPI/PostDiapiData/UpdateItem Catch-- Code: " + code + " Message: " + message);
                LogManager.LogMessage(ErrMsj, (int)Constants.LogTypes.SAP);

                string sapErrMessage = _company.GetLastErrorCode() + _company.GetLastErrorDescription();

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
        /// Crea un entrada de inventario con la lista de lineas y el proveedor
        /// </summary>
        /// <param name="_lines"></param>
        /// <param name="_businessParter"></param>
        /// <param name="_company"></param>
        /// <returns></returns>
        public ItemsResponse CreateGoodsReceipt(GoodsReceipt _goodsReceipt, ref Company _company)
        {
            Document_Lines lines = null;
            SAPbobsCOM.Documents oGoodReceipt = null;
            try
            {
                DateTime DocDate = DateTime.Now;
                oGoodReceipt = (SAPbobsCOM.Documents)_company.GetBusinessObject(BoObjectTypes.oPurchaseDeliveryNotes);

                oGoodReceipt.CardCode = _goodsReceipt.BusinessPartner.CardCode;
                oGoodReceipt.DocDate = DocDate;

                if (!String.IsNullOrEmpty(_goodsReceipt.Comments))
                {
                    oGoodReceipt.Comments = _goodsReceipt.Comments;
                }

                if (_goodsReceipt.UdfTarget != null)
                {
                    UserFields oUserFields = oGoodReceipt.UserFields;

                    _goodsReceipt.UdfTarget.ForEach(x =>
                    {
                        SetUdfValue(x.Name, x.FieldType, x.Value, ref oUserFields);
                    });
                }

                lines = oGoodReceipt.Lines;
                UserFields oUserFields2 = lines.UserFields;
               
                foreach (var line in _goodsReceipt.Lines)
                {
                    if (!string.IsNullOrEmpty(line.ItemNameXml))
                    {
                        SetUdfValue("U_DescriptionItemXml", "String", (line.ItemNameXml).ToString(), ref oUserFields2);
                    }                  
                    lines.ItemCode = line.ItemCode;
                    lines.UnitPrice = line.UnitPrice;
                    lines.Quantity = line.Quantity;
                    lines.TaxCode = line.TaxCode;
                    lines.DiscountPercent = line.Discount;
                    lines.WarehouseCode = line.WareHouse;
                    lines.TaxOnly = line.TaxOnly?SAPbobsCOM.BoYesNoEnum.tYES: SAPbobsCOM.BoYesNoEnum.tNO;
                    lines.Add();
                }


                if (!string.IsNullOrEmpty(_goodsReceipt.U_CLVS_POS_UniqueInvId))
                    oGoodReceipt.UserFields.Fields.Item("U_CLVS_POS_UniqueInvId").Value = _goodsReceipt.U_CLVS_POS_UniqueInvId;



                if (lines != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(lines);
                }

                if (oGoodReceipt.Add() != 0)
                {
                    string sapErrMessage = _company.GetLastErrorCode() + " - " + _company.GetLastErrorDescription();
                    LogManager.LogMessage("DIAPI/PostDiapiData/CreateGoodReceipt-- Error al crear la entrada de inventario , sapErrMessage: " + sapErrMessage, (int)Constants.LogTypes.General);
                    throw new Exception(_company.GetLastErrorCode() + _company.GetLastErrorDescription());
                }

                int docEntry = Convert.ToInt32(_company.GetNewObjectKey());



                if (lines != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(lines);
                }

                if (oGoodReceipt != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oGoodReceipt);
                }

                return new ItemsResponse
                {
                    Result = true,
                    DocEntry = docEntry
                     
                };

            }
            catch
            {
                if (lines != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(lines);
                }

                if (lines != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(lines);
                }

                if (oGoodReceipt != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oGoodReceipt);
                }

                if (oGoodReceipt != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oGoodReceipt);
                }

                throw;
            }
        }

        /// <summary>
        /// Funcion SDK que se encarga de crear socio en SAP 
        /// </summary>
        /// <param name="Customer"></param>
        /// <returns></returns>
        public static BaseResponse CreateCustomer(GetCustomerModel Customer, CredentialHolder _UserCredentials, string userId)
        {
            DAO.SuperV2_Entities db = new SuperV2_Entities();
            Company oCompany = DIAPICommon.CreateCompanyObject(_UserCredentials);
            SAPbobsCOM.BusinessPartners oBusinessPartner = null;
            UserFields udfBusinessPartners = null;
            oBusinessPartner = (SAPbobsCOM.BusinessPartners)oCompany.GetBusinessObject(BoObjectTypes.oBusinessPartners);
            udfBusinessPartners = oBusinessPartner.UserFields;
            int SerieType = 0;
            try
            {
                switch (Customer.CardType)
                {
                    case "C":
                        oBusinessPartner.CardType = SAPbobsCOM.BoCardTypes.cCustomer;
                        SerieType = (int)COMMON.Constants.SerialType.Customer;
                        break;
                    case "S":
                        oBusinessPartner.CardType = SAPbobsCOM.BoCardTypes.cSupplier;
                        SerieType = (int)COMMON.Constants.SerialType.Vendor;
                        break;
                }

                UserAssign userA = db.UserAssign.Where(x => x.UserId == userId).FirstOrDefault();
                DiapiSerie diapiSeries = userA.V_SeriesByUser.Where(x => x.V_Series.DocType == SerieType && x.V_Series.Type == (int)COMMON.Constants.SerieNumberingType.Online).Select(x => new DiapiSerie
                {
                    NumType = x.V_Series.Numbering,
                    Serie = x.V_Series.Serie
                }).FirstOrDefault();

                if (diapiSeries == null)
                {
                    throw new Exception(string.Format("No se ha definido una serie para crear Socio de Negocios"));
                }

                oBusinessPartner.Series = diapiSeries.Serie;
                oBusinessPartner.CardName = Customer.CardName;
                oBusinessPartner.FederalTaxID = Customer.LicTradNum;
                oBusinessPartner.Phone1 = Customer.Phone1;
                udfBusinessPartners.Fields.Item("E_Mail").Value = Customer.E_Mail;
                udfBusinessPartners.Fields.Item("U_TipoIdentificacion").Value = Customer.U_TipoIdentificacion;
                udfBusinessPartners.Fields.Item("U_provincia").Value = Customer.U_provincia;
                udfBusinessPartners.Fields.Item("U_canton").Value = Customer.U_canton;
                udfBusinessPartners.Fields.Item("U_distrito").Value = Customer.U_distrito;
                udfBusinessPartners.Fields.Item("U_barrio").Value = Customer.U_barrio;
                udfBusinessPartners.Fields.Item("U_direccion").Value = Customer.U_direccion;

                if (Customer.UdfTarget != null)
                {
                    Customer.UdfTarget.ForEach(x =>
                    {
                        SetUdfValue(x.Name, x.FieldType, x.Value, ref udfBusinessPartners);
                    });
                }

                if (udfBusinessPartners != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(udfBusinessPartners);
                }

                // se trata de guardar el cambio en la BD
                if (oBusinessPartner.Add() != 0)
                {
                    string sapErrMessage = oCompany.GetLastErrorCode() + " - " + oCompany.GetLastErrorDescription();
                    throw new Exception(sapErrMessage);
                }
                if (udfBusinessPartners != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(udfBusinessPartners);
                }
                if (oBusinessPartner != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oBusinessPartner);
                }
                if (oBusinessPartner != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oBusinessPartner);
                }

                return new BaseResponse
                {
                    Result = true
                };
            }
            catch (Exception ex)
            {
                int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
                string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;
                string ErrMsj = "-" + Convert.ToString("SAPDAO/PostSapData/CreateCustomer Catch-- Code: " + code + " Message: " + message);
                LogManager.LogMessage(ErrMsj, (int)Constants.LogTypes.SAP);

                throw;
            }

        }

        /// <summary>
        /// Funcion SDK que se encarga de Actualizar una entrega en SAP 
        /// </summary>
        /// <param name="Customer"></param>
        /// <returns></returns>
        public static BaseResponse UpdateCustomer(GetCustomerModel Customer, CredentialHolder _UserCredentials)
        {
            Company oCompany = DIAPICommon.CreateCompanyObject(_UserCredentials);
            SAPbobsCOM.BusinessPartners oBusinessPartner = null;
            UserFields udfBusinessPartners = null;
            oBusinessPartner = (SAPbobsCOM.BusinessPartners)oCompany.GetBusinessObject(BoObjectTypes.oBusinessPartners);
            udfBusinessPartners = oBusinessPartner.UserFields;

            try
            {
                if (!oBusinessPartner.GetByKey(Customer.CardCode))
                {
                    throw new Exception("El CardCode del Socio es incorrecto");
                }

                switch (Customer.CardType)
                {
                    case "C":
                        oBusinessPartner.CardType = SAPbobsCOM.BoCardTypes.cCustomer;
                        break;
                    case "S"
:
                        oBusinessPartner.CardType = SAPbobsCOM.BoCardTypes.cSupplier;
                        break;
                }

                oBusinessPartner.FederalTaxID = Customer.LicTradNum;
                oBusinessPartner.CardName = Customer.CardName;
                oBusinessPartner.Phone1 = Customer.Phone1;
                udfBusinessPartners.Fields.Item("E_Mail").Value = Customer.E_Mail;
                udfBusinessPartners.Fields.Item("U_TipoIdentificacion").Value = Customer.U_TipoIdentificacion;
                udfBusinessPartners.Fields.Item("U_provincia").Value = Customer.U_provincia;
                udfBusinessPartners.Fields.Item("U_canton").Value = Customer.U_canton;
                udfBusinessPartners.Fields.Item("U_distrito").Value = Customer.U_distrito;
                udfBusinessPartners.Fields.Item("U_barrio").Value = Customer.U_barrio;
                udfBusinessPartners.Fields.Item("U_direccion").Value = Customer.U_direccion;

                if (Customer.UdfTarget != null)
                {
                    Customer.UdfTarget.ForEach(x =>
                    {
                        SetUdfValue(x.Name, x.FieldType, x.Value, ref udfBusinessPartners);
                    });
                }

                if (udfBusinessPartners != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(udfBusinessPartners);
                }

                // se trata de guardar el cambio en la BD
                if (oBusinessPartner.Update() != 0)
                {
                    string sapErrMessage = oCompany.GetLastErrorCode() + " - " + oCompany.GetLastErrorDescription();
                    throw new Exception(sapErrMessage);
                }
                if (udfBusinessPartners != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(udfBusinessPartners);
                }
                if (oBusinessPartner != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oBusinessPartner);
                }
                if (oBusinessPartner != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oBusinessPartner);
                }

                return new BaseResponse
                {
                    Result = true
                };
            }
            catch (Exception ex)
            {
                throw;
               
            }

        }

        //Metodo para crear una factura de proveedor en SAP
        //recive como parametro un Objeto de tipo Factura y el objeto company para poder conectarce.
        public InvoiceSapResponse CreateapInvoice(CreateInvoice CreateapInvoice, string DBName, string userId, string FatherCard, ref Company oCompany)
        {
            var startTime = DateTime.Now;
            LogManager.LogMessage(string.Format("PostDIAPIData>CreateapInvoice. Start Time: {0}", startTime), 2);

            var CreateInvoiceToString = new JavaScriptSerializer().Serialize(CreateapInvoice);
            LogManager.LogMessage("DIAPI/PostDiapiData/CreateapInvoice-- Objeto recibido canPay: " + CreateInvoiceToString + ", company: " + DBName + ", userId: " + userId + ", FatherCard: " + FatherCard, (int)Constants.LogTypes.SAP);

            int DocEntry = 0;

            PaymentSapResponse paymentResponse = new PaymentSapResponse();

            try
            {
                DAO.SuperV2_Entities db = new SuperV2_Entities();
                DateTime DocDate = DateTime.Now;
                Documents apInv = null;

                apInv = (Documents)oCompany.GetBusinessObject(BoObjectTypes.oPurchaseInvoices);

                int SerieType = (int)COMMON.Constants.SerialType.ApInvoice;

                UserAssign userA = db.UserAssign.Where(x => x.UserId == userId).FirstOrDefault();

                DiapiSerie diapiSeries = userA.V_SeriesByUser.Where(x => x.V_Series.DocType == SerieType && x.V_Series.Type == (int)COMMON.Constants.SerieNumberingType.Online).Select(x => new DiapiSerie
                {
                    NumType = x.V_Series.Numbering,
                    Serie = x.V_Series.Serie
                }).FirstOrDefault();

                if (diapiSeries == null)
                {
                    throw new Exception(string.Format("No se ha definido una serie para crear factura proveedor Online"));
                }

                apInv.Series = diapiSeries.Serie;
                apInv.PaymentGroupCode = Convert.ToInt32(CreateapInvoice.Invoice.PaymentGroupCode);
                apInv.CardCode = CreateapInvoice.Invoice.CardCode;
                apInv.CardName = CreateapInvoice.Invoice.CardName;
                apInv.DocDate = DateTime.Now;
                apInv.Comments = CreateapInvoice.Invoice.Comments;
                apInv.DocType = BoDocumentTypes.dDocument_Items;//es de articulos, consultar si se van a enviar de servicio
                apInv.DocCurrency = CreateapInvoice.Invoice.DocCurrency;
                apInv.SalesPersonCode = CreateapInvoice.Invoice.SalesPersonCode;
                if (!string.IsNullOrEmpty(CreateapInvoice.Invoice.U_ClaveFe))
                {
                    apInv.UserFields.Fields.Item("U_ClaveFE").Value = CreateapInvoice.Invoice.U_ClaveFe;
                }
                if (!string.IsNullOrEmpty(CreateapInvoice.Invoice.U_NumFE))
                {
                    apInv.UserFields.Fields.Item("U_NumFE").Value = CreateapInvoice.Invoice.U_NumFE;

                }
                if (!string.IsNullOrEmpty(CreateapInvoice.Invoice.CLVS_POS_UniqueInvId))
                {
                    apInv.UserFields.Fields.Item("U_CLVS_POS_UniqueInvId").Value = CreateapInvoice.Invoice.CLVS_POS_UniqueInvId;

                }
                if (CreateapInvoice.Invoice.FEInfo != null)
                {
                    var FEInfo = CreateapInvoice.Invoice.FEInfo;

                    if (FEInfo.IdType != string.Empty)
                    {
                        if (int.Parse(FEInfo.IdType) != 0 && int.Parse(FEInfo.IdType) != 99)
                        {
                            AssignUserDefinedField(FEInfo.IdType, "U_TipoIdentificacion", ref apInv);
                            AssignUserDefinedField(FEInfo.Identification, "U_NumIdentFE", ref apInv);
                            AssignUserDefinedField(FEInfo.Email, "U_CorreoFE", ref apInv);
                            //AssignUserDefinedField(FEInfo.Provincia, "U_Provincia", ref oInv);
                            //AssignUserDefinedField(FEInfo.Canton, "U_Canton", ref oInv);
                            //AssignUserDefinedField(FEInfo.Distrito, "U_Distrito", ref oInv);
                            //AssignUserDefinedField(FEInfo.Barrio, "U_Barrio", ref oInv);
                            //AssignUserDefinedField(FEInfo.Direccion, "U_Direccion", ref oInv);
                        }
                    }
                }

                if (CreateapInvoice.Invoice.UdfTarget != null)
                {
                    UserFields oUserFields = apInv.UserFields;

                    CreateapInvoice.Invoice.UdfTarget.ForEach(x =>
                    {
                        SetUdfValue(x.Name, x.FieldType, x.Value, ref oUserFields);
                    });
                }
                foreach (var line in CreateapInvoice.Invoice.InvoiceLinesList)
                {
                    apInv.Lines.ItemCode = line.ItemCode;
                    apInv.Lines.Quantity = line.Quantity;
                    apInv.Lines.UnitPrice = line.UnitPrice;
                    apInv.Lines.DiscountPercent = line.DiscountPercent;
                    apInv.Lines.TaxCode = line.TaxCode;
                    apInv.Lines.WarehouseCode = line.WarehouseCode;
                    apInv.Lines.Add();
                }

                if (apInv.Add() != 0)
                {
                    string sapErrMessage = oCompany.GetLastErrorCode() + " - " + oCompany.GetLastErrorDescription();
                    string ModelObject = "Referencia CardCode #" + CreateapInvoice.Invoice.CardCode;
                    LogManager.LogMessage("DIAPI/PostDiapiData/CreateapInvoice-- Error al actualizar ARInvoice, sapErrMessage: " + sapErrMessage + ", ModelObject" + ModelObject, (int)Constants.LogTypes.SAP);
                    throw new Exception(sapErrMessage + ModelObject);
                }

                DocEntry = Convert.ToInt32(oCompany.GetNewObjectKey());

                if (DocEntry == 0)
                {
                    throw new Exception("DocEntry == 0 | " + oCompany.GetLastErrorCode() + " - " + oCompany.GetLastErrorDescription());
                }


                LogManager.LogMessage(string.Format("PostDIAPIData>CreateapInvoice. End Time: {0}| Total Time Time: {1}", DateTime.Now, DateTime.Now - startTime), (int)Constants.LogTypes.API);


                return new InvoiceSapResponse
                {
                    Result = true,
                    DocEntry = DocEntry,
                    PaymentResponse = paymentResponse
                };
            }
            catch (Exception ex)
            {
                int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
                string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;
                string ErrMsj = "-" + Convert.ToString("DIAPI/PostDiapiData/CreateapInvoice Catch-- Code: " + code + " Message: " + message);
                LogManager.LogMessage(ErrMsj, (int)Constants.LogTypes.SAP);
                if (oCompany != null)
                {
                    if (oCompany.Connected)
                    {
                        oCompany.Disconnect();
                    }
                }
                return new InvoiceSapResponse
                {
                    Result = false,
                    DocEntry = DocEntry,
                    Error = new ErrorInfo
                    {
                        Code = code,
                        Message = message
                    }
                };
            }
        }
        //Crea se encarga de crear el pago de la facture an sap
        //Recibe como parametro el docentry de la factura de SAP y el objeto company para poder conectarce, el modelo del pago el Id de usuario y en el caso de necesitar el Father Card
        // en caso de clientes que cuenten con una cuenta padre.
        public PaymentSapResponse CreateApPayment(CreatePaymentModel payment, string DBName, string user, string FatherCard, ref Company oCompany)
        {
            var startTime = DateTime.Now;
            LogManager.LogMessage(string.Format("PostDIAPIData>CreateApPayment. Start Time: {0}", startTime), 2);

            var paymentToString = new JavaScriptSerializer().Serialize(payment);
            LogManager.LogMessage("DIAPI/PostDiapiData/CreateApPayment-- Objeto recibido payment: " + paymentToString + ", company: " + DBName + ", user: " + user + ", FatherCard: " + FatherCard, (int)Constants.LogTypes.SAP);

            int DocEntry = 0;
            int DocNum = 0;
            double ccSum = 0;
            double checkSum = 0;
            try
            {

               



                DAO.SuperV2_Entities db = new SuperV2_Entities();
                DateTime DocDate = DateTime.Today;
                SAPbobsCOM.Payments oPay = null;

                oPay = (SAPbobsCOM.Payments)oCompany.GetBusinessObject(BoObjectTypes.oVendorPayments);
                int SerieType = (int)COMMON.Constants.SerialType.OutgoinPayment;
                UserAssign userA = db.UserAssign.Where(x => x.UserId == user).FirstOrDefault();

                DiapiSerie diapiSeries = userA.V_SeriesByUser.Where(x => x.V_Series.DocType == SerieType).Select(x => new DiapiSerie
                {
                    NumType = x.V_Series.Numbering,
                    Serie = x.V_Series.Serie
                }).FirstOrDefault();

                if (diapiSeries == null)
                {
                    throw new Exception(string.Format("No se ha definido una serie para crear pagos a proveedores"));
                }


                if (oPay.Invoices != null && oPay.Invoices.Count > 0)
                {
                    oPay.DocType = BoRcptTypes.rSupplier;
                }
                else
                {
                    oPay.DocType = BoRcptTypes.rAccount;
                }
                // oPay.DocType = BoRcptTypes.rAccount;// G/L account is not valid  [VPM4.AcctCode]
                oPay.Series = diapiSeries.Serie;
                oPay.DocCurrency = payment.DocCurrency;
                oPay.CardCode = !string.IsNullOrEmpty(FatherCard) ? FatherCard : payment.CardCode;
                oPay.DocDate = DateTime.Now;
                oPay.DueDate = DateTime.Now;
                oPay.CounterReference = payment.CounterReference;
                oPay.Remarks = payment.Remarks;

                if (payment.CashSum != 0)
                {
                    decimal change = 0;
                    if (payment.PaymentInvoices.Count > 0)
                    {
                        change = payment.PaymentInvoices[0].Change <= 0 ? 0 : payment.PaymentInvoices[0].Change;
                    }
                    else
                    {
                       // change = Convert.ToDecimal(payment.CashSum) - Convert.ToDecimal(payment.Total);
                    }
                    oPay.CashSum = Convert.ToDouble(payment.CashSum) - Convert.ToDouble(change);
                    oPay.CashAccount = payment.CashAccount;
                }

                if (payment.V_Checks != null && payment.V_Checks.Count > 0)
                {
                    foreach (var checks in payment.V_Checks)
                    {
                        oPay.CheckAccount = checks.CheckAccount;
                        oPay.Checks.AccounttNum = checks.AcctNum;
                        oPay.Checks.BankCode = checks.BankCode;
                        oPay.Checks.CountryCode = checks.CountryCode;
                        oPay.Checks.CheckNumber = Convert.ToInt32(checks.CheckNumber);
                        oPay.Checks.CheckSum = Convert.ToDouble(checks.CheckSum);
                        oPay.Checks.DueDate = Convert.ToDateTime(checks.DueDate);
                        oPay.DocCurrency = checks.Curr;
                        oPay.Checks.Add();
                        checkSum += Convert.ToDouble(checks.CheckSum);
                    }
                }

                if (payment.PaymentCreditCards != null && payment.PaymentCreditCards.Count > 0)
                {
                    foreach (var cc in payment.PaymentCreditCards)
                    {
                        //-- Codigo que ya estaba
                        oPay.CreditCards.CreditSum = Convert.ToDouble(cc.CreditSum);
                        string[] cardnumarray = cc.FormatCode.Split(' ');
                        oPay.CreditCards.CreditCard = Convert.ToInt32(cardnumarray[0]);
                        oPay.CreditCards.CreditCardNumber = Convert.ToString(cc.CreditCardNumber);

                        string fecha = cc.CardValid;
                        string[] array = fecha.Split('/');
                        var lastDayOfMonth = DateTime.DaysInMonth(Convert.ToInt32("20" + array[1]), Convert.ToInt32(array[0]));
                        var day = Convert.ToInt32(lastDayOfMonth);
                        DateTime nuevafecha = new DateTime(Convert.ToInt32("20" + array[1]), Convert.ToInt32(array[0]), day);

                         oPay.CreditCards.CardValidUntil = Convert.ToDateTime(nuevafecha);
                        // oPay.CreditCards.OwnerIdNum = cc.OwnerIdNum;
                        // oPay.BankChargeAmount = 20;
                        oPay.CreditCards.CreditAcct = cc.CreditAcct;
                        oPay.CreditCards.VoucherNum = cc.VoucherNum;

                        oPay.CreditCards.Add();
                        ccSum += Convert.ToDouble(cc.CreditSum);

                        //------------------ Codigo de prueba



                        //----------------- codigo utilizado en pago normal
                        //oPay.CreditCards.CreditSum = Convert.ToDouble(cc.CreditSum);
                        //string[] cardnumarray = cc.FormatCode.Split(' ');
                        //oPay.CreditCards.CreditCard = Convert.ToInt32(cardnumarray[0]);
                        //oPay.CreditCards.CreditCardNumber = Convert.ToString(cc.CreditCardNumber);

                        //string fecha = cc.CardValid;
                        //string[] array = fecha.Split('/');
                        //var lastDayOfMonth = DateTime.DaysInMonth(Convert.ToInt32("20" + array[1]), Convert.ToInt32(array[0]));
                        //var day = Convert.ToInt32(lastDayOfMonth);
                        //DateTime nuevafecha = new DateTime(Convert.ToInt32("20" + array[1]), Convert.ToInt32(array[0]), day);

                        //oPay.CreditCards.CardValidUntil = Convert.ToDateTime(nuevafecha);
                        //oPay.CreditCards.OwnerIdNum = cc.OwnerIdNum;
                        //oPay.CreditCards.VoucherNum = cc.VoucherNum;

                        //UserFields oUserFields = oPay.CreditCards.UserFields;
                        //SetUdfValue("U_ManualEntry", "String", cc.U_ManualEntry, ref oUserFields);
                        //oPay.CreditCards.Add();
                        //ccSum += Convert.ToDouble(cc.CreditSum);
                    }
                }

                if (payment.TransferSum > 0)
                {
                    oPay.TransferSum = Convert.ToDouble(payment.TransferSum);
                    oPay.TransferAccount = Convert.ToString(payment.TransferAccount);
                    oPay.TransferDate = Convert.ToDateTime(payment.TransferDate);
                    oPay.TransferReference = Convert.ToString(payment.TransferReference);
                }

                if (payment.PaymentInvoices.Count>0)
                {
                    foreach (var lines in payment.PaymentInvoices)
                    {
                        oPay.Invoices.DocEntry = lines.DocEntry;
                        oPay.Invoices.InvoiceType = BoRcptInvTypes.it_PurchaseInvoice;
                        double totalPay = (Convert.ToDouble(lines.SumApplied));// + Convert.ToDouble(ccSum) + Convert.ToDouble(checkSum) + Convert.ToDouble(payment.transfSum)) - Convert.ToDouble((lines.Change <= 0 ? 0 : lines.Change));

                        if (payment.DocCurrency == "COL")
                        {
                            oPay.Invoices.SumApplied = totalPay;
                            //  oPay.Invoices.AppliedFC = totalPay;
                            oPay.Invoices.AppliedFC = totalPay * Convert.ToDouble(payment.DocRate);//lines.InvTotal;                              
                        }
                        else
                        {
                            oPay.Invoices.AppliedFC = totalPay;
                            //oPay.Invoices.AppliedFC = totalPay;
                            oPay.Invoices.SumApplied = totalPay / Convert.ToDouble(payment.DocRate);// lines.InvTotal; //totalPay / Convert.ToDouble(payment.DocRate);
                        }
                        // oPay.UserFields.Fields.Item("U_MontoRecibido").Value = lines.U_MontoRecibido.ToString();
                        oPay.Invoices.Add();
                    }
                }

                if (payment.UdfTarget != null)
                {
                    UserFields oUserFields = oPay.UserFields;

                    payment.UdfTarget.ForEach(x =>
                    {
                        SetUdfValue(x.Name, x.FieldType, x.Value, ref oUserFields);
                    });
                }
                if (oPay.Add() != 0)
                {
                    string sapErrMessage = oCompany.GetLastErrorCode() + " - " + oCompany.GetLastErrorDescription();
                    string ModelObject = "Referencia CardCode #" + payment.CardCode;
                    LogManager.LogMessage("DIAPI/PostDiapiData/CreateApPayment-- Error al actualizar Pago, sapErrMessage: " + sapErrMessage + ", ModelObject" + ModelObject, (int)Constants.LogTypes.General);
                    throw new Exception(oCompany.GetLastErrorCode() + oCompany.GetLastErrorDescription() + "  Proveedor:" + payment.CardCode);
                }
                DocEntry = Convert.ToInt32(oCompany.GetNewObjectKey());

                LogManager.LogMessage(string.Format("                           PostDIAPIData>CreatePayment. End Time: {0}| Total Time Time: {1}", DateTime.Now, DateTime.Now - startTime), (int)Constants.LogTypes.API);

                return new PaymentSapResponse
                {
                    Result = true,
                    DocEntry = DocEntry,
                    DocNum = DocNum,
                    Error = null
                };
            }
            catch (Exception ex)
            {
                int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
                string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;
                string ErrMsj = "-" + Convert.ToString("DIAPI/PostDiapiData/CreateApPayment Catch-- Code: " + code + " Message: " + message);
                LogManager.LogMessage(ErrMsj, (int)Constants.LogTypes.SAP);

                string sapErrMessage = oCompany.GetLastErrorCode() + oCompany.GetLastErrorDescription();
                //if (oCompany != null)
                //{
                //    if (oCompany.Connected)
                //    {
                //        oCompany.Disconnect();
                //    }
                //}



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

        public ItemsResponse CreateGoodsReceiptStock(GoodsReceipt _goodsReceipt, ref Company _company)
        {
            Document_Lines lines = null;
            SAPbobsCOM.Documents oInventoryEntry = null;
            try
            {
                DateTime DocDate = DateTime.Now;
                oInventoryEntry = (SAPbobsCOM.Documents)_company.GetBusinessObject(BoObjectTypes.oInventoryGenEntry);

                // oGoodReceipt.CardCode = _businessParter.CardCode;

                oInventoryEntry.DocDate = DocDate;
                oInventoryEntry.PaymentGroupCode = _goodsReceipt.PriceList;

                if (!String.IsNullOrEmpty(_goodsReceipt.Comments))
                {
                    oInventoryEntry.Comments = _goodsReceipt.Comments;
                }
                if (!string.IsNullOrEmpty(_goodsReceipt.UserId))
                {
                    oInventoryEntry.UserFields.Fields.Item("U_User").Value = _goodsReceipt.UserId;
                }

                if(!string.IsNullOrEmpty(_goodsReceipt.U_CLVS_POS_UniqueInvId))
                    oInventoryEntry.UserFields.Fields.Item("U_CLVS_POS_UniqueInvId").Value = _goodsReceipt.U_CLVS_POS_UniqueInvId;

                if (_goodsReceipt.UdfTarget != null)
                {
                    UserFields oUserFields = oInventoryEntry.UserFields;

                    _goodsReceipt.UdfTarget.ForEach(x =>
                    {
                        SetUdfValue(x.Name, x.FieldType, x.Value, ref oUserFields);
                    });
                }

                lines = oInventoryEntry.Lines;

                foreach (var line in _goodsReceipt.Lines)
                {
                    lines.ItemCode = line.ItemCode;
                    lines.UnitPrice = line.UnitPrice;
                    lines.Quantity = line.Quantity;
                    lines.TaxCode = line.TaxCode;
                    lines.WarehouseCode = line.WareHouse;
                    string account = _goodsReceipt.GoodsReceiptAccount;
                    lines.AccountCode = account;

                    lines.Add();
                }

                if (lines != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(lines);
                }
               
                if (oInventoryEntry.Add() != 0)
                {
                    string sapErrMessage = _company.GetLastErrorCode() + " - " + _company.GetLastErrorDescription();
                    LogManager.LogMessage("DIAPI/PostDiapiData/CreateGoodsReceiptStock Error, sapErrMessage: " + sapErrMessage, (int)Constants.LogTypes.General);
                    throw new Exception(_company.GetLastErrorCode() + _company.GetLastErrorDescription());
                }

                int docEntry = Convert.ToInt32(_company.GetNewObjectKey());

                if (lines != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(lines);
                }

                if (oInventoryEntry != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oInventoryEntry);
                }

                if (oInventoryEntry != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oInventoryEntry);
                }

                return new ItemsResponse
                {
                    Result = true,
                    DocEntry = docEntry
                };

            }
            catch
            {
                if (lines != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(lines);
                }

                if (lines != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(lines);
                }

                if (oInventoryEntry != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oInventoryEntry);
                }

                if (oInventoryEntry != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oInventoryEntry);
                }

                throw;
            }
        }

        public ItemsResponse CreateGoodsIssueStock(GoodsReceipt goodsIssue, ref Company _company)
        {
            Document_Lines lines = null;
            SAPbobsCOM.Documents oInventoryExit = null;
            LogManager.LogMessage("                        DIAPI/PostDiapiData/CreateGoodsIssueStock-- Iniciando devolucion de mercaderia, ", (int)Constants.LogTypes.General);
            try
            {
                DateTime DocDate = DateTime.Now;
                oInventoryExit = (SAPbobsCOM.Documents)_company.GetBusinessObject(BoObjectTypes.oInventoryGenExit);

                oInventoryExit.DocDate = DocDate;
                oInventoryExit.PaymentGroupCode = goodsIssue.PriceList;
                if (!String.IsNullOrEmpty(goodsIssue.Comments))
                {
                    oInventoryExit.Comments = goodsIssue.Comments;
                }
                oInventoryExit.Comments = goodsIssue.Comments;
                oInventoryExit.NumAtCard = goodsIssue.NumAtCard;

                if (goodsIssue.UdfTarget != null)
                {
                    UserFields oUserFields = oInventoryExit.UserFields;

                    goodsIssue.UdfTarget.ForEach(x =>
                    {
                        SetUdfValue(x.Name, x.FieldType, x.Value, ref oUserFields);
                    });
                }
                lines = oInventoryExit.Lines;

                foreach (var line in goodsIssue.Lines)
                {
                    lines.ItemCode = line.ItemCode;
                    lines.UnitPrice = line.UnitPrice;
                    lines.Quantity = line.Quantity;
                    lines.TaxCode = line.TaxCode;
                    lines.WarehouseCode = line.WareHouse;
                    lines.AccountCode = goodsIssue.GoodsReceiptAccount;
                    lines.Add();
                }


                if (!string.IsNullOrEmpty(goodsIssue.U_CLVS_POS_UniqueInvId))
                    oInventoryExit.UserFields.Fields.Item("U_CLVS_POS_UniqueInvId").Value = goodsIssue.U_CLVS_POS_UniqueInvId;



                if (lines != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(lines);
                }

                if (oInventoryExit.Add() != 0)
                {
                    string sapErrMessage = _company.GetLastErrorCode() + " - " + _company.GetLastErrorDescription();
                    LogManager.LogMessage("                        DIAPI/PostDiapiData/CreateGoodsIssueStock-- Error al crear la devolucion de mercaderia , sapErrMessage: " + sapErrMessage, (int)Constants.LogTypes.General);
                    throw new Exception(_company.GetLastErrorCode() + _company.GetLastErrorDescription());
                }

                int docEntry = Convert.ToInt32(_company.GetNewObjectKey());

                if (lines != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(lines);
                }

                if (oInventoryExit != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oInventoryExit);
                }

                if (oInventoryExit != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oInventoryExit);
                }

                return new ItemsResponse
                {
                    Result = true,
                    DocEntry = docEntry
                };

            }
            catch
            {
                if (lines != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(lines);
                }

                if (lines != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(lines);
                }

                if (oInventoryExit != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oInventoryExit);
                }

                if (oInventoryExit != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oInventoryExit);
                }

                throw;
            }
        }

        public PurchaserOrderResponse CreatePurchaseOrder(PurchaseOrderModel _purchaseOrderModel, ref Company _company)
        {
            int DocEntry = 0;
            Document_Lines lines = null;
            SAPbobsCOM.Documents oPurchaseOrder = null;
            try
            {
                DateTime DocDate = DateTime.Now;
                oPurchaseOrder = (SAPbobsCOM.Documents)_company.GetBusinessObject(BoObjectTypes.oPurchaseOrders);

                oPurchaseOrder.CardCode = _purchaseOrderModel.BusinessPartner.CardCode;
                oPurchaseOrder.DocDate = DocDate;
                if (!String.IsNullOrEmpty(_purchaseOrderModel.Comments))
                {
                    oPurchaseOrder.Comments = _purchaseOrderModel.Comments;
                }
                if (_purchaseOrderModel.UdfTarget != null)
                {
                    UserFields oUserFields = oPurchaseOrder.UserFields;

                    _purchaseOrderModel.UdfTarget.ForEach(x =>
                    {
                        SetUdfValue(x.Name, x.FieldType, x.Value, ref oUserFields);
                    });
                }

                lines = oPurchaseOrder.Lines;

                foreach (var line in _purchaseOrderModel.Lines)
                {
                    lines.ItemCode = line.ItemCode;
                    lines.UnitPrice = line.UnitPrice;
                    lines.Quantity = line.Quantity;
                    lines.WarehouseCode = line.WareHouse;
                    lines.TaxCode = line.TaxCode;
                    lines.DiscountPercent = line.Discount;
                    lines.TaxOnly = line.TaxOnly ? SAPbobsCOM.BoYesNoEnum.tYES : SAPbobsCOM.BoYesNoEnum.tNO;
                    lines.Add();
                }


                if (!string.IsNullOrEmpty(_purchaseOrderModel.U_CLVS_POS_UniqueInvId))
                    oPurchaseOrder.UserFields.Fields.Item("U_CLVS_POS_UniqueInvId").Value = _purchaseOrderModel.U_CLVS_POS_UniqueInvId;

                if (oPurchaseOrder.Add() != 0)
                {
                    string sapErrMessage = _company.GetLastErrorCode() + " - " + _company.GetLastErrorDescription();
                    LogManager.LogMessage("DIAPI/PostDiapiData/CreatePurchaseOrder-- Error al crear la orden de compra , sapErrMessage: " + sapErrMessage, (int)Constants.LogTypes.General);
                    throw new Exception(_company.GetLastErrorCode() + _company.GetLastErrorDescription());
                }
                DocEntry = Convert.ToInt32(_company.GetNewObjectKey());
                if (lines != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(lines);
                }

                if (oPurchaseOrder != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oPurchaseOrder);
                }

                if (oPurchaseOrder != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oPurchaseOrder);
                }

                return new PurchaserOrderResponse
                {

                    Result = true,
                    PurchaseOrder = new PurchaseOrderModel
                    {

                        DocEntry = DocEntry
                    }
                };

            }
            catch (Exception ex)
            {
                if (lines != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(lines);
                }

                if (lines != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(lines);
                }

                if (oPurchaseOrder != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oPurchaseOrder);
                }

                if (oPurchaseOrder != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oPurchaseOrder);
                }

                int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
                string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;
                string ErrMsj = "-" + Convert.ToString("DIAPI/PostDiapiData/CreatePurchaseOrder Catch-- Code: " + code + " Message: " + message);
                LogManager.LogMessage(ErrMsj, (int)Constants.LogTypes.SAP);
                
                throw;
            }
        }

        public object UpdatePurchaseOrder(PurchaseOrderModel _purchaseOrderModel, ref Company _company)
        {
            Document_Lines lines = null;
            SAPbobsCOM.Documents oPurchaseOrder = null;
            try
            {
                DateTime DocDate = DateTime.Now;
                oPurchaseOrder = (SAPbobsCOM.Documents)_company.GetBusinessObject(BoObjectTypes.oPurchaseOrders);

                if (!oPurchaseOrder.GetByKey(_purchaseOrderModel.DocNum))
                {
                    throw new Exception($"No se ha encontrado la orden de compra {_purchaseOrderModel.DocNum}");
                }

                oPurchaseOrder.DocDate = DocDate;
                if (!String.IsNullOrEmpty(_purchaseOrderModel.Comments))
                {
                    oPurchaseOrder.Comments = _purchaseOrderModel.Comments;
                }

                if (_purchaseOrderModel.UdfTarget != null)
                {
                    UserFields oUserFields = oPurchaseOrder.UserFields;

                    _purchaseOrderModel.UdfTarget.ForEach(x =>
                    {
                        SetUdfValue(x.Name, x.FieldType, x.Value, ref oUserFields);
                    });
                }
                lines = oPurchaseOrder.Lines;

                int totalLines = lines.Count - 1;

                for (int c = totalLines; c >= 0; c--)
                {
                    lines.SetCurrentLine(c);
                    lines.Delete();
                }

                foreach (var line in _purchaseOrderModel.Lines)
                {
                    lines.TaxOnly = line.TaxOnly ? SAPbobsCOM.BoYesNoEnum.tYES : SAPbobsCOM.BoYesNoEnum.tNO;
                    lines.ItemCode = line.ItemCode;
                    lines.UnitPrice = line.UnitPrice;
                    lines.Quantity = line.Quantity;
                    lines.WarehouseCode = line.WareHouse;
                    lines.TaxCode = line.TaxCode;
                    lines.DiscountPercent = line.Discount;
                    lines.Add();
                }

                if (oPurchaseOrder.Update() != 0)
                {
                    string sapErrMessage = _company.GetLastErrorCode() + " - " + _company.GetLastErrorDescription();
                    LogManager.LogMessage("DIAPI/PostDiapiData/UpdatePurchaseOrder-- Error al actualizar la orden de compra , sapErrMessage: " + sapErrMessage, (int)Constants.LogTypes.General);
                    throw new Exception(_company.GetLastErrorCode() + _company.GetLastErrorDescription());
                }

                if (lines != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(lines);
                }

                if (oPurchaseOrder != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oPurchaseOrder);
                }

                if (oPurchaseOrder != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oPurchaseOrder);
                }


                return new PurchaserOrderResponse
                {

                    Result = true,
                    PurchaseOrder = new PurchaseOrderModel
                    {

                        DocNum = _purchaseOrderModel.DocNum
                    }
                };

            }
            catch (Exception ex)
            {
                if (lines != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(lines);
                }

                if (lines != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(lines);
                }

                if (oPurchaseOrder != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oPurchaseOrder);
                }

                if (oPurchaseOrder != null)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oPurchaseOrder);
                }

                int code = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.HResult : ex.InnerException.HResult : ex.HResult;
                string message = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;
                string ErrMsj = "-" + Convert.ToString("DIAPI/PostDiapiData/UpdatePurchaseOrder Catch-- Code: " + code + " Message: " + message);
                LogManager.LogMessage(ErrMsj, (int)Constants.LogTypes.SAP);

                throw;
            }
        }
        #region Paydesk
        public static BaseResponse CreateCashflow(CashflowModel cashflow, CredentialHolder _UserCredentials)
        {
            Company oCompany = null;
            UserTable oUdt = null;
            UserFields oUdtUdf = null;
            try
            {
                oCompany = DIAPICommon.CreateCompanyObject(_UserCredentials);
                cashflow.UserSignature = oCompany.UserSignature;
                oUdt = oCompany.UserTables.Item("CASHFLOW");
                oUdtUdf = oUdt.UserFields;
                oUdt.Code = $"USR{cashflow.UserSignature}_R{cashflow.Type}_{DateTime.Now:yyyyMMddHHmmss}";
                oUdt.Name = $"USR{cashflow.UserSignature}_R{cashflow.Type}_{DateTime.Now:yyyyMMddHHmmss}";
                oUdtUdf.Fields.Item("U_INTERNAL_K").Value = cashflow.UserSignature;
                oUdtUdf.Fields.Item("U_CreationDate").Value = DateTime.Now;
                oUdtUdf.Fields.Item("U_Amount").Value = cashflow.Amount;
                if (!string.IsNullOrEmpty(cashflow.Type)) oUdtUdf.Fields.Item("U_Type").Value = cashflow.Type;
                if (!string.IsNullOrEmpty(cashflow.Reason)) oUdtUdf.Fields.Item("U_Reason").Value = cashflow.Reason;
                if (!string.IsNullOrEmpty(cashflow.Details)) oUdtUdf.Fields.Item("U_Details").Value = cashflow.Details;

                if (oUdt.Add() != 0) throw new Exception($"--{oCompany.GetLastErrorCode()} --{oCompany.GetLastErrorDescription()}");

                return new BaseResponse { Result = true };
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (oUdtUdf != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUdtUdf);
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oUdtUdf);
                }
                if (oUdt != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oUdt);
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(oUdt);
                }

                if (oCompany != null)
                {
                    if (oCompany.Connected) oCompany.Disconnect();

                    oCompany = null;
                }
            }
        }
        #endregion
    }
}
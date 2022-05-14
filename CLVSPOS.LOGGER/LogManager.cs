using log4net.Config;
using CLVSPOS.COMMON;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CLVSPOS.MODELS;
using System.Reflection;



namespace CLVSPOS.LOGGER
{
    public class LogManager
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(LogManager));
        private static DateTime logDate = DateTime.Now;

        private static int ParseExceptionCode(Exception exc) {
            return exc.InnerException != null ? 
                          exc.InnerException.InnerException != null ? 
                             exc.InnerException.InnerException.HResult 
                             : exc.InnerException.HResult 
                          : exc.HResult;
        }

        private static string ParseExceptionMessage(Exception exc)
        {
            return exc.InnerException != null ?
                          exc.InnerException.InnerException != null ?
                             exc.InnerException.InnerException.Message
                             : exc.InnerException.Message
                          : exc.Message;
        }

        //public static object HandleExceptionWithReturn(Exception exc, 
        //                                     string message, 
        //                                     int sourceType, 
        //                                     string overwriteResponseMessage = "",
        //                                     bool overwriteMessageLog = false) {

        //    int excCode = ParseExceptionCode(exc);
        //    string excMessage = ParseExceptionMessage(exc);
        //    string logMessage = string.Empty;

        //    if (overwriteResponseMessage.Length == 0 || overwriteMessageLog)
        //    {
        //        logMessage = message;
        //    }
        //    else {
        //        logMessage = string.Format("{0}-- code: {1} -- message: {2}", message, excCode, excMessage);
        //    }

        //    LogManager.LogMessage(logMessage, sourceType); 
             
        //    BaseResponse response = new BaseResponse
        //    {
        //        result = false,
        //        errorInfo = new ErrorInfo
        //        {
        //            Code = overwriteResponseMessage.Length > 0 ? 1 :  excCode,
        //            Message = overwriteResponseMessage.Length > 0 ? overwriteResponseMessage : logMessage
        //        }
        //    };
           
        //    return response;           
        //}

        public static object HandleExceptionWithReturn(Exception exc, string responseType)
        {

    //        var a = AppDomain.CurrentDomain
    //.GetAssemblies()
    //.SelectMany(x => x.GetTypes())
    //.FirstOrDefault(t => t.Name == responseType);

            int excCode = ParseExceptionCode(exc);
            string excMessage = ParseExceptionMessage(exc);
            string instanceTarget = string.Format("CLVSPOS.MODELS.{0}, CLVSSUPER.MODELS",
                                                          responseType == string.Empty ? "BaseResponse" : responseType);

            Type oType = GetType(instanceTarget, responseType);
            object instance = Activator.CreateInstance(oType);
            
            var errorInfo = new ErrorInfo
            {
                Code = excCode,
                Message = excMessage
            };

            BindResponse(ref instance, errorInfo);

            return instance;
        }

        public static Type GetType(string _typeName, string _className)
        {
            Type type = Type.GetType(_typeName);
            if (type != null) return type;
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).FirstOrDefault(t => t.Name.Equals(_className));
        }


        public static void HandleException(  Exception exc,
                                             string message,
                                             int sourceType,
                                             string overwriteResponseMessage = "",
                                             bool overwriteMessageLog = false)
        {

            int excCode = ParseExceptionCode(exc);
            string excMessage = ParseExceptionMessage(exc);
            string logMessage = string.Empty;

            if (overwriteResponseMessage.Length == 0 || overwriteMessageLog)
            {
                logMessage = message;
            }
            else
            {
                logMessage = string.Format("{0}-- code: {1} -- message: {2}", message, excCode, excMessage);
            }

            LogManager.LogMessage(logMessage, sourceType);   
        }


        //Registra los logs de la aplicacion
        public static void LogMessage(string msg, int LogType)
        {
            int logtype = LogType;
            string date = Convert.ToString(DateTime.Now);
            string Log4NetPath = System.Configuration.ConfigurationManager.AppSettings["Log4NetPath"].ToString();

            switch (logtype)
            {
                case (int)Constants.LogTypes.General:
                    log4net.GlobalContext.Properties["LogFileName"] = @"" + Log4NetPath + "General_" + logDate.Day + "-" + logDate.Month + "-" + logDate.Year;
                    break;
                case (int)Constants.LogTypes.SAP:
                    log4net.GlobalContext.Properties["LogFileName"] = @"" + Log4NetPath + "SAP_" + logDate.Day + "-" + logDate.Month + "-" + logDate.Year;
                    break;
                case (int)Constants.LogTypes.API:
                    log4net.GlobalContext.Properties["LogFileName"] = @"" + Log4NetPath + "API_" + logDate.Day + "-" + logDate.Month + "-" + logDate.Year;
                    break;
                case (int) Constants.LogTypes.STOCK:
                    log4net.GlobalContext.Properties["LogFileName"] = @"" + Log4NetPath + "STOCK_" + logDate.Day + "-" + logDate.Month + "-" + logDate.Year;
                    break;
                case (int)Constants.LogTypes.CompanyActive:
                    log4net.GlobalContext.Properties["LogFileName"] = @"" + Log4NetPath + "CompanyActive_" + logDate.Day + "-" + logDate.Month + "-" + logDate.Year;
                    break;
                default:
                    log4net.GlobalContext.Properties["LogFileName"] = @"" + Log4NetPath + "Default_" + logDate.Day + "-" + logDate.Month + "-" + logDate.Year;
                    break;
            }
            XmlConfigurator.Configure();
            logger.Info(msg);
        }

        public static object HandleExceptionWithReturn(
                                             Exception exc,
                                             string responseType,
                                             string message,
                                             int sourceType,                                             
                                             bool overwriteMessageLog = false)
        {

            int excCode = ParseExceptionCode(exc);
            string excMessage = ParseExceptionMessage(exc);
            string logMessage = string.Empty;
            
            if (overwriteMessageLog)
            {
                logMessage = message;
            }
            else
            {
                logMessage = string.Format("{0}-- code: {1} -- message: {2}", message, excCode, excMessage);
            }

            //LogManager.LogMessage(logMessage, sourceType);
            // CLVSPOS.MODELS.{0} = NameSpace de la clase, CLVSSUPER.MODELS assembly del proyecto
            object instance = Activator.CreateInstance(Type.GetType(string.Format("CLVSPOS.MODELS.{0}, CLVSSUPER.MODELS", responseType == string.Empty ? "BaseResponse" : responseType)));

            var errorInfo = new ErrorInfo
            {
                Code = overwriteMessageLog ? 1 : excCode,
                Message = overwriteMessageLog ? string.Format("{0}{1}Campos inexistentes o con valores incorrectos", logMessage, Environment.NewLine) : logMessage
            };

            BindResponse(ref instance, errorInfo);
            
            return instance;
        }

        private static void BindResponse(ref object instance, ErrorInfo errorInfo) {
            try
            {
                PropertyInfo prop = instance.GetType().GetProperty("Result", BindingFlags.Public | BindingFlags.Instance);

                if (null != prop && prop.CanWrite)
                {
                    prop.SetValue(instance, false, null);
                }

                prop = instance.GetType().GetProperty("Error", BindingFlags.Public | BindingFlags.Instance);

                if (null != prop && prop.CanWrite)
                {
                    prop.SetValue(instance, errorInfo, null);
                }                
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
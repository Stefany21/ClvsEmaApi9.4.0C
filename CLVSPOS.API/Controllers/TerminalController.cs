using System;
using CLVSPOS.COMMON;
using CLVSPOS.LOGGER;
using CLVSPOS.MODELS;
using CLVSPOS.PROCESS;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CLVSPOS.API.Controllers
{
    [Authorize]
    public class TerminalController : ApiController
    {
        /// <summary>
        /// Endpoint que retorna la lista de todos los terminales registrados
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Banks/GetTerminals")]
        [ActionName("GetTerminals")]
        public HttpResponseMessage GetTerminals()
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, Process.GetTerminals());
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                                              (BankResponse)LogManager.HandleExceptionWithReturn(exc, "BankResponse", "api/Banks/GetAccountsBank", (int)Constants.LogTypes.API));
            }
        }
        /// <summary>
        /// Endpoint que retorna la lista de todos los terminales registrados
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Banks/GetTerminalsByUser")]
        [ActionName("GetTerminalsByUser")]
        public HttpResponseMessage GetTerminalsByUser(string userId)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, Process.GetTerminalsByUser(userId));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (BankResponse)LogManager.HandleExceptionWithReturn(exc, "BankResponse", "api/Banks/GetTerminalsByUser", (int)Constants.LogTypes.API));
            }
        }
        [Authorize]
        [HttpPost]
        [Route("api/Banks/UpdateTerminalsByUser")]
        [ActionName("UpdateTerminalsByUser")]
        public HttpResponseMessage UpdateTerminalsByUser(PPTerminalsByUser terminalsByUser)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, Process.UpdateTerminalsByUser(terminalsByUser));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (BankResponse)LogManager.HandleExceptionWithReturn(exc, "BankResponse", "api/Banks/GetTerminalsByUser", (int)Constants.LogTypes.API));
            }
        }
        /// <summary>
        /// Endpoint para obtener una terminal basado en su id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/Banks/GetTerminal")]
        [ActionName("GetTerminal")]
        public HttpResponseMessage GetTerminal(int id)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, Process.GetPPTerminal(id));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (BankResponse)LogManager.HandleExceptionWithReturn(exc, "BankResponse", "api/Banks/GetAccountsBank", (int)Constants.LogTypes.API));
            }
        }
        /// <summary>
        /// Endpoint para crear un terminal
        /// </summary>
        /// <param name="bacTerminal"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Banks/CreateTerminal")]
        [ActionName("CreateTerminal")]
        public HttpResponseMessage CreateTerminal(PPTerminal bacTerminal)
        {
            try
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK, Process.CreateTerminal(bacTerminal));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,
                                              (BankResponse)LogManager.HandleExceptionWithReturn(exc, "BankResponse", "api/Banks/GetAccountsBank", (int)Constants.LogTypes.API));
            }
        }
        /// <summary>
        /// Endopoint para actualizar la info de un terminal
        /// </summary>
        /// <param name="bacTerminal"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/Banks/UpdateTerminal")]
        [ActionName("UpdateTerminal")]
        public HttpResponseMessage UpdateTerminal(PPTerminal bacTerminal)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, Process.UpdateTerminal(bacTerminal));
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                                              (BankResponse)LogManager.HandleExceptionWithReturn(exc, "BankResponse", "api/Banks/GetAccountsBank", (int)Constants.LogTypes.API));
            }
        }
    }
}


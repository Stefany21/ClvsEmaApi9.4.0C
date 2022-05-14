using CLVSPOS.COMMON;
using CLVSPOS.LOGGER;
using CLVSPOS.MODELS;
using CLVSPOS.PROCESS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CLVSPOS.API.Controllers
{
    public class ConnectionsController : ApiController
    {
        [HttpGet]
        [Route("api/Connections/ConnectCompany")]
        [ActionName("ConnectCompany")]
        public HttpResponseMessage ConnectCompany(int MappId)
        {
            try
            {
                Process.ConnectCompany(MappId);
                return Request.CreateResponse(System.Net.HttpStatusCode.OK,"Ok" );
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError,exc.Message);
            }
        }
    }
}

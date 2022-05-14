using Microsoft.Owin.Security.OAuth;
using CLVSPOS.DAO;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Owin.Security;
using Microsoft.Owin;
using System.Web;
using System.Web.Script.Serialization;
using CLVSPOS.MODELS;

namespace CLVSPOS.API.OAuth.Providers
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async System.Threading.Tasks.Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        //se encarga de la validacion y autentificacion por token
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            try
            { 
                GetData getDBData = new GetData();
                IFormCollection parameters = await context.Request.ReadFormAsync();
                var is_login = parameters.Get("is_login");

                if (getDBData.ValidateAccess(context.Password, context.UserName, Convert.ToBoolean(is_login)))
                {
                    LoggedUser user = GetData.SelectLoggedUser(context.UserName, context.Password);

                    PPTerminalsByUserResponse terminalsRPS = GetData.GetPPTerminalsByUser(user.UserId);

                    //var company = PROCESS.Process.GetFavoriteCompany(userId);

                    ClaimsIdentity identity = new ClaimsIdentity(context.Options.AuthenticationType);
                    identity.AddClaim(new Claim("sub", context.UserName));
                    identity.AddClaim(new Claim("role", "user"));
                    identity.AddClaim(new Claim("ClientId", context.UserName));
                    identity.AddClaim(new Claim("userId", user.UserId));
                    identity.AddClaim(new Claim("WhCode", user.WhCode));


                    string serializedTerminal = "{}";
                    string serializedTerminals = "[]";

                    if (terminalsRPS != null && terminalsRPS.PPTerminalsByUser != null && terminalsRPS.PPTerminalsByUser.Count == 1)
                    {
                        PPTerminalResponse terminal = GetData.GetPPTerminal(terminalsRPS.PPTerminalsByUser[0].TerminalId);

                        if (terminal.Result)
                        {
                            serializedTerminal = new JavaScriptSerializer().Serialize(terminal.PPTerminal);
                        }
                    }

                    PPTerminalsResponse terminals = DAO.GetData.GetPPTerminals();

                    if (terminals != null && terminals.PPTerminals != null)
                    {
                        serializedTerminals = new JavaScriptSerializer().Serialize(terminals.PPTerminals);
                    }

                    AuthenticationProperties props = new AuthenticationProperties(new Dictionary<string, string>
                    {
                        { "UserName", context.UserName },
                        { "userId", user.UserId },
                        { "WhCode", user.WhCode },
                        { "WhName", user.WhName },
                        { "PrefixId" , user.PrefixId },
                        { "CompanyKey",COMMON.Common.GetDBObjectByKey(System.Reflection.MethodBase.GetCurrentMethod(),"CompanyKey")},
                        { "AppKey",COMMON.Common.GetDBObjectByKey(System.Reflection.MethodBase.GetCurrentMethod(),"AppKey")},
                        { "Terminal", serializedTerminal },
                        { "Terminals", serializedTerminals }
                    });

                    AuthenticationTicket ticket = new AuthenticationTicket(identity, props);
                    context.Validated(ticket);
                }
                else
                {
                    context.SetError("invalid_grant", "Username or password is incorrect");
                }
            }
            catch (Exception ex)
            {
                string errMsg = ex.InnerException != null ? ex.InnerException.InnerException != null ? ex.InnerException.InnerException.InnerException != null ? ex.InnerException.InnerException.InnerException.Message : ex.InnerException.InnerException.Message : ex.InnerException.Message : ex.Message;
                context.SetError("error", errMsg);
            }
        }
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }
            return Task.FromResult<object>(null);
        }
    }
}
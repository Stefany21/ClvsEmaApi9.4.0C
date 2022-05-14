using Microsoft.Owin.Security.OAuth;
using CLVSSUPER.DAO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Owin.Security;
using Microsoft.Owin;

namespace CLVSSUPER.API.OAuth.Providers
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
                GetData gd = new GetData();
                IFormCollection parameters = await context.Request.ReadFormAsync();
                var is_login = parameters.Get("is_login");

                if (gd.ValidateAccess(context.Password, context.UserName, Convert.ToBoolean(is_login)))
                {
                    var userId = DAO.GetData.GetUserId(context.Password, context.UserName);
                    //var company = PROCESS.Process.GetFavoriteCompany(userId);

                    var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                    identity.AddClaim(new Claim("sub", context.UserName));
                    identity.AddClaim(new Claim("role", "user"));
                    identity.AddClaim(new Claim("ClientId", context.UserName));
                    identity.AddClaim(new Claim("userId", userId));

                    var props = new AuthenticationProperties(new Dictionary<string, string>
                    {
                        { "UserName", context.UserName },
                        { "userId", userId }
                        //{ "companyId", company.CompanyId.ToString() },
                       // { "CompanyName", company.CompanyComercialName }
                    });

                    var ticket = new AuthenticationTicket(identity, props);
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
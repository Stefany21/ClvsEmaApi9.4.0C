﻿using System;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using System.Web;
using CLVSPOS.API.OAuth.Providers;

[assembly: OwinStartup(typeof(CLVSPOS.API.Startup))]

namespace CLVSPOS.API
{
    public class Startup
    {
        // se encarga del proseso de autentificacion y validacion por token
        public void Configuration(IAppBuilder app) { 
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            var config = new HttpConfiguration();
            WebApiConfig.Register(config);
            ConfigureOAuth(app);
            app.UseWebApi(config);
        }

        private void ConfigureOAuth(IAppBuilder appBuilder)
        {
            appBuilder.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            OAuthAuthorizationServerOptions oAuthAuthorizationServerOptions = new OAuthAuthorizationServerOptions()
            {
                TokenEndpointPath = new Microsoft.Owin.PathString("/token"), // token path
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                AllowInsecureHttp = true,
                Provider = new SimpleAuthorizationServerProvider()
            };

            // To create an access token on AppBuilder 
            appBuilder.UseOAuthAuthorizationServer(oAuthAuthorizationServerOptions);

            // We are setting Authentication type as a Bearer Authentication.
            appBuilder.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
            if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            {
                //These headers are handling the "pre-flight" OPTIONS call sent by the browser
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST,PATCH,DELETE");
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Origin, Content-Type, Accept, X-Requested-With");
                HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000");
                HttpContext.Current.Response.End();
            }


        }
    }
}

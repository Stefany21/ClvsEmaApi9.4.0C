using CLVSPOS.MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace CLVSPOS.COMMON
{
    public class Padron
    {
        private static bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        public static async Task<HttpResponseMessage> getToken(List<KeyValuePair<string, string>> listParams, string url)
        {
            try
            {
                var content = new FormUrlEncodedContent(listParams);
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                using (var client = new HttpClient())
                {
                    var response = client.PostAsync(url, content).Result;
                    return response;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public static async Task<HttpResponseMessage> GetBpInfoPadron(string BPIdentification, string access_token)
        {
            try
            {
                string url = System.Configuration.ConfigurationManager.AppSettings["URIGetBPInfo"].ToString();
                url = url + BPIdentification;
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrWhiteSpace(access_token))
                    {
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + access_token);
                    }
                    return client.GetAsync(url).Result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
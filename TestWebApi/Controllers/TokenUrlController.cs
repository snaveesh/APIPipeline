using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;

namespace TestWebApi.Controllers
{
    public class TokenUrlController : ApiController
    {
        public async System.Threading.Tasks.Task<HttpResponseMessage> GetTokenAsync()
        {
            try
            {
                var url = Environment.GetEnvironmentVariable("AzureKeyVaultUrl").ToString();
                HttpResponseMessage httpResponse;
                if(Request.Headers.Authorization.Parameter!=null && Request.Headers.Authorization.Parameter != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, Request.Headers.Authorization.Parameter);
                }
                if (Request.Headers.Contains("ClientID") && Request.Headers.Contains("ClientSecret"))
                {
                    var clientId = Request.Headers.GetValues("ClientID").FirstOrDefault();
                    var clientSecret = Request.Headers.GetValues("ClientSecret").FirstOrDefault();
                    AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
                    KeyVaultClient keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                    var secret = await keyVaultClient.GetSecretAsync(url+"secrets/" + clientId)
                            .ConfigureAwait(false);
                    if (secret.Value == clientSecret)
                    {
                        httpResponse = Request.CreateResponse(HttpStatusCode.OK, Environment.GetEnvironmentVariable("authToken"));
                    }
                    else
                    {
                        httpResponse = Request.CreateResponse(HttpStatusCode.Unauthorized, "Client details invalid!");

                    }
                }
                else
                {
                    httpResponse = Request.CreateResponse(HttpStatusCode.BadRequest, "ClientID and ClientSecret required in the header!");
                }
                return httpResponse;
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}

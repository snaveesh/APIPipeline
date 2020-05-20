using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace TestWebApi.Utilities
{
    public class CustomAuthorization:AuthorizationFilterAttribute
    {
        /// <summary>
        /// Authorizes the api user
        /// </summary>
        /// <param name="actionContext"> current http context</param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            try
            {
                var authToken = "";
                if (ConfigurationManager.AppSettings["IsTestingEnv"].ToString() == "true")
                {
                    authToken = ConfigurationManager.AppSettings["authToken"].ToString();
                }
                else
                {
                    authToken = Environment.GetEnvironmentVariable("authToken");
                }
                if (actionContext.Request.Headers.Authorization != null) 
                {
                    if(actionContext.Request.Headers.Authorization.Parameter== authToken)
                    {
                        base.OnAuthorization(actionContext);
                    }
                    else
                    {
                        actionContext.Response = actionContext.Request.CreateErrorResponse(System.Net.HttpStatusCode.Unauthorized,"Invalid Authorization token!");
                    }
                }
                else
                {
                    actionContext.Response = actionContext.Request.CreateErrorResponse(System.Net.HttpStatusCode.Forbidden,"No Authorization token found!");
                }
            }
            catch(Exception ex)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError,"Something went wrong! Internal server error!");
                return;
            }
        }
    }
}
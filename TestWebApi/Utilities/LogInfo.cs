using log4net;
using Microsoft.ApplicationInsights;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace TestWebApi.Utilities
{
    public class LogInfo:ActionFilterAttribute
    {
        private static TelemetryClient tc =new TelemetryClient();
        private static ILog _logger = LogManager.GetLogger("API logger");
        private DateTime _start;
        /// <summary>
        /// Filter to log before an action starts
        /// </summary>
        /// <param name="actionContext">current http context</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            _start = DateTime.Now;
            _logger.Info("Executing action " + actionContext.Request.Method+" for "+actionContext.Request.RequestUri+" at "+DateTime.Now);
            base.OnActionExecuting(actionContext);
        }
        /// <summary>
        /// filter to log after an action ends
        /// </summary>
        /// <param name="actionExecutedContext">current http context</param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            string key = "";
            if (ConfigurationManager.AppSettings["IsTestingEnv"].ToString() == "true")
            {
                key = ConfigurationManager.AppSettings["InstrumentationKey"].ToString();
            }
            else
            {
                key = Environment.GetEnvironmentVariable("InstrumentationKey");
            }
            tc.InstrumentationKey = key;
            tc.Context.User.Id = Environment.UserName;
            tc.Context.Session.Id = Guid.NewGuid().ToString();
            tc.Context.Device.OperatingSystem = Environment.OSVersion.ToString();
            tc.TrackEvent("Tracking event from customer web api request");
            if (null != actionExecutedContext.Response&& actionExecutedContext.Response.StatusCode!=HttpStatusCode.InternalServerError)
            {
                tc.TrackRequest(actionExecutedContext.Request.RequestUri.AbsoluteUri, _start, DateTime.Now - _start, actionExecutedContext.Response.StatusCode.ToString(), true);

            }
            else
            {
                tc.TrackRequest(actionExecutedContext.Request.RequestUri.AbsoluteUri, _start, DateTime.Now - _start, HttpStatusCode.InternalServerError.ToString(), false);
            }
            tc.Flush();
            _logger.Info("Executed action " + actionExecutedContext.Request.Method + " for " + actionExecutedContext.Request.RequestUri + " at " + DateTime.Now);
            base.OnActionExecuted(actionExecutedContext);
        }
    }
}
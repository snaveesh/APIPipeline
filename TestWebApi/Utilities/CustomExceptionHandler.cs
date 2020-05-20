using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace TestWebApi.Utilities
{
    public class CustomExceptionHandler:ExceptionHandler
    {
        /// <summary>
        /// global Exception handler
        /// </summary>
        /// <param name="context">current context</param>
        public override void Handle(ExceptionHandlerContext context)
        {
            var result = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(context.Exception.Message),
                ReasonPhrase = "Exception"
            };

            context.Result = new ErrorMessageResult(context.Request, result);
        }

        public class ErrorMessageResult : IHttpActionResult
        {
            private HttpRequestMessage _request;
            private readonly HttpResponseMessage _httpResponseMessage;
            /// <summary>
            /// Method to create error message
            /// </summary>
            /// <param name="request">current request</param>
            /// <param name="httpResponseMessage">http reponse message</param>
            public ErrorMessageResult(HttpRequestMessage request, HttpResponseMessage httpResponseMessage)
            {
                _request = request;
                _httpResponseMessage = httpResponseMessage;
            }

            /// <summary>
            /// Method of IHttpActionResult which needs to be implemented
            /// </summary>
            /// <param name="cancellationToken"></param>
            /// <returns></returns>
            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult(_httpResponseMessage);
            }
        }
    }
}
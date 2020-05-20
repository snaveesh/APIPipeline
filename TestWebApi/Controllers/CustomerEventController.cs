using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using TestWebApi.Models;
using TestWebApi.Utilities;

namespace TestWebApi.Controllers
{
    [CustomAuthorization]
    [LogInfo]
    public class CustomerEventController : ApiController
    {
        /// <summary>
        /// Raises and event to create a new customer
        /// </summary>
        /// <returns></returns>
        //[HttpGet]
        //public async System.Threading.Tasks.Task<HttpResponseMessage> GetEventAsync(string customerId)
        //{
        //    HttpResponseMessage response;
        //    try
        //    {
        //        string id = customerId.Replace("\"", "");
        //        CustomerEventRaiser customerEventRaiser = new CustomerEventRaiser();
        //        await customerEventRaiser.RaiseNewCustomerEvent(id);
        //        response = Request.CreateResponse(HttpStatusCode.OK, "new customer "+ id + " created using event hub");
        //    }
        //    catch(Exception ex)
        //    {
        //        response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
        //    }
        //    return response;

        //}

        /// <summary>
        /// Raises and event to create a new customer
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async System.Threading.Tasks.Task<HttpResponseMessage> GetEventAsync(Customer customer)
        {
            HttpResponseMessage response;
            try
            {
                await ServiceBusHandler.Initialize(JsonConvert.SerializeObject(customer));
                response = Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
            return response;

        }

        /// <summary>
        /// invoked by event to insert customer into db
        /// </summary>
        /// <param name="customer">customer details</param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage PostCustomer(CustomerEvent customer)
        {
            var url = "";
            if (ConfigurationManager.AppSettings["IsTestingEnv"].ToString() == "true")
            {
                url = ConfigurationManager.AppSettings["CustomerEventUrl"].ToString();
            }
            else
            {
                url = Environment.GetEnvironmentVariable("CustomerEventUrl");
            }
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response;
            try
            {
                var content = JsonConvert.SerializeObject(customer);
                if (null != content)
                {
                    var stringContent = new StringContent(content, UnicodeEncoding.UTF8, "application/json");
                    var result = httpClient.PostAsync(url, stringContent);
                    response = result.Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, "Customer details entered succesfully");
                    }
                    return response;
                }
                else
                {
                    throw new Exception("Failed to serialize object!");
                }
            }
            catch (Exception ex)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                return response;
            }
        }

    }
}

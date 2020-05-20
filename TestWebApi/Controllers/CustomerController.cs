using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TestWebApi.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using TestWebApi.Utilities;
using System.Threading.Tasks;

namespace TestWebApi.Controllers
{
    [CustomAuthorization]
    [LogInfo]
    public class CustomerController : ApiController
    {
        /// <summary>
        /// /GetCustomers:This api returns a list containing all the customers with the lastName in the db.
        /// </summary>
        /// <returns>List of customers</returns>
        [HttpGet]
        public HttpResponseMessage GetCustomers(string lastName)
        {
            string url = "";
            if (ConfigurationManager.AppSettings["IsTestingEnv"].ToString() == "true")
            {
                url = ConfigurationManager.AppSettings["CustomersViewUrl"].ToString() + "&lastName=" + lastName;
            }
            else
            {
               url = Environment.GetEnvironmentVariable("CustomersViewUrl") + "&lastName=" + lastName;
            }
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response;
            try
            {
                var clientResponse = httpClient.GetAsync(url);
                clientResponse.Wait();
                return clientResponse.Result;
            }
            catch(Exception ex)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                return response;
            }
        }

        /// <summary>
        /// /GetCustomers:This api returns a list containing the customer with the id in the db.
        /// </summary>
        /// <returns>List of customers</returns>
        [HttpGet]
        public HttpResponseMessage GetCustomersById(string id)
        {
            string url = "";
            if (ConfigurationManager.AppSettings["IsTestingEnv"].ToString() == "true")
            {
                url = ConfigurationManager.AppSettings["CustomersViewByIdUrl"].ToString();
            }
            else
            {
                url = Environment.GetEnvironmentVariable("CustomersViewByIdUrl");
            }

            url = url.Replace("{id}", id).Replace("\"","");
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response;
            try
            {
                var clientResponse = httpClient.GetAsync(url);
                clientResponse.Wait();
                return clientResponse.Result;
            }
            catch (Exception ex)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                return response;
            }
        }

        /// <summary>
        /// /GetCustomers:This api returns a list containing all the customers in the db.
        /// </summary>
        /// <returns>List of customers</returns>
        [HttpGet]
        public HttpResponseMessage GetCustomers()
        {
            string url = "";
            if (ConfigurationManager.AppSettings["IsTestingEnv"].ToString() == "true")
            {
                url = ConfigurationManager.AppSettings["CustomersViewUrl"].ToString();
            }
            else
            {
                url = Environment.GetEnvironmentVariable("CustomersViewUrl");
            }
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response;
            try
            {
                var clientResponse = httpClient.GetAsync(url);
                clientResponse.Wait();
                return clientResponse.Result;
            }
            catch (Exception ex)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                return response;
            }
        }

        /// <summary>
        /// This api posts customer data to databse
        /// </summary>
        /// <param name="customer">customer data</param>
        /// <returns>http response message</returns>
        [HttpPost]
        [CustomerDataValidation]
        public async Task<HttpResponseMessage> PostCustomer(Customer customer)
        {
            HttpResponseMessage response;
            HttpClient httpClient = new HttpClient();
            string url = "";
            string authToken = "";
            string eventbaseurl = "";
            if (ConfigurationManager.AppSettings["IsTestingEnv"].ToString() == "true")
            {
                url = ConfigurationManager.AppSettings["CutomerAddUrl"].ToString();
                authToken = ConfigurationManager.AppSettings["authToken"].ToString();
                eventbaseurl = ConfigurationManager.AppSettings["EventApiUrl"].ToString();
            }
            else
            {
                url = Environment.GetEnvironmentVariable("CutomerAddUrl").ToString();
                authToken = Environment.GetEnvironmentVariable("authToken");
                eventbaseurl = Environment.GetEnvironmentVariable("EventApiUrl");
            }
            try
            {
                if (Request.Headers.Contains("transactionID"))
                {
                    customer.TransactionID = Request.Headers.GetValues("transactionID").FirstOrDefault();
                }
                if (Request.Headers.Contains("agentID"))
                {
                    customer.AgentId = Request.Headers.GetValues("agentID").FirstOrDefault();
                }
                customer.id = GenerateCustomerId.Generate(9);
                var content = JsonConvert.SerializeObject(customer);
                var stringContent = new StringContent(content, UnicodeEncoding.UTF8, "application/json");
                var result = httpClient.PostAsync(url, stringContent);
                response = result.Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //HttpClient eventHttpClient = new HttpClient();
                    //var eventurl= eventbaseurl+"?customerId=" +customer.id;
                    //eventHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                    //var eventresponse=eventHttpClient.GetAsync(eventurl);
                    //eventresponse.Wait();
                    //response = Request.CreateResponse(HttpStatusCode.OK, "Customer details entered succesfully");
                    await ServiceBusHandler.Initialize(JsonConvert.SerializeObject(customer));
                    await CustomerEventRaiser.RaiseNewCustomerEvent(customer.id);
                }
                return response;
            }
            catch(Exception ex)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                return response;
            }
        }
    }
}

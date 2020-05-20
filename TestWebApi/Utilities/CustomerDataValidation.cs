using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using TestWebApi.Models;
using NHibernate.Validator.Cfg;
using NHibernate.Validator.Engine;
using Environment = NHibernate.Validator.Cfg.Environment;
using System.Net.Http;
using Newtonsoft.Json.Schema;

namespace TestWebApi.Utilities
{
    public class CustomerDataValidation:ActionFilterAttribute
    {
        private ValidatorEngine _ve = new ValidatorEngine();
        /// <summary>
        /// Filter implementation to validate input data
        /// </summary>
        /// <param name="actionContext">http action context</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            string rawRequest;
            using (var stream = new StreamReader(actionContext.Request.Content.ReadAsStreamAsync().Result))
            {
                stream.BaseStream.Position = 0;
                rawRequest = stream.ReadToEnd();
            }
            string errors = "";
            if (null != rawRequest)
            {
                if (!ValidateJson(rawRequest))
                {
                    string jsonexample = @"{'FirstName': {'type':'string','required': true},'LastName': {'type':'string','required': true},'Address': {'type':'string','required': true},'Phonenumber': {'type':'string','required': true}}";
                    errors += "Invalid Json! It should have the following structure: " + jsonexample;
                }
                else
                {
                    Customer customer = JsonConvert.DeserializeObject<Customer>(rawRequest);
                    ConfigureValidator();
                    if (!actionContext.Request.Headers.Contains("transactionID"))
                    {
                        errors += "transactionID is required in the header! ";
                    }
                    if (!actionContext.Request.Headers.Contains("agentID"))
                    {
                        errors += "agentID is required in the header! ";
                    }
                    if (!_ve.IsValid(customer))
                    {
                        var results = _ve.Validate(customer);
                        foreach (InvalidValue error in results)
                        {
                            errors += error.Message + ". ";
                        }
                    }
                }
            }
            else
            {
                errors += "Customer data missing in body of request!";
            }
            if (errors != "")
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(System.Net.HttpStatusCode.BadRequest, errors);
                return;
            }
            base.OnActionExecuting(actionContext);
        }

        /// <summary>
        /// validate input json schema
        /// </summary>
        /// <param name="data">json string</param>
        /// <returns></returns>
        private bool ValidateJson(string data)
        {
            JSchema schema = JSchema.Parse(@"{'description': 'Customer', 'type': 'object','properties':
            {'FirstName': {'type':'string','required': true},'LastName': {'type':'string','required': true},'Address': {'type':'string','required': true},'Phonenumber': {'type':'string','required': true}}
         }");
            JObject customer = JObject.Parse(data);
            return customer.IsValid(schema);
        }

        /// <summary>
        /// Configure nhibernate validator
        /// </summary>
        private void ConfigureValidator()
        {
            INHVConfiguration nhvc = new XmlConfiguration();
            nhvc.Properties[Environment.ApplyToDDL] = "true";
            nhvc.Properties[Environment.AutoregisterListeners] = "true";
            nhvc.Properties[Environment.ValidatorMode] = ValidatorMode.UseExternal.ToString();
            nhvc.Mappings.Add(new MappingConfiguration("TestWebApi", null));
            _ve.Configure(nhvc);
        }
    }
}
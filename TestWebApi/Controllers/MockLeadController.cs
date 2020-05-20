using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace TestWebApi.Controllers
{
    public class MockLeadController : ApiController
    {
        public HttpResponseMessage GetMockLead()
        {
            try
            {
                if (Request.Headers.Authorization.Parameter != null && Request.Headers.Authorization.Parameter != null)
                {
                    string content = "{\"LeadArraySuccessResponse\": {\"Lead\": [{\"Name\": \"TAMAMI MORIMOTO\",\"AddressLine1\": \"1-3-39-E 710 ROPPONGI\",\"AddressLine2\": null,\"AddressLine3\": null,\"City\": \"MINATO-KU\",\"PostalCode\": \"106-0032\",\"CountryCode\": \"JP\",\"PhoneNumber\": null,\"Email\": null,\"LeadNumber\": 5203865010,\"Club_Mbr_Type_Code\": null,\"Club_Service_Code\": null},{\"Name\": \"YASUHIRO MORIMOTO\",\"AddressLine1\": \"1-3-39-E 710 ROPPONGI\",\"AddressLine2\": null,\"AddressLine3\": null,\"City\": \"MINATO-KU\",\"PostalCode\": \"106-0032\",\"CountryCode\": \"JP\",\"PhoneNumber\": 9088431373,\"Email\": \"yasun@nifty.com\",\"LeadNumber\": 5203865010,\"Club_Mbr_Type_Code\": null,\"Club_Service_Code\": null}]}}";
                    var stringContent = new StringContent(content, UnicodeEncoding.UTF8, "application/json");
                    HttpResponseMessage response = new HttpResponseMessage();
                    response.Content = stringContent;
                    response.StatusCode = HttpStatusCode.OK;
                    return response;
                }
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}

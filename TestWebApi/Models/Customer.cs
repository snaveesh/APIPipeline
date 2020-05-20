using System;

namespace TestWebApi.Models
{
    public class Customer
    {
        public string id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Phonenumber { get; set; }
        public string TransactionID { get; set; }
        public string AgentId { get; set; }
    }
}
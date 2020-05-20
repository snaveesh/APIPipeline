using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TestWebApi.Utilities
{
    public class ServiceBusHandler
    {
        public static async Task Initialize(string message)
        {
            try
            {
                string ServiceBusConnectionString = "";
                string QueueName = "";
                if (ConfigurationManager.AppSettings["IsTestingEnv"].ToString() == "true")
                {
                    ServiceBusConnectionString = ConfigurationManager.AppSettings["ServiceBusConnectionString"].ToString();
                    QueueName = ConfigurationManager.AppSettings["ServiceBusTopic"].ToString();
                }
                else
                {
                    ServiceBusConnectionString=Environment.GetEnvironmentVariable("ServiceBusConnectionString");
                    QueueName=Environment.GetEnvironmentVariable("ServiceBusTopic");
                }
                ITopicClient topicClient = new TopicClient(ServiceBusConnectionString, QueueName);
                await AddMessage(message, topicClient);
                await topicClient.CloseAsync();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public static async Task AddMessage(string message,ITopicClient topicClient)
        {
            try
            {
                var encodedMessage = new Message(Encoding.UTF8.GetBytes(message));
                await topicClient.SendAsync(encodedMessage);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
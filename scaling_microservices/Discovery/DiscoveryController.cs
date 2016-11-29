using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;

namespace scaling_microservices.Controllers
{
    public class DiscoveryController : ApiController
    {
        IConnection serviceConnection;
        IModel channel;
        QueueDeclareOk responseQueue;
        IBasicProperties qProps;
        DiscoveryController()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };
            //port change prohibited
            //factory.Port = DiscoveryService.Instance.Port;
            serviceConnection = factory.CreateConnection();
            channel = serviceConnection.CreateModel();
            responseQueue = channel.QueueDeclare();
            qProps = channel.CreateBasicProperties();
            qProps.ReplyTo = responseQueue.QueueName;
        }


        [HttpGet]
        [ActionName("services")]
        public IEnumerable<string> Services()
        {
            try
            {
                var request = new QueueRequest() { method = "get" };
                channel.BasicPublish("", DiscoveryService.QueueName, qProps,
                    request.ToByteArray());
                //recieve reply from service
                //create response
                return new List<string>(){ };
            }
            catch(/*timeout*/Exception)
            {
                return new List<string>() { };
            }
        }

        [HttpGet]
        [ActionName("data")]
        public IEnumerable<KeyValuePair<string,DateTime>> Data()
        {
            try
            {
                //access to discovery
                return new List<KeyValuePair<string, DateTime>> { };
            }
            catch (Exception)
            {
                return new List<KeyValuePair<string, DateTime>> { };
            }
        }

        [HttpPost]
        [ActionName("ping")]
        public HttpResponseMessage Ping([FromUri] string id)
        {
            try
            {
                //Access discovery service
                return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
            }
            catch(Exception)
            {
                //do nothing really
                //prop: try to restart service
                return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;

namespace scaling_microservices.Discovery
{
    class DiscoveryController : ApiController
    {
        IConnection serviceConnection;
        IModel model;
        QueueDeclareOk responseQueue;
        IBasicProperties qProps;
        DiscoveryController()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = DiscoveryService.Instance.Port
            };

            serviceConnection = factory.CreateConnection();
            model = serviceConnection.CreateModel();
            responseQueue = model.QueueDeclare();
            qProps = model.CreateBasicProperties();
            qProps.ReplyTo = responseQueue.QueueName;
        }


        [HttpGet]
        public IEnumerable<string> Services()
        {
            try
            {

                model.BasicPublish("", DiscoveryService.QueueName, qProps,
                    System.Text.Encoding.UTF8.GetBytes(""));//convert json->string->bytes
                //access to discovery service via Rabbitmq
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

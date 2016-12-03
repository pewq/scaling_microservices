using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;
using Newtonsoft.Json;

namespace scaling_microservices.Controllers
{
    public class DiscoveryController : ApiController
    {
        IConnection serviceConnection;
        IModel channel;
        QueueDeclareOk responseQueue;
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
            
        }


        [HttpGet]
        [ActionName("services")]
        public IHttpActionResult Services()
        {
            try
            {
                var request = new QueueRequest() { method = "get" };
                var props = IService.CreateBasicProperties(channel, responseQueue.QueueName);
                channel.BasicPublish("", DiscoveryService.QueueName, props,
                    request.ToByteArray());
                QueueingBasicConsumer consumer = new QueueingBasicConsumer(channel);
                channel.BasicConsume(responseQueue.QueueName, false, consumer);
                BasicDeliverEventArgs ea = consumer.Queue.Dequeue() as BasicDeliverEventArgs;
                var body = Encoding.UTF8.GetString(ea.Body, 0, ea.Body.Length);
                channel.BasicAck(ea.DeliveryTag, false);
                return Json(JsonConvert.DeserializeObject(body));
            }
            catch(Exception e)
            {
                return new System.Web.Http.Results.ExceptionResult(e, this);
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

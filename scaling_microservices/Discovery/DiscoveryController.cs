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
        RabbitEndpoint endpoint;
        DiscoveryController()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };

            endpoint = new RabbitEndpoint();
        }


        [HttpGet]
        [ActionName("services")]
        public IHttpActionResult Services()
        {
            try
            {
                var request = new QueueRequest() { method = "get" };
                endpoint.SendTo(request, DiscoveryService.QueueName);
                var serviceResponse = endpoint.Recieve();
                return Json(JsonConvert.DeserializeObject(serviceResponse.StringBody));
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

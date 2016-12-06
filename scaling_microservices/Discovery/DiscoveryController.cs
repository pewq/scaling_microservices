using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public IHttpActionResult Data()
        {
            try
            {
                var request = new QueueRequest() { method = "data" };
                endpoint.SendTo(request, DiscoveryService.QueueName);
                var serviceResponse = endpoint.Recieve();
                return Json(JsonConvert.DeserializeObject(serviceResponse.StringBody));
            }
            catch (Exception e)
            {
                return new System.Web.Http.Results.ExceptionResult(e, this);
            }
        }

        [HttpPost]
        [ActionName("ping")]
        public IHttpActionResult Ping([FromUri] string id)
        {
            try
            {
                var request = new QueueRequest() { method = "ping" };
                request.arguments.Add("name", id);
                endpoint.SendTo(request, DiscoveryService.QueueName);
                var endpResponse = JsonConvert.DeserializeObject(endpoint.Recieve().StringBody);
                if(endpResponse.GetType().GetField("error") != null)
                {
                    throw new Exception(endpResponse.GetType().GetField("message").GetValue(endpResponse).ToString());
                }
                //Access discovery service
                return new System.Web.Http.Results.StatusCodeResult(System.Net.HttpStatusCode.OK, this);
            }
            catch(Exception e)
            {
                //do nothing really
                //prop: try to restart service
                return new System.Web.Http.Results.ExceptionResult(e, this);
            }
        }
    }
}

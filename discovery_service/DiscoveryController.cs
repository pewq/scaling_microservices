using System;
using System.Web.Http;
using RabbitMQ.Client;
using Newtonsoft.Json;
using scaling_microservices.Rabbit;
using scaling_microservices.Auth;

namespace discovery_service
{
    public class DiscoveryController : ApiController
    {
        SubscriptionEndpoint endpoint;
        DiscoveryController()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };

            endpoint = new SubscriptionEndpoint(factory);
        }

        [HttpGet]
        [ActionName("services")]
        public IHttpActionResult Services()
        {
            try
            {
                var request = new QueueRequest() { method = "get_services" };
                endpoint.SendTo(request, DiscoveryService.QueueName);
                var serviceResponse = endpoint.Recieve();
                return Json(JsonConvert.DeserializeObject(serviceResponse.StringBody));
            }
            catch (Exception e)
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
        [ServiceAuthorization("authservice")]
        public IHttpActionResult Ping([FromUri] string name, [FromUri] string token)
        {
            try
            {
                var request = new QueueRequest() { method = "ping" };
                request["name"] = name;
                request["token"] = token;
                //Access discovery service
                endpoint.SendTo(request, DiscoveryService.QueueName);
                var endpointResponseBody = endpoint.Recieve().StringBody;
                var statusTemplate = new { status = System.Net.HttpStatusCode.OK };
                //try to extract status
                try
                {
                    var exStatus = JsonConvert.DeserializeAnonymousType(endpointResponseBody, statusTemplate).status;
                    if(exStatus == System.Net.HttpStatusCode.OK)
                    {
                        return new System.Web.Http.Results.StatusCodeResult(System.Net.HttpStatusCode.OK, this);
                    }
                    if(exStatus == System.Net.HttpStatusCode.NotFound)
                    {
                        //TODO : extract and send message field
                        var actionResult = new System.Web.Http.Results.NotFoundResult(this);
                    }
                }
                catch (Exception e)
                {
                    //handle incorrect response
                }
                var errorTemplate = new { error = "" };
                try
                {
                    var exError = JsonConvert.DeserializeAnonymousType(endpointResponseBody, errorTemplate).error;
                    return new System.Web.Http.Results.ExceptionResult(new Exception(exError), this);
                }
                catch(Exception e)
                {
                    return new System.Web.Http.Results.ExceptionResult(e, this);
                }
            }
            catch (Exception e)
            {
                //do nothing really
                //prop: try to restart service
                return new System.Web.Http.Results.ExceptionResult(e, this);
            }
        }
    }
}
//TODO : provide port of webapp using Request.RequestUri.Port.ToString();
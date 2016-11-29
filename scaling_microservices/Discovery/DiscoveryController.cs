using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace scaling_microservices.Discovery
{
    class DiscoveryController : ApiController
    {
        [HttpGet]
        public IEnumerable<string> Services()
        {
            try
            {
                //access to discovery service via Rabbitmq
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

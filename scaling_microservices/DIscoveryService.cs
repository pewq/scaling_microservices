using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using System.Web;
using Newtonsoft.Json;

namespace scaling_microservices
{
    class DiscoveryService : IService
    {
        ServiceRegistry registry;

        public DiscoveryService(IConnection _connection, IModel _model, string _queueName) :
            base(_connection, _model, _queueName)
        {
            registry = new ServiceRegistry();
        }

        protected override void ThreadFunction()
        {
            throw new NotImplementedException();
        }

        protected override string ProcessRequest(string requestString)
        {
            var request = HttpUtility.ParseQueryString(requestString);
            
            string method = request.Get("method");
            switch(method)
            {
                case "get":
                    {
                        var response = new
                        {
                            response="success",
                            services = registry.GetServices()
                        };
                        return JsonConvert.SerializeObject(response);
                    }
                case "ping":
                    {
                        registry.Add(request.Get("name"));
                        var response = new
                        {
                            response = "success",
                            message = "registered"
                        };
                        return JsonConvert.SerializeObject(response);
                    }
                default:
                    {
                        var response = new
                        {
                            response = "error",
                            message = "unknown method. available methods: get, ping",
                        };
                        return JsonConvert.SerializeObject(response);
                    }
            }
        }
    }
}

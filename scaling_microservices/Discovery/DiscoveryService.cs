using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using System.Web;
using Newtonsoft.Json;

namespace scaling_microservices
{
    /// <note>singleton Instance, listens to queue with @QueueName</note>
    class DiscoveryService : IService
    {
        private ServiceRegistry registry;
        private DiscoveryService(IConnection _connection, IModel _model, string _queueName) :
            base(_connection, _model, _queueName)
        {
            registry = new ServiceRegistry();
        }

        private DiscoveryService(string queueName, string port) 
            : this(queueName, int.Parse(port))
        { }
        private DiscoveryService(string queueName, int port) :
            base(queueName, port.ToString())
        {
            this.Port = port;
            registry = new ServiceRegistry();
        }

        public int Port { get; private set; }
        public static DiscoveryService Instance { get; private set; }
        public const string QueueName = "DiscoveryCommandQueue";
        static DiscoveryService()
        {
            Instance = new DiscoveryService(QueueName, 9090);
        }

        protected override void ThreadFunction()
        {
            while(true)
            {
                var message = this.subscription.Next();
                var responseArguments = new
                {
                    replyQueue = message.BasicProperties.ReplyTo,
                    replyAddr = message.BasicProperties.ReplyToAddress,
                    correlationId = message.BasicProperties.CorrelationId,
                };
                var response = this.ProcessRequest(new QueueRequest(message.Body));
                //reply using responseArguments and response
                subscription.Ack();
            }
        }
        protected override string ProcessRequest(QueueRequest request)
        {
            string method = request.method;
            switch(method)
            {
                case "get":
                    {
                        var response = new
                        {
                            response = "success",
                            services = registry.GetServices()
                        };
                        return JsonConvert.SerializeObject(response);
                    }
                case "ping":
                    {
                        try
                        {
                            registry.Add(request.arguments["name"]);
                            var response = new
                            {
                                response = "success",
                                message = "registered"
                            };
                            return JsonConvert.SerializeObject(response);

                        }
                        catch (Exception)
                        {
                            var response = new
                            {
                                response = "error",
                                message = "no parameter 'name' provided"
                            };
                            return JsonConvert.SerializeObject(response);
                        }
                        
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

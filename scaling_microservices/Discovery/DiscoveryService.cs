using System;
using Newtonsoft.Json;
using scaling_microservices.Rabbit;
using scaling_microservices.Registry;

namespace scaling_microservices
{
    /// <note>singleton Instance, listens to queue with @QueueName</note>
    /// methods : 
    /// get
    /// ping(name,token)
    /// register(name,address,token,type)
    /// send_message(message)
    public class DiscoveryService : IService
    {
        private ServiceRegistry registry;

        private DiscoveryService(string queueName) : base(queueName)
        {
            this.registry = new ServiceRegistry();
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
            Instance = new DiscoveryService(QueueName);
        }
    }
}

using System;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;

namespace scaling_microservices.Rabbit
{
    public abstract class IEndpoint : IDisposable
    {
        public string InQueue { get; private set; }

        public IModel channel { get; private set; }
        public IConnection connection { get; private set; }

        public IEndpoint()
        {
            var factory = new ConnectionFactory();
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            var queue = channel.QueueDeclare();
            InQueue = queue.QueueName;
        }

        public IEndpoint(string inQName = "")
        {
            var factory = new ConnectionFactory();
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            var queue = channel.QueueDeclare(queue: inQName);
            InQueue = queue.QueueName;
        }

        public IEndpoint(string host, int port, string inQname)
        {
            var factory = new ConnectionFactory() { HostName = host/*, Port = port*/};
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            var queue = channel.QueueDeclare(queue: inQname);
            InQueue = queue.QueueName;
        }

        public IEndpoint(SubscriptionEndpoint other, string inQName = "")
        {
            connection = other.connection;
            channel = other.channel;
            var queue = channel.QueueDeclare(queue: inQName);
            InQueue = queue.QueueName;
        }

        public Message Message()
        {
            return new Message() { properties = CreateBasicProperties() };
        }

        /// <summary>
        /// Creates basic properties with correlationId from msg
        /// </summary>
        public IBasicProperties CreateBasicProperties(Message msg)
        {
            var props = channel.CreateBasicProperties();
            props.CorrelationId = msg.CorrelationId;
            return props;
        }

        /// <summary>
        /// Creates basic properties with new CorrelationID and set ReplyTo to this.InQueue
        /// </summary>
        public IBasicProperties CreateBasicProperties()
        {
            var props = channel.CreateBasicProperties();
            props.CorrelationId = Guid.NewGuid().ToString();
            props.ReplyTo = InQueue;
            return props;
        }

        public void Bind(string exchange, string routing)
        {
            channel.QueueBind(InQueue, exchange, routing);
        }

        public void UnBind(string exchange, string routing)
        {
            channel.QueueUnbind(InQueue, exchange, routing);
        }


        public void Dispose()
        {
            channel.QueueDelete(InQueue);
        }
    }
}

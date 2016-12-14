using System;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;

namespace scaling_microservices.Rabbit
{
    public class RabbitEndpoint : IDisposable
    {
        public string InQueue { get; private set; }

        public IModel channel { get; private set; }
        public IConnection connection { get; private set; }
        public ISubscription subscription { get; private set; }

        public RabbitEndpoint()
        {
            var factory = new ConnectionFactory();
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            var queue = channel.QueueDeclare();
            InQueue = queue.QueueName;
            subscription = new Subscription(channel, InQueue);
        }

        public RabbitEndpoint(string inQName = "")
        {
            var factory = new ConnectionFactory();
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            var queue = channel.QueueDeclare(queue: inQName);
            InQueue = queue.QueueName;
            subscription = new Subscription(channel, inQName);
        }

        public RabbitEndpoint(string host, int port, string inQname)
        {
            var factory = new ConnectionFactory() { HostName = host/*, Port = port*/};
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            var queue = channel.QueueDeclare(queue: inQname);
            InQueue = queue.QueueName;
            subscription = new Subscription(channel, InQueue);
        }

        public RabbitEndpoint(RabbitEndpoint other, string inQName = "")
        {
            connection = other.connection;
            channel = other.channel;
            var queue = channel.QueueDeclare(queue: inQName);
            InQueue = queue.QueueName;
            subscription = new Subscription(channel, InQueue);
        }
        public Message Recieve()
        {
            var msg = subscription.Next();
            return new Message() { properties = msg.BasicProperties, body = msg.Body };
        }

        public void Send(Message msg)
        {
            var props = CreateBasicProperties(msg);
            props.ContentEncoding = msg.Encoding;
            channel.BasicPublish("", msg.properties.ReplyTo, props, msg.body);
        }

        public void SendTo(Message msg, string toQName)
        {
            var properties = CreateBasicProperties();
            properties.ContentEncoding = msg.Encoding;
            channel.BasicPublish("", toQName, properties, msg.body);
        }
        
        public void SendTo(QueueRequest request, string toQName)
        {
            var properties = CreateBasicProperties();
            properties.ContentEncoding = typeof(QueueRequest).ToString();
            channel.BasicPublish("", toQName, properties, request.ToByteArray());
        }

        public void SendToExchange(QueueRequest request, string exchange, string routing)
        {
            var properties = CreateBasicProperties();
            properties.ContentEncoding = typeof(QueueRequest).ToString();
            channel.BasicPublish(exchange, routing, properties, request.ToByteArray());
        }

        public void SendToExchange(Message msg, string exchange, string routing)
        {
            var properties = CreateBasicProperties();
            //TODO : prevent Encoding from getting from undefined properties
            properties.ContentEncoding = msg.Encoding;
            channel.BasicPublish(exchange, routing, properties, msg.body);

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

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

        public IEndpoint(ConnectionFactory f)
        {
            connection = f.CreateConnection();
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
            return CreateBasicProperties(CorrelationId: msg.CorrelationId);
        }

        /// <summary>
        /// Creates basic properties with new CorrelationID and set ReplyTo to this.InQueue
        /// </summary>
        public IBasicProperties CreateBasicProperties()
        {
            return CreateBasicProperties(ReplyTo: InQueue);
        }

        public IBasicProperties CreateBasicProperties( 
            string CorrelationId = "",
            string ReplyTo = "",
            string Encoding = "")
        {
            var props = channel.CreateBasicProperties();
            props.CorrelationId = (CorrelationId != "") ? CorrelationId : Guid.NewGuid().ToString();
            if(Encoding != "")
            {
                props.ContentEncoding = Encoding;
            }
            if(ReplyTo != "")
            {
                props.ReplyTo = ReplyTo;
            }

            return props;
        }
        public void Send(Message msg)
        {
            var props = CreateBasicProperties(msg);
            props.ContentEncoding = msg.Encoding;
            channel.BasicPublish("", msg.properties.ReplyTo, props, msg.body);
        }

        public IBasicProperties SendTo(Message msg, string toQName)
        {
            var properties = CreateBasicProperties();
            properties.ContentEncoding = msg.Encoding;
            channel.BasicPublish("", toQName, properties, msg.body);
            return properties;
        }

        public IBasicProperties SendTo(QueueRequest request, string toQName)
        {
            var properties = CreateBasicProperties(ReplyTo: this.InQueue,
                Encoding: QueueRequest.classname);
            channel.BasicPublish("", toQName, properties, request.ToByteArray());
            return properties;
        }

        public IBasicProperties SendToExchange(QueueRequest request, string exchange, string routing)
        {
            var properties = CreateBasicProperties();
            properties.ContentEncoding = QueueRequest.classname;
            channel.BasicPublish(exchange, routing, properties, request.ToByteArray());
            return properties;
        }

        public IBasicProperties SendToExchange(Message msg, string exchange, string routing)
        {
            var properties = CreateBasicProperties();
            //TODO : prevent Encoding from getting from undefined properties
            properties.ContentEncoding = msg.Encoding;
            channel.BasicPublish(exchange, routing, properties, msg.body);
            return properties;
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
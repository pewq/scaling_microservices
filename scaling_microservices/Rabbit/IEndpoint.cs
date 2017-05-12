using System;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;

namespace scaling_microservices.Rabbit
{
    public abstract class IEndpoint : IDisposable
    {
        public string InQueue { get; private set; }

        //used to reinit connections/channels
        private ConnectionFactory factory;

        public IModel channel { get; private set; }
        public IConnection connection { get; private set; }
        
        public IEndpoint()
        {
            factory = new ConnectionFactory();
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            var queue = channel.QueueDeclare();
            InQueue = queue.QueueName;
        }

        public IEndpoint(ConnectionFactory f)
        {
            this.factory = f;
            connection = f.CreateConnection();
            channel = connection.CreateModel();
            var queue = channel.QueueDeclare();
            InQueue = queue.QueueName;
        }

        public IEndpoint(string inQName = "", bool tryPassiveDeclare = false)
        {
            factory = new ConnectionFactory();
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            QueueDeclareOk queue;
            if (tryPassiveDeclare)
            {
                queue = QueueExistsOrDeclare(inQName);
            }
            else
            {
                queue = channel.QueueDeclare(queue: inQName, exclusive: false);
            }
            InQueue = queue.QueueName;
        }

        public IEndpoint(string host, int port, string inQName = "", bool tryPassiveDeclare = false)
        {
            factory = new ConnectionFactory() { HostName = host/*, Port = port*/};
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            QueueDeclareOk queue;
            if (tryPassiveDeclare)
            {
                queue = QueueExistsOrDeclare(inQName);
            }
            else
            {
                queue = channel.QueueDeclare(queue: inQName, exclusive: false);
            }
            InQueue = queue.QueueName;
        }

        public IEndpoint(SubscriptionEndpoint other, string inQName = "", bool tryPassiveDeclare = false)
        {
            this.factory = other.factory;
            connection = other.connection;
            channel = other.channel;
            QueueDeclareOk queue;
            if (tryPassiveDeclare)
            {
                queue = QueueExistsOrDeclare(inQName);
            }
            else
            {
                queue = channel.QueueDeclare(queue: inQName, exclusive: false);
            }
            InQueue = queue.QueueName;
        }

        public Message Message()
        {
            return new Message() { Properties = CreateBasicProperties() };
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
            channel.BasicPublish("", msg.Properties.ReplyTo, props, msg.body);
        }

        public IBasicProperties SendTo(Message msg, string routing, string exchange = "")
        {
            var properties = CreateBasicProperties();
            properties.ContentEncoding = msg.Encoding;
            channel.BasicPublish(exchange, routing, properties, msg.body);
            return properties;
        }

        public IBasicProperties SendTo(QueueRequest request, string routing, string exchange = "")
        {
            var properties = CreateBasicProperties(ReplyTo: this.InQueue,
                Encoding: QueueRequest.classname);
            channel.BasicPublish(exchange, routing, properties, request.ToByteArray());
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

        public void DeclareExchange(string exchange, string type)
        {
            this.channel.ExchangeDeclare(exchange: exchange, type: type);
        }

        public bool ExchangeExists(string exchange)
        {
            try
            {
                this.channel.ExchangeDeclarePassive(exchange);
                return true;
            }
            catch (RabbitMQ.Client.Exceptions.OperationInterruptedException)
            {
                this.ReInit();
                return false;
            }
        }

        public void ExchangeExistsOrDeclare(string exchange, string type)
        {
            if (!ExchangeExists(exchange))
            {
                DeclareExchange(exchange, type);
            }
        }

        public QueueDeclareOk QueueExistsOrDeclare(string queueName)
        {
            try
            {
                return channel.QueueDeclarePassive(queueName);
            }
            catch (RabbitMQ.Client.Exceptions.OperationInterruptedException)
            {
                //reinit connection/channel
                this.ReInit();
                return channel.QueueDeclare(queue: queueName, exclusive: false);
            }
        }

        public void ReInit()
        {
            this.channel.Dispose();
            this.connection.Dispose();
            this.connection = factory.CreateConnection();
            this.channel = connection.CreateModel();            
        }

        public void Dispose()
        {
            channel.QueueDelete(InQueue);
        }
    }
}
using System;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;

namespace scaling_microservices
{
    public class RabbitEndpoint
    {
        public class Message
        {
            public IBasicProperties properties { get; set; }
            public byte[] body { get; set; }

            public string StringBody
            {
                get
                {
                    switch(properties.ContentEncoding)
                    {
                        case "UTF8": {
                                return System.Text.Encoding.UTF8.GetString(body);
                            }
                        case "ASCII":
                            {
                                return System.Text.Encoding.ASCII.GetString(body);
                            }
                        case "UTF32":
                            {
                                return System.Text.Encoding.UTF32.GetString(body);
                            }
                        case "Unicode":
                            {
                                return System.Text.Encoding.Unicode.GetString(body);
                            }
                        default:
                            return "";
                    }
                }               
            }
            
            public string Encoding
            {
                get
                {
                    return properties.ContentEncoding;
                }
            }

            public string CorrelationId
            {
                get
                {
                    return properties.CorrelationId;
                }
            }
        }

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

        public RabbitEndpoint(string inQName)
        {
            InQueue = inQName;
            var factory = new ConnectionFactory();
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            var queue = channel.QueueDeclare(queue: inQName);

            subscription = new Subscription(channel, inQName);
        }

        public RabbitEndpoint(string host, int port, string inQname)
        {
            InQueue = inQname;
            var factory = new ConnectionFactory() { HostName = host/*, Port = port*/};
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            var queue = channel.QueueDeclare(queue: inQname);
            subscription = new Subscription(channel, inQname);
        }

        public Message Recieve()
        {
            var msg = subscription.Next();
            return new Message() { properties = msg.BasicProperties, body = msg.Body };
        }

        public void Send(Message msg)
        {
            var props = channel.CreateBasicProperties();
            props.ReplyTo = msg.properties.ReplyTo;
            props.CorrelationId = msg.CorrelationId;
            props.ContentEncoding = msg.Encoding;
            channel.BasicPublish("", msg.properties.ReplyTo, props, msg.body);
        }


        public void SendTo(Message msg, string toQName)
        {
            var properties = channel.CreateBasicProperties();
            properties.ReplyTo = InQueue;
            properties.CorrelationId = Guid.NewGuid().ToString();
            properties.ContentEncoding = msg.Encoding;
            channel.BasicPublish("", toQName, properties, msg.body);
        }
        
        public void SendTo(QueueRequest request, string toQName)
        {
            var properties = channel.CreateBasicProperties();
            properties.ReplyTo = InQueue;
            properties.CorrelationId = Guid.NewGuid().ToString();
            properties.ContentEncoding = "QueueRequest";
            channel.BasicPublish("", toQName, properties, request.ToByteArray());
        }

        public IBasicProperties CreateBasicProperties(Message msg)
        {
            var props = channel.CreateBasicProperties();
            props.CorrelationId = msg.CorrelationId;
            return props;
        }

        /// <summary>
        /// Creates basic properties with new CorrelationID and set ReplyTo
        /// </summary>
        public IBasicProperties CreateBasicProperties()
        {
            var props = channel.CreateBasicProperties();
            props.CorrelationId = Guid.NewGuid().ToString();
            props.ReplyTo = InQueue;
            return props;
        }
    }
}

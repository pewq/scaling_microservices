using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;
using Newtonsoft.Json;

namespace scaling_microservices
{
    class RabbitEndpoint
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

        string InQueue;

        IModel channel;
        IConnection connection;
        ISubscription subscription;

        public RabbitEndpoint(string inQName)
        {
            InQueue = inQName;
            var factory = new ConnectionFactory();
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            var queue = channel.QueueDeclare(queue: inQName);

            subscription = new Subscription(channel, inQName);
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
    }
}

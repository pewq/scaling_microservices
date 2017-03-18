using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;

namespace scaling_microservices.Rabbit
{
    public class SubscriptionEndpoint : IEndpoint
    {
        public ISubscription subscription { get; private set; }

        public SubscriptionEndpoint() : base()
        {
            subscription = new Subscription(channel, InQueue);
        }

        public SubscriptionEndpoint(string inQName = "") : base(inQName)
        {
            subscription = new Subscription(channel, inQName);
        }

        public SubscriptionEndpoint(string host, int port, string inQName) : base(host,port,inQName)
        {
            subscription = new Subscription(channel, InQueue);
        }

        public SubscriptionEndpoint(SubscriptionEndpoint other, string inQName = "") : base(other, inQName)
        {
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
    }
}

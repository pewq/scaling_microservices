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
    }
}

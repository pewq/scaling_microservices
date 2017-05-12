using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;

namespace scaling_microservices.Rabbit
{
    public class SubscriptionEndpoint : IEndpoint
    {
        public ISubscription subscription { get; private set; }

        public SubscriptionEndpoint() : base()
        {
            subscription = new Subscription(base.channel, base.InQueue);
        }

        public SubscriptionEndpoint(ConnectionFactory f) : base(f)
        {
            subscription = new Subscription(base.channel, base.InQueue);
        }

        public SubscriptionEndpoint(string inQName = "") : base(inQName)
        {
            subscription = new Subscription(base.channel, base.InQueue);
        }

        public SubscriptionEndpoint(string host, int port, string inQName) : base(host,port,inQName)
        {
            subscription = new Subscription(channel, InQueue);
        }
        public SubscriptionEndpoint(string host, int port, string inQName, bool tryPassiveDeclare) : base(host, port, inQName, tryPassiveDeclare)
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
            return new Message() { Properties = msg.BasicProperties, body = msg.Body };
        }

        public QueueResponse Recieve(int msTimeout)
        {
            RabbitMQ.Client.Events.BasicDeliverEventArgs outRes;
            var flag = subscription.Next(msTimeout, out outRes);
            if(flag)
            {
                var resp = new QueueResponse(outRes.Body, outRes.BasicProperties);
            }
            return null;
        }
    }
}

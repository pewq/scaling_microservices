using scaling_microservices.Rabbit;

namespace scaling_microservices.Proxy
{
    public abstract class IProxy
    {
        public string route { get; private set; }

        public string exchange { get; private set; }

        protected SubscriptionEndpoint endpoint;

        protected IProxy(string _route, string _exchange)
        {
            endpoint = new SubscriptionEndpoint();
            route = _route;
            exchange = _exchange;
        }

        public virtual void Send(QueueRequest req)
        {
            endpoint.SendTo(req, route, exchange);
        }
    }
}

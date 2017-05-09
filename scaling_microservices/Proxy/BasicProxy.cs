using scaling_microservices.Rabbit;

namespace scaling_microservices.Proxy
{
    public class BasicProxy
    {
        public string route { get; private set; }

        public string exchange { get; private set; }

        protected SubscriptionEndpoint endpoint;

        public BasicProxy(string _route, string _exchange)
        {
            endpoint = new SubscriptionEndpoint();
            route = _route;
            exchange = _exchange;
        }

        public virtual void Send(QueueRequest req)
        {
            endpoint.SendTo(req, route, exchange);
        }

        public virtual void Send(string messageString)
        {
            var message = new Message() { StringBody = messageString };
            endpoint.SendTo(message, route, exchange);
        }
        public virtual void Send(Message message)
        {
            endpoint.SendTo(message, route, exchange);
        }
        public virtual void Send(object obj)
        {

        }
    }
}

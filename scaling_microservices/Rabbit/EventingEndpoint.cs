using System;
using RabbitMQ.Client.Events;

namespace scaling_microservices.Rabbit
{
    class EventingEndpoint : IEndpoint
    {
        EventingBasicConsumer consumer;

        public event EventHandler<Message> handlers = null;

        public EventingEndpoint() : base()
        {
            ThisInit();
        }

        public EventingEndpoint(string inQName = "") : base(inQName)
        {
            ThisInit();
        }

        public EventingEndpoint(string host, int port, string inQname) : base(host, port, inQname)
        {
            ThisInit();
        }

        private void ThisInit()
        {
            consumer = new EventingBasicConsumer(channel);
            consumer.Received += Consumer_Received;
        }

        private void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            if(handlers != null)
            {
                var message = new Message() { properties = e.BasicProperties, body = e.Body };
                handlers(this, message);
            }
        }
    }
}

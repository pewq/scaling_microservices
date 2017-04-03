using System;
using System.Collections;
using System.Collections.Generic;
using RabbitMQ.Client.Events;

namespace scaling_microservices.Rabbit
{
    public class EventingEndpoint : IEndpoint
    {
        EventingBasicConsumer consumer;

        public event EventHandler<Message> OnRecieved = null;

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
            try
            {
                var ch = channel;
                consumer = new EventingBasicConsumer(ch);
                consumer.Received += Consumer_Received;
                channel.BasicConsume(base.InQueue, false, "", false, true, null, consumer);
            }
            catch(Exception e)
            {
                Console.Write(e.Message);
            }

        }

        private void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            if(OnRecieved != null)
            {
                var message = new Message() { properties = e.BasicProperties, body = e.Body };
                if (correlatedCallbacks.ContainsKey(e.BasicProperties.CorrelationId))
                {
                    (correlatedCallbacks[e.BasicProperties.CorrelationId])(this, message);
                }
                else
                {
                    OnRecieved(this, message);
                }
            }
        }


        private Dictionary<string, EventHandler<Message>> correlatedCallbacks = new Dictionary<string, EventHandler<Message>>();
        public void SendWithCallback(string toQName, QueueRequest request, EventHandler<Message> callback)
        {
            var props = SendTo(request, toQName);
            correlatedCallbacks.Add(props.CorrelationId, callback);
        }
    }
}

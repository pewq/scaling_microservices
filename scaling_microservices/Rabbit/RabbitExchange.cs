using System;
using System.Collections.Generic;
using System.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;

namespace scaling_microservices.Rabbit
{
    public class RabbitExchange : IDisposable
    {
        //TODO : routingKey?
        public class ConnectionInfo
        {
            public string name { get; set; }
            public enum Type
            {
                Queue,
                Exchange
            }
            public enum InOut
            {
                In,
                Out
            }
            public Type type { get; set; }

            public InOut inout { get; set; }
        }

        public IModel channel { get; private set; }
        public IConnection connection { get; private set; }

        public string Name { get; private set; }

        public List<ConnectionInfo> Connections { get; private set; }

        public List<ConnectionInfo> InConnections
        {
            get
            {
                return Connections
                    .Where(x => x.inout == ConnectionInfo.InOut.In)
                    .ToList();
            }
        }
        public List<ConnectionInfo> OutConnections
        {
            get
            {
                return Connections
                    .Where(x => x.inout == ConnectionInfo.InOut.Out)
                    .ToList();
            }
        }

        public RabbitExchange(string name, string type)
        {
            var factory = new ConnectionFactory();
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.ExchangeDeclare(name, type);
            this.Name = name;
        }
        /// <warning>use this with uninstantiated queues/exchanges</warning>
        public void Bind(string other, ConnectionInfo.Type type, string routingKey = "")
        {
            if(routingKey == "")
            {
                routingKey = other;
            }
            switch(type)
            {
                case ConnectionInfo.Type.Queue:
                    {
                        channel.QueueBind(other, Name, routingKey);
                        break;
                    }
                case ConnectionInfo.Type.Exchange:
                    {
                        channel.ExchangeBind(other, Name, routingKey);
                        break;
                    }
            }
            Connections.Add(new ConnectionInfo()
            {
                name = other,
                type = type,
                inout = ConnectionInfo.InOut.Out
            });
        }

        public void Bind(RabbitEndpoint other, string routingKey = "")
        {
            if(routingKey == "")
            {
                routingKey = other.InQueue;
            }
            channel.QueueBind(other.InQueue, Name, routingKey);
            Connections.Add(new ConnectionInfo()
            {
                name = other.InQueue,
                type = ConnectionInfo.Type.Queue,
                inout = ConnectionInfo.InOut.Out
            });
        }
        public void Bind(RabbitExchange other, string routingKey = "")
        {

        }

        public void BindExchange(string other, string routingKey = "")
        {
            if(routingKey =="")
            {
                other = routingKey;
            }
            channel.ExchangeBind(Name, other, routingKey);
            Connections.Add(new ConnectionInfo()
            {
                name = other,
                type = ConnectionInfo.Type.Exchange,
                inout = ConnectionInfo.InOut.In
            });
        }
        public void BindExchange(RabbitExchange other, string routingKey = "")
        {

        }

        #region IDisposable

        private bool disposed = false;

        public void Dispose()
        {
            if(!disposed)
            {
                channel.ExchangeDelete(Name);
            }
        }
        #endregion //IDisposable
    }
}

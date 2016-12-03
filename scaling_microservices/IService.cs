using System;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;

namespace scaling_microservices
{
    /// <summary>
    /// provides subscription to ancestor classes
    /// </summary>
    public abstract class IService
    {
        protected string connectionString;//database connection string
        protected IConnection connection;
        protected IModel channel;
        protected Subscription subscription;
        protected Thread thread;
        protected abstract void ThreadFunction();
        protected abstract string ProcessRequest(QueueRequest request);
        public void Start()
        {
            thread = new Thread(ThreadFunction);
            thread.Start();
        }
        public void Stop()
        {
            thread.Abort();
        }
        public IService(IConnection _connection, IModel _model, string queueName)
        {
            connection = _connection;
            channel = _model;
            subscription = new Subscription(channel, queueName);
        }
        public IService(string queueName, string port)
        {
            //port change prohibited
            var factory = new ConnectionFactory() { HostName = "localhost"/*, Port = int.Parse(port)*/ };
            connection = factory.CreateConnection();
            
            {
                channel = connection.CreateModel();
                channel.QueueDeclare(queueName, false, false, false, null);

                subscription = new Subscription(connection.CreateModel(), queueName);
            }
        }

        public static IBasicProperties CreateBasicProperties(IModel channel, string responseQueue, string address = "")
        {
            var props = channel.CreateBasicProperties();
            props.CorrelationId = Guid.NewGuid().ToString();
            props.ReplyTo = responseQueue;
            if(address != "")
            {
                //set props.ReplyToAddress
            }
            return props;
        }
    }
}

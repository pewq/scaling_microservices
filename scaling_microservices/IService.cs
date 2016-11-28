using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;

namespace scaling_microservices
{
    public abstract class IService
    {
        protected string connectionString;
        protected IConnection connection;
        protected IModel channel;
        protected Subscription subscription;
        protected Thread thread;
        protected abstract void ThreadFunction();
        protected abstract string ProcessRequest(string request);
        public void Start()
        {
            thread = new Thread(ThreadFunction);
            thread.Start();
        }
        public void Stop()
        {
            thread.Interrupt();
        }
        public IService(IConnection _connection, IModel _model, string queueName)
        {
            connection = _connection;
            channel = _model;
            subscription = new Subscription(channel, queueName);
        }
    }
}

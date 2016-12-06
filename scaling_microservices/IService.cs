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
        protected RabbitEndpoint endpoint { get; private set; }
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
            endpoint = new RabbitEndpoint(/*_connection , _model,*/ queueName);
        }
        public IService(string queueName, string port)
        {
            endpoint = new RabbitEndpoint("localhost", int.Parse(port), queueName);
        }
    }
}

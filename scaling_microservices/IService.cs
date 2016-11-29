using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;
using System.Web.Http;

namespace scaling_microservices
{
    public abstract class IService : ApiController
    {
        protected string connectionString;//database connection string
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
        public IService(string queueName, string port)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", Port = int.Parse(port) };
            using (var conn = factory.CreateConnection())
            {
                var model = conn.CreateModel();
                model.QueueDeclare(queueName, false, false, false, null);

                subscription = new Subscription(conn.CreateModel(), queueName);
            }
        }
    }
}

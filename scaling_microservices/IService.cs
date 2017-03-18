using System.Collections.Generic;
using System.Threading;

namespace scaling_microservices.Rabbit
{
    /// <summary>
    /// provides subscription to ancestor classes
    /// </summary>
    public abstract class IService
    {
        protected delegate void RequestHandle(QueueRequest req);

        protected string connectionString;//database connection string
        protected SubscriptionEndpoint endpoint { get; private set; }
        protected Thread thread;

        protected EventDictionary<RequestHandle> Handlers { get; private set; } 
            = new EventDictionary<RequestHandle>();
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

        public IService()
        {
            endpoint = new SubscriptionEndpoint();
            Handlers.Add("default", new RequestHandle(defaultHandler));
        }

        public IService(string queueName)
        {
            endpoint = new SubscriptionEndpoint(/*_connection , _model,*/ queueName);
            Handlers.Add("default", new RequestHandle(defaultHandler));
        }
        public IService(string queueName, string port)
        {
            endpoint = new SubscriptionEndpoint("localhost", int.Parse(port), queueName);
            Handlers.Add("default", new RequestHandle(defaultHandler));
        }

        #region Handlers
        private void defaultHandler(QueueRequest req)
        {

        }
        #endregion
    }   
}


/*TODO : 
 * remove thread
 */
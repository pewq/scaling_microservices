using System.Threading;
using RabbitMQ.Client;
using Newtonsoft.Json;

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
        //protected abstract void ThreadFunction();
        protected abstract string ProcessRequest(QueueRequest request);
        //public void Start()
        //{
        //    thread = new Thread(ThreadFunction);
        //    thread.Start();
        //}
        //public void Stop()
        //{
        //    thread.Abort();
        //}

        public IService()
        {
            endpoint = new SubscriptionEndpoint();
            ThisInit();
        }

        public IService(string queueName)
        {
            endpoint = new SubscriptionEndpoint(/*_connection , _model,*/ queueName);
            ThisInit();
        }
        public IService(string queueName, string port)
        {
            endpoint = new SubscriptionEndpoint("localhost", int.Parse(port), queueName);
            ThisInit();
        }

        private void ThisInit()
        {
            Handlers.Add("default", new RequestHandle(defaultHandler));
            ResponseHandler += ResponseHandlerFun;
        }

        #region Handlers
        private void defaultHandler(QueueRequest req)
        {

        }

        protected delegate void ResponseHandlerDelegate(IBasicProperties props, object body);

        protected event ResponseHandlerDelegate ResponseHandler = null;

        protected void ResponseHandlerFun(IBasicProperties DestinationProps, object responseBody)
        {
            Message msg = new Message();
            msg.StringBody = JsonConvert.SerializeObject(responseBody);
            endpoint.SendTo(msg, DestinationProps.ReplyTo);
        }
        #endregion
    }   
}


/*TODO : 
 * remove thread
 */
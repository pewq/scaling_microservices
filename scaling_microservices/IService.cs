using System.Collections.Generic;
using System.Threading;
using scaling_microservices.Rabbit;

namespace scaling_microservices
{
    /// <summary>
    /// provides subscription to ancestor classes
    /// </summary>
    public abstract class IService
    {
        protected string connectionString;//database connection string
        protected SubscriptionEndpoint endpoint { get; private set; }
        protected Thread thread;

        protected EventDictionary<Dictionary<string,string>> Handlers { get; private set; } 
            = new EventDictionary<Dictionary<string,string>>();
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
            Handlers.Add("default", new Handler<Dictionary<string,string>>(defaultHandler));
        }

        public IService(string queueName)
        {
            endpoint = new SubscriptionEndpoint(/*_connection , _model,*/ queueName);
            Handlers.Add("default", new Handler<Dictionary<string, string>>(defaultHandler));
        }
        public IService(string queueName, string port)
        {
            endpoint = new SubscriptionEndpoint("localhost", int.Parse(port), queueName);
            Handlers.Add("default", new Handler<Dictionary<string, string>>(defaultHandler));
        }

        #region Handlers
        private void defaultHandler(Dictionary<string,string> args)
        {

        }
    }
}


/*TODO : 
 * add event handlers
 * add default handler
 * add field to lock on
 */

/* //prop1
 * p.handler += new handler(DelegateMethod1())
 * p.handler += new handler(DelegateMethod2())
 * 
 * p.handle(args);//should call method1 then method2; this behaviour needs overriding
 *                //so that only one delegate will execute
 * 
 * //prop2
 * p.handlers["ping"] = new handler(PingDelegate());
 * p.handlers["data"] = new handler(DataDelegate());
 * 
 * p.handle(request.method, request.args as EventArgs);
 * //in this case everything is fine.
 * //maybe even pass request as single param.
 * 
 * maybe implement some mechanism to stop after first delegate was called
 * or just foreach
 * ????
 * troubles with default handler possible
 */
using System;
using RabbitMQ.Client;
using Newtonsoft.Json;

namespace scaling_microservices.Rabbit
{
    /// <summary>
    /// provides subscription to ancestor classes
    /// </summary>
    public abstract class IService
    {
        protected string connectionString;//database connection string
        protected EventingEndpoint endpoint { get; private set; }

        protected EventDictionary<RequestHandleDelegate> Handlers { get; private set; } 
            = new EventDictionary<RequestHandleDelegate>();
        protected virtual void ProcessRequest(QueueRequest request)
        {
            RequestHandleDelegate handle;
            try
            {
                if (null != (handle = (RequestHandleDelegate)Handlers[request.method]))
                {
                    handle(request);
                }
            }
            catch (Exception e)
            {
                OnException(e, request);
            }
        }

        public IService()
        {
            endpoint = new EventingEndpoint();
            ThisInit();
        }

        public IService(string queueName)
        {
            endpoint = new EventingEndpoint(/*_connection , _model,*/ queueName);
            ThisInit();
        }
        public IService(string queueName, string port)
        {
            endpoint = new EventingEndpoint("localhost", int.Parse(port), queueName);
            ThisInit();
        }



        /// <summary>
        /// common part for all constructors
        /// </summary>
        private void ThisInit()
        {
            endpoint.OnRecieved += Endpoint_OnRecieved;
            Handlers.Add("default", new RequestHandleDelegate(DefaultHandlerFun));
            ResponseEvent += __responseHandlerFun;
            ExceptionEvent += ExceptionHandlerFun;
            OnRequest += ProcessRequest;
        }

        #region Handlers
        protected delegate void RequestHandleDelegate(QueueRequest req);

        protected event RequestHandleDelegate OnRequest = null;

        protected virtual void DefaultHandlerFun(QueueRequest req)
        {

        }
        //handle for endpoint recieved
        private void Endpoint_OnRecieved(object sender, Message e)
        {
            QueueRequest request = new QueueRequest(e.body, e.properties);
            OnRequest(request);
        }
        #region ExceptionHandler
        protected delegate void ExceptionHandlerDelegate(object o, QueueRequest req, Exception e);

        protected event ExceptionHandlerDelegate ExceptionEvent = null;

        protected void OnException(Exception e, QueueRequest req)
        {
            ExceptionEvent(this, req, e);
        }
        protected virtual void ExceptionHandlerFun(object o, QueueRequest req, Exception e)
        {
            
        }
        #endregion
        #region Response Handler
        protected delegate void ResponseHandlerDelegate(object o, IBasicProperties props, object body);

        protected event ResponseHandlerDelegate ResponseEvent = null;

        protected void OnResponse(IBasicProperties props, object body)
        {
            this.ResponseEvent(this, props, body);
        }

        private void __responseHandlerFun(object o, IBasicProperties DestinationProps, object responseBody)
        {
            Message msg = new Message();
            msg.StringBody = JsonConvert.SerializeObject(responseBody);
            endpoint.SendTo(msg, DestinationProps.ReplyTo);
        }
        #endregion
        #endregion
    }
}
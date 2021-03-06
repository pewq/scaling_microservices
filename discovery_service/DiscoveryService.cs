﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using scaling_microservices.Rabbit;
using scaling_microservices.Registry;
using scaling_microservices.Proxy;

namespace discovery_service
{
    /// <note>singleton Instance, listens to queue with @QueueName</note>
    /// methods : 
    /// get_services()
    /// get_all_data()
    /// ping(name,token)
    /// register(name,address,token,type)
    /// send_message(message)
    public class DiscoveryService : IService
    {
        const int statusOK = 200;

        private ServiceRegistry registry { get; set; }

        private string BroadcastExchange = "disc_fanout_exchange";
        private EventingEndpoint broadcastEndpoint;

        public DiscoveryService(string queueName) : base(queueName)
        {
            this.registry = new ServiceRegistry();
            ThisInit();
        }
        private DiscoveryService(string queueName, string port)
            : this(queueName, int.Parse(port))
        {
            this.registry = new ServiceRegistry();
            ThisInit();
        }
        private DiscoveryService(string queueName, int port) :
            base(queueName, port.ToString())
        {
            this.Port = port;
            registry = new ServiceRegistry();
            ThisInit();
        }

        public int Port { get; private set; }
        //public static DiscoveryService Instance { get; private set; }
        public const string QueueName = "DiscoveryCommandQueue";
        static DiscoveryService()
        {
            //Instance = new DiscoveryService(QueueName);
        }

        private void ThisInit()
        {
            this.broadcastEndpoint = new EventingEndpoint();
            broadcastEndpoint.ExchangeExistsOrDeclare(BroadcastExchange, "fanout");
            //bind endpoint.Queue to echange
            broadcastEndpoint.Bind(BroadcastExchange, "");
            broadcastEndpoint.OnRecieved += (obj, message) => {
                if (message.Properties.ReplyTo == endpoint.InQueue || message.Properties.ReplyTo == broadcastEndpoint.InQueue)
                {
                    return;
                }
                if (message.Encoding == QueueRequest.classname)
                {
                    QueueRequest req = new QueueRequest(message.body, message.Properties);
                    base.ProcessRequest(req);
                }
            };


            this.Handlers.Add("ping", (RequestHandleDelegate)pingHandler);
            this.Handlers.Add("register", (RequestHandleDelegate)registerHandler);
            this.Handlers.Add("get_services", (RequestHandleDelegate)getServicesHandler);
            this.Handlers.Add("get_all_data", (RequestHandleDelegate)getAllHandler);
            this.Handlers.Add("is_alive", (RequestHandleDelegate)isAliveHandler);
            this.Handlers.Add("broadcast", (RequestHandleDelegate)broadcastHandler);
            registerToClient();
        }

        private void registerToClient()
        {
            var req = new QueueRequest() { method = "register_discovery" };
            req.arguments.Add("address", this.endpoint.InQueue);
            endpoint.SendTo(req, client_service.ClientService.QueueName);
        }

        #region Handlers
        private void pingHandler(QueueRequest req)
        {
            try
            {
                string name = req["name"];
                string token = req["token"];
                if (registry.Ping(name, token))
                { 
                    OnResponseWBCast(req.properties, new { status = System.Net.HttpStatusCode.OK }, req);
                }
                else
                {
                    OnResponse(req.properties, new { status = System.Net.HttpStatusCode.NotFound , message = "service was not registered or timed out" });
                }
            }
            catch (Exception e)
            {
                OnException(e, req);
            }
        }

        private void registerHandler(QueueRequest req)
        {
            try
            {
                string name = req["name"];
                string token = req["token"];
                string address = req["address"];
                string type = req["type"];
                string owner = req["owner"];

                registry.Add(name, address, token, type, owner);

                OnResponseWBCast(req.properties, new { status = statusOK }, req);
            }
            catch (Exception e)
            {
                OnException(e, req);
            }
        }
         
        private void getServicesHandler(QueueRequest req)
        {
            try
            {
                //Debug.Assert(req.arguments.Count == 0);
                var services = registry.GetServices();

                OnResponse(req.properties, services);
            }
            catch (Exception e)
            {
                OnException(e, req);
            }
        }

        private void getAllHandler(QueueRequest req)
        {
            try
            {
                var data = registry.Get();

                OnResponse(req.properties, data);
            }
            catch (Exception e)
            {
                OnException(e, req);
            }
        }
        private void isAliveHandler(QueueRequest req)
        {
            try
            {
                OnResponse(req.properties, new { is_alive = true });
            }
            catch (Exception e)
            {
                OnException(e, req);
            }
        }

        private void broadcastHandler(QueueRequest req)
        {
            //TODO : idk, how this should respond. or should it at all?
            List<string> targetTypes = new List<string>();
            List<string> targetServices = new List<string>();
            if(req.arguments.ContainsKey("target_types"))
            {
                targetTypes = JsonConvert.DeserializeObject<List<string>>(req["target_types"]);
            }
            else
            {
                targetTypes = registry.GetTypes();
                if(req.arguments.ContainsKey("exclude_types"))
                {
                    List<string> excludeTypes = JsonConvert.DeserializeObject<List<string>>(req["exclude_types"]);
                    targetTypes = targetTypes.Except(excludeTypes).ToList();
                }
            }
            try
            {
                targetServices = registry.GetServices(targetTypes, req["owner"]);
                targetServices.ForEach(t =>
                {
                    var proxy = new BasicProxy(t, "");
                    proxy.Send(req);
                });
            }
            catch(Exception e)
            {
                OnException(e, req);
            }
        }

        private void broadcastDiscoveryHandler(QueueRequest req)
        {

        }
        #endregion

        #region OnException
        //TODO : implement handler (mb base handler & remove override)
        protected override void ExceptionHandlerFun(object o, QueueRequest req, Exception e)
        {
            base.ExceptionHandlerFun(o, req, e);
        }

        #endregion

        protected void OnResponseWBCast(RabbitMQ.Client.IBasicProperties props, object response, QueueRequest bcastRequest)
        {
            endpoint.SendTo(bcastRequest, "", BroadcastExchange);
            base.OnResponse(props, response);
        }
    }
}
//TODO: controller should send it's address on register;
//discovery should register to client service on creation of endpoint;

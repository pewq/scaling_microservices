using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using scaling_microservices;
using scaling_microservices.Rabbit;
using discovery_service;


namespace client_service
{
    class ClientService : IService
    {
        Thread sendThread;

        Timer timer;

        public ClientService() : base()
        {
            timer = new Timer(
                    (a) =>
                    {
                        var req = new QueueRequest() { method = "ping" };
                        req.arguments.Add("name", endpoint.InQueue);
                        req.arguments.Add("token", "");
                        endpoint.SendTo(req, DiscoveryService.QueueName);
                    },
                    null,
                    0,
                    30* 1000
                );
            sendThread = new Thread(() =>
            {
                while (true)
                {
                    var msgLine = Console.ReadLine();
                    var request = new QueueRequest() { method = "send_message" };
                    request.arguments.Add("message", msgLine);
                    endpoint.SendTo(request, DiscoveryService.QueueName);
                }
            });
            sendThread.Start();
            var registerReq = new QueueRequest() { method = "register" };
            registerReq.arguments.Add("name", base.endpoint.InQueue);
            registerReq.arguments.Add("address", base.endpoint.InQueue);
            registerReq.arguments.Add("token", "");
            registerReq.arguments.Add("type", "");
            endpoint.SendTo(registerReq, DiscoveryService.QueueName);
        }

    }
}

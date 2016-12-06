using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using scaling_microservices;
using System.Threading;

namespace client_service
{
    class ClientService : scaling_microservices.IService
    {
        Thread inoutThread;

        Timer timer;

        public ClientService() : base()
        {
            timer = new Timer(
                    (a) =>
                    {
                        var req = new QueueRequest() { method = "ping" };
                        req.arguments.Add("name", endpoint.InQueue);
                        endpoint.SendTo(req, DiscoveryService.QueueName);
                    },
                    null,
                    0,
                    30* 1000
                );
            inoutThread = new Thread(() =>
            {
                while (true)
                {
                    var msg = endpoint.Recieve();
                    Console.WriteLine(msg.StringBody);
                }
            });
            inoutThread.Start();
        }

        protected override string ProcessRequest(QueueRequest request)
        {
            return "";
        }

        protected override void ThreadFunction()
        {
            while(true)
            {
                var msgLine = Console.ReadLine();
                var request = new QueueRequest() { method = "send_message" };
                request.arguments.Add("message", msgLine);
                endpoint.SendTo(request, DiscoveryService.QueueName);
            }
        }
    }
}

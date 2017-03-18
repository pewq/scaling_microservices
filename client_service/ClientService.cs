using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using scaling_microservices;
using scaling_microservices.Rabbit;


namespace client_service
{
    class ClientService : scaling_microservices.IService
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
        }

        protected override string ProcessRequest(QueueRequest request)
        {
            return "";
        }

        protected override void ThreadFunction()
        {
            while(true)
            {
                var msg = endpoint.Recieve();
                Console.WriteLine(msg.StringBody);
            }
        }
    }
}

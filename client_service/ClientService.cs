using System;
using scaling_microservices.Rabbit;

namespace client_service
{
    public class ClientService : IService
    {
        public const string QueueName = "ClientCommandQueue";

        //TODO: implement locking mechanism for whole service
        //when no discovery service is found
        bool discoveryFoundFlag;
        string discoveryServicePath;

        public ClientService() : base(QueueName)
        {
            ThisInit();
        }

        private void ThisInit()
        {
            Handlers.Add("register_discovery", (RequestHandleDelegate)discoveryRegisterHandler);
        }
        
        private bool CheckDiscoveryHealth()
        {
            var ep = new SubscriptionEndpoint();

            ep.SendTo(new QueueRequest() { method = "is_alive" }, discoveryServicePath);
            var response = ep.Recieve(1 * 1000);
            return !ReferenceEquals(null, response);
        }

        #region Handlers
        private void discoveryRegisterHandler(QueueRequest req)
        {
            try
            {
                discoveryServicePath = req["address"];
                Console.WriteLine(discoveryServicePath);
            }
            catch(Exception e)
            {
                OnException(e, req);
            }
        }

        #endregion

    }
}

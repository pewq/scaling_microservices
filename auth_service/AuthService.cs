using scaling_microservices.Rabbit;

namespace auth_service
{
    class AuthService : IService
    {
        public AuthService() : base()
        {
            ThisInit();
        }

        public AuthService(string queueName) : base(queueName)
        {
            ThisInit();
        }



        private void ThisInit()
        {
            //Init db here;
        }
    }
}

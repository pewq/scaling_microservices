using scaling_microservices.Auth.Identity;

namespace scaling_microservices.Auth
{
    class UAuthServices : IUAuthServices
    {
        //private readonly UserRepository repository;
        
        public UAuthServices(/*UserRepository repo*/)
        {
            //repository = repo;
        }

        /// <summary>
        /// Public method to authenticate user by user name and password.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public int Authenticate(string userName, string password)
        {
            var user = new AuthenticationIdentity("user", "pwd") { UserId = 1 };//repository.Get(u => u.UserName == userName && u.Password == password);
            if (user != null && user.UserId > 0)
            {
                return user.UserId;
            }
            return 0;
        }
    }
}
//idk, what is Authenticate
//TODO : move this to auth service
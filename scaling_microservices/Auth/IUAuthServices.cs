namespace scaling_microservices.Auth
{
    interface IUAuthServices
    {
        int Authenticate(string userName, string password);
    }
}

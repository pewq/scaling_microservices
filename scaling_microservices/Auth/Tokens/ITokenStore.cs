using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scaling_microservices.Auth.Tokens
{
    public interface ITokenStore
    {
        TokenEntity GenerateToken(int userId, string[] roles = null);

        bool ValidateToken(string tokenId);
        
        bool RemoveUser(int userId);
    }
}

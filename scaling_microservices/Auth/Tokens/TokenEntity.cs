﻿using System;
using System.Linq;
namespace scaling_microservices.Auth.Tokens
{
    public class TokenEntity
    {
        public int UserId { get; set; }
        public string AuthToken { get; set; }
        public DateTime IssuedOn { get; set; }
        public DateTime ExpiresOn { get; set; }

        public string[] Roles { get; set; }

        public TokenEntity() { }

        public TokenEntity(TokenEntity t)
        {
            UserId = t.UserId;
            AuthToken = t.AuthToken;
            IssuedOn = t.IssuedOn;
            ExpiresOn = t.ExpiresOn;
            Roles = t.Roles?.ToArray();
        }
    }
}

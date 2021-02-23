using System;

namespace dic.sso.identityserver.oauth.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } 
    }
}
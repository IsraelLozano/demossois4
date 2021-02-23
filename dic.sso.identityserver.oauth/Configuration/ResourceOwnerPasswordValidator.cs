using dic.sso.identityserver.oauth.Helpers;
using dic.sso.identityserver.oauth.Repositories;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dic.sso.identityserver.oauth.Configuration
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private IUserRepository userRepository;

        public ResourceOwnerPasswordValidator(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var user = await userRepository.GetAsync(context.UserName, HashHelper.Sha512(context.Password + context.UserName));
            if (user!=null)
            {
                context.Result = new GrantValidationResult(user.Id.ToString(), authenticationMethod: "custom");
            }
            else
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid Credentials");

            }
        }
    }
}

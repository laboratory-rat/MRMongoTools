using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MRMongoTools.Extensions.Identity.Component;
using MRMongoTools.Extensions.Identity.Interface;

namespace MRMongoTools.Extensions.Identity.Manager
{
    public class MRUserManager : UserManager<MRUser>
    {
        public MRUserManager(
            IMRUserStore<MRUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<MRUser> passwordHasher,
            IEnumerable<IUserValidator<MRUser>> userValidators,
            IEnumerable<IPasswordValidator<MRUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors, 
            IServiceProvider services,
            ILogger<UserManager<MRUser>> logger) 
                : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }
    }
}

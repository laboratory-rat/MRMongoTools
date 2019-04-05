using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MRMongoTools.Extensions.Identity.Component;
using MRMongoTools.Extensions.Identity.Interface;

namespace MRMongoTools.Extensions.Identity.Manager
{
    public class MRRoleManager : RoleManager<MRRole>
    {
        public MRRoleManager(IMRRoleStore store, IEnumerable<IRoleValidator<MRRole>> roleValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<MRRole>> logger) : base(store, roleValidators, keyNormalizer, errors, logger)
        {
        }
    }
}

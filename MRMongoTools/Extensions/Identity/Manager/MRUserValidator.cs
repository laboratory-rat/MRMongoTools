using Microsoft.AspNetCore.Identity;
using MRMongoTools.Extensions.Identity.Component;
using System.Threading.Tasks;

namespace MRMongoTools.Extensions.Identity.Manager
{
    class MRUserValidator<TUser> : UserValidator<TUser>, IUserValidator<TUser>
        where TUser : MRUser, new()
    {
        public MRUserValidator(IdentityErrorDescriber errors = null) : base(errors) { }

        public override Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
            => Task.FromResult(IdentityResult.Success);
    }
}

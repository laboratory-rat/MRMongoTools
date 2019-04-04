using Microsoft.AspNetCore.Identity;
using MRMongoTools.Extensions.Identity.Component;
using MRMongoTools.Infrastructure.Interface;

namespace MRMongoTools.Extensions.Identity.Interface
{
    public interface IMRUserStore : IMRUserStore<MRUser> { }

    public interface IMRUserStore<U> : 
        IRepository<U>, 
        IUserStore<U>, 
        IUserLoginStore<U>,
        IUserClaimStore<U>,
        IUserPasswordStore<U>,
        IUserEmailStore<U>,
        IUserSecurityStampStore<U>,
        IUserTwoFactorStore<U>,
        IUserRoleStore<U>
        where U : MRUser, new()
    {
    }
}

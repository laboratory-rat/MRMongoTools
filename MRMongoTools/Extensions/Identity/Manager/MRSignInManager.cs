using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MRMongoTools.Extensions.Identity.Component;

namespace MRMongoTools.Extensions.Identity.Manager
{
    public class MRSignInManager<TUser> : SignInManager<TUser>
        where TUser : MRUser, new()
    {
        public MRSignInManager(MRUserManager<TUser> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<MRUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<TUser>> logger, IAuthenticationSchemeProvider schemes) : base(userManager, contextAccessor, null, optionsAccessor, logger, schemes)
        {
        }
    }
}

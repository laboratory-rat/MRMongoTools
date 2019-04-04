using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MRMongoTools.Extensions.Identity.Component;

namespace MRMongoTools.Extensions.Identity.Manager
{
    public class MRSignInManager : SignInManager<MRUser>
    {
        public MRSignInManager(MRUserManager userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<MRUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<MRUser>> logger, IAuthenticationSchemeProvider schemes) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes)
        {
        }
    }
}

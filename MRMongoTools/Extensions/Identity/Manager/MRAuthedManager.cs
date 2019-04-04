using Microsoft.AspNetCore.Http;
using MRMongoTools.Extensions.Identity.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace MRMongoTools.Extensions.Identity.Manager
{
    public abstract class MRAuthedManager
    {
        protected readonly IHttpContextAccessor _httpContextAccessor;

        protected string _currentUserEmail => _httpContextAccessor.HttpContext.User?.FindFirst(ClaimsIdentity.DefaultNameClaimType)?.Value;
        protected string _currentUserId => _httpContextAccessor.HttpContext.User?.FindFirst(MRClaimSettings.ID)?.Value;
        protected List<string> _currentUserRoles => _httpContextAccessor.HttpContext.User?.FindAll(ClaimsIdentity.DefaultRoleClaimType)?.Select(x => x.Value).ToList() ?? new List<string>();

        protected bool _isInRole(string roleName) => _currentUserRoles.Contains(roleName);

        public MRAuthedManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
    }
}

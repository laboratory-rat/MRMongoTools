using Microsoft.AspNetCore.Http;
using MRMongoTools.Extensions.Identity.Settings;
using System.Collections.Generic;
using System.Linq;

namespace MRMongoTools.Extensions.Identity.Manager
{
    public class MRIdentityManager
    {
        protected readonly IHttpContextAccessor _httpContextAccessor;

        protected string _currentUserId => _httpContextAccessor.HttpContext.User?.FindFirst(MRTokenClaims.Id)?.Value ?? null;
        protected string _currentUserEmail => _httpContextAccessor.HttpContext.User?.FindFirst(MRTokenClaims.Email)?.Value ?? null;
        protected List<string> _currentUserRoles => _httpContextAccessor.HttpContext.User?.FindAll(MRTokenClaims.Role)?.Select(x => x.Value).ToList() ?? new List<string>();

        protected bool _isInRole(string role) => _currentUserRoles.Contains(role);

        public MRIdentityManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
    }
}

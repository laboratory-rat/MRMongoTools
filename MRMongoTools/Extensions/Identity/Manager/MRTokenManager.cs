using Microsoft.IdentityModel.Tokens;
using MRMongoTools.Extensions.Identity.Component;
using MRMongoTools.Extensions.Identity.Settings;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace MRMongoTools.Extensions.Identity.Manager
{
    public class MRTokenManager<TUser>
        where TUser : MRUser, new()
    {
        protected readonly MRTokenSettings _settings;

        public MRTokenManager(MRTokenSettings settings)
        {
            _settings = settings;
        }

        public virtual Tuple<string, DateTime> Generate(TUser user, IEnumerable<string> roles)
        {
            var identity = GetIdentity(user, roles);

            var now = DateTime.UtcNow;
            var expires = now.Add(TimeSpan.FromSeconds(_settings.Lifetime));

            var jwt = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                notBefore: now,
                expires: expires,
                claims: identity.Claims,
                signingCredentials: new SigningCredentials(MRTokenSettings.GetSymmetricSecurityKey(_settings.Key), SecurityAlgorithms.HmacSha256));

            return new Tuple<string, DateTime>(new JwtSecurityTokenHandler().WriteToken(jwt), expires);
        }

        public virtual List<Claim> GetClaims(TUser user, IEnumerable<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(MRClaimSettings.ID, user.Id)
            };

            if (roles != null && roles.Any())
            {
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role));
                }
            }

            return claims;
        }

        protected virtual ClaimsIdentity GetIdentity(TUser user, IEnumerable<string> roles)
            => new ClaimsIdentity(GetClaims(user, roles), "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
    }
}

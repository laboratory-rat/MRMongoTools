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
    public class MRTokenManager
    {
        protected readonly MRTokenSettings Settings;

        public MRTokenManager(MRTokenSettings tokenSettings)
        {
            Settings = tokenSettings;
        }

        public virtual string Generate(MRUser user, IEnumerable<string> roles)
        {
            var identity = GetIdentity(user, roles);

            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                issuer: Settings.Issuer,
                audience: Settings.Audience,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(Settings.Lifetime)),
                signingCredentials: new SigningCredentials(MRTokenSettings.GetSymmetricSecurityKey(Settings.Key), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        protected virtual ClaimsIdentity GetIdentity(MRUser user, IEnumerable<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(MRTokenClaims.Email, user.Email),
                new Claim(MRTokenClaims.FirstName, user.FirstName),
                new Claim(MRTokenClaims.LastName, user.LastName),
                new Claim(MRTokenClaims.Id, user.Id),
            };

            if(roles != null && roles.Any())
                claims.AddRange(roles.Select(x => new Claim(MRTokenClaims.Role, x)));

            return new ClaimsIdentity(claims, "Token", MRTokenClaims.Email, MRTokenClaims.Role);
        }
    }
}

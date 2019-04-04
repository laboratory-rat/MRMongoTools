using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MRMongoTools.Extensions.Identity.Settings
{
    public class MRTokenSettings
    {
        /// <summary>
        /// Issuer
        /// </summary>
        public string Issuer;

        /// <summary>
        /// Validate issuer
        /// </summary>
        public bool ValidateIssuer { get; set; }

        /// <summary>
        /// Audience
        /// </summary>
        public string Audience;

        /// <summary>
        /// Key
        /// </summary>
        public string Key;

        /// <summary>
        /// Lifetimes (in minutes)
        /// </summary>
        public int Lifetime;

        /// <summary>
        /// Require https
        /// </summary>
        public bool RequireHttps { get; set; }

        /// <summary>
        /// Validate lifetime
        /// </summary>
        public bool ValidateLifetime { get; internal set; }

        /// <summary>
        /// Validate audience
        /// </summary>
        public bool ValidateAudience { get; internal set; }
        public bool ValidateSigningKey { get; internal set; }

        public static SymmetricSecurityKey GetSymmetricSecurityKey(string key)
            => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
    }
}

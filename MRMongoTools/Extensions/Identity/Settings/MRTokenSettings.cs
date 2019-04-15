using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MRMongoTools.Extensions.Identity.Settings
{
    public class MRTokenSettings
    {
        /// <summary>
        /// Issuer
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Validate issuer
        /// </summary>
        public bool ValidateIssuer { get; set; }

        /// <summary>
        /// Audience
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Lifetimes (in minutes)
        /// </summary>
        public int Lifetime { get; set; }

        /// <summary>
        /// Require https
        /// </summary>
        public bool RequireHttps { get; set; }

        /// <summary>
        /// Validate lifetime
        /// </summary>
        public bool ValidateLifetime { get; set; }

        /// <summary>
        /// Validate audience
        /// </summary>
        public bool ValidateAudience { get; set; }

        public bool ValidateSigningKey { get; set; }

        public static SymmetricSecurityKey GetSymmetricSecurityKey(string key)
            => new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
    }
}

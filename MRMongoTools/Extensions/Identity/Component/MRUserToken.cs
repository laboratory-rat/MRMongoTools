using System;

namespace MRMongoTools.Extensions.Identity.Component
{
    public class MRUserToken
    {
        public string Issuer { get; set; }
        public string Value { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;
        public DateTime? ExpireTime { get; set; }
    }
}

using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MRMongoTools.Component;
using MRMongoTools.Extensions.Identity.Enum;
using MRMongoTools.Infrastructure.Attr;
using MRMongoTools.Infrastructure.Interface;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace MRMongoTools.Extensions.Identity.Component
{
    [CollectionAttr("User")]
    public class MRUser : MREntity, IEntity, IUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }

        [BsonRepresentation(BsonType.String)]
        public UserSex Sex { get; set; } = UserSex.UNDEFINED;

        public string NormalizedEmail { get; set; }
        public string Email { get; set; }
        public bool IsEmailConfirmed { get; set; }

        public bool Isblocked { get; set; }
        public string BlockReason { get; set; }
        public int FailedLoginCount { get; set; }

        public MRUserImage Image { get; set; }

        public List<MRUserTel> Tels { get; set; } = new List<MRUserTel>();
        public List<Claim> Claims { get; set; } = new List<Claim>();
        public List<MRUserToken> Tokens { get; set; } = new List<MRUserToken>();
        public List<Microsoft.AspNetCore.Identity.UserLoginInfo> Logins { get; set; } = new List<Microsoft.AspNetCore.Identity.UserLoginInfo>();
        public List<MRUserRole> Roles { get; set; } = new List<MRUserRole>();

        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; } = Guid.NewGuid().ToString();
        public bool TwoFactorEnabled { get; set; }
    }
}
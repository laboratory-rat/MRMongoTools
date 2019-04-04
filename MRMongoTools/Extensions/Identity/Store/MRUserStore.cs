using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using MRMongoTools.Component;
using MRMongoTools.Extensions.Identity.Component;
using MRMongoTools.Extensions.Identity.Interface;
using MRMongoTools.Infrastructure.Settings;
using MRMongoTools.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MREntityState = MRMongoTools.Infrastructure.Enum.EntityState;

namespace MRMongoTools.Extensions.Identity.Store
{
    public class MRUserStore : Repository<MRUser>, IMRUserStore
    {
        public MRUserStore(MRDatabaseConnectionSettings settings) : base(settings) { }

        protected override QueryBuilder<MRUser> _builder =>
            new QueryBuilder<MRUser>()
            .ProjectionExclude(x => x.BlockReason)
            .ProjectionExclude(x => x.Claims)
            .ProjectionExclude(x => x.FailedLoginCount)
            .ProjectionExclude(x => x.Logins)
            .ProjectionExclude(x => x.Roles)
            .ProjectionExclude(x => x.SecurityStamp)
            .ProjectionExclude(x => x.Tokens)
            .ProjectionExclude(x => x.TwoFactorEnabled)
            .ProjectionExclude(x => x.PasswordHash);

        protected virtual QueryBuilder<MRUser> _clearBuilder => new QueryBuilder<MRUser>();

        #region User

        public Task<string> GetUserIdAsync(MRUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.Id);

        public Task<string> GetUserNameAsync(MRUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.UserName);

        public async Task SetUserNameAsync(MRUser user, string userName, CancellationToken cancellationToken)
            => await UpdateByQuery(_builder.Eq(x => x.Id, user.Id).UpdateSet(x => x.UserName, userName));

        public Task<string> GetNormalizedUserNameAsync(MRUser user, CancellationToken cancellationToken)
            => Task.FromResult(user.NormalizedUserName);

        public async Task SetNormalizedUserNameAsync(MRUser user, string normalizedName, CancellationToken cancellationToken)
            => await UpdateByQuery(_builder.Eq(x => x.Id, user.Id).UpdateSet(x => x.NormalizedUserName, normalizedName)); 

        public async Task<IdentityResult> CreateAsync(MRUser user, CancellationToken cancellationToken)
        {
            if(await Any(x => x.Email == user.Email))
            {
                return IdentityResult.Failed(new IdentityError[]
                {
                    new IdentityError
                    {
                        Code = "1",
                        Description = $"User with email {user.Email} already exists"
                    }
                });
            }

            await Insert(user);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(MRUser user, CancellationToken cancellationToken)
        {
            await DeleteSoft(user);
            return IdentityResult.Success;
        }

        public async Task<MRUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
            => await Get(x => x.Id, userId);

        public async Task<MRUser> FindByNameAsync(string userName, CancellationToken cancellationToken)
            => await Get(x => x.UserName, userName);

        public async Task<MRUser> FindByEmail(string email)
            => await Get(x => x.NormalizedEmail, email.ToUpperInvariant());

        public async Task<IdentityResult> UpdateAsync(MRUser user, CancellationToken cancellationToken)
        {
            await UpdateByQuery(_clearBuilder.Eq(x => x.Id, user.Id)
                            .UpdateSet(x => x.FirstName, user.FirstName)
                            .UpdateSet(x => x.LastName, user.LastName)
                            .UpdateSet(x => x.Image, user.Image)
                            .UpdateSet(x => x.Sex, user.Sex)
                            .UpdateSet(x => x.Tels, user.Tels)
                            .UpdateSet(x => x.UpdateTime, DateTime.UtcNow));

            return IdentityResult.Success;
        }

        #endregion

        #region login

        public async Task AddLoginAsync(MRUser user, UserLoginInfo login, CancellationToken cancellationToken)
            => await UpdateByQuery(_builder.Eq(x => x.Id, user.Id).UpdateAddToSet(x => x.Logins, login));

        public async Task RemoveLoginAsync(MRUser user,  string loginProvider, string providerKey, CancellationToken cancellationToken)
            => await UpdateByQuery(_builder.Eq(x => x.Id, user.Id).UpdatePullWhere(x => x.Logins, x => x.LoginProvider == loginProvider && x.ProviderKey == providerKey));

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(MRUser user, CancellationToken cancellationToken)
        {
            List<UserLoginInfo> result;
            if (user.Logins != null)
            {
                result = user.Logins;
            }
            else
            {
                result = (await _collection.Find(_builder.Eq(x => x.Id, user.Id).Filter).Project<MRUser>(_builder.ProjectionBuilder.Include(x => x.Id).Include(x => x.CreateTime).Include(x => x.UpdateTime).Include(x => x.Logins)).FirstOrDefaultAsync())?.Logins;
            }

            return result;
        }

        public async Task<MRUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
            => await GetByQueryFirst(_clearBuilder.Match(x => x.Logins, x => x.LoginProvider == loginProvider && x.ProviderKey == providerKey));

        #endregion

        #region claims

        public async Task<IList<Claim>> GetClaimsAsync(MRUser user, CancellationToken cancellationToken)
        {
            if (user.Claims != null && user.Claims.Any())
                return user.Claims;

            return (await _collection.Find(_builder.Eq(x => x.Id, user.Id).Filter).Project<MRUser>(_builder.ProjectionBuilder.Include(x => x.Id).Include(x => x.CreateTime).Include(x => x.Claims)).FirstOrDefaultAsync())?.Claims ?? new List<Claim>();
        }

        public async Task<IList<MRUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
            => (await GetByQuery(_builder.Match(x => x.Claims, x => x.Issuer == claim.Issuer)))?.ToList();

        public async Task AddClaimsAsync(MRUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            await UpdateByQuery(_clearBuilder.Eq(x => x.Id, user.Id).UpdateAddToSetEach(x => x.Claims, claims));
        }

        public async Task ReplaceClaimAsync(MRUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            await UpdateByQuery(_clearBuilder.Eq(x => x.Id, user.Id).UpdatePull(x => x.Claims, claim).UpdatePush(x => x.Claims, newClaim));
        }

        public async Task RemoveClaimsAsync(MRUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            var query = _clearBuilder.Eq(x => x.Id, user.Id);
            foreach(var claim in claims)
            {
                query = query.UpdatePull(x => x.Claims, claim);
            }

            await UpdateByQuery(query);
        }

        #endregion

        #region password

        public async Task SetPasswordHashAsync(MRUser user, string passwordHash, CancellationToken cancellationToken)
            => await UpdateByQuery(_builder.Eq(x => x.Id, user.Id).UpdateSet(x => x.PasswordHash, passwordHash));

        public async Task<string> GetPasswordHashAsync(MRUser user, CancellationToken cancellationToken)
            => (await GetByQueryFirst(_clearBuilder.Eq(x => x.Id, user.Id).ProjectionInclude(x => x.PasswordHash)))?.PasswordHash;

        public async Task<bool> HasPasswordAsync(MRUser user, CancellationToken cancellationToken)
            => !string.IsNullOrWhiteSpace((await GetByQueryFirst(_clearBuilder.Eq(x => x.Id, user.Id).ProjectionInclude(x => x.PasswordHash)))?.PasswordHash);

        #endregion

        #region email

        public async Task SetEmailAsync(MRUser user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            await UpdateByQuery(_builder.Eq(x => x.Id, user.Id).UpdateSet(x => x.Email, email));
        }

        public async Task SetNormalizedEmailAsync(MRUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            await UpdateByQuery(_builder.Eq(x => x.Id, user.Id).UpdateSet(x => x.NormalizedEmail, normalizedEmail));

        }

        public async Task<string> GetEmailAsync(MRUser user, CancellationToken cancellationToken)
            => (await GetByQueryFirst(_builder.Eq(x => x.Id, user.Id).ProjectionInclude(x => x.Email)))?.Email;

        public async Task<string> GetNormalizedEmailAsync(MRUser user, CancellationToken cancellationToken)
            => (await GetByQueryFirst(_clearBuilder.Eq(x => x.Id, user.Id)))?.NormalizedEmail;

        public async Task<bool> GetEmailConfirmedAsync(MRUser user, CancellationToken cancellationToken)
            => (await GetByQueryFirst(_clearBuilder.Eq(x => x.Id, user.Id).ProjectionInclude(x => x.IsEmailConfirmed)))?.IsEmailConfirmed ?? false;

        public async Task SetEmailConfirmedAsync(MRUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.IsEmailConfirmed = confirmed;
            await UpdateByQuery(_builder.Eq(x => x.Id, user.Id).UpdateSet(x => x.IsEmailConfirmed, confirmed));
        }

        public async Task<MRUser> FindByEmailAsync(string email, CancellationToken cancellationToken)
            => await GetFirst(x => x.NormalizedEmail == email.ToUpperInvariant());

        #endregion

        #region security stamp

        public async Task SetSecurityStampAsync(MRUser user, string stamp, CancellationToken cancellationToken)
        {
            user.SecurityStamp = stamp;
            await UpdateByQuery(_builder.Eq(x => x.Id, user.Id).UpdateSet(x => x.SecurityStamp, stamp));
        }

        public async Task<string> GetSecurityStampAsync(MRUser user, CancellationToken cancellationToken)
            => (await GetByQueryFirst(_clearBuilder.Eq(x => x.Id, user.Id).ProjectionInclude(x => x.SecurityStamp)))?.SecurityStamp;

        #endregion

        #region two factor

        public async Task SetTwoFactorEnabledAsync(MRUser user, bool enabled, CancellationToken cancellationToken)
        {
            user.TwoFactorEnabled = enabled;
            await UpdateByQuery(_builder.Eq(x => x.Id, user.Id).UpdateSet(x => x.TwoFactorEnabled, enabled));
        }

        public async Task<bool> GetTwoFactorEnabledAsync(MRUser user, CancellationToken cancellationToken)
            => (await GetByQueryFirst(_clearBuilder.Eq(x => x.Id, user.Id).ProjectionInclude(x => x.TwoFactorEnabled)))?.TwoFactorEnabled ?? false;

        #endregion

        #region role

        public async Task<IList<string>> GetRolesAsync(MRUser user, CancellationToken cancellationToken)
            => (await GetByQueryFirst(_clearBuilder.Eq(x => x.Id, user.Id).ProjectionInclude(x => x.Roles)))?.Roles.Select(x => x.Name).ToList() ?? new List<string>();

        public async Task AddToRoleAsync(MRUser user, string roleName, CancellationToken cancellationToken)
        {
            var roles = await GetRolesAsync(user, cancellationToken);
            if (roles == null || !roles.Any())
            {
                await UpdateByQuery(_builder.Eq(x => x.Id, user.Id).UpdateSet(x => x.Roles, new List<MRUserRole>
                {
                    new MRUserRole
                    {
                        CreatedTime = DateTime.UtcNow,
                        Name = roleName,
                        NormalizedName = roleName.ToUpperInvariant()
                    }
                }));
            }
            else if (!roles.Any(x => x.ToUpperInvariant() == roleName.ToUpperInvariant()))
            {
                await UpdateByQuery(_builder.Eq(x => x.Id, user.Id).UpdatePush(x => x.Roles, new MRUserRole
                {
                    CreatedTime = DateTime.UtcNow,
                    Name = roleName,
                    NormalizedName = roleName.ToUpperInvariant()
                }));
            }
        }

        public async Task RemoveFromRoleAsync(MRUser user, string roleName, CancellationToken cancellationToken)
        {
            var roles = await GetRolesAsync(user, cancellationToken);
            if (roles != null && roles.Any(x => x.ToUpperInvariant() == roleName.ToUpperInvariant()))
            {
                await UpdateByQuery(_builder.Eq(x => x.Id, user.Id).UpdatePullWhere(x => x.Roles, x => x.NormalizedName == roleName.ToUpperInvariant()));
            }
        }

        public async Task<bool> IsInRoleAsync(MRUser user, string roleName, CancellationToken cancellationToken)
        {
            var roles = await GetRolesAsync(user, cancellationToken);
            if (roles == null || roles.Any()) return false;
            return roles.Any(x => x.ToUpperInvariant() == roleName.ToUpperInvariant());
        }

        public async Task<IList<MRUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
            => (await GetByQuery(_builder.Where(x => x.Roles != null).Match(x => x.Roles, x => x.Name == roleName)))?.ToList();

        #endregion

        public void Dispose() { }
    }
}

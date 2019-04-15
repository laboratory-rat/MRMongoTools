using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MRMongoTools.Extensions.Identity.Component;
using MRMongoTools.Extensions.Identity.Interface;
using MRMongoTools.Extensions.Identity.Manager;
using MRMongoTools.Extensions.Identity.Settings;
using MRMongoTools.Extensions.Identity.Store;
using MRMongoTools.Infrastructure.Settings;
using System;

namespace MRMongoTools.Extensions.Identity.Service
{
    public static class ServiceStartup
    {
        public static void AddMRMongoIdentity
            <TUser, TUserStore, TUserManager>(this IServiceCollection services, MRDatabaseConnectionSettings settings, MRTokenSettings tokenSettings, Action<IdentityOptions> userSignupActions = null)
            where TUser : MRUser, new()
            where TUserStore : MRUserStore<TUser>
            where TUserManager : MRUserManager<TUser>
        {
            services.AddSingleton(settings);
            services.AddSingleton(tokenSettings);

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient<IMRUserStore<TUser>, TUserStore>();
            services.AddTransient<IMRRoleStore, MRRoleStore>();
            services.AddTransient<IUserValidator<TUser>, MRUserValidator<TUser>>();

            services.AddTransient<MRRoleManager>();
            services.AddTransient<TUserManager>();
            services.AddTransient<MRSignInManager<TUser>>();
            services.AddSingleton<MRTokenManager<TUser>>();

            services.AddSingleton<MRTokenManager>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = tokenSettings.RequireHttps;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = tokenSettings.ValidateIssuer,
                        ValidIssuer = tokenSettings.Issuer,
                        ValidateAudience = tokenSettings.ValidateAudience,
                        ValidAudience = tokenSettings.Audience,
                        ValidateLifetime = tokenSettings.ValidateLifetime,
                        IssuerSigningKey = MRTokenSettings.GetSymmetricSecurityKey(tokenSettings.Key),
                        ValidateIssuerSigningKey = tokenSettings.ValidateSigningKey,
                    };
                });

            userSignupActions = userSignupActions ?? new Action<IdentityOptions>((a) => {
                a.User.RequireUniqueEmail = true;
            });

            services.AddIdentityCore<TUser>(userSignupActions)
                .AddDefaultTokenProviders();
        }
    }
}

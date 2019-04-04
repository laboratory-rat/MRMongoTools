using Microsoft.AspNetCore.Identity;
using MRMongoTools.Extensions.Identity.Component;
using MRMongoTools.Infrastructure.Interface;

namespace MRMongoTools.Extensions.Identity.Interface
{
    public interface IMRRoleStore : IRepository<MRRole>, IRoleStore<MRRole> { }
}

using Microsoft.AspNet.Identity;
using MRMongoTools.Extensions.Identity.Component;
using MRMongoTools.Extensions.Identity.Interface;

namespace MRMongoTools.Extensions.Identity.Manager
{
    public class MRRoleManager : RoleManager<MRRole>
    {
        public MRRoleManager(IRoleStore<MRRole, string> store) : base(store)
        {
        }
    }
}

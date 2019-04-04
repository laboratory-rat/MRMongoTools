using Microsoft.AspNet.Identity;
using MRMongoTools.Component;
using MRMongoTools.Infrastructure.Attr;
using MRMongoTools.Infrastructure.Interface;

namespace MRMongoTools.Extensions.Identity.Component
{
    [CollectionAttr("Role")]
    public class MRRole : Entity, IEntity, IRole
    {
        public string Name { get; set; }
        public string NormalizedName { get; set; }
    }
}

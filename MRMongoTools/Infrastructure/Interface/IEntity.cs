using MRMongoTools.Infrastructure.Enum;
using System;

namespace MRMongoTools.Infrastructure.Interface
{
    public interface IEntity
    {
        string Id { get; set; }
        DateTime CreateTime { get; set; }
        DateTime? UpdateTime { get; set; }
        EntityState State { get; set; }
    }
}

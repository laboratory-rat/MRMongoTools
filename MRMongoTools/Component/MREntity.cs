using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MRMongoTools.Infrastructure.Enum;
using MRMongoTools.Infrastructure.Interface;
using System;

namespace MRMongoTools.Component
{
    public abstract class MREntity : IEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        [BsonRepresentation(BsonType.String)]
        public EntityState State { get; set; }
    }
}

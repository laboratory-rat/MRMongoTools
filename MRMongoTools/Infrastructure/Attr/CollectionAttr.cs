using System;
namespace MRMongoTools.Infrastructure.Attr
{
    public class CollectionAttr : Attribute
    {
        public string Name { get; set; }

        public CollectionAttr(string name)
        {
            Name = name;
        }
    }
}

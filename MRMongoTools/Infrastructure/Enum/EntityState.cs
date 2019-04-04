using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MRMongoTools.Infrastructure.Enum
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EntityState
    {
        Active = 0,
        Archived,
        None
    }
}

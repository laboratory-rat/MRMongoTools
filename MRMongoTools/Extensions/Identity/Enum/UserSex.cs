using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MRMongoTools.Extensions.Identity.Enum
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum UserSex
    {
        UNDEFINED = 0,
        MALE,
        FIMALE
    }
}

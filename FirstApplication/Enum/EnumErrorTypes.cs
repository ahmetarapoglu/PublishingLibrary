using System.Text.Json.Serialization;

namespace BookShop.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EnumErrorTypes
    {
        Danger = 1,
        Success = 2,
        Warning = 3,
    }
}

using System.Text.Json.Serialization;

namespace BookShop.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EnumFileType
    {
       Image  = 10,
       File  = 20,
    }
}

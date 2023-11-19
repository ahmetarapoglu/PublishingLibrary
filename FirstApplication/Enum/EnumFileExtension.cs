using System.Text.Json.Serialization;

namespace BookShop.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EnumFileExtension
    {
       Webp  = 10,
       Jpeg  = 20,
       Png   = 30,
       Bmp   = 40,
       pbm   = 50,
       Gif   = 60,
       Tga   = 70,
       Tiff  = 80,
       other = 90
    }
}

using BookShop.Enum;

namespace BookShop.Services
{
    public class Error
    {
        public EnumErrorTypes? Key { get; set; }
        public string? Description { get; set; }
        public string? Title { get; set; }
        public string? Code { get; set; }

        public Error()
        {
        }
        public Error(string? description) {
            Description = description;
        }
        public Error(string? code,string? description)
        {
            Code = code;
            Description = description;
        }
        public Error(string? title, string? code, string? description)
        {
            Code = code;
            Title = title;
            Description = description;
        }
        public Error(EnumErrorTypes? key,string? title, string? code, string? description)
        {
            Key = key;
            Title = title;
            Code = code;
            Description = description;
        }
    }
}

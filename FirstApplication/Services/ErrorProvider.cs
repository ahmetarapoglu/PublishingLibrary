using BookShop.Enum;

namespace BookShop.Services
{
    public static class ErrorProvider
    {
        public static Error NotValid = new() { Code = "100", Description = "Hello", Key = EnumErrorTypes.Danger, Title = "Error-401" };
        public static Error DataNotFound = new() { Code = "100", Description = "DataNotFound", Key = EnumErrorTypes.Danger, Title = "DataNotFound" };
    }
}

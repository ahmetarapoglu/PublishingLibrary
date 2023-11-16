using FluentValidation;

namespace BookShop.Services
{
    public class AbstractValidatorBase<T> : AbstractValidator<T>
    {
        public static string NotEmpty (string Name)
        {
            return "Please enter" + Name +  "name";
        }
    }
}

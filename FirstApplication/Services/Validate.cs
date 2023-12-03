using BookShop.Enum;
using FluentValidation.Results;

namespace BookShop.Services
{
    public static class ValidationHelper
    {
        public static void Validate<T>(this T entity, AbstractValidatorBase<T> validator) where T : class
        {

            ValidationResult result = validator.Validate(entity);

            if (!result.IsValid)
            {
                var errors = result.Errors.Select(i => new Error
                {
                    Key = EnumErrorTypes.Danger,
                    Code = i.ErrorCode,
                    Title = i.PropertyName,
                    Description = i.ErrorMessage,
                }).ToList();

                throw new OzelException(errors);
            }

        }
    }
}

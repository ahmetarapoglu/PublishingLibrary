using BookShop.Enum;
using FluentValidation;

namespace BookShop.Services
{
    public interface IValidation<T> where T : class
    {
        public void Validator(T model);
    }
    public class Validation<T> : IValidation<T> where T : class
    {
        private readonly IValidator<T> _requestValidator;

        public Validation(IValidator<T> requestValidator)
        {
            _requestValidator = requestValidator;
        }

        public void Validator(T model)
        {

            var validation = _requestValidator.Validate(model);

            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(i => new Error
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

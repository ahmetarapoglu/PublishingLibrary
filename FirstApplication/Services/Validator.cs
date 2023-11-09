using BookShop.Models.RequestModels;
using FluentValidation;

namespace BookShop.Services
{
    public class Validator<T> where T : class
    {
        private readonly IValidator<T> _requestValidator;

        public Validator()
        {
        }
        public Validator(IValidator<T> requestValidator)
        {
            _requestValidator = requestValidator;
        }

        public void Validations(T model) {

            var validation = _requestValidator.Validate(model);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(error => new Error
                {
                    Key = Enum.EnumErrorTypes.Danger,
                    Code = "400",
                    Title = error.PropertyName,
                    Description = error.ErrorMessage,
                }).ToList();

                throw new OzelException(errors);
            }
        }

    }
}

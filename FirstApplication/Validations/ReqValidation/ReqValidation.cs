using BookShop.Models.RequestModels;
using FluentValidation;

namespace BookShop.Validations.ReqValidation
{

    public class DataTableReqValidation : AbstractValidator<DataTableRequest>
    {
        public DataTableReqValidation()
        {
            RuleFor(i => i.CurrentPage)
                //.NotEmpty().WithMessage("CurrentPage is required.")
                .GreaterThan(0).WithMessage("CurrentPage must be greater than 0.");

            RuleFor(i => i.PageSize)
                //.NotEmpty().WithMessage("PageSize is required.")
                .GreaterThan(0).WithMessage("PageSize must be greater than 0.");
        }
    }
}

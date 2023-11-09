using BookShop.Models.RequestModels;
using FluentValidation;

namespace BookShop.Validations.ReqValidation
{

    public class DataTableReqValidation : AbstractValidator<DataTableRequest>
    {
        public DataTableReqValidation()
        {
            RuleFor(i => i.Current)
                .GreaterThan(0).WithMessage("CurrentPage must be greater than 0.");

            RuleFor(i => i.PageSize)
                .GreaterThan(0).WithMessage("PageSize must be greater than 0.");
        }
    }
}

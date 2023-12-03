using BookShop.Models.RequestModels;
using BookShop.Services;
using FluentValidation;

namespace BookShop.Validations.ReqValidation
{

    public class DataTableReqValidator : AbstractValidatorBase<DataTableRequest>
    {
        public DataTableReqValidator()
        {
            RuleFor(i => i.Current)
                .GreaterThan(0).WithMessage("CurrentPage must be greater than 0.").WithErrorCode("Re-01");

            RuleFor(i => i.PageSize)
                .GreaterThan(0).WithMessage("PageSize must be greater than 0.").WithErrorCode("Re-02");
        }
    }
}

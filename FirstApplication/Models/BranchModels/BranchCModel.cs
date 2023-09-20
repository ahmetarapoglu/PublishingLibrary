using BookShop.Entities;

namespace BookShop.Models.BranchModels
{
    public class BranchCModel : BranchModel
    {

        public static Func<BranchCModel, Branch> Fill => model => new Branch
        {
            BranchName = model.BranchName,
            BranchAddress = model.BranchAddress,
            PhoneNumber = model.PhoneNumber,
        };
    }
}

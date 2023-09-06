using BookShop.Entities;

namespace BookShop.Models.BranchModels
{
    public class BranchCUModel
    {
        public int Id { get; set; }
        public string BranchName { get; set; }
        public string BranchAddress { get; set; }
        public string PhoneNumber { get; set; }

        public static Func<BranchCUModel, Branch> Fill => model => new Branch
        {
            BranchName = model.BranchName,
            BranchAddress = model.BranchAddress,
            PhoneNumber = model.PhoneNumber,
        };
    }
}

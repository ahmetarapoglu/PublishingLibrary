using BookShop.Entities;
using BookShop.Models.OrderModels;

namespace BookShop.Models.BranchModels
{
    public class BranchRModel : BranchModel
    {
        public int Id { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal TotalPayment { get; set; }
        public decimal RemainingPayment { get; set; }

        public List<OrderRModel> Orders { get; set; }
    }
}

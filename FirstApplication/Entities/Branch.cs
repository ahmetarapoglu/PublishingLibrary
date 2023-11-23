using BookShop.Services;

namespace BookShop.Entities
{
    public class Branch : BaseEntity
    {
        public string BranchName { get; set; }
        public string BranchAddress { get; set; }
        public string PhoneNumber { get; set; }

        public List<BranchPayment> BranchPayments { get; set; }
        public List<Order> Orders { get; set; }

    }
}

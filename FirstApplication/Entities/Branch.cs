namespace BookShop.Entities
{
    public class Branch
    {
        public int Id { get; set; }
        public string BranchName { get; set; }
        public string BranchAddress { get; set; }
        public string PhoneNumber { get; set; }

        public List<BranchPayment> BranchPayments { get; set; }
        public List<Order> Orders { get; set; }

    }
}

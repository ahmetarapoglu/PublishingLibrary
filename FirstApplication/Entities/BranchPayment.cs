namespace BookShop.Entities
{
    public class BranchPayment
    {
        public int Id { get; set; }
        public int PaymentNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaidDate { get; set; }


        public int BranchId { get; set; }
        public Branch Branch { get; set; }
    }
}

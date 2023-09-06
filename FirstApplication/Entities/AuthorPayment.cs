namespace BookShop.Entities
{
    public class AuthorPayment
    {
        public int Id { get; set; }
        public int PaymentNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaidDate { get; set; }

        public int AuthorId { get; set; }
        public Author Author { get; set; }

    }
}

using BookShop.Entities;

namespace BookShop.Models.AuthorPaymentModels
{
    public class AuthorPaymentCModel
    {
        public int AuthorId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaidDate { get; set; }

        public static Func<AuthorPaymentCModel,int, AuthorPayment> Fill => (model ,number)
            => new AuthorPayment
            {
                AuthorId = model.AuthorId,
                Amount = model.Amount,
                PaidDate = model.PaidDate,
                PaymentNumber = number + 1,
            };
    }
}

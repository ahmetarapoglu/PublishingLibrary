using BookShop.Entities;
using BookShop.Models.AuthorPaymentModels;

namespace BookShop.Models.BranchPaymentModels
{
    public class BranchPaymentCModel
    {
        public int BranchId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaidDate { get; set; }
        public static Func<BranchPaymentCModel,int, BranchPayment> Fill => (model, number)
            => new BranchPayment
            {
                BranchId = model.BranchId,
                Amount = model.Amount,
                PaidDate = model.PaidDate,
                PaymentNumber = number + 1,
            };
    }
}

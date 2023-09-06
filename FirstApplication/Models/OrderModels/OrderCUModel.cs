using BookShop.Entities;

namespace BookShop.Models.OrderModels
{
    public class OrderCUModel
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int BookVersionId { get; set; }
        public int BookCount { get; set; }
        public static Func<OrderCUModel, Order> Fill => model => new Order
        {
            BranchId = model.BranchId,
            BookVersionId = model.BookVersionId,
            BookCount = model.BookCount
        };
    }
}

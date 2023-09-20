using BookShop.Entities;

namespace BookShop.Models.OrderModels
{
    public class OrderCModel : OrderModel
    {
        public static Func<OrderCModel, Order> Fill => model => new Order
        {
            BranchId = model.BranchId,
            BookVersionId = model.BookVersionId,
            BookCount = model.BookCount
        };
    }
}

using BookShop.Models.AuthorAddressModels;
using BookShop.Models.AuthorBiyografi;

namespace BookShop.Models.AuthorModels
{
    public class AuthorRModel : AuthorModel
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalPayment { get; set; }
        public decimal RemainingPayment { get; set; }
        public DateTime CreateDate { get; set; }

        public List<BookInAuthors> Books { get; set; }
    }
}

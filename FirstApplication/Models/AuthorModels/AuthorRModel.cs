using BookShop.Models.AuthorAddressModels;
using BookShop.Models.AuthorBiyografi;

namespace BookShop.Models.AuthorModels
{
    public class AuthorRModel
    {
        public int Id { get; set; }
        public string NameSurname { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalPayment { get; set; }
        public decimal RemainingPayment { get; set; }

        public AuthorAddressModel AuthorAddress { get; set; }
        public AuthorBiographyModel AuthorBiography { get; set; }
        public List<BookInAuthors> Books { get; set; }
    }
}

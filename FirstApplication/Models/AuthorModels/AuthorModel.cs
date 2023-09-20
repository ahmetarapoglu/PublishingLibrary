using BookShop.Models.AuthorAddressModels;
using BookShop.Models.AuthorBiyografi;

namespace BookShop.Models.AuthorModels
{
    public class AuthorModel
    {
        public string NameSurname { get; set; }
        public AuthorAddressModel AuthorAddress { get; set; }
        public AuthorBiographyModel AuthorBiography { get; set; }
    }
}

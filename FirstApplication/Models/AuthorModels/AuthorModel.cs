using BookShop.Models.AuthorAddressModels;
using BookShop.Models.AuthorBiyografi;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Models.AuthorModels
{
    public class AuthorModel
    {
        [Required(ErrorMessage = "AuthorName is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters.")]
        public string NameSurname { get; set; }
        public string Image { get; set; }
        public AuthorAddressModel AuthorAddress { get; set; }
        public AuthorBiographyModel AuthorBiography { get; set; }
    }
}

using BookShop.Entities;
using BookShop.Models.AuthorAddressModels;
using BookShop.Models.AuthorBiyografi;
using System.Diagnostics.Metrics;

namespace BookShop.Models.AuthorModels
{
    public class AuthorCUModel
    {
        public int Id { get; set; }
        public string NameSurname { get; set; }
        public decimal TotalPayment { get; set; }
        public AuthorAddressModel AuthorAddress { get; set; }
        public AuthorBiographyModel AuthorBiography { get; set; }

        public static Func<AuthorCUModel, Author> Fill => model =>
        new Author
        {
            Id = model.Id,
            NameSurname = model.NameSurname,
            TotalPayment = model.TotalPayment,
            AuthorAddress = new AuthorAddress
            {
                Country = model.AuthorAddress.Country,
                City = model.AuthorAddress.City,
                PostCode = model.AuthorAddress.PostCode
            },
            AuthorBiography = new AuthorBiography
            {
                Email = model.AuthorBiography.Email,
                PhoneNumber = model.AuthorBiography.PhoneNumber,
                NativeLanguage = model.AuthorBiography.NativeLanguage,
                Education = model.AuthorBiography.Education
            }
        };
    }
}

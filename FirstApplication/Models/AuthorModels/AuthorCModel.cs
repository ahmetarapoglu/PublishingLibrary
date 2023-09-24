using BookShop.Entities;
using BookShop.Models.AuthorAddressModels;
using BookShop.Models.AuthorBiyografi;

namespace BookShop.Models.AuthorModels
{
    public class AuthorCModel : AuthorModel
    {
        public static Func<AuthorCModel, Author> Fill => model =>
        new Author
        {
            NameSurname = model.NameSurname,
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

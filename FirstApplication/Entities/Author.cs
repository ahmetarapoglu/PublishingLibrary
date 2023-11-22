using BookShop.Services;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Entities
{
    public class Author : BaseEntity
    {
        public string NameSurname { get; set; }
        public string Image { get; set; }

        public AuthorAddress AuthorAddress { get; set; }
        public AuthorBiography AuthorBiography { get; set; }

        public List<BookAuthor> BookAuthors { get; set; }

        public List<AuthorPayment> AuthorPayments { get; set; }


    }
}

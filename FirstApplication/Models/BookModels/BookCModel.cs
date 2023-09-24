using BookShop.Entities;
using System.ComponentModel.DataAnnotations;
using Xunit.Abstractions;

namespace BookShop.Models.BookModels
{
    public class BookCModel : BookModel
    {
        [Required(ErrorMessage = "CategoryId is required")]
        public int CategoryId { get; set; }
        public static Func<BookCModel, Book> Fill => model =>
            new Book
            {
                Title = model.Title,
                Description = model.Description,
                PublishedDate = model.PublishedDate,
                CategoryId = model.CategoryId,
                BookAuthors = model.BookAuthors.Select(i => new BookAuthor
                {
                    AuthorId = i.AuthorId,
                    AuhorRatio = i.AuhorRatio,
                }).ToList(),
                BookVersions = new List<BookVersion>
                {
                    new BookVersion
                    {
                        Number = 1,
                        BookCount = model.BookVersions.BookCount,
                        CostPrice = model.BookVersions.CostPrice,
                        SellPrice = model.BookVersions.SellPrice,
                        LibraryRatio = model.BookVersions.LibraryRatio,
                    },
                }
            };

    }
}


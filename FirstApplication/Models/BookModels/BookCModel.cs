using BookShop.Entities;
using System.ComponentModel.DataAnnotations;
using Xunit.Abstractions;

namespace BookShop.Models.BookModels
{
    public class BookCModel : BookModel
    {
        public List<int> CategoriesId { get; set; }
    }
}


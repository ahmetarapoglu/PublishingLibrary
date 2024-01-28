using BookShop.Abstract;
using BookShop.Entities;
using BookShop.Models.BookModels;
using BookShop.Models.BookVersionModels;
using BookShop.Models.RequestModels;
using BookShop.Services;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace BookShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookController : ControllerBase
    {
        private readonly IRepository<Book> _bookRepository;
        private readonly IRepository<BookCategory> _bookCategoryRepository;
        private readonly IRepository<BookAuthor> _bookAuthorRepository;

        public BookController(
            IRepository<Book> bookRepository,
            IRepository<BookCategory> bookCategoryRepository,
            IRepository<BookAuthor> bookAuthorRepository)
        {
            _bookRepository = bookRepository;
            _bookCategoryRepository = bookCategoryRepository;
            _bookAuthorRepository = bookAuthorRepository;
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetBooks(BookRequest model)
        {
            try
            {
                //Where.
                Expression<Func<Book, bool>> filter = i => true;

                //Date(Filter).
                if (model.StartDate != null)
                    filter = filter.And(i => i.CreateDate.Date >= model.StartDate.Value.Date);

                if (model.EndDate != null)
                    filter = filter.And(i => i.CreateDate.Date <= model.EndDate.Value.Date);

                //Search.
                if (!string.IsNullOrEmpty(model.Search))
                    filter = filter.And(i => i.Title.Contains(model.Search));

                //Sort.
                Expression<Func<Book, object>> Order = model.Order switch
                {
                    "id" => i => i.Id,
                    "title" => i => i.Title,
                    "date" => i => i.CreateDate,
                    _ => i => i.Id,
                };

                //OrderBy.
                IOrderedQueryable<Book> orderBy(IQueryable<Book> i)
                   => model.SortDir == "ascend"
                   ? i.OrderBy(Order)
                   : i.OrderByDescending(Order);

                //Select
                static IQueryable<BookRModel> select(IQueryable<Book> query) => query.Select(entity => new BookRModel
                {
                    Id = entity.Id,
                    Title = entity.Title,
                    Description = entity.Description,
                    PublishedDate = entity.PublishedDate,
                    Cover = entity.Cover,
                    CreateDate = entity.CreateDate,
                    LibraryRatio = entity.LibraryRatio,
                    Categories = entity.BookCategories.Select(i => new BookCategoryModel
                    {
                        Id = i.Category.Id,
                        CategoryName = i.Category.CategoryName
                    }).ToList(),
                    BookAuthors = entity.BookAuthors.Select(i => new AuthorInBookModel
                    {
                        AuthorId = i.AuthorId,
                        AuhorRatio = i.AuhorRatio,
                    }).ToList(),

                    BookVersions = entity.BookVersions.Select(i =>
                    new BookVersionRModel
                    {
                        Id = i.Id,
                        BookCount = i.BookCount,
                        Number = i.Number,
                        CostPrice = i.CostPrice,
                        TotalCostPrice = i.CostPrice * i.BookCount,
                        SellPrice = i.SellPrice,
                        TotalSellPrice = i.SellPrice * i.BookCount,
                        ProfitTotal = i.ProfitTotal,
                    }).ToList(),

                });

                var (total, data) = await _bookRepository.GetListAndTotalAsync(select, filter, null, orderBy, skip: model.Skip, take: model.Take);

                return Ok(new { data, total });
            }
            catch (OzelException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            try
            {
                //Where
                Expression<Func<Book, bool>> filter = i => i.Id == id;

                //Select
                static IQueryable<BookRModel> select(IQueryable<Book> query) => query.Select(entity => new BookRModel
                {
                    Id = entity.Id,
                    Title = entity.Title,
                    Description = entity.Description,
                    PublishedDate = entity.PublishedDate,
                    Cover = entity.Cover,
                    LibraryRatio = entity.LibraryRatio,
                    CreateDate = entity.CreateDate,
                    Categories = entity.BookCategories.Select(i => new BookCategoryModel
                    {
                        Id = i.Category.Id,
                        CategoryName = i.Category.CategoryName
                    }).ToList(),
                    BookAuthors = entity.BookAuthors.Select(i => new AuthorInBookModel
                    {
                        AuthorId = i.AuthorId,
                        AuhorRatio = i.AuhorRatio,
                    }).ToList(),

                    BookVersions = entity.BookVersions.Select(i =>
                    new BookVersionRModel
                    {
                        Id = i.Id,
                        BookCount = i.BookCount,
                        Number = i.Number,
                        CostPrice = i.CostPrice,
                        TotalCostPrice = i.CostPrice * i.BookCount,
                        SellPrice = i.SellPrice,
                        TotalSellPrice = i.SellPrice * i.BookCount,
                        ProfitTotal = i.ProfitTotal,
                    }).ToList(),

                });

                var book = await _bookRepository.FindAsync(select, filter);

                return Ok(book);
            }
            catch (OzelException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateBook(BookCModel model)
        {
            try
            {
                var entity = new Book
                {
                    Title = model.Title,
                    Description = model.Description,
                    PublishedDate = model.PublishedDate,
                    Cover = model.Cover,
                    BookCategories = model.CategoriesId.Select(i => new BookCategory
                    {
                        CategoryId = i
                    }).ToList(),
                    CreateDate = DateTime.Now,
                    BookAuthors = model.BookAuthors.Select(i => new BookAuthor
                    {
                        AuthorId = i.AuthorId,
                        AuhorRatio = i.AuhorRatio,
                    }).ToList()
                };

                await _bookRepository.AddAsync(entity);

                return Ok();
            }
            catch (OzelException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UpdateBook(BookUModel model)
        {

            try
            {
                if (model.Id < 0 || model?.Id == null)
                    throw new Exception("Reauested Book Not Found!.");

                //Where
                Expression<Func<BookCategory, bool>> filter_BookCategory = i => i.BookId == model.Id;
                Expression<Func<BookAuthor, bool>> filter_BookAuthor = i => i.BookId == model.Id;

                await _bookCategoryRepository.DeleteRangeAsync(filter_BookCategory);
                await _bookAuthorRepository.DeleteRangeAsync(filter_BookAuthor);

                //Include.
                IIncludableQueryable<Book, object> include(IQueryable<Book> query) => query
                   .Include(i => i.BookAuthors)
                   .Include(i => i.BookCategories)
                   .Include(i => i.BookVersions);

                void action(Book book)
                {
                    book!.Title = model.Title;
                    book.Description = model.Description;
                    book.PublishedDate = model.PublishedDate;
                    book.Cover = model.Cover;
                    book.LibraryRatio = model.LibraryRatio;
                    book.BookCategories = model.CategoriesId.Select(categoryId => new BookCategory
                    {
                        CategoryId = categoryId,
                    }).ToList();

                    book.BookAuthors = model.BookAuthors.Select(i => new BookAuthor
                    {
                        AuthorId = i.AuthorId,
                        AuhorRatio = i.AuhorRatio,
                    }).ToList();
                }
                await _bookRepository.UpdateAsync(action, i => i.Id == model.Id, include);

                return Ok();
            }
            catch (OzelException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                //Where
                Expression<Func<Book, bool>> filter = i => i.Id == id;

                await _bookRepository.DeleteAsync(filter);

                return Ok();
            }
            catch (OzelException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

using BookShop.Db;
using BookShop.Entities;
using BookShop.Models.AuthorAddressModels;
using BookShop.Models.AuthorBiyografi;
using BookShop.Models.AuthorModels;
using BookShop.Models.BookModels;
using BookShop.Models.BookVersionModels;
using BookShop.Models.RequestModels;
using BookShop.Validations.ReqValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;

namespace BookShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookController : ControllerBase
    {
        private readonly AppDbContext _context;
        public BookController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("GetBooks")]
        public async Task<IActionResult> GetBooks(BookRequest model)
        {
            var dtrValidation = new DataTableReqValidation();
            var validationResult = dtrValidation.Validate(model);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    throw new Exception($"Error: {error.ErrorMessage}");
                }
            }

            try
            {
                var books = await _context.Books
                    .Include(i => i.Category)
                    .Include(i => i.BookAuthors)
                    .Include(i => i.BookVersions).ToListAsync();

                var data = books
                    .Where(i => i.Title.Contains(model.Search))
                    .Skip(model.Skip)
                    .Take(model.Take)
                    .Select(i => new BookRModel
                    {
                        Id = i.Id,
                        Title = i.Title,
                        Description = i.Description,
                        PublishedDate = i.PublishedDate,
                        CategoryName = i.Category.CategoryName,
                        BookAuthors = i.BookAuthors.Select(i=>new AuthorInBookModel
                        {
                            AuthorId = i.AuthorId,
                            AuhorRatio = i.AuhorRatio,
                        }).ToList(),

                        BookVersions =i.BookVersions.Select(i=>
                        new BookVersionRModel
                        {
                            Id=i.Id,
                            BookCount = i.BookCount,
                            Number = i.Number,
                            CostPrice = i.CostPrice,
                            TotalCostPrice = i.CostPrice * i.BookCount,
                            SellPrice = i.SellPrice,
                            TotalSellPrice = i.SellPrice * i.BookCount,
                            LibraryRatio = i.LibraryRatio
                        }).ToList(),

                    });

                return Ok(new { total = books.Count, data });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetBook/{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            try
            {
                var data = await _context.Books
                    .Include(i=>i.Category)
                    .Include(i => i.BookAuthors)
                    .Include(i => i.BookVersions)
                    .FirstOrDefaultAsync(i => i.Id == id) ?? throw new Exception($"Book With this id :{id} Not Found!.");

                var book = new BookRModel
                {
                    Id = data.Id,
                    Title = data.Title,
                    Description = data.Description,
                    PublishedDate = data.PublishedDate,
                    CategoryName = data.Category.CategoryName,
                    BookAuthors = data.BookAuthors.Select(i => new AuthorInBookModel
                    {
                        AuthorId = i.AuthorId,
                        AuhorRatio = i.AuhorRatio,
                    }).ToList(),

                    BookVersions = data.BookVersions.Select(i =>
                    new BookVersionRModel
                    {
                        Id=i.Id,
                        BookCount = i.BookCount,
                        Number = i.Number,
                        CostPrice = i.CostPrice,
                        TotalCostPrice = i.CostPrice * i.BookCount,
                        SellPrice = i.SellPrice,
                        TotalSellPrice = i.SellPrice * i.BookCount,
                        LibraryRatio = i.LibraryRatio,
                    }).ToList(),
                };
                return Ok(book);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("CreateBook")]
        public async Task<IActionResult> CreateBook(BookCModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                _context.Add(BookCModel.Fill(model));
                _context.SaveChanges();
                return Ok("Book Created Successfly!.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("CreateVersion")]
        public async Task<IActionResult> CreateVersion(NewVersionCUModel model)
        {
            try
            {
                if (model.BookId == 0 || model.BookId == null)
                {
                    throw new Exception("Reauested Book Not Found!.");
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var book = await _context.Books
                    .Include(i => i.BookVersions)
                    .FirstOrDefaultAsync(i => i.Id == model.BookId);

                if (book == null)
                {
                    throw new Exception("Requested Book Not Found!.");
                }

                var number = book.BookVersions.OrderByDescending(i => i.Id).Select(i => i.Number).FirstOrDefault();
                

                _context.Add(NewVersionCUModel.Fill(model,number));
                _context.SaveChanges();
                return Ok("Version Created Successfly!.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("UpdateBook")]
        public async Task<IActionResult> UpdateBook(BookUModel model)
        {
            try
            {
                if (model.Id < 0 || model.Id == null)
                {
                    throw new Exception("Reauested Book Not Found!.");
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var book = await _context.Books
                    .Include(i => i.Category)
                    .Include(i => i.BookAuthors)
                    .Include(i => i.BookVersions)
                    .FirstOrDefaultAsync(i => i.Id == model.Id);

                if (book == null)
                {
                    throw new Exception("Requested Book Not Found!.");
                }

                book.Title = model.Title;
                book.Description = model.Description;
                book.PublishedDate = model.PublishedDate;
                book.CategoryId = model.CategoryId;
                book.BookAuthors = model.BookAuthors.Select(i => new BookAuthor
                {
                    AuthorId = i.AuthorId,
                    AuhorRatio = i.AuhorRatio,
                }).ToList();
                book.BookVersions = new List<BookVersion>
                {
                    new BookVersion
                    {
                        BookCount = model.BookVersions.BookCount,
                        CostPrice = model.BookVersions.CostPrice,
                        SellPrice = model.BookVersions.SellPrice,
                        LibraryRatio = model.BookVersions.LibraryRatio,
                    },
                };

                _context.SaveChanges();
                return Ok("Book Updated Succefuly!.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteBook")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                var book = await _context.Books.FirstOrDefaultAsync(i => i.Id == id) ?? throw new Exception($"Book whith this ID:{id} Nof Found!.");
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
                return Ok("Book Removed Successfuly!.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

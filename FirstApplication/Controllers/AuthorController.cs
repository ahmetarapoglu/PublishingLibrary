using BookShop.Db;
using BookShop.Entities;
using BookShop.Models.AuthorAddressModels;
using BookShop.Models.AuthorBiyografi;
using BookShop.Models.AuthorModels;
using BookShop.Models.BookVersionModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthorController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AuthorController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        [Route("GetAuthors")]
        public async Task<IActionResult> GetAuthors()
        {
            try
            {
                var x = 0m;
                var y = 0m;

                var Authors = await _context.Authors
                    .Include(i => i.AuthorAddress)
                    .Include(i => i.AuthorBiography)
                    .Include(i => i.AuthorPayments)
                    .Include(i => i.BookAuthors)
                    .ThenInclude(i=>i.Book)
                    .ThenInclude(i=>i.BookVersions)
                    .ThenInclude(i=>i.Orders)
                  
                    .Select(i => new AuthorRModel
                    {
                        Id = i.Id,
                        NameSurname = i.NameSurname,
                        TotalAmount  = 0,
                        TotalPayment =  i.AuthorPayments.Sum(i => i.Amount),
                        RemainingPayment = x - y,
                        AuthorAddress = new AuthorAddressModel
                        {
                            Country = i.AuthorAddress.Country,
                            City = i.AuthorAddress.City,
                            PostCode = i.AuthorAddress.PostCode,
                        },
                        AuthorBiography = new AuthorBiographyModel
                        {
                            Email = i.AuthorBiography.Email,
                            PhoneNumber = i.AuthorBiography.PhoneNumber,
                            NativeLanguage = i.AuthorBiography.NativeLanguage,
                            Education = i.AuthorBiography.Education
                        },
                        Books = i.BookAuthors.Select(i => new BookInAuthors
                        {
                            Id = i.Book.Id,
                            Title = i.Book.Title,
                            Description = i.Book.Description,
                            PublishedDate = i.Book.PublishedDate,
                            CategoryName = i.Book.Category.CategoryName,
                            BookVersions = i.Book.BookVersions.Select(i =>
                            new BookVersionRModel
                            {
                                Id = i.Id,
                                Number = i.Number,
                                BookCount = i.BookCount,
                                CostPrice = i.CostPrice,
                                TotalCostPrice = i.CostPrice * i.BookCount,
                                SellPrice = i.SellPrice,
                                TotalSellPrice = i.SellPrice * i.BookCount,
                                LibraryRatio = i.LibraryRatio,
                            }).ToList()
                        }).ToList(),
                    }).ToListAsync();


                return Ok(Authors);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetAuthor/{id}")]
        public async Task<IActionResult> GetAuthor(int id)
        {
            try
            {
                var data = await _context.Authors
                    .Include(i => i.AuthorAddress)
                    .Include(i => i.AuthorBiography)
                    .Include(i => i.BookAuthors)
                    .ThenInclude(i => i.Book)
                    .ThenInclude(i => i.BookVersions)
                    .ThenInclude(i => i.Orders)
                    .Include(i => i.BookAuthors)
                    .ThenInclude(i => i.Book)
                    .ThenInclude(i => i.Category)
                    .Include(i => i.AuthorPayments)
                    .FirstOrDefaultAsync(i => i.Id == id) ?? throw new Exception($"Author With this id :{id} Not Found!.");

                var order = await _context.Orders.Include(i => i.BookVersion).Select(i => i.Id).ToListAsync();

                var x = 0m;
                var y = 0m;
                var author = new AuthorRModel
                {
                    Id = data.Id,
                    NameSurname = data.NameSurname,
                    TotalAmount = x = data.AuthorPayments.Sum(i => i.Amount),
                    TotalPayment = y = data.AuthorPayments.Sum(i => i.Amount),
                    RemainingPayment = x - y,
                    AuthorAddress = new AuthorAddressModel
                    {
                        Country = data.AuthorAddress.Country,
                        City = data.AuthorAddress.City,
                        PostCode = data.AuthorAddress.PostCode,
                    },
                    AuthorBiography = new AuthorBiographyModel
                    {
                        Email = data.AuthorBiography.Email,
                        PhoneNumber = data.AuthorBiography.PhoneNumber,
                        NativeLanguage = data.AuthorBiography.NativeLanguage,
                        Education = data.AuthorBiography.Education
                    },

                    Books = data.BookAuthors.Select(i => new BookInAuthors
                    {
                        Id = i.Book.Id,
                        Title = i.Book.Title,
                        Description = i.Book.Description,
                        PublishedDate = i.Book.PublishedDate,
                        CategoryName = i.Book.Category.CategoryName,
                        BookVersions = i.Book.BookVersions.Select(i =>
                        new BookVersionRModel
                        {
                            Id = i.Id,
                            Number = i.Number,
                            BookCount = i.BookCount,
                            CostPrice = i.CostPrice,
                            TotalCostPrice = i.CostPrice * i.BookCount,
                            SellPrice = i.SellPrice,
                            TotalSellPrice = i.SellPrice * i.BookCount,
                            LibraryRatio = i.LibraryRatio,
                        }).ToList()
                    }).ToList()
                };
                return Ok(author);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("CreateAuthor")]
        public async Task<IActionResult> CreateAuthor(AuthorCUModel model)
        {
            try
            {
                _context.Add(AuthorCUModel.Fill(model));
                _context.SaveChanges();
                return Ok("Author Created Successfly!.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("UpdateAuthor")]
        public async Task<IActionResult> UpdateAuthor(AuthorCUModel model)
        {
            try
            {
                if (model.Id == 0 || model.Id == null)
                {
                    throw new Exception("Reauested Author Not Found!.");
                }
                var author = await _context.Authors
                    .Include(i => i.AuthorAddress)
                    .Include(i => i.AuthorBiography)
                    .FirstOrDefaultAsync(i => i.Id == model.Id);

                if (author == null)
                {
                    throw new Exception("Requested Author Not Found!.");
                }
                author.NameSurname = model.NameSurname;
                author.AuthorAddress = new AuthorAddress
                {
                    Country = model.AuthorAddress.Country,
                    City = model.AuthorAddress.City,
                    PostCode = model.AuthorAddress.PostCode,
                };
                author.AuthorBiography = new AuthorBiography
                {
                    Email = model.AuthorBiography.Email,
                    PhoneNumber = model.AuthorBiography.PhoneNumber,
                    NativeLanguage = model.AuthorBiography.NativeLanguage,
                    Education = model.AuthorBiography.Education
                };
                _context.SaveChanges();
                return Ok("Author Updated Succefuly!.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteAuthor")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            try
            {
                var author = await _context.Authors.FirstOrDefaultAsync(i => i.Id == id) ?? throw new Exception($"Author whith this ID:{id} Nof Found!.");
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();
                return Ok("Author Removed Successfuly!.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

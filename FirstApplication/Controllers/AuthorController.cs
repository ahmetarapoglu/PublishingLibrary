using BookShop.Db;
using BookShop.Entities;
using BookShop.Models.AuthorAddressModels;
using BookShop.Models.AuthorBiyografi;
using BookShop.Models.AuthorModels;
using BookShop.Models.BookVersionModels;
using BookShop.Models.RequestModels;
using BookShop.Validations.ReqValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

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


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetAuthors(AuthorRequest model)
        {

            try
            {
                var authors = await _context.Authors
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
                    .Where(i => i.NameSurname.Contains(model.Search))
                    .OrderByDescending(i => i.Id)
                    .ToListAsync();

                var data = authors
                .Skip(model.Skip)
                .Take(model.Take)
                .Select(i => new AuthorRModel
                {
                    Id = i.Id,
                    NameSurname = i.NameSurname,
                    Image = i.Image,
                    TotalAmount = 0,
                    TotalPayment = i.AuthorPayments.Sum(i => i.Amount),
                    RemainingPayment = 0,
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
                });


                return Ok(new {total = authors.Count, data });
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
                    Image = data.Image,
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
        [Route("[action]")]
        public async Task<IActionResult> CreateAuthor(AuthorCModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _context.Add(AuthorCModel.Fill(model));
                 _context.SaveChanges();
                return Ok(new { status = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> UpdateAuthor(AuthorUModel model)
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
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                author.NameSurname = model.NameSurname;
                author.Image = model.Image;
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
                return Ok(new { status = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("[action]")]
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

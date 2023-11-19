using BookShop.Db;
using BookShop.Entities;
using BookShop.Models.BookModels;
using BookShop.Models.BookVersionModels;
using BookShop.Models.RequestModels;
using BookShop.Validations.ReqValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersionController : ControllerBase
    {
        private readonly AppDbContext _context;
        public VersionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetVersions(VersionRequest model)
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
                var versions = await _context.VersionBooks
                    .Where(i=>i.BookId==model.BookId)
                    .OrderByDescending(i => i.Id)
                    .ToListAsync();

                var data = versions
                    .Skip(model.Skip)
                    .Take(model.Take)
                    .Select(i => new BookVersionRModel
                        {
                           Id = i.Id,
                           Number = i.Number,
                           BookCount = i.BookCount,
                           CostPrice = i.CostPrice,
                           TotalCostPrice = i.CostPrice * i.BookCount,
                           SellPrice = i.SellPrice,
                           TotalSellPrice = i.SellPrice * i.BookCount,
                           LibraryRatio = i.LibraryRatio,
                        }
                    );

                return Ok(new { total = versions.Count, data });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<IActionResult> GetVersion(int id)
        {
            try
            {
                var data = await _context.VersionBooks
                    .FirstOrDefaultAsync(i => i.Id == id) ?? throw new Exception($"Version With this id :{id} Not Found!.");

                var book = new BookVersionRModel
                {
                    Id = data.Id,
                    BookCount = data.BookCount,
                    Number = data.Number,
                    CostPrice = data.CostPrice,
                    TotalCostPrice = data.CostPrice * data.BookCount,
                    SellPrice = data.SellPrice,
                    TotalSellPrice = data.SellPrice * data.BookCount,
                    LibraryRatio = data.LibraryRatio,
                };
              
                return Ok(book);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateVersion(BookVersionCModel model)
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


                _context.Add(BookVersionCModel.Fill(model, number));
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
        public async Task<IActionResult> UpdateVersion(BookVersionUModel model)
        {

            try
            {
                if (model.Id < 0 || model.Id == null)
                {
                    throw new Exception("Reauested Version Not Found!.");
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var version = await _context.VersionBooks
                    .FirstOrDefaultAsync(i => i.Id == model.Id);

                if (version == null)
                {
                    throw new Exception("Requested Version Not Found!.");
                }

                version.BookCount = model.BookCount;
                version.CostPrice = model.CostPrice;
                version.SellPrice = model.SellPrice;
                version.LibraryRatio = model.LibraryRatio;
               
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
        public async Task<IActionResult> DeleteVersion(int id)
        {
            try
            {
                var version = await _context.VersionBooks
                    .FirstOrDefaultAsync(i => i.Id == id)
                    ?? throw new Exception($"Version whith this ID:{id} Nof Found!.");

                _context.VersionBooks.Remove(version);
                await _context.SaveChangesAsync();
                return Ok("Version Removed Successfuly!.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}

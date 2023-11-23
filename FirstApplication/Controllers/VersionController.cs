using BookShop.Abstract;
using BookShop.Db;
using BookShop.Entities;
using BookShop.Models.AuthorAddressModels;
using BookShop.Models.AuthorBiyografi;
using BookShop.Models.AuthorModels;
using BookShop.Models.BookVersionModels;
using BookShop.Models.RequestModels;
using BookShop.Services;
using BookShop.Validations.ReqValidation;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersionController : ControllerBase
    {
        private readonly IRepository<BookVersion> _versionRepository;
        private readonly AppDbContext _context;

        public VersionController(
            IRepository<BookVersion> versionRepository,
            AppDbContext context)
        {
            _versionRepository = versionRepository;
            _context = context;

        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetVersions(VersionRequest model)
        {

            try
            {
                //Where.
                Expression<Func<BookVersion, bool>> filter = i => true;

                //Date(Filter).
                if (model.StartDate != null)
                    filter = filter.And(i => i.CreateDate.Date >= model.StartDate.Value.Date);

                if (model.EndDate != null)
                    filter = filter.And(i => i.CreateDate.Date <= model.EndDate.Value.Date);

                //Search.
                //if (!string.IsNullOrEmpty(model.Search))
                //    filter = filter.And(i => i.Number.Contains(model.Search));

                //Sort.
                Expression<Func<BookVersion, object>> Order = model.Order switch
                {
                    "id" => i => i.Id,
                    "number" => i => i.Number,
                    _ => i => i.Id,
                };

                //OrderBy.
                IOrderedQueryable<BookVersion> orderBy(IQueryable<BookVersion> i)
                   => model.SortDir == "ascend"
                   ? i.OrderBy(Order)
                   : i.OrderByDescending(Order);

                //Select
                static IQueryable<BookVersionRModel> select(IQueryable<BookVersion> query) => query
                    .Select(entity => new BookVersionRModel
                    {
                        Id = entity.Id,
                        BookCount = entity.BookCount,
                        Number = entity.Number,
                        CostPrice = entity.CostPrice,
                        TotalCostPrice = entity.CostPrice * entity.BookCount,
                        SellPrice = entity.SellPrice,
                        TotalSellPrice = entity.SellPrice * entity.BookCount,
                        LibraryRatio = entity.LibraryRatio,

                    });

                var (total, data) = await _versionRepository.GetListAndTotalAsync(select, filter, null, orderBy, skip: model.Skip, take: model.Take);

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
        public async Task<IActionResult> GetVersion(int id)
        {
            try
            {
                //Where
                Expression<Func<BookVersion, bool>> filter = i => i.Id == id;

                //Select
                static IQueryable<BookVersionRModel> select(IQueryable<BookVersion> query) => query
                    .Select(entity => new BookVersionRModel
                {
                    Id = entity.Id,
                    BookCount = entity.BookCount,
                    Number = entity.Number,
                    CostPrice = entity.CostPrice,
                    TotalCostPrice = entity.CostPrice * entity.BookCount,
                    SellPrice = entity.SellPrice,
                    TotalSellPrice = entity.SellPrice * entity.BookCount,
                    LibraryRatio = entity.LibraryRatio,

                });

                var version = await _versionRepository.FindAsync(select, filter);

                return Ok(version);
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
        public async Task<IActionResult> CreateVersion(BookVersionCModel model)
        {
            try
            {
                if (model.BookId == 0 || model.BookId == null)
                    throw new Exception("Reauested Book Not Found!.");
                

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);


                var book = await _context.Books
                    .Include(i => i.BookVersions)
                    .FirstOrDefaultAsync(i => i.Id == model.BookId);

                var number = book!.BookVersions.OrderByDescending(i => i.Id).Select(i => i.Number).FirstOrDefault();

                var entity = new BookVersion
                {
                    BookId = model.BookId,
                    Number = number + 1,
                    BookCount = model.BookCount,
                    CostPrice = model.CostPrice,
                    SellPrice = model.SellPrice,
                    LibraryRatio = model.LibraryRatio
                };

                await _versionRepository.AddAsync(entity);

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
        public async Task<IActionResult> UpdateVersion(BookVersionUModel model)
        {

            try
            {
                if (model.Id < 0 || model.Id == null)
                    throw new Exception("Reauested Version Not Found!.");
               
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                //Where
                Expression<Func<BookVersion, bool>> filter = i => i.Id == model.Id;

                var entity = await _versionRepository.FindAsync(filter);

                entity!.BookCount = model.BookCount;
                entity.CostPrice = model.CostPrice;
                entity.SellPrice = model.SellPrice;
                entity.LibraryRatio = model.LibraryRatio;

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
        public async Task<IActionResult> DeleteVersion(int id)
        {
            try
            {
                //Where
                Expression<Func<BookVersion, bool>> filter = i => i.Id == id;

                await _versionRepository.DeleteAsync(filter);

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

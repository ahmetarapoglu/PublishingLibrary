using BookShop.Abstract;
using BookShop.Entities;
using BookShop.Models.BranchModels;
using BookShop.Models.OrderModels;
using BookShop.Models.RequestModels;
using BookShop.Services;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace BookShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BranchController : ControllerBase
    {
        private readonly IRepository<Branch> _branchRepository;

        public BranchController(IRepository<Branch> branchRepository)
        {
            _branchRepository = branchRepository;
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetBranches(BranchRequest model)
        {

            try
            {
                //Where.
                Expression<Func<Branch, bool>> filter = i => true;

                //Date(Filter).
                if (model.StartDate != null)
                    filter = filter.And(i => i.CreateDate.Date >= model.StartDate.Value.Date);

                if (model.EndDate != null)
                    filter = filter.And(i => i.CreateDate.Date <= model.EndDate.Value.Date);

                //Search.
                if (!string.IsNullOrEmpty(model.Search))
                    filter = filter.And(i => i.BranchName.Contains(model.Search));

                //Sort.
                Expression<Func<Branch, object>> Order = model.Order switch
                {
                    "id" => i => i.Id,
                    "branchName" => i => i.BranchName,
                    _ => i => i.Id,
                };

                //OrderBy.
                IOrderedQueryable<Branch> orderBy(IQueryable<Branch> i)
                   => model.SortDir == "ascend"
                   ? i.OrderBy(Order)
                   : i.OrderByDescending(Order);

                //Select
                static IQueryable<BranchRModel> select(IQueryable<Branch> query) => query.Select(entity => new BranchRModel
                {
                    Id = entity.Id,
                    BranchAddress = entity.BranchAddress,
                    BranchName = entity.BranchName,
                    PhoneNumber = entity.PhoneNumber,
                    TotalAmount = entity.Orders.Sum(i => ((i.BookVersion.SellPrice - i.BookVersion.CostPrice) * i.BookCount)),
                    TotalPayment = entity.BranchPayments.Sum(i => i.Amount),
                    RemainingPayment = entity.Orders.Sum(i => ((i.BookVersion.SellPrice - i.BookVersion.CostPrice) * i.BookCount)) - entity.BranchPayments.Sum(i => i.Amount),
                    Orders = entity.Orders.Select(i => new OrderRModel
                    {
                        Id = i.Id,
                        BranchId = i.BranchId,
                        BookCount = i.BookCount,
                        BookVersionId = i.BookVersionId,
                        Total = (i.BookVersion.SellPrice - i.BookVersion.CostPrice) * i.BookCount
                    }).ToList(),
                });

                var (total, data) = await _branchRepository.GetListAndTotalAsync(select, filter, null, orderBy, skip: model.Skip, take: model.Take);

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
        public async Task<IActionResult> GetBranch(int id)
        {
            try
            {
                //Where
                Expression<Func<Branch, bool>> filter = i => i.Id == id;

                //Select
                static IQueryable<BranchRModel> select(IQueryable<Branch> query) => query.Select(entity => new BranchRModel
                {
                    Id = entity.Id,
                    BranchAddress = entity.BranchAddress,
                    BranchName = entity.BranchName,
                    PhoneNumber = entity.PhoneNumber,
                    TotalAmount = entity.Orders.Sum(i => ((i.BookVersion.SellPrice - i.BookVersion.CostPrice) * i.BookCount)),
                    TotalPayment = entity.BranchPayments.Sum(i => i.Amount),
                    RemainingPayment = entity.Orders.Sum(i => ((i.BookVersion.SellPrice - i.BookVersion.CostPrice) * i.BookCount)) - entity.BranchPayments.Sum(i => i.Amount),
                    Orders = entity.Orders.Select(i => new OrderRModel
                    {
                        Id = i.Id,
                        BranchId = i.BranchId,
                        BookCount = i.BookCount,
                        BookVersionId = i.BookVersionId,
                        Total = (i.BookVersion.SellPrice - i.BookVersion.CostPrice) * i.BookCount
                    }).ToList(),
                });

                var branch = await _branchRepository.FindAsync(select, filter);

                return Ok(branch);
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
        public async Task<IActionResult> CreateBranch(BranchCModel model)
        {
            try
            {
                var entity = new Branch
                {
                    BranchName = model.BranchName,
                    BranchAddress = model.BranchAddress,
                    PhoneNumber = model.PhoneNumber,
                    CreateDate = DateTime.Now,
                };

                await _branchRepository.AddAsync(entity);

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
        public async Task<IActionResult> UpdateBranch(BranchUModel model)
        {
            try
            {
                if (model.Id == 0 || model.Id == null)
                    throw new Exception("Reauested Author Not Found!.");

                //Where
                Expression<Func<Branch, bool>> filter = i => i.Id == model.Id;

                var entity = await _branchRepository.FindAsync(filter);

                entity!.BranchName = model.BranchName;
                entity.BranchAddress = model.BranchAddress;
                entity.PhoneNumber = model.PhoneNumber;

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
        public async Task<IActionResult> DeleteBranch(int id)
        {
            try
            {
                //Where
                Expression<Func<Branch, bool>> filter = i => i.Id == id;

                await _branchRepository.DeleteAsync(filter);

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

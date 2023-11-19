using BookShop.Db;
using BookShop.Entities;
using BookShop.Models.AuthorAddressModels;
using BookShop.Models.AuthorBiyografi;
using BookShop.Models.AuthorModels;
using BookShop.Models.BranchModels;
using BookShop.Models.OrderModels;
using BookShop.Models.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static NuGet.Packaging.PackagingConstants;

namespace BookShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BranchController : ControllerBase
    {
        private readonly AppDbContext _context;
        public BranchController(AppDbContext context)
        {
            _context = context;
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetBranches(BranchRequest model)
        {

            try
            {
                var x = 0m;
                var y = 0m;
                var branches = await _context.Branches
                    .Include(i => i.Orders).ThenInclude(i=>i.BookVersion)
                    .Include(i => i.BranchPayments)
                    .Where(i => i.BranchName.Contains(model.Search))
                    .OrderByDescending(i => i.Id)
                    .ToListAsync();

                var data = branches
                    .Skip(model.Skip)
                    .Take(model.Take)
                    .Select(i => new BranchRModel
                    {
                        Id = i.Id,
                        BranchAddress = i.BranchAddress,
                        BranchName = i.BranchName,
                        PhoneNumber = i.PhoneNumber,
                        TotalAmount = i.Orders.Sum(i=>((i.BookVersion.SellPrice - i.BookVersion.CostPrice) * i.BookCount)),
                        TotalPayment = i.BranchPayments.Sum(i=>i.Amount),
                        RemainingPayment = i.Orders.Sum(i => ((i.BookVersion.SellPrice - i.BookVersion.CostPrice) * i.BookCount)) - i.BranchPayments.Sum(i => i.Amount),
                        Orders = i.Orders.Select(i=>new OrderRModel
                        {
                            Id = i.Id,
                            BranchId = i.BranchId,
                            BookCount = i.BookCount,
                            BookVersionId = i.BookVersionId,
                            Total = (i.BookVersion.SellPrice - i.BookVersion.CostPrice) * i.BookCount
                        }).ToList(),
                    });

                return Ok(new { total = branches.Count, data });
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
                var data = await _context.Branches.Include(i => i.Orders).ThenInclude(i=>i.BookVersion).Include(i => i.BranchPayments).FirstOrDefaultAsync(i => i.Id == id) 
                    ?? throw new Exception($"Branch With this id :{id} Not Found!.");

                var branch = new BranchRModel
                {
                    Id = data.Id,
                    BranchAddress= data.BranchAddress,
                    BranchName = data.BranchName,
                    PhoneNumber = data.PhoneNumber,
                    TotalAmount = data.Orders.Sum(i => ((i.BookVersion.SellPrice - i.BookVersion.CostPrice) * i.BookCount)) ,
                    TotalPayment = data.BranchPayments.Sum(i => i.Amount),
                    RemainingPayment = data.Orders.Sum(i => ((i.BookVersion.SellPrice - i.BookVersion.CostPrice) * i.BookCount)) - data.BranchPayments.Sum(i => i.Amount),
                    Orders = data.Orders.Select(i => new OrderRModel
                    {
                        Id = i.Id,
                        BranchId = i.BranchId,
                        BookCount = i.BookCount,
                        BookVersionId = i.BookVersionId,
                        Total = (i.BookVersion.SellPrice - i.BookVersion.CostPrice) * i.BookCount
                    }).ToList(),
                };
                return Ok(branch);
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
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _context.Add(BranchCModel.Fill(model));
                _context.SaveChanges();
                return Ok(new { status = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UpdateBranch(BranchUModel model)
        {
            try
            {
                if (model.Id == 0 || model.Id == null)
                {
                    throw new Exception("Reauested Branch Not Found!.");
                }
                var branch = await _context.Branches.FirstOrDefaultAsync(i => i.Id == model.Id);

                if (branch == null)
                {
                    throw new Exception("Requested Branch Not Found!.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                 branch.BranchName = model.BranchName;
                 branch.BranchAddress = model.BranchAddress;
                 branch.PhoneNumber = model.PhoneNumber;

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
        public async Task<IActionResult> DeleteBranch(int id)
        {
            try
            {
                var branch = await _context.Branches.FirstOrDefaultAsync(i => i.Id == id) ?? throw new Exception($"Branch whith this ID:{id} Nof Found!.");
                _context.Branches.Remove(branch);
                await _context.SaveChangesAsync();
                return Ok("Branch Removed Successfuly!.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

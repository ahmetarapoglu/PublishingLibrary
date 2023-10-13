using BookShop.Db;
using BookShop.Entities;
using BookShop.Models.AuthorAddressModels;
using BookShop.Models.AuthorBiyografi;
using BookShop.Models.AuthorModels;
using BookShop.Models.OrderModels;
using BookShop.Models.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;
        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("GetOrders")]
        public async Task<IActionResult> GetOrders(OrderRequest model)
        {
            try
            {
                var orders = await _context.Orders.Include(i => i.Invoice).Include(i => i.BookVersion).Where(i => i.Branch.BranchName.Contains(model.Search)).ToListAsync();
                var data = orders
                  
                    .Skip(model.Skip)
                    .Take(model.Take)
                    .Select(i => new OrderRModel
                    {
                  Id = i.Id,
                  IsInvoiced = i.Invoice != null ? i.Invoice.IsInvoiced : false,
                  BranchId = i.BranchId,
                  BookVersionId = i.BookVersionId,
                  BookCount = i.BookCount,
                  Total = i.BookVersion.SellPrice  * i.BookCount,
                  profitTotal = (i.BookVersion.SellPrice - i.BookVersion.CostPrice) * i.BookCount,
                });

                return Ok(new { total= orders.Count, data });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetOrder/{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            try
            {
                var data = await _context.Orders.Include(i=>i.Invoice).Include(i => i.BookVersion)
                    .FirstOrDefaultAsync(i => i.Id == id) ?? throw new Exception($"Order With this id :{id} Not Found!.");

                var order = new OrderRModel
                {
                    Id = data.Id,
                    IsInvoiced = data.Invoice != null ? data.Invoice.IsInvoiced : false,
                    BranchId = data.BranchId,
                    BookVersionId = data.BookVersionId,
                    BookCount = data.BookCount,
                    Total = data.BookVersion.SellPrice  * data.BookCount,
                    profitTotal = (data.BookVersion.SellPrice - data.BookVersion.CostPrice) * data.BookCount,
                };
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("CreateOrder")]
        public async Task<IActionResult> CreateOrder(OrderCModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                _context.Add(OrderCModel.Fill(model));
                _context.SaveChanges();
                return Ok("Order Created Successfly!.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("UpdateOrder")]
        public async Task<IActionResult> UpdateOrder(OrderUModel model)
        {
            try
            {
                if (model.Id < 0 || model.Id == null)
                {
                    throw new Exception("Reauested Order Not Found!.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var order = await _context.Orders
                    .FirstOrDefaultAsync(i => i.Id == model.Id);

                if (order == null)
                {
                    throw new Exception("Requested Order Not Found!.");
                }

                order.BranchId = model.BranchId;
                order.BookVersionId = model.BookVersionId;
                order.BookCount = model.BookCount;
                _context.SaveChanges();
                return Ok("Order Updated Succefuly!.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteOrder")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                var order = await _context.Orders.FirstOrDefaultAsync(i => i.Id == id) ?? throw new Exception($"Order whith this ID:{id} Nof Found!.");
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                return Ok("Order Removed Successfuly!.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}

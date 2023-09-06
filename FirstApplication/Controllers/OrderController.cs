using BookShop.Db;
using BookShop.Entities;
using BookShop.Models.AuthorAddressModels;
using BookShop.Models.AuthorBiyografi;
using BookShop.Models.AuthorModels;
using BookShop.Models.OrderModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpGet]
        [Route("GetOrders")]
        public async Task<IActionResult> GetOrders()
        {
            try
            {
                var orders = await _context.Orders.Select(i => new OrderRModel
                {
                  Id = i.Id,
                  BranchId = i.BranchId,
                  BookVersionId = i.BookVersionId,
                  BookCount = i.BookCount,
                }).ToListAsync();

                return Ok(orders);
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
                var data = await _context.Orders
                    .FirstOrDefaultAsync(i => i.Id == id) ?? throw new Exception($"Order With this id :{id} Not Found!.");

                var order = new OrderRModel
                {
                    Id = data.Id,
                    BranchId = data.BranchId,
                    BookVersionId = data.BookVersionId,
                    BookCount = data.BookCount,
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
        public async Task<IActionResult> CreateOrder(OrderCUModel model)
        {
            try
            {
                _context.Add(OrderCUModel.Fill(model));
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
        public async Task<IActionResult> UpdateOrder(OrderCUModel model)
        {
            try
            {
                if (model.Id == 0 || model.Id == null)
                {
                    throw new Exception("Reauested Order Not Found!.");
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

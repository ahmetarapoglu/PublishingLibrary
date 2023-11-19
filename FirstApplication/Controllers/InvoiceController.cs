using BookShop.Db;
using BookShop.Entities;
using BookShop.Models.InvoiceModels;
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
    public class InvoiceController : ControllerBase
    {
        private readonly AppDbContext _context;
        public InvoiceController(AppDbContext context)
        {
            _context = context;
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetInvoices(InvoiceRequest model)
        {
            try
            {
                var invoices = await _context.Invoices
                    .Include(i => i.Order)
                    .ThenInclude(i => i.BookVersion)
                    .OrderByDescending(i => i.Id)
                    .ToListAsync();
                var data = invoices
                    .Skip(model.Skip)
                    .Take(model.Take)
                    .Select(i => new InvoiceRModel
                    {
                        Id = i.Id,
                        Order = new OrderRModel
                        {
                            Id = i.Id,
                            BranchId = i.Order.BranchId,
                            BookVersionId = i.Order.BookVersionId,
                            BookName = i.Order.BookVersion.Book.Title,
                            BookCount = i.Order.BookCount,
                            Total = i.Order.BookVersion.SellPrice * i.Order.BookCount,
                            profitTotal = (i.Order.BookVersion.SellPrice - i.Order.BookVersion.CostPrice) * i.Order.BookCount,
                        },
                    });

                return Ok(new { total= invoices.Count, data });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("[action]/{id}")]
        public async Task<IActionResult> GetInvoice(int id)
        {
            try
            {
                var data = await _context.Invoices.Include(i=>i.Order).ThenInclude(i=>i.BookVersion)
                    .FirstOrDefaultAsync(i => i.Id == id) ?? throw new Exception($"Invoice With this id :{id} Not Found!.");

                var invoice = new InvoiceRModel
                {
                    Id = data.Id,
                    Order = new OrderRModel
                    {
                        Id = data.Id,
                        BranchId = data.Order.BranchId,
                        BookVersionId = data.Order.BookVersionId,
                        BookName = data.Order.BookVersion.Book.Title,
                        BookCount = data.Order.BookCount,
                        Total = data.Order.BookVersion.SellPrice * data.Order.BookCount,
                        profitTotal = (data.Order.BookVersion.SellPrice - data.Order.BookVersion.CostPrice) * data.Order.BookCount,
                    },
                
                };
                return Ok(invoice);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateInvoice(InvoiceCModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var order = await _context.Orders.Include(i => i.Invoice).FirstOrDefaultAsync(i=>i.Id == model.OrderId);
                if(!order.IsInvoiced) {
                    order.BookVersionId = model.BookVersionId;
                    order.BookCount = model.BookCount;
                    order.IsInvoiced = true;
                }
                _context.Add(InvoiceCModel.Fill(model));
                _context.SaveChanges();
                return Ok(new {});
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> UpdateInvoice(InvoiceUModel model)
        {
            try
            {

                if (!ModelState.IsValid) return BadRequest(ModelState);
       
                if (model.Id < 0 || model.Id == null)  throw new Exception("Reauested Order Not Found!.");

                var invoice = await _context.Invoices
                    .FirstOrDefaultAsync(i => i.Id == model.Id) ?? throw new Exception("Requested Order Not Found!.");

                var order = await _context.Orders.Include(i => i.Invoice).FirstOrDefaultAsync(i => i.Id == model.OrderId);
                if (!order.IsInvoiced)
                {
                    order.BookVersionId = model.BookVersionId;
                    order.BookCount = model.BookCount;
                    order.IsInvoiced = true;
                }
                _context.SaveChanges();
                return Ok("Invoice Updated Succefuly!.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            try
            {
                var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.Id == id) ?? throw new Exception($"Invoice whith this ID:{id} Nof Found!.");
                _context.Invoices.Remove(invoice);
                await _context.SaveChangesAsync();
                return Ok("Invoice Removed Successfuly!.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}

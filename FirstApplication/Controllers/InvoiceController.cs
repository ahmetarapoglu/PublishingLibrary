using BookShop.Abstract;
using BookShop.Entities;
using BookShop.Models.InvoiceModels;
using BookShop.Models.OrderModels;
using BookShop.Models.RequestModels;
using BookShop.Services;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace BookShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InvoiceController : ControllerBase
    {
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IRepository<Order> _orderRepository;

        public InvoiceController(
            IRepository<Invoice> invoiceRepository,
            IRepository<Order> orderRepository)
        {
            _invoiceRepository = invoiceRepository;
            _orderRepository = orderRepository;
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetInvoices(InvoiceRequest model)
        {
            try
            {

                //Where.
                Expression<Func<Invoice, bool>> filter = i => true;

                //Date(Filter).
                if (model.StartDate != null)
                    filter = filter.And(i => i.CreateDate.Date >= model.StartDate.Value.Date);

                if (model.EndDate != null)
                    filter = filter.And(i => i.CreateDate.Date <= model.EndDate.Value.Date);

                //Sort.
                Expression<Func<Invoice, object>> Order = model.Order switch
                {
                    "id" => i => i.Id,
                    "createDate" => i => i.CreateDate,
                    _ => i => i.Id,
                };

                //OrderBy.
                IOrderedQueryable<Invoice> orderBy(IQueryable<Invoice> i)
                   => model.SortDir == "ascend"
                   ? i.OrderBy(Order)
                   : i.OrderByDescending(Order);

                //Select
                static IQueryable<InvoiceRModel> select(IQueryable<Invoice> query) => query.Select(entity => new InvoiceRModel
                {
                    Id = entity.Id,
                    Order = new OrderRModel
                    {
                        Id = entity.Id,
                        BranchId = entity.Order.BranchId,
                        BookVersionId = entity.Order.BookVersionId,
                        BookName = entity.Order.BookVersion.Book.Title,
                        BranchName = entity.Order.Branch.BranchName,
                        BookCount = entity.Order.BookCount,
                        Number = entity.Order.BookVersion.Number,
                        Total = entity.Order.BookVersion.SellPrice * entity.Order.BookCount,
                        profitTotal = (entity.Order.BookVersion.SellPrice - entity.Order.BookVersion.CostPrice) * entity.Order.BookCount,
                        IsInvoiced = entity.Order.IsInvoiced,
                    },
                });

                var (total, data) = await _invoiceRepository.GetListAndTotalAsync(select, filter, null, orderBy, skip: model.Skip, take: model.Take);

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
        public async Task<IActionResult> GetInvoice(int id)
        {
            try
            {
                //Where
                Expression<Func<Invoice, bool>> filter = i => i.Id == id;

                //Select
                static IQueryable<InvoiceRModel> select(IQueryable<Invoice> query) => query.Select(entity => new InvoiceRModel
                {
                    Id = entity.Id,
                    Order = new OrderRModel
                    {
                        Id = entity.Id,
                        BranchId = entity.Order.BranchId,
                        BookVersionId = entity.Order.BookVersionId,
                        BookName = entity.Order.BookVersion.Book.Title,
                        BranchName = entity.Order.Branch.BranchName,
                        BookCount = entity.Order.BookCount,
                        Number = entity.Order.BookVersion.Number,
                        Total = entity.Order.BookVersion.SellPrice * entity.Order.BookCount,
                        profitTotal = (entity.Order.BookVersion.SellPrice - entity.Order.BookVersion.CostPrice) * entity.Order.BookCount,
                        IsInvoiced = entity.Order.IsInvoiced,
                    },
                });

                var invoice = await _invoiceRepository.FindAsync(select, filter);

                return Ok(invoice);
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
        public async Task<IActionResult> CreateInvoice(InvoiceCModel model)
        {
            try
            {

                //Where
                Expression<Func<Order, bool>> filter = i => i.Id == model.OrderId;

                var order = await _orderRepository.FindAsync(filter);

                if (order!.IsInvoiced)
                    throw new OzelException(ErrorProvider.NotValid);

                order.BookVersionId = model.BookVersionId;
                order.BookCount = model.BookCount;
                order.IsInvoiced = true;

                var entity = new Invoice
                {
                    OrderId = model.OrderId,
                };

                await _orderRepository.UpdateAsync(order);
                await _invoiceRepository.AddAsync(entity);

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
        public async Task<IActionResult> UpdateInvoice(InvoiceUModel model)
        {
            try
            {

                if (model.Id < 0 || model.Id == null)
                    throw new Exception("Reauested Order Not Found!.");

                //Where
                Expression<Func<Invoice, bool>> filter = i => i.Id == model.Id;

                //Include.
                static IIncludableQueryable<Invoice, object> include(IQueryable<Invoice> query) => query.Include(i => i.Order);

                var entity = await _invoiceRepository.FindAsync(filter, include);

                if (entity!.Order.IsInvoiced)
                {
                    entity.Order.BookVersionId = model.BookVersionId;
                    entity.Order.BookCount = model.BookCount;
                    entity.Order.IsInvoiced = true;
                }

                await _invoiceRepository.UpdateAsync(entity);

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
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            try
            {
                //Where
                Expression<Func<Invoice, bool>> filter = i => i.Id == id;

                await _invoiceRepository.DeleteAsync(filter);
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

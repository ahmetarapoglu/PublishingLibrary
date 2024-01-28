using BookShop.Abstract;
using BookShop.Db;
using BookShop.Entities;
using BookShop.Models.AuthorAddressModels;
using BookShop.Models.AuthorBiyografi;
using BookShop.Models.AuthorModels;
using BookShop.Models.BookVersionModels;
using BookShop.Models.OrderModels;
using BookShop.Models.RequestModels;
using BookShop.Services;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IRepository<Order> _orderRepository;
        public OrderController(IRepository<Order> orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetOrders(OrderRequest model)
        {
            try
            {
                //Where.
                Expression<Func<Order, bool>> filter = i => true;

                //Date(Filter).
                if (model.StartDate != null)
                    filter = filter.And(i => i.CreateDate.Date >= model.StartDate.Value.Date);

                if (model.EndDate != null)
                    filter = filter.And(i => i.CreateDate.Date <= model.EndDate.Value.Date);

                //Search.
                //if (!string.IsNullOrEmpty(model.Search))
                //    filter = filter.And(i => i.BookCount.Contains(model.Search));

                //Sort.
                Expression<Func<Order, object>> Order = model.Order switch
                {
                    "id" => i => i.Id,
                    "bookCount" => i => i.BookCount,
                    "date" => i => i.CreateDate,
                    _ => i => i.Id,
                };

                //OrderBy.
                IOrderedQueryable<Order> orderBy(IQueryable<Order> i)
                   => model.SortDir == "ascend"
                   ? i.OrderBy(Order)
                   : i.OrderByDescending(Order);

                //Select.
                static IQueryable<OrderRModel> select(IQueryable<Order> query) => query
                    .Select(entity => new OrderRModel
                    {
                        Id = entity.Id,
                        IsInvoiced = entity.IsInvoiced,
                        BranchId = entity.BranchId,
                        BranchName = entity.Branch.BranchName,
                        BookVersionId = entity.BookVersionId,
                        Number = entity.BookVersion.Number,
                        BookName = entity.BookVersion.Book.Title,
                        BookCount = entity.BookCount,
                        CreateDate = entity.CreateDate,
                        Total = entity.BookVersion.SellPrice * entity.BookCount,
                        profitTotal = (entity.BookVersion.SellPrice - entity.BookVersion.CostPrice) * entity.BookCount,
                    });

                var (total, data) = await _orderRepository.GetListAndTotalAsync(select, filter, null, orderBy, skip: model.Skip, take: model.Take);

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
        public async Task<IActionResult> GetOrder(int id)
        {
            try
            {
                //Where
                Expression<Func<Order, bool>> filter = i => i.Id == id;

                //Select
                static IQueryable<OrderRModel> select(IQueryable<Order> query) => query.Select(entity => new OrderRModel
                {
                    Id = entity.Id,
                    IsInvoiced = entity.IsInvoiced,
                    BranchId = entity.BranchId,
                    BranchName = entity.Branch.BranchName,
                    BookVersionId = entity.BookVersionId,
                    Number = entity.BookVersion.Number,
                    BookName = entity.BookVersion.Book.Title,
                    BookCount = entity.BookCount,
                    CreateDate = entity.CreateDate,
                    Total = entity.BookVersion.SellPrice * entity.BookCount,
                    profitTotal = (entity.BookVersion.SellPrice - entity.BookVersion.CostPrice) * entity.BookCount,

                });

                var order = await _orderRepository.FindAsync(select, filter);

                return Ok(order);
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
        public async Task<IActionResult> CreateOrder(OrderCModel model)
        {
            try
            {

                var entity = new Order
                {
                    BranchId = model.BranchId,
                    BookVersionId = model.BookVersionId,
                    BookCount = model.BookCount,
                    IsInvoiced = false,
                    CreateDate = DateTime.Now,
                };

                await _orderRepository.AddAsync(entity);

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
        public async Task<IActionResult> UpdateOrder(OrderUModel model)
        {
            try
            {
                if (model.Id < 0 || model?.Id == null)
                    throw new Exception("Reauested Order Not Found!.");

                //Where
                Expression<Func<Order, bool>> filter = i => i.Id == model.Id;

                var entity = await _orderRepository.FindAsync(filter);

                entity!.BranchId = model.BranchId;
                entity.BookVersionId = model.BookVersionId;
                entity.BookCount = model.BookCount;
                entity.IsInvoiced = false;

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
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                //Where
                Expression<Func<Order, bool>> filter = i => i.Id == id;

                await _orderRepository.DeleteAsync(filter);

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

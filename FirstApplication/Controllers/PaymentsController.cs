using BookShop.Abstract;
using BookShop.Db;
using BookShop.Entities;
using BookShop.Models.AuthorPaymentModels;
using BookShop.Models.BranchPaymentModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IRepository<AuthorPayment> _paymentRepository;
        private readonly IRepository<BranchPayment> _BranchPayment;

        public PaymentsController(
            AppDbContext context,
            IRepository<AuthorPayment> paymentRepository,
            IRepository<BranchPayment> BranchPayment)
        {
            _context = context;
            _paymentRepository = paymentRepository;
            _BranchPayment = BranchPayment;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> PayToAuthor(AuthorPaymentCModel model)
        {
            try
            {
                var number = _context.AuthorPayments.OrderByDescending(i => i.Id).Select(i => i.PaymentNumber).FirstOrDefault();

                var entity = new AuthorPayment
                {
                    AuthorId = model.AuthorId,
                    Amount = model.Amount,
                    PaidDate = model.PaidDate,
                    PaymentNumber = number + 1,
                    CreateDate = DateTime.Now,
                };

                await _paymentRepository.AddAsync(entity);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CollectionFromBranch(BranchPaymentCModel model)
        {
            try
            {
                var number = _context.BranchPayments.OrderByDescending(i => i.Id).Select(i => i.PaymentNumber).FirstOrDefault();

                var entity = new BranchPayment
                {
                    BranchId = model.BranchId,
                    Amount = model.Amount,
                    PaidDate = model.PaidDate,
                    PaymentNumber = number + 1,
                    CreateDate = DateTime.Now,
                };

                await _BranchPayment.AddAsync(entity);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpGet]
        //[Route("[action]")]
        //public async Task<IActionResult> AccountActivities()
        //{
        //    try
        //    {
        //        var totalBooksSell = _context.Orders.Sum(i => i.BookVersion.SellPrice * i.BookCount);
        //        var totalBooksCost = _context.Orders.Sum(i => i.BookVersion.CostPrice * i.BookCount);

        //        var profitTotal = totalBooksSell - totalBooksCost;

        //        var LibProfitTotal = _context.Orders.Sum(i => (((i.BookVersion.SellPrice - i.BookVersion.CostPrice) * i.BookCount) * i.BookVersion.LibraryRatio) / 100);
        //        var AuthorProfitTotal = profitTotal - LibProfitTotal;

        //        var totalAuthorPayments = _context.AuthorPayments.Sum(i => i.Amount);
        //        var remainingAuthorsPayments = AuthorProfitTotal - totalAuthorPayments;

        //        var totalBranchPayments = _context.BranchPayments.Sum(i => i.Amount);
        //        var remainingBranchesPayments = totalBooksSell - totalBranchPayments;

        //        return Ok(new
        //        {
        //            totalBooksCost,
        //            totalBooksSell,
        //            profitTotal,
        //            LibProfitTotal,
        //            AuthorProfitTotal,
        //            totalAuthorPayments,
        //            remainingAuthorsPayments,
        //            totalBranchPayments,
        //            remainingBranchesPayments
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Till()
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}

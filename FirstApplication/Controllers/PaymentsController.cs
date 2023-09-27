using BookShop.Db;
using BookShop.Models.AuthorModels;
using BookShop.Models.AuthorPaymentModels;
using BookShop.Models.BranchPaymentModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BookShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public PaymentsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("PayToAuthor")]
        public async Task<IActionResult> PayToAuthor(AuthorPaymentCModel model)
        {
            try
            {
                var number = _context.AuthorPayments.OrderByDescending(i => i.Id).Select(i => i.PaymentNumber).FirstOrDefault();

                _context.Add(AuthorPaymentCModel.Fill(model, number));
                _context.SaveChanges();
                return Ok("Successfly!.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("CollectionFromBranch")]
        public async Task<IActionResult> CollectionFromBranch(BranchPaymentCModel model)
        {
            try
            {
                var number = _context.BranchPayments.OrderByDescending(i => i.Id).Select(i => i.PaymentNumber).FirstOrDefault();

                _context.Add(BranchPaymentCModel.Fill(model,number));
                _context.SaveChanges();
                return Ok("Successfly!.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("AccountActivities")]
        public async Task<IActionResult> AccountActivities()
        {
            try
            {
                var totalBooksSell = _context.Orders.Sum(i=>i.BookVersion.SellPrice * i.BookCount);
                var totalBooksCost = _context.Orders.Sum(i=>i.BookVersion.CostPrice * i.BookCount);

                var profitTotal = totalBooksSell - totalBooksCost;

                var LibProfitTotal = _context.Orders.Sum(i =>  (((i.BookVersion.SellPrice - i.BookVersion.CostPrice) * i.BookCount) * i.BookVersion.LibraryRatio) / 100);
                var AuthorProfitTotal = profitTotal - LibProfitTotal;

                var totalAuthorPayments = _context.AuthorPayments.Sum(i => i.Amount);
                var remainingAuthorsPayments = AuthorProfitTotal - totalAuthorPayments;

                var totalBranchPayments = _context.BranchPayments.Sum(i => i.Amount);
                var remainingBranchesPayments = totalBooksSell - totalBranchPayments;

                return Ok(new
                {
                    totalBooksCost,
                    totalBooksSell,
                    profitTotal,
                    LibProfitTotal,
                    AuthorProfitTotal,
                    totalAuthorPayments,
                    remainingAuthorsPayments,
                    totalBranchPayments,
                    remainingBranchesPayments
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("Till")]
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

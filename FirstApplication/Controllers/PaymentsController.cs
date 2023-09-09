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
        [Route("PaymentsFromBranch")]
        public async Task<IActionResult> PaymentsFromBranch(BranchPaymentCModel model)
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

    }
}

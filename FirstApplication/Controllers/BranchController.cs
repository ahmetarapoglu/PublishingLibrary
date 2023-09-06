using BookShop.Db;
using BookShop.Entities;
using BookShop.Models.AuthorAddressModels;
using BookShop.Models.AuthorBiyografi;
using BookShop.Models.AuthorModels;
using BookShop.Models.BranchModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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


        [HttpGet]
        [Route("GetBranches")]
        public async Task<IActionResult> GetBranches()
        {
            try
            {
                var Branches = await _context.Branches
                    .Select(i => new BranchRModel
                    {
                        Id = i.Id,
                        BranchAddress = i.BranchAddress,
                        BranchName = i.BranchName,
                        PhoneNumber = i.PhoneNumber,

                    }).ToListAsync();

                return Ok(Branches);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetBranch/{id}")]
        public async Task<IActionResult> GetBranch(int id)
        {
            try
            {
                var data = await _context.Branches.FirstOrDefaultAsync(i => i.Id == id) ?? throw new Exception($"Branch With this id :{id} Not Found!.");
                var branch = new BranchRModel
                {
                    Id = data.Id,
                    BranchAddress= data.BranchAddress,
                    BranchName = data.BranchName,
                    PhoneNumber = data.PhoneNumber,
                };
                return Ok(branch);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("CreateBranch")]
        public async Task<IActionResult> CreateBranch(BranchCUModel model)
        {
            try
            {
                _context.Add(BranchCUModel.Fill(model));
                _context.SaveChanges();
                return Ok("Branch Created Successfly!.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("UpdateBranch")]
        public async Task<IActionResult> UpdateBranch(BranchCUModel model)
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

                 branch.BranchName = model.BranchName;
                 branch.BranchAddress = model.BranchAddress;
                 branch.PhoneNumber = model.PhoneNumber;

                _context.SaveChanges();
                return Ok("Branch Updated Succefuly!.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteBranch")]
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

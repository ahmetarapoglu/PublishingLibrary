using BookShop.Db;
using BookShop.Entities;
using BookShop.Models.CategoryModels;
using BookShop.Models.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("GetCategories")]
        public async Task<IActionResult> GetCategories(CategoryRequest model)
        {
            try
            {
                var categories =await _context.Categories
                    .Where(i => i.CategoryName.Contains(model.Search))
                    .Skip(model.Skip)
                    .Take(model.Take)
                    .Select(i=>new CategoryRModel
                {
                    Id = i.Id,  
                    CategoryName = i.CategoryName,
                }).ToListAsync();

                return Ok(new { categories.Count , categories });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetCategory/{id}")]
        public async Task<IActionResult> GetCategory(int id) {
            try
            {
                var data = await _context.Categories.FirstOrDefaultAsync(i => i.Id == id) ?? throw new Exception($"Category With this id :{id} Not Found!.");
                var category = new CategoryRModel
                {
                    Id = data.Id,
                    CategoryName = data.CategoryName,
                };
                return Ok(category);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("CreateCategory")]
        public async Task<IActionResult> CreateCategory(CategoryCModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                _context.Add(CategoryCModel.Fill(model));
                _context.SaveChanges();
                return Ok("Category Created Successfly!.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("UpdateCategory")]
        public async Task<IActionResult> UpdateCategory(CategoryUModel model)
        {
            try
            {
                if(model.Id ==0 || model.Id == null)
                {
                    throw new Exception("Reauested Category Not Found!.");
                }
                var category = await _context.Categories.FirstOrDefaultAsync(i=>i.Id == model.Id);
                if(category == null)
                {
                     throw new Exception("Requested Category Not Found!.");
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                category.CategoryName = model.CategoryName;
                _context.SaveChanges();
                return Ok("Category Updated Succefuly!.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteCategory")]
        public async Task<IActionResult> DeleteCategory (int id)
        {
            try
            {
                var category = await _context.Categories.FirstOrDefaultAsync(i => i.Id == id) ?? throw new Exception($"Category whith this ID:{id} Nof Found!.");
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return Ok("Category Removed Successfuly!.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}

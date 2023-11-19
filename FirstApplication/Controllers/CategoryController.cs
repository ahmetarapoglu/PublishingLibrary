using BookShop.Abstract;
using BookShop.Entities;
using BookShop.Models.CategoryModels;
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
    [Authorize]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IRepository<Category> _categoryrepository;
        private readonly IValidation<CategoryRequest> _validate;


        public CategoryController(
            IRepository<Category> categoryrepository,
            IValidation<CategoryRequest> validate)
        {
            _categoryrepository = categoryrepository;
            _validate = validate;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetCategories(CategoryRequest model)
        {
            try
            {
                //Validation
                _validate.Validator(model);

                //Where.
                Expression<Func<Category, bool>> filter = Category => true;

                //Date(Filter).
                if (model.StartDate != null)
                    filter = filter.And(i => i.CreateDate.Date >= model.StartDate.Value.Date);

                if (model.EndDate != null)
                    filter = filter.And(i => i.CreateDate.Date <= model.EndDate.Value.Date);

                //Search.
                if (!string.IsNullOrEmpty(model.Search))
                    filter = filter.And(i => i.CategoryName.Contains(model.Search));

                //Include.
                static IIncludableQueryable<Category, object> include(IQueryable<Category> query) => query.Include(i => i.Books);

                //Sort.
                Expression<Func<Category, object>> Order = model.Order switch
                {
                    "id" => i => i.Id,
                    "categoryName" => i => i.CategoryName,
                    _ => i => i.Id,
                };

                //OrderBy.
                IOrderedQueryable<Category> orderBy(IQueryable<Category> i)
                   => model.SortDir == "ascend"
                   ? i.OrderBy(Order)
                   : i.OrderByDescending(Order);

                //Select.
                static IQueryable<CategoryRModel> select(IQueryable<Category> query) => query.Select(entity => new CategoryRModel
                {
                    Id = entity.Id,
                    CategoryName = entity.CategoryName,
                    CreateDate = entity.CreateDate,
                });

                var (total, data) = await _categoryrepository.GetListAndTotalAsync(select, filter, include, orderBy, skip: model.Skip, take: model.Take);

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
        public async Task<IActionResult> GetCategory(int id)
        {
            try
            {
                //Where
                Expression<Func<Category, bool>> filter = i => i.Id == id;

                //Select
                static IQueryable<CategoryRModel> select(IQueryable<Category> query) => query.Select(entity => new CategoryRModel
                {
                    Id = entity.Id,
                    CategoryName = entity.CategoryName,
                    CreateDate = entity.CreateDate
                });

                var category = await _categoryrepository.FindAsync(select, filter);

                return Ok(category);
            }
            catch (OzelException ex)
            {
                return BadRequest(ex.Errors);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateCategory(CategoryCModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var entity = new Category
                {
                    CategoryName = model.CategoryName,
                };

                await _categoryrepository.AddAsync(entity);

                return Ok(new { status = true });
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

        [HttpPut]
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UpdateCategory(CategoryUModel model)
        {
            try
            {
                if (model.Id == 0 || model.Id == null)
                {
                    throw new Exception("Reauested Category Not Found!.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                //Where
                Expression<Func<Category, bool>> filter = i => i.Id == model.Id;

                var entity = await _categoryrepository.FindAsync(filter);

                entity!.CategoryName = model.CategoryName;

                await _categoryrepository.UpdateAsync(entity);

                return Ok(new { status = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                //Where
                Expression<Func<Category, bool>> filter = i => i.Id == id;

                await _categoryrepository.DeleteAsync(filter);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}

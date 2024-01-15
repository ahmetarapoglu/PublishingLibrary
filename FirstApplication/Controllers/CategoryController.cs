using BookShop.Abstract;
using BookShop.Entities;
using BookShop.Models.CategoryModels;
using BookShop.Models.RequestModels;
using BookShop.Services;
using BookShop.Validations.ReqValidation;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace BookShop.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IRepository<Category> _categoryrepository;

        public CategoryController(
            IRepository<Category> categoryrepository)
        {
            _categoryrepository = categoryrepository;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetCategories(DataTableRequest model)
        {
            try
            {
                //Validation
                model.Validate(new DataTableReqValidator());

                //Where.
                Expression<Func<Category, bool>> filter = i => true;

                //Date(Filter).
                if (model.StartDate != null)
                    filter = filter.And(i => i.CreateDate.Date >= model.StartDate.Value.Date);

                if (model.EndDate != null)
                    filter = filter.And(i => i.CreateDate.Date <= model.EndDate.Value.Date);

                //Search.
                if (!string.IsNullOrEmpty(model.Search))
                    filter = filter.And(i => i.CategoryName.Contains(model.Search));

                //Include.
                //static IIncludableQueryable<Category, object> include(IQueryable<Category> query) => query.Include(i => i.Books);

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

                var (total, data) = await _categoryrepository.GetListAndTotalAsync(select, filter, null, orderBy, skip: model.Skip, take: model.Take);

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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateCategory(CategoryCModel model)
        {
            try
            {
                var entity = new Category
                {
                    CategoryName = model.CategoryName,
                    CreateDate = DateTime.Now,
                };

                await _categoryrepository.AddAsync(entity);

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
        public async Task<IActionResult> UpdateCategory(CategoryUModel model)
        {
            try
            {
                if (model.Id == 0 || model?.Id == null)
                    throw new Exception("Reauested Category Not Found!.");

                //Where
                Expression<Func<Category, bool>> filter = i => i.Id == model.Id;

                void action(Category user)
                {
                    user!.CategoryName = model.CategoryName;
                }

                await _categoryrepository.UpdateAsync(action, filter);

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
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                //Where
                Expression<Func<Category, bool>> filter = i => i.Id == id;

                await _categoryrepository.DeleteAsync(filter);

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

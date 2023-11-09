using BookShop.Abstract;
using BookShop.Entities;
using BookShop.Models.CategoryModels;
using BookShop.Models.RequestModels;
using BookShop.Services;
using BookShop.Validations.ReqValidation;
using FluentValidation;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Error = BookShop.Services.Error;

namespace BookShop.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IRepository<Category> _categoryrepository;
        private readonly IValidator<CategoryRequest> _requestValidator;


        public CategoryController(
            IRepository<Category> categoryrepository,
            IValidator<CategoryRequest> requestValidator)
        {
            _categoryrepository = categoryrepository;
            _requestValidator = requestValidator;
        }

        [HttpPost]
        [Route("GetCategories")]
        public async Task<IActionResult> GetCategories(CategoryRequest model)
        {
            try
            {
                Validator<CategoryRequest> t = new();
                t.Validations(model);

                //var validation = _requestValidator.Validate(model);
                //if (!validation.IsValid)
                //{
                //    var errors = validation.Errors.Select(error => new Error
                //    {
                //        Key = Enum.EnumErrorTypes.Danger,
                //        Code = "400",
                //        Title = error.PropertyName,
                //        Description = error.ErrorMessage,
                //    }).ToList();

                //    throw new OzelException(errors);
                //}

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

                //include.
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
        [Route("GetCategory/{id}")]
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
        [Route("CreateCategory")]
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
        [Route("UpdateCategory")]
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
        [Route("DeleteCategory")]
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

﻿using BookShop.Abstract;
using BookShop.Db;
using BookShop.Entities;
using BookShop.Models.AuthorAddressModels;
using BookShop.Models.AuthorBiyografi;
using BookShop.Models.AuthorModels;
using BookShop.Models.BookVersionModels;
using BookShop.Models.CategoryModels;
using BookShop.Models.RequestModels;
using BookShop.Services;
using BookShop.Validations.ReqValidation;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Linq.Expressions;

namespace BookShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthorController : ControllerBase
    {
        private readonly IRepository<Author> _authorRepository;
        public AuthorController( IRepository<Author> authorRepository)
        {
            _authorRepository = authorRepository;
        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetAuthors(AuthorRequest model)
        {

            try
            {
                //Where.
                Expression<Func<Author, bool>> filter = i => true;

                //Date(Filter).
                if (model.StartDate != null)
                    filter = filter.And(i => i.CreateDate.Date >= model.StartDate.Value.Date);

                if (model.EndDate != null)
                    filter = filter.And(i => i.CreateDate.Date <= model.EndDate.Value.Date);

                //Search.
                if (!string.IsNullOrEmpty(model.Search))
                    filter = filter.And(i => i.NameSurname.Contains(model.Search));

                //Sort.
                Expression<Func<Author, object>> Order = model.Order switch
                {
                    "id" => i => i.Id,
                    "nameSurname" => i => i.NameSurname,
                    "date" => i => i.CreateDate,
                    _ => i => i.Id,
                };

                //OrderBy.
                IOrderedQueryable<Author> orderBy(IQueryable<Author> i)
                   => model.SortDir == "ascend"
                   ? i.OrderBy(Order)
                   : i.OrderByDescending(Order);

                //Select.
                static IQueryable<AuthorRModel> select(IQueryable<Author> query) => query
                    .Select(entity => new AuthorRModel
                    {
                        Id = entity.Id,
                        NameSurname = entity.NameSurname,
                        Image = entity.Image,
                        CreateDate = entity.CreateDate,
                        TotalAmount = entity.BookAuthors.Select(i => (i.AuhorRatio * i.Book.BookVersions.Sum(i => i.ProfitTotal)) / (100)).FirstOrDefault(),                       
                        TotalPayment = entity.AuthorPayments.Sum(i => i.Amount),
                        RemainingPayment = entity.BookAuthors.Select(i => (i.AuhorRatio * i.Book.BookVersions.Sum(i => i.ProfitTotal)) / (100)).FirstOrDefault() - entity.AuthorPayments.Sum(i => i.Amount),
                        AuthorAddress = new AuthorAddressModel
                        {
                            Country = entity.AuthorAddress.Country,
                            City = entity.AuthorAddress.City,
                            PostCode = entity.AuthorAddress.PostCode,
                        },
                        AuthorBiography = new AuthorBiographyModel
                        {
                            Email = entity.AuthorBiography.Email,
                            PhoneNumber = entity.AuthorBiography.PhoneNumber,
                            NativeLanguage = entity.AuthorBiography.NativeLanguage,
                            Education = entity.AuthorBiography.Education
                        },
                        Books = entity.BookAuthors.Select(i => new BookInAuthors
                        {
                            Id = i.Book.Id,
                            Title = i.Book.Title,
                            Description = i.Book.Description,
                            PublishedDate = i.Book.PublishedDate,
                            LibraryRatio = i.Book.LibraryRatio,
                            CategoryName = i.Book.BookCategories.Select(i=>i.Category.CategoryName).ToList(),
                            BookVersions = i.Book.BookVersions.Select(i =>
                            new BookVersionRModel
                            {
                                Id = i.Id,
                                Number = i.Number,
                                BookCount = i.BookCount,
                                CostPrice = i.CostPrice,
                                TotalCostPrice = i.CostPrice * i.BookCount,
                                SellPrice = i.SellPrice,
                                TotalSellPrice = i.SellPrice * i.BookCount,
                                ProfitTotal = i.ProfitTotal,
                            }).ToList()
                        }).ToList(),
                    });

                var (total, data) = await _authorRepository.GetListAndTotalAsync(select, filter,null, orderBy, skip: model.Skip, take: model.Take);

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
        [Route("GetAuthor/{id}")]
        public async Task<IActionResult> GetAuthor(int id)
        {
            try
            {
                //Where
                Expression<Func<Author, bool>> filter = i => i.Id == id;
                decimal c = 0;

                //Select
                static IQueryable<AuthorRModel> select(IQueryable<Author> query) => query.Select(entity => new AuthorRModel
                {
                    Id = entity.Id,
                    NameSurname = entity.NameSurname,
                    Image = entity.Image,
                    CreateDate = entity.CreateDate,
                    TotalAmount = entity.BookAuthors.Select(i=> (i.AuhorRatio * i.Book.BookVersions.Sum(i => i.ProfitTotal)) /(100)).FirstOrDefault(),
                    TotalPayment = entity.AuthorPayments.Sum(i => i.Amount),
                    RemainingPayment = entity.BookAuthors.Select(i => (i.AuhorRatio * i.Book.BookVersions.Sum(i => i.ProfitTotal)) / (100)).FirstOrDefault() - entity.AuthorPayments.Sum(i => i.Amount),
                    AuthorAddress = new AuthorAddressModel
                    {
                        Country = entity.AuthorAddress.Country,
                        City = entity.AuthorAddress.City,
                        PostCode = entity.AuthorAddress.PostCode,
                    },
                    AuthorBiography = new AuthorBiographyModel
                    {
                        Email = entity.AuthorBiography.Email,
                        PhoneNumber = entity.AuthorBiography.PhoneNumber,
                        NativeLanguage = entity.AuthorBiography.NativeLanguage,
                        Education = entity.AuthorBiography.Education
                    },
                    Books = entity.BookAuthors.Select(i => new BookInAuthors
                    {
                        Id = i.Book.Id,
                        Title = i.Book.Title,
                        Description = i.Book.Description,
                        PublishedDate = i.Book.PublishedDate,
                        LibraryRatio = i.Book.LibraryRatio,
                        CategoryName = i.Book.BookCategories.Select(i => i.Category.CategoryName).ToList(),
                        BookVersions = i.Book.BookVersions.Select(i =>
                        new BookVersionRModel
                        {
                            Id = i.Id,
                            Number = i.Number,
                            BookCount = i.BookCount,
                            CostPrice = i.CostPrice,
                            TotalCostPrice = i.CostPrice * i.BookCount,
                            SellPrice = i.SellPrice,
                            TotalSellPrice = i.SellPrice * i.BookCount,
                            ProfitTotal = i.ProfitTotal,
                        }).ToList()
                    }).ToList()

                });

                var author = await _authorRepository.FindAsync(select, filter);

                return Ok(author);
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
        public async Task<IActionResult> CreateAuthor(AuthorCModel model)
        {
            try
            {
                var entity = new Author
                {
                    NameSurname = model.NameSurname,
                    Image = model.Image,
                    CreateDate = DateTime.Now,
                    AuthorAddress = new AuthorAddress
                    {
                        Country = model.AuthorAddress.Country,
                        City = model.AuthorAddress.City,
                        PostCode = model.AuthorAddress.PostCode
                    },
                    AuthorBiography = new AuthorBiography
                    {
                        Email = model.AuthorBiography.Email,
                        PhoneNumber = model.AuthorBiography.PhoneNumber,
                        NativeLanguage = model.AuthorBiography.NativeLanguage,
                        Education = model.AuthorBiography.Education
                    }
                };

                await _authorRepository.AddAsync(entity);

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
        public async Task<IActionResult> UpdateAuthor(AuthorUModel model)
        {
            try
            {
                if (model.Id == 0 || model?.Id == null)
                    throw new Exception("Reauested Author Not Found!.");
               

                //Where
                Expression<Func<Author, bool>> filter = i => i.Id == model.Id;

                void action(Author user)
                {
                    user!.NameSurname = model.NameSurname;
                    user.Image = model.Image;

                    user.AuthorAddress.Country = model.AuthorAddress.Country;
                    user.AuthorAddress.City = model.AuthorAddress.City;
                    user.AuthorAddress.PostCode = model.AuthorAddress.PostCode;

                    user.AuthorBiography.Email = model.AuthorBiography.Email;
                    user.AuthorBiography.PhoneNumber = model.AuthorBiography.PhoneNumber;
                    user.AuthorBiography.NativeLanguage = model.AuthorBiography.NativeLanguage;
                    user.AuthorBiography.Education = model.AuthorBiography.Education;
                }

                //Include.
                static IIncludableQueryable<Author, object> include(IQueryable<Author> query) => query
                    .Include(i => i.AuthorAddress)
                    .Include(i => i.AuthorBiography);
 

                await _authorRepository.UpdateAsync(action, filter, include);

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
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            try
            {
                //Where
                Expression<Func<Author, bool>> filter = i => i.Id == id;

                await _authorRepository.DeleteAsync(filter);
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

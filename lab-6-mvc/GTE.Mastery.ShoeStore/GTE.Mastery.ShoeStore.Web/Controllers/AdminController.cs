using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using GTE.Mastery.ShoeStore.Business.Dtos;
using GTE.Mastery.ShoeStore.Business.Interfaces;
using GTE.Mastery.ShoeStore.Domain.Entities;
using GTE.Mastery.ShoeStore.Domain.Enums;
using GTE.Mastery.ShoeStore.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GTE.Mastery.ShoeStore.Domain;
using Microsoft.AspNetCore.Authorization;
using GTE.Mastery.ShoeStore.Data.Interfaces;
using System.Drawing;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.OutputCaching;

namespace GTE.Mastery.ShoeStore.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IValidator<UpdateShoeDto> _validator;
        private readonly IDataHelper _dataHelper;
        private readonly IConfiguration _configuration;
        private readonly IShoeService _shoeService;

        public AdminController(IValidator<UpdateShoeDto> validator, IDataHelper dataHelper,
            IConfiguration configuration, IShoeService shoeService)
        {
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _dataHelper = dataHelper ?? throw new ArgumentNullException(nameof(dataHelper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _shoeService = shoeService ?? throw new ArgumentNullException(nameof(shoeService));
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            int maxRowCountPerPage = int.Parse(_configuration["Paging:MaxRowCountPerShoePage"]);

            var totalPageCount = (await _shoeService.ListShoesAsync()).Count();
            var shoes = await _shoeService.ListShoesAsync((page - 1) * maxRowCountPerPage, maxRowCountPerPage);

            ShoeViewModel model = new ShoeViewModel(shoes, totalPageCount, page, maxRowCountPerPage);

            return View(model);
        }

        [HttpGet]
        public IActionResult CreateShoe(int totalRowCount, int maxRowCountPerPage, int totalPageCount)
        {
            ViewBag.TotalRowCount = totalRowCount; //total row count
            ViewBag.MaxRowCountPerPage = maxRowCountPerPage; // max row count per page
            ViewBag.TotalPageCount = totalPageCount; // total page count 

            UpdateShoeDto shoeDto = new UpdateShoeDto();
            InitializeDto(shoeDto);

            return View(shoeDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateShoe(int totalRowCount, int maxRowCountPerPage, int totalPageCount, UpdateShoeDto shoeDto)
        {
            ValidationResult validationResult = _validator.Validate(shoeDto);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                InitializeDto(shoeDto);

                return RedirectToAction("CreateShoe", new { totalRowCount, maxRowCountPerPage, totalPageCount });
            }

            if (string.IsNullOrEmpty(shoeDto.ImagePath))
            {
                shoeDto.ImagePath = _configuration.GetSection("DefaultImagePath").Value;
            }

            var shoe = await _shoeService.CreateShoeAsync(shoeDto);

            if (shoe == null)
            {
                return BadRequest("The shoe has not been created successfully");
            }

            if (totalRowCount % maxRowCountPerPage == 0)
            {
                return RedirectToAction("Index", new { page = totalPageCount + 1 });
            }

            return RedirectToAction("Index", new { page = totalPageCount });
        }

        [HttpGet]
        public async Task<IActionResult> EditShoe(int id, int page)
        {
            ViewBag.CurrentPage = page;

            var shoe = await _shoeService.GetShoeAsync(id);

            if (shoe != null)
            {
                UpdateShoeDto shoeDto = new UpdateShoeDto()
                {
                    Id = id,
                    Name = shoe.Name,
                    Vendor = shoe.Vendor,
                    ImagePath = shoe.ImagePath,
                    Price = shoe.Price,
                    Gender = shoe.Gender,
                    BrandId = shoe.BrandId,
                    CategoryId = shoe.CategoryId,
                    SizeId = shoe.SizeId,
                    ColorId = shoe.ColorId
                };

                InitializeDto(shoeDto);

                return View(shoeDto);
            }

            return BadRequest(shoe);
        }

        [HttpPost]
        public async Task<IActionResult> EditShoe(int id, int page, UpdateShoeDto shoeDto)
        {
            ValidationResult validationResult = _validator.Validate(shoeDto);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                InitializeDto(shoeDto);

                return RedirectToAction("EditShoe", new { id = shoeDto.Id, page});
            }

            var shoe = await _shoeService.UpdateShoeAsync(id, shoeDto);

            if (shoe != null)
            {
                return RedirectToAction("Index", new { page });
            }

            return BadRequest($"The shoe with Id={id} has not been updated successfully");
        }
                
        [HttpPost]
        public async Task<IActionResult> DeleteShoe(ShoeDto shoeDto, int page, int totalPageCount, int totalRowCount, int maxRowCountPerPage)
        {
            await _shoeService.DeleteShoeAsync(shoeDto.Id);

            if (page == totalPageCount && totalRowCount % maxRowCountPerPage == 1)
            {
                return RedirectToAction("Index", new { page = totalPageCount - 1 });
            }

            return RedirectToAction("Index", new { page });
        }

        private void InitializeDto(UpdateShoeDto shoeDto)
        {
            shoeDto.Genders = Enum.GetValues<Gender>().ToList();
            ShoeViewData shoeViewData = _dataHelper.GetViewData();

            shoeDto.Brands = shoeViewData.Brands;
            shoeDto.Categories = shoeViewData.Categories;
            shoeDto.Sizes = shoeViewData.Sizes;
            shoeDto.Colors = shoeViewData.Colors;
        }
    }
}

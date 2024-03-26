using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using GTE.Mastery.ShoeStore.Business.Dtos;
using GTE.Mastery.ShoeStore.Business.Interfaces;
using GTE.Mastery.ShoeStore.Data.Interfaces;
using GTE.Mastery.ShoeStore.Domain;
using GTE.Mastery.ShoeStore.Domain.Enums;
using GTE.Mastery.ShoeStore.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace GTE.Mastery.ShoeStore.Web.Controllers
{
    [Authorize(Roles = RoleTypes.Admin)]
    public class AdminController : Controller
    {
        private readonly IValidator<CreateEditShoeDto> _validator;
        private readonly IShoeService _shoeService;

        public AdminController(IValidator<CreateEditShoeDto> validator,
            IShoeService shoeService, IMapper mapper)
        {
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _shoeService = shoeService ?? throw new ArgumentNullException(nameof(shoeService));
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 6;
            var count = (await _shoeService.ListShoesAsync()).Count();           
            var shoes = await _shoeService.ListShoesAsync((page - 1) * pageSize, pageSize);

            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            ShoeViewModel shoeViewModel = new ShoeViewModel(shoes, pageViewModel);

            return View(shoeViewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            CreateEditShoeDto shoeDto = new CreateEditShoeDto();
            _shoeService.InitializeDto(shoeDto);
            return View(shoeDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateShoe([FromForm] CreateEditShoeDto shoeDto)
        {
            ValidationResult validationResult = _validator.Validate(shoeDto);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                _shoeService.InitializeDto(shoeDto);
                return View(shoeDto);
            }

            shoeDto.ImagePath = "shoe.png";

            var shoe = await _shoeService.CreateShoeAsync(shoeDto);

            if (shoe != null)
            {
                return RedirectToAction("Index");
            }

            return View(shoeDto);
        }

        [HttpGet]
        public async Task<IActionResult> EditShoe([FromRoute] int id)
        {
            var shoe = await _shoeService.GetShoeAsync(id);

            if (shoe != null)
            {
                CreateEditShoeDto shoeDto = new CreateEditShoeDto()
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

                _shoeService.InitializeDto(shoeDto);

                return View(shoeDto);
            }

            return View("Edit");
        }

        [HttpPost]
        public async Task<IActionResult> EditShoe([FromRoute] int id,  [FromForm] CreateEditShoeDto shoeDto)
        {
            ValidationResult validationResult = _validator.Validate(shoeDto);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                _shoeService.InitializeDto(shoeDto);
                return View(shoeDto);
            }

            var shoe = _shoeService.UpdateShoeAsync(id,shoeDto);

            if (shoe != null)
            {
                return RedirectToAction("Index");
            }

            return View("Edit");
        }

        public async Task<IActionResult> DeleteShoe([FromRoute] int id)
        {
            await _shoeService.DeleteShoeAsync(id);
            return RedirectToAction("Index");
        }
    }
}

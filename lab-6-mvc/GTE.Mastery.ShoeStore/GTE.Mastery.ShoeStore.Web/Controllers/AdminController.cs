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
using GTE.Mastery.ShoeStore.Web.Filters;
using GTE.Mastery.ShoeStore.Domain;
using Microsoft.AspNetCore.Authorization;

namespace GTE.Mastery.ShoeStore.Web.Controllers
{
    [ShoeStoreFilterAuthorize("Admin")]
    public class AdminController : Controller
    {
        private readonly IValidator<UpdateShoeDto> _validator;
        private readonly IConfiguration _configuration;
        private readonly IShoeService _shoeService;

        private readonly IRepositoryFactory<Size> _sizeFactory;
        private readonly IRepositoryFactory<Brand> _brandFactory;
        private readonly IRepositoryFactory<Category> _categoryFactory;
        private readonly IRepositoryFactory<Color> _colorFactory;

        public AdminController(IValidator<UpdateShoeDto> validator, IConfiguration configuration,
            IShoeService shoeService, IRepositoryFactory<Brand> brandFactory,
            IRepositoryFactory<Category> categoryFactory, IRepositoryFactory<Color> colorFactory,
            IRepositoryFactory<Size> sizeFactory)
        {
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _shoeService = shoeService ?? throw new ArgumentNullException(nameof(shoeService));
            _brandFactory = brandFactory ?? throw new ArgumentNullException(nameof(brandFactory));
            _categoryFactory = categoryFactory ?? throw new ArgumentNullException(nameof(categoryFactory));
            _colorFactory = colorFactory ?? throw new ArgumentNullException(nameof(colorFactory));
            _sizeFactory = sizeFactory ?? throw new ArgumentNullException(nameof(sizeFactory));
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page=1)
        {
            int pageSize = 6;
            var count = (await _shoeService.ListShoesAsync()).Count();
            var shoes = await _shoeService.ListShoesAsync((page - 1) * pageSize, pageSize);

            ShoeViewModel model = new ShoeViewModel(shoes, count, page, pageSize);

            return View(model);
        }

        [HttpGet]
        public IActionResult CreateShoe()
        {
            UpdateShoeDto shoeDto = new UpdateShoeDto();
            InitializeDto();
            return View(shoeDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateShoe([FromForm] UpdateShoeDto shoeDto)
        {
            ValidationResult validationResult = _validator.Validate(shoeDto);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                InitializeDto();
                return View(shoeDto);
            }

            if (string.IsNullOrEmpty(shoeDto.ImagePath))
            {
                shoeDto.ImagePath = _configuration.GetSection("DefaultImagePath").Value;
            }            

            var shoe = await _shoeService.CreateShoeAsync(shoeDto);

            if (shoe != null)
            {
                return RedirectToAction("Index");
            }

            return BadRequest(shoe);
        }

        [HttpGet]
        public async Task<IActionResult> EditShoe([FromRoute] int id)
        {
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

                InitializeDto();

                return View(shoeDto);
            }

            return BadRequest(shoe);
        }

        [HttpPost]
        public async Task<IActionResult> EditShoe([FromRoute] int id,  [FromForm] UpdateShoeDto shoeDto)
        {
            ValidationResult validationResult = _validator.Validate(shoeDto);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                InitializeDto();
                return View(shoeDto);
            }

            var shoe = _shoeService.UpdateShoeAsync(id,shoeDto);

            if (shoe != null)
            {
                return RedirectToAction("Index");
            }

            return BadRequest(shoe);
        }

        public async Task<IActionResult> DeleteShoe([FromRoute] int id)
        {
            await _shoeService.DeleteShoeAsync(id);
            return RedirectToAction("Index");
        }

        private void InitializeDto()
        {
            ViewBag.Genders = Enum.GetValues(typeof(Gender)).Cast<Gender>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();

            ViewBag.Sizes = new SelectList(_sizeFactory.GetEntities(), "Id", "Value");
            ViewBag.Brands = new SelectList(_brandFactory.GetEntities(), "Id", "Name");
            ViewBag.Categories = new SelectList(_categoryFactory.GetEntities(), "Id", "Name");
            ViewBag.Colors = new SelectList(_colorFactory.GetEntities(), "Id", "Name");
        }
    }
}

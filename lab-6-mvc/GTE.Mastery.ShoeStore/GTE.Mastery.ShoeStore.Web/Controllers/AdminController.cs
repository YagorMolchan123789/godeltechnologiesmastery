using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using GTE.Mastery.ShoeStore.Business.Dtos;
using GTE.Mastery.ShoeStore.Business.Interfaces;
using GTE.Mastery.ShoeStore.Domain.Enums;
using GTE.Mastery.ShoeStore.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using GTE.Mastery.ShoeStore.Web.Configurations;

namespace GTE.Mastery.ShoeStore.Web.Controllers
{
    [Authorize(Roles = nameof(RoleTypes.Admin))]
    public class AdminController : Controller
    {
        private readonly IValidator<UpdateShoeDto> _validator;
        private readonly IDataHelper _dataHelper;
        private readonly ImageOptions _imageOptions;
        private readonly PagingOptions _pagingOptions;
        private readonly IShoeService _shoeService;

        public AdminController(IValidator<UpdateShoeDto> validator, IDataHelper dataHelper,
            IOptions<PagingOptions> pagingOptions, IOptions<ImageOptions> imageOptions, IShoeService shoeService)
        {
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _dataHelper = dataHelper ?? throw new ArgumentNullException(nameof(dataHelper));
            _imageOptions = imageOptions.Value ?? throw new ArgumentNullException(nameof(imageOptions.Value));
            _pagingOptions = pagingOptions.Value ?? throw new ArgumentNullException(nameof(pagingOptions.Value));
            _shoeService = shoeService ?? throw new ArgumentNullException(nameof(shoeService));
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            int maxRowCountPerPage = _pagingOptions.MaxRowCountPerShoePage;

            var totalPageCount = (await _shoeService.ListAsync()).Count();
            var shoes = await _shoeService.ListAsync((page - 1) * maxRowCountPerPage, maxRowCountPerPage);

            ShoeViewModel model = new ShoeViewModel(shoes, totalPageCount, page, maxRowCountPerPage);

            return View(model);
        }

        [HttpGet]
        public IActionResult CreateShoe(int totalRowCount, int maxRowCountPerPage, int totalPageCount)
        {
            UpdateShoeDto shoeDto = new UpdateShoeDto();

            InitializePageParameters(totalRowCount, maxRowCountPerPage, totalPageCount);
            GetAuxillaryData(shoeDto);

            return View(shoeDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateShoe(int totalRowCount, int maxRowCountPerPage, int totalPageCount, UpdateShoeDto shoeDto)
        {
            ValidationResult validationResult = _validator.Validate(shoeDto);

            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);

                InitializePageParameters(totalRowCount, maxRowCountPerPage, totalPageCount);
                GetAuxillaryData(shoeDto);

                return View(shoeDto);
            }

            if (string.IsNullOrEmpty(shoeDto.ImagePath))
            {
                shoeDto.ImagePath = _imageOptions.DefaultPath;
            }

            var shoe = await _shoeService.CreateAsync(shoeDto);

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

            var shoe = await _shoeService.GetAsync(id);

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

                InitializePageParameters(page);
                GetAuxillaryData(shoeDto);

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
                
                InitializePageParameters(page);
                GetAuxillaryData(shoeDto);

                return View(shoeDto);
            }

            var shoe = await _shoeService.UpdateAsync(id, shoeDto);

            if (shoe != null)
            {
                return RedirectToAction("Index", new { page });
            }

            return BadRequest($"The shoe with Id={id} has not been updated successfully");
        }
                
        [HttpPost]
        public async Task<IActionResult> DeleteShoe(ShoeDto shoeDto, int page, int totalPageCount, int totalRowCount, int maxRowCountPerPage)
        {
            await _shoeService.DeleteAsync(shoeDto.Id);

            if (page == totalPageCount && totalRowCount % maxRowCountPerPage == 1)
            {
                return RedirectToAction("Index", new { page = totalPageCount - 1 });
            }

            return RedirectToAction("Index", new { page });
        }

        private void GetAuxillaryData(UpdateShoeDto shoeDto)
        {
            shoeDto.Genders = Enum.GetValues<Gender>().ToList();

            ShoeAuxillaryData shoeAuxillaryData = _dataHelper.GetAuxillaryData();
            
            shoeDto.Brands = shoeAuxillaryData.Brands;
            shoeDto.Categories = shoeAuxillaryData.Categories;
            shoeDto.Sizes = shoeAuxillaryData.Sizes;
            shoeDto.Colors = shoeAuxillaryData.Colors;
        }

        private void InitializePageParameters(int totalRowCount, int maxRowCountPerPage, int totalPageCount)
        {
            ViewBag.TotalRowCount = totalRowCount;
            ViewBag.MaxRowCountPerPage = maxRowCountPerPage;
            ViewBag.TotalPageCount = totalPageCount;
        }

        private void InitializePageParameters(int currentPage)
        {
            ViewBag.CurrentPage = currentPage;
        }
    }
}

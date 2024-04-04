using GTE.Mastery.ShoeStore.Business.Interfaces;
using GTE.Mastery.ShoeStore.Domain;
using GTE.Mastery.ShoeStore.Web.Configurations;
using GTE.Mastery.ShoeStore.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Options;

namespace GTE.Mastery.ShoeStore.Web.Controllers
{
    [Authorize]
    public class ShoeController : Controller
    {
        private readonly IShoeService _shoeService;
        private readonly PagingOptions _pagingOptions;

        public ShoeController(IShoeService shoeService, IOptions<PagingOptions> pagingOptions)
        {
            _shoeService = shoeService ?? throw new ArgumentNullException(nameof(shoeService));
            _pagingOptions = pagingOptions.Value ?? throw new ArgumentNullException(nameof(pagingOptions.Value));
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int maxRowCountPerPage = _pagingOptions.MaxRowCountPerShoePage; 
            var totalRowCount = (await _shoeService.ListAsync()).Count();
            var shoes = await _shoeService.ListAsync((page - 1) * maxRowCountPerPage, maxRowCountPerPage);

            ShoeViewModel model = new ShoeViewModel(shoes, totalRowCount, page, maxRowCountPerPage);

            return View(model);
        }

        public async Task<IActionResult> GetShoe([FromRoute] int id)
        {
            var shoe = await _shoeService.GetAsync(id);

            if (shoe == null)
            {
                return BadRequest($"The shoe with Id={id} is not found");
            }

            return View(shoe);
        }
    }
}

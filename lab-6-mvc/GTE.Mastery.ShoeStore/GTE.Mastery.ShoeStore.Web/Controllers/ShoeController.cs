using GTE.Mastery.ShoeStore.Business.Interfaces;
using GTE.Mastery.ShoeStore.Domain;
using GTE.Mastery.ShoeStore.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;

namespace GTE.Mastery.ShoeStore.Web.Controllers
{
    [Authorize]
    public class ShoeController : Controller
    {
        private readonly IShoeService _shoeService;

        public ShoeController(IShoeService shoeService)
        {
            _shoeService = shoeService ?? throw new ArgumentNullException(nameof(shoeService));
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 4;
            var count = (await _shoeService.ListShoesAsync()).Count();
            var shoes = await _shoeService.ListShoesAsync((page - 1) * pageSize, pageSize);

            PageViewModel pageViewModel = new PageViewModel(count, page, pageSize);
            ShoeViewModel shoeViewModel = new ShoeViewModel(shoes, pageViewModel);

            return View(shoeViewModel);
        }

        public async Task<IActionResult> GetShoe([FromRoute] int id)
        {
            var shoe = await _shoeService.GetShoeAsync(id);
            return View(shoe);
        }
    }
}

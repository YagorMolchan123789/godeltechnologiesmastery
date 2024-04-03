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
        private readonly IConfiguration _configuration;

        public ShoeController(IShoeService shoeService, IConfiguration configuration)
        {
            _shoeService = shoeService ?? throw new ArgumentNullException(nameof(shoeService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int maxRowCountPerPage = int.Parse(_configuration["Paging:MaxRowCountPerShoePage"]); 
            var totalRowCount = (await _shoeService.ListShoesAsync()).Count();
            var shoes = await _shoeService.ListShoesAsync((page - 1) * maxRowCountPerPage, maxRowCountPerPage);

           ShoeViewModel model = new ShoeViewModel(shoes, totalRowCount, page, maxRowCountPerPage);

            return View(model);
        }

        public async Task<IActionResult> GetShoe([FromRoute] int id)
        {
            var shoe = await _shoeService.GetShoeAsync(id);

            if (shoe == null)
            {
                return BadRequest($"The shoe with Id={id} is not found");
            }

            return View(shoe);
        }
    }
}

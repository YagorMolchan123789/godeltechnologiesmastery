using GTE.Mastery.ShoeStore.Business.Interfaces;
using GTE.Mastery.ShoeStore.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GTE.Mastery.ShoeStore.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IShoeService _shoeService;
        private readonly IConfiguration _configuration; 

        public HomeController(IShoeService shoeService, IConfiguration configuration)
        {
            _shoeService = shoeService ?? throw new ArgumentNullException(nameof(shoeService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration)); 
        }

        public async Task<IActionResult> Index()
        {
            int maxRowCountPerHomePage = int.Parse(_configuration["Paging:MaxRowCountPerHomePage"]);

            var shoes = await _shoeService.ListShoesAsync(take: maxRowCountPerHomePage);
            return View(shoes);
        }

        public IActionResult Contact()
        {
            return View();
        }
    }
}

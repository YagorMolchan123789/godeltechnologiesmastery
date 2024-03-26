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

        public HomeController(IShoeService shoeService)
        {
            _shoeService = shoeService ?? throw new ArgumentNullException(nameof(shoeService));
        }

        public async Task<IActionResult> Index()
        {
            int take = 5;
            var shoes = await _shoeService.ListShoesAsync(take: take);
            return View(shoes);
        }

        public IActionResult Contact()
        {
            return View();
        }
    }
}

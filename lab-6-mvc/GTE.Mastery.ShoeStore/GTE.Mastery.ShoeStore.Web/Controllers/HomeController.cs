using GTE.Mastery.ShoeStore.Business.Interfaces;
using GTE.Mastery.ShoeStore.Web.Configurations;
using GTE.Mastery.ShoeStore.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace GTE.Mastery.ShoeStore.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IShoeService _shoeService;
        private readonly PagingOptions _pagingOptions;

        public HomeController(IShoeService shoeService, IOptions<PagingOptions> pagingOptions)
        {
            _shoeService = shoeService ?? throw new ArgumentNullException(nameof(shoeService));
            _pagingOptions = pagingOptions.Value ?? throw new ArgumentNullException(nameof(pagingOptions.Value));
        }

        public async Task<IActionResult> Index()
        {
            int maxRowCountPerPage = _pagingOptions.MaxRowCountPerHomePage;

            var shoes = await _shoeService.ListAsync(take: maxRowCountPerPage);
            return View(shoes);
        }

        public IActionResult Contact()
        {
            return View();
        }
    }
}

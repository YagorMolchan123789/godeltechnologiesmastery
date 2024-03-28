using GTE.Mastery.ShoeStore.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace GTE.Mastery.ShoeStore.Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{code}")]
        public IActionResult Index(int code)
        {
            Response.Clear();
            Response.StatusCode = code;

            ErrorViewModel model = new ErrorViewModel();
            model.ErrorCode = code;

            switch (code)
            {
                case 401:
                    return RedirectToAction("Login", "Account");
                case 403:
                    ViewData["Title"] = "Forbidden Error";
                    model.ErrorMessage = "You do not have sufficient rights to access this resource";
                    break;
                case 404:
                    ViewData["Title"] = "Page Not Found";
                    model.ErrorMessage = "Page is not found";
                    break;
                case 500:
                    ViewData["Title"] = "Internal Server Error";
                    model.ErrorMessage = "Some Internal Server error Occurred while processing your request";
                    break;
            }

            return View(model);
        }


    }
}

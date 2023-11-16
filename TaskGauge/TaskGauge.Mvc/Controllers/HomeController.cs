using Microsoft.AspNetCore.Mvc;
namespace TaskGauge.Mvc.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Room()
        {
            return View();
        }
    }
}
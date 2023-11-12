using Microsoft.AspNetCore.Mvc;

namespace TaskGauge.Mvc.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

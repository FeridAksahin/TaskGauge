using Microsoft.AspNetCore.Mvc;

namespace TaskGauge.Mvc.Controllers
{
    public class RoomController : Controller
    {
        RoomStatic roomUser = RoomStatic.Instance;
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult NewRoom(string roomName)
        {
            return Json(null);
        }
    }
}

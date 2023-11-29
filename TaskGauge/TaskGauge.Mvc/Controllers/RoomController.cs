using Microsoft.AspNetCore.Mvc;
using TaskGauge.DataAccessLayer.Interface;

namespace TaskGauge.Mvc.Controllers
{
    public class RoomController : Controller
    {
        RoomStatic roomUser = RoomStatic.Instance;

        private IRoomDal _roomDal;


        public RoomController(IRoomDal roomDal)
        {
            _roomDal = roomDal;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult NewRoom(string roomName)
        {
            if (_roomDal.IsRoomExist(roomName))
            {
                return Json("Room name is exist.");
            }
            _roomDal.AddRoom(roomName);
            return Json(null);
        }
    }
}

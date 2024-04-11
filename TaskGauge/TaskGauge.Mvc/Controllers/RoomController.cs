using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskGauge.Common;
using TaskGauge.DataAccessLayer.Interface;
using TaskGauge.DataTransferObject;
using TaskGauge.ViewModel;

namespace TaskGauge.Mvc.Controllers
{

    [Authorize]
    public class RoomController : Controller
    {
        RoomStatic roomUser = RoomStatic.Instance;
        public static bool IsAllRoomExist = false;
        private IRoomDal _roomDal;
        private UserInformation _user;


        public RoomController(IRoomDal roomDal, UserInformation user)
        {
            _roomDal = roomDal;
            _user = user;
            if (!IsAllRoomExist)
            {
                _roomDal.GetAllRoomIntoStaticList();
                _roomDal.GetAllTaskIntoRedisList();
                IsAllRoomExist = true;
            }
        }

        public IActionResult Index(string roomName = "")
        {
            bool isAdmin = false;
            if (!string.IsNullOrEmpty(roomName))
            {
                isAdmin = _roomDal.IsTheLoggedInUserTheRoomAdministrator(roomName);
            }
            ViewBag.IsAdmin = isAdmin;
            ViewBag.RoomName = roomName;    
            return View(isAdmin);
        }

        [HttpPost]
        public JsonResult NewRoom(string roomName)
        {
            if (_roomDal.IsRoomExist(roomName))
            {
                return Json("Room name is exist.");
            }
            var result = _roomDal.AddRoom(roomName);

            if (result)
            {
                RoomStatic.Instance.room.Add(new Room { isActive = true, Name = roomName });
            }
            return result ? Json(true) : Json("Try again.");

        }

        [HttpGet]
        public JsonResult JoinRoom(string roomName)
        {
            return Json(IsRoomExistAndActive(roomName));
        }

        private string IsRoomExistAndActive(string roomName)
        {
            try
            {
                foreach (var item in roomUser.room)
                {
                    if (item.Name.Equals(roomName) && item.isActive)
                    {
                        return "success";
                    }
                }

                return _roomDal.IsExistRoomName(roomName) ? "error: The room is closed." : "error: The room entered does not exist.";
            }
            catch(Exception exception)
            {
                return $"error: {exception.Message}";
            }
        }
    }
}

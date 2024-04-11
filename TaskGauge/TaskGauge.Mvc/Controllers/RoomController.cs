using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text.Json;
using TaskGauge.Common;
using TaskGauge.DataAccessLayer.Interface;
using TaskGauge.DataTransferObject;
using TaskGauge.Services;

namespace TaskGauge.Mvc.Controllers
{

    [Authorize]
    public class RoomController : Controller
    {
        RoomStatic roomUser = RoomStatic.Instance;
        public static bool IsAllRoomExist = false;
        private IRoomDal _roomDal;
        private UserInformation _user;
        private readonly IDatabase _redisDatabase;


        public RoomController(IRoomDal roomDal, UserInformation user, RedisService redisService)
        {
            _redisDatabase = redisService.Connect(0);
            _roomDal = roomDal;
            _user = user;
            if (!IsAllRoomExist)
            {
                _roomDal.GetAllRoomIntoRedisList();
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
                _redisDatabase.ListRightPush(TextResources.RedisCacheKeys.AllActiveRoom, JsonSerializer.Serialize(new Room { isActive = true, Name = roomName }));
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
                foreach (var item in _roomDal.GetAllActiveRoomFromRedis())
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

using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TaskGauge.Common;
using TaskGauge.Common.Enum;
using TaskGauge.DataAccessLayer.Interface;
using TaskGauge.DataTransferObject;

namespace TaskGauge.Mvc.Controllers
{

    public class LoginController : Controller
    {

        private IUserDal _userDal;

        public LoginController(IUserDal userDal)
        {
            _userDal = userDal;
        }

        [HttpGet]
        public IActionResult Index(bool isRegisterUser = false)
        {
            ViewBag.RegisterMessage = isRegisterUser ? TextResources.SuccessfullyRegisteredUser : null;
            var securityQuestions = Enum.GetValues(typeof(UserSecurityQuestion))
            .Cast<UserSecurityQuestion>()
            .Select(x => new
            {
                Id = (int)x,
                Text = GetEnumDisplayName(x)
            })
            .ToList();

            ViewBag.SecurityQuestions = securityQuestions;
            return View();
        }

        [HttpPost]
        public JsonResult Register(RegisterDto registerDto)
        {
            var result = _userDal.Register(registerDto);
            string type = "success";
            if (result.Contains("error"))
            {
                type = "error";
            }
            return Json(new {message = result.Split("Type")[0], type = type});
        }

        private string GetEnumDisplayName(Enum value)
        {
            var displayAttribute = value.GetType()
                .GetField(value.ToString())
                .GetCustomAttributes(typeof(DisplayAttribute), false)
                .SingleOrDefault() as DisplayAttribute;

            return displayAttribute?.Name ?? value.ToString();
        }
    }
}

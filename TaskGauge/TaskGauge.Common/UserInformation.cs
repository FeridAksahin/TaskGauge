using Microsoft.AspNetCore.Http;
using System;

namespace TaskGauge.Common
{
    public class UserInformation
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserInformation(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public string GetUsernameFromCookie()
        { 
            var httpContext = _httpContextAccessor.HttpContext;

            return httpContext.Request.Cookies["Username"]; 
        }

        public int GetUserIdFromCookie()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            return Convert.ToInt32(httpContext.Request.Cookies["UserId"]); 
        }

        public string GetUserRole()
        {
            return _httpContextAccessor.HttpContext.Request.Cookies["UserRole"];
        }

        public string GetSessionValue(string key)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null && httpContext.Session.TryGetValue(key, out byte[] value))
            {
                return System.Text.Encoding.UTF8.GetString(value);
            }

            return null;
        }
    }
}

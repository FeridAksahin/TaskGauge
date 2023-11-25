﻿using Microsoft.AspNetCore.Http;
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

        public string GetUserNameFromCookie()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            string userName = httpContext.Request.Cookies["YourCookieName"];

            return userName;
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
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskGauge.Common
{
    public static class TextResources
    {
        public static readonly string UsernameExists = "The username entered already exists. Please enter another username."; 
        public static readonly string SuccessfullyRegisteredUser = "You have registered."; 
        public static readonly string WrongPassword = "The entered password is incorrect."; 
        public static readonly string WrongUsername = "The entered username is incorrect."; 
        public static class RedisCacheKeys
        {
            public static readonly string AllRoomTasks = "allRoomTasks";
            public static readonly string AllActiveRoom = "allActiveRoom";
            public static readonly string RoomUser = "roomUser";
        }
    }
}

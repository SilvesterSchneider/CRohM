﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.DataTransferObjects
{
    public class UserDto
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string AccessToken { get; set; }
        //TODO: implement endpoint for login with refresh token
        //public string RefreshToken { get; set; }
    }

    public class UserCreateDto
    {
        public string Email { get; set; }
    }
}
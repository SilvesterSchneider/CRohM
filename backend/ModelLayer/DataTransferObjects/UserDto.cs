using ModelLayer.Helper;
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ModelLayer.DataTransferObjects
{
    /// <summary>
    /// RAM: 20%
    /// </summary>
    public class UserDto
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string AccessToken { get; set; }
        public bool UserLockEnabled { get; set; }
        public bool hasPasswordChanged { get; set; }
        public DateTime LastLoginDate { get; set; }
        public bool IsSuperAdmin { get; set; }
        //TODO: implement endpoint for login with refresh token
        //public string RefreshToken { get; set; }
    }

    public class UserCreateDto
    {
        [Required(AllowEmptyStrings = false)]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string LastName { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Email { get; set; }
    }
}

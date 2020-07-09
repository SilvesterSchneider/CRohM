using ModelLayer.Models;
using ModelLayer.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Helper
{
    public enum UserRight
    {
        USER_DELETE = 1,
        USER_CREATE = 2
    }

    public class AllRoles {
        private static Dictionary<UserRight, string> rolesNames = GetStringValues();
        private static List<Permission> all = AllRole();

        private static Dictionary<UserRight, string> GetStringValues()
        {
            Dictionary<UserRight, string> dict = new Dictionary<UserRight, string>();
            dict.Add(UserRight.USER_CREATE, "CreateUser");
            dict.Add(UserRight.USER_DELETE, "DeleteUser");
            return dict;
        }

        private static List<Permission> AllRole() {
            List<Permission> list = new List<Permission>();
            foreach (UserRight key in rolesNames.Keys)
            {
                list.Add(new Permission() { Id = 0, Name = rolesNames[key], UserRight = key });
            }
            return list;
        }

        public static List<Permission> GetAllRoles()
        {
            return new List<Permission>(all);
        }

        public static Dictionary<UserRight, string> GetRolesNamesMap()
        {
            return rolesNames;
        }
    }
}

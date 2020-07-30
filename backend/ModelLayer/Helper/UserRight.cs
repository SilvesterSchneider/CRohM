using ModelLayer.Models;
using ModelLayer.Models.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelLayer.Helper
{
    // TODO: Instead of putting all permissions in one enum -> separate regarding roles (admin, datenschutzbeauftragter etc.)
    public enum UserRight
    {
        USER_DELETE = 1,         //permission to delete an user
        USER_CREATE = 2,         //permission to create an user
        DELETE_CONTACT = 3,      //permission to delete a contact
        ASSIGN_ROLE = 4,         //permission to assign user a new role
        RESET_USER_PASSWORT = 5, //permission to reset the passwort of an user
        MODIFY_ROLECONCEPT = 6,  //permission to see and modify roleconcept
        MODIFY_CONTACT = 7,      //permission to see and modify contacts 
        ACCESS_CONTACT_DATA = 8, //permission to see data of a contact
        INFORM_CONTACT = 9       //permission to inform contact after changing or deleting contact data 
    }
    // TODO: Rename roles to permissions !! 
    public class AllRoles {
        private static Dictionary<UserRight, string> rolesNames = GetStringValues();
        private static List<Permission> all = AllRole();
        //private static List<Permission> adminPermission = AdminPermission();

        private static Dictionary<UserRight, string> GetStringValues()
        {
            Dictionary<UserRight, string> dict = new Dictionary<UserRight, string>();
            dict.Add(UserRight.USER_CREATE, "CreateUser");
            dict.Add(UserRight.USER_DELETE, "DeleteUser");
            dict.Add(UserRight.DELETE_CONTACT, "DeleteContact");
            dict.Add(UserRight.ASSIGN_ROLE, "AssignRole");
            dict.Add(UserRight.RESET_USER_PASSWORT, "ResetUserPasswort");
            dict.Add(UserRight.MODIFY_ROLECONCEPT, "ModifyRoleConcept");
            dict.Add(UserRight.MODIFY_CONTACT, "ModifyContact");
            dict.Add(UserRight.ACCESS_CONTACT_DATA, "AccessContactData");
            dict.Add(UserRight.INFORM_CONTACT, "InformContact");
            
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
        // Is this method neccessary? When the above exist AllRole()
        public static List<Permission> GetAllRoles()
        {
            return new List<Permission>(all);
        }


        // Set the permissions of an admin 
        public static List<Permission> AdminPermissions()
        {
             List<Permission> list = new List<Permission>();
             foreach (KeyValuePair<UserRight, string> key in rolesNames)
             {
                 // check if value matches an admin permission
                 if (key.Value == "CreateUser" || key.Value == "DeleteUser" || key.Value == "DeleteContact" ||
                     key.Value == "AssignRole" || key.Value == "ResetUserPasswort" ||
                     key.Value == "ModifyRoleConcept")
                 {
                     list.Add(new Permission() { Id = 0, Name = key.Value, UserRight = key.Key});
                 }
             }
             return list;
        }

        // Set the permissions of a Datenschutzbeauftragen
        public static List<Permission> DatenschutzBeauftragterPermissions()
        {
            List<Permission> list = new List<Permission>();
            foreach (KeyValuePair<UserRight, string> key in rolesNames)
            {
                if(key.Value == "ModifyContact" || key.Value == "DeleteContact" ||
                   key.Value == "AccessContactData" || key.Value == "InformContact")
                {
                    list.Add(new Permission() { Id = 1, Name = key.Value, UserRight = key.Key });
                }
            }
            return list;
        }

        public static Dictionary<UserRight, string> GetRolesNamesMap()
        {
            return rolesNames;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using ModelLayer;
using ModelLayer.Helper;
using ModelLayer.Models;
using RepositoryLayer;

namespace ServiceLayer
{
    public interface IUserService
    {

        Task<User> FindByNameAsync(string credentialsName);

        Task<IdentityResult> CreateCRohMUserAsync(User user);

        Task<User> FindByEmailAsync(string email);

        Task<IdentityResult> CreateAsync(User user, string password);

        Task<IdentityResult> AddToRoleAsync(User user, string role);

        Task ChangePasswordForUser(int primKey);

        Task<List<User>> GetAsync();
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetByIdAsync(long id);
        Task<List<string>> GetRolesAsync(User user);

        Task DeleteUserAsync(User user);
        Task<bool> DeletePermissionGroupByUserIdAsync(long groupIdToDelete, long userId);
        Task<bool> AddPermissionGroupByUserIdAsync(long permissionGroupId, long userId);

        Task<IdentityResult> SetUserLockedAsync(long id);

        Task<string> GetUserNameByIdAsync(long id);

        Task<IdentityResult> ChangePasswordForUserAsync(long primKey, string newPassword);
        Task UpdateUserAsync(User user);
        Task SetUserNameAsync(User user, string username);
        Task<IdentityResult> UpdateAsync(User user);
    }

    public class UserService : IUserService
    {
        private readonly IUserManager _userManager;


        private readonly IMailProvider mailProvider;

        private IUserPermissionGroupRepository userPermissionGroupRepo;

        private readonly IPermissionGroupService permissionsService;
        public IQueryable<User> Users => _userManager.Users;

        public UserService(IUserManager userManager, IMailProvider mailProvider, IPermissionGroupService permissionsService,
            IUserPermissionGroupRepository userPermissionGroupRepo)
        {
            this.permissionsService = permissionsService;
            this.userPermissionGroupRepo = userPermissionGroupRepo;
            _userManager = userManager;
            this.mailProvider = mailProvider;
        }

        public UserService(IUserManager userManager, IMailProvider mailProvider)
        {

            _userManager = userManager;
            this.mailProvider = mailProvider;
        }

        public async Task<IdentityResult> CreateCRohMUserAsync(User user)
        {
            PasswordGenerator pwGenerator = new PasswordGenerator();
            var password = pwGenerator.Generate();

            user.UserName = await GetUniqueUserNameAsync(user.FirstName, user.LastName);

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                mailProvider.Registration(user.UserName, password, user.Email);
            }

            return result;
        }

        /// <summary>
        /// Change the password service.
        /// </summary>
        /// <param name="primKey">the primary key of the user to be changed</param>
        /// <returns></returns>
        public async Task ChangePasswordForUser(int primKey)
        {
            //TODO: refactor -> make unit test, return result
            //TODO: generate client class for frontend
            User userToBeUpdated = null;
            //why iterate over user?
            foreach (User us in Users)
            {
                if (us.Id == primKey)
                {
                    userToBeUpdated = us;
                    break;
                }
            }
            if (userToBeUpdated != null && !string.IsNullOrEmpty(userToBeUpdated.Email))
            {
                string newPassword = new PasswordGenerator(PasswordGuidelines.RequiredMinLength, PasswordGuidelines.GetMaximumLength(),
                    PasswordGuidelines.GetAmountOfLowerLetters(), PasswordGuidelines.GetAmountOfUpperLetters(), PasswordGuidelines.GetAmountOfNumerics(),
                    PasswordGuidelines.GetAmountOfSpecialChars()).Generate();
                await _userManager.ChangePasswordAsync(userToBeUpdated, newPassword);
                mailProvider.PasswordReset(newPassword, userToBeUpdated.Email);
            }
        }

        public async Task<List<User>> GetAsync()
        {
            List<User> users = await _userManager.Users.Include(x => x.Permission).ThenInclude(y => y.PermissionGroup).ToListAsync();
          /*  foreach (User usr in users)
            {
                foreach (PermissionGroup groups in usr.Permission)
                {
                    PermissionGroup permissions = await permissionsService.GetPermissionGroupByIdAsync(groups.Id);
                }
            } */
            return users;
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IdentityResult> CreateAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> AddToRoleAsync(User user, string role)
        {
            return await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<List<string>> GetRolesAsync(User user)
        {
            var list = await _userManager.GetRolesAsync(user);
            return list.ToList();
        }

        /// <summary>
        /// Get the unique username
        /// </summary>
        /// <param name="firstName">the first name</param>
        /// <param name="lastName">the last name</param>
        /// <returns>the created unique username</returns>
        public virtual async Task<string> GetUniqueUserNameAsync(string firstName, string lastName) //needs to be virtual for unit test
        {
            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentNullException(nameof(firstName), "firstName can not be null");
            }
            if (string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentNullException(nameof(lastName), "lastName can not be null");
            }

            var count = await Users.CountAsync();

            return lastName + firstName.Substring(0, 2) + count + 1;
        }

        public async Task<User> FindByNameAsync(string credentialsName)
        {
            return await _userManager.FindByNameAsync(credentialsName);
        }

        public async Task<IdentityResult> SetUserLockedAsync(long id)
        {
            User user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user != null && !user.IsDeleted)
            {
                return await _userManager.SetUserLockedAsync(user, !user.UserLockEnabled);
            }
            else if (user == null)
            {
                return IdentityResult.Failed(new IdentityError());
            }
            else
            {
                return IdentityResult.Success;
            }
        }

        public async Task<User> GetByIdAsync(long id)
        {
            return await _userManager.Users.Include(x => x.Permission).ThenInclude(y => y.PermissionGroup).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> AddPermissionGroupByUserIdAsync(long permissionGroupId, long userId)
        {
            PermissionGroup permissiongroup = await permissionsService.GetByIdAsync(permissionGroupId);
            User user = await GetByIdAsync(userId);

            if (user != null && permissiongroup != null)
            {
                foreach (UserPermissionGroup group in user.Permission)
                {
                    if (group.PermissionGroupId == permissionGroupId)
                    {
                        return true;
                    }
                }
                UserPermissionGroup newConnection = new UserPermissionGroup() { PermissionGroup = permissiongroup, User = user, UserId = userId, PermissionGroupId = permissionGroupId };
                UserPermissionGroup savedConnection = await userPermissionGroupRepo.CreateAsync(newConnection);
                if (savedConnection != null)
                {
                    user.Permission.Add(savedConnection);
                    IdentityResult result = await _userManager.UpdateAsync(user);
                    IList<string> permissions = await _userManager.GetRolesAsync(user);
                    foreach (Permission perm in permissiongroup.Permissions)
                    {
                        if (!permissions.Contains(perm.Name))
                        {
                            await _userManager.AddToRoleAsync(user, perm.Name);
                        }
                    }
                    if (result == IdentityResult.Success)
                    {
                        return true;
                    }
                } else
                {
                    return false;
                }
            }
            return false;
        }

        public async Task<bool> DeletePermissionGroupByUserIdAsync(long GroupIdToDelete, long userId)
        {
            User modifieduser = await GetByIdAsync(userId);
            UserPermissionGroup connectionToDelete = await userPermissionGroupRepo.GetUserPermissionGroupByIdAsync(GroupIdToDelete, userId);
            bool adminsleft = false;
            if (connectionToDelete == null || modifieduser == null)
            {
                return false;
            }

            //Überprüfen ob noch ein Admin übrig bleibt -> Wegen dem Seed ist die Adminid = 1
            if (GroupIdToDelete == 1)
            {
                List<User> users = await GetAsync();
                foreach (User user in users)
                {
                    if (user != modifieduser)
                    {
                        foreach (UserPermissionGroup connection in user.Permission)
                        {
                            if (connection.PermissionGroupId == 1)
                            {
                                adminsleft = true;
                            }
                        }
                    }
                }
            }

            if (GroupIdToDelete != 1 || adminsleft)
            {
                List<Permission> permissionstodelete = new List<Permission>();
                List<Permission> permissionstoStay = new List<Permission>();
                foreach (UserPermissionGroup connection in modifieduser.Permission)
                {
                    PermissionGroup group = connection.PermissionGroup;
                    if (group != null && group.Id == GroupIdToDelete)
                    {
                        foreach (Permission perm in group.Permissions)
                        {
                            permissionstodelete.Add(perm);
                        }
                    }
                    else if (group != null)
                    {
                        foreach (Permission perm in group.Permissions)
                        {
                            permissionstoStay.Add(perm);
                        }
                    }

                }

                //Lösche Permission wenn sie nicht in anderer zugewiesener Permissiongroup vorkommt.
                foreach (Permission permToDelete in permissionstodelete)
                {
                    if (!permissionstoStay.Contains(permToDelete))
                    {
                        await _userManager.RemoveRolesAsync(modifieduser, permToDelete.Name);
                    }
                }
                modifieduser.Permission.Remove(connectionToDelete);
                await userPermissionGroupRepo.DeleteAsync(connectionToDelete);
                await _userManager.UpdateAsync(modifieduser);
                return true;
            }
            return false;
        }

        public async Task<string> GetUserNameByIdAsync(long id)
        {
            User userToFind = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (userToFind != null)
            {
                string userName = userToFind.FirstName + " " + userToFind.LastName;
                if (string.IsNullOrEmpty(userName.Trim()))
                {
                    userName = "admin";
                }
                return userName;
            }
            else
            {
                return string.Empty;
            }
        }

        public async Task<IdentityResult> ChangePasswordForUserAsync(long primKey, string newPassword)
        {
            User userToBeUpdated = Users.FirstOrDefault(x => x.Id == primKey);
            if (userToBeUpdated != null && !string.IsNullOrEmpty(userToBeUpdated.Email))
            {
                await _userManager.ChangePasswordAsync(userToBeUpdated, newPassword);
                userToBeUpdated.hasPasswordChanged = true;
                return await _userManager.UpdateUserAsync(userToBeUpdated);
            }
            else
            {
                return IdentityResult.Failed(new IdentityError[] { new IdentityError() { Code = "404", Description = "Benutzer nicht gefunden!" } });
            }
        }

        public async Task UpdateUserAsync(User user)
        {
            await _userManager.UpdateAsync(user);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task DeleteUserAsync(User user)
        {
            await _userManager.DeleteUserAsync(user);
		}
		
        public async Task<IdentityResult> UpdateAsync(User user)
        {
            User userToUpdate = await _userManager.Users.Include(x => x.Permission).FirstOrDefaultAsync(x => x.Id == user.Id);
            if (userToUpdate != null)
            {
                userToUpdate.Email = user.Email;
                userToUpdate.FirstName = user.FirstName;
                userToUpdate.LastName = user.LastName;
                return await _userManager.UpdateUserAsync(userToUpdate);
            }
            else
            {
                return IdentityResult.Failed(new IdentityError[] { new IdentityError() { Code = "Nicht gefunden", Description = "User nicht gefunden!" } });
            }
        }

        public async Task SetUserNameAsync(User user, string username)
        {
            await _userManager.SetUserNameAsync(user, username);
        }
    }

    #region DefaultUserManager

    public interface IUserManager
    {
        Task<IdentityResult> CreateAsync(User user);

        Task<IdentityResult> CreateAsync(User user, string password);

        Task<User> FindByNameAsync(string name);

        Task<User> FindByEmailAsync(string email);

        Task<IdentityResult> AddToRoleAsync(User user, string role);
        Task<IdentityResult> RemoveRolesAsync(User user, string permission);

        Task<IdentityResult> ChangePasswordAsync(User user, string newPassword);

        Task<IdentityResult> SetUserLockedAsync(User user, bool lockoutEnabled);

        Task<IdentityResult> UpdateAsync(User user);
        Task<IList<string>> GetRolesAsync(User user);


        IQueryable<User> Users { get; }

        Task DeleteUserAsync(User user);

        Task<IdentityResult> UpdateUserAsync(User user);
        Task SetUserNameAsync(User user, string username);
    }

    public class DefaultUserManager : IUserManager
    {
        private const int YESTERDAY = -1;
        private const int FUTURE_YEARS = 100;
        private readonly UserManager<User> _manager;

        public DefaultUserManager(UserManager<User> manager)
        {
            _manager = manager;
        }

        public async Task<IdentityResult> CreateAsync(User user)
        {
            return await _manager.CreateAsync(user);
        }

        public async Task DeleteUserAsync(User user)
        {
            await _manager.DeleteAsync(user);
        }

        public async Task<IdentityResult> CreateAsync(User user, string password)
        {
            return await _manager.CreateAsync(user, password);
        }

        public async Task<IList<string>> GetRolesAsync(User user)
        {
            return await _manager.GetRolesAsync(user);
        }

        public async Task<IdentityResult> RemoveRolesAsync(User user, string permission)
        {
            return await _manager.RemoveFromRoleAsync(user, permission);
        }

        public async Task<IdentityResult> UpdateAsync(User user)
        {
            return await _manager.UpdateAsync(user);
        }

        public async Task<IdentityResult> SetUserLockedAsync(User user, bool lockEnabled)
        {
            DateTime date = DateTime.Today.AddDays(YESTERDAY);
            if (lockEnabled)
            {
                date = date.AddYears(FUTURE_YEARS);
            }
            return await _manager.SetLockoutEndDateAsync(user, date);
        }

        public async Task<User> FindByNameAsync(string name)
        {
            return await _manager.FindByNameAsync(name);
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            return await _manager.FindByEmailAsync(email);
        }

        public async Task<IdentityResult> AddToRoleAsync(User user, string role)
        {
            return await _manager.AddToRoleAsync(user, role);
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string newPassword)
        {
            await _manager.RemovePasswordAsync(user);
            return await _manager.AddPasswordAsync(user, newPassword);
        }

        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            return await _manager.UpdateAsync(user);
        }

        public async Task SetUserNameAsync(User user, string username)
        {
            await _manager.SetUserNameAsync(user, username);
        }

        public IQueryable<User> Users => _manager.Users.Include(x => x.Permission);
    }

    #endregion DefaultUserManager

}

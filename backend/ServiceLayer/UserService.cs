using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ModelLayer;
using ModelLayer.Helper;
using ModelLayer.Models;

namespace ServiceLayer
{
    public interface IUserService
    {
        Task<bool> SetPermissionIdsByUserIdAsync(List<int> permissionGroups, long id);

        Task<User> FindByNameAsync(string credentialsName);

        Task<IdentityResult> CreateCRohMUserAsync(User user);

        Task<User> FindByEmailAsync(string email);

        Task<IdentityResult> CreateAsync(User user, string password);

        Task<IdentityResult> AddToRoleAsync(User user, string role);

        Task ChangePasswordForUser(int primKey);

        Task<List<User>> GetAsync();

        Task<User> GetByIdAsync(long id);
    }

    public class UserService : IUserService
    {
        private readonly IUserManager _userManager;

        //TODO: fix it with di
        private readonly IMailProvider mailProvider;

        private readonly IPermissionGroupService permissionsService;
        public IQueryable<User> Users => _userManager.Users;

        public UserService(IUserManager userManager, IMailProvider mailProvider, IPermissionGroupService permissionsService)
        {
            this.permissionsService = permissionsService;
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
                string newPassword = new PasswordGenerator(PasswordGuidelines.RequiredLength, PasswordGuidelines.GetMaximumLength(),
                    PasswordGuidelines.GetAmountOfLowerLetters(), PasswordGuidelines.GetAmountOfUpperLetters(), PasswordGuidelines.GetAmountOfNumerics(),
                    PasswordGuidelines.GetAmountOfSpecialChars()).Generate();
                await _userManager.ChangePasswordAsync(userToBeUpdated, userToBeUpdated.PasswordHash, newPassword).ConfigureAwait(false);
                mailProvider.PasswordReset(newPassword, userToBeUpdated.Email);
            }
        }

        public async Task<List<User>> GetAsync()
        {
            List<User> users = await _userManager.Users.ToListAsync();
            foreach (User usr in users)
            {
                foreach (PermissionGroup groups in usr.Permission)
                {               
                    PermissionGroup permissions = await permissionsService.GetPermissionGroupByIdAsync(groups.Id);
                    //groups.Permissions.AddRange(permissions.Permissions); //TODO Silvester fragen
                }
            }
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

        public async Task<User> GetByIdAsync(long id)
        {
            return await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
        }


        public async Task<bool> SetPermissionIdsByUserIdAsync(List<int> permissionGroups, long id)
        {
            List<User> users = await GetAsync();
            List<PermissionGroup> allpermissiongroups = await permissionsService.GetAllPermissionGroupAsync();
            User user = await GetByIdAsync(id);
            bool adminsleft = false;
            if (user != null)
            {
                if (!permissionGroups.Contains(1))
                {
                    foreach (User u in users)
                    {
                        if (user != u)
                        {
                            foreach (PermissionGroup group in u.Permission)
                            {
                                if (group.Id == 1)
                                {
                                    adminsleft = true;
                                    break;
                                }
                            }
                        }
                        if (adminsleft)
                        {
                            break;
                        }
                    }
                }
                else {
                    adminsleft = true;
                }


                if (adminsleft == true)
                {
                    List<PermissionGroup> groupstodelete = new List<PermissionGroup>();

                    user.Permission.ForEach(x =>
                    {
                        if (!permissionGroups.Contains(int.Parse(x.Id.ToString())))
                        {
                            groupstodelete.Add(x);
                        }
                    });

                    groupstodelete.ForEach(x => {
                        user.Permission.Remove(x);
                    });


                    permissionGroups.ForEach(x => {
                        allpermissiongroups.ForEach(y => {
                            if (y.Id == x)
                            {
                                user.Permission.Add(y);

                            }
                        });
                    });

                    

                    
                    await _userManager.UpdateAsync(user);
                    return true;
                }
            }
            return false;
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

        Task<IdentityResult> ChangePasswordAsync(User user, string newPassword, string currentPassword);

        Task<IdentityResult> UpdateAsync(User user);


        IQueryable<User> Users { get; }
    }

    public class DefaultUserManager : IUserManager
    {
        private readonly UserManager<User> _manager;

        public DefaultUserManager(UserManager<User> manager)
        {
            _manager = manager;
        }

        public async Task<IdentityResult> CreateAsync(User user)
        {
            return await _manager.CreateAsync(user);
        }

        public async Task<IdentityResult> CreateAsync(User user, string password)
        {
            return await _manager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> UpdateAsync(User user)
        {
            return await _manager.UpdateAsync(user);
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

        public async Task<IdentityResult> ChangePasswordAsync(User user, string newPassword, string currentPassword)
        {
            return await _manager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public IQueryable<User> Users => _manager.Users.Include(x => x.Permission);
    }

    #endregion DefaultUserManager
}
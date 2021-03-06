using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

        Task ChangePasswordForUser(int primKey);

        Task<List<User>> GetAsync();

        Task<List<User>> GetAllUsersAsync();

        Task<User> GetByIdAsync(long id);

        Task<List<string>> GetRolesAsync(User user);

        Task<IdentityResult> AddToRoleAsync(User user, string role);

        Task<IdentityResult> RemoveRoleFromUser(User user, string permission);

        Task<IList<Claim>> GetClaimsAsync(User user);

        Task<IdentityResult> AddClaimAsync(User user, Claim claim);

        Task<IdentityResult> RemoveClaimAsync(User user, Claim claim);

        Task<IdentityResult> DeleteUserAsync(User user);

        Task<IdentityResult> SetUserLockedAsync(long id);

        Task<string> GetUserNameByIdAsync(long id);

        Task<IdentityResult> ChangePasswordForUserAsync(long primKey, string newPassword);

        Task UpdateUserAsync(User user);

        Task<IdentityResult> SetUserNameAsync(User user, string username);

        Task<IdentityResult> UpdateAsync(User user);

        /// <summary>
        /// Update all roles of a user
        /// </summary>
        /// <param name="userId">the user id</param>
        /// <param name="roles">all roles to be assigned</param>
        /// <returns></returns>
        Task<IdentityResult> UpdateAllRolesAsync(long userId, List<string> roles);

        /// <summary>
        /// Determines whether a user is a datenschutzbeauftragter or not
        /// </summary>
        /// <param name="userId">user id</param>
        /// <returns>true if it is a datenschutzbeauftragter, otherwise false</returns>
        Task<bool> IsDataSecurityEngineer(long userId);

        /// <summary>
        /// Returns all user with specific role
        /// </summary>
        /// <param name="roleName">Role name which will be searched for</param>
        /// <returns>List of users</returns>
        Task<List<User>> GetUsersInRoleAsync(string roleName);
    }

    public class UserService : IUserService
    {
        private readonly IUserManager _userManager;

        private readonly IMailService mailProvider;

        private IRoleService roleService;

        public IQueryable<User> Users => _userManager.Users;

        public UserService(IUserManager userManager, IMailService mailProvider, IRoleService roleService)
        {
            _userManager = userManager;
            this.roleService = roleService;
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
            List<User> users = await _userManager.Users.ToListAsync();
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
            return await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
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

        public async Task<IdentityResult> DeleteUserAsync(User user)
        {
            return await _userManager.DeleteUserAsync(user);
        }

        public async Task<IdentityResult> UpdateAsync(User user)
        {
            User userToUpdate = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
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

        public async Task<IdentityResult> SetUserNameAsync(User user, string username)
        {
            return await _userManager.SetUserNameAsync(user, username);
        }

        public async Task<IList<Claim>> GetClaimsAsync(User user)
        {
            return await _userManager.GetClaimsAsync(user);
        }

        public async Task<IdentityResult> AddClaimAsync(User user, Claim claim)
        {
            return await _userManager.AddClaimAsync(user, claim);
        }

        public async Task<IdentityResult> RemoveClaimAsync(User user, Claim claim)
        {
            return await _userManager.RemoveClaimAsync(user, claim);
        }

        public async Task<IdentityResult> RemoveRoleFromUser(User user, string role)
        {
            return await _userManager.RemoveRoleFromUser(user, role);
        }

        public async Task<IdentityResult> UpdateAllRolesAsync(long userId, List<string> roles)
        {
            User user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError() { Code = "301", Description = "User not found!" });
            }
            if (user.Id == 1 && !roles.Contains(RoleClaims.ADMIN_GROUP))
            {
                roles.Add(RoleClaims.ADMIN_GROUP);
            }
            List<Claim> claimsToHave = new List<Claim>();
            foreach (string role in roles)
            {
                foreach (Claim claimToHave in await roleService.GetClaimsAsync(await roleService.FindRoleByNameAsync(role)))
                {
                    if (!claimsToHave.Contains(claimToHave))
                    {
                        claimsToHave.Add(claimToHave);
                    }
                }
            }
            List<string> existingRoles = await GetRolesAsync(user);
            List<string> rolesToDelete = new List<string>();
            foreach (string roleToDelete in existingRoles)
            {
                if (roles.FirstOrDefault(x => x.Equals(roleToDelete)) == null)
                {
                    rolesToDelete.Add(roleToDelete);
                }
            }
            foreach (string roleToDelete in rolesToDelete)
            {
                await RemoveRoleFromUser(user, roleToDelete);
                foreach (Claim claimToDelete in await roleService.GetClaimsAsync(await roleService.FindRoleByNameAsync(roleToDelete)))
                {
                    if (!claimsToHave.Contains(claimToDelete))
                    {
                        await RemoveClaimAsync(user, claimToDelete);
                    }
                }
            }
            List<string> rolesToAdd = new List<string>();
            foreach (string roleToAdd in roles)
            {
                if (existingRoles.FirstOrDefault(x => x.Equals(roleToAdd)) == null)
                {
                    rolesToAdd.Add(roleToAdd);
                }
            }
            foreach (string roleToAdd in rolesToAdd)
            {
                await AddToRoleAsync(user, roleToAdd);
                foreach (Claim claimToAdd in await roleService.GetClaimsAsync(await roleService.FindRoleByNameAsync(roleToAdd)))
                {
                    await AddClaimAsync(user, claimToAdd);
                }
            }
            return IdentityResult.Success;
        }

        public async Task<bool> IsDataSecurityEngineer(long userId)
        {
            User user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
            {
                return false;
            }
            List<string> roles = await GetRolesAsync(user);
            return roles.Contains(RoleClaims.DATA_SECURITY_ENGINEER_GROUP);
        }

        public async Task<List<User>> GetUsersInRoleAsync(string roleName)
        {
            IList<User> usersInRoleAsync = await _userManager.GetUsersInRoleAsync(roleName);
            return usersInRoleAsync.ToList();
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

        Task<IdentityResult> RemoveRoleFromUser(User user, string role);

        Task<IdentityResult> ChangePasswordAsync(User user, string newPassword);

        Task<IdentityResult> SetUserLockedAsync(User user, bool lockoutEnabled);

        Task<IdentityResult> UpdateAsync(User user);

        Task<IList<string>> GetRolesAsync(User user);

        Task<IList<Claim>> GetClaimsAsync(User user);

        Task<IdentityResult> AddClaimAsync(User user, Claim claim);

        Task<IdentityResult> RemoveClaimAsync(User user, Claim claim);

        IQueryable<User> Users { get; }

        Task<IdentityResult> DeleteUserAsync(User user);

        Task<IdentityResult> UpdateUserAsync(User user);

        Task<IdentityResult> SetUserNameAsync(User user, string username);

        Task<IList<User>> GetUsersInRoleAsync(string roleName);
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

        public async Task<IdentityResult> DeleteUserAsync(User user)
        {
            return await _manager.DeleteAsync(user);
        }

        public async Task<IdentityResult> CreateAsync(User user, string password)
        {
            return await _manager.CreateAsync(user, password);
        }

        public async Task<IList<string>> GetRolesAsync(User user)
        {
            return await _manager.GetRolesAsync(user);
        }

        public async Task<IdentityResult> RemoveRoleFromUser(User user, string role)
        {
            return await _manager.RemoveFromRoleAsync(user, role);
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

        public async Task<IdentityResult> SetUserNameAsync(User user, string username)
        {
            return await _manager.SetUserNameAsync(user, username);
        }

        public async Task<IList<User>> GetUsersInRoleAsync(string roleName)
        {
            return await _manager.GetUsersInRoleAsync(roleName);
        }

        public async Task<IList<Claim>> GetClaimsAsync(User user)
        {
            return await _manager.GetClaimsAsync(user);
        }

        public async Task<IdentityResult> AddClaimAsync(User user, Claim claim)
        {
            return await _manager.AddClaimAsync(user, claim);
        }

        public async Task<IdentityResult> RemoveClaimAsync(User user, Claim claim)
        {
            return await _manager.RemoveClaimAsync(user, claim);
        }

        public IQueryable<User> Users => _manager.Users;
    }

    #endregion DefaultUserManager
}

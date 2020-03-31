using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Helper;
using ModelLayer.Models;

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
    }

    public class UserService : IUserService
    {
        private readonly IUserManager _userManager;

        //TODO: fix it with di
        private readonly IMailProvider mailProvider;

        public IQueryable<User> Users => _userManager.Users;

        public UserService(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> CreateCRohMUserAsync(User user)
        {
            PasswordGenerator pwGenerator = new PasswordGenerator();
            var password = pwGenerator.Generate();

            user.UserName = await GetUniqueUserNameAsync(user.FirstName, user.LastName);

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                //TODO: send mail to created user
                Console.WriteLine("Mail send to user");
                Console.WriteLine($"password is : {password}");
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

        public IQueryable<User> Users => _manager.Users;
    }

    #endregion DefaultUserManager
}
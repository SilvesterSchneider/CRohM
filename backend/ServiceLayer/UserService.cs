using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelLayer.Models;

namespace ServiceLayer
{
    public class UserService : UserManager<User>
    {
        public UserService(IUserStore<User> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<User> passwordHasher,
            IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        /// <summary>
        /// Get the unique username
        /// </summary>
        /// <param name="firstName">the first name</param>
        /// <param name="lastName">the last name</param>
        /// <returns>the created unique username</returns>
        public string GetUniqueUserName(string firstName, string lastName)
        {
            return lastName + firstName.Substring(0, 2) + Users.Count() + 1;
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelLayer.Helper;
using ModelLayer.Models;

namespace ServiceLayer
{
    public class UserService : UserManager<User>
    {
        private IMailProvider mailProvider;

        public UserService(IUserStore<User> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<User> passwordHasher,
            IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger, IMailProvider mailProvider) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            this.mailProvider = mailProvider;
        }

        /// <summary>
        /// Change the password service.
        /// </summary>
        /// <param name="primKey">the primary key of the user to be changed</param>
        /// <returns></returns>
        public async Task ChangePasswordForUser(int primKey)
        {
            User userToBeUpdated = null;
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
                await ChangePasswordAsync(userToBeUpdated, userToBeUpdated.PasswordHash, newPassword).ConfigureAwait(false);
                mailProvider.SendMailContainingNewPasswort(newPassword, userToBeUpdated.Email);
            }
        }
    }
}
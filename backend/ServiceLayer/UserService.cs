using System;
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
        private readonly int MIN_LENGTH = 6;
        private readonly int MAX_LENGTH = 10;
        private readonly int MIN_LOW = 2;
        private readonly int MIN_UPPER = 2;
        private readonly int MIN_NUM = 1;
        private readonly int MIN_SPEC = 1;
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

        public async Task ChangePasswordForUser(int id)
        {
            User userToBeUpdated = null;
            foreach (User us in Users)
            {
                if (us.Id == id)
                {
                    userToBeUpdated = us;
                    break;
                }
            }
            if (userToBeUpdated != null && !string.IsNullOrEmpty(userToBeUpdated.Email))
            {
                string newPassword = new PasswordGenerator(MIN_LENGTH, MAX_LENGTH, MIN_LOW, MIN_UPPER, MIN_NUM, MIN_SPEC).Generate();
                await ChangePasswordAsync(userToBeUpdated, userToBeUpdated.PasswordHash, newPassword).ConfigureAwait(false);
                mailProvider.SendMailContainingNewPasswort(newPassword, userToBeUpdated.Email);
            }
        }
    }
}
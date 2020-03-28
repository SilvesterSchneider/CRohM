using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ModelLayer.Models;
using WebApi.Helper;

namespace ServiceLayer
{
    public interface ISignInService
    {
        /// <summary>
        /// Create a jwt for authorize via http header
        /// </summary>
        /// <param name="user">Create token for this user</param>
        string CreateToken(User user);

        Task<bool> CanSignInAsync(User user);

        Task<SignInResult> CheckPasswordSignInAsync(User user, string password, bool lockoutOnFailure);

        AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl, string userId);

        Task<ClaimsPrincipal> CreateUserPrincipalAsync(User user);

        Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent);

        Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent, bool bypassTwoFactor);

        Task ForgetTwoFactorClientAsync();

        Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync();

        Task<ExternalLoginInfo> GetExternalLoginInfoAsync(string expectedXsrf);

        Task<User> GetTwoFactorAuthenticationUserAsync();

        bool IsSignedIn(ClaimsPrincipal principal);

        Task<bool> IsTwoFactorClientRememberedAsync(User user);

        Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure);

        Task<SignInResult> PasswordSignInAsync(User user, string password, bool isPersistent, bool lockoutOnFailure);

        Task RefreshSignInAsync(User user);

        Task RememberTwoFactorClientAsync(User user);

        Task SignInAsync(User user, AuthenticationProperties authenticationProperties, string authenticationMethod);

        Task SignInAsync(User user, bool isPersistent, string authenticationMethod);

        Task SignInWithClaimsAsync(User user, AuthenticationProperties authenticationProperties, IEnumerable<Claim> additionalClaims);

        Task SignInWithClaimsAsync(User user, bool isPersistent, IEnumerable<Claim> additionalClaims);

        Task SignOutAsync();

        Task<SignInResult> TwoFactorAuthenticatorSignInAsync(string code, bool isPersistent, bool rememberClient);

        Task<SignInResult> TwoFactorRecoveryCodeSignInAsync(string recoveryCode);

        Task<SignInResult> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberClient);

        Task<IdentityResult> UpdateExternalAuthenticationTokensAsync(ExternalLoginInfo externalLogin);

        Task<User> ValidateSecurityStampAsync(ClaimsPrincipal principal);

        Task<bool> ValidateSecurityStampAsync(User user, string securityStamp);

        Task<User> ValidateTwoFactorSecurityStampAsync(ClaimsPrincipal principal);

        IUserClaimsPrincipalFactory<User> ClaimsFactory { get; set; }
        HttpContext Context { get; set; }
        ILogger Logger { get; set; }
        IdentityOptions Options { get; set; }
        UserManager<User> UserManager { get; set; }
    }

    public class SignInService : SignInManager<User>, ISignInService
    {
        private readonly AppSettings _appSettings;

        // had to include dll because fail from microsoft
        // https://www.gitmemory.com/issue/aspnet/AspNetCore/12536/515210764
        public SignInService(UserManager<User> userService, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<User> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<User>> logger, IAuthenticationSchemeProvider schemes, IUserConfirmation<User> confirmation, IOptions<AppSettings> appSettings) :
            base(userService, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
            _appSettings = appSettings.Value;
        }

        /// <summary>
        /// Create a jwt for authorize via http header
        /// </summary>
        /// <param name="user">Create token for this user</param>
        public string CreateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.JwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
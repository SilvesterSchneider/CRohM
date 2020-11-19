using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ModelLayer.Helper;
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
        string CreateToken(User user, List<string> roles, IList<Claim> claims);

        Task<SignInResult> PasswordSignInAsync(User user, string password);

        Task<SignInResult> CheckPasswordSignInAsync(User user, string password);
    }

    public class SignInService : ISignInService
    {
        private readonly AppSettings _appSettings;
        private readonly ISignInManager _signInManager;

        // had to include dll because fail from microsoft
        // https://www.gitmemory.com/issue/aspnet/AspNetCore/12536/515210764
        public SignInService(IOptions<AppSettings> appSettings, ISignInManager signInManager)

        {
            _signInManager = signInManager;
            _appSettings = appSettings.Value;
        }

        /// <summary>
        /// Create a jwt for authorize via http header
        /// </summary>
        /// <param name="user">Create token for this user</param>
        public string CreateToken(User user, List<string> roles, IList<Claim> claims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.JwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim("Id", user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            foreach (string role in roles)
            {
                tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, role));
            }
            foreach (Claim claim in claims)
            {
                tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, claim.Type));
            }

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [Obsolete("do not use this anymore, this function uses cookies we do not need")]
        public async Task<SignInResult> PasswordSignInAsync(User user, string password)
        {
            return await _signInManager.PasswordSignInAsync(user, password);
        }

        public async Task<SignInResult> CheckPasswordSignInAsync(User user, string password)
        {
            return await _signInManager.CheckPasswordSignInAsync(user, password);
        }
    }

    #region DefaultSignInManager

    public interface ISignInManager
    {
        Task<SignInResult> PasswordSignInAsync(User user, string password);

        Task<SignInResult> CheckPasswordSignInAsync(User user, string password);
    }

    public class DefaultSignInManager : ISignInManager
    {
        private readonly SignInManager<User> _manager;

        public DefaultSignInManager(SignInManager<User> manager)
        {
            _manager = manager;
        }

        public async Task<SignInResult> PasswordSignInAsync(User user, string password)
        {
            if (user.Id == 1)
            {
                return await _manager.PasswordSignInAsync(user, password, false, false);
            }
            else
            {
                return await _manager.PasswordSignInAsync(user, password, false, true);
            }
        }

        public async Task<SignInResult> CheckPasswordSignInAsync(User user, string password)
        {
            if (user.Id == 1)
            {
                return await _manager.CheckPasswordSignInAsync(user, password, false);
            }
            else
            {
                return await _manager.CheckPasswordSignInAsync(user, password, true);
            }
        }

        #endregion DefaultSignInManager
    }
}

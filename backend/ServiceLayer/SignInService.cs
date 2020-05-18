using System;
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
        string CreateToken(User user);

        Task<SignInResult> PasswordSignInAsync(User user, string password);
    }

    public class SignInService : ISignInService
    {
        private readonly AppSettings _appSettings;
        private readonly ISignInManager _signInManager;
        private User loggedInUser;

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

        public async Task<SignInResult> PasswordSignInAsync(User user, string password)
        {
            return await _signInManager.PasswordSignInAsync(user, password);
        }
    }

    #region DefaultSignInManager

    public interface ISignInManager
    {
        Task<SignInResult> PasswordSignInAsync(User user, string password);
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
            return await _manager.PasswordSignInAsync(user, password, false, true);
        }
    }

    #endregion DefaultSignInManager
}
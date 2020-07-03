using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DataTransferObjects;
using ModelLayer.Helper;
using NSwag.Annotations;
using ServiceLayer;

namespace WebApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ISignInService _signInService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public AuthController(
            ISignInService signInService,
            IUserService userService,
            IMapper mapper)
        {
            _signInService = signInService;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost]
        [AllowAnonymous]
        [SwaggerResponse(HttpStatusCode.OK, typeof(UserDto), Description = "Login erfolgreich")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(string))]
        public async Task<IActionResult> Login([FromBody] CredentialsDto credentials)
        {
            var user = await _userService.FindByNameAsync(credentials.UserNameOrEmail);
            if (user == null)
            {
                user = await _userService.FindByEmailAsync(credentials.UserNameOrEmail);
                if (user == null)
                {
                    return BadRequest("Login fehlgeschlagen!");
                }
            }

            if (user.UserLockEnabled)
            {
                return BadRequest("Benutzer ist gesperrt! Bitte den Administrator kontaktieren");
            } 

            var signInAsync = await _signInService.PasswordSignInAsync(user, credentials.Password);

            if (signInAsync.Succeeded)
            {
                var userDto = _mapper.Map<UserDto>(user);
                userDto.AccessToken = _signInService.CreateToken(user);
                return Ok(userDto);
            }

            return BadRequest("Login fehlgeschlagen!");
        }

        /// <summary>
        /// The password change controler request.
        /// </summary>
        /// <param name="primKey">the primary key of the user to be changed</param>
        /// <returns></returns>
        [Route("changePassword")]
        [HttpPut]
        [SwaggerResponse(HttpStatusCode.OK, typeof(bool), Description = "successfully updated")]
        public async Task<IActionResult> ChangePassword([FromQuery]int primKey)
        {
            await _userService.ChangePasswordForUser(primKey).ConfigureAwait(false);
            return Ok(true);
        }

        /// <summary>
        /// The password change controler request.
        /// </summary>
        /// <param name="id">the primary key of the user to be changed</param>
        /// <param name="newPassword">the new password to be changed</param>
        /// <returns></returns>
        [Route("updatePassword")]
        [HttpPut("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(bool), Description = "successfully updated")]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(string), Description = "conflict in update process!")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(void), Description = "user not found!")]
        public async Task<IActionResult> UpdatePassword([FromQuery]long id, [FromQuery]String newPassword)
        {
            if (!PasswordGuidelines.IsPasswordWithinRestrictions(newPassword))
            {
                string text = "Passwort sollte eine Länge haben von " + PasswordGuidelines.RequiredMinLength + " bis zu " + PasswordGuidelines.MaxLength + " Zeichen" +
                    (PasswordGuidelines.RequireDigit ? ", eine Anzahl von " + PasswordGuidelines.GetAmountOfNumerics() + " Zahlen" : "") + 
                    (PasswordGuidelines.RequireNonAlphanumeric ? ", eine Anzahl von " + PasswordGuidelines.GetAmountOfSpecialChars() + " Sonderzeichen" : "") +
                    (PasswordGuidelines.RequireLowercase ? ", eine Anzahl von " + PasswordGuidelines.GetAmountOfLowerLetters() + " kleinen Buchstaben" : "") +
                    (PasswordGuidelines.RequireUppercase ? ", eine Anzahl von " + PasswordGuidelines.GetAmountOfUpperLetters() + " großen Buchstaben." : "");
                return Conflict(text);
            }
            IdentityResult result = await _userService.ChangePasswordForUserAsync(id, newPassword).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return Ok(true);
            } 
            else 
            {
                return NotFound();
            }          
        }

        //TODO: implement endpoint for login with refresh token
        /*    [HttpPost("refreshLogin")]
                [AllowAnonymous]
                [SwaggerResponse(HttpStatusCode.OK, typeof(), Description = "successful login")]
                [SwaggerResponse(HttpStatusCode.BadRequest, typeof(void), Description = "not successful login")]
                public async Task<IActionResult> Login([FromBody] string refreshToken)
                {
                    return BadRequest();
                }*/
    }
}
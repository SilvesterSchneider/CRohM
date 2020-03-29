﻿using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DataTransferObjects;
using ModelLayer.Models;
using NSwag.Annotations;
using ServiceLayer;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

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
        [SwaggerResponse(HttpStatusCode.OK, typeof(UserDto), Description = "successful login")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(void), Description = "not successful login")]
        public async Task<IActionResult> Login([FromBody] CredentialsDto credentials)
        {
            var user = await _userService.FindByNameAsync(credentials.Name);
            if (user == null)
            {
                return BadRequest();
            }

            // TODO: clarify if lockoutOnFailure should be enabled
            // isPersistent = false because we use jwt, no sessions
            SignInResult signInAsync =
                await _signInService.PasswordSignInAsync(credentials.Name, credentials.Password, false, false);

            if (signInAsync.Succeeded)
            {
                var userDto = _mapper.Map<UserDto>(user);
                userDto.AccessToken = _signInService.CreateToken(user);
                return Ok(userDto);
            }

            return BadRequest();
        }

        /// <summary>
        /// The password change controler request.
        /// </summary>
        /// <param name="primKey">the primary key of the user to be changed</param>
        /// <returns></returns>
        [Route("api/auth/changePassword")]
        [HttpPut]
        [SwaggerResponse(HttpStatusCode.OK, typeof(bool), Description = "successfully updated")]
        public async Task<IActionResult> ChangePassword([FromQuery]int primKey)
        {
            await _userService.ChangePasswordForUser(primKey).ConfigureAwait(false);
            return Ok(true);
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
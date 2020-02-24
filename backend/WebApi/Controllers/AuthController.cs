using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelLayer;
using ModelLayer.DataTransferObjects;
using ModelLayer.Models;
using NSwag.Annotations;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace WebApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;

        public AuthController(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpPost]
        [AllowAnonymous]
        [SwaggerResponse(HttpStatusCode.OK, typeof(bool), Description = "successful login")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(void), Description = "not successful login")]
        public async Task<IActionResult> Login([FromBody]CredentialsDto credentials)
        {
            //TODO mit michelle klären ob cookie oder jwt
            //TODO default benutzer admin admin erstellen
            SignInResult signInAsync = await _signInManager.PasswordSignInAsync(credentials.Name, credentials.Password, false, false);

            if (signInAsync.Succeeded)
            {
                return Ok();
            }

            return Unauthorized();
        }
    }
}
using System.Net;
using System.Threading.Tasks;
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
        private readonly SignInService _signInService;

        public AuthController(SignInService signInService)
        {
            _signInService = signInService;
        }

        [HttpPost]
        [AllowAnonymous]
        [SwaggerResponse(HttpStatusCode.OK, typeof(string), Description = "successful login")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, typeof(void), Description = "not successful login")]
        public async Task<IActionResult> Login([FromBody]CredentialsDto credentials)
        {
            // isPersistent = false because we use jwt, no sessions
            // TODO: clarify if lockoutOnFailure should be enabled
            SignInResult signInAsync = await _signInService.PasswordSignInAsync(credentials.Name, credentials.Password, false, false);

            if (signInAsync.Succeeded)
            {
                return Ok("jwt token");
            }

            return Unauthorized();
        }
    }
}
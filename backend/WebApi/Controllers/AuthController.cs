using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DataTransferObjects;
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
        [SwaggerResponse(HttpStatusCode.OK, typeof(UserDto), Description = "successful login")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(void), Description = "not successful login")]
        public async Task<IActionResult> Login([FromBody] CredentialsDto credentials)
        {
            var user = await _userService.FindByNameAsync(credentials.UserNameOrEmail);
            if (user == null)
            {
                user = await _userService.FindByEmailAsync(credentials.UserNameOrEmail);
                if (user == null)
                {
                    return BadRequest();
                }
            }

            var signInAsync = await _signInService.PasswordSignInAsync(user, credentials.Password);

            if (signInAsync.Succeeded)
            {
                var userDto = _mapper.Map<UserDto>(user);
                userDto.AccessToken = _signInService.CreateToken(user);
                return Ok(userDto);
            }

            return BadRequest();
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
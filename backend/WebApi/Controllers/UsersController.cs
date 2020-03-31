using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DataTransferObjects;
using ModelLayer.Models;
using NSwag.Annotations;
using ServiceLayer;

namespace WebApi.Controllers
{
    //TODO: add role access control
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private static IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost]
        [SwaggerResponse(HttpStatusCode.Created, typeof(UserDto), Description = "successfully created")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(void), Description = "unsuccessfully request")]
        public async Task<IActionResult> Post(UserCreateDto userToCreate)
        {
            var user = _mapper.Map<User>(userToCreate);

            var result = await _userService.CreateCRohMUserAsync(user);

            if (result.Succeeded)
            {
                var userDto = _mapper.Map<UserDto>(user);
                var uri = $"https://{Request.Host}{Request.Path}/{userDto.Id}";
                return Created(uri, userDto);
            }

            return BadRequest();
        }
    }
}
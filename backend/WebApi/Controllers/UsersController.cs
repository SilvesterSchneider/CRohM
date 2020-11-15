using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DataTransferObjects;
using ModelLayer.Helper;
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

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<UserDto>), Description = "successfully found")]
        public async Task<IActionResult> Get()
        {
            List<User> users = await _userService.GetAsync();

            return Ok(_mapper.Map<List<UserDto>>(users));
        }

        [Authorize(Roles = "Anlegen eines Benutzers")]
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

        // TODO: Check passt, die Berechtigung hier?
        [Authorize(Roles = "Zuweisung einer neuen Rolle zu einem Benutzer")]
        [HttpPut]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully updated")]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(void), Description = "conflict in update process")]
        public async Task<IActionResult> Put(UserDto userToUpdate)
        {
            var user = _mapper.Map<User>(userToUpdate);
            var result = await _userService.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok();
            }

            return Conflict();
        }

        // put updates user with id {id} via frontend
        [Authorize(Roles = "Löschen / Deaktivieren eines Benutzers")]
        [HttpPut("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully updated")]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(void), Description = "conflict in update process")]
        public async Task<IActionResult> UpdateLockoutState(long id)
        {
            if (id != 1)
            {
                var result = await _userService.SetUserLockedAsync(id);
                if (result.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    return Conflict();
                }
            }
            else
            {
                return Ok();
            }
        }

        [Authorize(Roles = "Einsehen und Überarbeiten des Rollenkonzepts")]
        [HttpGet("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<string>), Description = "successfully found")]
        public async Task<IActionResult> GetAllRolesForUser(long id)
        {
            User user = await _userService.GetByIdAsync(id);
            List<string> roles = new List<string>();
            if (user != null)
            {
                roles = await _userService.GetRolesAsync(user);
            }
            return Ok(roles);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DataTransferObjects;
using ModelLayer.Models;
using NSwag.Annotations;
using ServiceLayer;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IMapper _mapper;
        private IRoleService roleService;
        private IUserService userService;

        public RoleController(IMapper _mapper, IRoleService roleService, IUserService userService)
        {
            this._mapper = _mapper;
            this.userService = userService;
            this.roleService = roleService;
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<RoleDto>), Description = "successfully found")]
        public async Task<IActionResult> Get()
        {
            List<RoleDto> roles = await roleService.GetAllRolesWithAllClaimsAsync();
            return Ok(roles);
        }

        [HttpPost]
        [SwaggerResponse(HttpStatusCode.Created, typeof(RoleDto), Description = "successfully created")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(void), Description = "unsuccessfully request")]
        public async Task<IActionResult> Post(RoleDto roleToCreate)
        {
            var result = await roleService.CreateWithClaimsAsync(roleToCreate);

            if (result.Succeeded)
            {
                var uri = $"https://{Request.Host}{Request.Path}/{roleToCreate.Id}";
                return Created(uri, roleToCreate);
            }

            return BadRequest();
        }

        [HttpPut]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully updated")]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(void), Description = "conflict in update process")]
        public async Task<IActionResult> Put(RoleDto roleToUpdate)
        {
            var resut = await roleService.UpdateRoleWithClaimsAsync(roleToUpdate);
            if (resut == IdentityResult.Success)
            {
                return Ok();
            }
            else
            {
                return Conflict();
            }
        }

        [HttpPut("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully updated")]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(void), Description = "conflict in update process")]
        public async Task<IActionResult> ChangeUserRoles(long userId, [FromBody] List<string> rolesToUpdate)
        {
            var resut = await userService.UpdateAllRolesAsync(userId, rolesToUpdate);
            if (resut == IdentityResult.Success)
            {
                return Ok();
            }
            else
            {
                return Conflict();
            }
        }
    }
}

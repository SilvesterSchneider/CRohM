using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DataTransferObjects;
using ModelLayer.Helper;
using ModelLayer.Models;
using NSwag.Annotations;
using ServiceLayer;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private IRoleService roleService;
        private IUserService userService;

        public RoleController(IRoleService roleService, IUserService userService)
        {
            this.userService = userService;
            this.roleService = roleService;
        }
        [ClaimsAuthorization(ClaimType = "Einsehen und Überarbeiten des Rollenkonzepts",
                            ClaimValue = "Einsehen und Überarbeiten des Rollenkonzepts")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<RoleDto>), Description = "successfully found")]
        public async Task<IActionResult> Get()
        {
            List<RoleDto> roles = await roleService.GetAllRolesWithAllClaimsAsync();
            return Ok(roles);
        }
        [ClaimsAuthorization(ClaimType = "Einsehen und Überarbeiten des Rollenkonzepts",
                            ClaimValue = "Einsehen und Überarbeiten des Rollenkonzepts")]
        [HttpGet("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<string>), Description = "successfully found")]
        public async Task<IActionResult> GetAllClaims(long id)
        {
            List<string> list = new List<string>();
            foreach (Claim claim in await Task.FromResult(RoleClaims.GetAllClaims()))
            {
                list.Add(claim.Value);
            }
            return Ok(list);
        }
        [ClaimsAuthorization(ClaimType = "Einsehen und Überarbeiten des Rollenkonzepts",
                            ClaimValue = "Einsehen und Überarbeiten des Rollenkonzepts")]
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
        [ClaimsAuthorization(ClaimType = "Einsehen und Überarbeiten des Rollenkonzepts",
                            ClaimValue = "Einsehen und Überarbeiten des Rollenkonzepts")]
        [HttpPut]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully updated")]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(void), Description = "conflict in update process")]
        public async Task<IActionResult> Put(RoleDto roleToUpdate)
        {
            if (roleToUpdate.Name.Equals(RoleClaims.ADMIN_GROUP))
            {
                return Ok();
            }
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
        [ClaimsAuthorization(ClaimType = "Einsehen und Überarbeiten des Rollenkonzepts",
                            ClaimValue = "Einsehen und Überarbeiten des Rollenkonzepts")]
        [HttpPut("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully updated")]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(void), Description = "conflict in update process")]
        public async Task<IActionResult> ChangeUserRoles(long id, [FromBody] List<string> rolesToUpdate)
        {
            var resut = await userService.UpdateAllRolesAsync(id, rolesToUpdate);
            if (resut == IdentityResult.Success)
            {
                return Ok();
            }
            else
            {
                return Conflict();
            }
        }
        [ClaimsAuthorization(ClaimType = "Einsehen und Überarbeiten des Rollenkonzepts",
                            ClaimValue = "Einsehen und Überarbeiten des Rollenkonzepts")]
        [HttpDelete("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully deleted")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(void), Description = "address not found")]
        public async Task<IActionResult> Delete(long id)
        {
            if (id == 1)
            {
                return Ok(); ;
            }
            Role roleToDelete = await roleService.FindRoleByIdAsync(id);
            if (roleToDelete == null)
            {
                return NotFound();
            }
            IdentityResult result = await roleService.DeleteAsync(roleToDelete);
            if (result == IdentityResult.Success)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
    }
}

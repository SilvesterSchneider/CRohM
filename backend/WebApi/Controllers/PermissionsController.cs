using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer;
using ModelLayer.DataTransferObjects;
using NSwag.Annotations;
using ServiceLayer;
using Microsoft.AspNetCore.Authorization;
using ModelLayer.Models;
using Microsoft.AspNetCore.Identity;

namespace WebApi.Controllers
{ 
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class PermissionsController : ControllerBase
    {
        
        IPermissionGroupService permission;
        IUserService userservice;
        IMapper mapper;

        public PermissionsController(IPermissionGroupService permissionGroupService, IUserService userService, IMapper mapper) {
            this.permission = permissionGroupService;
            this.userservice = userService;
            this.mapper = mapper;
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<PermissionGroupDto>), Description = "successfully request")]
        public async Task<IActionResult> GetAllPermissionGroups()
        {
            List<PermissionGroup> list = await permission.GetAllPermissionGroupAsync();
            return Ok(mapper.Map<List<PermissionGroupDto>>(list));
        }

        [HttpDelete("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully request")]
        public async Task<IActionResult> DeletePermissionGroup(long id)
        {
            if (id != 1)
            {
                await permission.DeleteAsyncWithAllDependencies(id);
                return Ok();
            }
            return Ok("Der Admin darf nicht gelöscht oder bearbeitet werden.");
        }

        [HttpPut]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully request")]
        public async Task<IActionResult> UpdatePermissionGroup([FromBody] PermissionGroupDto group)
        {
            if(group.id != 1)
            {
                await permission.UpdatePermissionGroupByIdAsync(mapper.Map<PermissionGroup>(group));
                return Ok();
            }

            return Ok("Der Admin darf nicht gelöscht oder bearbeitet werden.");
        }


        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully request")]
        public async Task<IActionResult> CreatePermissionGroup([FromBody] PermissionGroupDto group)
        {
                await permission.CreatePermissionGroupAsync(mapper.Map<PermissionGroup>(group));
                return Ok();
        }
    }
}
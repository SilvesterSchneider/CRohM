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
            await permission.DeleteAsyncWithAllDependencies(id);
            return Ok();
        }

        [HttpPut]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully request")]
        public async Task<IActionResult> CreatOrModifyGroup([FromBody] PermissionGroupDto group)
        {
            if(group.id != 1)
            {
                PermissionGroupDto newGroup = new PermissionGroupDto();
                newGroup.id = group.id;
                newGroup.Name = group.Name;
                group.Permissions.ForEach(x => {
                    if (x.IsEnabled)
                    {
                        PermissionDto permission = newGroup.Permissions.FirstOrDefault(y => y.Grant.Equals(x.Grant));
                        if (permission != null)
                        {
                            permission.IsEnabled = true;
                        }
                    }
                });
                await permission.CreateOrModifyPermissionGroupByIdAsync(mapper.Map<PermissionGroup>(newGroup));
                return Ok();
            }

            return Ok("Der Admin darf nicht gelöscht oder bearbeitet werden.");
        }
        
        /*
        [HttpGet("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<PermissionGroupDto>), Description = "successfully request")]
        public async Task<IActionResult> GetPermissionGroupsByUserId(long id)
        {
            List<PermissionGroup> list = await userservice.GetPermissionGroupsByUserIdAsync(id);
            return Ok(mapper.Map<List<PermissionGroupDto>>(list));
        }

        [HttpPut]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully request")]
        public async Task<IActionResult> SetUserPermissionsById([FromBody] List<int> groups, long id)
        {
            List<PermissionGroupDto> newGroups = new List<PermissionGroupDto>();

            foreach (int group in groups) {
                PermissionGroupDto newGroup = new PermissionGroupDto();
   
              
                newGroups.Add(newGroup);
            }

            if (await userservice.SetPermissionGroupsByUserIdAsync(mapper.Map<List<PermissionGroup>>(newGroups), id)) {

                return Ok();
            };

            return Forbid();
            
        }
        */
    }
}
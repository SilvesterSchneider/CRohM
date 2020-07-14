using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using AutoMapper;
using NSwag.Annotations;
using ServiceLayer;


namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserPermissionsController : ControllerBase
    {
        private static IUserService _userService;
        private readonly IMapper _mapper;

        public UserPermissionsController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }



        [HttpDelete("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully request")]
        public async Task<IActionResult> DeletePermissionsByUserIdAsync(int permissiongroupid, long id)
        {
            if (await _userService.DeletePermissionGroupByUserIdAsync(permissiongroupid, id))
            {
                return Ok();
            }

            return Ok("Es muss mindestens ein Admin vorhanden sein.");
        }

        [HttpPut("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully request")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(void), Description = "Permission ID und UserID müssen größer 0 sein.")]
        public async Task<IActionResult> AddPermissionsByUserIdAsync(int permissiongroupid, long id)
        {
            if (permissiongroupid > 0 && id > 0){
                if (await _userService.AddPermissionGroupByUserIdAsync(permissiongroupid, id))
                {
                    return Ok();
                }
                return Ok("Die Permissiongroup konnte nicht hinzugefügt werden.");
            }
            return BadRequest("PermissionID und UserID müssen größer 0 sein.");
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models;
using NSwag.Annotations;
using ServiceLayer;
using System;
using System.Net;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TestController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IUserCheckDateService userCheckDateService;

        public TestController(IUserService userService, IUserCheckDateService userCheckDateService)
        {
            this.userService = userService;
            this.userCheckDateService = userCheckDateService;
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successful")]
        public async Task<IActionResult> Test()
        {
            await Task.Delay(1);
            return Ok();
        }

        [HttpDelete("inactiveuser")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully deleted")]
        public async Task<IActionResult> DeleteInactiveUser()
        {
            await userCheckDateService.CheckAllUsersAsync();
            return Ok();
        }

        [HttpPost("user/{id}/lastLoginDate/{lastLoginDate}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully deleted")]
        public async Task<IActionResult> SetLastLoginDate([FromRoute] long id, [FromRoute] String lastLoginDate)
        {
            User user = await userService.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            DateTime lastDate;
            if (!DateTime.TryParse(lastLoginDate, out lastDate))
            {
                return NotFound();
            }

            user.LastLoginDate = lastDate;
            await userService.UpdateUserAsync(user);
            return Ok();
        }
    }
}

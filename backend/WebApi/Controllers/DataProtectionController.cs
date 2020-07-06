using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DataTransferObjects;
using NSwag.Annotations;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DataProtectionController : ControllerBase
    {
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully send message")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(void), Description = "not successful")]
        public async Task<IActionResult> SendMessage([FromBody]SendInfoDTO sendInoInfoDto)
        {
            if (ModelState.IsValid)
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpDelete]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully send message")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(void), Description = "not successful")]
        public async Task<IActionResult> SendDeleteMessage([FromBody]SendInfoDTO sendInoInfoDto)
        {
            if (ModelState.IsValid)
            {
                return Ok();
            }

            return BadRequest();
        }
    }

    public class SendInfoDTO
    {
        public ContactDto Delete { get; set; }
    }
}
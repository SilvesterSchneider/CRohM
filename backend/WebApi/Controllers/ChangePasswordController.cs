using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChangePasswordController : ControllerBase
    {
        private UserService _users;
        private readonly IMapper _mapper;

        public ChangePasswordController(UserService users, IMapper mapper)
        {
            this._users = users;
            this._mapper = mapper;
        }

        [HttpPut]
        [SwaggerResponse(HttpStatusCode.OK, typeof(bool), Description = "successfully updated")]
        public async Task<IActionResult> Change([FromQuery]int id)
        {
            await _users.ChangePasswordForUser(id).ConfigureAwait(false);
            return Ok(true);
        }
    }
}

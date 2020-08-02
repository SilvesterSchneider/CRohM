using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DataTransferObjects;
using ModelLayer.Models;
using NSwag.Annotations;
using ServiceLayer;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserLoginController : ControllerBase
    {
        private IMapper mapper;
        private IUserLoginService userLoginService;

        public UserLoginController(IMapper mapper, IUserLoginService userLoginService)
        {
            this.mapper = mapper;
            this.userLoginService = userLoginService;
        }

        [HttpGet("id")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(DateTime), Description = "successfully found")]
        public async Task<IActionResult> GetTheLastLoginTimeOfUserById([FromQuery] long id)
        {
            DateTime userLoginTime = await userLoginService.GetTheLastLoginDateTimeForUserByIdAsync(id);
            return Ok(userLoginTime);
        }
    }
}

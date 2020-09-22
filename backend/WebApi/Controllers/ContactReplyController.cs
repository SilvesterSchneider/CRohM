
using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DataTransferObjects;
using ModelLayer.Models;
using NSwag.Annotations;
using RepositoryLayer;
using ServiceLayer;
using System.Linq;
using ModelLayer.Helper;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactReplyController : ControllerBase
    {
        private readonly IContactReplyService contactReplyService;

        public ContactReplyController(IContactReplyService contactReplyService) {
            this.contactReplyService = contactReplyService;
        }

        [HttpPut("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully created")]
        public async Task<IActionResult> SendDisclosureById([FromRoute] long id)
        {
            await contactReplyService.ContactReply(id);
            return Ok();
        }
    }


}

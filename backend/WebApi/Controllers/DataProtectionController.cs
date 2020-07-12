using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DataTransferObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSwag.Annotations;
using ServiceLayer;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DataProtectionController : ControllerBase
    {
        private readonly IMailService _mailService;
        private readonly IDataProtectionService _dataProtectionService;

        public DataProtectionController(IMailService mailService, IDataProtectionService dataProtectionService)
        {
            _mailService = mailService;
            _dataProtectionService = dataProtectionService;
        }

        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully send message")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(void), Description = "not successful")]
        public async Task<IActionResult> SendUpdateMessage([FromBody]SendInfoDTO sendInoInfoDto)
        {
            if (ModelState.IsValid)
            {
                var emailData = _dataProtectionService.GetContactChangesForEmail(sendInoInfoDto.ContactChanges);

                if (sendInoInfoDto.Contact?.ContactPossibilities.Mail == null)
                {
                    return BadRequest("could not send message");
                }

                _mailService.SendDataProtectionUpdateMessage(sendInoInfoDto.Contact.Name, sendInoInfoDto.Contact.PreName, sendInoInfoDto.Contact.ContactPossibilities.Mail, emailData);

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
        public bool Delete { get; set; }
        public JObject ContactChanges { get; set; }
        public ContactDto Contact { get; set; }
    }
}
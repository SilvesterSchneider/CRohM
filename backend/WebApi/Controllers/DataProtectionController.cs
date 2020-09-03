using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DataTransferObjects;
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
        private readonly IPermissionGroupService _permissionGroupService;
        private readonly IDataProtectionService _dataProtectionService;

        public DataProtectionController(IMailService mailService,
            IDataProtectionService dataProtectionService,
            IPermissionGroupService permissionGroupService)
        {
            _mailService = mailService;
            _dataProtectionService = dataProtectionService;
            _permissionGroupService = permissionGroupService;
        }

        [HttpPost]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(string), Description = "not successful")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully send message")]
        public IActionResult SendUpdateMessage([FromBody]SendInfoDTO sendInoInfoDto)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(sendInoInfoDto.Contact?.ContactPossibilities?.Mail))
                {
                    return BadRequest("could not send message");
                }

                if (!sendInoInfoDto.Delete)
                {
                    var emailData = _dataProtectionService.GetContactChangesForEmail(sendInoInfoDto.ContactChanges);
                    if (!string.IsNullOrEmpty(emailData))
                        _mailService.SendDataProtectionUpdateMessage(sendInoInfoDto.Contact.Name, sendInoInfoDto.Contact.PreName, sendInoInfoDto.Contact.ContactPossibilities.Mail, emailData);
                }
                else
                {
                    var emailData = _dataProtectionService.GetContactDeletesForEmail();
                    if (!string.IsNullOrEmpty(emailData))
                        _mailService.SendDataProtectionDeleteMessage(sendInoInfoDto.Contact.Name, sendInoInfoDto.Contact.PreName, sendInoInfoDto.Contact.ContactPossibilities.Mail, emailData);
                }

                return Ok();
            }

            return BadRequest("");
        }

        [HttpGet("officer")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(bool), Description = "successful request")]
        public async Task<IActionResult> IsThereAnyDataProtectionOfficerInTheSystem()
        {
            bool isThereAnyDataProtectionOfficer = await _permissionGroupService.IsThereAnyDataProtectionOfficerAsync();

            return Ok(isThereAnyDataProtectionOfficer);
        }
    }

    public class SendInfoDTO
    {
        public bool Delete { get; set; }
        public JObject ContactChanges { get; set; }

        [Required]
        public ContactDto Contact { get; set; }
    }
}

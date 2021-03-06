using System.ComponentModel.DataAnnotations;
using System.Linq;
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

        private readonly IUserService _userService;
        private readonly IDataProtectionService _dataProtectionService;

        public DataProtectionController(IMailService mailService,
            IDataProtectionService dataProtectionService, IUserService userService)
        {
            _mailService = mailService;
            _dataProtectionService = dataProtectionService;
            _userService = userService;
        }

        [Authorize(Roles = "Mitteilung an einen Kontakt nach Löschung oder Änderung")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(string), Description = "not successful")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully send message")]
        public async Task<IActionResult> SendUpdateMessage([FromBody]SendInfoDTO sendInoInfoDto)
        {
            await Task.Yield();

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
        [SwaggerResponse(HttpStatusCode.OK, typeof(bool), Description = "protection officer found or not")]
        public async Task<IActionResult> IsThereAnyDataProtectionOfficerInTheSystem()
        {
            var users = await _userService.GetUsersInRoleAsync("Datenschutzbeauftragter");

            if (users.Any())
            {
                return Ok(true);
            }

            return Ok(false);
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

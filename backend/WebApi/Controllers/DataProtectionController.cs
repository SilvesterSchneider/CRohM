using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
                var bla = await _dataProtectionService.MakeSomethingAsync(sendInoInfoDto.Customer, new ContactDto() { Name = "hans" });
                _mailService.SendDataProtectionUpdateMessage("Herr Lord Superman", "Därbe", "silvester.schneider@gmail.com", bla);

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
        public JObject Customer { get; set; }
    }

    /*
     *{
          type: this.compareValues(obj1, obj2),
          data: obj1 === undefined ? obj2 : obj1
          }*

        public class ContactDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PreName { get; set; }
        public AddressDto Address { get; set; }
        public ContactPossibilitiesDto ContactPossibilities { get; set; }
        public List<OrganizationDto> Organizations { get; set; } = new List<OrganizationDto>();
        public List<EventDto> Events { get; set; } = new List<EventDto>();
        public List<HistoryElementDto> History { get; set; } = new List<HistoryElementDto>();
    }
     */
}
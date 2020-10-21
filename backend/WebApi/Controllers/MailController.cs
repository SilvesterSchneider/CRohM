using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DataTransferObjects;
using ModelLayer.Models;
using NSwag.Annotations;
using ServiceLayer;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IMapper _mapper;
        private IEventService eventService;

        public MailController(IMapper mapper, IEventService eventService)
        {
            this._mapper = mapper;
            this.eventService = eventService;
        }

        /// <summary>
        /// Eine Einladungsmail an kontakte senden
        /// </summary>
        /// <param name="mailContent">der mail inhalt</param>
        /// <param name="contactIds">die kontakt ids an die eine mail versendet werden soll</param>
        /// <returns></returns>
        [HttpPut]
        [SwaggerResponse(HttpStatusCode.OK, typeof(bool), Description = "successfully send mails")]
        public async Task<IActionResult> SendInvitationMails([FromBody]List<long> contactIds, [FromHeader]List<long> orgaIds, [FromQuery]string mailContent)
        {
            return Ok(await eventService.SendInvitationMailsAsync(contactIds, orgaIds, mailContent));
        }

        /// <summary>
        /// den standart text für die einladung holen.
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(string), Description = "successfully get mail text")]
        public async Task<IActionResult> GetSendInvitationText(string eventName, string date, string time)
        {
            string text = MailService.GetMailForInvitationAsTemplate(eventName, date, time);
            return await Task.FromResult(Ok(text));
        }
    }
}

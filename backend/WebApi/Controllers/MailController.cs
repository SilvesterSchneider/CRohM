using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DataTransferObjects;
using ModelLayer.Helper;
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
        private IMailService mailService;

        public MailController(IMapper mapper, IEventService eventService, IMailService mailService)
        {
            this._mapper = mapper;
            this.mailService = mailService;
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
        public async Task<IActionResult> SendInvitationMails([FromBody]List<long> contactIds, [FromQuery]string mailContent)
        {
            return Ok(await eventService.SendInvitationMailsAsync(contactIds, mailContent));
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

        /// <summary>
        /// die mail einstellungen holen
        /// <returns></returns>
        [HttpGet("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(MailCredentialsSerializableDto), Description = "successfully get mail data")]
        public async Task<IActionResult> GetEmailCredentials()
        {
            return await Task.FromResult(Ok(_mapper.Map<MailCredentialsSerializableDto>(new MailCredentialsSerializable(MailCredentialsHelper.GetMailCredentials()))));
        }

        /// <summary>
        /// Die mail einstellungen speichern.
        /// </summary>
        /// <param name="data">die einstellungen</param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully post mail data")]
        public async Task<IActionResult> SaveMailCredentials([FromBody] MailCredentialsSerializableDto data)
        {
            await Task.Run(() => MailCredentialsHelper.SaveMailCredentials(new MailCredentials(_mapper.Map<MailCredentialsSerializable>(data))));
            return Ok();
        }

        /// <summary>
        /// Eine freie mail senden
        /// </summary>
        /// <param name="subject">betreff</param>
        /// <param name="address">adresse des empfängers</param>
        /// <param name="mailContent">der mail inhalt</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(bool), Description = "successfully send mail")]
        public async Task<IActionResult> SendMail(string subject, string address, string mailContent)
        {
            return Ok(await mailService.SendMailToAddress(subject, address, mailContent));
        }
    }
}

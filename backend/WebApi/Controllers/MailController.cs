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
    public class MailController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserService userService;
        private readonly IModificationEntryService modService;
        private IEventService eventService;

        public MailController(IMapper mapper, IEventService eventService, IModificationEntryService modService, IUserService userService)
        {
            this._mapper = mapper;
            this.userService = userService;
            this.modService = modService;
            this.eventService = eventService;
        }

        /// <summary>
        /// Einen bestehenden event aktualisieren
        /// </summary>
        /// <param name="eventToModify">das zu bearbeitende event</param>
        /// <param name="id">die event id</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(EventDto), Description = "successfully send mails")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(void), Description = "bad request")]
        public async Task<IActionResult> SendInvitationMails([FromRoute]long id, [FromBody]List<long> contactIds, [FromQuery]string mailContent)
        {
            Event oldOne = await eventService.GetEventByIdWithAllIncludesAsync(id);
            if (oldOne == null)
            {
                return BadRequest();
            }

            User userOfChange = await userService.FindByNameAsync(User.Identity.Name);
            await modService.UpdateEventsAsync(oldOne.Contacts, userOfChange, oldOne.Id, oldOne.Participated, contactIds);
            if (await eventService.SendInvitationMailsAsync(oldOne, contactIds, mailContent))
            {
                await modService.CommitChanges();
            }
            EventDto eventToModify = _mapper.Map<EventDto>(oldOne);
            return Ok(eventToModify);
        }
    }
}

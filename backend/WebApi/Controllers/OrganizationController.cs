using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModelLayer.DataTransferObjects;
using ModelLayer.Models;
using NSwag.Annotations;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WebApi.Helper;
using WebApi.Wrapper;

namespace WebApi.Controllers
{
    [Route("api/organization")]
    [ApiController]
    [Authorize]
    public class OrganizationController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserService userService;
        private readonly IModificationEntryService modService;
        private readonly IOrganizationService _organizationService;
        private readonly ILogger _logger;
        private readonly IContactService contactService;
        private readonly IHistoryService historyService;
        private readonly IEventService eventService;

        public OrganizationController(
            IMapper mapper,
            IOrganizationService organizationService,
            IUserService userService,
            ILoggerFactory logger,
            IModificationEntryService modService,
            IContactService contactService,
            IHistoryService historyService,
            IEventService eventService)
        {
            _mapper = mapper;
            this.eventService = eventService;
            this.contactService = contactService;
            this.userService = userService;
            this.modService = modService;
            _organizationService = organizationService;
            _logger = logger.CreateLogger(nameof(OrganizationController));
            this.historyService = historyService;
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<OrganizationDto>), Description = "successfully found")]
        public async Task<IActionResult> Get()
        {
            var organizations = await _organizationService.GetAllOrganizationsWithIncludesAsync();
            var organizationsDto = _mapper.Map<List<OrganizationDto>>(organizations);

            return Ok(organizationsDto);
        }

        [HttpGet("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(OrganizationDto), Description = "successfully found")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(void), Description = "address not found")]
        public async Task<IActionResult> GetById(long id)
        {
            var organization = await _organizationService.GetByIdAsync(id);

            if (organization == null)
            {
                return NotFound();
            }

            var organizationDto = _mapper.Map<OrganizationDto>(organization);
            return Ok(organizationDto);
        }

        [HttpPut("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(OrganizationDto), Description = "successfully updated")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(void), Description = "bad request")]
        public async Task<IActionResult> Put([FromBody]OrganizationDto organization, [FromRoute]long id)
        {
            if (organization == null)
            {
                return BadRequest();
            }
            if (id != organization.Id)
            {
                return BadRequest();
            }
            User userOfChange = await userService.FindByNameAsync(User.Identity.Name);
            var mappedOrganization = _mapper.Map<Organization>(organization);
            await modService.UpdateOrganizationAsync(userOfChange, await _organizationService.GetByIdAsync(id), mappedOrganization, await userService.IsDataSecurityEngineer(userOfChange.Id));
            if (await _organizationService.UpdateAsyncWithAlleDependencies(mappedOrganization))
            {
                await modService.CommitChanges();
            }
            var organizationDto = _mapper.Map<OrganizationDto>(mappedOrganization);
            return Ok(organizationDto);
        }

        /// <summary>
        /// Add new contact to organization
        /// </summary>
        /// <param name="id">The id of the organization</param>
        /// <param name="contactId">The id of the contact which will be added</param>
        [HttpPut("{id}/addContact")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(OrganizationDto), Description = "successfully updated")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(void), Description = "bad request")]
        public async Task<IActionResult> AddContact([FromRoute]long id, [FromBody]long contactId)
        {
            Organization organization;
            try
            {
                organization = await _organizationService.AddContactAsync(id, contactId);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e.Message);
                return BadRequest();
            }

            var organizationDto = _mapper.Map<OrganizationDto>(organization);
            User userOfChange = await userService.FindByNameAsync(User.Identity.Name);
            string contactName = string.Empty;
            Contact contact = await contactService.GetByIdAsync(contactId);
            if (contact != null)
            {
                contactName = contact.PreName + " " + contact.Name;
            }
            await modService.ChangeEmployeesOfOrganization(id, contactName, false, userOfChange);
            return Ok(organizationDto);
        }

        /// <summary>
        /// Remove a contact from a organization
        /// </summary>
        /// <param name="id">The id of the organization</param>
        /// <param name="contactId">The id of the contact which will be removed</param>
        [HttpPut("{id}/removeContact")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully updated")]
        [SwaggerResponse(HttpStatusCode.BadRequest, typeof(void), Description = "bad request")]
        public async Task<IActionResult> RemoveContact([FromRoute]long id, [FromBody]long contactId)
        {
            try
            {
                await _organizationService.RemoveContactAsync(id, contactId);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e.Message);
                return BadRequest();
            }
            User userOfChange = await userService.FindByNameAsync(User.Identity.Name);
            string contactName = string.Empty;
            Contact contact = await contactService.GetByIdAsync(contactId);
            if (contact != null)
            {
                contactName = contact.PreName + " " + contact.Name;
            }
            await modService.ChangeEmployeesOfOrganization(id, contactName, true, userOfChange);
            return Ok();
        }

        [HttpPost]
        [SwaggerResponse(HttpStatusCode.Created, typeof(OrganizationDto), Description = "successfully created")]
        public async Task<IActionResult> Post([FromBody]OrganizationCreateDto organizationToCreate)
        {
            Organization organization = await _organizationService.CreateAsync(_mapper.Map<Organization>(organizationToCreate));

            var organizationDto = _mapper.Map<OrganizationDto>(organization);
            var uri = $"https://{Request.Host}{Request.Path}/{organizationDto.Id}";
            User userOfChange = await userService.FindByNameAsync(User.Identity.Name);
            await modService.CreateNewOrganizationEntryAsync(userOfChange, organization.Id);
            return Created(uri, organizationDto);
        }

        [HttpDelete("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully deleted")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(void), Description = "address not found")]
        public async Task<IActionResult> Delete(long id)
        {
            Organization organization = await _organizationService.GetByIdAsync(id);
            if (organization == null)
            {
                return NotFound();
            }
            List<HistoryElement> historyList = organization.History;
            for (int index = 0; index < historyList.Count; index++)
            {
                await historyService.DeleteAsync(historyList[index]);
            }

            await _organizationService.DeleteAsync(organization);
            await modService.UpdateOrganizationByDeletionAsync(id);
            return Ok();
        }

        // creates new contact in db via frontend
        [HttpPost("{id}/historyElement")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully created")]
        public async Task<IActionResult> PostHistoryElement([FromBody]HistoryElementCreateDto historyToCreate, [FromRoute]long id)
        {
            await _organizationService.AddHistoryElement(id, _mapper.Map<HistoryElement>(historyToCreate));
            User userOfChange = await userService.FindByNameAsync(User.Identity.Name);
            await modService.UpdateOrganizationByHistoryElementAsync(userOfChange, id, historyToCreate.Name + ":" + historyToCreate.Comment);
            return Ok();
        }

        [HttpGet("{id}/history")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(PagedResponse<List<object>>), Description = "successfully found")]
        public async Task<IActionResult> GetHistory(long id, [FromQuery] PaginationFilter filter)
        {
            PaginationFilter validFilter = new PaginationFilter(filter.PageStart, filter.PageSize);
            List<Event> events = await eventService.GetEventsForOrganization(id);
            List<HistoryElement> history = await historyService.GetHistoryByOrganisationAsync(id, validFilter.PageStart, validFilter.PageSize);
            List<object> mergedResult = events.Cast<object>().Concat(history.Cast<object>()).ToList();
            mergedResult.Sort((e1, e2) => DateTime.Compare(getDate(e2), getDate(e1)));
            List<object> pagination = mergedResult.Skip(validFilter.PageStart).Take(validFilter.PageSize).ToList();
            return Ok(new PagedResponse<List<object>>(pagination, validFilter.PageStart, validFilter.PageSize, mergedResult.Count));
        }

        private DateTime getDate(object element)
        {
            if (element is Event)
            {
                return ((Event)element).Date;
            }

            if (element is HistoryElement)
            {
                return ((HistoryElement)element).Date;
            }

            return DateTime.Now;
        }
    }
}

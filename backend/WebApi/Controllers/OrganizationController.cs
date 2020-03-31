using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DataTransferObjects;
using ModelLayer.Models;
using NSwag.Annotations;
using ServiceLayer;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/organization")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly IMapper _mapper;
        private IOrganizationService organizationService;

        public OrganizationController(IMapper mapper, IOrganizationService organizationService)
        {
            this._mapper = mapper;
            this.organizationService = organizationService;
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<OrganizationDto>), Description = "successfully found")]
        public async Task<IActionResult> Get()
        {
            var organizations = await organizationService.GetAsync();
            var organizationsDto = _mapper.Map<List<OrganizationDto>>(organizations);

            return Ok(organizationsDto);
        }

        [HttpGet("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(OrganizationDto), Description = "successfully found")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(void), Description = "address not found")]
        public async Task<IActionResult> GetById(long id)
        {
            var organization = await organizationService.GetByIdAsync(id);

            if (organization == null)
            {
                return NotFound();
            }

            var organizationDto = _mapper.Map<OrganizationDto>(organization);
            return Ok(organizationDto);
        }

        [HttpPut("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(OrganizationDto), Description = "successfully updated")]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(void), Description = "conflict in update process")]
        public async Task<IActionResult> Put(OrganizationDto organization)
        {
            var mappedOrganization = _mapper.Map<Organization>(organization);
            await organizationService.UpdateAsync(mappedOrganization);
            if (organization == null)
            {
                return Conflict();
            }
            var organizationDto = _mapper.Map<OrganizationDto>(mappedOrganization);
            return Ok(organizationDto);
        }

        [HttpPost]
        [SwaggerResponse(HttpStatusCode.Created, typeof(OrganizationDto), Description = "successfully created")]
        public async Task<IActionResult> Post(OrganizationCreateDto organizationToCreate)
        {
            Organization organization = await organizationService.CreateAsync(_mapper.Map<Organization>(organizationToCreate));

            var organizationDto = _mapper.Map<OrganizationDto>(organization);
            var uri = $"https://{Request.Host}{Request.Path}/{organizationDto.Id}";
            return Created(uri, organizationDto);
        }

        [HttpDelete("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully deleted")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(void), Description = "address not found")]
        public async Task<IActionResult> Delete(long id)
        {
            Organization organization = await organizationService.GetByIdAsync(id);
            if (organization == null)
            {
                return NotFound();
            }
            await organizationService.DeleteAsync(organization);
            return Ok();
        }
    }
}

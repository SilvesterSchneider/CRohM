using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DataTransferObjects;
using NSwag.Annotations;
using ServiceLayer;

namespace WebApi.Controllers
{
    [Route("api/contact")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IMapper _mapper;
        private IContactService contactService;

        public ContactController(IMapper mapper, IContactService contactService)
        {
            _mapper = mapper;
            this.contactService = contactService;
        }


        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<ContactDto>), Description = "successfully found")]
        public async Task<IActionResult> Get()
        {
            var organizations = await organizationService.GetAsync();
            var organizationsDto = _mapper.Map<List<OrganizationDto>>(organizations);

            return Ok(organizationsDto);
        }
    }
}
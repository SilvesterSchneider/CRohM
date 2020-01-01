using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DataTransferObjects;
using NSwag.Annotations;
using ServiceLayer;

namespace WebApi.Controllers
{
    //TODO: add role access control
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressesController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<AddressDto>), Description = "successfully found")]
        public async Task<IActionResult> Get()
        {
            var addresses = await _addressService.GetAsync();
            return Ok(addresses);
        }

        [HttpGet("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(AddressDto), Description = "successfully found")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(void), Description = "address not found")]
        public async Task<IActionResult> GetById(string id)
        {
            var address = await _addressService.GetByIdAsync(id);

            if (address == null)
            {
                return NotFound();
            }

            return Ok(address);
        }

        [HttpPut("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(AddressDto), Description = "successfully updated")]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(void), Description = "conflict in update process")]
        public async Task<IActionResult> Put(AddressDto address)
        {
            address = await _addressService.UpdateAsync(address);
            if (address == null)
            {
                return Conflict();
            }

            return Ok(address);
        }

        [HttpPost]
        [SwaggerResponse(HttpStatusCode.Created, typeof(AddressDto), Description = "successfully created")]
        public async Task<IActionResult> Post(AddressCreateDto addressToCreate)
        {
            AddressDto addressDto = await _addressService.CreateAsync(addressToCreate);
            if (addressDto == null)
            {
                //TODO: implement - maybe NotCreatedException with handler returning code 500 ?!
                throw new NotImplementedException();
            }

            var uri = $"https://{Request.Host}{Request.Path}/{addressDto.Id}";
            return Created(uri, addressDto);
        }

        [HttpDelete("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully deleted")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(void), Description = "address not found")]
        public async Task<IActionResult> Delete(string id)
        {
            bool okResult = await _addressService.DeleteAsync(id);
            if (!okResult)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}
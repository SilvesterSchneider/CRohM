using System;
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
    //TODO: add role access control
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressService _addressService;
        private readonly IMapper _mapper;

        public AddressesController(IAddressService addressService, IMapper mapper)
        {
            _addressService = addressService;
            _mapper = mapper;
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<AddressDto>), Description = "successfully found")]
        public async Task<IActionResult> Get()
        {
            var addresses = await _addressService.GetAsync();
            var addressDtos = _mapper.Map<List<AddressDto>>(addresses);
            return Ok(addressDtos);
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

            var addressDto = _mapper.Map<AddressDto>(address);
            return Ok(addressDto);
        }

        [HttpPut("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(AddressDto), Description = "successfully updated")]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(void), Description = "conflict in update process")]
        public async Task<IActionResult> Put(AddressDto address)
        {
            var mappedAddress = _mapper.Map<Address>(address);
            await _addressService.UpdateAsync(mappedAddress);
            if (address == null)
            {
                return Conflict();
            }
            var addressDto = _mapper.Map<AddressDto>(mappedAddress);
            return Ok(addressDto);
        }

        [HttpPost]
        [SwaggerResponse(HttpStatusCode.Created, typeof(AddressDto), Description = "successfully created")]
        public async Task<IActionResult> Post(AddressCreateDto addressToCreate)
        {
            Address address = await _addressService.CreateAsync(_mapper.Map<Address>(addressToCreate));
            if (address == null)
            {
                //TODO: implement - maybe NotCreatedException with handler returning code 500 ?!
                throw new NotImplementedException();
            }

            var uri = $"https://{Request.Host}{Request.Path}/{address.Id}";
            return Created(uri, address);
        }

        [HttpDelete("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void), Description = "successfully deleted")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(void), Description = "address not found")]
        public async Task<IActionResult> Delete(string id)
        {
            Address address = await _addressService.GetByIdAsync(id);
            if (address == null)
            {
                return NotFound();
            }
            await _addressService.DeleteAsync(address);
            return Ok();
        }
    }
}
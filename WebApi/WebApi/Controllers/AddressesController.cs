using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelLayer;
using ModelLayer.DataTransferObjects;
using ModelLayer.Models;
using ServiceLayer;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressesController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        // GET: api/Addresses
        [HttpGet]
        public async Task<ActionResult<List<AddressDto>>> GetAddresses()
        {
            var addresses = await _addressService.GetAsync();
            return addresses;
        }

        //// GET: api/Addresses/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Address>> GetAddress(string id)
        //{
        //    var address = await _context.Addresses.FindAsync(id);

        //    if (address == null)
        //    {
        //        return NotFound();
        //    }

        //    return address;
        //}

        //// PUT: api/Addresses/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for
        //// more details see https://aka.ms/RazorPagesCRUD.
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutAddress(string id, Address address)
        //{
        //    if (id != address.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(address).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!AddressExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/Addresses
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for
        //// more details see https://aka.ms/RazorPagesCRUD.
        //[HttpPost]
        //public async Task<ActionResult<Address>> PostAddress(Address address)
        //{
        //    _context.Addresses.Add(address);
        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (AddressExists(address.Id))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return CreatedAtAction("GetAddress", new { id = address.Id }, address);
        //}

        //// DELETE: api/Addresses/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<Address>> DeleteAddress(string id)
        //{
        //    var address = await _context.Addresses.FindAsync(id);
        //    if (address == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Addresses.Remove(address);
        //    await _context.SaveChangesAsync();

        //    return address;
        //}

        //private bool AddressExists(string id)
        //{
        //    return _context.Addresses.Any(e => e.Id == id);
        //}
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ModelLayer.DataTransferObjects;
using ModelLayer.Models;
using RepositoryLayer;

namespace ServiceLayer
{
    public interface IAddressService
    {
        public Task<List<AddressDto>> GetAsync();

        public Task<AddressDto> GetByIdAsync(string id);

        public Task<AddressDto> CreateAsync(AddressCreateDto addressToCreate);

        public Task<bool> DeleteAsync(string id);

        public Task<AddressDto> UpdateAsync(AddressDto address);
    }

    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IMapper _mapper;

        public AddressService(IAddressRepository addressRepository, IMapper mapper)
        {
            _addressRepository = addressRepository;
            _mapper = mapper;
        }

        public async Task<List<AddressDto>> GetAsync()
        {
            var addresses = await _addressRepository.GetAsync();

            return _mapper.Map<List<AddressDto>>(addresses);
        }

        public async Task<AddressDto> GetByIdAsync(string id)
        {
            var address = await _addressRepository.GetByIdAsync(id);

            return _mapper.Map<AddressDto>(address);
        }

        public async Task<AddressDto> CreateAsync(AddressCreateDto addressToCreate)
        {
            if (addressToCreate == null) return null;

            var address = await _addressRepository.CreateAsync(_mapper.Map<Address>(addressToCreate));

            return _mapper.Map<AddressDto>(address);
        }

        //TODO: improve return value - bool is not that descriptive
        public async Task<bool> DeleteAsync(string id)
        {
            var address = await _addressRepository.GetByIdAsync(id);

            if (address == null)
            {
                return false;
            }

            await _addressRepository.DeleteAsync(address);

            return true;
        }

        public async Task<AddressDto> UpdateAsync(AddressDto addressToUpdate)
        {
            if (addressToUpdate == null) return null;

            Address address;
            try
            {
                address = await _addressRepository.UpdateAsync(_mapper.Map<Address>(addressToUpdate));
            }
            catch (DbUpdateConcurrencyException e)
            {
                //TODO: handle exception
                Console.WriteLine(e.Message);
                return null;
            }

            return _mapper.Map<AddressDto>(address);
        }
    }
}
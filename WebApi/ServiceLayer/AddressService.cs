using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using ModelLayer.DataTransferObjects;
using RepositoryLayer;

namespace ServiceLayer
{
    public interface IAddressService
    {
        public Task<List<AddressDto>> GetAsync();
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
    }
}
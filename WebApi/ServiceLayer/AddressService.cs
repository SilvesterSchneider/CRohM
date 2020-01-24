using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ModelLayer;
using ModelLayer.DataTransferObjects;
using ModelLayer.Models;
using RepositoryLayer;

namespace ServiceLayer
{
    public interface IAddressService : IAddressRepository
    {
    }

    public class AddressService : AddressRepository, IAddressService
    {
        public AddressService(CrmContext ctx) : base(ctx)
        {
        }
    }
}
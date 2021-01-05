using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DataTransferObjects;
using ModelLayer.Models;
using NSwag.Annotations;
using RepositoryLayer;
using ServiceLayer;
using WebApi.Helper;
using WebApi.Wrapper;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModificationEntryController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IModificationEntryRepository modificationEntryRepository;
        private readonly IUserService userService;

        public ModificationEntryController(IMapper mapper, IModificationEntryRepository modificationEntryRepository, IUserService userService)
        {
            this.mapper = mapper;
            this.userService = userService;
            this.modificationEntryRepository = modificationEntryRepository;
        }

        [HttpGet]
        [Authorize]
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<ModificationEntryDto>), Description = "successfully found")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(void), Description = "contact not found")]
        public async Task<IActionResult> GetSortedListByType(MODEL_TYPE modelDataType)
        {
            User userOfChange = await userService.FindByNameAsync(User.Identity.Name);
            List<ModificationEntry> entries = await modificationEntryRepository.GetSortedModificationEntriesByModelDataTypeAsync(modelDataType, userOfChange);
            if (entries != null)
            {
                return Ok(mapper.Map<List<ModificationEntryDto>>(entries));
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("id")]
        [Authorize]
        [SwaggerResponse(HttpStatusCode.OK, typeof(PagedResponse<List<ModificationEntryDto>>), Description = "successfully found")]
        public async Task<IActionResult> GetSortedListByTypeAndId([FromQuery] long id, [FromQuery] MODEL_TYPE modelDataType, [FromQuery] PaginationFilter filter)
        {
            if(filter == null)
            {
                List<ModificationEntry> fullResult = await modificationEntryRepository.GetModificationEntriesByIdAndModelTypeAsync(id, modelDataType);
                return Ok(new PagedResponse<List<ModificationEntryDto>>(mapper.Map<List<ModificationEntryDto>>(fullResult), 0, 0, 0));

            }
            PaginationFilter validFilter = new PaginationFilter(filter.PageStart, filter.PageSize);
            List<ModificationEntry> entries = await modificationEntryRepository.GetModificationEntriesByIdAndModelTypePaginationAsync(id, modelDataType, validFilter.PageStart, validFilter.PageSize);
            int totalRecords = await modificationEntryRepository.GetModificationEntriesByIdAndModelTypeCountAsync(id, modelDataType);
            return Ok(new PagedResponse<List<ModificationEntryDto>>(mapper.Map<List<ModificationEntryDto>>(entries), validFilter.PageStart, validFilter.PageSize, totalRecords));
        }
    }
}

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

namespace WebApi.Controllers
{
    [Route("api/home")]
    [ApiController]
    public class ModificationEntryController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IModificationEntryRepository modificationEntryRepository;

        public ModificationEntryController(IMapper mapper, IModificationEntryRepository modificationEntryRepository)
        {
            this.mapper = mapper;
            this.modificationEntryRepository = modificationEntryRepository;
        }

        [HttpGet]
        [Authorize]
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<ModificationEntryDto>), Description = "successfully found")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(void), Description = "contact not found")]
        public async Task<IActionResult> GetSortedListByType(MODEL_TYPE modelDataType)
        {
            List<ModificationEntry> entries = await modificationEntryRepository.GetSortedModificationEntriesByModelDataTypeAsync(modelDataType);
            if (entries.Any())
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
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<ModificationEntryDto>), Description = "successfully found")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(void), Description = "contact not found")]
        public async Task<IActionResult> GetSortedListByTypeAndId([FromQuery] long id, [FromQuery] MODEL_TYPE modelDataType)
        {
            List<ModificationEntry> entries = await modificationEntryRepository.GetModificationEntriesByIdAndModelTypeAsync(id, modelDataType);
            if (entries.Any())
            {
                return Ok(mapper.Map<List<ModificationEntryDto>>(entries));
            }
            else
            {
                return NotFound();
            }
        }
    }
}

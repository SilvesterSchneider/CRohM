using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DataTransferObjects;
using ModelLayer.Models;
using NSwag.Annotations;
using ServiceLayer;

namespace WebApi.Controllers
{
    [Route("api/home")]
    [ApiController]
    public class ModificationEntryController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IModificationEntryService modificationEntryService;

        public ModificationEntryController(IMapper mapper, IModificationEntryService modificationEntryService)
        {
            this._mapper = mapper;
            this.modificationEntryService = modificationEntryService;
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<ModificationEntryDto>), Description = "successfully found")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(void), Description = "contact not found")]
        public async Task<IActionResult> GetSortedListByType(MODEL_TYPE modelDataType)
        {
            List<ModificationEntry> entries = await modificationEntryService.GetSortedModificationEntriesByTypeAsync(modelDataType);
            if (entries != null)
            {
                return Ok(_mapper.Map<List<ModificationEntryDto>>(entries));
            }
            else
            {
                return NotFound();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DataTransferObjects;
using ModelLayer.Models;
using NSwag.Annotations;
using ServiceLayer;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EducationalOpportunityController : ControllerBase
    {
        private readonly IEducationalOpportunityService _educationalOpportunityService;
        private readonly IMapper _mapper;

        public EducationalOpportunityController(
            IEducationalOpportunityService educationalOpportunityService,
            IMapper mapper)
        {
            _educationalOpportunityService = educationalOpportunityService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<EducationalOpportunityDto>), Description = "successfully found")]
        public async Task<IActionResult> Get([FromQuery]float ects)
        {
            List<EducationalOpportunity> educationalOpportunities;

            if (Math.Abs(ects - default(float)) < 0)
            {
                educationalOpportunities = await _educationalOpportunityService.GetAsync();
            }
            else
            {
                educationalOpportunities = await _educationalOpportunityService.GetByEctsAsync(ects);
            }
            //TODO: add automapper
            return Ok(_mapper.Map<List<EducationalOpportunityDto>>(educationalOpportunities));
        }
    }
}
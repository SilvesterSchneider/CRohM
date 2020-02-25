using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        public EducationalOpportunityController(IEducationalOpportunityService educationalOpportunityService)
        {
            _educationalOpportunityService = educationalOpportunityService;
        }

        [HttpGet]
        //TODO: change return type to dto
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<EducationalOpportunity>), Description = "successfully found")]
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
            return Ok(educationalOpportunities);
        }
    }
}
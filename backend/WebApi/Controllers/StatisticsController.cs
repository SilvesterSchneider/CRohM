using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using ServiceLayer;
using static ModelLayer.DataTransferObjects.StatisticsDto;

namespace WebApi.Controllers
{
    /// <summary>
    /// RAM: 100%
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private IStatisticsService statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            this.statisticsService = statisticsService;
        }

        [HttpGet("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(List<VerticalGroupedBarDto>), Description = "successfully found")]
        public async Task<IActionResult> GetVerticalGroupedBarDataByType([FromRoute]STATISTICS_VALUES id)
        {
            List<VerticalGroupedBarDto> list = new List<VerticalGroupedBarDto>();
            if (id == STATISTICS_VALUES.INVITED_AND_PARTICIPATED_EVENT_PERSONS)
            {
                list = await statisticsService.GetInvitedAndParticipatedRelationOfEvents();
            }
            else if (id == STATISTICS_VALUES.ALL_CREATED_OBJECTS)
            {
                list = await statisticsService.GetAllCreatedObjects();
            }
            else if (id == STATISTICS_VALUES.ALL_TAGS)
            {
                list = await statisticsService.GetAllTags();
            }
            return Ok(list);
        }
    }
}

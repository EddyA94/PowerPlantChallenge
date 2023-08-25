using Microsoft.AspNetCore.Mvc;
using PowerPlant.Domain.Models.Contracts;
using PowerPlant.Domain.Models.DTO;
using PowerPlant.Infrastructure.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace PowerPlant.Api.Controllers
{
    [ApiController]
    public class ProductionPlanController : ControllerBase
    {
        private readonly IPowerPlantService _powerPlantService;

        public ProductionPlanController(IPowerPlantService powerPlantService)
        {
            _powerPlantService = powerPlantService;
        }

        [HttpPost]
        [Route("CalculatePower")]
        public IActionResult CalculatePower([FromBody][Required] LoadRequest loadRequestDto)
        {
            var res = _powerPlantService.GetProductionPlan(loadRequestDto);
            return Ok(res);
        }
    }
}

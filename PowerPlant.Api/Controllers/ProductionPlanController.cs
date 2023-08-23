using Microsoft.AspNetCore.Http;
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
        public  IActionResult AddNewBeer([FromBody][Required] LoadRequest_Dto loadRequest_Dto)
        {
            if (loadRequest_Dto.Load <= 0 || loadRequest_Dto.Fuels == null || loadRequest_Dto.PowerPlants == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Missing Fields required (Load or Fuels or PowerPlants)");
            }

            try
            {
                var res=  _powerPlantService.GetProductionPlan(loadRequest_Dto);
                return Ok(res);
            }
            catch (CustomExceptions ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}

using PowerPlant.Domain.Models.Contracts;
using PowerPlant.Domain.Models.DTO;
using PowerPlant.Domain.Models.DataModel;
using PowerPlant.Domain.Models.Enums;
using PowerPlant.Infrastructure.Exceptions;
using PowerPlant.Domain.Models;

namespace PowerPlant.Infrastructure.Services
{
    public class PowerPlantService : IPowerPlantService
    {
        public IEnumerable<PowerOutput> GetProductionPlan(LoadRequest loadRequest)
        {
            if (loadRequest.PowerPlants == null)
            {
                throw new CustomExceptions("PowerPlants Cannot Be null", (int)System.Net.HttpStatusCode.BadRequest);
            }

            if (loadRequest.Fuels == null)
            {
                throw new CustomExceptions("Fuel Cannot Be null", (int)System.Net.HttpStatusCode.BadRequest);
            }

            if (loadRequest.Load <= 0)
            {
                throw new CustomExceptions("Load Cannot Be less or equale to 0", (int)System.Net.HttpStatusCode.BadRequest);
            }

            return CalculatePower.CalculateGeneratedPower(loadRequest.PowerPlants, loadRequest.Fuels, loadRequest.Load);
        }

   
    }
}

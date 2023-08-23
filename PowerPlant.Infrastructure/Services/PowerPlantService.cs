using PowerPlant.Domain.Models.Contracts;
using PowerPlant.Domain.Models.DTO;
using PowerPlant.Domain.Models.DataModel;
using PowerPlant.Domain.Models.Enums;
using PowerPlant.Infrastructure.Exceptions;

namespace PowerPlant.Infrastructure.Services
{
    public class PowerPlantService : IPowerPlantService
    {
        public IEnumerable<PowerOutput_Dto> GetProductionPlan(LoadRequest_Dto loadRequest)
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

            return CalculatePower(loadRequest.PowerPlants, loadRequest.Fuels, loadRequest.Load);
        }

        private static IEnumerable<PowerOutput_Dto> CalculatePower(IEnumerable<PowerPlant_Dto> powerPlants_Dto, Fuel_Dto fuel, decimal load)
        {
            var CalculatedPower = new List<PowerPlants>();
            foreach (var powerPlant in powerPlants_Dto)
            {
                PowerPlants PowerPlant = new PowerPlants()
                {
                    Efficiency = powerPlant.Efficiency,
                    Name = powerPlant.Name,
                    PMax = powerPlant.PMax,
                    PMin = powerPlant.PMin,
                    Type = powerPlant.Type,
                    ActualPMax = GetActualPMax(powerPlant, fuel),
                    FuelCost = GetFuelCost(powerPlant, fuel)
                };
                CalculatedPower.Add(PowerPlant);
            }
            return ReportPlanLoad(OrderPowerPlants(CalculatedPower), load);
        }

        private static decimal GetFuelCost(PowerPlant_Dto powerPlant, Fuel_Dto fuel)
        {
            switch (powerPlant.Type)
            {
                case PowerPlantType.GasFired:
                    return fuel.Gas / powerPlant.Efficiency;

                case PowerPlantType.TurboJet:
                    return fuel.Kerosine / powerPlant.Efficiency;

                default:
                    return 0.0M;
            }
        }

        private static decimal GetActualPMax(PowerPlant_Dto powerPlant, Fuel_Dto fuel)
        {
            switch (powerPlant.Type)
            {
                case PowerPlantType.WindTurbine:
                    return powerPlant.PMax / 100.0M * fuel.Wind;
                default:
                    return powerPlant.PMax;
            }
        }

        private static IEnumerable<PowerPlants> OrderPowerPlants(IEnumerable<PowerPlants> powerPlants)
        {
            return powerPlants.OrderByDescending(i => i.Efficiency)
                .ThenBy(i => i.FuelCost)
                .ThenByDescending(i => i.ActualPMax);
        }

        private static IEnumerable<PowerOutput_Dto> ReportPlanLoad(IEnumerable<PowerPlants> powerPlants, decimal planLoad)
        {
            var ret = new List<PowerOutput_Dto>();
            var load = planLoad;
            powerPlants
               .ToList()
               .ForEach(p => {
                   if (p.ActualPMax == 0 || p.PMin > load)
                   {
                       ret.Add(new PowerOutput_Dto()
                       {
                           Name = p.Name,
                           p = 0,
                       });
                       return;
                   }
                   if (load <= p.ActualPMax && load >= p.PMin)
                   {
                       ret.Add(new PowerOutput_Dto()
                       {
                           Name = p.Name,
                           p = Math.Round(load,2),
                       });
                       load = 0;
                       return;
                   }
                   ret.Add(new PowerOutput_Dto()
                   {
                       Name = p.Name,
                       p = Math.Round(p.ActualPMax,2),
                   });
                   load -= p.ActualPMax;
               });
            return ret;
        }
    }
}

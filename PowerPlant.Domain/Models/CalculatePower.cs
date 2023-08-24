using PowerPlant.Domain.Models.DataModel;
using PowerPlant.Domain.Models.DTO;
using PowerPlant.Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerPlant.Domain.Models
{
    public class CalculatePower
    {
        public static IEnumerable<PowerOutput> CalculateGeneratedPower(IEnumerable<Domain.Models.DTO.PowerPlant> powerPlants_Dto, Domain.Models.DTO.Fuel fuel, decimal load)
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

        private static decimal GetFuelCost(Domain.Models.DTO.PowerPlant powerPlant, Domain.Models.DTO.Fuel fuel)
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

        private static decimal GetActualPMax(Domain.Models.DTO.PowerPlant powerPlant, Domain.Models.DTO.Fuel fuel)
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

        private static IEnumerable<PowerOutput> ReportPlanLoad(IEnumerable<PowerPlants> powerPlants, decimal planLoad)
        {
            var ret = new List<PowerOutput>();
            var load = planLoad;
            powerPlants
               .ToList()
               .ForEach(p => {
                   if (p.ActualPMax == 0 || p.PMin > load)
                   {
                       ret.Add(new PowerOutput()
                       {
                           Name = p.Name,
                           p = 0,
                       });
                       return;
                   }
                   if (load <= p.ActualPMax && load >= p.PMin)
                   {
                       ret.Add(new PowerOutput()
                       {
                           Name = p.Name,
                           p = Math.Round(load, 2),
                       });
                       load = 0;
                       return;
                   }
                   ret.Add(new PowerOutput()
                   {
                       Name = p.Name,
                       p = Math.Round(p.ActualPMax, 2),
                   });
                   load -= p.ActualPMax;
               });
            return ret;
        }
    }
}

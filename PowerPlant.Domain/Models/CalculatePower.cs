using PowerPlant.Domain.Models.DataModel;
using PowerPlant.Domain.Models.DTO;
using PowerPlant.Domain.Models.Enums;

namespace PowerPlant.Domain.Models
{
    public static class CalculatePower
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
                    return powerPlant.PMax * fuel.Wind / 100;
                default:
                    return powerPlant.PMax;
            }
        }

        private static List<PowerPlants> OrderPowerPlants(List<PowerPlants> powerPlants)
        {
            return powerPlants.OrderByDescending(i => i.Efficiency)
                .ThenBy(i => i.FuelCost)
                .ThenByDescending(i => i.ActualPMax).ToList();
        }

        private static List<List<PowerPlants>> GetPermutations<PowerPlants>(List<PowerPlants> list)
        {
            List<List<PowerPlants>> result = new List<List<PowerPlants>>();
            GeneratePermutations(list, 0, result);
            return result;
        }

        private static void GeneratePermutations<PowerPlants>(List<PowerPlants> list, int position, List<List<PowerPlants>> result)
        {
            if (position == list.Count)
            {
                result.Add(new List<PowerPlants>(list));
                return;
            }

            for (int i = position; i < list.Count; i++)
            {
                Swap(list, position, i);
                GeneratePermutations(list, position + 1, result);
                Swap(list, position, i);
            }
        }

        private static void Swap<PowerPlants>(List<PowerPlants> list, int i, int j)
        {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        private static IEnumerable<PowerOutput> ReportPlanLoad(List<PowerPlants> powerPlants, decimal planLoad)
        {
            var combinations = GetPermutations(powerPlants);
            var ret = new List<PowerOutput>();
            for (int i = 0; i < combinations.ToList().Count() - 1; i++)
            {
                if (ret.Sum(S => S.p) == planLoad)
                {
                    break;
                }
                else
                {
                    ret.Clear();
                }

                var load = planLoad;
                var currentLoad = 0M;
                combinations[i].ToList().ForEach(p =>
                {
                    if (p.PMin < planLoad && p.PMax < planLoad - currentLoad && i < combinations[i].Count() - 1)
                    {
                        var nextSource = combinations[i].ElementAt(i + 1);
                        var Expected = Math.Min(planLoad - currentLoad - nextSource.PMin, p.PMax);
                        currentLoad += Expected;
                        ret.Add(new PowerOutput()
                        {
                            Name = p.Name,
                            p = Expected,
                        });
                        load -= Expected;

                        return;
                    }
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
            }
            return ret;
        }
    }
}

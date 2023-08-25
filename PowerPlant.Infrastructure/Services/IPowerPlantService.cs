using PowerPlant.Domain.Models.DataModel;
using PowerPlant.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerPlant.Domain.Models.Contracts
{
    public interface IPowerPlantService
    {
        IEnumerable<PowerOutput> GetProductionPlan(LoadRequest loadRequest);
    }
}

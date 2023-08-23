using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerPlant.Domain.Models.DataModel
{
    public class Fuel
    {
        public decimal Gas { get; set; }

        public decimal Kerosine { get; set; }

        public decimal Co2 { get; set; }

        public decimal Wind { get; set; }
    }
}

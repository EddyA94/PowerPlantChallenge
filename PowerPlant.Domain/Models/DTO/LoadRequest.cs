using Newtonsoft.Json;

namespace PowerPlant.Domain.Models.DTO
{
    public class LoadRequest
    {
        [JsonProperty("load")]
        public decimal Load { get; set; }
        [JsonProperty("fuels")]
        public Fuel? Fuels { get; set; }
        [JsonProperty("powerplants")]
        public IEnumerable<PowerPlant> PowerPlants { get; set; }
    }
}

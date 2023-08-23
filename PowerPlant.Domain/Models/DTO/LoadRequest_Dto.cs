using Newtonsoft.Json;

namespace PowerPlant.Domain.Models.DTO
{
    public class LoadRequest_Dto
    {
        [JsonProperty("load")]
        public decimal Load { get; set; }
        [JsonProperty("fuels")]
        public Fuel_Dto? Fuels { get; set; }
        [JsonProperty("powerplants")]
        public IEnumerable<PowerPlant_Dto> PowerPlants { get; set; }
    }
}

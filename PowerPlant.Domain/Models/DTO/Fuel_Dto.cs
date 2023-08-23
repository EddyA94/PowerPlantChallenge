using Newtonsoft.Json;

namespace PowerPlant.Domain.Models.DTO
{
    public class Fuel_Dto
    {
        [JsonProperty("gas(euro/MWh)")]
        public decimal Gas { get; set; }

        [JsonProperty("kerosine(euro/MWh)")]
        public decimal Kerosine { get; set; }

        [JsonProperty("co2(euro/ton)")]
        public int Co2 { get; set; }

        [JsonProperty("wind(%)")]
        public int Wind { get; set; }
    }
}

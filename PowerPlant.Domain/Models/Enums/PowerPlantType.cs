using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;

namespace PowerPlant.Domain.Models.Enums
{
    public enum PowerPlantType
    {
        [EnumMember(Value = "gasfired")]
        GasFired = 0,
        [EnumMember(Value = "turbojet")]
        TurboJet = 1,
        [EnumMember(Value = "windturbine")]
        WindTurbine = 2,
    }
}

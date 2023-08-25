using Newtonsoft.Json;
using PowerPlant.Domain.Models;
using PowerPlant.Domain.Models.DTO;
using PowerPlant.Domain.Models.Enums;
using PowerPlant.Infrastructure.Exceptions;
using PowerPlant.Infrastructure.Services;
using System.Net;

namespace PowerPlant.Test
{
    public class PowerPlantServiceTests
    {
        private readonly PowerPlantService _powerPlantService;

        public PowerPlantServiceTests()
        {
            _powerPlantService = new PowerPlantService();
        }

        public static IEnumerable<object[]> TestData()
        {
            yield return new object[] { null,new Fuel(), 100 };
            yield return new object[] { new List<Domain.Models.DTO.PowerPlant>(), null, 100 };
            yield return new object[] { new List<Domain.Models.DTO.PowerPlant>(), new Fuel(), -1 };
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void GetProductionPlan_Null_ThrowsCustomException(IEnumerable<Domain.Models.DTO.PowerPlant> PowerPlants, Fuel Fuel, int Load)
        {
            // Arrange
            var loadRequest = new LoadRequest
            {
                PowerPlants = null,
                Fuels = Fuel,
                Load = 100
            };

            // Act & Assert
            var exception = Assert.Throws<CustomExceptions>(() => _powerPlantService.GetProductionPlan(loadRequest));
            Assert.True((int)HttpStatusCode.BadRequest== exception.StatusCode);
            Assert.True("PowerPlants Cannot Be null"== exception.Message);
        }

        [Theory]
        [InlineData("payload/payload1.json")]
        [InlineData("payload/payload2.json")]
        [InlineData("payload/payload3.json")]
        void TestCalculator(string payloadFile)
        {
            // Setup
            var payload = JsonConvert.DeserializeObject<LoadRequest>(File.ReadAllText(payloadFile));
            var Ppayload = _powerPlantService.GetProductionPlan(payload);
            decimal required = payload.Load;
            var calculated = CalculatePower.CalculateGeneratedPower(payload.PowerPlants, payload.Fuels, required);
            Assert.Equal(payload.Load, calculated.Sum(x => x.p));
            calculated.Where(W => W.p > 0).ToList().ForEach(x =>
            {
                // Using xUnit's Assert to check the ranges
                Assert.True(x.p <= payload.PowerPlants.Where(S => S.Name == x.Name).FirstOrDefault().PMax, x.Name + " Value of p should be less than or equal to PMax");
                Assert.True(x.p >= payload.PowerPlants.Where(S => S.Name == x.Name).FirstOrDefault().PMin, x.Name + " Value of p should be greater than or equal to PMin");
            });
        }

        private LoadRequest GetMockLoadRequest()
        {
            return new LoadRequest
            {
                PowerPlants = new List<Domain.Models.DTO.PowerPlant>  {
                new ()
                {
                    Name = "windpark1",
                    Type = PowerPlantType.WindTurbine,
                    Efficiency = 1,
                    PMin = 0,
                    PMax = 150,
                },
                new()
                {
                    Name = "windpark2",
                    Type = PowerPlantType.WindTurbine,
                    Efficiency = 1,
                    PMin = 0,
                    PMax = 36,
                },
                new ()
                {
                    Name = "gasfiredbig1",
                    Type = PowerPlantType.GasFired,
                    Efficiency = 0.53M,
                    PMin = 100,
                    PMax = 460,
                },
                new ()
                {
                    Name = "gasfiredbig2",
                    Type = PowerPlantType.GasFired,
                    Efficiency = 0.53M,
                    PMin = 100,
                    PMax = 460,
                },
                new ()
                {
                    Name = "gasfiredsomewhatsmaller",
                    Type = PowerPlantType.GasFired,
                    Efficiency = 0.37M,
                    PMin = 200,
                    PMax = 2100,
                },
                new ()
                {
                    Name = "tj1",
                    Type = PowerPlantType.TurboJet,
                    Efficiency = 0.3M,
                    PMin = 0,
                    PMax = 16,
                },
            },
                Fuels = new Fuel
                {
                    Co2 = 20,
                    Gas = 13.4M,
                    Kerosine = 50.8M,
                    Wind = 60,
                },
                Load = 480
            };
        }

        private List<PowerOutput> GetMockOutputRequest()
        {
            return new List<PowerOutput>
             {
                 new ()
                 {
                     Name="windpark1",
                     p=90.0M
                 },
                 new ()
                 {
                     Name="windpark2",
                     p=21.60M
                 },
                 new ()
                 {
                     Name="gasfiredbig1",
                     p=368.40M
                 },
                new ()
                 {
                     Name="tj1",
                     p=0
                 },
                 new ()
                 {
                     Name="gasfiredbig2",
                     p=0
                 },
                 new ()
                 {
                     Name="gasfiredsomewhatsmaller",
                     p=0
                 },
                
             };
        }

    }
}
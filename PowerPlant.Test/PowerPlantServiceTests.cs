using PowerPlant.Domain.Models.Contracts;
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


        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetProductionPlan_PowerPlantsNull_ThrowsCustomException()
        {
            // Arrange
            var loadRequest = new LoadRequest_Dto
            {
                PowerPlants = null,
                Fuels = new Fuel_Dto(),
                Load = 100
            };

            // Act & Assert
            var exception = Assert.Throws<CustomExceptions>(() => _powerPlantService.GetProductionPlan(loadRequest));
            Assert.AreEqual((int)HttpStatusCode.BadRequest, exception.StatusCode);
            Assert.AreEqual("PowerPlants Cannot Be null", exception.Message);
        }

        [Test]
        public void GetProductionPlan_FuelsNull_ThrowsCustomException()
        {
            // Arrange
            var loadRequest = new LoadRequest_Dto
            {
                PowerPlants = new List<PowerPlant_Dto>(),
                Fuels = null,
                Load = 100
            };

            // Act & Assert
            var exception = Assert.Throws<CustomExceptions>(() => _powerPlantService.GetProductionPlan(loadRequest));
            Assert.AreEqual((int)HttpStatusCode.BadRequest, exception.StatusCode);
            Assert.AreEqual("Fuel Cannot Be null", exception.Message);
        }

        [Test]
        public void GetProductionPlan_LoadLessOrEqual0_ThrowsCustomException()
        {
            // Arrange
            var loadRequest = new LoadRequest_Dto
            {
                PowerPlants = new List<PowerPlant_Dto>(),
                Fuels = new Fuel_Dto(),
                Load = -1
            };

            // Act & Assert
            var exception = Assert.Throws<CustomExceptions>(() => _powerPlantService.GetProductionPlan(loadRequest));
            Assert.AreEqual((int)HttpStatusCode.BadRequest, exception.StatusCode);
            Assert.AreEqual("Load Cannot Be less or equale to 0", exception.Message);
        }

        [Test]
        public void GetProductionPlan_ThrowsCustomException()
        {
            // Arrange
            var LoadtMock = GetMockLoadRequest();
            var OutputMock = GetMockOutputRequest();

            //Act
            var result = _powerPlantService.GetProductionPlan(LoadtMock);

            //Assert
            for (int i = 0; i < result.ToList().Count; i++)
            {
                Assert.AreEqual(result.ToList()[i].Name, OutputMock[i].Name);
                Assert.AreEqual(result.ToList()[i].p, OutputMock[i].p);
            }
        }

        private LoadRequest_Dto GetMockLoadRequest()
        {
            return new LoadRequest_Dto
            {
                PowerPlants = new List<PowerPlant_Dto>  {
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
                Fuels = new Fuel_Dto
                {
                    Co2 = 20,
                    Gas = 13.4M,
                    Kerosine = 50.8M,
                    Wind = 60,
                },
                Load = 480
            };
        }

        private List<PowerOutput_Dto> GetMockOutputRequest()
        {
            return new List<PowerOutput_Dto>
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
                     Name="gasfiredbig2",
                     p=0
                 },
                 new ()
                 {
                     Name="gasfiredsomewhatsmaller",
                     p=0
                 },
                 new ()
                 {
                     Name="tj1",
                     p=0
                 },
             };
        }

    }
}
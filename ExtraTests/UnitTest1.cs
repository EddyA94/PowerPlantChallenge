using Newtonsoft.Json;
using PowerPlant.Domain.Models;
using PowerPlant.Domain.Models.DTO;
using PowerPlant.Infrastructure.Services;

namespace ExtraTests
{
    public class Tests
    {
        private readonly PowerPlantService _powerPlantService;

        public Tests()
        {
            _powerPlantService = new PowerPlantService();
        }


        [Theory]
        [InlineData("payload/payload1.json")]
        [InlineData("payload/payload2.json")]
        [InlineData("payload/payload3.json")]
        [InlineData("payload/payload4.json")]
        [InlineData("payload/payload5.json")]
        [InlineData("payload/payload6.json")]
        [InlineData("payload/payload7.json")]
        [InlineData("payload/payload8.json")]
        [InlineData("payload/payload9.json")]
        [InlineData("payload/payload10.json")]

        void TestMeritOrderCalculator(string payloadFile)
        {
            // Setup
            var payload = JsonConvert.DeserializeObject<LoadRequest>(File.ReadAllText(payloadFile));
            var Ppayload = _powerPlantService.GetProductionPlan(payload);
            decimal required = payload.Load;
            var calculated = CalculatePower.CalculateGeneratedPower(payload.PowerPlants, payload.Fuels, required);
            Assert.Equal(payload.Load, calculated.Sum(x => x.p));
            calculated.Where(W=>W.p>0).ToList().ForEach(x =>
            {
                // Using xUnit's Assert to check the ranges
                Assert.True(x.p <= payload.PowerPlants.Where(S=>S.Name == x.Name).FirstOrDefault().PMax, x.Name + " Value of p should be less than or equal to PMax");
                Assert.True(x.p >= payload.PowerPlants.Where(S => S.Name == x.Name).FirstOrDefault().PMin, x.Name + " Value of p should be greater than or equal to PMin");
            });
        }
    }
}
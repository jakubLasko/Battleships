using Battleships.Models.DataTypes;
using Battleships.Models.Enums;
using Battleships.Services;
using Battleships.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Battleships.Tests.Services
{
    [TestFixture]
    public class ShipsDefinitionServiceTests
    {
        private Mock<ILogger<ShipsDefinitionService>> loggerMock;
        private IShipsDefinitionService service;

        [SetUp]
        public void Setup()
        {
            loggerMock = new Mock<ILogger<ShipsDefinitionService>>();

            // Create a mock for options
            var appSettingsMock = new Mock<IOptions<AppSettings>>();
            appSettingsMock.Setup(x => x.Value).Returns(new AppSettings() { ShipsDefinitionPath = "./Definitions/ShipsDefinition.json" });

            service = new ShipsDefinitionService(loggerMock.Object, appSettingsMock.Object);
        }

        [Test]
        public async Task LoadShipDefinitionsAsyncTest()
        {
            var result = await service.LoadShipDefinitionsAsync(CancellationToken.None);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.Not.Empty);
                Assert.That(result.Select(x => x.Type),
                    Is.EquivalentTo(new[]
                    {
                        ShipType.Carrier,
                        ShipType.Battleship,
                        ShipType.Cruiser,
                        ShipType.Submarine,
                        ShipType.Destroyer
                    }));
            });
        }

        [Test]
        public async Task ShipShapesTest()
        {
            var result = await service.LoadShipDefinitionsAsync(CancellationToken.None);

            // Test if carrier shape is as expected
            var carrier = result.FirstOrDefault(x => x.Type == ShipType.Carrier);

            Assert.Multiple(() =>
            {
                Assert.That(carrier, Is.Not.Null);
                Assert.That(carrier!.Shape, Has.Count.EqualTo(6));
                Assert.That(carrier.Shape, Does.Contain(new Vector2(0, 0)));
                Assert.That(carrier.Shape, Does.Contain(new Vector2(1, 0)));
                Assert.That(carrier.Shape, Does.Contain(new Vector2(2, 0)));
                Assert.That(carrier.Shape, Does.Contain(new Vector2(3, 0)));
                Assert.That(carrier.Shape, Does.Contain(new Vector2(4, 0)));
                Assert.That(carrier.Shape, Does.Contain(new Vector2(2, 1)));
            });

            // Note: Other shapes would follow similar pattern
        }

        [Test]
        public void CancellationTokenTest()
        {
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            Assert.ThrowsAsync<TaskCanceledException>(async () => await service.LoadShipDefinitionsAsync(cts.Token));
        }
    }
}

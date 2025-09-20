using Battleships.Storages;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace Battleships.Tests.Storages
{
    [TestFixture]
    public class GameStorageTests
    {
        private GameStorage storage;
        private Mock<IMemoryCache> cacheMock;
        private Mock<ILogger<GameStorage>> loggerMock;

        [SetUp]
        public void Setup()
        {
            cacheMock = new Mock<IMemoryCache>();
            loggerMock = new Mock<ILogger<GameStorage>>();
            storage = new GameStorage(cacheMock.Object, loggerMock.Object);
        }

        [Test]
        public void CreationTest()
        {
            Assert.Throws<ArgumentNullException>(() => new GameStorage(null!, loggerMock.Object));
            Assert.Throws<ArgumentNullException>(() => new GameStorage(cacheMock.Object, null!));
            Assert.IsNotNull(storage);
        }
    }
}

using Battleships.Models;
using Battleships.Models.DataTypes;
using Battleships.Storages;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace Battleships.Tests.Storages
{
    [TestFixture]
    public class GameStorageTests
    {
        private Mock<IMemoryCache> cacheMock;
        private Mock<ILogger<GameStorage>> loggerMock;

        [SetUp]
        public void Setup()
        {
            cacheMock = new Mock<IMemoryCache>();
            loggerMock = new Mock<ILogger<GameStorage>>();
        }

        [Test]
        public void CreationTest()
        {
            Assert.Throws<ArgumentNullException>(() => new GameStorage(null!, loggerMock.Object));
            Assert.Throws<ArgumentNullException>(() => new GameStorage(cacheMock.Object, null!));

            var storage = CreateStorage();
            Assert.IsNotNull(storage);
        }

        [Test]
        public void AddOpenGameTest()
        {
            var storage = CreateStorage();
            var game = new Game();
            game.Initialize(new Player("Player1"), new Vector2(10, 10), Common.GetShipDefinitions());

            var cacheEntry = new Mock<ICacheEntry>();
            cacheMock.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(cacheEntry.Object);

            storage.AddGame(game);
            var openGames = storage.GetOpenGames();

            Assert.Multiple(() =>
            {
                Assert.That(openGames, Has.Count.EqualTo(1));
                Assert.That(openGames[0], Is.EqualTo(game));
            });
            cacheMock.Verify(x => x.CreateEntry(game.Id), Times.Once);
        }

        [Test]
        public void RemoveOpenGameTest()
        {
            var storage = CreateStorage();
            var game = new Game();
            game.Initialize(new Player("Player1"), new Vector2(10, 10), Common.GetShipDefinitions());

            var cacheEntry = new Mock<ICacheEntry>();
            cacheMock.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(cacheEntry.Object);

            storage.AddGame(game);

            var result = storage.RemoveOpenGame(game.Id);
            var openGames = storage.GetOpenGames();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(openGames, Is.Empty);
            });
        }

        [Test]
        public void RemoveGameTest()
        {
            var storage = CreateStorage();
            var game = new Game();
            game.Initialize(new Player("Player1"), new Vector2(10, 10), Common.GetShipDefinitions());

            var cacheEntry = new Mock<ICacheEntry>();
            cacheMock.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(cacheEntry.Object);

            storage.AddGame(game);

            var result = storage.RemoveGame(game.Id);
            var openGames = storage.GetOpenGames();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.True);
                Assert.That(openGames, Is.Empty);
            });
            cacheMock.Verify(x => x.Remove(game.Id), Times.Once);
        }

        private GameStorage CreateStorage()
        {
            return new GameStorage(cacheMock.Object, loggerMock.Object);
        }
    }
}

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
        private Mock<ILogger<GameStorage>> loggerMock;

        [SetUp]
        public void Setup()
        {
            loggerMock = new Mock<ILogger<GameStorage>>();
        }

        [Test]
        public void CreationTest()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            Assert.Throws<ArgumentNullException>(() => new GameStorage(null!, loggerMock.Object));
            Assert.Throws<ArgumentNullException>(() => new GameStorage(memoryCache, null!));

            var storage = new GameStorage(memoryCache, loggerMock.Object);
            Assert.IsNotNull(storage);
        }

        [Test]
        public void AddOpenGameTest()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var storage = new GameStorage(memoryCache, loggerMock.Object);
            var game = new Game();
            game.Initialize(new Player("Player1"), new Vector2(10, 10), Common.GetShipDefinitions());

            storage.AddGame(game);
            var openGames = storage.GetOpenGames();
            var cachedGame = memoryCache.Get<Game>(game.Id);

            Assert.Multiple(() =>
            {
                Assert.That(openGames, Has.Count.EqualTo(1));
                Assert.That(openGames[0], Is.EqualTo(game));
                Assert.That(cachedGame, Is.EqualTo(game));
            });
        }

        [Test]
        public void RemoveOpenGameTest()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var storage = new GameStorage(memoryCache, loggerMock.Object);
            var game = new Game();
            game.Initialize(new Player("Player1"), new Vector2(10, 10), Common.GetShipDefinitions());

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
        public void GetGameTest()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var storage = new GameStorage(memoryCache, loggerMock.Object);
            var game = new Game();
            game.Initialize(new Player("Player1"), new Vector2(10, 10), Common.GetShipDefinitions());

            storage.AddGame(game);

            var result = storage.GetGame(game.Id);
            var openGames = storage.GetOpenGames();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(game));
                Assert.That(openGames, Is.Not.Empty);
            });
        }
    }
}

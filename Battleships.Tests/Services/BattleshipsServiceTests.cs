using Battleships.Models;
using Battleships.Models.DataTypes;
using Battleships.Models.Enums;
using Battleships.Models.GameIO;
using Battleships.Services;
using Battleships.Services.Interfaces;
using Battleships.Storages.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Battleships.Tests.Services
{
    [TestFixture]
    public class BattleshipsServiceTests
    {
        private Mock<ILogger<BattleshipsService>> loggerMock;
        private Mock<IShipsDefinitionService> shipsDefinitionServiceMock;
        private Mock<IGameStorage> gameStorageMock;
        private BattleshipsService service;

        [SetUp]
        public void Setup()
        {
            loggerMock = new Mock<ILogger<BattleshipsService>>();
            shipsDefinitionServiceMock = new Mock<IShipsDefinitionService>();
            gameStorageMock = new Mock<IGameStorage>();

            // Create service instance with mocked dependencies
            service = new BattleshipsService(
                loggerMock.Object,
                shipsDefinitionServiceMock.Object,
                gameStorageMock.Object
            );

            // Setup default ship definitions
            var shipDefinitions = Common.GetShipDefinitions();

            // Setup mock to return ship definitions
            shipsDefinitionServiceMock
                .Setup(x => x.LoadShipDefinitionsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(shipDefinitions);
        }

        [Test]
        public async Task StartGameAsyncTest()
        {
            var gameStartData = new GameCreateData
            {
                Player = new Player("Player1"),
                Opponent = new Player("Player2"),
                BoardSizeX = 10,
                BoardSizeY = 10
            };

            var result = await service.CreateGameAsync(gameStartData, CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsInitialized, Is.True);
                Assert.That(result.State, Is.EqualTo(GameState.InProgress));
                Assert.That(result.PlayerBoard.Size.X, Is.EqualTo(gameStartData.BoardSizeX));
                Assert.That(result.PlayerBoard.Size.Y, Is.EqualTo(gameStartData.BoardSizeY));
            });
            gameStorageMock.Verify(x => x.AddGame(It.IsAny<Game>()), Times.Once);
        }

        [Test]
        public void StartGameAsyncEmptyTest()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.CreateGameAsync(default, CancellationToken.None));
        }

        [Test]
        public void ShootWaterTest()
        {
            var game = CreateGame();
            var waterCellPosition = Common.FindWaterCell(game.OpponentBoard.Grid);

            var shootData = new ShootData() { GameId = game.Id, Position = waterCellPosition };
            var result = service.Shoot(shootData);

            Assert.That(result.State, Is.EqualTo(ShotState.Water));
            Assert.That(result.GameState, Is.EqualTo(GameState.InProgress));

            // Test passing turn
            Assert.That(game.PlayerOnTurn, Is.EqualTo(game.Opponent));
        }

        [Test]
        public void ShootShipTest()
        {
            var game = CreateGame();
            var ship = game.OpponentBoard.Ships.First(x => !x.IsSunk);

            var shootData = new ShootData()
            {
                GameId = game.Id,
                Position = ship.Cells.First(c => !ship.Hits.Any(h => h == c))
            };

            var result = service.Shoot(shootData);

            Assert.That(result.State, Is.EqualTo(ShotState.Hit));
            Assert.That(result.GameState, Is.EqualTo(GameState.InProgress));

            // Test continuation of turn
            Assert.That(game.PlayerOnTurn, Is.EqualTo(game.Player));
        }

        [Test]
        public void PlayerWinTest()
        {
            var game = CreateGame();

            ShotResult shotResult = default;
            foreach (var ship in game.OpponentBoard.Ships)
            {
                foreach (var cell in ship.Cells)
                {
                    var shootData = new ShootData() { GameId = game.Id, Position = cell };
                    shotResult = service.Shoot(shootData);
                }
            }

            Assert.That(shotResult, Is.Not.EqualTo((ShotResult)default));
            Assert.That(shotResult.State, Is.EqualTo(ShotState.ShipSunk));
            Assert.That(shotResult.GameState, Is.EqualTo(GameState.Finished));
            Assert.That(game.PlayerOnTurn, Is.EqualTo(game.Player));
        }

        [Test]
        public void OpponentWinTest()
        {
            var game = CreateGame();

            // Player shoot a miss to switch turn to opponent
            var playerShot = service.Shoot(new ShootData()
            {
                GameId = game.Id,
                Position = Common.FindWaterCell(game.OpponentBoard.Grid)
            });

            Assert.That(playerShot.State, Is.EqualTo(ShotState.Water));
            Assert.That(playerShot.GameState, Is.EqualTo(GameState.InProgress));
            Assert.That(game.PlayerOnTurn, Is.EqualTo(game.Opponent));

            ShotResult shotResult = default;
            foreach (var ship in game.PlayerBoard.Ships)
            {
                foreach (var cell in ship.Cells)
                {
                    var shootData = new ShootData() { GameId = game.Id, Position = cell };
                    shotResult = service.Shoot(shootData);
                }
            }

            Assert.That(shotResult, Is.Not.EqualTo((ShotResult)default));
            Assert.That(shotResult.State, Is.EqualTo(ShotState.ShipSunk));
            Assert.That(shotResult.GameState, Is.EqualTo(GameState.Finished));
            Assert.That(game.PlayerOnTurn, Is.EqualTo(game.Opponent));
        }

        /// <summary>
        /// Creates fresh game for testing
        /// </summary>
        /// <returns>Fresh Game</returns>
        private Game CreateGame()
        {
            // Note: For test we force synchronous loading
            // Later we could switch to async if needed
            var shipDefinitions = shipsDefinitionServiceMock.Object.LoadShipDefinitionsAsync(CancellationToken.None).GetAwaiter().GetResult();

            var game = new Game();
            game.Initialize(
                new Player("Player1"),
                new Player("Player2"),
                new Vector2(10, 10),
                shipDefinitions
            );

            game.Start();

            gameStorageMock
                .Setup(x => x.GetGame(game.Id))
                .Returns(game);

            return game;
        }
    }
}

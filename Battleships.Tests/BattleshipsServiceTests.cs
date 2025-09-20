using Battleships.Models;
using Battleships.Models.DataTypes;
using Battleships.Models.Enums;
using Battleships.Models.GameIO;
using Battleships.Models.GameSetup;
using Battleships.Services;
using Battleships.Services.Interfaces;
using Battleships.Storages.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace Battleships.Tests
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
            var shipDefinitions = new List<ShipDefinition>
            {
                new() { Type = ShipType.Carrier, Shape = new List<Vector2> { new(0, 0), new(1, 0), new(2, 0), new(3, 0), new(4, 0), new(2, 1) }, Count = 1, AllowRotation = true },
                new() { Type = ShipType.Battleship, Shape = new List<Vector2> { new(1, 0), new(1, 1), new(1, 2), new(0, 1), new(2, 1) }, Count = 1, AllowRotation = true },
                new() { Type = ShipType.Cruiser, Shape = new List<Vector2> { new(0, 0), new(1, 0), new(2, 0) }, Count = 1, AllowRotation = true },
                new() { Type = ShipType.Submarine, Shape = new List<Vector2> { new(0, 0), new(1, 0) }, Count = 2, AllowRotation = true },
                new() { Type = ShipType.Destroyer, Shape = new List<Vector2> { new(0, 0) }, Count = 2, AllowRotation = true }
            };

            // Setup mock to return ship definitions
            shipsDefinitionServiceMock
                .Setup(x => x.LoadShipDefinitionsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(shipDefinitions);
        }

        [Test]
        public async Task StartGameAsyncTest()
        {
            var gameStartData = new GameStartData
            {
                FirstPlayer = new Player { Name = "Player1" },
                SecondPlayer = new Player { Name = "Player2" },
                BoardSizeX = 10,
                BoardSizeY = 10
            };

            var result = await service.StartGameAsync(gameStartData, CancellationToken.None);

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
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.StartGameAsync(default, CancellationToken.None));
        }

        [Test]
        public void ShootWaterTest()
        {
            var game = CreateGame();
            var waterCellPosition = FindWaterCell(game.OpponentBoard.Grid);

            var shootData = new ShootData(game.Id, waterCellPosition);
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

            var shootData = new ShootData(game.Id, ship.Cells.First(c => !ship.Hits.Any(h => h == c)));

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
                    var shootData = new ShootData(game.Id, cell);
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
            var playerShot = service.Shoot(new ShootData(game.Id, FindWaterCell(game.OpponentBoard.Grid)));

            Assert.That(playerShot.State, Is.EqualTo(ShotState.Water));
            Assert.That(playerShot.GameState, Is.EqualTo(GameState.InProgress));
            Assert.That(game.PlayerOnTurn, Is.EqualTo(game.Opponent));

            ShotResult shotResult = default;
            foreach (var ship in game.PlayerBoard.Ships)
            {
                foreach (var cell in ship.Cells)
                {
                    var shootData = new ShootData(game.Id, cell);
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
            // For test we force synchronous loading
            // Later we could switch to async if needed
            var shipDefinitions = shipsDefinitionServiceMock.Object.LoadShipDefinitionsAsync(CancellationToken.None).GetAwaiter().GetResult();

            var game = new Game();
            game.Initialize(
                new Player { Name = "Player1" },
                new Player { Name = "Player2" },
                new Vector2(10, 10),
                shipDefinitions
            );

            game.Start();

            gameStorageMock
                .Setup(x => x.GetGame(game.Id))
                .Returns(game);

            return game;
        }

        private Vector2 FindWaterCell(Cell[,] grid)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    if (grid[x, y].State == CellState.Water)
                    {
                        return new Vector2(x, y);
                    }
                }
            }

            throw new InvalidOperationException("No water cell found on the board");
        }
    }
}

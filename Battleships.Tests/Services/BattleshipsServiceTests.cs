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
        public async Task CreateGameAsyncTest()
        {
            var gameStartData = new GameCreateData
            {
                Player = new Player("Player1"),
                BoardSizeX = 10,
                BoardSizeY = 10
            };

            var result = await service.CreateGameAsync(gameStartData, CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.IsInitialized, Is.True);
                Assert.That(result.State, Is.EqualTo(GameState.WaitingForOpponent));
                Assert.That(result.PlayerBoard.Size.X, Is.EqualTo(gameStartData.BoardSizeX));
                Assert.That(result.PlayerBoard.Size.Y, Is.EqualTo(gameStartData.BoardSizeY));
                Assert.That(result.PlayerOnTurn, Is.EqualTo(result.Player));
            });
            gameStorageMock.Verify(x => x.AddGame(It.IsAny<Game>()), Times.Once);
        }

        [Test]
        public void CreateGameAsyncEmptyTest()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.CreateGameAsync(default, CancellationToken.None));
        }

        [Test]
        public async Task JoinGameAsyncTest()
        {
            var game = new Game();
            game.Initialize(new Player("Player1"), new Vector2(10, 10), Common.GetShipDefinitions());

            gameStorageMock
                .Setup(x => x.GetGame(game.Id))
                .Returns(game);

            var opponent = new Player("Player2");
            var result = await service.JoinGameAsync(game.Id, opponent, CancellationToken.None);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsInitialized, Is.True);
                Assert.That(result.State, Is.EqualTo(GameState.Ready));
                Assert.That(result.Opponent, Is.EqualTo(opponent));
                Assert.That(result.OpponentBoard, Is.Not.Null);
                Assert.That(result.OpponentBoard.Ships, Is.Not.Empty);
            });
        }

        [Test]
        public void JoinGameAsyncEmptyTest()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await service.CreateGameAsync(default, CancellationToken.None));
        }

        [Test]
        public void JoinAlreadyFullGameTest()
        {
            var game = CreateGame();

            gameStorageMock
                .Setup(x => x.GetGame(game.Id))
                .Returns(game);

            Assert.ThrowsAsync<InvalidOperationException>(async () => await service.JoinGameAsync(game.Id, new Player("Player3"), CancellationToken.None));
        }

        [Test]
        public void GetGameTest()
        {
            var game = CreateGame();

            gameStorageMock
                .Setup(x => x.GetGame(game.Id))
                .Returns(game);

            var result = service.GetGame(game.Id);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Id, Is.EqualTo(game.Id));
                Assert.That(result.State, Is.EqualTo(GameState.InProgress));
                Assert.That(result.Player, Is.Not.Null);
                Assert.That(result.Opponent, Is.Not.Null);
            });

            gameStorageMock.Verify(x => x.GetGame(game.Id), Times.Once);
        }

        [Test]
        public void ShootWaterTest()
        {
            var game = CreateGame();
            var waterCellPosition = Common.FindWaterCell(game.OpponentBoard.Grid);

            TestContext.WriteLine("Opponent board before shoot.");
            TestContext.WriteLine(Common.PrintBoard(game.OpponentBoard));

            var result = service.Shoot(game.Id, game.Player.Id, waterCellPosition);

            TestContext.WriteLine("Opponent board after shoot.");
            TestContext.WriteLine(Common.PrintBoard(game.OpponentBoard));

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

            TestContext.WriteLine("Opponent board before shoot.");
            TestContext.WriteLine(Common.PrintBoard(game.OpponentBoard));

            var result = service.Shoot(game.Id, game.Player.Id, ship.Cells.First(c => !ship.Hits.Any(h => h == c)));

            TestContext.WriteLine("Opponent board after shoot.");
            TestContext.WriteLine(Common.PrintBoard(game.OpponentBoard));

            Assert.That(result.State, Is.EqualTo(ShotState.Hit));
            Assert.That(result.GameState, Is.EqualTo(GameState.InProgress));

            // Test continuation of turn
            Assert.That(game.PlayerOnTurn, Is.EqualTo(game.Player));
        }

        [Test]
        public void ShootPlayerNotOnTurnTest()
        {
            var game = CreateGame();

            Assert.Throws<InvalidOperationException>(() => service.Shoot(game.Id, game.Opponent.Id, new Vector2(0, 0)));
        }

        [Test]
        public void PlayerWinTest()
        {
            var game = CreateGame();

            TestContext.WriteLine("Opponent board initial state.");
            TestContext.WriteLine(Common.PrintBoard(game.OpponentBoard));

            ShotResult shotResult = default;
            foreach (var ship in game.OpponentBoard.Ships)
            {
                foreach (var cell in ship.Cells)
                    shotResult = service.Shoot(game.Id, game.Player.Id, cell);
            }

            TestContext.WriteLine("Opponent board at the end of the game.");
            TestContext.WriteLine(Common.PrintBoard(game.OpponentBoard));

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
            var playerShot = service.Shoot(game.Id, game.Player.Id, Common.FindWaterCell(game.OpponentBoard.Grid));

            Assert.That(playerShot.State, Is.EqualTo(ShotState.Water));
            Assert.That(playerShot.GameState, Is.EqualTo(GameState.InProgress));
            Assert.That(game.PlayerOnTurn, Is.EqualTo(game.Opponent));

            ShotResult shotResult = default;
            foreach (var ship in game.PlayerBoard.Ships)
            {
                foreach (var cell in ship.Cells)
                    shotResult = service.Shoot(game.Id, game.Opponent.Id, cell);
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
            game.Initialize(new Player("Player1"), new Vector2(10, 10), shipDefinitions);

            // Join opponent
            game.Join(new Player("Player2"), shipDefinitions);

            // Start the game
            game.Start();

            gameStorageMock
                .Setup(x => x.GetGame(game.Id))
                .Returns(game);

            return game;
        }
    }
}

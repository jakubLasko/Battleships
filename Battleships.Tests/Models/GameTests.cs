using Battleships.Models;
using Battleships.Models.DataTypes;
using Battleships.Models.Enums;
using Battleships.Models.GameSetup;

namespace Battleships.Tests.Models
{
    [TestFixture]
    public class GameTests
    {
        private Player player;
        private Player opponent;
        private Vector2 boardSize;
        private List<ShipDefinition> shipDefinitions;

        [SetUp]
        public void Setup()
        {
            player = new Player("Player1");
            opponent = new Player("Player2");
            boardSize = new Vector2(10, 10);
            shipDefinitions = Common.GetShipDefinitions();
        }

        [Test]
        public void InitializeTest()
        {
            var game = new Game();
            game.Initialize(player, boardSize, shipDefinitions);

            Assert.Multiple(() =>
            {
                Assert.That(game.IsInitialized, Is.True);
                Assert.That(game.State, Is.EqualTo(GameState.WaitingForOpponent));
                Assert.That(game.Player.Name, Is.EqualTo(player.Name));
                Assert.That(game.Opponent, Is.Null);
                Assert.That(game.CurrentTurn, Is.EqualTo(1));
                Assert.That(game.PlayerOnTurn, Is.EqualTo(game.Player));
                Assert.That(game.PlayerBoard.Size, Is.EqualTo(boardSize));
                Assert.That(game.OpponentBoard, Is.Null);
            });
        }

        [Test]
        public void StartTest()
        {
            var game = StartNewGame();

            Assert.That(game.Opponent, Is.EqualTo(opponent));
            Assert.That(game.OpponentBoard.Size, Is.EqualTo(boardSize));
            Assert.That(game.State, Is.EqualTo(GameState.InProgress));
        }

        [Test]
        public void SwitchTurnTest()
        {
            var game = StartNewGame();
            int turn = game.CurrentTurn;

            game.SwitchTurn();

            Assert.Multiple(() =>
            {
                Assert.That(game.PlayerOnTurn, Is.EqualTo(opponent));
                Assert.That(game.CurrentTurn, Is.EqualTo(turn + 1));
            });
        }

        private Game StartNewGame()
        {
            var game = new Game();
            game.Initialize(player, boardSize, shipDefinitions);
            game.Join(opponent, shipDefinitions);

            game.Start();

            return game;
        }
    }
}

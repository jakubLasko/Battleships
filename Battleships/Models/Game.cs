using Battleships.Models.DataTypes;
using Battleships.Models.Enums;
using Battleships.Models.GameIO;
using Battleships.Models.GameSetup;

namespace Battleships.Models
{
    public class Game
    {
        public Guid Id { get; } = Guid.NewGuid();
        public GameState State { get; private set; } = GameState.NotStarted;
        public Player Player { get; private set; }
        public Player Opponent { get; private set; }
        public Board PlayerBoard { get; private set; }
        public Board OpponentBoard { get; private set; }
        public int CurrentTurn { get; private set; }
        public Player PlayerOnTurn { get; private set; }
        public bool IsInitialized { get; private set; }

        public void Initialize(Player player, Player opponent, Vector2 boardSize, List<ShipDefinition> shipDefinitions)
        {
            if (State != GameState.NotStarted)
                throw new InvalidOperationException("Game has already been initialized");

            Player = player ?? throw new ArgumentNullException(nameof(player));
            Opponent = opponent ?? throw new ArgumentNullException(nameof(opponent));

            // Prepare Player board
            PlayerBoard = new Board(boardSize);
            PlayerBoard.PlaceShipsRandomly(shipDefinitions);

            // Prepare Opponent board
            OpponentBoard = new Board(boardSize);
            OpponentBoard.PlaceShipsRandomly(shipDefinitions);

            // Set initial turn
            CurrentTurn = 1;
            PlayerOnTurn = Player;

            IsInitialized = true;
        }

        public void Start()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("Game must be initialized before start.");

            if (State != GameState.NotStarted)
                throw new InvalidOperationException("Game has already started or finished");

            State = GameState.InProgress;
        }

        public void SwitchTurn()
        {
            if (State != GameState.InProgress)
                throw new InvalidOperationException("Game is not in progress");

            CurrentTurn++;
            PlayerOnTurn = PlayerOnTurn == Player ? Opponent : Player;
        }

        public ShotResult Shoot(Vector2 position)
        {
            if (State != GameState.InProgress)
                throw new InvalidOperationException("Game is not in progress");

            Board board = PlayerOnTurn == Player ? OpponentBoard : PlayerBoard;
            ShotState shotState = board.Shoot(position);

            // Check if game is finished
            if (board.AllShipsSunk())
            {
                State = GameState.Finished;
            }
            else
            {
                // Switch turns only on miss
                if (shotState == ShotState.Water)
                {
                    SwitchTurn();
                }
            }

            return new ShotResult(Id.ToString(), shotState, State);
        }
    }
}

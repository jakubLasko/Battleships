using Battleships.Models.DataTypes;
using Battleships.Models.Enums;
using Battleships.Models.GameIO;
using Battleships.Models.GameSetup;

namespace Battleships.Models
{
    /// <summary>
    /// Represents a game instance, managing players, boards, turns, and game state.
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Gets the unique identifier for this game instance.
        /// </summary>
        public Guid Id { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets the current state of the game.
        /// </summary>
        public GameState State { get; private set; } = GameState.NotStarted;

        /// <summary>
        /// Gets the player.
        /// </summary>
        public Player Player { get; private set; }

        /// <summary>
        /// Gets the opponent.
        /// </summary>
        public Player Opponent { get; private set; }

        /// <summary>
        /// Gets the player's game board, which holds their ships.
        /// </summary>
        public Board PlayerBoard { get; private set; }

        /// <summary>
        /// Gets the opponent's game board, which holds their ships.
        /// </summary>
        public Board OpponentBoard { get; private set; }

        /// <summary>
        /// Gets the current turn of the game.
        /// </summary>
        public int CurrentTurn { get; private set; }

        /// <summary>
        /// Gets the player whose turn it currently is in the game.
        /// </summary>
        public Player PlayerOnTurn { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the game has been successfully initialized.
        /// </summary>
        public bool IsInitialized { get; private set; }

        private Vector2 boardSize;

        /// <summary>
        /// Initializes the game with the specified player, board size, and ship definitions.  
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="boardSize">The size of the game board.</param>
        /// <param name="shipDefinitions">A list of ship definitions.</param>
        public void Initialize(Player player, Vector2 boardSize, List<ShipDefinition> shipDefinitions)
        {
            if (State != GameState.NotStarted)
                throw new InvalidOperationException("Game has already been initialized");

            Player = player ?? throw new ArgumentNullException(nameof(player));

            // Prepare Player board
            this.boardSize = boardSize;
            PlayerBoard = new Board(boardSize);
            PlayerBoard.PlaceShipsRandomly(shipDefinitions);

            // Set initial turn
            CurrentTurn = 1;
            PlayerOnTurn = Player;

            IsInitialized = true;
            State = GameState.WaitingForOpponent;
        }

        /// <summary>
        /// Joins the game and initializes opponent board with the specified ships.
        /// </summary>
        /// <param name="opponent">The opponent.</param>
        /// <param name="shipDefinitions">A list of ship definitions.</param>
        public void Join(Player opponent, List<ShipDefinition> shipDefinitions)
        {
            if (!IsInitialized)
                throw new InvalidOperationException("Game must be initialized before start.");

            if (State != GameState.WaitingForOpponent)
                throw new InvalidOperationException("Game has already started or finished.");

            Opponent = opponent ?? throw new ArgumentNullException(nameof(opponent));

            // Prepare Opponent board
            OpponentBoard = new Board(boardSize);
            OpponentBoard.PlaceShipsRandomly(shipDefinitions);

            State = GameState.Ready;
        }

        /// <summary>
        /// Starts the game.
        /// </summary>
        public void Start()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("Game must be initialized before start.");

            if (State != GameState.Ready)
                throw new InvalidOperationException("Game has already started or finished");

            State = GameState.InProgress;
        }

        /// <summary>
        /// Switches the turn to the other player.
        /// </summary>
        public void SwitchTurn()
        {
            if (State != GameState.InProgress)
                throw new InvalidOperationException("Game is not in progress");

            CurrentTurn++;
            PlayerOnTurn = PlayerOnTurn == Player ? Opponent : Player;
        }

        /// <summary>
        /// Shoots at the specified position on the opponent's board.
        /// </summary>
        /// <param name="position">Position to shoot at.</param>
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

            return new ShotResult()
            { 
                GameId = Id.ToString(),
                State = shotState,
                GameState = State
            };
        }
    }
}

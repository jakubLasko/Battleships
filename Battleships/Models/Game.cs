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
        public Player FirstPlayer { get; private set; }
        public Player SecondPlayer { get; private set; }
        public Board Board { get; private set; }
        public int CurrentTurn { get; private set; }
        public Player PlayerOnTurn { get; private set; }
        public bool IsInitialized { get; private set; }

        public void Initialize(Player firstPlayer, Player secondPlayer, Vector2 boardSize, List<ShipTemplate> shipTemplates)
        {
            if (State != GameState.NotStarted)
                throw new InvalidOperationException("Game has already been initialized");

            FirstPlayer = firstPlayer ?? throw new ArgumentNullException(nameof(firstPlayer));
            SecondPlayer = secondPlayer ?? throw new ArgumentNullException(nameof(secondPlayer));

            // Prepare Board
            Board = new Board(boardSize);
            Board.PlaceShipsRandomly(shipTemplates);

            // Set initial turn
            CurrentTurn = 1;
            PlayerOnTurn = FirstPlayer;

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


        // TODO: Players switch turn only if the shot was a miss
        public void SwitchTurn()
        {
            if (State != GameState.InProgress)
                throw new InvalidOperationException("Game is not in progress");

            CurrentTurn++;
            PlayerOnTurn = PlayerOnTurn == FirstPlayer ? SecondPlayer : FirstPlayer;
        }

        public ShotResult Shoot(Vector2 position)
        {
            if (State != GameState.InProgress)
                throw new InvalidOperationException("Game is not in progress");

            // TODO: not sure I wanna go into Board and call Shoot there
            var shotState = Board.Shoot(position);

            // Switch turns only on miss
            if (shotState == ShotState.Water)
            {
                SwitchTurn();
            }
            else
            {
                // Update hits count
                PlayerOnTurn.Hits++;
            }

            // Check if game is finished
            if (Board.AllShipsSunk())
            {
                State = GameState.Finished;

                // TODO: End active game
            }

            return new ShotResult(Id.ToString(), shotState, State);
        }
    }
}

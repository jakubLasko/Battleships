using Battleships.Models;
using Battleships.Models.DataTypes;
using Battleships.Models.Enums;
using Battleships.Models.GameIO;
using Battleships.Services.Interfaces;
using Battleships.Storages.Interfaces;

namespace Battleships.Services
{
    /// <summary>
    /// Provides functionality for main game mechanics.
    /// </summary>
    public class BattleshipsService : IBattleshipsService
    {
        /// <summary>
        /// A logger used to log messages and events.
        /// </summary>
        private readonly ILogger<BattleshipsService> logger;

        /// <summary>
        /// Provides access to ship definitions and related operations.
        /// </summary>
        private readonly IShipsDefinitionService shipsDefinitionService;

        /// <summary>
        /// A Storage used to store and retrieve games.
        /// </summary>
        private readonly IGameStorage gameStorage;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="logger">The logger used to log messages and events.</param>
        /// <param name="shipsDefinitionService">The service for getting ship definitions.</param>
        /// <param name="gameStorage">The storage for storing and retreiving games.</param>
        public BattleshipsService(ILogger<BattleshipsService> logger, IShipsDefinitionService shipsDefinitionService, IGameStorage gameStorage)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.shipsDefinitionService = shipsDefinitionService ?? throw new ArgumentNullException(nameof(shipsDefinitionService));
            this.gameStorage = gameStorage ?? throw new ArgumentNullException(nameof(gameStorage));
        }

        /// <summary>
        /// Asynchronously creates a new game.
        /// </summary>
        /// <param name="data">The configuration data for the game.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task<Game> CreateGameAsync(GameCreateData data, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(data);

            logger.LogDebug($"Starting new game with board size: {data.BoardSizeX}x{data.BoardSizeY}.");

            try
            {
                logger.LogDebug("Loading ship templates.");

                var shipDefinitions = await shipsDefinitionService.LoadShipDefinitionsAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                if (shipDefinitions.Any(s => s.Shape.Count == 0))
                    throw new ArgumentException("Invalid ship template - empty shape");

                logger.LogDebug($"Ship templates loaded: {shipDefinitions.Count}");

                logger.LogDebug("Initializing new game instance.");

                Game game = new Game();
                game.Initialize(data.Player, new Vector2(data.BoardSizeX, data.BoardSizeY), shipDefinitions);

                logger.LogDebug($"Initialization of game {game.Id} finished.");

                gameStorage.AddGame(game);

                logger.LogDebug($"Waiting for opponent for game {game.Id}.");

                return game;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to start new game.");
                throw;
            }
        }

        /// <summary>
        /// Asynchronously joins a player to an existing game.
        /// </summary>
        /// <param name="gameId">The unique identifier of the game to join.</param>
        /// <param name="player">The player attempting to join the game.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task<Game> JoinGameAsync(Guid gameId, Player player, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(gameId);
            ArgumentNullException.ThrowIfNull(player);

            logger.LogDebug($"Joining game {gameId}.");

            try
            {
                logger.LogDebug("Loading ship templates.");

                var shipDefinitions = await shipsDefinitionService.LoadShipDefinitionsAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                if (shipDefinitions.Any(s => s.Shape.Count == 0))
                    throw new ArgumentException("Invalid ship template - empty shape");

                logger.LogDebug($"Ship templates loaded: {shipDefinitions.Count}");

                logger.LogDebug($"Retrieving game {gameId} from storage.");

                Game game = gameStorage.GetGame(gameId);

                logger .LogDebug($"Game {gameId} retrieved. Joining player {player.Name}.");

                game.Join(player, shipDefinitions);

                logger.LogDebug($"Player {player.Name} joined game {gameId}. Game is now ready.");

                logger.LogDebug($"Removing open game {gameId} from storage");

                gameStorage.RemoveOpenGame(gameId);

                return game;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to join game {gameId}.");
                throw;
            }
        }

        /// <summary>
        /// Retrieves a list of games that are currently open and available to join.
        /// </summary>
        public List<Game> GetOpenGames()
        {
            try
            {
                logger.LogDebug("Retrieving open games from storage.");

                List<Game> openGames = gameStorage.GetOpenGames();

                logger.LogDebug($"Open games retrieved: {openGames.Count}");

                return openGames;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to retrieve open games.");
                throw;
            }
        }

        /// <summary>
        /// Retrieves a game by its unique identifier.
        /// </summary>
        /// <param name="gameId">The unique identifier of the game to retrieve.</param>
        public Game GetGame(Guid gameId)
        {
            ArgumentNullException.ThrowIfNull(gameId);

            logger.LogDebug($"Retrieving game {gameId} from storage.");

            try
            {
                Game game = gameStorage.GetGame(gameId);
                logger.LogDebug($"Game {gameId} retrieved successfully.");

                return game;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to retrieve game {gameId}.");
                throw;
            }
        }

        /// <summary>
        /// Processes a shot attempt by a player in the specified game at the given position.
        /// </summary>
        /// <param name="gameId">The unique identifier of the game.</param>
        /// <param name="playerId">The unique identifier of the player attempting the shot.</param>
        /// <param name="position">The coordinates of the shot within the game board.</param>
        /// <returns>A <see cref="ShotResult"/> object representing the outcome of the shot.</returns>
        public ShotResult Shoot(Guid gameId, Guid playerId, Vector2 position)
        {
            try
            {
                logger.LogDebug($"Processing shot at position {position} in game {gameId}.");

                Game game = gameStorage.GetGame(gameId);

                // Validate that it's the player's turn
                if (game.PlayerOnTurn.Id != playerId)
                {
                    var errorMessage = $"Invalid turn. Player {game.PlayerOnTurn.Id} is on turn, but {playerId} attempted to shoot.";
                    logger.LogWarning(errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

                ShotResult result = game.Shoot(position);

                if (game.State == GameState.Finished)
                {
                    logger.LogDebug($"Game {gameId} finished. Winner: {game.PlayerOnTurn.Name}");

                    // Game cleanup
                    gameStorage.RemoveGame(gameId);
                }

                logger.LogDebug($"Shot processed: {result.State} at {position.X},{position.Y} in game {gameId}.");
                logger.LogDebug($"Player on turn: {game.PlayerOnTurn.Name}.");

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error processing shot in game {gameId}.");
                throw;
            }
        }
    }
}

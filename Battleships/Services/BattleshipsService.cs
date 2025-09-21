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
        /// Asynchronously starts a new game.
        /// </summary>
        /// <param name="data">The configuration data for the game.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task<Game> StartGameAsync(GameStartData data, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(data);

            logger.LogDebug($"Starting new game with board size: {data.BoardSizeX}x{data.BoardSizeY}.");

            try
            {
                logger.LogTrace("Loading ship templates.");

                var shipDefinitions = await shipsDefinitionService.LoadShipDefinitionsAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                if (shipDefinitions.Any(s => s.Shape.Count == 0))
                    throw new ArgumentException("Invalid ship template - empty shape");

                logger.LogTrace($"Ship templates loaded: {shipDefinitions.Count}");

                logger.LogTrace("Initializing new game instance.");

                Game game = new Game();
                game.Initialize(data.Player, data.Opponent, new Vector2(data.BoardSizeX, data.BoardSizeY), shipDefinitions);

                logger.LogTrace($"Initialization of game {game.Id} finished.");

                gameStorage.AddGame(game);

                logger.LogTrace($"Starting game {game.Id}.");

                game.Start();

                logger.LogTrace($"Game {game.Id} started successfully.");

                return game;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to start new game.");
                throw;
            }
        }

        /// <summary>
        /// Processes a shot and returns the result of the shot.
        /// </summary>
        /// <param name="data">The data representing the shot.</param>
        /// <returns>A <see cref="ShotResult"/>Outcome of the shot, with states.</returns>
        public ShotResult Shoot(ShootData data)
        {
            try
            {
                Vector2 position = data.Position;

                logger.LogDebug($"Processing shot at position {position} in game {data.GameId}.");

                Game game = gameStorage.GetGame(data.GameId);
                ShotResult result = game.Shoot(data.Position);

                if (game.State == GameState.Finished)
                {
                    logger.LogDebug($"Game {data.GameId} finished. Winner: {game.PlayerOnTurn.Name}");

                    // Game cleanup
                    gameStorage.RemoveGame(data.GameId);
                }

                logger.LogDebug($"Shot processed: {result.State} at {position.X},{position.Y} in game {data.GameId}.");

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error processing shot in game {data.GameId}.");
                throw;
            }
        }
    }
}

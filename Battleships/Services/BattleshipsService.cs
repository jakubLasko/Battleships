using Battleships.Models;
using Battleships.Models.DataTypes;
using Battleships.Models.GameIO;
using Battleships.Services.Interfaces;

namespace Battleships.Services
{
    public class BattleshipsService(
        ILogger<BattleshipsService> logger, 
        IShipsDefinitionService shipsDefinitionService)
        : IBattleshipsService
    {
        protected ILogger<BattleshipsService> Logger { get; } = logger ?? throw new ArgumentNullException(nameof(logger));
        protected IShipsDefinitionService ShipsDefinitionService { get; } = shipsDefinitionService ?? throw new ArgumentNullException(nameof(shipsDefinitionService));

        public async Task<Game> StartGameAsync(GameStartData data, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(data);

            Logger.LogDebug($"Starting new game with board size: {data.BoardSizeX}x{data.BoardSizeY}.");

            try
            {
                Logger.LogTrace("Loading ship templates.");

                var shipTemplates = await ShipsDefinitionService.LoadShipTemplatesAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                if (shipTemplates.Any(s => s.Shape.Count == 0))
                    throw new ArgumentException("Invalid ship template - empty shape");

                Logger.LogTrace($"Ship templates loaded: {shipTemplates.Count}");

                Logger.LogTrace("Initializing new game instance.");

                var game = new Game();
                game.Initialize(data.FirstPlayer, data.SecondPlayer, new Vector2(data.BoardSizeX, data.BoardSizeY), shipTemplates);

                Logger.LogTrace($"Initialization of game {game.Id} finished.");

                // TODO: will have to store the game somewhere - example
                //activeGames[game.Id] = game;

                Logger.LogTrace($"Starting game {game.Id}.");

                game.Start();

                Logger.LogTrace($"Game {game.Id} started successfully.");

                return game;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to start new game.");
                throw;
            }
        }

        public ShotResult Shoot(Guid gameId, Vector2 position)
        {
            // TODO: I have to get active game from some kind of storage

            /*if (!activeGames.TryGetValue(gameId, out var game))
            {
                Logger.LogError($"Game {gameId} not found.");
                throw new KeyNotFoundException("Game not found.");
            }*/

            try
            {
                Logger.LogDebug($"Processing shot at position {position} in game {gameId}.");

                var result = game.Shoot(position);

                Logger.LogInformation("Shot processed: {State} at {X},{Y} in game {GameId}",
                    result.State, position.X, position.Y, gameId);

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error processing shot in game {GameId}", gameId);
                throw;
            }
        }
    }
}

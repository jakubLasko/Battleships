using Battleships.Models;
using Battleships.Models.DataTypes;
using Battleships.Models.GameIO;
using Battleships.Services.Interfaces;
using Battleships.Storages.Interfaces;

namespace Battleships.Services
{
    public class BattleshipsService : IBattleshipsService
    {
        private readonly ILogger<BattleshipsService> logger;
        private readonly IShipsDefinitionService shipsDefinitionService;
        private readonly IGameStorage gameStorage;

        public BattleshipsService(ILogger<BattleshipsService> logger, IShipsDefinitionService shipsDefinitionService, IGameStorage gameStorage)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.shipsDefinitionService = shipsDefinitionService ?? throw new ArgumentNullException(nameof(shipsDefinitionService));
            this.gameStorage = gameStorage ?? throw new ArgumentNullException(nameof(gameStorage));
        }

        public async Task<Game> StartGameAsync(GameStartData data, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(data);

            logger.LogDebug($"Starting new game with board size: {data.BoardSizeX}x{data.BoardSizeY}.");

            try
            {
                logger.LogTrace("Loading ship templates.");

                var shipTemplates = await shipsDefinitionService.LoadShipTemplatesAsync(cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                if (shipTemplates.Any(s => s.Shape.Count == 0))
                    throw new ArgumentException("Invalid ship template - empty shape");

                logger.LogTrace($"Ship templates loaded: {shipTemplates.Count}");

                logger.LogTrace("Initializing new game instance.");

                var game = new Game();
                game.Initialize(data.FirstPlayer, data.SecondPlayer, new Vector2(data.BoardSizeX, data.BoardSizeY), shipTemplates);

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

        public ShotResult Shoot(ShootData data)
        {
            try
            {
                Vector2 position = data.Position;

                logger.LogDebug($"Processing shot at position {position} in game {data.GameId}.");

                Game game = gameStorage.GetGame(data.GameId);
                ShotResult result = game.Shoot(data.Position);

                // TODO: handle end game and turn change logic

                logger.LogInformation($"Shot processed: {result.State} at {position.X},{position.Y} in game {data.GameId}.");

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

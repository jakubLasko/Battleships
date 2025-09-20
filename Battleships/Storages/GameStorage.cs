using Battleships.Models;
using Battleships.Storages.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Battleships.Storages
{
    public class GameStorage : IGameStorage
    {
        private readonly IMemoryCache cache;
        private readonly ILogger<GameStorage> logger;
        private readonly MemoryCacheEntryOptions cacheOptions;

        public GameStorage(IMemoryCache cache, ILogger<GameStorage> logger)
        {
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Set expiration so we don't have any lingering games in memory
            cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(2));
        }

        public Game GetGame(Guid gameId)
        {
            if (!cache.TryGetValue(gameId, out Game? game))
            {
                logger.LogError($"Game {gameId} not found in storage.");
                throw new KeyNotFoundException($"Game {gameId} not found");
            }

            if (game is null)
                throw new InvalidOperationException($"Game {gameId} is null.");

            return game;
        }

        public void AddGame(Game game)
        {
            ArgumentNullException.ThrowIfNull(game);

            cache.Set(game.Id, game, cacheOptions);
            logger.LogDebug($"Game {game.Id} added to storage.");
        }

        public bool RemoveGame(Guid gameId)
        {
            cache.Remove(gameId);
            logger.LogDebug($"Game {gameId} removed from storage.");

            return true;
        }
    }
}

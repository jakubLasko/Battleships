using Battleships.Models;
using Battleships.Storages.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Battleships.Storages
{
    /// <summary>
    /// Provides an in-memory storage mechanism for managing game instances.
    /// </summary>
    public class GameStorage : IGameStorage
    {
        /// <summary>
        /// Represents the memory cache used for storing and retrieving data in memory.
        /// </summary>
        private readonly IMemoryCache cache;

        /// <summary>
        /// A logger used to log messages and events.
        /// </summary>
        private readonly ILogger<GameStorage> logger;

        /// <summary>
        /// Represents the configuration options for a cache entry.
        /// </summary>
        private readonly MemoryCacheEntryOptions cacheOptions;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="cache">The memory cache used to store game data.</param>
        /// <param name="logger">The logger used to log messages and events.</param>
        public GameStorage(IMemoryCache cache, ILogger<GameStorage> logger)
        {
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Set expiration so we don't have any lingering games in memory
            cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(2));
        }

        /// <summary>
        /// Retrieves a game from the cache using the specified identifier.
        /// </summary>
        /// <param name="gameId">The unique identifier of the game to retrieve.</param>
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

        /// <summary>
        /// Adds a game to the storage cache.
        /// </summary>
        /// <param name="game">The game to be added into the storage.</param>
        public void AddGame(Game game)
        {
            ArgumentNullException.ThrowIfNull(game);

            cache.Set(game.Id, game, cacheOptions);
            logger.LogDebug($"Game {game.Id} added to storage.");
        }

        /// <summary>
        /// Removes the game with the specified identifier from the storage.
        /// </summary>
        /// <param name="gameId">The unique identifier of the game to remove.</param>
        public bool RemoveGame(Guid gameId)
        {
            cache.Remove(gameId);
            logger.LogDebug($"Game {gameId} removed from storage.");

            return true;
        }
    }
}

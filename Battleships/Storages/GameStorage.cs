using Battleships.Models;
using Battleships.Models.Enums;
using Battleships.Storages.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

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

        private readonly ConcurrentDictionary<Guid, Game> openGames;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="cache">The memory cache used to store game data.</param>
        /// <param name="logger">The logger used to log messages and events.</param>
        public GameStorage(IMemoryCache cache, ILogger<GameStorage> logger)
        {
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            openGames = new ConcurrentDictionary<Guid, Game>();

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

            logger.LogDebug($"Game {gameId} retrieved from storage.");

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

            if (game.State == GameState.WaitingForOpponent)
            {
                openGames.TryAdd(game.Id, game);
                logger.LogDebug($"Game {game.Id} added to open games list.");
            }

            logger.LogDebug($"Game {game.Id} added to storage.");
        }

        /// <summary>
        /// Removes the game with the specified identifier from the storage.
        /// </summary>
        /// <param name="gameId">The unique identifier of the game to remove.</param>
        public bool RemoveGame(Guid gameId)
        {
            cache.Remove(gameId);
            openGames.TryRemove(gameId, out _);

            logger.LogDebug($"Game {gameId} removed from storage.");

            return true;
        }

        public bool RemoveOpenGame(Guid gameId)
        {
            if (openGames.TryRemove(gameId, out _))
            {
                logger.LogDebug($"Game {gameId} removed from open games list.");
                return true;
            }
            else
            {
                logger.LogError($"Attempted to remove game {gameId} from open games list, but it was not found.");
                return false;
            }
        }

        public List<Game> GetOpenGames()
        {
            return openGames.Values.ToList();
        }
    }
}

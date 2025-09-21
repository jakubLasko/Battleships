using Battleships.Models;

namespace Battleships.Storages.Interfaces
{
    /// <summary>
    /// Game storage interface for managing game instances.
    /// </summary>
    public interface IGameStorage
    {
        /// <summary>
        /// Adds a new game to the collection.
        /// </summary>
        /// <param name="game">The game to add. Cannot be <see langword="null"/>.</param>
        void AddGame(Game game);

        /// <summary>
        /// Gets the game with the specified unique identifier.
        /// </summary>
        /// <param name="gameId">The unique identifier of the game to retrieve.</param>
        Game GetGame(Guid gameId);

        /// <summary>
        /// Removes the game with the specified identifier from the collection.
        /// </summary>
        /// <param name="gameId">The unique identifier of the game to remove.</param>
        bool RemoveGame(Guid gameId);

        /// <summary>
        /// Retrieves a list of games that are currently open and available to join.
        /// </summary>
        List<Game> GetOpenGames();

        /// <summary>
        /// Removes an open game from the collection of available games.
        /// </summary>
        /// <param name="gameId">The unique identifier of the game to remove.</param>
        bool RemoveOpenGame(Guid gameId);
    }
}

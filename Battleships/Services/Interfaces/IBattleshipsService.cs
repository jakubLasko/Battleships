using Battleships.Models;
using Battleships.Models.DataTypes;
using Battleships.Models.GameIO;

namespace Battleships.Services.Interfaces
{
    /// <summary>
    /// Defines the main service, providing methods to start a game and perform shots.
    /// </summary>
    public interface IBattleshipsService
    {
        /// <summary>
        /// Asynchronously creates a new game using the specified game start data.
        /// </summary>
        /// <param name="data">The data required to create the game.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public Task<Game> CreateGameAsync(GameCreateData data, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously joins an existing game with the specified player.
        /// </summary>
        /// <param name="gameId">The unique identifier of the game to join.</param>
        /// <param name="player">The player attempting to join the game.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public Task<Game> JoinGameAsync(Guid gameId, Player player, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the game associated with the gameId.
        /// </summary>
        /// <param name="gameId">The unique identifier of the game to retrieve.</param>
        public Game GetGame(Guid gameId);

        /// <summary>
        /// Retrieves a list of games that are currently open and available to join.
        /// </summary>
        public List<Game> GetOpenGames();

        /// <summary>
        /// Executes a shot based on the provided data and returns the result of the shot.
        /// </summary>
        /// <param name="gameId">The unique identifier of the game.</param>
        /// <param name="playerId">The unique identifier of the player taking the shot.</param>
        /// <param name="position">The target position of the shot.</param>
        public ShotResult Shoot(Guid gameId, Guid playerId, Vector2 position);
    }
}

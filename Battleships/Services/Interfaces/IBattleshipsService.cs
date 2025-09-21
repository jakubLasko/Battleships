using Battleships.Models;
using Battleships.Models.GameIO;

namespace Battleships.Services.Interfaces
{
    /// <summary>
    /// Defines the main service, providing methods to start a game and perform shots.
    /// </summary>
    public interface IBattleshipsService
    {
        /// <summary>
        /// Starts a new game asynchronously using the specified game start data.
        /// </summary>
        /// <param name="data">The data required to initialize the game.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public Task<Game> StartGameAsync(GameStartData data, CancellationToken cancellationToken);

        /// <summary>
        /// Executes a shot based on the provided data and returns the result of the shot.
        /// </summary>
        /// <param name="data">The data describing result of the shot.</param>
        public ShotResult Shoot(ShootData data);
    }
}

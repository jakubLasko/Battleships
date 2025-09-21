
using Battleships.Models.Enums;

namespace Battleships.Models.GameIO
{
    /// <summary>
    /// Represents the result of a shot in a game, including the current game state and the outcome of the shot.
    /// Immutable struct.
    /// </summary>
    public struct ShotResult
    {
        /// <summary>
        /// Gets the unique identifier of the game.
        /// </summary>
        required public string GameId { get; init; }

        /// <summary>
        /// Gets the state of the shot.
        /// </summary>
        required public ShotState State { get; init; }

        /// <summary>
        /// Gets the current state of the game.
        /// </summary>
        required public GameState GameState { get; init; }
    }
}

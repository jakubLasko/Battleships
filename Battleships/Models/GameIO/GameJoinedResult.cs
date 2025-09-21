namespace Battleships.Models.GameIO
{
    /// <summary>
    /// Represents the result of successfully joining a game.
    /// Immutable struct.
    /// </summary>
    public struct GameJoinedResult
    {
        /// <summary>
        /// Gets the unique identifier for the game.
        /// </summary>
        required public string GameId { get; init; }

        /// <summary>
        /// Gets the unique identifier for the player.
        /// </summary>
        required public string PlayerId { get; init; }
    }
}

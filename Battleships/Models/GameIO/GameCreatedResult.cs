namespace Battleships.Models.GameIO
{
    /// <summary>
    /// Represents the result of creating a new game.
    /// Immutable struct.
    /// </summary>
    public struct GameCreatedResult
    {
        /// <summary>
        /// Gets the unique identifier of the game.
        /// </summary>
        required public string GameId { get; init; }

        /// <summary>
        /// Gets the unique identifier for the player.
        /// </summary>
        required public string PlayerId { get; init; }
    }
}

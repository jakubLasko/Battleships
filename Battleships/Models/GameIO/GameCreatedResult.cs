namespace Battleships.Models.GameIO
{
    /// <summary>
    /// Represents the result of creating a new game, including the unique identifier of the created game.
    /// Immutable struct.
    /// </summary>
    public struct GameCreatedResult
    {
        /// <summary>
        /// Gets the unique identifier of the game.
        /// </summary>
        required public string GameId { get; init; }

        required public string PlayerId { get; init; }
    }
}

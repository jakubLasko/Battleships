namespace Battleships.Models.GameIO
{
    /// <summary>
    /// Represents the data required for a player to join a game.
    /// Immutable struct.
    /// </summary>
    public struct GameJoinData
    {
        /// <summary>
        /// Gets the unique identifier for the game.
        /// </summary>
        required public string GameId { get; init; }

        /// <summary>
        /// Gets the player.
        /// </summary>
        required public Player Player { get; init; }
    }
}

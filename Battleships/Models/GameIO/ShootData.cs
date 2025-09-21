using Battleships.Models.DataTypes;

namespace Battleships.Models.GameIO
{
    /// <summary>
    /// Data required to perform a shoot action in an ongoing game of Battleships.
    /// Immutable struct.
    /// </summary>
    public struct ShootData
    {
        /// <summary>
        /// Gets the unique identifier of the game.
        /// </summary>
        required public string GameId { get; init; }

        /// <summary>
        /// Gets the player's ID performing the shot.
        /// </summary>
        required public string PlayerId { get; init; }

        /// <summary>
        /// Gets the position of the object in a 2D space.
        /// </summary>
        required public Vector2 Position { get; init; }
    }
}

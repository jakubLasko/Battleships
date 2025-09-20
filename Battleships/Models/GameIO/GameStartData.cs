using System.ComponentModel.DataAnnotations;

namespace Battleships.Models.GameIO
{
    /// <summary>
    /// Data required to start a new game of Battleships.
    /// Immutable struct.
    /// </summary>
    public struct GameStartData
    {
        /// <summary>
        /// Gets the player.
        /// Required property.
        /// </summary>
        required public Player Player { get; init; }
        required public Player Opponent { get; init; }

        /// <summary>
        /// Gets the opponent.
        /// Required property.
        /// </summary>
        [Range(10, 20)]
        required public int BoardSizeX { get; init; }

        /// <summary>
        /// Need to make sure board has sufficient size to place boats.
        /// </summary>
        [Range(10, 20)]
        required public int BoardSizeY { get; init; }
    }
}

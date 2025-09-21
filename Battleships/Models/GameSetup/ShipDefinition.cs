using Battleships.Models.DataTypes;
using Battleships.Models.Enums;

namespace Battleships.Models.GameSetup
{
    /// <summary>
    /// Represents the definition of a ship in the game.
    /// </summary>
    public class ShipDefinition
    {
        /// <summary>
        /// Gets or sets the type of the ship.
        /// </summary>
        public ShipType Type { get; set; }

        /// <summary>
        /// Gets or sets the collection of points that define the shape.
        /// </summary>
        public List<Vector2> Shape { get; set; } = [];

        /// <summary>
        /// Gets or sets the total number of ships to be spawned.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether rotation is allowed.
        /// </summary>
        public bool AllowRotation { get; set; } = true;
    }
}

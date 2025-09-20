using Battleships.Models.DataTypes;
using Battleships.Models.Enums;
using System.Text.Json.Serialization;

namespace Battleships.Models
{
    /// <summary>
    /// Represents a ship in the game, including its type, position, and hit status.
    /// </summary>
    /// <param name="type">Type of the ship.</param>
    public class Ship(ShipType type)
    {
        /// <summary>
        /// Gets the unique identifier for this ship.
        /// </summary>
        public Guid Id { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets the type of the ship.
        /// </summary>
        public ShipType Type { get; } = type;

        /// <summary>
        /// Gets the collection of cells that form the shape of the ship.
        /// </summary>
        public List<Vector2> Cells { get; } = [];

        /// <summary>
        /// Gets the collection of hits registered on the ship.
        /// </summary>
        public HashSet<Vector2> Hits { get; } = [];

        /// <summary>
        /// Gets a value indicating whether the ship is sunk.
        /// </summary>
        [JsonIgnore]
        public bool IsSunk => Hits.Count >= Cells.Count;
    }
}

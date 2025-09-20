using Battleships.Models.Enums;

namespace Battleships.Models
{
    /// <summary>
    /// Represents a single cell in a grid, with a state and an optional associated ship identifier.
    /// </summary>
    /// <param name="state">Current state of the cell</param>
    /// <param name="shipId">Optional shipId</param>
    public class Cell(CellState state = CellState.Water, Guid? shipId = null)
    {
        /// <summary>
        /// Gets the unique identifier of the ship, or <see langword="null"/> if the ship ID is not set.
        /// </summary>
        public Guid? ShipId { get; } = shipId;

        /// <summary>
        /// Gets or sets the current state of the cell.
        /// </summary>
        public CellState State { get; set; } = state;
    }
}

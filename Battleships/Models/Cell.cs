using Battleships.Models.Enums;

namespace Battleships.Models
{
    public class Cell(CellState state = CellState.Water, Guid? shipId = null)
    {
        public Guid? ShipId { get; } = shipId;
        public CellState State { get; } = state;
    }
}

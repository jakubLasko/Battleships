using Battleships.Models;
using Battleships.Models.DataTypes;
using Battleships.Models.Enums;
using Battleships.Models.GameSetup;

namespace Battleships.Tests
{
    public static class Common
    {
        public static List<ShipDefinition> GetShipDefinitions()
        {
            return new List<ShipDefinition>
            {
                new() { Type = ShipType.Carrier, Shape = new List<Vector2> { new(0, 0), new(1, 0), new(2, 0), new(3, 0), new(4, 0), new(2, 1) }, Count = 1, AllowRotation = true },
                new() { Type = ShipType.Battleship, Shape = new List<Vector2> { new(1, 0), new(1, 1), new(1, 2), new(0, 1), new(2, 1) }, Count = 1, AllowRotation = true },
                new() { Type = ShipType.Cruiser, Shape = new List<Vector2> { new(0, 0), new(1, 0), new(2, 0) }, Count = 1, AllowRotation = true },
                new() { Type = ShipType.Submarine, Shape = new List<Vector2> { new(0, 0), new(1, 0) }, Count = 2, AllowRotation = true },
                new() { Type = ShipType.Destroyer, Shape = new List<Vector2> { new(0, 0) }, Count = 2, AllowRotation = true }
            };
        }

        public static Vector2 FindWaterCell(Cell[,] grid)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    if (grid[x, y].State == CellState.Water)
                    {
                        return new Vector2(x, y);
                    }
                }
            }

            throw new InvalidOperationException("No water cell found on the board");
        }
    }
}

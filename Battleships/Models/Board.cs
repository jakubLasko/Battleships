using Battleships.Models.DataTypes;
using Battleships.Models.Enums;
using Battleships.Models.GameSetup;

namespace Battleships.Models
{
    /// <summary>
    /// Represents a game board.
    /// Manages the grid, ship placement, and shooting mechanics.
    /// </summary>
    public class Board
    {
        /// <summary>
        /// Gets the size of the board.
        /// </summary>
        public Vector2 Size { get; }

        /// <summary>
        /// Gets the two-dimensional array representing the grid of cells.
        /// </summary>
        public Cell[,] Grid { get; }

        /// <summary>
        /// Gets the collection of ships currently on the board.
        /// </summary>
        public List<Ship> Ships { get; } = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="Board"/> class with the specified board size.
        /// </summary>
        /// <param name="boardSize">The dimensions of the board, represented as a <see cref="Vector2"/></param>
        public Board(Vector2 boardSize)
        {
            Size = boardSize;
            Grid = new Cell[Size.X, Size.Y];

            for (int x = 0; x < Size.X; x++)
            {
                for (int y = 0; y < Size.Y; y++)
                {
                    Grid[x, y] = new Cell();
                }
            }
        }

        #region Public methods

        /// <summary>
        /// Determines whether all ships on the board have been sunk.
        /// </summary>
        public bool AllShipsSunk() => Ships.All(s => s.IsSunk);

        /// <summary>
        /// Places ships randomly on the grid based on the provided ship templates.
        /// </summary>
        /// <param name="shipTemplates">A list of ship definitions to be placed on the board</param>
        public void PlaceShipsRandomly(List<ShipDefinition> shipTemplates)
        {
            Random rng = new Random();

            foreach (var shipTemplate in shipTemplates)
            {
                int placed = 0;
                for (int attempt = 0; attempt < 1000 && placed < shipTemplate.Count; attempt++)
                {
                    // Only apply rotation if allowed
                    int rotation = shipTemplate.AllowRotation ? rng.Next(4) * 90 : 0;
                    var rotatedShape = RotateShape(shipTemplate.Shape, rotation);

                    // Calculate bounds of the rotated shape
                    int minX = rotatedShape.Min(c => c.X);
                    int maxX = rotatedShape.Max(c => c.X);
                    int minY = rotatedShape.Min(c => c.Y);
                    int maxY = rotatedShape.Max(c => c.Y);

                    // Adjust placement range based on shape bounds
                    int width = maxX - minX + 1;
                    int height = maxY - minY + 1;

                    // Normalize the shape to start from 0,0
                    var normalizedShape = rotatedShape.Select(p => new Vector2 { X = p.X - minX, Y = p.Y - minY }).ToList();

                    // Try to place within valid bounds
                    int originX = rng.Next(Size.X - width + 1);
                    int originY = rng.Next(Size.Y - height + 1);
                    var origin = new Vector2 { X = originX, Y = originY };

                    if (CanPlaceShip(this, normalizedShape, origin))
                    {
                        var ship = new Ship(shipTemplate.Type);
                        foreach (var relCell in normalizedShape)
                        {
                            var cellPos = new Vector2
                            {
                                X = origin.X + relCell.X,
                                Y = origin.Y + relCell.Y
                            };

                            ship.Cells.Add(cellPos);
                            Grid[cellPos.X, cellPos.Y] = new Cell(CellState.Ship, ship.Id);
                        }

                        Ships.Add(ship);
                        placed++;
                    }
                }

                if (placed == 0)
                    throw new Exception($"Could not place ship of type {shipTemplate.Type}.");
            }
        }

        /// <summary>
        /// Fires a shot at the specified position on the game board and updates the state of the targeted cell.
        /// </summary>
        /// <param name="position">The coordinates of the cell to target</param>
        public ShotState Shoot(Vector2 position)
        {
            if (position.X < 0 || position.X >= Size.X || position.Y < 0 || position.Y >= Size.Y)
                throw new ArgumentOutOfRangeException(nameof(position), "Shot position is outside the board.");

            // Lookup cell
            Cell cell = Grid[position.X, position.Y];

            switch (cell.State)
            {
                case CellState.Water:
                    {
                        cell.State = CellState.Miss;
                        return ShotState.Water;
                    }
                case CellState.Ship:
                    {
                        cell.State = CellState.Hit;

                        Ship ship = Ships.First(s => s.Id == cell.ShipId);
                        ship.Hits.Add(position);

                        return ship.IsSunk ? ShotState.ShipSunk : ShotState.Hit;
                    }
                case CellState.Hit:
                case CellState.Miss:
                    throw new InvalidOperationException("This position was already shot at.");
                default:
                    throw new InvalidOperationException("Unknown cell state.");
            }
        }

        #endregion Public methods

        #region Helper methods

        // Rotation helper
        private List<Vector2> RotateShape(List<Vector2> shape, int rotation)
        {
            return shape.Select(cell =>
            {
                return rotation switch
                {
                    0 => new Vector2 { X = cell.X, Y = cell.Y },
                    90 => new Vector2 { X = -cell.Y, Y = cell.X },
                    180 => new Vector2 { X = -cell.X, Y = -cell.Y },
                    270 => new Vector2 { X = cell.Y, Y = -cell.X },
                    _ => cell
                };
            }).ToList();
        }

        // Placement validation
        private bool CanPlaceShip(Board board, List<Vector2> shape, Vector2 origin)
        {
            int width = board.Size.X;
            int height = board.Size.Y;

            // Collect all cells the ship would occupy
            var shipCells = shape.Select(cell => new Vector2 { X = origin.X + cell.X, Y = origin.Y + cell.Y }).ToList();

            // Check bounds
            if (shipCells.Any(c => c.X < 0 || c.Y < 0 || c.X >= width || c.Y >= height))
                return false;

            // Check for overlap and gap
            foreach (var cell in shipCells)
            {
                // Check the 3x3 area around each cell for other ships
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        int nx = cell.X + dx;
                        int ny = cell.Y + dy;
                        if (nx >= 0 && ny >= 0 && nx < width && ny < height)
                        {
                            if (board.Grid[nx, ny].State == CellState.Ship)
                                return false;
                        }
                    }
                }
            }

            return true;
        }

        #endregion Helper methods
    }
}

using Battleships.Models;
using Battleships.Models.DataTypes;
using Battleships.Models.Enums;
using Battleships.Models.GameSetup;

namespace Battleships.Tests.Models
{
    [TestFixture]
    public class BoardTests
    {
        private List<ShipDefinition> shipDefinitions;
        private Vector2 boardSize;

        [SetUp]
        public void Setup()
        {
            boardSize = new Vector2(10, 10);
            shipDefinitions = Common.GetShipDefinitions();
        }

        [Test]
        public void InitialStateTest()
        {
            var board = new Board(boardSize);
            Assert.Multiple(() =>
            {
                Assert.That(board.Size, Is.EqualTo(boardSize));
                Assert.That(board.Ships, Is.Empty);

                // Check all cells are initialized as water
                for (int x = 0; x < board.Size.X; x++)
                {
                    for (int y = 0; y < board.Size.Y; y++)
                    {
                        Assert.That(board.Grid[x, y].State, Is.EqualTo(CellState.Water));
                        Assert.That(board.Grid[x, y].ShipId, Is.Null);
                    }
                }
            });
        }

        [Test]
        public void ShipPlacementTest()
        {
            var board = new Board(boardSize);
            board.PlaceShipsRandomly(shipDefinitions);

            Assert.Multiple(() =>
            {
                Assert.That(board.Ships, Has.Count.EqualTo(shipDefinitions.Sum(x => x.Count)));

                foreach (var shipType in Enum.GetValues<ShipType>())
                {
                    var definition = shipDefinitions.FirstOrDefault(s => s.Type == shipType);
                    if (definition != null)
                    {
                        Assert.That(board.Ships.Count(s => s.Type == shipType), Is.EqualTo(definition.Count));

                        foreach (var ship in board.Ships.Where(s => s.Type == shipType))
                            Assert.That(ship.Cells.Count, Is.EqualTo(definition.Shape.Count));
                    }
                }
            });
        }

        [Test]
        public void GapsBetweenShipsTest()
        {
            var board = new Board(boardSize);
            board.PlaceShipsRandomly(shipDefinitions);

            foreach (var ship in board.Ships)
            {
                foreach (var cell in ship.Cells)
                {
                    // Check surrounding cells
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if (dx == 0 && dy == 0) 
                                continue;

                            var adjacentPos = new Vector2(cell.X + dx, cell.Y + dy);

                            if (adjacentPos.X >= 0 && adjacentPos.X < board.Size.X &&
                                adjacentPos.Y >= 0 && adjacentPos.Y < board.Size.Y)
                            {
                                // Skip cells that belong to the same ship
                                if (!ship.Cells.Contains(adjacentPos))
                                    Assert.That(board.Grid[adjacentPos.X, adjacentPos.Y].State, Is.Not.EqualTo(CellState.Ship));
                            }
                        }
                    }
                }
            }
        }
    }
}

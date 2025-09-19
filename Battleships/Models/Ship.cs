using Battleships.Models.DataTypes;
using Battleships.Models.Enums;
using System.Text.Json.Serialization;

namespace Battleships.Models
{
    public class Ship(ShipType type)
    {
        public Guid Id { get; } = Guid.NewGuid();
        public ShipType Type { get; } = type;
        public List<Vector2> Cells { get; } = [];
        public HashSet<Vector2> Hits { get; } = [];

        [JsonIgnore]
        public bool IsSunk => Hits.Count >= Cells.Count;

        public int GetCurrentSize() => Math.Clamp(Cells.Count - Hits.Count, 0, int.MaxValue);
    }
}

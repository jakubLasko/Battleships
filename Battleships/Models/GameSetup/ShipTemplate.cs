using Battleships.Models.DataTypes;
using Battleships.Models.Enums;

namespace Battleships.Models.GameSetup
{
    public class ShipTemplate
    {
        public ShipType Type { get; set; }
        public List<Vector2> Shape { get; set; } = [];
        public int Count { get; set; }
        public bool AllowRotation { get; set; } = true;
    }
}

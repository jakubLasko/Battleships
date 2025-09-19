using Battleships.Models.DataTypes;
using System.ComponentModel.DataAnnotations;

namespace Battleships.Models.GameIO
{
    public struct ShootData(Guid gameId, Vector2 position)
    {
        [Required]
        public Guid GameId { get; } = gameId;
        [Required]
        public Vector2 Position { get; } = position;
    }
}

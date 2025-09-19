
using Battleships.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Battleships.Models.GameIO
{
    public struct ShotResult(string gameId, ShotState state, GameState gameState)
    {
        [Required]
        public string GameId { get; } = gameId;
        [Required]
        public ShotState State { get; } = state;
        [Required]
        public GameState GameState { get; } = gameState;
    }
}

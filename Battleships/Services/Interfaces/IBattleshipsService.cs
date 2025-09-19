using Battleships.Models;
using Battleships.Models.DataTypes;
using Battleships.Models.GameIO;

namespace Battleships.Services.Interfaces
{
    public interface IBattleshipsService
    {
        public Task<Game> StartGameAsync(GameStartData data, CancellationToken cancellationToken);
        public ShotResult Shoot(Guid gameId, Vector2 position);
    }
}

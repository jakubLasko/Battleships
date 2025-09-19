using Battleships.Models;

namespace Battleships.Storages.Interfaces
{
    public interface IGameStorage
    {
        void AddGame(Game game);
        Game GetGame(Guid gameId);
        bool RemoveGame(Guid gameId);
        void CleanupInactiveGames();
    }
}

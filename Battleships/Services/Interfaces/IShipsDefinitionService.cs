using Battleships.Configuration.Entities;

namespace Battleships.Services.Interfaces
{
    public interface IShipsDefinitionService
    {
        public Task<List<ShipTemplate>> LoadShipTemplatesAsync(CancellationToken cancellationToken);
    }
}

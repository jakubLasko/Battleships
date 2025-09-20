using Battleships.Models.GameSetup;

namespace Battleships.Services.Interfaces
{
    public interface IShipsDefinitionService
    {
        public Task<List<ShipDefinition>> LoadShipDefinitionsAsync(CancellationToken cancellationToken);
    }
}

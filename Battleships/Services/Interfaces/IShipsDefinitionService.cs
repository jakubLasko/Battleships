using Battleships.Configs;

namespace Battleships.Services.Interfaces
{
    public interface IShipsDefinitionService
    {
        public List<ShipTemplate> GetShipTemplates();
    }
}

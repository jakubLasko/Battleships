using Battleships.Models.GameSetup;

namespace Battleships.Services.Interfaces
{
    /// <summary>
    /// Provides functionality to load and retrieve ship definitions.
    /// </summary>
    public interface IShipsDefinitionService
    {
        /// <summary>
        /// Asynchronously loads a list of ship definitions.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        public Task<List<ShipDefinition>> LoadShipDefinitionsAsync(CancellationToken cancellationToken);
    }
}

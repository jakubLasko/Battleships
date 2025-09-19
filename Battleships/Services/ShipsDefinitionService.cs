using Battleships.Configuration.Entities;
using Battleships.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Battleships.Services
{
    public class ShipsDefinitionService(
        ILogger<ShipsDefinitionService> logger,
        IOptions<AppSettings> appSettings) 
        : IShipsDefinitionService
    {
        protected ILogger<ShipsDefinitionService> Logger { get; } = logger ?? throw new ArgumentNullException(nameof(logger));
        protected IOptions<AppSettings> AppSettings { get; } = appSettings ?? throw new ArgumentNullException(nameof(appSettings));

        public async Task<List<ShipTemplate>> LoadShipTemplatesAsync(CancellationToken cancellationToken)
        {
            var shipsConfigPath = AppSettings.Value.ShipsConfigPath;
            if (!File.Exists(shipsConfigPath))
            {
                Logger.LogError("Ships config file not found at path: {ShipsConfigPath}", shipsConfigPath);
                throw new FileNotFoundException($"Ships config file not found at path: {shipsConfigPath}");
            }
            try
            {
                // Using guarantees disposal of the stream
                await using var fileStream = File.OpenRead(shipsConfigPath);

                // Deserialize the JSON content to a list of ShipTemplate objects
                var shipTemplates = await JsonSerializer.DeserializeAsync<List<ShipTemplate>>(
                    fileStream,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                    cancellationToken
                );

                if (shipTemplates == null || shipTemplates.Count == 0)
                {
                    Logger.LogError("No ship templates found in config file at path: {ShipsConfigPath}", shipsConfigPath);
                    throw new Exception($"No ship templates found in config file at path: {shipsConfigPath}");
                }

                return shipTemplates;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error reading or deserializing ships config file at path: {ShipsConfigPath}", shipsConfigPath);
                throw;
            }
        }
    }
}

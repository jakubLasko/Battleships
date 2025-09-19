using Battleships.Models.GameSetup;
using Battleships.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Battleships.Services
{
    public class ShipsDefinitionService : IShipsDefinitionService
    {
        private readonly ILogger<ShipsDefinitionService> logger;
        private readonly IOptions<AppSettings> appSettings;

        public ShipsDefinitionService(ILogger<ShipsDefinitionService> logger, IOptions<AppSettings> appSettings)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        public async Task<List<ShipTemplate>> LoadShipTemplatesAsync(CancellationToken cancellationToken)
        {
            var shipsConfigPath = appSettings.Value.ShipsTemplatesPath;
            if (!File.Exists(shipsConfigPath))
            {
                logger.LogError($"Ships config file not found at path: {shipsConfigPath}.");
                throw new FileNotFoundException($"Ships config file not found at path: {shipsConfigPath}.");
            }
            try
            {
                // Using guarantees disposal of the stream
                await using FileStream fileStream = File.OpenRead(shipsConfigPath);

                // Deserialize the JSON content to a list of ShipTemplate objects
                List<ShipTemplate>? shipTemplates = await JsonSerializer.DeserializeAsync<List<ShipTemplate>>(
                    fileStream,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                    cancellationToken
                );

                if (shipTemplates == null || shipTemplates.Count == 0)
                {
                    logger.LogError($"No ship templates found in config file at path: {shipsConfigPath}.");
                    throw new Exception($"No ship templates found in config file at path: {shipsConfigPath}.");
                }

                return shipTemplates;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error reading or deserializing ships config file at path: {shipsConfigPath}.");
                throw;
            }
        }
    }
}

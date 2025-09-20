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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        /// Note: This is wasteful since we have to read and deserialize the file every time.
        /// Better approach would be to read it once and cache the result, but this is simpler for now.
        public async Task<List<ShipDefinition>> LoadShipDefinitionsAsync(CancellationToken cancellationToken)
        {
            var shipsConfigPath = appSettings.Value.ShipsDefinitionPath;
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
                List<ShipDefinition>? shipTemplates = await JsonSerializer.DeserializeAsync<List<ShipDefinition>>(
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

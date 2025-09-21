using Battleships.Models.GameSetup;
using Battleships.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Battleships.Services
{
    /// <summary>
    /// Provides functionality for loading ship definitions from a configuration file.
    /// </summary>
    public class ShipsDefinitionService : IShipsDefinitionService
    {
        /// <summary>
        /// A logger used to log messages and events.
        /// </summary>
        private readonly ILogger<ShipsDefinitionService> logger;

        /// <summary>
        /// Represents the application settings configuration options.
        /// </summary>
        private readonly IOptions<AppSettings> appSettings;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="logger">The logger used to log messages and events.</param>
        /// <param name="appSettings">The application settings containing configuration values for the service.</param>
        public ShipsDefinitionService(ILogger<ShipsDefinitionService> logger, IOptions<AppSettings> appSettings)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        /// <summary>
        /// Asynchronously loads a list of ship definitions from a JSON configuration file.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
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
                logger.LogDebug($"Loading ship definitions from config file at path: {shipsConfigPath}.");

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

                logger.LogDebug($"Successfully loaded {shipTemplates.Count} ship definitions from config file.");

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

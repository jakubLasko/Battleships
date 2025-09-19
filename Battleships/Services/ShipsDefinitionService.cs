using Battleships.Configs;
using Battleships.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Battleships.Services
{
    public class ShipsDefinitionService(
        ILogger<ShipsDefinitionService> logger,
        IOptions<AppSettings> appSettings) 
        : IShipsDefinitionService
    {
        protected ILogger<ShipsDefinitionService> Logger { get; } = logger ?? throw new ArgumentNullException(nameof(logger));
        protected IOptions<AppSettings> AppSettings { get; } = appSettings ?? throw new ArgumentNullException(nameof(appSettings));

        public List<ShipTemplate> GetShipTemplates()
        {
            var shipsConfigPath = AppSettings.Value.ShipsConfigPath;
            if (!File.Exists(shipsConfigPath))
            {
                Logger.LogError("Ships config file not found at path: {ShipsConfigPath}", shipsConfigPath);
                throw new FileNotFoundException($"Ships config file not found at path: {shipsConfigPath}");
            }
            try
            {
                var json = File.ReadAllText(shipsConfigPath);
                var shipTemplates = System.Text.Json.JsonSerializer.Deserialize<List<ShipTemplate>>(json);
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

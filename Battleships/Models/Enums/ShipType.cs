using System.Text.Json.Serialization;

namespace Battleships.Models.Enums
{
    /// <summary>
    /// Defines the types of ships available in the game.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ShipType
    {
        Carrier,    
        Battleship, 
        Cruiser,    
        Submarine,
        Destroyer
    }
}

using System.Text.Json.Serialization;

namespace Battleships.Models.Enums
{
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

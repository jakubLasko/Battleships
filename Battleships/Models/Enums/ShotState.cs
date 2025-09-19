using System.Text.Json.Serialization;

namespace Battleships.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ShotState
    {
        Water,
        Hit,
        ShipSunk
    }
}

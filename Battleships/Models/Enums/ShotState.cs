using System.Text.Json.Serialization;

namespace Battleships.Models.Enums
{
    /// <summary>
    /// Defines the result of a shot fired in the game.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ShotState
    {
        Water,
        Hit,
        ShipSunk
    }
}

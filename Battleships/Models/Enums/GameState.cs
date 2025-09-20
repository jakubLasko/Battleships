using System.Text.Json.Serialization;

namespace Battleships.Models.Enums
{
    /// <summary>
    /// Defines the current state of the game.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GameState
    {
        NotStarted,
        InProgress,
        Finished
    }
}

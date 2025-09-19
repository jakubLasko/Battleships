using System.Text.Json.Serialization;

namespace Battleships.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GameState
    {
        NotStarted,
        InProgress,
        Finished
    }
}

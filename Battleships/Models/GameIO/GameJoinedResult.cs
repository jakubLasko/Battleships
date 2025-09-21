namespace Battleships.Models.GameIO
{
    public struct GameJoinedResult
    {
        required public string GameId { get; init; }

        required public string PlayerId { get; init; }
    }
}

namespace Battleships.Models.GameIO
{
    public struct GameJoinData
    {
        required public string GameId { get; init; }
        required public Player Player { get; init; }
    }
}

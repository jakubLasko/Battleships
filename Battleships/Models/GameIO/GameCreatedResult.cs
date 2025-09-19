namespace Battleships.Models.GameIO
{
    /// <summary>
    /// Represents the result of creating a new game, including the unique identifier of the created game.
    /// </summary>
    /// <param name="gameId">ID of created game</param>
    public struct GameCreatedResult(string gameId)
    {
        public string GameId { get; } = gameId;
    }
}

using System.ComponentModel.DataAnnotations;

namespace Battleships.Models.GameIO
{
    /// <summary>
    /// Data for starting a new game.
    /// Shallow immutable struct.
    /// </summary>
    /// <param name="firstPlayer"></param>
    /// <param name="secondPlayer"></param>
    /// <param name="boardSizeX"></param>
    /// <param name="boardSizeY"></param>
    public struct GameStartData(
        [Required] Player firstPlayer,
        [Required] Player secondPlayer,
        [Range(10, 100)] int boardSizeX,
        [Range(10, 100)] int boardSizeY)
    {
        public Player FirstPlayer { get; } = firstPlayer;
        public Player SecondPlayer { get; } = secondPlayer;

        /// <summary>
        /// Need to make sure board has sufficient size to place boats.
        /// </summary>
        public int BoardSizeX { get; } = boardSizeX;

        /// <summary>
        /// Need to make sure board has sufficient size to place boats.
        /// </summary>
        public int BoardSizeY { get; } = boardSizeY;
    }
}

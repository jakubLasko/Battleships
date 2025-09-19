using System.ComponentModel.DataAnnotations;

namespace Battleships.Models.GameIO
{
    // TODO: make this immutable
    public struct GameStartData()
    {
        [Required]
        public Player FirstPlayer { get; set; }
        [Required]
        public Player SecondPlayer { get; set; }

        /// <summary>
        /// Need to make sure board has sufficient size to place boats.
        /// </summary>
        [Range(10, 50)]
        public int BoardSizeX { get; set; }

        /// <summary>
        /// Need to make sure board has sufficient size to place boats.
        /// </summary>
        [Range(10, 50)]
        public int BoardSizeY { get; set; }
    }
}

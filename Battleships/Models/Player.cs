using System.ComponentModel.DataAnnotations;

namespace Battleships.Models
{
    /// <summary>
    /// Represents a player with a name.
    /// <param name="name">Name of the player</param>
    public class Player(string name)
    {
        /// <summary>
        /// Gets or sets the name associated with the player.
        /// </summary>
        [Required]
        public string Name { get; set; } = name;
    }
}

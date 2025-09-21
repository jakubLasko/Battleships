using System.ComponentModel.DataAnnotations;

namespace Battleships.Models
{
    /// <summary>
    /// Represents a player with a name.
    /// <param name="name">Name of the player</param>
    public class Player(string name)
    {
        /// <summary>
        /// Gets the unique identifier for this player.
        /// </summary>
        public Guid Id { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the name associated with the player.
        /// </summary>
        [Required]
        public string Name { get; set; } = name;
    }
}

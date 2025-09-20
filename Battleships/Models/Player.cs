using System.ComponentModel.DataAnnotations;

namespace Battleships.Models
{
    public class Player
    {
        [Required]
        public string Name { get; set; }
    }
}

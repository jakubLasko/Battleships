using Battleships.Models.DataTypes;
using Battleships.Models.GameIO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Battleships.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BattleshipsController(ILogger<BattleshipsController> logger) : ControllerBase
    {
        protected ILogger<BattleshipsController> Logger { get; } = logger;

        [HttpPost]
        [ActionName("game/start")]
        public ActionResult<GameCreatedResult> StartGame([FromBody] GameStartData data)
        {
            ArgumentNullException.ThrowIfNull(data);

            return Ok(new GameCreatedResult("Test"));
        }

        [HttpPut]
        [ActionName("game/shoot/{gameId}")]
        public ActionResult<ShootResult> Shoot([FromRoute][Required] string gameId, [FromBody][Required] Position position)
        {
            ArgumentNullException.ThrowIfNull(gameId);
            ArgumentNullException.ThrowIfNull(position);

            return Ok(new ShootResult("Test"));
        }
    }
}

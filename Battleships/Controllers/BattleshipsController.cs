using Battleships.Models;
using Battleships.Models.DataTypes;
using Battleships.Models.GameIO;
using Battleships.Services;
using Battleships.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Battleships.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BattleshipsController : ControllerBase
    {
        private readonly ILogger<BattleshipsController> logger;
        private readonly IBattleshipsService battleshipsService;

        public BattleshipsController(ILogger<BattleshipsController> logger, IBattleshipsService battleshipsService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.battleshipsService = battleshipsService ?? throw new ArgumentNullException(nameof(battleshipsService));
        }

        [HttpPost]
        [ActionName("game/start")]
        public async Task<ActionResult<GameCreatedResult>> StartGameAsync([FromBody] GameStartData data, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(data);

            if (data.FirstPlayer is null || string.IsNullOrWhiteSpace(data.FirstPlayer.Name))
            {
                logger.LogError("First player is missing or has no name.");
                return BadRequest("First player is required and must have a name.");
            }

            if (data.SecondPlayer is null || string.IsNullOrWhiteSpace(data.SecondPlayer.Name))
            {
                logger.LogError("Second player is missing or has no name.");
                return BadRequest("Second player is required and must have a name.");
            }

            try
            {
                Game game = await battleshipsService.StartGameAsync(data, cancellationToken);
                if (game == null)
                {
                    logger.LogError("Game creation failed.");
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create game.");
                }

                return Ok(new GameCreatedResult(game.Id.ToString()));
            }
            catch (OperationCanceledException)
            {
                logger.LogError("Game creation was canceled.");
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "Game creation was canceled.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception occurred while starting a new game.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while starting the game.");
            }
        }

        [HttpPut]
        [ActionName("game/shoot/{gameId}")]
        public ActionResult<ShootResult> Shoot([FromRoute][Required] string gameId, [FromBody][Required] Vector2 position)
        {
            ArgumentNullException.ThrowIfNull(gameId);
            ArgumentNullException.ThrowIfNull(position);

            // Shoot can be sync since it does we quick operation on in-memory data

            return Ok(new ShootResult("Test"));
        }
    }
}

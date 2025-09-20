using Battleships.Models;
using Battleships.Models.DataTypes;
using Battleships.Models.GameIO;
using Battleships.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Battleships.Controllers
{
    [Route("api/[controller]/[action]")]
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

        /// <summary>
        /// Stars a new game of Battleships.
        /// </summary>
        /// <param name="data">GameStartData required for starting a new game.</param>
        /// <param name="cancellationToken">The cancellation token used to prevent hanging.</param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("game/start")]
        public async Task<ActionResult<GameCreatedResult>> StartGameAsync([FromBody] GameStartData data, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(data);

            if (data.Player is null || string.IsNullOrWhiteSpace(data.Player.Name))
            {
                logger.LogError("First player is missing or has no name.");
                return BadRequest("First player is required and must have a name.");
            }

            if (data.Opponent is null || string.IsNullOrWhiteSpace(data.Opponent.Name))
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

                return Ok(new GameCreatedResult() { GameId = game.Id.ToString() });
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

        /// <summary>
        /// Processes a shot attempt in the specified game at the given position.
        /// </summary>
        /// <param name="gameId">The unique identifier of the game.</param>
        /// <param name="position">The coordinates of the shot. Must be within the valid board boundaries.</param>
        /// <returns>ShotResult with result of the shot and game state.</returns>
        [HttpPut]
        [ActionName("game/shoot/{gameId}")]
        public ActionResult<ShotResult> Shoot([FromRoute][Required] string gameId, [FromBody][Required] Vector2 position)
        {
            ArgumentNullException.ThrowIfNull(gameId);
            ArgumentNullException.ThrowIfNull(position);

            // Shoot can be sync since it does only quick operation on in-memory data
            try
            {
                if (!Guid.TryParse(gameId, out Guid gameGuid))
                {
                    logger.LogError($"Invalid game ID format: {gameId}");
                    return BadRequest("Invalid game ID format");
                }

                ShotResult result = battleshipsService.Shoot(new ShootData()
                { 
                    GameId = gameGuid,
                    Position = position
                });

                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                logger.LogError($"Game not found: {gameId}");
                return NotFound("Game not found");
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning($"Invalid shot attempt: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                logger.LogWarning($"Shot outside board: {ex.Message}");
                return BadRequest("Shot position is outside the board");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing shot");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the shot");
            }
        }
    }
}

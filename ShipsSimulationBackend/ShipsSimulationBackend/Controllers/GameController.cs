using Microsoft.AspNetCore.Mvc;
using ShipsSimulationBackend.Models;

namespace ShipsSimulationBackend.Controllers;

[ApiController]
[Route("game")]
public class GameController : ControllerBase
{
    private readonly Game _game;
    public GameController()
    {
        _game = new Game();
    }

    [HttpPost("simulate")]
    public ActionResult<GameResult> Simulate()
    {
        _game.Play();
        return Ok(GameResult.FromGame(_game));
    }
}
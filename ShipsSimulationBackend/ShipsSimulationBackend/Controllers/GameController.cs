using Microsoft.AspNetCore.Mvc;
using ShipsSimulationBackend.Models;

namespace ShipsSimulationBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class GameController : ControllerBase
{
    private readonly Game _game;
    public GameController()
    {
        _game = new Game();
    }

    [HttpPost("play")]
    public IActionResult Play()
    {
        _game.Play();
        //todo return result
        return Ok();
    }
}
using Microsoft.AspNetCore.Mvc;

namespace ShipsSimulationBackend.Controllers;

[ApiController]
[Route("[controller]")]
public class GameController : ControllerBase
{
    public GameController()
    {
        // todo initialization
    }

    [HttpPost("play")]
    public IActionResult Play()
    {
        // todo method to begin the game
        return Ok();
    }

    [HttpGet("result")]
    public IActionResult GetResult()
    {
        // todo method to return result of the game { winner, boards final fields }
        return Ok();
    }
}
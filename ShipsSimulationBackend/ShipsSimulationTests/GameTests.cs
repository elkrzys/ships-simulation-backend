using ShipsSimulationBackend.Models;

namespace ShipsSimulationTests;

public class GameTests
{
    [Test]
    public void MustSimulateGame()
    {
        //given
        var game = new Game();
        
        //when
        game.Play();
        
        //then
        Assert.NotNull(game.Beginner);
        Assert.NotNull(game.Winner);
        
        if (game.Winner == "Player 1")
        {
            Assert.False(game.Player1.IsLost);
            Assert.True(game.Player2.IsLost);
        }
        else
        {
            Assert.False(game.Player2.IsLost);
            Assert.True(game.Player1.IsLost);
        }
    }
}
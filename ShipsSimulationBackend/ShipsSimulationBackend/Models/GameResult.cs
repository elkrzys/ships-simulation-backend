namespace ShipsSimulationBackend.Models;

public class GameResult
{
    public string Beginner { get; set; }
    
    public string Winner { get; set; }

    public Player Player1 { get; set; }
    
    public Player Player2 { get; set; }

    public int TotalRounds { get; set; }

    public static GameResult FromGame(Game game) => new()
    {
        Beginner = game.Beginner,
        Winner = game.Winner,
        Player1 = game.Player1,
        Player2 = game.Player2,
        TotalRounds = game.TotalRounds
    };

}
namespace ShipsSimulationBackend.Models;

public class Game
{
    public Player Player1 { get; }
    public Player Player2 { get; }

    public string Beginner;

    public string Winner;

    public int TotalRounds;

    private int _roundPlayer;

    public Game()
    {
        Player1 = new Player();
        Player2 = new Player();
    }

    public void Play()
    {
        Player1.PlaceShipsRandomly();
        Player2.PlaceShipsRandomly();
        SetBeginner();

        while (!Player1.IsLost && !Player2.IsLost)
        {
            if (_roundPlayer == 0)
            {
                ++TotalRounds;
                while (true)
                {
                    var firePosition = Player1.Fire();
                    var opponentFieldState = Player2.ProcessOwnFieldAndShipHit(firePosition);
                    Player1.ChangeOpponentFieldState(firePosition, opponentFieldState);
                    
                    if(opponentFieldState == FieldState.Sunk)
                    {
                        var sunkPositions = Player2.GetAllSunkShipPositionsFromFirstSunkField(firePosition);
                        Player1.MarkOpponentSunkFieldsFromPositions(sunkPositions);
                    }

                    if (opponentFieldState == FieldState.Miss)
                    {
                        break;
                    }
                }
                _roundPlayer = 1;
            }
            else
            {
                ++TotalRounds;
                while (true)
                {
                    var firePosition = Player2.Fire();
                    var opponentFieldState = Player1.ProcessOwnFieldAndShipHit(firePosition);
                    Player2.ChangeOpponentFieldState(firePosition, opponentFieldState);
                    
                    if(opponentFieldState == FieldState.Sunk)
                    {
                        var sunkPositions = Player1.GetAllSunkShipPositionsFromFirstSunkField(firePosition);
                        Player2.MarkOpponentSunkFieldsFromPositions(sunkPositions);
                    }

                    if (opponentFieldState == FieldState.Miss)
                    {
                        break;
                    }
                }
                _roundPlayer = 0;
                
            }
        }

        Winner = Player1.IsLost ? "Player 2" : "Player 1";
    }

    private void SetBeginner()
    {
        var random = new Random();
        _roundPlayer = random.Next(Int16.MaxValue) % 2;
        Beginner = _roundPlayer == 0 ? "Player 1" : "Player 2";
    }

}
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
        InitGame();

        while (!Player1.IsLost && !Player2.IsLost)
        {
            if (_roundPlayer == 0)
            {
                PlayRound(Player1, Player2);
                _roundPlayer = 1;
            }
            else
            {
                PlayRound(Player2, Player1);
                _roundPlayer = 0;
            }
        }
        
        Winner = Player1.IsLost ? "Player 2" : "Player 1";
    }

    private void SetBeginner()
    {
        var random = new Random();
        _roundPlayer = random.Next(0, 2);
        Beginner = _roundPlayer == 0 ? "Player 1" : "Player 2";
    }

    private void InitGame()
    {
        Player1.PlaceShipsRandomly();
        Player2.PlaceShipsRandomly();
        SetBeginner();
    }

    private void PlayRound(Player currentPlayer, Player opponent)
    {
        ++TotalRounds;
        while (true)
        {
            var firePosition = currentPlayer.Fire();
            var opponentFieldState = opponent.ProcessOwnFieldAndShipHit(firePosition);
            currentPlayer.ChangeOpponentFieldState(firePosition, opponentFieldState);
                    
            if(opponentFieldState == FieldState.Sunk)
            {
                var sunkPositions = opponent.GetAllSunkShipPositionsFromFirstSunkField(firePosition);
                currentPlayer.MarkOpponentSunkFieldsFromPositions(sunkPositions);
            }

            if (opponentFieldState == FieldState.Miss)
            {
                break;
            }
            
            if (opponent.IsLost)
            {
                return;
            }
        }
    }

}
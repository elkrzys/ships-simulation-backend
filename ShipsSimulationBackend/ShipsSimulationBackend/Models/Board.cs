namespace ShipsSimulationBackend.Models;

public class Board
{
    public List<Field> Fields { get; }
    
    public Board()
    {
        Fields = new List<Field>();
        InitBoard();
    }
    private void InitBoard()
    {
        foreach (var row in Enumerable.Range(1, 10))
        {
            foreach (var column in Enumerable.Range(1, 10))
            {
                Fields.Add(new Field
                {
                    Position = new Position(row, column) 
                });
            }
        }
    }
}
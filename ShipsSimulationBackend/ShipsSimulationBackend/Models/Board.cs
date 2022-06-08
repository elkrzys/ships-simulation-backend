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

    public bool AreFieldsAvailable(Position begin, Position end)
    {
        if (end.Row > 10 || end.Column > 10)
        {
            return false;
        }
        
        return GetFieldsBetweenPositions(begin, end).All(ff => ff.OccupyingShip == null);
    }

    public IEnumerable<Field> GetFieldsBetweenPositions(Position begin, Position end)
    {
        return Fields
            .Where(f =>
                f.Position.Row >= begin.Row && f.Position.Column >= begin.Column &&
                f.Position.Row <= end.Row && f.Position.Column <= end.Column);
    }

    public IEnumerable<Field> GetAvailableFields()
    {
        return Fields.FindAll(field =>
            field.State != FieldState.Hit &&
            field.State != FieldState.Miss &&
            field.State != FieldState.Sunk);
    }
    public void MarkFieldsAsSunk(Ship ship)
    {
        Fields.FindAll(f => f.OccupyingShip.Type == ship.Type).ForEach(f => f.State = FieldState.Sunk);
    }
}
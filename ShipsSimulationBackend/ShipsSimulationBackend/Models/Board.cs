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

    public List<Field> GetAvailableNeighboursFieldsForPosition(Position position)
    {
        return GetAvailableFields()
            .Where(field => GetPositionNeighbours(position)
                .Any(pos => pos.Row == field.Position.Row && pos.Column == field.Position.Column))
            .ToList();
    }
    
    private IEnumerable<Position> GetPositionNeighbours(Position position)
    {
        var neighbours = new List<Position>();
        if (position.Row > 1)
        {
            neighbours.Add(new Position(position.Row - 1, position.Column));
        }

        if (position.Row < 10)
        {
            neighbours.Add(new Position(position.Row + 1, position.Column));
        }

        if (position.Column > 1)
        {
            neighbours.Add(new Position(position.Row, position.Column - 1));
        }

        if (position.Column < 10)
        {
            neighbours.Add(new Position(position.Row, position.Column + 1));
        }

        return neighbours;
    }

    public List<Position> GetRemainingHitPositions(Position lastHit)
    {
        return Fields
            .Where(field => field.State == FieldState.Hit)
            .Select(field => field.Position)
            .Where(pos => pos.Row != lastHit.Row && pos.Column != lastHit.Column)
            .ToList();
    }

    public IEnumerable<Field> GetAvailableFields()
    {
        return Fields.FindAll(field =>
            field.State != FieldState.Hit &&
            field.State != FieldState.Miss &&
            field.State != FieldState.Sunk);
    }
    public void MarkOccupiedFieldsAsSunk(Ship ship)
    {
        GetFieldsOccupiedByShip(ship).ForEach(f => f.State = FieldState.Sunk);
    }
    
    public void MarkFieldsAsSunkByPositions(List<Position> positions)
    {
        Fields.FindAll(field => positions
                .Any(position => field.Position.Row == position.Row && field.Position.Column == position.Column))
            .ForEach(field => field.State = FieldState.Sunk);
    }

    public List<Field> GetFieldsOccupiedByShip(Ship ship)
    {
        return Fields.FindAll(f => f.OccupyingShip != null && f.OccupyingShip.Type == ship.Type);
    }
}
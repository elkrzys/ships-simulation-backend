namespace ShipsSimulationBackend.Models;

public class Board
{
    private readonly List<Field> _fields;
    
    public Board()
    {
        _fields = new List<Field>();
        InitBoard();
    }
    
    public bool AreFieldsAvailable(Position begin, Position end)
    {
        if (end.Row > 10 || end.Column > 10)
        {
            return false;
        }
        
        return GetFieldsBetweenPositions(begin, end).All(field => field.OccupyingShip == null);
    }

    public List<Field> GetFieldsBetweenPositions(Position begin, Position end)
    {
        return _fields
            .Where(f =>
                f.Position.Row >= begin.Row && f.Position.Column >= begin.Column &&
                f.Position.Row <= end.Row && f.Position.Column <= end.Column).ToList();
    }

    public IEnumerable<Field> GetAvailableNeighboursFieldsForPosition(Position position)
    {
        return GetAvailableFields()
            .Where(field => GetPositionNeighbours(position).Any(pos => pos == field.Position));
    }

    public List<Position> GetRemainingHitPositions()
    {
        return _fields
            .Where(field => field.State == FieldState.Hit)
            .Select(field => field.Position)
            .ToList();
    }

    public List<Field> GetAvailableFields()
    {
        return _fields.FindAll(field => field.State is FieldState.Empty or FieldState.Occupied).ToList();
    }
    
    public void MarkOccupiedFieldsAsSunk(Ship ship)
    {
        GetFieldsOccupiedByShip(ship).ForEach(field => field.State = FieldState.Sunk);
    }
    
    public void MarkFieldsAsSunkByPositions(IEnumerable<Position> positions)
    {
        _fields.FindAll(field => positions.Any(position => field.Position == position))
            .ForEach(field => field.State = FieldState.Sunk);
    }

    public List<Field> GetFieldsOccupiedByShip(Ship ship)
    {
        return _fields.FindAll(field => field.OccupyingShip != null && field.OccupyingShip.Type == ship.Type);
    }

    public Field GetFieldOnPosition(Position position)
    {
        return _fields.Find(field => field.Position == position)!;
    }
    
    private void InitBoard()
    {
        foreach (var row in Enumerable.Range(1, 10))
        {
            foreach (var column in Enumerable.Range(1, 10))
            {
                _fields.Add(new Field
                {
                    Position = new Position(row, column) 
                });
            }
        }
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
}
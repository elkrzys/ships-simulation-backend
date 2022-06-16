namespace ShipsSimulationBackend.Models;

public class Board
{
    public List<Field> Fields { get; }
    
    public Board()
    {
        Fields = new List<Field>();
        InitBoard();
    }
    
    public bool AreFieldsAvailable(Position begin, Position end)
    {
        if (end.Row > 10 || end.Column > 10)
        {
            return false;
        }
        
        return !AreAnyShipsNearby(begin, end);
    }

    private bool AreAnyShipsNearby(Position begin, Position end)
    {
       var newBegin = new Position(begin);
       var newEnd = new Position(end);
       
       // check rectangle around the ship
       if (newBegin.Row > 1) --newBegin.Row;
       if (newBegin.Column > 1) --newBegin.Column;
       if (newEnd.Row < 10) ++newEnd.Row;
       if (newEnd.Column < 10) ++newEnd.Column;

       for (var row = newBegin.Row; row <= newEnd.Row; row++)
       {
           for (var column = newBegin.Column; column <= newEnd.Column; column++)
           {
               if (Fields.Find(field => field.Position == new Position(row, column))!.State == FieldState.Occupied)
               {
                   return true;
               }
           }
       }

       return false;
    }

    public List<Field> GetFieldsBetweenPositions(Position begin, Position end)
    {
        return Fields
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
        return Fields
            .Where(field => field.State == FieldState.Hit)
            .Select(field => field.Position)
            .ToList();
    }

    public List<Field> GetAvailableFields()
    {
        return Fields.FindAll(field => field.State is FieldState.Empty or FieldState.Occupied).ToList();
    }
    
    public void MarkOccupiedFieldsAsSunk(Ship ship)
    {
        GetFieldsOccupiedByShip(ship).ForEach(field => field.State = FieldState.Sunk);
    }
    
    public void MarkFieldsAsSunkByPositions(IEnumerable<Position> positions)
    {
        Fields.FindAll(field => positions.Any(position => field.Position == position))
            .ForEach(field => field.State = FieldState.Sunk);
    }

    public List<Field> GetFieldsOccupiedByShip(Ship ship)
    {
        return Fields.FindAll(field => field.OccupyingShip != null && field.OccupyingShip.Type == ship.Type);
    }

    public Field GetFieldOnPosition(Position position)
    {
        return Fields.Find(field => field.Position == position)!;
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
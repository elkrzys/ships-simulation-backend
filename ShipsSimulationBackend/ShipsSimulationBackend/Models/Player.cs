namespace ShipsSimulationBackend.Models;

public class Player
{
    public Board OwnBoard { get; }
    public Board OpponentBoard { get; }
    public List<Ship> Ships { get; }
    public int TotalShots { get; set; }
    public int TotalHits { get; set; }
    
    private Position _lastHit;
    
    public Player()
    {
        OwnBoard = new Board();
        OpponentBoard = new Board();
        
        _lastHit = null;

        TotalShots = 0;
        TotalHits = 0;

        Ships = new List<Ship>
        {
            new(ShipType.Carrier),
            new(ShipType.Battleship),
            new(ShipType.Submarine),
            new(ShipType.Destroyer),
            new(ShipType.PatrolBoat)
        };
    }

    public bool IsLost => Ships.All(ship => ship.IsSunk);

    public void PlaceShipsRandomly()
    {
        Ships.ForEach(ship =>
        {
            while (true)
            {
                if (PlaceShipRandomlyOrReturnFalse(ship)) break;
            }
        });
    }

    private Field GetRandomFieldToShot()
    {
        var availableFields = OpponentBoard.GetAvailableFields().ToList();
        var random = new Random();
        return availableFields.ElementAt(random.Next(availableFields.Count));
    }

    private Field GetNeighbourFieldToShot(Position hit)
    {
        var availableFields = OpponentBoard.GetAvailableNeighboursFieldsForPosition(hit);

        if (!availableFields.Any())
        {
            OpponentBoard.GetRemainingHitPositions(hit).ForEach(hitPosition => 
                availableFields.AddRange(OpponentBoard.GetAvailableNeighboursFieldsForPosition(hitPosition)));
        }
        
        var random = new Random();
        var fieldsCount = availableFields.Count;
        var index = random.Next(fieldsCount);
        
        return availableFields[index];
    }

    public Position Fire()
    {
        ++TotalShots;
        var targetField = _lastHit == null ? GetRandomFieldToShot() : GetNeighbourFieldToShot(_lastHit);
        return targetField.Position;
    }

    public void ChangeOpponentFieldState(Position position, FieldState newState)
    {
        _lastHit = newState == FieldState.Hit ? position : null!;

        var field = OpponentBoard.Fields.Find(field =>
            field.Position.Row == position.Row && field.Position.Column == position.Column);
        field!.State = newState;
    }
    
    public FieldState ProcessOwnFieldAndShipHit(Position position)
    {
        ++TotalHits;
        var field = OwnBoard.Fields.Find(field =>
            field.Position.Row == position.Row && field.Position.Column == position.Column)!;

        switch (field.State)
        {
            case FieldState.Empty:
                field.State = FieldState.Miss;
                break;
            case FieldState.Occupied:
                field.State = FieldState.Hit;
                var ship = field.OccupyingShip!;
                ++ship.Hits;
                if (ship.IsSunk)
                {
                    OwnBoard.MarkOccupiedFieldsAsSunk(ship);
                }
                break;
        }
        return field.State;
    }

    public List<Position> GetAllSunkShipPositionsFromFirstSunkField(Position position)
    {
        var sunkField = OwnBoard.Fields.Find(field =>
            field.Position.Row == position.Row && field.Position.Column == position.Column);
        return OwnBoard.GetFieldsOccupiedByShip(sunkField.OccupyingShip)
            .Select(field => field.Position)
            .ToList();
    }

    public void MarkOpponentSunkFieldsFromPositions(List<Position> positions)
    {
        OpponentBoard.MarkFieldsAsSunkByPositions(positions);
    }

    private bool PlaceShipRandomlyOrReturnFalse(Ship ship)
    {
        var random = new Random();

        // even number represent horizontal, odd represents vertical direction
        var direction = random.Next(1, 11) % 2;
        var beginPosition = new Position(random.Next(1, 11), random.Next(1, 11));
        var endPosition = new Position(0, 0);

        if (direction == 0)
        {
            endPosition.Row = beginPosition.Row;
            endPosition.Column = beginPosition.Column + ship.Size - 1;
        }
        else
        {
            endPosition.Row = beginPosition.Row + ship.Size - 1;
            endPosition.Column = beginPosition.Column;
        }

        if (!OwnBoard.AreFieldsAvailable(beginPosition, endPosition))
        {
            return false;
        }

        OwnBoard.GetFieldsBetweenPositions(beginPosition, endPosition)
            .ToList()
            .ForEach(field =>
            {
                field.State = FieldState.Occupied;
                field.OccupyingShip = ship;
            });

        return true;
    }
}
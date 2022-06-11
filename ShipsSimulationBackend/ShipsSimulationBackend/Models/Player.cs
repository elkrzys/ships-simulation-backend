namespace ShipsSimulationBackend.Models;

public class Player
{
    public Board OwnBoard { get; }
    
    public Board OpponentBoard { get; }
    
    public int TotalShots { get; set; }
    
    private readonly List<Ship> _ships;
    
    private readonly Random _random;

    private Position? _lastHit;

    public Player()
    {
        OwnBoard = new Board();
        OpponentBoard = new Board();

        _lastHit = null;

        TotalShots = 0;

        _ships = new List<Ship>
        {
            new(ShipType.Carrier),
            new(ShipType.Battleship),
            new(ShipType.Submarine),
            new(ShipType.Destroyer),
            new(ShipType.PatrolBoat)
        };

        _random = new Random();
    }

    public bool IsLost => _ships.All(ship => ship.IsSunk);

    public void PlaceShipsRandomly()
    {
        _ships.ForEach(ship =>
        {
            while (true)
            {
                if (PlaceShipRandomlyAndReturnStatus(ship))
                {
                    break;
                }
            }
        });
    }

    public Position Fire()
    {
        ++TotalShots;
        var targetField = _lastHit is null ? GetRandomFieldToShot() : GetNeighbourFieldToShot();
        return targetField.Position;
    }

    public void ChangeOpponentFieldState(Position position, FieldState newState)
    {
        _lastHit = newState == FieldState.Hit ? position : null;
        var field = OpponentBoard.GetFieldOnPosition(position);
        field.State = newState;
    }

    public FieldState ProcessOwnFieldAndShipHit(Position position)
    {
        var field = OwnBoard.GetFieldOnPosition(position);

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

    public IEnumerable<Position> GetAllSunkShipPositionsFromFirstSunkField(Position position)
    {
        var sunkField = OwnBoard.GetFieldOnPosition(position);
        return OwnBoard.GetFieldsOccupiedByShip(sunkField.OccupyingShip).Select(field => field.Position);
    }

    public void MarkOpponentSunkFieldsFromPositions(IEnumerable<Position> positions)
    {
        OpponentBoard.MarkFieldsAsSunkByPositions(positions);
    }

    private bool PlaceShipRandomlyAndReturnStatus(Ship ship)
    {
        var beginPosition = new Position(_random.Next(1, 11), _random.Next(1, 11));
        var endPosition = SetShipEndPosition(beginPosition, ship.Size);

        if (!OwnBoard.AreFieldsAvailable(beginPosition, endPosition))
        {
            return false;
        }

        OwnBoard.GetFieldsBetweenPositions(beginPosition, endPosition)
            .ForEach(field =>
            {
                field.State = FieldState.Occupied;
                field.OccupyingShip = ship;
            });

        return true;
    }

    private Position SetShipEndPosition(Position beginPosition, int shipSize)
    {
        // even number represent horizontal, odd represents vertical direction
        var direction = _random.Next(1, 11) % 2;
        var endPosition = new Position(0, 0);
        if (direction == 0)
        {
            endPosition.Row = beginPosition.Row;
            endPosition.Column = beginPosition.Column + shipSize - 1;
        }
        else
        {
            endPosition.Row = beginPosition.Row + shipSize - 1;
            endPosition.Column = beginPosition.Column;
        }

        return endPosition;
    }

    private Field GetRandomFieldToShot()
    {
        var availableFields = OpponentBoard.GetAvailableFields();
        return availableFields[_random.Next(availableFields.Count)];
    }

    private Field GetNeighbourFieldToShot()
    {
        var availableFields = new List<Field>();
        OpponentBoard.GetRemainingHitPositions().ForEach(hitPosition =>
            availableFields.AddRange(OpponentBoard.GetAvailableNeighboursFieldsForPosition(hitPosition)));

        return availableFields[_random.Next(availableFields.Count)];
    }
}
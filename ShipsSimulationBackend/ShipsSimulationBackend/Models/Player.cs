namespace ShipsSimulationBackend.Models;

public class Player
{
    public Board OwnBoard { get; }
    public Board OpponentBoard { get; }
    public List<Ship> Ships { get; }
    public int TotalShots { get; set; }
    public int TotalHits { get; set; }
    public int MaxSeries { get; set; }
    
    public Player()
    {
        OwnBoard = new Board();
        OpponentBoard = new Board();

        TotalShots = 0;
        TotalHits = 0;
        MaxSeries = 0;

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
            while (!PlaceShipRandomlyOrReturnFalse(ship))
            {
                PlaceShipRandomlyOrReturnFalse(ship);
            }
        });

        string board = "";

        for (int i = 1; i < 11; i++)
        {
            for (int j = 1; j < 11; j++)
            {
                var state = OwnBoard.Fields.Find(field => field.Position.Row == i && field.Position.Column == j).State;
                if (state == FieldState.Empty)
                {
                    board += "o ";
                }

                if (state == FieldState.Occupied)
                {
                    board += "X ";
                }
            }

            board += "\n";
        }
    }

    private Field GetRandomFieldToShot()
    {
        var availableFields = OpponentBoard.GetAvailableFields().ToList();
        var random = new Random();
        return availableFields.ElementAt(random.Next(availableFields.Count));
    }

    private Field GetNeighbourFieldToShot(Position hit)
    {
        var neighboursPositions = new List<Position>();
        if (hit.Row > 1)
        {
            neighboursPositions.Add(new Position(hit.Row - 1, hit.Column));
        }

        if (hit.Row < 10)
        {
            neighboursPositions.Add(new Position(hit.Row + 1, hit.Column));
        }

        if (hit.Column > 1)
        {
            neighboursPositions.Add(new Position(hit.Row, hit.Column - 1));
        }

        if (hit.Column < 10)
        {
            neighboursPositions.Add(new Position(hit.Row, hit.Column + 1));
        }

        var random = new Random();
        var availableFields = OpponentBoard.GetAvailableFields()
            .Where(field =>
                neighboursPositions.Any(pos => pos.Row == field.Position.Row && pos.Column == field.Position.Column))
            .ToList();

        return availableFields[random.Next(availableFields.Count)];
    }

    public FireResult Fire(Position? lastHit = null)
    {
        ++TotalShots;

        var targetField = lastHit == null ? GetRandomFieldToShot() : GetNeighbourFieldToShot(lastHit);
        if (targetField.State == FieldState.Empty)
        {
            targetField.State = FieldState.Miss;
            return new FireResult
            {
                Position = targetField.Position,
                State = FieldState.Miss
            };
        }

        ProcessFieldAndShipHit(targetField);
        return new FireResult
        {
            Position = targetField.Position,
            State = targetField.State
        };
    }

    private void ProcessFieldAndShipHit(Field field)
    {
        ++TotalHits;
        field.State = FieldState.Hit;
        ProcessShipHit(field.OccupyingShip);
    }

    private void ProcessShipHit(Ship ship)
    {
        ++ship.Hits;
        if (ship.IsSunk)
        {
            OpponentBoard.MarkFieldsAsSunk(ship);
        }
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
namespace ShipsSimulationBackend.Models;

public class Ship
{
    public int Size => Type switch
    {
        ShipType.Carrier => 5,
        ShipType.Battleship => 4,
        ShipType.Destroyer => 3,
        ShipType.Submarine => 3,
        ShipType.PatrolBoat => 2,
        _ => throw new ArgumentOutOfRangeException()
    };

    public ShipType Type { get; }
    public int Hits { get; set; }

    public bool IsSunk => Size <= Hits;

    public Ship(ShipType type)
    {
        Type = type;
        Hits = 0;
    }

}
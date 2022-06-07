namespace ShipsSimulationBackend.Models;

public class Field
{
    public Position Position { get; set; }
    public FieldState State { get; set; } = FieldState.Empty;
    
    public Ship? OccupyingShip { get; set; }
}
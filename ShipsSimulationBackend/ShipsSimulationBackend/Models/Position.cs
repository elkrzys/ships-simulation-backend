namespace ShipsSimulationBackend.Models;

public record Position(int Row, int Column)
{
    public int Row { get; set; } = Row;
    
    public int Column { get; set; } = Column;

    public Position(Position position)
    {
        Row = position.Row;
        Column = position.Column;
    }
}
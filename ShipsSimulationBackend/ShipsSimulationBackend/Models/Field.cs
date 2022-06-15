using System.Text.Json.Serialization;

namespace ShipsSimulationBackend.Models;

public class Field
{
    public Position Position { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public FieldState State { get; set; } = FieldState.Empty;

    [JsonIgnore]
    public Ship? OccupyingShip { get; set; }
}
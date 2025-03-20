
using System.Text.Json.Serialization;

public class Epreuve
{
    public required string Id { get; set; }
    public required string Name { get; set; }

    [JsonPropertyName("photos")]
    public required List<Photo> Photos { get; set; } = new List<Photo>();
}
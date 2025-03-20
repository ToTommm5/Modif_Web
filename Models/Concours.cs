public class Concours
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Img_url { get; set; }
    public required List<Epreuve> Epreuves { get; set; } = new List<Epreuve>();


}
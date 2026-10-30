namespace AzubiGehaltsCheck.Entities.Leistung;

public class Leistung
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Beschreibung {get; set; }
    public required string AntragsLink { get; set; }
    /// Rechtsquelle bzw. offizielle Seite, aus der die Regeln stammen.</summary>
    public required string Quelle { get; set; }
    public DateOnly GueltigAb { get; set; }
    public DateOnly? GueltigBis { get; set; }
    public bool Aktiv { get; set; } = true;

     public List<Regel> Regeln { get; set; } = [];
}
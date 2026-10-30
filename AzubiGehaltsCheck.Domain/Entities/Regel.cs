using AzubiGehaltsCheck.Entities.Leistung;

namespace AzubiGehaltsCheck.Domain.Entities;

/// Eine Bedingung einer Leistung, z. B. "Alter KleinerAls 25".
/// Eine Leistung passt nur, wenn ALLE ihre Regeln erfüllt sind.
public class Regel
{
    public int Id { get; set; }
    public int LeistungId { get; set; }
    public Leistung? Leistung { get; set; }

    public RegelFeld Feld { get; set; }
    public RegelOperator Operator { get; set; }
    ///Vergleichswert als Text, z. B. "25", "true" oder "betrieblich".
    public required string Wert { get; set; }
}

/// Felder aus dem Fragebogen, auf die sich eine Regel beziehen kann
public enum RegelFeld
{
    Alter,
    Ausbildungsart,
    Bruttoverguetung,
    WohntBeiEltern,
    ArbeitswegKm
}

public enum RegelOperator
{
    Gleich,
    Ungleich,
    KleinerAls,
    KleinerGleich,
    GroesserAls,
    GroesserGleich
}
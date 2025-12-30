namespace UniversiteDomain.Dtos;

public class NoteCsvDto
{
    public string NumEtud { get; set; } = string.Empty;

    public string Nom { get; set; } = string.Empty;

    public string Prenom { get; set; } = string.Empty;

    public string NumeroUe { get; set; } = string.Empty;

    public string IntituleUe { get; set; } = string.Empty;

    public string Note { get; set; } = string.Empty;

    public float? GetNoteValue()
    {
        if (string.IsNullOrWhiteSpace(Note))
            return null;

        if (float.TryParse(Note, System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture, out float result))
            return result;

        return null;
    }
}

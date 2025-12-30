namespace UniversiteDomain.Exceptions.NoteExceptions;

public class CsvImportException : Exception
{
    public List<CsvImportError> Errors { get; }

    public CsvImportException(string message, List<CsvImportError> errors) : base(message)
    {
        Errors = errors ?? new List<CsvImportError>();
    }

    public CsvImportException(List<CsvImportError> errors)
        : base($"{errors?.Count ?? 0} erreur(s) trouvée(s) dans le fichier CSV")
    {
        Errors = errors ?? new List<CsvImportError>();
    }
}

public class CsvImportError
{
    public int Ligne { get; set; }
    public string Erreur { get; set; } = string.Empty;

    public CsvImportError(int ligne, string erreur)
    {
        Ligne = ligne;
        Erreur = erreur;
    }
}

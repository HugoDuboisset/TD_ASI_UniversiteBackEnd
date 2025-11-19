namespace UniversiteDomain.Exceptions.EtudiantExceptions;

public class EtudiantNotFoundException : Exception
{
    public EtudiantNotFoundException()
        : base("Étudiant introuvable.")
    { }

    public EtudiantNotFoundException(string message)
        : base(message)
    { }

    public EtudiantNotFoundException(long id)
        : base($"Étudiant introuvable (Id = {id}).")
    { }

    public EtudiantNotFoundException(string message, Exception inner)
        : base(message, inner)
    { }
}
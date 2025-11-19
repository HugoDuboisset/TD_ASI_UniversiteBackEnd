namespace UniversiteDomain.Exceptions.ParcoursExceptions;

public class ParcoursNotFoundException : Exception
{
    public ParcoursNotFoundException()
        : base("Parcours introuvable.")
    { }

    public ParcoursNotFoundException(string message)
        : base(message)
    { }

    public ParcoursNotFoundException(long id)
        : base($"Parcours introuvable (Id = {id}).")
    { }

    public ParcoursNotFoundException(string message, Exception inner)
        : base(message, inner)
    { }
}
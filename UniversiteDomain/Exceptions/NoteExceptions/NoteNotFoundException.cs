namespace UniversiteDomain.Exceptions.NoteExceptions;

public class NoteNotFoundException : Exception
{
    public NoteNotFoundException()
        : base("Note introuvable.")
    { }

    public NoteNotFoundException(string message)
        : base(message)
    { }

    public NoteNotFoundException(long etudiantId, long ueId)
        : base($"Aucune note trouvée pour l'étudiant {etudiantId} dans l'UE {ueId}.")
    { }

    public NoteNotFoundException(string message, Exception inner)
        : base(message, inner)
    { }
}


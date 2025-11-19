namespace UniversiteDomain.Exceptions.NoteExceptions;

public class UeNotInParcoursException : Exception
{
    public UeNotInParcoursException()
        : base("L'UE ne fait pas partie du parcours de l'étudiant.")
    { }

    public UeNotInParcoursException(string message)
        : base(message)
    { }

    public UeNotInParcoursException(long etudiantId, long ueId)
        : base($"L'UE {ueId} ne fait pas partie du parcours de l'étudiant {etudiantId}.")
    { }

    public UeNotInParcoursException(string message, Exception inner)
        : base(message, inner)
    { }
}


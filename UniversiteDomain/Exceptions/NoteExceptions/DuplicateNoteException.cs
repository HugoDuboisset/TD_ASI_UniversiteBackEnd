namespace UniversiteDomain.Exceptions.NoteExceptions;

public class DuplicateNoteException : Exception
{
    public DuplicateNoteException()
        : base("Une note existe déjà pour cet étudiant dans cette UE.")
    { }

    public DuplicateNoteException(string message)
        : base(message)
    { }

    public DuplicateNoteException(long etudiantId, long ueId)
        : base($"Une note existe déjà pour l'étudiant {etudiantId} dans l'UE {ueId}.")
    { }

    public DuplicateNoteException(string message, Exception inner)
        : base(message, inner)
    { }
}


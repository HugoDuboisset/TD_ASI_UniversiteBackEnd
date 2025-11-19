namespace UniversiteDomain.Exceptions.NoteExceptions;

public class InvalidNoteValeurException : Exception
{
    public InvalidNoteValeurException()
        : base("La valeur de la note doit être comprise entre 0 et 20.")
    { }

    public InvalidNoteValeurException(string message)
        : base(message)
    { }

    public InvalidNoteValeurException(float valeur)
        : base($"La valeur de la note doit être comprise entre 0 et 20. Valeur reçue : {valeur}")
    { }

    public InvalidNoteValeurException(string message, Exception inner)
        : base(message, inner)
    { }
}


namespace UniversiteDomain.Exceptions.ParcoursExceptions;

public class DuplicateInscriptionException : Exception
{
    public DuplicateInscriptionException()
        : base("Inscription en double détectée.")
    { }

    public DuplicateInscriptionException(string message)
        : base(message)
    { }

    public DuplicateInscriptionException(long idEtudiant, long idParcours)
        : base($"L'étudiant {idEtudiant} est déjà inscrit dans le parcours {idParcours}.")
    { }

    public DuplicateInscriptionException(string message, Exception inner)
        : base(message, inner)
    { }
}
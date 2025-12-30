namespace UniversiteDomain.Entities;

public class Note
{
    public float Valeur { get; set; }
    public long EtudiantId { get; set; }
    public long UeId { get; set; }

    public Etudiant Etudiant { get; set; } = null!;
    public Ue Ue { get; set; } = null!;
}
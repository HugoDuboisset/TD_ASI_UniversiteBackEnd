namespace UniversiteDomain.Entities;

public class Ue
{
    public long Id { get; set; }
    public string NumeroUe { get; set; }
    public string Intitule { get; set; }
    
    public List<Parcours>? EnseigneeDans { get; set; } = new();
    
    //one to many : une UE peut avoir plusieurs notes
    public List<Note> Notes { get; set; } = new();
    
    public override string ToString()
    {
        return "ID "+Id +" : "+NumeroUe+" - "+Intitule;
    }
    
}


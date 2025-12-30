using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class ParcoursRepository(UniversiteDbContext context) : Repository<Parcours>(context), IParcoursRepository
{
    public async Task<int> GetNombreEtudiantsAsync(long idParcours)
    {
        ArgumentNullException.ThrowIfNull(Context.Etudiants);
        return await Task.FromResult(Context.Etudiants.Count(e => e.ParcoursSuivi != null && e.ParcoursSuivi.Id == idParcours));
    }
    
    public async Task<int> GetNombreEtudiantsAsync(Parcours parcours)
    {
        return await GetNombreEtudiantsAsync(parcours.Id);
    }

    public async Task<Parcours> AddEtudiantAsync(long idParcours, long idEtudiant)
    {
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        ArgumentNullException.ThrowIfNull(Context.Etudiants);
        
        Parcours parcours = (await Context.Parcours.FindAsync(idParcours))!;
        Etudiant etudiant = (await Context.Etudiants.FindAsync(idEtudiant))!;
        
        if (parcours.Inscrits == null)
        {
            parcours.Inscrits = new List<Etudiant>();
        }
        
        parcours.Inscrits.Add(etudiant);
        await Context.SaveChangesAsync();
        
        return parcours;
    }

    public async Task<Parcours> AddEtudiantAsync(Parcours parcours, Etudiant etudiant)
    {
        return await AddEtudiantAsync(parcours.Id, etudiant.Id);
    }

    public async Task<Parcours> AddEtudiantAsync(Parcours? parcours, List<Etudiant> etudiants)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        long[] idEtudiants = etudiants.Select(e => e.Id).ToArray();
        return await AddEtudiantAsync(parcours.Id, idEtudiants);
    }

    public async Task<Parcours> AddEtudiantAsync(long idParcours, long[] idEtudiants)
    {
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        ArgumentNullException.ThrowIfNull(Context.Etudiants);
        
        Parcours parcours = (await Context.Parcours.FindAsync(idParcours))!;
        
        if (parcours.Inscrits == null)
        {
            parcours.Inscrits = new List<Etudiant>();
        }
        
        foreach (var idEtudiant in idEtudiants)
        {
            Etudiant etudiant = (await Context.Etudiants.FindAsync(idEtudiant))!;
            parcours.Inscrits.Add(etudiant);
        }
        
        await Context.SaveChangesAsync();
        
        return parcours;
    }

    public async Task<Parcours> AddUeAsync(long idParcours, long idUe)
    {
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        ArgumentNullException.ThrowIfNull(Context.Ues);
        
        Parcours parcours = (await Context.Parcours.FindAsync(idParcours))!;
        Ue ue = (await Context.Ues.FindAsync(idUe))!;
        
        if (parcours.UesEnseignees == null)
        {
            parcours.UesEnseignees = new List<Ue>();
        }
        
        parcours.UesEnseignees.Add(ue);
        await Context.SaveChangesAsync();
        
        return parcours;
    }

    public async Task<Parcours> AddUeAsync(Parcours parcours, Ue ue)
    {
        return await AddUeAsync(parcours.Id, ue.Id);
    }

    public async Task<Parcours> AddUeAsync(Parcours? parcours, List<Ue> ues)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        long[] idUes = ues.Select(u => u.Id).ToArray();
        return await AddUeAsync(parcours.Id, idUes);
    }

    public async Task<Parcours> AddUeAsync(long idParcours, long[] idUes)
    {
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        ArgumentNullException.ThrowIfNull(Context.Ues);
        
        Parcours parcours = (await Context.Parcours.FindAsync(idParcours))!;
        
        if (parcours.UesEnseignees == null)
        {
            parcours.UesEnseignees = new List<Ue>();
        }
        
        foreach (var idUe in idUes)
        {
            Ue ue = (await Context.Ues.FindAsync(idUe))!;
            parcours.UesEnseignees.Add(ue);
        }
        
        await Context.SaveChangesAsync();
        
        return parcours;
    }
}
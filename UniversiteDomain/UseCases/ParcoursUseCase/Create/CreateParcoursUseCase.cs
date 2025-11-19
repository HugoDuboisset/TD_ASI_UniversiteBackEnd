using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;

namespace UniversiteDomain.UseCases.ParcoursUseCase.Create;

public class CreateParcoursUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Parcours> ExecuteAsync(long id, string nomParcours, int anneeFormation)
    {
        var p = new Parcours{Id = id, NomParcours = nomParcours, AnneeFormation = anneeFormation};
        return await ExecuteAsync(p);
    }

    public async Task<Parcours> ExecuteAsync(Parcours parcours)
    {
        await CheckBusinessRules(parcours);
        var repo = repositoryFactory.ParcoursRepository();
        Parcours p = await repo.CreateAsync(parcours);
        await repositoryFactory.SaveChangesAsync();
        return p;
    }
    private async Task CheckBusinessRules(Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(parcours.NomParcours);
        ArgumentNullException.ThrowIfNull(repositoryFactory);
        
        var repo = repositoryFactory.ParcoursRepository();
        ArgumentNullException.ThrowIfNull(repo);
        
        // On recherche un parcours avec le même code
        List<Parcours> existe = await repo.FindByConditionAsync(p=>p.NomParcours.Equals(parcours.NomParcours));

        // Si un parcours avec le même code existe déjà, on lève une exception personnalisée
        if (existe is {Count:>0}) throw new DuplicateNomParcoursException(parcours.NomParcours+ " - ce code de parcours est déjà affecté à un parcours");
        
        // Le métier définit que les nom doite contenir plus de 3 lettres
        if (parcours.NomParcours.Length < 3) throw new InvalidNomParcoursException(parcours.NomParcours +" incorrect - Le nom d'un parcours doit contenir plus de 3 caractères");
    } 
}
